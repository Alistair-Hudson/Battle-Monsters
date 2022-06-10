using BattleMonsters.Moves;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleMonsters.Monster
{
    public class GenericMonster
    {
        public MonsterBase Base { get; set; }
        public int Level { get; set; }
        
        public int CurrentHealth { get; set; }
        public List<Moves.GenericMove> KnownMoves { get; set; }

        public GenericMonster(MonsterBase monsterBase, int level)
        {
            Base = monsterBase;
            Level = level;
            CurrentHealth = MaxHealth;

            KnownMoves = new List<Moves.GenericMove>();
            foreach (var move in Base.LearnableMoves)
            {
                if (move.Level <= Level)
                {
                    KnownMoves.Add(new Moves.GenericMove(move.MoveBase));
                }
                if (KnownMoves.Count >= 4)
                {
                    break;
                }
            }
        }

        public int Attack { get => Mathf.FloorToInt(Base.Attack * Level); }
        public int Defense { get => Mathf.FloorToInt(Base.Defense * Level); }
        public int Speed { get => Mathf.FloorToInt(Base.Speed * Level); }
        public int MaxHealth { get => Mathf.FloorToInt(Base.MaxHealth * Level); }

        public DamageDetails RecieveDamage(GenericMove attack, GenericMonster attacker)
        {
            DamageDetails damageDetails = new DamageDetails();
            float typeModifier = Utils.TypeChart.GetEffectiveness(attack.Base.Type, Base.Type1) * Utils.TypeChart.GetEffectiveness(attack.Base.Type, Base.Type2);
            float randomModifier = Random.Range(0.85f, 1f);
            float a = (2f * attacker.Level + 10f) / 250f;
            float d = a * attack.Base.Power * ((float)attacker.Attack / Defense) + 2f;
            int damage = Mathf.FloorToInt(d * randomModifier * typeModifier);

            if (Random.value * 100f <= 6.25f)
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
                damageDetails.Effective = DamageDetails.Effectiveness.Super;
            }
            else if (typeModifier < 1)
            {
                damageDetails.Effective = DamageDetails.Effectiveness.Not;
            }
            
            return damageDetails;
        }

        public class DamageDetails
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
        }
    }
}