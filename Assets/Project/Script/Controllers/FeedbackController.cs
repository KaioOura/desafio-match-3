using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.ScriptableObjects.Feedback;
using Gazeus.DesafioMatch3.Views;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Controllers
{
    public class FeedbackController : MonoBehaviour
    {
        [SerializeField] private float _cascadePitchStep = 1f;

        private GameController _game;
        private BoardView _boardView;
        private HintController _hint;
        private EffectsView _effects;
        private FeedbackConfig _config;

        #region Unity
        private void OnDestroy()
        {
            if (_game != null)
            {
                _game.TileSelected -= OnTileSelected;
                _game.SwapStarted -= OnSwapStarted;
                _game.SelectionCleared -= OnSelectionCleared;
                _game.SwapRejected -= OnSwapRejected;
                _game.NoMovesLeft -= OnNoMovesLeft;
            }

            if (_boardView != null)
            {
                _boardView.CascadeStarted -= OnCascadeStarted;
                _boardView.TilesSpawned -= OnTilesSpawned;
            }

            if (_hint != null) _hint.HintShown -= OnHintShown;
        }
        #endregion

        public void Initialize(GameController game, BoardView boardView, HintController hint,
            EffectsView effects, FeedbackConfig config)
        {
            _game = game;
            _boardView = boardView;
            _hint = hint;
            _effects = effects;
            _config = config;

            _game.TileSelected += OnTileSelected;
            _game.SwapStarted += OnSwapStarted;
            _game.SelectionCleared += OnSelectionCleared;
            _game.SwapRejected += OnSwapRejected;
            _game.NoMovesLeft += OnNoMovesLeft;

            _boardView.CascadeStarted += OnCascadeStarted;
            _boardView.TilesSpawned += OnTilesSpawned;

            _hint.HintShown += OnHintShown;
        }

        private void OnTileSelected(int x, int y)
        {
            _boardView.Highlights.SetSelected(x, y);
            _effects.PlaySound(_config.Select);
        }

        private void OnSwapStarted(int x, int y)
        {
            _boardView.Highlights.ClearSelection();
            _effects.PlaySound(_config.Select);
        }

        private void OnSelectionCleared()
        {
            _boardView.Highlights.ClearSelection();
            _effects.PlaySound(_config.Deselect);
        }

        private void OnSwapRejected(int x, int y)
        {
            _effects.Play(_config.InvalidSwap, _boardView.GetTilePosition(x, y));
        }

        private void OnNoMovesLeft()
        {
            _effects.PlaySound(_config.GameOver);
        }

        private void OnCascadeStarted(BoardSequence step, int index)
        {
            _effects.PlaySound(_config.Match, Mathf.Pow(2f, index * _cascadePitchStep / 12f));

            for (int i = 0; i < step.MatchedPosition.Count; i++)
            {
                Vector2Int position = step.MatchedPosition[i];
                _effects.Spawn(_config.Match, _boardView.GetTilePosition(position.x, position.y));
            }
        }

        private void OnTilesSpawned()
        {
            _effects.PlaySound(_config.Spawn);
        }

        private void OnHintShown()
        {
            _effects.PlaySound(_config.Hint);
        }
    }
}
