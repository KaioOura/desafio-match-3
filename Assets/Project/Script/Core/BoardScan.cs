using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Core
{
    public class BoardScan
    {
        // That's the short arm of an L of four. Detectors needing
        // longer runs filter on their own.
        private const int MinimumRunLength = 2;

        public Board Board { get; }
        public List<Match> Runs { get; }

        public BoardScan(Board board)
        {
            Board = board;
            Runs = new List<Match>();

            AddRunsFoundInRows();
            AddRunsFoundInColumns();
        }

        private void AddRunsFoundInRows()
        {
            for (int y = 0; y < Board.Height; y++)
            {
                int x = 0;
                while (x < Board.Width)
                {
                    int length = CountMatchingTilesFrom(x, y, 1, 0);
                    if (length >= MinimumRunLength) Runs.Add(CreateRunFrom(x, y, 1, 0, length));

                    x += length;
                }
            }
        }

        private void AddRunsFoundInColumns()
        {
            for (int x = 0; x < Board.Width; x++)
            {
                int y = 0;
                while (y < Board.Height)
                {
                    int length = CountMatchingTilesFrom(x, y, 0, 1);
                    if (length >= MinimumRunLength) Runs.Add(CreateRunFrom(x, y, 0, 1, length));

                    y += length;
                }
            }
        }

        // Never returns zero, so the cursor above always moves forward.
        private int CountMatchingTilesFrom(int x, int y, int stepX, int stepY)
        {
            if (Board[x, y].IsEmpty) return 1;

            int type = Board[x, y].Type;
            int length = 1;

            while (Board.Contains(x + stepX * length, y + stepY * length) &&
                   Board[x + stepX * length, y + stepY * length].Type == type)
            {
                length++;
            }

            return length;
        }

        private Match CreateRunFrom(int x, int y, int stepX, int stepY, int length)
        {
            List<Vector2Int> positions = new(length);
            for (int i = 0; i < length; i++)
            {
                positions.Add(new Vector2Int(x + stepX * i, y + stepY * i));
            }

            bool isHorizontal = stepX != 0;

            return new Match(Board[x, y].Type, positions, positions[length / 2],
                isHorizontal ? MatchAxis.Row : MatchAxis.Column);
        }
    }
}
