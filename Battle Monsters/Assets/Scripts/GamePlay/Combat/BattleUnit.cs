using BattleMonsters.Monster;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleMonsters.GamePlay.Combat
{
    public class BattleUnit : MonoBehaviour
    {
        [SerializeField]
        private bool _isPlayer;
        public bool IsPlayer { get => _isPlayer; }

        private GenericMonster _monster = null;
        private Transform _vfxPoint = null;
        private Animator _animator = null;
        private AudioSource _audioSource = null;

        public GenericMonster Monster { get => _monster; }
        public Transform VFXPoint { get => _vfxPoint; }
        public Animator Animator { get => _animator; }
        public AudioSource AudioSource { get => _audioSource; }

        public void Setup(GenericMonster monster)
        {
            //Instanitate Mon
            _monster = monster;
            MonsterModelPassthrough model = Instantiate(monster.Base.Model, transform).gameObject.GetComponent<MonsterModelPassthrough>();

            _vfxPoint = model.VFXPoint;
            _animator = model.Animator;
            _audioSource = model.AudioSource;
        }
    }
}