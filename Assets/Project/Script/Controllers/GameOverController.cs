using Gazeus.DesafioMatch3.Core;
using Gazeus.DesafioMatch3.ScriptableObjects;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Controllers
{
    public class GameOverController : MonoBehaviour
    {
        [SerializeField] private GameController _game;
        [SerializeField] private ScreenDefinition _screen;

        #region Unity
        private void Awake()
        {
            _game.NoMovesLeft += OnNoMovesLeft;
        }

        private void OnDestroy()
        {
            _game.NoMovesLeft -= OnNoMovesLeft;
        }
        #endregion

        private void OnNoMovesLeft()
        {
            ScreenManager.Instance.Open(_screen);
        }
    }
}
