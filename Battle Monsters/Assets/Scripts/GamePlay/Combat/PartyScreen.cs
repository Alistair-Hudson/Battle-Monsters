using BattleMonsters.Monster;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BattleMonsters.GamePlay.Combat
{
    public class PartyScreen : MonoBehaviour
    {
        PartyMemberUI[] _partyMembers;

        public void Init()
        {
            _partyMembers = GetComponentsInChildren<PartyMemberUI>();
        }

        public void SetPartyData(List<GenericMonster> monsters, GenericMonster activeMon)
        {
            for (int i = 0; i < _partyMembers.Length; i++)
            {
                if (i < monsters.Count)
                {
                    _partyMembers[i].gameObject.SetActive(true);
                    _partyMembers[i].SetData(monsters[i]);
                    if (monsters[i].CurrentHealth <= 0)
                    {
                        _partyMembers[i].GetComponent<Button>().interactable = false;
                    }
                    else if (monsters[i] == activeMon)
                    {
                        _partyMembers[i].GetComponent<Button>().interactable = false;
                    }
                    else
                    {
                        _partyMembers[i].GetComponent<Button>().interactable = true;
                    }
                }
                else
                {
                    _partyMembers[i].gameObject.SetActive(false);
                }
            }
        }
    }
}