using System.Collections.Generic;
using Gazeus.DesafioMatch3.ScriptableObjects;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Core
{
    public class ScoreService
    {
        private readonly ScoreConfig _scoreConfig;
        private readonly TileTypeConfig _tileTypeConfig;

        public int Score { get; private set; }

        public ScoreService(ScoreConfig scoreConfig, TileTypeConfig tileTypeConfig)
        {
            _scoreConfig = scoreConfig;
            _tileTypeConfig = tileTypeConfig;
        }

        public void Reset()
        {
            Score = 0;
        }

        public int RegisterCascade(List<int> destroyedTypes, int cascadeIndex)
        {
            int points = CalculatePoints(destroyedTypes, cascadeIndex);
            Score += points;

            return points;
        }

        private int CalculatePoints(List<int> destroyedTypes, int cascadeIndex)
        {
            if (destroyedTypes == null || destroyedTypes.Count == 0) return 0;

            int tilePoints = 0;
            for (int i = 0; i < destroyedTypes.Count; i++)
            {
                tilePoints += _tileTypeConfig.GetPoints(destroyedTypes[i]);
            }

            int extraTiles = Mathf.Max(0, destroyedTypes.Count - _scoreConfig.MinimumMatchSize);
            float sizeBonus = 1f + extraTiles * _scoreConfig.BonusPerExtraTile;
            float comboMultiplier = 1f + cascadeIndex * _scoreConfig.ComboIncrementPerCascade;

            return Mathf.RoundToInt(tilePoints * sizeBonus * comboMultiplier);
        }
    }
}
