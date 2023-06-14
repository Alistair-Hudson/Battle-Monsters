using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BattleMonsters.GamePlay.Combat
{
    public class HUD : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _name;
        [SerializeField]
        private HealthBar _health;
        [SerializeField]
        private Image _statusCondition;

        private Monster.GenericMonster _monster;

        public void SetHUDData(Monster.GenericMonster monster)
        {
            _monster = monster;
            _name.text = monster.Base.Species;
            _health.SetHealth(monster.MaxHealth, monster.CurrentHealth);
            SetStatusCondition(Utils.Conditions.PermanentConditionColours[monster.PermanentCondition]);
        }

        public void UpdateHealth()
        {
            _health.SetHealth(_monster.MaxHealth, _monster.CurrentHealth, true);
        }

        public void SetStatusCondition(Color statusColour)
        {
            _statusCondition.color = statusColour;
        }
    }
}