using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BattleMonsters.Monster
{
    public class MonsterModelPassthrough : MonoBehaviour
    {
        [SerializeField]
        private Transform _vfxPoint = null;

        private Animator _animator = null;
        private AudioSource _audioSource = null;

        public Transform VFXPoint { get => _vfxPoint; }
        public Animator Animator { get => _animator; }
        public AudioSource AudioSource { get => _audioSource; }

        public UnityEvent AnimationTriggered = new UnityEvent();

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _audioSource = GetComponent<AudioSource>();
        }

        public void AnimationTrigger()
        {
            AnimationTriggered.Invoke();
        }
    }
}