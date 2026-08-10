using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gazeus.DesafioMatch3.Scenes
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Slider _progressBar;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private float _fadeDuration = 0.35f;

        private Canvas _canvas;

        #region Unity
        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvasGroup.alpha = 0f;

            SetProgress(0f);
            SetVisible(false);
        }
        #endregion

        public void SetVisible(bool visible)
        {
            _canvas.enabled = visible;
            _canvasGroup.blocksRaycasts = visible;
        }

        public Tween FadeIn()
        {
            return Fade(1f).SetEase(Ease.OutQuad);
        }

        public Tween FadeOut()
        {
            return Fade(0f).SetEase(Ease.InQuad);
        }

        public void SetProgress(float normalized)
        {
            _progressBar.value = Mathf.Clamp01(normalized);
        }

        public void SetLabel(string text)
        {
            if (_label != null) _label.text = text;
        }

        private Tween Fade(float target)
        {
            return DOTween.To(() => _canvasGroup.alpha, alpha => _canvasGroup.alpha = alpha, target, _fadeDuration);
        }
    }
}
