using System;
using DG.Tweening;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Views
{
    public class ParticleEffectView : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particles;

        private ParticleSystemRenderer[] _renderers;
        private float _lifetime;

        #region Unity
        private void Awake()
        {
            _renderers = GetComponentsInChildren<ParticleSystemRenderer>(true);

            ParticleSystem.MainModule main = _particles.main;
            _lifetime = main.duration + main.startLifetime.constantMax;
        }
        #endregion

        public void Play(Action onComplete)
        {
            _particles.Clear(true);
            _particles.Play(true);

            DOVirtual.DelayedCall(_lifetime, () => onComplete()).SetTarget(transform);
        }

        public void SetSortingOrder(int order)
        {
            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].sortingOrder = order;
            }
        }
    }
}
