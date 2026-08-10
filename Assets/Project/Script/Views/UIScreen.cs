using System;
using Gazeus.DesafioMatch3.Core;
using Gazeus.DesafioMatch3.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Gazeus.DesafioMatch3.Views
{
    [RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
    public class UIScreen : MonoBehaviour, IScreen
    {
        public event Action Opened;
        public event Action Closed;

        [SerializeField] private ScreenDefinition _definition;
        [Tooltip("Root screen this scene opens with.")]
        [SerializeField] private bool _openOnRegister;

        private Canvas _canvas;
        private GraphicRaycaster _raycaster;
        private ScreenManager _manager;

        public string Name => name;
        public ScreenDefinition Definition => _definition;
        public bool OpenOnRegister => _openOnRegister;
        public bool IsVisible => _canvas != null && _canvas.enabled;

        #region Unity
        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _raycaster = GetComponent<GraphicRaycaster>();

            if (_definition == null)
            {
                Debug.LogError("UIScreen has no ScreenDefinition assigned.", this);
                return;
            }

            if (_definition.BlocksInput) CreateBlocker();

            _canvas.enabled = false;
            _raycaster.enabled = false;
        }

        private void OnEnable()
        {
            if (_definition == null) return;

            _manager = ScreenManager.Instance;
            _manager.Register(this);
        }

        private void OnDisable()
        {
            if (_manager == null) return;

            _manager.Unregister(this);
            _manager = null;
        }
        #endregion

        public void SetVisible(bool visible)
        {
            if (_canvas.enabled == visible) return;

            _canvas.enabled = visible;
            _raycaster.enabled = visible;

            if (visible) Opened?.Invoke();
            else Closed?.Invoke();
        }

        public void SetSortingOrder(int order)
        {
            _canvas.overrideSorting = true;
            _canvas.sortingOrder = order;
        }

        private void CreateBlocker()
        {
            GameObject blocker = new("InputBlocker", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            blocker.layer = gameObject.layer;

            RectTransform rect = (RectTransform)blocker.transform;
            rect.SetParent(transform, false);
            rect.SetAsFirstSibling();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image image = blocker.GetComponent<Image>();
            image.color = _definition.BlockerColor;
            image.raycastTarget = true;
        }
    }
}
