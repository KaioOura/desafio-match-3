using Gazeus.DesafioMatch3.Controllers;
using Gazeus.DesafioMatch3.ScriptableObjects;
using Gazeus.DesafioMatch3.ScriptableObjects.Feedback;
using Gazeus.DesafioMatch3.Views;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Core
{
    [DefaultExecutionOrder(-100)]
    public class GameplayInstaller : MonoBehaviour
    {
        [Header("Views")]
        [SerializeField] private BoardView _boardView;
        [SerializeField] private ScoreView _scoreView;
        [SerializeField] private EffectsView _effectsView;

        [Header("Controllers")]
        [SerializeField] private GameController _game;
        [SerializeField] private HintController _hint;
        [SerializeField] private GameOverController _gameOver;
        [SerializeField] private FeedbackController _feedback;

        [Header("Configs")]
        [SerializeField] private TileTypeConfig _tileTypes;
        [SerializeField] private SpecialMatchConfig _specialMatches;
        [SerializeField] private ScoreConfig _scoreConfig;
        [SerializeField] private FeedbackConfig _feedbackConfig;

        [Header("Scene")]
        [SerializeField] private ButtonView _hintButton;
        [SerializeField] private ScreenDefinition _gameOverScreen;
        [SerializeField] private LevelConfig _fallbackLevel;

        #region Unity
        private void Awake()
        {
            LevelConfig level = LevelSession.Current != null ? LevelSession.Current : _fallbackLevel;
            if (level == null)
            {
                Debug.LogError("GameplayInstaller has no LevelConfig to start from.", this);
                return;
            }

            GameService gameService = new(_tileTypes, _specialMatches);
            ScoreService scoreService = new(_scoreConfig, _tileTypes);

            _boardView.Initialize(_tileTypes);
            _game.Initialize(gameService, scoreService, _boardView, _scoreView, level);
            _hint.Initialize(_game, _boardView, _hintButton);
            _gameOver.Initialize(_game, _gameOverScreen);
            _feedback.Initialize(_game, _boardView, _hint, _effectsView, _feedbackConfig);
        }
        #endregion
    }
}
