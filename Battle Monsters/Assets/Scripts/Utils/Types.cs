using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleMonsters.Utils
{
    public enum Type
    {
        None = 0,
        Fire,
        Water,
        Grass
    }

    public class TypeChart
    {
        static float[][] chart =
        {
            //                          None    Fire    Water   Grass
            /*None*/    new float[] {   1,      1,      1,      1},
            /*Fire*/    new float[] {   1,      1f,     0.5f,   2f},
            /*Water*/   new float[] {   1,      2f,     1f,     0.5f},
            /*Grass*/   new float[] {   1,      0.5f,   2f,     1f}
        };

        public static float GetEffectiveness(Type attckType, Type defenseType)
        {
            return chart[(int)attckType][(int)defenseType];
        }
    }
}
