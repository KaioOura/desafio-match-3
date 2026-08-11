using Gazeus.DesafioMatch3.Core;
using Gazeus.DesafioMatch3.ScriptableObjects;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Controllers
{
    public class GameOverController : MonoBehaviour
    {
        private GameController _game;
        private ScreenDefinition _screen;

        #region Unity
        private void OnDestroy()
        {
            if (_game != null) _game.NoMovesLeft -= OnNoMovesLeft;
        }
        #endregion

        public void Initialize(GameController game, ScreenDefinition screen)
        {
            _game = game;
            _screen = screen;

            _game.NoMovesLeft += OnNoMovesLeft;
        }

        private void OnNoMovesLeft()
        {
            ScreenManager.Instance.Open(_screen);
        }
    }
}
