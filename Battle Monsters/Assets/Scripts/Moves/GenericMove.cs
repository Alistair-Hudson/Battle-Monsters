using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleMonsters.Moves
{
    public abstract class GenericMove
    {
        public abstract void UseMove(Monster.MonsterStatManager target);
    }
}
