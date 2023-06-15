using BattleMonsters.Moves;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleMonsters.Monster
{
    [CreateAssetMenu(fileName = "Monster", menuName = "ScriptableObjects/Monster")]
    [InlineEditor]
    public class MonsterBase : ScriptableObject
    {
        [BoxGroup("Basic Info")]
        [SerializeField]
        private string _species = "";

        [HorizontalGroup("Game Data", 75)]
        [PreviewField(75)]
        [HideLabel]
        [SerializeField]
        private GameObject _model;

        [VerticalGroup("Game Data/Stats")]
        [SerializeField]
        private Utils.Type _type1;
        [VerticalGroup("Game Data/Stats")]
        [SerializeField]
        private Utils.Type _type2;
        [VerticalGroup("Game Data/Stats")]
        [SerializeField]
        private int _baseSpeed = 1;
        [VerticalGroup("Game Data/Stats")]
        [SerializeField]
        private int _baseAttack = 1;
        [VerticalGroup("Game Data/Stats")]
        [SerializeField]
        private int _baseDefense = 1;
        [VerticalGroup("Game Data/Stats")]
        [SerializeField]
        private int _maxHealth = 10;

        [SerializeField]
        private List<MoveBase> _moveSet;


        public string Species { get => _species; }
        public GameObject Model { get => _model; }
        public Utils.Type Type1 { get => _type1; }
        public Utils.Type Type2 { get => _type2; }
        public int Speed { get => _baseSpeed; }
        public int Attack { get => _baseAttack; }
        public int Defense { get => _baseDefense; }
        public int MaxHealth { get => _maxHealth; }
        public List<MoveBase> MoveSet { get => _moveSet; }

        public void SetMonster(string[] data)
        {
            _species = data[0];
            Enum.TryParse(data[1], out _type1);
            Enum.TryParse(data[2], out _type2);
            _baseSpeed = int.Parse(data[3]);
            _baseAttack = int.Parse(data[4]);
            _baseDefense = int.Parse(data[5]);
            _maxHealth = int.Parse(data[6]);
            _moveSet = new List<MoveBase>();
            _moveSet.Add(Resources.Load($"Moves/{data[7]}") as MoveBase);
            _moveSet.Add(Resources.Load($"Moves/{data[8]}") as MoveBase);
            _moveSet.Add(Resources.Load($"Moves/{data[9]}") as MoveBase);
            _moveSet.Add(Resources.Load($"Moves/{data[10]}") as MoveBase);
        }

    }
}
