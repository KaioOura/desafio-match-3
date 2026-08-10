using Gazeus.DesafioMatch3.Controllers;
using Gazeus.DesafioMatch3.ScriptableObjects;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Views
{
    public class ScreenButton : ButtonView
    {
        [SerializeField] private ScreenDefinition _target;

        protected override void OnClick()
        {
            ScreenManager.Instance.Open(_target);
        }
    }
}
