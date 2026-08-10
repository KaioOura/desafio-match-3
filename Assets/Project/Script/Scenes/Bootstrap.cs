using UnityEngine;

namespace Gazeus.DesafioMatch3.Scenes
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private string _firstScene = SceneNames.Menu;

        #region Unity
        private void Start()
        {
            SceneLoader.Instance.Load(_firstScene);
        }
        #endregion
    }
}
