using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Core
{
    public class GameService
    {
        private Board _board;
        private List<int> _tileTypes;

        public Board StartGame(int boardWidth, int boardHeight)
        {
            _tileTypes = new List<int> { 0, 1, 2, 3 };
            _board = CreateBoard(boardWidth, boardHeight);

            return _board;
        }

        public bool IsValidMovement(int fromX, int fromY, int toX, int toY)
        {
            _board.Swap(fromX, fromY, toX, toY);
            bool createsMatch = HasAnyMatch(_board);
            _board.Swap(fromX, fromY, toX, toY);

            return createsMatch;
        }

        public List<BoardSequence> SwapTile(int fromX, int fromY, int toX, int toY)
        {
            Board board = _board.Clone();
            board.Swap(fromX, fromY, toX, toY);

            List<BoardSequence> boardSequences = new();
            List<Vector2Int> matchedPositions = FindMatches(board);

            while (matchedPositions.Count > 0)
            {
                List<MovedTileInfo> movedTiles = new();
                List<AddedTileInfo> addedTiles = new();

                ClearTiles(board, matchedPositions);
                ApplyGravity(board, movedTiles);
                Refill(board, addedTiles);

                boardSequences.Add(new BoardSequence
                {
                    MatchedPosition = matchedPositions,
                    MovedTiles = movedTiles,
                    AddedTiles = addedTiles
                });

                matchedPositions = FindMatches(board);
            }

            _board = board;

            return boardSequences;
        }

        private Board CreateBoard(int width, int height)
        {
            Board board = new(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    List<int> noMatchTypes = new(_tileTypes);

                    if (x > 1 && board[x - 1, y].Type == board[x - 2, y].Type)
                    {
                        noMatchTypes.Remove(board[x - 1, y].Type);
                    }

                    if (y > 1 && board[x, y - 1].Type == board[x, y - 2].Type)
                    {
                        noMatchTypes.Remove(board[x, y - 1].Type);
                    }

                    int type = noMatchTypes[Random.Range(0, noMatchTypes.Count)];
                    board[x, y] = new Tile(type);
                }
            }

            return board;
        }

        private bool HasRunOfThree(Board board, int x, int y, int dx, int dy)
        {
            if (!board.Contains(x + dx * 2, y + dy * 2)) return false;

            int type = board[x, y].Type;

            return type == board[x + dx, y + dy].Type &&
                   type == board[x + dx * 2, y + dy * 2].Type;
        }

        private bool HasAnyMatch(Board board)
        {
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    if (HasRunOfThree(board, x, y, 1, 0)) return true;
                    if (HasRunOfThree(board, x, y, 0, 1)) return true;
                }
            }

            return false;
        }

        private List<Vector2Int> FindMatches(Board board)
        {
            bool[,] matched = new bool[board.Width, board.Height];

            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    MarkRun(board, matched, x, y, 1, 0);
                    MarkRun(board, matched, x, y, 0, 1);
                }
            }

            List<Vector2Int> matchedPositions = new();
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    if (matched[x, y]) matchedPositions.Add(new Vector2Int(x, y));
                }
            }

            return matchedPositions;
        }

        private void MarkRun(Board board, bool[,] matched, int x, int y, int dx, int dy)
        {
            if (!HasRunOfThree(board, x, y, dx, dy)) return;

            for (int i = 0; i < 3; i++)
            {
                matched[x + dx * i, y + dy * i] = true;
            }
        }

        private void ClearTiles(Board board, List<Vector2Int> positions)
        {
            for (int i = 0; i < positions.Count; i++)
            {
                Vector2Int position = positions[i];
                board[position.x, position.y] = Tile.Empty;
            }
        }

        private void ApplyGravity(Board board, List<MovedTileInfo> movedTiles)
        {
            for (int x = 0; x < board.Width; x++)
            {
                int write = board.Height - 1;

                for (int read = board.Height - 1; read >= 0; read--)
                {
                    if (board[x, read].IsEmpty) continue;

                    if (read != write)
                    {
                        board[x, write] = board[x, read];
                        board[x, read] = Tile.Empty;

                        movedTiles.Add(new MovedTileInfo
                        {
                            From = new Vector2Int(x, read),
                            To = new Vector2Int(x, write)
                        });
                    }

                    write--;
                }
            }
        }

        private void Refill(Board board, List<AddedTileInfo> addedTiles)
        {
            for (int y = board.Height - 1; y >= 0; y--)
            {
                for (int x = board.Width - 1; x >= 0; x--)
                {
                    if (!board[x, y].IsEmpty) continue;

                    int type = _tileTypes[Random.Range(0, _tileTypes.Count)];
                    board[x, y] = new Tile(type);

                    addedTiles.Add(new AddedTileInfo
                    {
                        Position = new Vector2Int(x, y),
                        Type = type
                    });
                }
            }
        }
    }
}
