using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BattleMonsters.GamePlay.Combat
{
    public class HUD : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _name;
        [SerializeField]
        private TMP_Text _level;
        [SerializeField]
        private HealthBar _health;

        private Monster.GenericMonster _monster;

        public void SetHUDData(Monster.GenericMonster monster)
        {
            _monster = monster;
            _name.text = monster.Base.Species;
            _level.text = "Lvl: " + monster.Level;
            _health.SetHealth(monster.MaxHealth, monster.CurrentHealth);
        }

        public void UpdateHealth()
        {
            _health.SetHealth(_monster.MaxHealth, _monster.CurrentHealth, true);
        }
    }
}