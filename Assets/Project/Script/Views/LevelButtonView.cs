using System;
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
            Clicked?.Invoke(_index);
        }
    }
}
