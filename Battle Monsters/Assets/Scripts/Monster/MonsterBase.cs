using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleMonsters.Monster
{
    [CreateAssetMenu(fileName = "Monster", menuName = "ScriptableObjects/Monster")]
    public class MonsterBase : ScriptableObject
    {
        [SerializeField]
        private string _species = "";
        [SerializeField]
        private GameObject _model;

        [SerializeField]
        private Utils.Type _type1;
        [SerializeField]
        private Utils.Type _type2;

        [SerializeField]
        private List<LearnableMove> _learnableMoves;

        [SerializeField]
        private int _baseSpeed = 1;
        [SerializeField]
        private int _baseAttack = 1;
        [SerializeField]
        private int _baseDefense = 1;
        [SerializeField]
        private int _maxHealth = 10;

        public string Species { get => _species; }
        public Utils.Type Type1 { get => _type1; }
        public Utils.Type Type2 { get => _type2; }
        public int Speed { get => _baseSpeed; }
        public int Attack { get => _baseAttack; }
        public int Defense { get => _baseDefense; }
        public int MaxHealth { get => _maxHealth; }
        public List<LearnableMove> LearnableMoves { get => _learnableMoves; }

    }

    [Serializable]
    public class LearnableMove
    {
        [SerializeField]
        private Moves.MoveBase _moveBase;
        [SerializeField]
        private int _level;

        public Moves.MoveBase MoveBase { get => _moveBase; }
        public int Level { get => _level; }
    }
}
