using BattleMonsters.Monster;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleMonsters.GamePlay.Combat
{
    public class BattleUnit : MonoBehaviour
    {
        [SerializeField]
        private bool _isPlayer;
        public bool IsPlayer { get => _isPlayer; }

        private Animator _animator = null;
        public Animator Animator { get => _animator; }

        public GenericMonster Monster { get; set; }

        public void Setup(GenericMonster monster)
        {
            //Instanitate Mon
            Monster = monster;
            GameObject model = Instantiate(monster.Base.Model, transform);
            _animator = GetComponentInChildren<Animator>();
        }
    }
}