using System.Collections.Generic;
using Gazeus.DesafioMatch3.Core;
using Gazeus.DesafioMatch3.Scenes;
using Gazeus.DesafioMatch3.ScriptableObjects;
using Gazeus.DesafioMatch3.Views;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Controllers
{
    public class LevelSelectController : MonoBehaviour
    {
        [SerializeField] private UIScreen _screen;
        [SerializeField] private LevelCatalog _catalog;
        [SerializeField] private LevelButtonView _buttonTemplate;
        [SerializeField] private Transform _container;
        [SerializeField] private string _targetScene = SceneNames.Gameplay;

        private readonly List<LevelButtonView> _buttons = new();
        private readonly List<LevelConfig> _levels = new();

        #region Unity
        private void Awake()
        {
            _buttonTemplate.gameObject.SetActive(false);
            _screen.Opened += OnScreenOpened;
        }

        private void OnDestroy()
        {
            _screen.Opened -= OnScreenOpened;
            ClearButtons();
        }
        #endregion

        private void OnScreenOpened()
        {
            if (_catalog == null)
            {
                Debug.LogError("LevelSelectController has no LevelCatalog assigned.", this);
                return;
            }

            ClearButtons();
            CreateButtons();
        }

        private void CreateButtons()
        {
            IReadOnlyList<LevelConfig> levels = _catalog.Levels;
            for (int i = 0; i < levels.Count; i++)
            {
                LevelConfig level = levels[i];
                if (level == null)
                {
                    Debug.LogError($"LevelCatalog has an empty entry at index {i}.", _catalog);
                    continue;
                }

                LevelButtonView button = Instantiate(_buttonTemplate, _container);
                button.name = level.name;
                button.gameObject.SetActive(true);
                button.Setup(_levels.Count, level.DisplayName);
                button.Clicked += OnLevelClick;

                _levels.Add(level);
                _buttons.Add(button);
            }
        }

        private void ClearButtons()
        {
            foreach (LevelButtonView button in _buttons)
            {
                button.Clicked -= OnLevelClick;
                button.gameObject.SetActive(false);
                Destroy(button.gameObject);
            }

            _buttons.Clear();
            _levels.Clear();
        }

        private void OnLevelClick(int index)
        {
            LevelSession.Select(_levels[index]);
            SceneLoader.Instance.Load(_targetScene);
        }
    }
}
