using BattleMonsters.Moves;
using BattleMonsters.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleMonsters.Monster
{
    [Serializable]
    public class GenericMonster
    {
        [SerializeField]
        private MonsterBase _base;
        [SerializeField]
        private int _level;
        public MonsterBase Base { get => _base;}
        public int Level { get => _level;}

        private int _baseAccuracy = 100;
        private int _baseEvasion = 100;

        public int CurrentHealth { get; private set; }
        public List<GenericMove> KnownMoves { get; private set; }
        public Dictionary<Stat, int> BaseStats { get; private set; }
        public Dictionary<Stat, int> CurrentStats { get; private set; }
        public Conditions.PermanentCondition PermanentCondition { get; private set; }
        public Conditions.TemporaryCondition TemporaryCondition { get; private set; }
        public int SleepCount { get; set; }
        public int ConfusionCount { get; set; }

        public void Init()
        {
            KnownMoves = new List<GenericMove>();
            foreach (var move in Base.LearnableMoves)
            {
                if (move.Level <= Level)
                {
                    KnownMoves.Add(new GenericMove(move.MoveBase));
                }
                if (KnownMoves.Count >= 4)
                {
                    break;
                }
            }

            BaseStats = new Dictionary<Stat, int>();
            BaseStats.Add(Stat.Attack, Mathf.FloorToInt(Base.Attack * Level));
            BaseStats.Add(Stat.Defense, Mathf.FloorToInt(Base.Defense * Level));
            BaseStats.Add(Stat.Speed, Mathf.FloorToInt(Base.Speed * Level));
            BaseStats.Add(Stat.Accuracy, _baseAccuracy);
            BaseStats.Add(Stat.Evasion, _baseEvasion);
            MaxHealth = Mathf.FloorToInt(Base.MaxHealth * Level);

            CurrentStats = new Dictionary<Stat, int>();
            CurrentStats.Add(Stat.Attack, BaseStats[Stat.Attack]);
            CurrentStats.Add(Stat.Defense, BaseStats[Stat.Defense]);
            CurrentStats.Add(Stat.Speed, BaseStats[Stat.Speed]);
            CurrentStats.Add(Stat.Accuracy, BaseStats[Stat.Accuracy]);
            CurrentStats.Add(Stat.Evasion, BaseStats[Stat.Evasion]);
            CurrentHealth = MaxHealth;
        }

        public int Attack { get => PermanentCondition == Conditions.PermanentCondition.Burnt ? Mathf.FloorToInt(CurrentStats[Stat.Attack] / 2) : CurrentStats[Stat.Attack]; }
        public int Defense { get => CurrentStats[Stat.Defense]; }
        public int Speed { get => PermanentCondition == Conditions.PermanentCondition.Paralyzed ? Mathf.FloorToInt(CurrentStats[Stat.Speed] / 2) : CurrentStats[Stat.Speed]; }
        public int MaxHealth { get; private set; }
        public int Accuracy { get => CurrentStats[Stat.Accuracy]; }
        public int Evasion { get => CurrentStats[Stat.Evasion]; }

        public OnHitResult ReceiveDamage(GenericMove attack, GenericMonster attacker)
        {
            OnHitResult result = OnHitResult.None;
            float typeModifier = TypeChart.GetEffectiveness(attack.Base.Type, Base.Type1) * TypeChart.GetEffectiveness(attack.Base.Type, Base.Type2);
            float randomModifier = UnityEngine.Random.Range(0.85f, 1f);
            float a = (2f * attacker.Level + 10f) / 250f;
            float d = a * attack.Base.Power * ((float)attacker.Attack / Defense) + 2f;
            int damage = Mathf.FloorToInt(d * randomModifier * typeModifier);

            if (UnityEngine.Random.value * 100f <= 6.25f)
            {
                damage *= 2;
                result |= OnHitResult.CriticalHit;
            }

            CurrentHealth -= damage;
            if (CurrentHealth <= 0f)
            {
                CurrentHealth = 0;
                result |= OnHitResult.KO;
            }

            if (typeModifier == 0)
            {
                result |= OnHitResult.NoEffect;
            }
            else if (typeModifier > 1)
            {
                result |= OnHitResult.SuperEffective;
            }
            else if (typeModifier < 1)
            {
                result |= OnHitResult.NotVeryEffective;
            }

            return result;
        }

        public void ApplyStatusEffect(GenericMove attack, GenericMonster attacker)
        {
            MoveEffects effects = attack.Base.Effects;
            foreach (var statMod in effects.TargetStatEffects)
            {
                CurrentStats[statMod.Stat] = Mathf.FloorToInt(Mathf.Clamp(CurrentStats[statMod.Stat] + BaseStats[statMod.Stat] * statMod.Modifier, BaseStats[statMod.Stat] / 2, BaseStats[statMod.Stat] * 2));
                if (statMod.Modifier < 0)
                {
                    
                }
                else if (statMod.Modifier > 0)
                {
                    
                }
            }
            foreach (var statMod in effects.UserStatEffects)
            {
                attacker.CurrentStats[statMod.Stat] = Mathf.FloorToInt(Mathf.Clamp(attacker.CurrentStats[statMod.Stat] + attacker.BaseStats[statMod.Stat] * statMod.Modifier, attacker.BaseStats[statMod.Stat] / 2, attacker.BaseStats[statMod.Stat] * 2));
                if (statMod.Modifier < 0)
                {
                    
                }
                else if (statMod.Modifier > 0)
                {
                    
                }
            }
        }

        public void ApplyStatusCondition(Conditions.PermanentCondition condition)
        {
            if (PermanentCondition == Conditions.PermanentCondition.None)
            {
                PermanentCondition = condition;
                if (condition == Conditions.PermanentCondition.Asleep)
                {
                    SleepCount = UnityEngine.Random.Range(2, 6);
                }
            }
        }

        public void ApplyVolatileStatusCondition(Conditions.TemporaryCondition condition)
        {
            if (!TemporaryCondition.HasFlag(condition))
            {
                TemporaryCondition |= condition;
                if (condition.HasFlag(Conditions.TemporaryCondition.Confusion))
                {
                    ConfusionCount = UnityEngine.Random.Range(2, 6);
                }
            }
        }

        public bool ReceiveConditionalDamage(int damage)
        {
            CurrentHealth -= damage;
            if (CurrentHealth <= 0)
            {
                return true;
            }
            return false;
        }

        public void HealHealth(int heal)
        {
            CurrentHealth += heal;
            if (CurrentHealth >= MaxHealth)
            {
                CurrentHealth = MaxHealth;
            }
        }

        public void CureStatus (Utils.Conditions.PermanentCondition permanentCondition, Utils.Conditions.TemporaryCondition temporaryCondition)
        {
            if ((PermanentCondition & permanentCondition) != 0)
            {
                PermanentCondition = Conditions.PermanentCondition.None;
            }

            if ((TemporaryCondition & temporaryCondition) != 0)
            {
                TemporaryCondition = Conditions.TemporaryCondition.None;
            }
        }

        public class MoveResults
        {
            public enum Effectiveness
            {
                Normal = 0,
                Not,
                Super
            }
            public bool IsKO { get; set; }
            public Effectiveness Effective { get; set; }
            public bool Critical { get; set; }
            public Dictionary<Stat, int> TargetStatsAffected { get; set; }
            public Dictionary<Stat, int> UserStatsAffected { get; set; }
            public Conditions.PermanentCondition TargetPermCondition { get; set; }
            public Conditions.PermanentCondition UserPermCondition { get; set; }
            public Conditions.TemporaryCondition TargetTempCondition { get; set; }
            public Conditions.TemporaryCondition UserTempCondition { get; set; }

            public Conditions.WeatherCondition WeatherCondition;
        }
    }
}