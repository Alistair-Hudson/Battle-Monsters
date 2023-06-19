using BattleMonsters.Monster;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BattleMonsters.GamePlay.Combat
{
    public class BattleUnit : MonoBehaviour
    {
        [SerializeField]
        private bool _isPlayer;
        public bool IsPlayer { get => _isPlayer; }

        private GenericMonster _monster = null;
        private MonsterModelPassthrough _model = null;

        public GenericMonster Monster { get => _monster; }
        public MonsterModelPassthrough Model { get => _model; }

        public void Setup(GenericMonster monster)
        {
            //Instanitate Mon
            _monster = monster;
            _model = Instantiate(monster.Base.Model, transform).gameObject.GetComponent<MonsterModelPassthrough>();
        }
    }
}