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
    }
}