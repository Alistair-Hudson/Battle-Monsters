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
        [Tooltip("For moves that are a sure hit set value to -1")]
        [Range(0, 100)]
        private int _accuracy;
        [SerializeField]
        private int _uses;
        [SerializeField] 
        private MoveEffects _moveEffects;
        [SerializeField]
        private MoveTarget _moveTarget;
        [SerializeField]
        private MoveType _moveType = MoveType.Melee;

        public string MoveID { get => _moveID; }
        public string Description { get => _description; }
        public Utils.Type Type { get => _type; }
        public int Power { get => _power; }
        public int Accuracy { get => _accuracy; }
        public int Uses { get => _uses; }
        public MoveEffects Effects { get => _moveEffects; }
        public MoveTarget Target { get => _moveTarget; }
        public MoveType MoveType { get => _moveType; }
    }
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField]
    private List<StatEffect> _targetStatEffects;
    public List<StatEffect> TargetStatEffects { get => _targetStatEffects; }
    [SerializeField]
    private List<StatEffect> _userStatEffects;
    public List<StatEffect> UserStatEffects { get => _userStatEffects; }
    public Conditions.PermanentCondition TargetPermCondition;
    [Range(0, 1)]
    public float ChanceOfTargetPermCondition;
    public Conditions.PermanentCondition UserPermCondition;
    [Range(0, 1)]
    public float ChanceOfUserPermCondition;
    public Conditions.TemporaryCondition TargetTempCondition;
    [Range(0, 1)]
    public float ChanceOfTargetTempCondition;
    public Conditions.TemporaryCondition UserTempCondition;
    [Range(0, 1)]
    public float ChanceOfUserTempCondition;
    public Conditions.WeatherCondition WeatherCondition;
    [Range(0, 1)]
    public float ChanceOfWeatherCondition;
}

[System.Serializable]
public class StatEffect
{
    public Stat Stat;
    [Range(-1, 1)]
    public float Modifier;
    [Range(0, 1)]
    public float ChanceOfEffect;
}

public enum MoveTarget
{
    Enemy = 0,
    Self
}

public enum MoveType
{
    Melee = 0,
    Ranged,
    SelfBuff
}