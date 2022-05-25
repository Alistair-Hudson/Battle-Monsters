using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleMonsters.Monster
{
    public class MonsterStatManager : MonoBehaviour
    {
        [SerializeField]
        private int _baseSpeed = 1;
        [SerializeField]
        private int _baseAttack = 1;
        [SerializeField]
        private int _baseDefense = 1;
        [SerializeField]
        private int _baseHealth = 10;

        private int _maxHealth;
        private int _currentHealth;

        public int Level { get; private set; }
        public int Speed { get => _baseSpeed * Level; }
        public int Attack { get => _baseAttack * Level; }
        public int Defense { get => _baseDefense * Level; }
        public int Health { get => _currentHealth; }

        private void Awake()
        {
            Level = 1;
            _maxHealth = _baseHealth * Level;
            _currentHealth = _maxHealth;
        }

        public void ReceiveDamage(int damage)
        {
            _currentHealth -= damage;
            if (_currentHealth <= 0)
            {
                //knocked out
            }
        }

        public void Heal(int healAmount)
        {
            _currentHealth += healAmount;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        }

        private void LevelUp()
        {
            Level++;
            _maxHealth = _baseHealth * Level;
        }
    }
}