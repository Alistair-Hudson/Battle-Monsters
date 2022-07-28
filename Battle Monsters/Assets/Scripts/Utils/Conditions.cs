using System;
using System.Collections.Generic;
using UnityEngine;

namespace BattleMonsters.Utils
{
    public class Conditions
    {
        public enum PermanentCondition
        {
            None = 0,
            Asleep,
            Poisoned,
            Frozen,
            Burnt,
            Paralyzed
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
            Confusion = 1,
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