using Gazeus.DesafioMatch3.Core;
using Gazeus.DesafioMatch3.ScriptableObjects;
using Gazeus.DesafioMatch3.ScriptableObjects.Feedback;
using Gazeus.DesafioMatch3.Views;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Controllers
{
    public class GameOverController : MonoBehaviour
    {
        [SerializeField] private GameController _game;
        [SerializeField] private ScreenDefinition _screen;
        [SerializeField] private EffectsView _effects;
        [SerializeField] private EffectCue _gameOverCue;

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
            _effects.PlaySound(_gameOverCue);

            ScreenManager.Instance.Open(_screen);
        }
    }
}
