using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleMonsters.Moves
{
    [CreateAssetMenu(fileName = "Move", menuName = "ScriptableObjects/Move")]
    public class MoveBase: ScriptableObject
    {
        [SerializeField]
        private string _moveID;
        [SerializeField]
        private string _description;
        [SerializeField]
        private Utils.Type _type;
        [SerializeField]
        private int _power;
        [SerializeField]
        private int _accuracy;
        [SerializeField]
        private int _uses;

        public string MoveID { get => _moveID; }
        public string Description { get => _description; }
        public Utils.Type Type { get => _type; }
        public int Power { get => _power; }
        public int Accuracy { get => _accuracy; }
        public int Uses { get => _uses; }
    }
}
