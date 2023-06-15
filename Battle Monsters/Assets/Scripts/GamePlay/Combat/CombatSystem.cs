using BattleMonsters.Monster;
using BattleMonsters.Moves;
using BattleMonsters.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BattleMonsters.Monster.GenericMonster;

namespace BattleMonsters.GamePlay.Combat
{
    public class CombatSystem : MonoBehaviour
    {
        [SerializeField]
        private BattleUnit _playerUnit;
        [SerializeField]
        private HUD _playerHUD;
        [SerializeField]
        private BattleUnit _opponentUnit;
        [SerializeField]
        private HUD _opponentHUD;
        [SerializeField]
        private CombatDialog _dialogBox;

        [SerializeField]
        private GameObject _dialogText;
        [SerializeField]
        private GameObject _actionButtons;
        [SerializeField]
        private AttackButtonsManager _attackButtons;

        [SerializeField]
        private GenericMonster _playerMon;
        [SerializeField]
        private GenericMonster _wildMon;

        [SerializeField]
        private float _textDelayTime = 1f;
        [SerializeField]
        private float _thawChance = 0.2f;
        [SerializeField]
        private float _paralyzeOvercomeChance = 0.25f;
        [SerializeField]
        private float _confusionHurtChance = 0.33f; 

        private bool _isPlayerFainted = false;

        private Utils.Conditions.WeatherCondition _weatherCondition = Utils.Conditions.WeatherCondition.None;
        private int _weatherCountdown = 0;

        private PriorityQueue<BattleUnit> _priorityQueue = new PriorityQueue<BattleUnit>(false);

        /// <summary>
        /// TODO: Remove
        /// </summary>
        private void Start()
        {
            StartCoroutine(SetUpBattle());
        }

        private void StartBattle(GenericMonster playerMon, GenericMonster wildMon)
        {
            _playerMon = playerMon;
            _wildMon = wildMon;
            StartCoroutine(SetUpBattle());
        }

        private IEnumerator SetUpBattle()
        {
            //Set up HUDS
            _wildMon.Init();
            _playerMon.Init();
            _playerUnit.Setup(_playerMon);
            _playerHUD.SetHUDData(_playerUnit.Monster);
            _opponentUnit.Setup(_wildMon);
            _opponentHUD.SetHUDData(_opponentUnit.Monster);

            _dialogBox.SetDialog($"A wild {_opponentUnit.Monster.Base.Species} appeard!");
            yield return new WaitForSeconds(_textDelayTime);
            _dialogBox.SetDialog($"What Should {_playerUnit.Monster.Base.Species} do?");
            EnableActions(true);
        }

        public void CancelAttacks()
        {
            EnableAttacks(false);
            EnableDialog(true);
            EnableActions(true);
            _dialogBox.SetDialog($"What Should {_playerUnit.Monster.Base.Species} do?");
        }

        public void EnableDialog(bool enable)
        {
            _dialogText.SetActive(enable);
        }

        public void EnableActions(bool enable)
        {
            _actionButtons.SetActive(enable);
        }

        public void EnableAttacks(bool enable)
        {
            _attackButtons.gameObject.SetActive(enable);
            _attackButtons.SetButtons(_playerUnit.Monster.Moves);
        }

        public void PerformAttack(int attackButtonIndex)
        {
            StartCoroutine(ExecuteBattleRound(attackButtonIndex));
        }

        private IEnumerator ExecuteBattleRound(int attackButtonIndex)
        {
            _priorityQueue.Enqueue(_playerUnit.Monster.Speed, _playerUnit);
            _priorityQueue.Enqueue(_opponentUnit.Monster.Speed, _opponentUnit);

            while (_priorityQueue.Count > 0)
            {
                BattleUnit unit = _priorityQueue.Dequeue();
                if (unit.IsPlayer)
                {
                    yield return PerformPlayerAttack(attackButtonIndex);
                }
                else
                {
                    yield return PerformEnemyAttack();
                    if (_isPlayerFainted)
                    {
                        _priorityQueue.Clear();
                        break;
                    }
                }
            }
            _isPlayerFainted = false;

            yield return EndofRoundEffects();
        }

        private IEnumerator PerformPlayerAttack(int attackButtonIndex)
        {
            GenericMove attack = _playerUnit.Monster.Moves[attackButtonIndex];
            yield return PerformAttack(_playerUnit, _opponentUnit, attack);
        }

        private IEnumerator PerformEnemyAttack()
        {
            GenericMove attack = _opponentUnit.Monster.Moves[UnityEngine.Random.Range(0, _opponentUnit.Monster.Moves.Count)];
            yield return PerformAttack(_opponentUnit, _playerUnit, attack);
        }

