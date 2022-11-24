using System;
using System.Collections.Generic;
using UnityEngine;

namespace BattleMonsters.Utils
{
    public class Conditions
    {
        [Flags]
        public enum PermanentCondition
        {
            None = 0,
            Asleep = 1<<1,
            Poisoned = 1<<2,
            Frozen = 1<<3,
            Burnt = 1<<4,
            Paralyzed = 1<<5
        }

        public static readonly Dictionary<PermanentCondition, Color> PermanentConditionColours = new Dictionary<PermanentCondition, Color> {
            { PermanentCondition.None, Color.white},
            { PermanentCondition.Asleep, Color.grey},
            { PermanentCondition.Poisoned, Color.magenta},
            { PermanentCondition.Frozen, Color.cyan},
            { PermanentCondition.Burnt, Color.red},
            { PermanentCondition.Paralyzed, Color.yellow}
        };

        [Flags]
        public enum TemporaryCondition
        {
            None = 0,
            Confusion = 1<<1,
            Curse = 1<<2
        }

        public enum WeatherCondition
        {
            None = 0,
            Sunny,
            Raining,
            Hail,
            SandStorm
        }
    }
}