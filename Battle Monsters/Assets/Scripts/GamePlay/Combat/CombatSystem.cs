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
        private PartyScreen _partyScreen;

        [SerializeField]
        private GameObject _dialogText;
        [SerializeField]
        private GameObject _actionButtons;
        [SerializeField]
        private AttackButtonsManager _attackButtons;

        [SerializeField]
        private Party _playerParty;
        [SerializeField]
        private GenericMonster _wildMon;

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

        private void StartBattle(Party playerParty, GenericMonster wildMon)
        {
            _playerParty = playerParty;
            _wildMon = wildMon;
            StartCoroutine(SetUpBattle());
        }

        private IEnumerator SetUpBattle()
        {
            //Set up HUDS
            _wildMon.Init();
            _playerUnit.Setup(_playerParty.GetHealthyMon());
            _playerHUD.SetHUDData(_playerUnit.Monster);
            _opponentUnit.Setup(_wildMon);
            _opponentHUD.SetHUDData(_opponentUnit.Monster);

            //Set Players party
            _partyScreen.gameObject.SetActive(true);
            _partyScreen.Init();
            _partyScreen.gameObject.SetActive(false);

            _dialogBox.SetDialog($"A wild {_opponentUnit.Monster.Base.Species} appeard!");
            yield return new WaitForSeconds(1f);
            _dialogBox.SetDialog($"What Should {_playerUnit.Monster.Base.Species} do?");
            EnableActions(true);
        }

        public void OpenPartyScreen()
        {
            _partyScreen.SetPartyData(_playerParty.PartyList, _playerUnit.Monster);
            _partyScreen.gameObject.SetActive(true);
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
            _attackButtons.SetButtons(_playerUnit.Monster.KnownMoves);
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
            GenericMove attack = _playerUnit.Monster.KnownMoves[attackButtonIndex];
            yield return PerformAttack(_playerUnit, _opponentUnit, attack);
        }

        private IEnumerator PerformEnemyAttack()
        {
            GenericMove attack = _opponentUnit.Monster.KnownMoves[UnityEngine.Random.Range(0, _opponentUnit.Monster.KnownMoves.Count)];
            yield return PerformAttack(_opponentUnit, _playerUnit, attack);
        }

        private IEnumerator PerformAttack(BattleUnit sourceUnit, BattleUnit targetUnit, GenericMove attack)
        {
            EnableAttacks(false);
            EnableDialog(true);

            if (!IsAbleToAttack(sourceUnit))
            {
                yield return new WaitForSeconds(1f);
                yield break;
            }

            attack.Uses--;
            _dialogBox.SetDialog($"{sourceUnit.Monster.Base.Species} used {attack.Base.MoveID}");

            yield return new WaitForSeconds(1f);

            MoveResults damageDetails = new MoveResults();
            switch (attack.Base.Target)
            {
                case MoveTarget.Enemy:
                    damageDetails = targetUnit.Monster.RecieveAttack(attack, sourceUnit.Monster);
                    break;
                case MoveTarget.Self:
                    damageDetails = targetUnit.Monster.RecieveAttack(attack, sourceUnit.Monster);
                    break;
                default:
                    break;
            }

            _playerHUD.UpdateHealth();
            _opponentHUD.UpdateHealth();

            yield return new WaitForSeconds(1f);

            if (damageDetails.Critical)
            {
                _dialogBox.SetDialog($"Critical Hit!");
                yield return new WaitForSeconds(1f);
            }

            if (damageDetails.Effective != MoveResults.Effectiveness.Normal)
            {
                _dialogBox.SetDialog($"It was {damageDetails.Effective} effective");
                yield return new WaitForSeconds(1f);
            }

            if (damageDetails.IsKO)
            {
                _dialogBox.SetDialog($"{targetUnit.Monster.Base.Species} was knocked out");
                yield return new WaitForSeconds(1f);

                ExecuteTargetFainted(targetUnit);
                yield break;
            }

            yield return RunStatusAffects(targetUnit, damageDetails);
            yield return RunStatusAffects(sourceUnit, damageDetails);

            if (damageDetails.TargetPermCondition != Conditions.PermanentCondition.None)
            {
                _dialogBox.SetDialog($"{targetUnit.Monster.Base.Species} is {damageDetails.TargetPermCondition}");
                (targetUnit.IsPlayer ? _playerHUD : _opponentHUD).SetStatusCondition(Conditions.PermanentConditionColours[damageDetails.TargetPermCondition]);
                yield return new WaitForSeconds(1f);
            }

            if (damageDetails.UserPermCondition != Conditions.PermanentCondition.None)
            {
                _dialogBox.SetDialog($"{sourceUnit.Monster.Base.Species} is {damageDetails.UserPermCondition}");
                (sourceUnit.IsPlayer ? _playerHUD : _opponentHUD).SetStatusCondition(Conditions.PermanentConditionColours[damageDetails.TargetPermCondition]);
                yield return new WaitForSeconds(1f);
            }

            if (damageDetails.TargetTempCondition != Conditions.TemporaryCondition.None)
            {
                _dialogBox.SetDialog($"{targetUnit.Monster.Base.Species} is {damageDetails.TargetTempCondition}");
                yield return new WaitForSeconds(1f);
            }

            if (damageDetails.UserTempCondition != Conditions.TemporaryCondition.None)
            {
                _dialogBox.SetDialog($"{sourceUnit.Monster.Base.Species} is {damageDetails.UserTempCondition}");
                yield return new WaitForSeconds(1f);
            }

            if (damageDetails.WeatherCondition != Utils.Conditions.WeatherCondition.None && damageDetails.WeatherCondition != _weatherCondition)
            {
                _weatherCondition = damageDetails.WeatherCondition;
                _weatherCountdown = 5;
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
                sourceUnit.Monster.TemporaryCondition = Utils.Conditions.TemporaryCondition.None;
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
                sourceUnit.Monster.PermanentCondition = Utils.Conditions.PermanentCondition.None;
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
                sourceUnit.Monster.PermanentCondition = Utils.Conditions.PermanentCondition.None;
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
            if (targetUnit.IsPlayer)
            {
                var nextMon = _playerParty.GetHealthyMon();
                if (nextMon != null)
                {
                    OpenPartyScreen();
                    _isPlayerFainted = true;
                }
            }

            //exit battle
        }

        public void CallSwitchMonster(int index)
        {
            StartCoroutine(SwitchMonster(index));
        }

        IEnumerator SwitchMonster(int monsterPartyIndex)
        {
            _partyScreen.gameObject.SetActive(false);
            bool activeSwitch = false;
            if (_playerUnit.Monster.CurrentHealth > 0)
            {
                _dialogBox.SetDialog($"return {_playerUnit.Monster.Base.Species}");
                yield return new WaitForSeconds(1f);
                activeSwitch = true;
            }
            var newMon = _playerParty.GetMonByIndex(monsterPartyIndex);
            _dialogBox.SetDialog($"Go {newMon.Base.Species}!");
            yield return new WaitForSeconds(1f);
            _playerUnit.Setup(newMon);
            _playerHUD.SetHUDData(_playerUnit.Monster);

            if (activeSwitch)
            {
                yield return PerformEnemyAttack();
            }

            yield return EndofRoundEffects();
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
                    yield return new WaitForSeconds(1f);
                    break;
                case Conditions.PermanentCondition.Burnt:
                    isKO = unit.Monster.ReceiveConditionalDamage(Mathf.FloorToInt(unit.Monster.MaxHealth / 16f));
                    _dialogBox.SetDialog($"{unit.Monster.Base.Species} was hurt by the burn");
                    yield return new WaitForSeconds(1f);
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

        private IEnumerator RunStatusAffects(BattleUnit unit, MoveResults moveResults)
        {
            foreach (var stat in moveResults.TargetStatsAffected)
            {
                switch (stat.Value)
                {
                    case -1:
                        _dialogBox.SetDialog($"{unit.Monster.Base.Species}'s {stat.Key} was lowered");
                        yield return new WaitForSeconds(1f);
                        break;
                    case 1:
                        _dialogBox.SetDialog($"{unit.Monster.Base.Species}'s {stat.Key} was raised");
                        yield return new WaitForSeconds(1f);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}