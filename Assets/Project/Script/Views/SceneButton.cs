using Gazeus.DesafioMatch3.Scenes;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Views
{
    public class SceneButton : ButtonView
    {
        [SerializeField] private string _targetScene = SceneNames.Menu;

        protected override void OnClick()
        {
            SceneLoader.Instance.Load(_targetScene);
        }
    }
}
