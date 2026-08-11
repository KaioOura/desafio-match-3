using System;
using Gazeus.DesafioMatch3.Core.Audio;
using Gazeus.DesafioMatch3.ScriptableObjects.Feedback;
using UnityEngine;
using UnityEngine.UI;

namespace Gazeus.DesafioMatch3.Views
{
    [RequireComponent(typeof(Button))]
    public class ButtonView : MonoBehaviour
    {
        public event Action Clicked;

        [SerializeField] private SoundCue _clickCue;

        private Button _button;

        #region Unity
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(HandleClick);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(HandleClick);
        }
        #endregion
        
        private void HandleClick()
        {
            AudioService.Instance.Play(_clickCue);

            OnClick();
        }

        protected virtual void OnClick()
        {
            Clicked?.Invoke();
        }
    }
}
