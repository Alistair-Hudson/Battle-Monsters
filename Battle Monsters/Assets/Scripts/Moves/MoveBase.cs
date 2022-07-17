using BattleMonsters.Utils;
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
        [SerializeField] 
        private MoveEffects _moveEffects;
        [SerializeField]
        private MoveTarget _moveTarget;

        public string MoveID { get => _moveID; }
        public string Description { get => _description; }
        public Utils.Type Type { get => _type; }
        public int Power { get => _power; }
        public int Accuracy { get => _accuracy; }
        public int Uses { get => _uses; }
        public MoveEffects Effects { get => _moveEffects; }
        public MoveTarget Target { get => _moveTarget; }
    }
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] private List<StatEffect> _statEffects;
    public List<StatEffect> StatEffects { get => _statEffects; }
}

[System.Serializable]
public class StatEffect
{
    public Stat Stat;
    public int Modifier;
}

public enum MoveTarget
{
    Enemy = 0,
    Self
}