        private IEnumerator PerformAttack(BattleUnit sourceUnit, BattleUnit targetUnit, GenericMove attack)
        {
            EnableAttacks(false);
            EnableDialog(true);

            if (!IsAbleToAttack(sourceUnit))
            {
                yield return new WaitForSeconds(_textDelayTime);
                yield break;
            }

            attack.Uses--;
            _dialogBox.SetDialog($"{sourceUnit.Monster.Base.Species} used {attack.Base.MoveID}");
            switch (attack.Base.MoveType)
            {
                case MoveType.Melee:
                    sourceUnit.Animator.SetTrigger("Melee");
                    break;
                case MoveType.Ranged:
                    sourceUnit.Animator.SetTrigger("Ranged");
                    break;
                case MoveType.SelfBuff:
                    sourceUnit.Animator.SetTrigger("Buff");
                    break;
            }

            yield return new WaitForSeconds(_textDelayTime);

            if (!CheckHit(attack, sourceUnit, targetUnit))
            {
                _dialogBox.SetDialog($"{sourceUnit.Monster.Base.Species} missed!");
                targetUnit.Animator.SetTrigger("Dodge");
                yield return new WaitForSeconds(_textDelayTime);
                yield break;
            }

            targetUnit.Animator.SetTrigger("Hit");
            OnHitResult damageDetails = OnHitResult.None;
            switch (attack.Base.Target)
            {
                case MoveTarget.Enemy:
                    damageDetails = targetUnit.Monster.ReceiveDamage(attack, sourceUnit.Monster);
                    break;
                case MoveTarget.Self:
                    damageDetails = targetUnit.Monster.ReceiveDamage(attack, sourceUnit.Monster);
                    break;
                default:
                    break;
            }

            _playerHUD.UpdateHealth();
            _opponentHUD.UpdateHealth();

            yield return new WaitForSeconds(_textDelayTime);

            if (damageDetails.HasFlag(OnHitResult.CriticalHit))
            {
                _dialogBox.SetDialog($"Critical Hit!");
                yield return new WaitForSeconds(_textDelayTime);
            }

            if (damageDetails.HasFlag(OnHitResult.SuperEffective))
            {
                _dialogBox.SetDialog($"It was super effective");
                yield return new WaitForSeconds(_textDelayTime);
            }
            if (damageDetails.HasFlag(OnHitResult.NotVeryEffective))
            {
                _dialogBox.SetDialog($"It was not very effective");
                yield return new WaitForSeconds(_textDelayTime);
            }
            if (damageDetails.HasFlag(OnHitResult.NoEffect))
            {
                _dialogBox.SetDialog($"It has no effect");
                yield return new WaitForSeconds(_textDelayTime);
            }

            if (damageDetails.HasFlag(OnHitResult.KO))
            {
                _dialogBox.SetDialog($"{targetUnit.Monster.Base.Species} was knocked out");
                targetUnit.Animator.SetTrigger("Defeat");
                yield return new WaitForSeconds(_textDelayTime);
                ExecuteTargetFainted(targetUnit);
                yield break;
            }

            yield return RunStatusAffects(targetUnit, attack.Base.Effects.TargetStatEffects);
            yield return RunStatusAffects(sourceUnit, attack.Base.Effects.UserStatEffects);

            if (attack.Base.Effects.TargetPermCondition != Conditions.PermanentCondition.None)
            {
                if (UnityEngine.Random.Range(0f, 1f) <= attack.Base.Effects.ChanceOfTargetPermCondition)
                {
                    targetUnit.Monster.ApplyStatusCondition(attack.Base.Effects.TargetPermCondition);
                    _dialogBox.SetDialog($"{targetUnit.Monster.Base.Species} is {attack.Base.Effects.TargetPermCondition}");
                    (targetUnit.IsPlayer ? _playerHUD : _opponentHUD).SetStatusCondition(Conditions.PermanentConditionColours[attack.Base.Effects.TargetPermCondition]);
                    yield return new WaitForSeconds(_textDelayTime);
                }
            }

            if (attack.Base.Effects.UserPermCondition != Conditions.PermanentCondition.None)
            {
                if (UnityEngine.Random.Range(0f, 1f) <= attack.Base.Effects.ChanceOfUserPermCondition)
                {
                    sourceUnit.Monster.ApplyStatusCondition(attack.Base.Effects.UserPermCondition);
                    _dialogBox.SetDialog($"{sourceUnit.Monster.Base.Species} is {attack.Base.Effects.UserPermCondition}");
                    (sourceUnit.IsPlayer ? _playerHUD : _opponentHUD).SetStatusCondition(Conditions.PermanentConditionColours[attack.Base.Effects.UserPermCondition]);
                    yield return new WaitForSeconds(_textDelayTime);
                }
            }

            if (attack.Base.Effects.TargetTempCondition != Conditions.TemporaryCondition.None)
            {
                if (UnityEngine.Random.Range(0f, 1f) <= attack.Base.Effects.ChanceOfTargetTempCondition)
                {
                    targetUnit.Monster.ApplyVolatileStatusCondition(attack.Base.Effects.TargetTempCondition);
                    _dialogBox.SetDialog($"{targetUnit.Monster.Base.Species} is {attack.Base.Effects.TargetTempCondition}");
                    yield return new WaitForSeconds(_textDelayTime);
                }
            }

            if (attack.Base.Effects.UserTempCondition != Conditions.TemporaryCondition.None)
            {
                if (UnityEngine.Random.Range(0f, 1f) <= attack.Base.Effects.ChanceOfUserTempCondition)
                {
                    sourceUnit.Monster.ApplyVolatileStatusCondition(attack.Base.Effects.UserTempCondition);
                    _dialogBox.SetDialog($"{sourceUnit.Monster.Base.Species} is {attack.Base.Effects.UserTempCondition}");
                    yield return new WaitForSeconds(_textDelayTime);
                }
            }

            if (attack.Base.Effects.WeatherCondition != Utils.Conditions.WeatherCondition.None && attack.Base.Effects.WeatherCondition != _weatherCondition)
            {
                if (UnityEngine.Random.Range(0f, 1f) <= attack.Base.Effects.ChanceOfWeatherCondition)
                {
                    _weatherCondition = attack.Base.Effects.WeatherCondition;
                    _weatherCountdown = 5;
                    _dialogBox.SetDialog($"It started {attack.Base.Effects.WeatherCondition}");
                    yield return new WaitForSeconds(_textDelayTime);
                }
            }
        }

