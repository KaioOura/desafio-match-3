using System.Collections.Generic;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Models
{
    public class Match
    {
        public int Type { get; }
        public List<Vector2Int> Positions { get; }
        
        public Vector2Int Origin { get; }

        public MatchAxis Axis { get; }

        public int Size => Positions.Count;

        public Match(int type, List<Vector2Int> positions, Vector2Int origin, MatchAxis axis)
        {
            Type = type;
            Positions = positions;
            Origin = origin;
            Axis = axis;
        }

        public bool Contains(Vector2Int position)
        {
            return Positions.Contains(position);
        }

        public bool IsEndPoint(Vector2Int position)
        {
            return position.Equals(Positions[0]) || position.Equals(Positions[Size - 1]);
        }
    }
}
