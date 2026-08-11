using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Gazeus.DesafioMatch3.Views
{
    public class TileSpotView : MonoBehaviour
    {
        public event Action<int, int> Clicked;

        [SerializeField] private Button _button;
        [SerializeField] private Image _deadBackground;
        [SerializeField] private Color _deadColor = new(0.1f, 0.1f, 0.12f, 1f);
        [SerializeField] private Image _highlight;
        [SerializeField] private Color _highlightColor = new(1f, 1f, 1f, 0.55f);
        [SerializeField] private float _highlightPulseDuration = 0.45f;
        [SerializeField] private Image _selection;
        [SerializeField] private Color _selectionColor = new(0.3f, 0.85f, 1f, 0.5f);
        [SerializeField] private float _selectionPunchScale = 0.12f;
        [SerializeField] private float _selectionPunchDuration = 0.25f;

        private Tween _highlightTween;
        private int _x;
        private int _y;

        #region Unity
        private void Awake()
        {
            _button.onClick.AddListener(OnTileClick);
        }

        private void OnDestroy()
        {
            _highlightTween?.Kill();
            _highlightTween = null;
        }
        #endregion

        public Tween AnimatedSetTile(GameObject tile)
        {
            tile.transform.SetParent(transform);
            tile.transform.DOKill();

            return tile.transform.DOMove(transform.position, 0.3f);
        }
        
        public void SetDead()
        {
            _button.enabled = false;

            if (_deadBackground == null) return;

            _deadBackground.color = _deadColor;
            _deadBackground.enabled = true;
        }

        public void SetHighlighted(bool highlighted)
        {
            if (_highlight == null) return;

            _highlightTween?.Kill();
            _highlightTween = null;

            if (!highlighted)
            {
                _highlight.enabled = false;
                return;
            }

            _highlight.transform.SetAsLastSibling();
            _highlight.color = new Color(_highlightColor.r, _highlightColor.g, _highlightColor.b, 0f);
            _highlight.enabled = true;

            _highlightTween = DOTween
                .To(() => _highlight.color.a, SetHighlightAlpha, _highlightColor.a, _highlightPulseDuration)
                .SetLoops(-1, LoopType.Yoyo);
        }

        public void SetSelected(bool selected)
        {
            if (_selection == null) return;

            _selection.transform.SetAsLastSibling();
            _selection.color = _selectionColor;
            _selection.enabled = selected;

            if (!selected) return;

            Transform selectionTransform = _selection.transform;
            selectionTransform.DOKill(true);
            selectionTransform.localScale = Vector3.one;
            selectionTransform.DOPunchScale(Vector3.one * _selectionPunchScale, _selectionPunchDuration, 6, 0.6f);
        }

        public void SetPosition(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public void SetTile(GameObject tile)
        {
            tile.transform.SetParent(transform, false);
            tile.transform.position = transform.position;
        }

        private void SetHighlightAlpha(float alpha)
        {
            _highlight.color = new Color(_highlightColor.r, _highlightColor.g, _highlightColor.b, alpha);
        }

        private void OnTileClick()
        {
            Clicked?.Invoke(_x, _y);
        }
    }
}
