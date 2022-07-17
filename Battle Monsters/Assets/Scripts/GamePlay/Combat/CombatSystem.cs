using BattleMonsters.Monster;
using BattleMonsters.Moves;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        private bool _isPlayerFainted = false;

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
            if (_playerUnit.Monster.Speed >= _opponentUnit.Monster.Speed)
            {
                yield return PerformPlayerAttack(attackButtonIndex);
                yield return PerformEnemyAttack();

            }
            else
            {
                yield return PerformEnemyAttack();
                if (!_isPlayerFainted)
                {
                    yield return PerformPlayerAttack(attackButtonIndex);
                }
                _isPlayerFainted = false;
            }

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
            //if Asleep
                //try awake
                //if still asleep
                    //"is asleep"
                    //wakeup chance increase
                    //exit
                //else
                    //"wokeup"

            //if frozen
                //try unfreeze
                //if still frozen
                    //"is frozen"
                    //exit
                //else
                    //"unfroze"

            attack.Uses--;
            _dialogBox.SetDialog($"{sourceUnit.Monster.Base.Species} used {attack.Base.MoveID}");

            yield return new WaitForSeconds(1f);

            var damageDetails = targetUnit.Monster.RecieveDamage(attack, sourceUnit.Monster);
            _playerHUD.UpdateHealth();
            _opponentHUD.UpdateHealth();

            yield return new WaitForSeconds(1f);

            if (damageDetails.Critical)
            {
                _dialogBox.SetDialog($"Critical Hit!");
                yield return new WaitForSeconds(1f);
            }

            if (damageDetails.Effective != GenericMonster.DamageDetails.Effectiveness.Normal)
            {
                _dialogBox.SetDialog($"It was {damageDetails.Effective} effective");
                yield return new WaitForSeconds(1f);
            }

            if (damageDetails.IsKO)
            {
                _dialogBox.SetDialog($"{targetUnit.Monster.Base.Species} was knocked out");
                yield return new WaitForSeconds(1f);

                ExecuteTargetFainted(targetUnit);
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
            if ((_playerUnit.Monster.Speed >= _opponentUnit.Monster.Speed))
            {
                yield return ApplyStatusEffects(_playerUnit);
                yield return ApplyStatusEffects(_opponentUnit);
            }
            else
            {
                yield return ApplyStatusEffects(_opponentUnit);
                yield return ApplyStatusEffects(_playerUnit);
            }
            //Weathereffects
            _dialogBox.SetDialog($"What Should {_playerUnit.Monster.Base.Species} do?");
            EnableActions(true);
            yield return new WaitForSeconds(1f);
        }

        private IEnumerator ApplyStatusEffects(BattleUnit unit)
        {
            yield return null;
        }
    }
}