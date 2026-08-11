using System;
using Gazeus.DesafioMatch3.Core;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.ScriptableObjects;
using Gazeus.DesafioMatch3.Views;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Controllers
{
    public class GameController : MonoBehaviour
    {
        public event Action<Move> MoveAvailable;
        public event Action NoMovesLeft;
        public event Action PlayerActed;
        public event Action<int, int> TileSelected;
        public event Action<int, int> SwapStarted;
        public event Action SelectionCleared;
        public event Action<int, int> SwapRejected;

        private GameService _gameService;
        private BoardView _boardView;
        private LevelConfig _level;
        private Move _availableMove;
        private bool _hasAvailableMove;
        private bool _isAnimating;
        private int _selectedX = -1;
        private int _selectedY = -1;

        #region Unity
        private void OnDestroy()
        {
            if (_boardView == null) return;

            _boardView.TileClicked -= OnTileClick;
            _boardView.CascadeFinished -= OnBoardSettled;
        }

        private void Start()
        {
            if (_gameService == null) return;

            Board board = _gameService.StartGame(_level);
            _boardView.CreateBoard(board);

            EvaluateBoard();
        }
        #endregion

        public void Initialize(GameService gameService, BoardView boardView, LevelConfig level)
        {
            _gameService = gameService;
            _boardView = boardView;
            _level = level;

            _boardView.TileClicked += OnTileClick;
            _boardView.CascadeFinished += OnBoardSettled;
        }

        private void EvaluateBoard()
        {
            _hasAvailableMove = _gameService.TryFindMove(out _availableMove);

            NotifyMoveAvailability();
        }

        private void NotifyMoveAvailability()
        {
            if (_hasAvailableMove) MoveAvailable?.Invoke(_availableMove);
            else NoMovesLeft?.Invoke();
        }

        private void OnBoardSettled()
        {
            _isAnimating = false;

            EvaluateBoard();
        }

        private void OnTileClick(int x, int y)
        {
            if (_isAnimating) return;

            PlayerActed?.Invoke();

            if (_selectedX > -1 && _selectedY > -1)
            {
                if (Mathf.Abs(_selectedX - x) + Mathf.Abs(_selectedY - y) > 1)
                {
                    _selectedX = -1;
                    _selectedY = -1;

                    SelectionCleared?.Invoke();

                    NotifyMoveAvailability();
                }
                else
                {
                    _isAnimating = true;

                    SwapStarted?.Invoke(x, y);

                    _boardView.SwapTiles(_selectedX, _selectedY, x, y).onComplete += () =>
                    {
                        bool isValid = _gameService.IsValidMovement(_selectedX, _selectedY, x, y);
                        if (isValid)
                        {
                            _boardView.Play(_gameService.SwapTile(_selectedX, _selectedY, x, y));
                        }
                        else
                        {
                            SwapRejected?.Invoke(x, y);

                            _boardView.SwapTiles(x, y, _selectedX, _selectedY).onComplete += () =>
                            {
                                _isAnimating = false;

                                NotifyMoveAvailability();
                            };
                        }
                        _selectedX = -1;
                        _selectedY = -1;
                    };
                }
            }
            else
            {
                _selectedX = x;
                _selectedY = y;

                TileSelected?.Invoke(x, y);
            }
        }
    }
}
