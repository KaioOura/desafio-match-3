using UnityEngine;
using UnityEngine.UI;

namespace Gazeus.DesafioMatch3.Views
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(GridLayoutGroup))]
    public class GridCellSizeFitter : MonoBehaviour
    {
        [SerializeField] private int _columns = 10;
        [SerializeField] private int _rows = 10;

        private AspectRatioFitter _aspectRatioFitter;
        private GridLayoutGroup _grid;
        private RectTransform _rectTransform;

        #region Unity
        private void Awake()
        {
            CacheComponents();
        }

        private void OnEnable()
        {
            CacheComponents();
            Refit();
        }

        private void OnRectTransformDimensionsChange()
        {
            Refit();
        }
        
        #endregion
        
        public void SetBoardSize(int columns, int rows)
        {
            _columns = columns;
            _rows = rows;

            if (_columns > 0)
            {
                _grid.constraintCount = _columns;
            }
            
            if (_aspectRatioFitter != null && _columns > 0 && _rows > 0)
            {
                _aspectRatioFitter.aspectRatio = (float)_columns / _rows;
            }

            Refit();
        }

        private void CacheComponents()
        {
            if (_rectTransform == null) _rectTransform = (RectTransform)transform;
            if (_grid == null) _grid = GetComponent<GridLayoutGroup>();
            if (_aspectRatioFitter == null) _aspectRatioFitter = GetComponent<AspectRatioFitter>();
        }

        private void Refit()
        {
            if (_grid == null || _rectTransform == null) return;
            if (_columns <= 0 || _rows <= 0) return;

            RectOffset padding = _grid.padding;
            Vector2 spacing = _grid.spacing;
            Rect rect = _rectTransform.rect;

            float availableWidth = rect.width - padding.horizontal - spacing.x * (_columns - 1);
            float availableHeight = rect.height - padding.vertical - spacing.y * (_rows - 1);
            
            if (availableWidth <= 0 || availableHeight <= 0) return;

            float cell = Mathf.Floor(Mathf.Min(availableWidth / _columns, availableHeight / _rows));
            if (cell <= 0) return;
            
            if (Mathf.Approximately(_grid.cellSize.x, cell) && Mathf.Approximately(_grid.cellSize.y, cell)) return;

            _grid.cellSize = new Vector2(cell, cell);
        }
    }
}
