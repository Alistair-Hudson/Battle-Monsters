using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BattleMonsters.Monster
{
    public class Party : MonoBehaviour
    {
        [SerializeField]
        List<GenericMonster> _party = new List<GenericMonster>();

        public List<GenericMonster> PartyList { get => _party; }

        private void Start()
        {
            foreach (var mon in _party)
            {
                mon.Init();
            }
        }

        public GenericMonster GetHealthyMon()
        {
            return _party.Where(x => x.CurrentHealth > 0).FirstOrDefault();
        }

        public GenericMonster GetMonByIndex(int index)
        {
            return _party[index];
        }
    }
}