using DG.Tweening;
using Gazeus.DesafioMatch3.Core.Pooling;
using TMPro;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Views
{
    public class ScoreView : MonoBehaviour
    {
        [SerializeField] private RectTransform _popupContainer;
        [SerializeField] private ScorePopupView _scorePopupPrefab;
        [SerializeField] private TMP_Text _scoreLabel;
        [SerializeField] private float _punchDuration = 0.3f;
        [SerializeField] private float _punchScale = 0.25f;

        private PrefabPool _popupPool;

        #region Unity
        private void Awake()
        {
            Transform root = PrefabPool.CreateRoot(transform, "[Popup Pool]");
            _popupPool = new PrefabPool(_scorePopupPrefab.gameObject, root);
        }
        #endregion

        public void ResetScore(int score)
        {
            _scoreLabel.text = score.ToString();
        }

        public void SetScore(int score)
        {
            _scoreLabel.text = score.ToString();

            Transform labelTransform = _scoreLabel.transform;
            labelTransform.DOKill(true);
            labelTransform.localScale = Vector3.one;
            labelTransform.DOPunchScale(Vector3.one * _punchScale, _punchDuration, 6, 0.6f);
        }

        public void ShowPoints(int points, Vector3 worldPosition)
        {
            GameObject instance = _popupPool.Get(_popupContainer);
            instance.transform.position = worldPosition;

            ScorePopupView popup = instance.GetComponent<ScorePopupView>();
            popup.Play(points, () => _popupPool.Release(instance));
        }
    }
}
