using Gazeus.DesafioMatch3.Models;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Core
{
    public class MoveFinder
    {
        private readonly MatchResolver _matchResolver;

        public MoveFinder(MatchResolver matchResolver)
        {
            _matchResolver = matchResolver;
        }

        public bool TryFindMove(Board board, out Move move)
        {
            int cellCount = board.Width * board.Height;
            int start = UnityEngine.Random.Range(0, cellCount);

            for (int i = 0; i < cellCount; i++)
            {
                int cell = (start + i) % cellCount;
                int x = cell % board.Width;
                int y = cell / board.Width;

                if (!IsMovable(board, x, y)) continue;

                if (CreatesMatch(board, x, y, x + 1, y, out move)) return true;
                if (CreatesMatch(board, x, y, x, y + 1, out move)) return true;
            }

            move = default;

            return false;
        }

        private bool CreatesMatch(Board board, int fromX, int fromY, int toX, int toY, out Move move)
        {
            move = default;

            if (!board.Contains(toX, toY)) return false;
            if (!IsMovable(board, toX, toY)) return false;
            if (board[fromX, fromY].Type == board[toX, toY].Type) return false;

            board.Swap(fromX, fromY, toX, toY);
            bool createsMatch = _matchResolver.HasAnyMatch(board);
            board.Swap(fromX, fromY, toX, toY);

            if (!createsMatch) return false;

            move = new Move(new Vector2Int(fromX, fromY), new Vector2Int(toX, toY));

            return true;
        }

        private bool IsMovable(Board board, int x, int y)
        {
            return !board.IsDead(x, y) && !board[x, y].IsEmpty;
        }
    }
}
