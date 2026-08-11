using UnityEngine;

namespace Gazeus.DesafioMatch3.ScriptableObjects.Feedback
{
    [CreateAssetMenu(fileName = "FeedbackConfig", menuName = "Feedback/FeedbackConfig")]
    public class FeedbackConfig : ScriptableObject
    {
        [Header("Player input")]
        [SerializeField] private EffectCue _select;
        [SerializeField] private EffectCue _deselect;
        [SerializeField] private EffectCue _invalidSwap;

        [Header("Board")]
        [SerializeField] private EffectCue _match;
        [SerializeField] private EffectCue _spawn;

        [Header("Session")]
        [SerializeField] private EffectCue _hint;
        [SerializeField] private EffectCue _gameOver;

        public EffectCue Select => _select;
        public EffectCue Deselect => _deselect;
        public EffectCue InvalidSwap => _invalidSwap;
        public EffectCue Match => _match;
        public EffectCue Spawn => _spawn;
        public EffectCue Hint => _hint;
        public EffectCue GameOver => _gameOver;
    }
}
