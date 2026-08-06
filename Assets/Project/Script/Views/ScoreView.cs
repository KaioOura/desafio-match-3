using DG.Tweening;
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
            ScorePopupView popup = Instantiate(_scorePopupPrefab, _popupContainer);
            popup.transform.position = worldPosition;
            popup.Play(points);
        }
    }
}
