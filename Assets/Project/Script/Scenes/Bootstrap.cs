using UnityEngine;

namespace Gazeus.DesafioMatch3.Scenes
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private string _firstScene = SceneNames.Menu;

        #region Unity
        private void Start()
        {
            if (SceneLoader.Instance == null)
            {
                Debug.LogError("Boot scene has no SceneLoader.", this);
                return;
            }

            SceneLoader.Instance.Load(_firstScene);
        }
        #endregion
    }
}
