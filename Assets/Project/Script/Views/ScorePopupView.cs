using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Views
{
    public class ScorePopupView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private float _duration = 0.8f;
        [SerializeField] private float _floatDistance = 80f;

        public void Play(int points, Action onComplete)
        {
            _label.text = $"+{points}";
            _canvasGroup.alpha = 1f;
            
            Tween fade = DOTween.To(() => _canvasGroup.alpha, alpha => _canvasGroup.alpha = alpha, 0f, _duration);

            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOMoveY(transform.position.y + _floatDistance, _duration).SetEase(Ease.OutCubic));
            sequence.Join(fade.SetEase(Ease.InQuad));

            sequence.SetTarget(transform);
            sequence.onComplete += () => onComplete();
        }
    }
}
