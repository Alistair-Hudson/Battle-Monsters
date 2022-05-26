using BattleMonsters.Monster;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleMonsters.GamePlay.Combat
{
    public class BattleUnit : MonoBehaviour
    {
        [SerializeField]
        private MonsterBase _base;
        [SerializeField]
        private int _level;
        [SerializeField]
        private bool _isPlayer;

        public GenericMonster Monster { get; set; }

        public void Setup()
        {
            Monster = new GenericMonster(_base, _level);
            //Instanitae Mon
        }
    }
}