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
        
        public int CurrentHealth { get; set; }
        public List<GenericMove> KnownMoves { get; set; }
        public Dictionary<Stat, int> BaseStats { get; private set; }
        public Dictionary<Stat, int> CurrentStats { get; private set; }
        public Dictionary<Stat, int> StatModifiers { get; private set; }

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
            MaxHealth = Mathf.FloorToInt(Base.MaxHealth * Level);

            CurrentStats = new Dictionary<Stat, int>();
            CurrentStats.Add(Stat.Attack, BaseStats[Stat.Attack]);
            CurrentStats.Add(Stat.Defense, BaseStats[Stat.Defense]);
            CurrentStats.Add(Stat.Speed, BaseStats[Stat.Speed]);
            CurrentHealth = MaxHealth;

            StatModifiers = new Dictionary<Stat, int>();
            StatModifiers.Add(Stat.Attack, 0);
            StatModifiers.Add(Stat.Defense, 0);
            StatModifiers.Add(Stat.Speed, 0);
        }

        public int Attack { get => CurrentStats[Stat.Attack]; }
        public int Defense { get => CurrentStats[Stat.Defense]; }
        public int Speed { get => CurrentStats[Stat.Speed]; }
        public int MaxHealth { get; private set; }

        public MoveResults RecieveAttack(GenericMove attack, GenericMonster attacker)
        {
            MoveResults damageDetails = new MoveResults();
            damageDetails.StatsAffected = new Dictionary<Stat, int>();
            SetDamageDetails(attack, attacker, damageDetails);

            if (!damageDetails.IsKO)
            {
                ApplyEffects(attack.Base.Effects, damageDetails);
            }

            return damageDetails;
        }

        private void SetDamageDetails(GenericMove attack, GenericMonster attacker, MoveResults damageDetails)
        {
            float typeModifier = TypeChart.GetEffectiveness(attack.Base.Type, Base.Type1) * TypeChart.GetEffectiveness(attack.Base.Type, Base.Type2);
            float randomModifier = UnityEngine.Random.Range(0.85f, 1f);
            float a = (2f * attacker.Level + 10f) / 250f;
            float d = a * attack.Base.Power * ((float)attacker.CurrentStats[Stat.Attack] / CurrentStats[Stat.Defense]) + 2f;
            int damage = Mathf.FloorToInt(d * randomModifier * typeModifier);

            if (UnityEngine.Random.value * 100f <= 6.25f)
            {
                damage *= 2;
                damageDetails.Critical = true;
            }

            CurrentHealth -= damage;
            if (CurrentHealth <= 0f)
            {
                CurrentHealth = 0;
                damageDetails.IsKO = true;
            }

            if (typeModifier > 1)
            {
                damageDetails.Effective = MoveResults.Effectiveness.Super;
            }
            else if (typeModifier < 1)
            {
                damageDetails.Effective = MoveResults.Effectiveness.Not;
            }
        }

        private void ApplyEffects(MoveEffects effects, MoveResults damageDetails)
        {
            foreach (var statMod in effects.StatEffects)
            {
                StatModifiers[statMod.Stat] = Mathf.Clamp(StatModifiers[statMod.Stat] + statMod.Modifier, -6, 6);
                float modifier = 0.5f * StatModifiers[statMod.Stat] + 1f;
                if (modifier < 0)
                {
                    CurrentStats[statMod.Stat] = Mathf.FloorToInt(BaseStats[statMod.Stat] / -modifier);
                    damageDetails.StatsAffected.Add(statMod.Stat, -1);
                }
                else if (modifier > 0)
                {
                    CurrentStats[statMod.Stat] = Mathf.FloorToInt(BaseStats[statMod.Stat] * modifier);
                    damageDetails.StatsAffected.Add(statMod.Stat, 1);
                }
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
            public Dictionary<Stat, int> StatsAffected { get; set; }
        }
    }
}