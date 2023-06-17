using UnityEngine;
using UnityEditor;
using System.IO;
using BattleMonsters.Moves;
using BattleMonsters.Monster;

namespace BattleMonsters.Editor
{
    public class CSVtoSO : MonoBehaviour
    {
        private static string _movesCSVPath = "/Editor/CSVs/Moves.csv";
        private static string _monstersCSVPath = "/Editor/CSVs/Monsters.csv";

        [MenuItem("Utils/Generate Moves")]
        public static void GenerateMoves()
        {
            string[] lines = File.ReadAllLines(Application.dataPath + _movesCSVPath);

            for (int i = 1; i < lines.Length; i++)
            {
                string[] line = lines[i].Split(',');
                MoveBase move = AssetDatabase.LoadAssetAtPath($"Assets/Resources/Moves/{line[0]}.asset", typeof(MoveBase)) as MoveBase;
                if (move)
                {
                    move.SetMove(line);
                }
                else
                {
                    move = ScriptableObject.CreateInstance<MoveBase>();
                    move.SetMove(line);

                    AssetDatabase.CreateAsset(move, $"Assets/Resources/Moves/{move.MoveID}.asset");
                }
            }

            AssetDatabase.SaveAssets();
        }

        [MenuItem("Utils/Generate Monsters")]
        public static void GenerateMonsters()
        {
            string[] lines = File.ReadAllLines(Application.dataPath + _monstersCSVPath);

            for (int i = 1; i < lines.Length; i++)
            {
                string[] line = lines[i].Split(',');

                MonsterBase monster = AssetDatabase.LoadAssetAtPath($"Assets/Resources/Monsters/{line[0]}.asset", typeof(MonsterBase)) as MonsterBase;
                if (monster)
                {
                    monster.SetMonster(line);
                }
                else
                {

                    monster = ScriptableObject.CreateInstance<MonsterBase>();
                    monster.SetMonster(line);

                    AssetDatabase.CreateAsset(monster, $"Assets/Resources/Monsters/{monster.Species}.asset");
                }
            }

            AssetDatabase.SaveAssets();
        }
    }
}