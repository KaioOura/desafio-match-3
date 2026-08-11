using System;
using Gazeus.DesafioMatch3.Core.Audio;
using Gazeus.DesafioMatch3.ScriptableObjects.Feedback;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gazeus.DesafioMatch3.Views
{
    public class LevelButtonView : MonoBehaviour
    {
        public event Action<int> Clicked;

        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private SoundCue _clickCue;

        private int _index;

        #region Unity
        private void Awake()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClick);
        }
        #endregion

        public void Setup(int index, string label)
        {
            _index = index;
            _label.text = label;
        }

        private void OnClick()
        {
            AudioService.Instance.Play(_clickCue);

            Clicked?.Invoke(_index);
        }
    }
}
