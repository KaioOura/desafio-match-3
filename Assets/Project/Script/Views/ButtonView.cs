using UnityEngine;
using UnityEngine.UI;

namespace Gazeus.DesafioMatch3.Views
{
    [RequireComponent(typeof(Button))]
    public abstract class ButtonView : MonoBehaviour
    {
        private Button _button;

        #region Unity
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClick);
        }
        #endregion

        protected abstract void OnClick();
    }
}
