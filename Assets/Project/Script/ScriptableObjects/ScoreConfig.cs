using UnityEngine;

namespace Gazeus.DesafioMatch3.ScriptableObjects
{
    [CreateAssetMenu(fileName = "ScoreConfig", menuName = "Gameplay/ScoreConfig")]
    public class ScoreConfig : ScriptableObject
    {
        [SerializeField] private int _minimumMatchSize = 3;
        [SerializeField] private float _bonusPerExtraTile = 0.5f;
        [SerializeField] private float _comboIncrementPerCascade = 1f;

        public int MinimumMatchSize => _minimumMatchSize;
        public float BonusPerExtraTile => _bonusPerExtraTile;
        public float ComboIncrementPerCascade => _comboIncrementPerCascade;
    }
}
