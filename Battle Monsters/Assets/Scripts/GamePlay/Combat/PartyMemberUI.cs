using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BattleMonsters.GamePlay.Combat
{
    public class PartyMemberUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _name;
        [SerializeField]
        private TMP_Text _level;
        [SerializeField]
        private HealthBar _health;

        private Monster.GenericMonster _monster;

        public void SetData(Monster.GenericMonster monster)
        {
            _monster = monster;
            _name.text = monster.Base.Species;
            _level.text = "Lvl: " + monster.Level;
            _health.SetHealth(monster.MaxHealth, monster.CurrentHealth);
        }
    }
}