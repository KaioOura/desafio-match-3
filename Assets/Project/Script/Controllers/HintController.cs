using System;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Views;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Controllers
{
    public class HintController : MonoBehaviour
    {
        public event Action HintShown;

        [Tooltip("Seconds of player inactivity before the hint shows up on its own.")]
        [SerializeField] private float _delay = 5f;

        private GameController _game;
        private BoardView _boardView;
        private ButtonView _hintButton;
        private Move _move;
        private bool _armed;
        private bool _isVisible;
        private float _idleTime;

        #region Unity
        private void OnDestroy()
        {
            if (_game != null)
            {
                _game.MoveAvailable -= OnMoveAvailable;
                _game.PlayerActed -= OnPlayerActed;
            }

            if (_hintButton != null) _hintButton.Clicked -= Show;
        }

        private void Update()
        {
            if (!_armed || _isVisible) return;

            _idleTime += Time.deltaTime;
            if (_idleTime < _delay) return;

            Show();
        }
        #endregion

        public void Initialize(GameController game, BoardView boardView, ButtonView hintButton)
        {
            _game = game;
            _boardView = boardView;
            _hintButton = hintButton;

            _game.MoveAvailable += OnMoveAvailable;
            _game.PlayerActed += OnPlayerActed;

            if (_hintButton != null) _hintButton.Clicked += Show;
        }

        private void Show()
        {
            if (!_armed || _isVisible) return;

            _isVisible = true;
            _boardView.Highlights.ShowHint(_move);

            HintShown?.Invoke();
        }

        private void OnMoveAvailable(Move move)
        {
            _move = move;
            _armed = true;
            _idleTime = 0f;
        }

        private void OnPlayerActed()
        {
            _armed = false;
            _idleTime = 0f;

            if (!_isVisible) return;

            _isVisible = false;
            _boardView.Highlights.ClearHint();
        }
    }
}
