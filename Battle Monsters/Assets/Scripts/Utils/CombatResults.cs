using System;

namespace BattleMonsters.Utils
{
    [Flags]
    public enum OnHitResult
    {
        None = 0,
        CriticalHit = 1 << 1,
        SuperEffective = 1 << 2,
        NotVeryEffective = 1 << 3,
        NoEffect = 1 << 4,
        KO = 1 << 5
    }
}