using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleMonsters.Monster
{
    public class Health : MonoBehaviour
    {
        [SerializeField]
        private float _baseHealth = 100;

        private float _maxHealth;

        public float CurrentHealth { get; private set; }

        private void Awake()
        {
            _maxHealth = _baseHealth;
            CurrentHealth = _maxHealth;
        }

        public void ReceiveDamage(float damage)
        {
            CurrentHealth -= damage;
            if (CurrentHealth <= 0)
            {
                //knocked out
            }
        }

        public void Heal(float healAmount)
        {
            CurrentHealth += healAmount;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, _maxHealth);
        }
    }
}