using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleMonsters.Moves
{
    public class GenericMove
    {
        public MoveBase Base { get; set; }
        public int Uses { get; set; }

        public GenericMove(MoveBase moveBase)
        {
            Base = moveBase;
            Uses = moveBase.Uses;
        }
    }
}