using System;
using Gazeus.DesafioMatch3.Core;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.ScriptableObjects;
using Gazeus.DesafioMatch3.ScriptableObjects.Feedback;
using Gazeus.DesafioMatch3.Views;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Controllers
{
    public class GameController : MonoBehaviour
    {
        public event Action<Move> MoveAvailable;
        public event Action NoMovesLeft;
        public event Action PlayerActed;

        [SerializeField] private BoardView _boardView;
        [SerializeField] private ScoreView _scoreView;
        [SerializeField] private EffectsView _effects;
        [SerializeField] private EffectCue _selectCue;
        [SerializeField] private EffectCue _deselectCue;
        [SerializeField] private EffectCue _invalidSwapCue;
        [SerializeField] private ScoreConfig _scoreConfig;
        [SerializeField] private TileTypeConfig _tileTypeConfig;
        [SerializeField] private SpecialMatchConfig _specialMatchConfig;
        [SerializeField] private LevelConfig _level;

        private GameService _gameService;
        private ScoreService _scoreService;
        private Move _availableMove;
        private bool _hasAvailableMove;
        private bool _isAnimating;
        private int _selectedX = -1;
        private int _selectedY = -1;

        #region Unity
        private void Awake()
        {
            _gameService = new GameService(_tileTypeConfig, _specialMatchConfig);
            _scoreService = new ScoreService(_scoreConfig, _tileTypeConfig);
            _boardView.TileClicked += OnTileClick;
            _boardView.CascadeStarted += RegisterScore;
            _boardView.CascadeFinished += OnBoardSettled;
        }

        private void OnDestroy()
        {
            _boardView.TileClicked -= OnTileClick;
            _boardView.CascadeStarted -= RegisterScore;
            _boardView.CascadeFinished -= OnBoardSettled;
        }

        private void Start()
        {
            LevelConfig level = LevelSession.Current != null ? LevelSession.Current : _level;
            if (level == null)
            {
                Debug.LogError("GameController has no LevelConfig assigned.", this);
                return;
            }

            Board board = _gameService.StartGame(level);
            _boardView.CreateBoard(board);

            _scoreService.Reset();
            _scoreView.ResetScore(_scoreService.Score);

            EvaluateBoard();
        }
        #endregion

        private void RegisterScore(BoardSequence sequence, int index)
        {
            int points = _scoreService.RegisterCascade(sequence.MatchedTypes, index);
            _scoreView.SetScore(_scoreService.Score);
            _scoreView.ShowPoints(points, _boardView.GetMatchCenter(sequence.MatchedPosition));
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

                    _boardView.ClearSelection();
                    _effects.PlaySound(_deselectCue);

                    NotifyMoveAvailability();
                }
                else
                {
                    _isAnimating = true;
                    _boardView.ClearSelection();
                    _effects.PlaySound(_selectCue);
                    _boardView.SwapTiles(_selectedX, _selectedY, x, y).onComplete += () =>
                    {
                        bool isValid = _gameService.IsValidMovement(_selectedX, _selectedY, x, y);
                        if (isValid)
                        {
                            _boardView.Play(_gameService.SwapTile(_selectedX, _selectedY, x, y));
                        }
                        else
                        {
                            _effects.Play(_invalidSwapCue, _boardView.GetTilePosition(x, y));
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

                _boardView.SetSelected(x, y);
                _effects.PlaySound(_selectCue);
            }
        }
    }
}