        private bool IsAbleToAttack(BattleUnit sourceUnit)
        {
            bool isAbleToAttack = true;
            switch (sourceUnit.Monster.PermanentCondition)
            {
                case Utils.Conditions.PermanentCondition.Asleep:
                    sourceUnit.Monster.SleepCount--;
                    isAbleToAttack = CheckSleepStatus(sourceUnit);
                    break;
                case Utils.Conditions.PermanentCondition.Frozen:
                    isAbleToAttack = CheckFrozenStatus(sourceUnit);
                    break;
                case Utils.Conditions.PermanentCondition.Paralyzed:
                    isAbleToAttack = CheckParalyzeStatus(sourceUnit);
                    break;
                default:
                    break;
            }

            if (sourceUnit.Monster.TemporaryCondition.HasFlag(Conditions.TemporaryCondition.Confusion))
            {
                isAbleToAttack = CheckConfusion(sourceUnit);
                sourceUnit.Monster.ConfusionCount--;
            }

            return isAbleToAttack;
        }

        private bool CheckConfusion(BattleUnit sourceUnit)
        {
            if (sourceUnit.Monster.ConfusionCount-- <= 0)
            {
                _dialogBox.SetDialog($"{sourceUnit.Monster.Base.Species} snapped oput of confusion");
                sourceUnit.Monster.CureStatus(Utils.Conditions.PermanentCondition.None, Utils.Conditions.TemporaryCondition.Confusion);
                return true;
            }
            else
            {
                if (UnityEngine.Random.Range(0, 1) <= _confusionHurtChance)
                {
                    _dialogBox.SetDialog($"{sourceUnit.Monster.Base.Species} hurt itself in confusion");
                    return false;
                }
                return true;
            }
    
        }

        private bool CheckFrozenStatus(BattleUnit sourceUnit)
        {
            if (UnityEngine.Random.Range(0, 1) <= _thawChance)
            {
                _dialogBox.SetDialog($"{sourceUnit.Monster.Base.Species} thawed out");
                sourceUnit.Monster.CureStatus(Utils.Conditions.PermanentCondition.Frozen, Utils.Conditions.TemporaryCondition.None);
                (sourceUnit.IsPlayer ? _playerHUD : _opponentHUD).SetStatusCondition(Conditions.PermanentConditionColours[Conditions.PermanentCondition.None]);
                return true;
            }
            else
            {
                _dialogBox.SetDialog($"{sourceUnit.Monster.Base.Species} is still frozen");
                return false;
            }
        }

