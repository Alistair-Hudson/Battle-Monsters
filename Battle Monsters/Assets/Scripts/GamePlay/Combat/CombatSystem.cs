using BattleMonsters.Monster;
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
            _playerUnit.Setup(_playerParty.GetHealthyMon());
            _playerHUD.SetHUDData(_playerUnit.Monster);
            _opponentUnit.Setup(_wildMon);
            _opponentHUD.SetHUDData(_opponentUnit.Monster);

            //Set Players party
            _partyScreen.gameObject.SetActive(true);
            _partyScreen.Init();
            _partyScreen.SetPartyData(_playerParty.PartyList);
            _partyScreen.gameObject.SetActive(false);

            _dialogBox.SetDialog($"A wild {_opponentUnit.Monster.Base.Species} appeard!");
            yield return new WaitForSeconds(1f);
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
            _attackButtons.SetButtons(_playerUnit.Monster.KnownMoves);
        }

        public void PerformAttack(int attackButtonIndex)
        {
            StartCoroutine(PerformPlayerAttack(attackButtonIndex));
        }

        private IEnumerator PerformPlayerAttack(int attackButtonIndex)
        {
            var attack = _playerUnit.Monster.KnownMoves[attackButtonIndex];
            EnableAttacks(false);
            EnableDialog(true);
            _dialogBox.SetDialog($"{_playerUnit.Monster.Base.Species} used {attack.Base.MoveID}");
            
            yield return new WaitForSeconds(1f);

            var damageDetails = _opponentUnit.Monster.RecieveDamage(attack, _playerUnit.Monster);
            _opponentHUD.UpdateHealth();
            yield return new WaitForSeconds(1f);

            if (damageDetails.Critical)
            {
                _dialogBox.SetDialog($"Critical Hit!");
                yield return new WaitForSeconds(1f);
            }

            if (damageDetails.Effective != Monster.GenericMonster.DamageDetails.Effectiveness.Normal)
            {
                _dialogBox.SetDialog($"It was {damageDetails.Effective} effective");
                yield return new WaitForSeconds(1f);
            }

            if (damageDetails.IsKO)
            {
                _dialogBox.SetDialog($"{_opponentUnit.Monster.Base.Species} was knocked out");
                yield return new WaitForSeconds(1f);

                //exit batle
            }
            else
            {
                StartCoroutine(PerformEnemyAttack());
            }
        }

        private IEnumerator PerformEnemyAttack()
        {
            var attack = _opponentUnit.Monster.KnownMoves[UnityEngine.Random.Range(0, _opponentUnit.Monster.KnownMoves.Count)];
            _dialogBox.SetDialog($"{_opponentUnit.Monster.Base.Species} used {attack.Base.MoveID}");

            yield return new WaitForSeconds(1f);

            var damageDetails = _playerUnit.Monster.RecieveDamage(attack, _opponentUnit.Monster);
            _playerHUD.UpdateHealth();
            yield return new WaitForSeconds(1f);

            if (damageDetails.Critical)
            {
                _dialogBox.SetDialog($"Critical Hit!");
                yield return new WaitForSeconds(1f);
            }

            if (damageDetails.Effective != Monster.GenericMonster.DamageDetails.Effectiveness.Normal)
            {
                _dialogBox.SetDialog($"It was {damageDetails.Effective} effective");
                yield return new WaitForSeconds(1f);
            }

            if (damageDetails.IsKO)
            {
                _dialogBox.SetDialog($"{_playerUnit.Monster.Base.Species} was knocked out");
                yield return new WaitForSeconds(1f);
                var nextMon = _playerParty.GetHealthyMon();
                if (nextMon != null)
                {
                    _playerUnit.Setup(nextMon);
                    _playerHUD.SetHUDData(_playerUnit.Monster);
                }
                else
                {
                    //exit battle
                }
            }
            else
            {
                _dialogBox.SetDialog($"What Should {_playerUnit.Monster.Base.Species} do?");
                EnableActions(true);
            }
        }
    }
}