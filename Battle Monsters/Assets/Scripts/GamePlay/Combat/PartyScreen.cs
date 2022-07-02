using BattleMonsters.Monster;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BattleMonsters.GamePlay.Combat
{
    public class PartyScreen : MonoBehaviour
    {
        PartyMemberUI[] _partyMembers;

        public void Init()
        {
            _partyMembers = GetComponentsInChildren<PartyMemberUI>();
        }

        public void SetPartyData(List<GenericMonster> monsters)
        {
            for (int i = 0; i < _partyMembers.Length; i++)
            {
                if (i < monsters.Count)
                {
                    _partyMembers[i].gameObject.SetActive(true);
                    _partyMembers[i].SetData(monsters[i]);
                }
                else
                {
                    _partyMembers[i].gameObject.SetActive(false);
                }
            }
        }
    }
}