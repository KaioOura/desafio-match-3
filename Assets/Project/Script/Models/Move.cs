using UnityEngine;

namespace Gazeus.DesafioMatch3.Models
{
    public readonly struct Move
    {
        public Vector2Int From { get; }
        public Vector2Int To { get; }

        public Move(Vector2Int from, Vector2Int to)
        {
            From = from;
            To = to;
        }
    }
}
