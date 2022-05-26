using BattleMonsters.Moves;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleMonsters.Monster
{
    [RequireComponent(typeof(MonsterStatManager))]
    public class MoveSet : MonoBehaviour
    {
        [Serializable]
        private struct MoveData
        {
            public int Level;
            public GenericMove Move;
        }

        [SerializeField]
        private List<MoveData> _moveLearnSet = new List<MoveData>();

        public List<GenericMove> KnownMoves { get; private set; }

        private void Awake()
        {
            GetComponent<MonsterStatManager>().OnLevelUp += OnLevelUp;
        }

        private void OnLevelUp(int level)
        {
            foreach (var moveData in _moveLearnSet)
            {
                if (moveData.Level != level) { continue; }
                if (KnownMoves.Count < 4) { KnownMoves.Add(moveData.Move); }
                else
                {
                    //delete a move
                    KnownMoves.Add(moveData.Move);
                }
            }
        }
    }
}
