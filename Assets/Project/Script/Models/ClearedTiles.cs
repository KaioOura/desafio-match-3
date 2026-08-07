using System.Collections.Generic;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Models
{
    public class ClearedTiles
    {
        private readonly bool[,] _alreadyAdded;

        public List<Vector2Int> Positions { get; }

        public ClearedTiles(int width, int height)
        {
            _alreadyAdded = new bool[width, height];
            Positions = new List<Vector2Int>();
        }

        public void Add(int x, int y)
        {
            //Needed otherwise the same tile would be scored twice
            if (_alreadyAdded[x, y]) return;

            _alreadyAdded[x, y] = true;
            Positions.Add(new Vector2Int(x, y));
        }

        public void Add(List<Vector2Int> positions)
        {
            for (int i = 0; i < positions.Count; i++)
            {
                Vector2Int position = positions[i];
                Add(position.x, position.y);
            }
        }
    }
}
