using UnityEngine;
using UnityEngine.UI;

namespace Gazeus.DesafioMatch3.Scenes
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private string _targetScene = SceneNames.Gameplay;

        #region Unity
        private void Awake()
        {
            _playButton.onClick.AddListener(OnPlayClick);
        }

        private void OnDestroy()
        {
            _playButton.onClick.RemoveListener(OnPlayClick);
        }
        #endregion

        private void OnPlayClick()
        {
            if (SceneLoader.Instance == null)
            {
                Debug.LogError("No SceneLoader available. Start the game from the Boot scene.", this);
                return;
            }

            if (SceneLoader.Instance.IsLoading) return;

            _playButton.interactable = false;
            SceneLoader.Instance.Load(_targetScene);
        }
    }
}
