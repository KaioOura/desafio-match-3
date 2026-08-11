using System;
using Gazeus.DesafioMatch3.Core;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Views;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Controllers
{
    public class ScoreController : MonoBehaviour
    {
        public event Action<int, Vector3> PointsScored;

        private ScoreService _scoreService;
        private ScoreView _scoreView;
        private BoardView _boardView;

        #region Unity
        private void OnDestroy()
        {
            if (_boardView != null) _boardView.CascadeStarted -= RegisterScore;
        }
        #endregion

        public void Initialize(ScoreService scoreService, ScoreView scoreView, BoardView boardView)
        {
            _scoreService = scoreService;
            _scoreView = scoreView;
            _boardView = boardView;

            _boardView.CascadeStarted += RegisterScore;

            _scoreService.Reset();
            _scoreView.ResetScore(_scoreService.Score);
        }

        private void RegisterScore(BoardSequence sequence, int index)
        {
            int points = _scoreService.RegisterCascade(sequence.MatchedTypes, index);

            _scoreView.SetScore(_scoreService.Score);

            PointsScored?.Invoke(points, _boardView.GetMatchCenter(sequence.MatchedPosition));
        }
    }
}
