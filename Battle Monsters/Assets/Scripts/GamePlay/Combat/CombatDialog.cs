using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BattleMonsters.GamePlay.Combat
{
    public class CombatDialog : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _dialog;

        public void SetDialog(string dialog)
        {
            StartCoroutine(TypeDialog(dialog));
        }

        private IEnumerator TypeDialog(string dialog)
        {
            _dialog.text = "";
            foreach (var character in dialog.ToCharArray())
            {
                _dialog.text += character;
                yield return null;
            }
        }
    }
}