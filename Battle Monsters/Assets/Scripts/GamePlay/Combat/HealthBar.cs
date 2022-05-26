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

        public void SetHealth(float maxHealth, float health)
        {
            _healthBar.fillAmount = health / maxHealth;
            _healthText.text = $"Health: {health}/{maxHealth}";
        }
    }
}