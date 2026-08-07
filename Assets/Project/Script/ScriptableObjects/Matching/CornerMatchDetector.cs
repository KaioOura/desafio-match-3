using System.Collections.Generic;
using Gazeus.DesafioMatch3.Core;
using Gazeus.DesafioMatch3.Models;
using UnityEngine;

namespace Gazeus.DesafioMatch3.ScriptableObjects.Matching
{
    // Two runs of the same type, one per axis, sharing a tile: the L, T and + shapes.
    [CreateAssetMenu(fileName = "CornerMatchDetector", menuName = "Gameplay/Matching/Corner")]
    public class CornerMatchDetector : MatchDetector
    {
        [Tooltip("Which crossings this detector recognizes.")]
        [SerializeField] private CornerShape _shapes = CornerShape.L | CornerShape.T | CornerShape.Plus;

        [Tooltip("Tiles in the whole crossing, counting the shared tile once: a run of three with " +
                 "one tile hanging off it makes 4, and the L of two runs of three makes 5.")]
        [SerializeField] private int _minimumTiles = 5;

        public override void Detect(BoardScan scan, List<Match> matches)
        {
            // j starts at i + 1: every pair of runs is looked at once.
            for (int i = 0; i < scan.Runs.Count; i++)
            {
                for (int j = i + 1; j < scan.Runs.Count; j++)
                {
                    Match corner = TryCreateCorner(scan.Runs[i], scan.Runs[j]);
                    if (corner != null) matches.Add(corner);
                }
            }
        }

        private Match TryCreateCorner(Match first, Match second)
        {
            // Size check before the geometric one: throws out most pairs for free. The
            // shortest possible arm is 2 tiles, the smallest run the scan hands out.
            if (first.Size + second.Size - 1 < _minimumTiles) return null;

            if (!TryFindCrossing(first, second, out Vector2Int crossing)) return null;
            if (!_shapes.HasFlag(ClassifyShape(first, second, crossing))) return null;

            // The crossing tile belongs to both runs: it counts once.
            int size = first.Size + second.Size - 1;

            List<Vector2Int> positions = new(size);
            positions.AddRange(first.Positions);
            for (int i = 0; i < second.Size; i++)
            {
                if (!second.Positions[i].Equals(crossing)) positions.Add(second.Positions[i]);
            }

            return new Match(first.Type, positions, crossing, MatchAxis.Both);
        }

        private CornerShape ClassifyShape(Match first, Match second, Vector2Int crossing)
        {
            int endPoints = 0;
            if (first.IsEndPoint(crossing)) endPoints++;
            if (second.IsEndPoint(crossing)) endPoints++;

            switch (endPoints)
            {
                case 2: return CornerShape.L;
                case 1: return CornerShape.T;
                default: return CornerShape.Plus;
            }
        }

        private bool TryFindCrossing(Match first, Match second, out Vector2Int crossing)
        {
            crossing = default;

            if (first.Type != second.Type) return false;
            if (first.Axis == second.Axis) return false;

            Match row = first.Axis == MatchAxis.Row ? first : second;
            Match column = first.Axis == MatchAxis.Row ? second : first;

            // The only tile a row and a column can share is where they meet.
            Vector2Int candidate = new(column.Origin.x, row.Origin.y);
            if (!row.Contains(candidate) || !column.Contains(candidate)) return false;

            crossing = candidate;

            return true;
        }
    }
}
