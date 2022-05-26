using BattleMonsters.Moves;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BattleMonsters.GamePlay.Combat
{
    public class AttackButtonsManager : MonoBehaviour
    {
        [SerializeField]
        private List<Button> _attacks; 

        public void SetButtons(List<GenericMove> moves)
        {
            int i = 0;
            for (; i < moves.Count; i++)
            {
                _attacks[i].gameObject.SetActive(true);
                _attacks[i].GetComponentInChildren<TMP_Text>().text = $"{moves[i].Base.MoveID}\n{moves[i].Base.Type}\n{moves[i].Uses}/{moves[i].Base.Uses}";
            }
            for (; i < _attacks.Count; i++)
            {
                _attacks[i].gameObject.SetActive(false);
            }
        }
    }
}
