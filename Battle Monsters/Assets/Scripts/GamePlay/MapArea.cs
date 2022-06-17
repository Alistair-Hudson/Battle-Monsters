using BattleMonsters.Monster;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleMonsters.GamePlay.Map
{
    public class MapArea : MonoBehaviour
    {
        [SerializeField]
        List<GenericMonster> _wildMons = new List<GenericMonster>();

        public GenericMonster GetRandomMon() 
        {
            var mon = _wildMons[Random.Range(0, _wildMons.Count)];
            mon.Init();
            return mon;
        }
    }
}