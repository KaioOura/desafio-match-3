using Gazeus.DesafioMatch3.Core.Audio;
using Gazeus.DesafioMatch3.Core.Pooling;
using Gazeus.DesafioMatch3.ScriptableObjects.Feedback;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Views
{
    public class EffectsView : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private int _sortingOrder = 5;
        [SerializeField] private RectTransform _popupContainer;
        [SerializeField] private ScorePopupView _pointsPopupPrefab;

        private PrefabPoolRegistry _pool;

        #region Unity
        private void Awake()
        {
            _pool = new PrefabPoolRegistry(transform, "[Effect Pool]");
        }
        #endregion

        public void PlaySound(EffectCue cue, float pitchScale = 1f)
        {
            if (cue == null) return;

            AudioService.Instance.Play(cue.Sound, pitchScale);
        }

        public void Spawn(EffectCue cue, Vector3 worldPosition)
        {
            if (cue == null || cue.Particles == null) return;

            GameObject instance = _pool.Get(cue.Particles.gameObject, _container);
            instance.transform.position = worldPosition;

            ParticleEffectView effect = instance.GetComponent<ParticleEffectView>();
            effect.SetSortingOrder(_sortingOrder);
            effect.Play(() => _pool.Release(instance));
        }

        public void Play(EffectCue cue, Vector3 worldPosition, float pitchScale = 1f)
        {
            PlaySound(cue, pitchScale);
            Spawn(cue, worldPosition);
        }

        public void ShowPoints(int points, Vector3 worldPosition)
        {
            GameObject instance = _pool.Get(_pointsPopupPrefab.gameObject, _popupContainer);
            instance.transform.position = worldPosition;

            ScorePopupView popup = instance.GetComponent<ScorePopupView>();
            popup.Play(points, () => _pool.Release(instance));
        }
    }
}
