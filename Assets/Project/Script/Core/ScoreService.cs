using UnityEngine;

namespace Gazeus.DesafioMatch3.Core
{
    public class ScoreService
    {
        private const int BasePointsPerTile = 10;
        private const int MinimumMatchSize = 3;
        private const float BonusPerExtraTile = 0.5f;

        public int Score { get; private set; }

        public void Reset()
        {
            Score = 0;
        }

        public int RegisterCascade(int destroyedTileCount, int cascadeIndex)
        {
            int points = CalculatePoints(destroyedTileCount, cascadeIndex);
            Score += points;

            return points;
        }

        private int CalculatePoints(int destroyedTileCount, int cascadeIndex)
        {
            if (destroyedTileCount <= 0) return 0;

            int extraTiles = Mathf.Max(0, destroyedTileCount - MinimumMatchSize);
            float sizeBonus = 1f + extraTiles * BonusPerExtraTile;
            int comboMultiplier = cascadeIndex + 1;

            return Mathf.RoundToInt(BasePointsPerTile * destroyedTileCount * sizeBonus) * comboMultiplier;
        }
    }
}
