using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BattleMonsters.GamePlay.Combat
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField]
        private Image _healthBar;
        [SerializeField]
        private TMP_Text _healthText;

        public void SetHealth(float maxHealth, float health, bool animate = false)
        {
            if (!animate)
            {
                _healthBar.fillAmount = health / maxHealth;
                _healthText.text = $"Health: {health}/{maxHealth}";
            }
            else
            {
                StartCoroutine(AnimateHealthBar(health, maxHealth));
            }
        }

        private IEnumerator AnimateHealthBar(float newHealth, float maxHealth)
        {
            float currentFill = _healthBar.fillAmount;
            float newFill = newHealth / maxHealth;
            float difference = currentFill - newFill;

            while (currentFill - newFill > Mathf.Epsilon)
            {
                currentFill -= difference * Time.deltaTime;
                _healthBar.fillAmount = currentFill;
                _healthText.text = $"Health: {(int)(maxHealth * currentFill)}/{maxHealth}";
                yield return null;
            }
            _healthBar.fillAmount = newFill;
            _healthText.text = $"Health: {newHealth}/{maxHealth}";
        }
    }
}