        private bool CheckParalyzeStatus(BattleUnit sourceUnit)
        {
            if (UnityEngine.Random.Range(0, 1) > _paralyzeOvercomeChance)
            {
                _dialogBox.SetDialog($"{sourceUnit.Monster.Base.Species} is unable to move");
                return false;
            }

            return true;
        }

        private bool CheckSleepStatus(BattleUnit sourceUnit)
        {
            if (sourceUnit.Monster.SleepCount-- <= 0)
            {
                _dialogBox.SetDialog($"{sourceUnit.Monster.Base.Species} woke up");
                sourceUnit.Monster.CureStatus(Utils.Conditions.PermanentCondition.Asleep, Utils.Conditions.TemporaryCondition.None);
                (sourceUnit.IsPlayer ? _playerHUD : _opponentHUD).SetStatusCondition(Conditions.PermanentConditionColours[Conditions.PermanentCondition.None]);
                return true;
            }
            else
            {
                _dialogBox.SetDialog($"{sourceUnit.Monster.Base.Species} is fast asleep");
                return false;
            }
        }

        private void ExecuteTargetFainted(BattleUnit targetUnit)
        {
            //exit battle
        }

        private bool CheckHit(GenericMove move, BattleUnit source, BattleUnit target)
        {
            if (move.Base.Accuracy < 0)
            {
                return true;
            }
            float accuracy = move.Base.Accuracy * (source.Monster.Accuracy / target.Monster.Evasion);
            return UnityEngine.Random.Range(0, 101) <= accuracy;
        }

        private IEnumerator EndofRoundEffects()
        {
            _priorityQueue.Enqueue(_playerUnit.Monster.Speed, _playerUnit);
            _priorityQueue.Enqueue(_opponentUnit.Monster.Speed, _opponentUnit);

            while (_priorityQueue.Count > 0)
            {
                BattleUnit unit = _priorityQueue.Dequeue();
                if (unit.IsPlayer)
                {
                    yield return ApplyStatusConditions(_playerUnit);
                }
                else
                {
                    yield return ApplyStatusConditions(_opponentUnit);
                }
            }
            yield return ApplyWeather();
            _dialogBox.SetDialog($"What Should {_playerUnit.Monster.Base.Species} do?");
            EnableActions(true);
            yield return new WaitForSeconds(1f);
        }

        private IEnumerator ApplyStatusConditions(BattleUnit unit)
        {
            bool isKO = false;
            switch (unit.Monster.PermanentCondition)
            {
                case Conditions.PermanentCondition.Poisoned:
                    isKO = unit.Monster.ReceiveConditionalDamage(Mathf.FloorToInt(unit.Monster.MaxHealth / 8f));
                    _dialogBox.SetDialog($"{unit.Monster.Base.Species} was hurt by the poison");
                    yield return new WaitForSeconds(_textDelayTime);
                    break;
                case Conditions.PermanentCondition.Burnt:
                    isKO = unit.Monster.ReceiveConditionalDamage(Mathf.FloorToInt(unit.Monster.MaxHealth / 16f));
                    _dialogBox.SetDialog($"{unit.Monster.Base.Species} was hurt by the burn");
                    yield return new WaitForSeconds(_textDelayTime);
                    break;
                default:
                    break;
            }
            (unit.IsPlayer ? _playerHUD : _opponentHUD).UpdateHealth();
            if (isKO)
            {
                ExecuteTargetFainted(unit);
            }
        }

        private IEnumerator ApplyWeather()
        {
            switch (_weatherCondition)
            {
                case Utils.Conditions.WeatherCondition.Hail:
                    break;
                case Utils.Conditions.WeatherCondition.SandStorm:
                    break;
                default:
                    break;
            }
            _weatherCountdown--;
            if (_weatherCountdown <= 0)
            {
                _weatherCondition = Utils.Conditions.WeatherCondition.None;
            }
            yield return null;
        }

        private IEnumerator RunStatusAffects(BattleUnit unit, List<StatEffect> stats)
        {
            foreach (var stat in stats)
            {
                if (stat.Modifier < 0)
                {
                    _dialogBox.SetDialog($"{unit.Monster.Base.Species}'s {stat.Stat} was lowered");
                    unit.Animator.SetTrigger("Debuff");
                    yield return new WaitForSeconds(_textDelayTime);
                }else if (stat.Modifier > 0)
                {
                    _dialogBox.SetDialog($"{unit.Monster.Base.Species}'s {stat.Stat} was raised");
                    unit.Animator.SetTrigger("Buff");
                    yield return new WaitForSeconds(_textDelayTime);
                }
            }
        }
    }
}