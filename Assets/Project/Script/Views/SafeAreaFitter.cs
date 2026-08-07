using UnityEngine;

namespace Gazeus.DesafioMatch3.Views
{
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaFitter : MonoBehaviour
    {
        private Rect _appliedSafeArea;
        private int _appliedScreenHeight;
        private int _appliedScreenWidth;
        private RectTransform _rectTransform;

        #region Unity
        private void Awake()
        {
            _rectTransform = (RectTransform)transform;
        }

        private void OnEnable()
        {
            Apply();
        }

        private void Update()
        {
            // A screen rotation does not reliably raise OnRectTransformDimensionsChange on this
            // rect, so poll instead and bail out early while nothing has changed.
            if (Screen.safeArea == _appliedSafeArea
                && Screen.width == _appliedScreenWidth
                && Screen.height == _appliedScreenHeight)
            {
                return;
            }

            Apply();
        }
        #endregion

        private void Apply()
        {
            int screenWidth = Screen.width;
            int screenHeight = Screen.height;
            if (screenWidth <= 0 || screenHeight <= 0) return;

            Rect safeArea = Screen.safeArea;

            _rectTransform.anchorMin = new Vector2(safeArea.xMin / screenWidth, safeArea.yMin / screenHeight);
            _rectTransform.anchorMax = new Vector2(safeArea.xMax / screenWidth, safeArea.yMax / screenHeight);
            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;

            _appliedSafeArea = safeArea;
            _appliedScreenWidth = screenWidth;
            _appliedScreenHeight = screenHeight;
        }
    }
}
