using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.ScriptableObjects;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Core
{
    public class GameService
    {
        private const int MaxBoardRerolls = 20;

        private readonly TileTypeConfig _tileTypeConfig;
        private readonly MatchResolver _matchResolver;

        private Board _board;
        private List<int> _tileTypes;

        public GameService(TileTypeConfig tileTypeConfig, SpecialMatchConfig specialMatchConfig)
        {
            _tileTypeConfig = tileTypeConfig;
            _matchResolver = new MatchResolver(specialMatchConfig);
        }

        public Board StartGame(int boardWidth, int boardHeight)
        {
            _tileTypes = new List<int>(_tileTypeConfig.Count);
            for (int type = 0; type < _tileTypeConfig.Count; type++)
            {
                _tileTypes.Add(type);
            }

            _board = CreateBoard(boardWidth, boardHeight);

            return _board;
        }

        public bool IsValidMovement(int fromX, int fromY, int toX, int toY)
        {
            _board.Swap(fromX, fromY, toX, toY);
            bool createsMatch = _matchResolver.HasAnyMatch(_board);
            _board.Swap(fromX, fromY, toX, toY);

            return createsMatch;
        }

        public List<BoardSequence> SwapTile(int fromX, int fromY, int toX, int toY)
        {
            Board board = _board.Clone();
            board.Swap(fromX, fromY, toX, toY);

            List<BoardSequence> boardSequences = new();
            
            List<Vector2Int> matchedPositions = _matchResolver.FindPositionsToClear(board, new Vector2Int(toX, toY));

            while (matchedPositions.Count > 0)
            {
                List<MovedTileInfo> movedTiles = new();
                List<AddedTileInfo> addedTiles = new();

                List<int> matchedTypes = CollectTypes(board, matchedPositions);

                ClearTiles(board, matchedPositions);
                ApplyGravity(board, movedTiles);
                Refill(board, addedTiles);

                boardSequences.Add(new BoardSequence
                {
                    MatchedPosition = matchedPositions,
                    MatchedTypes = matchedTypes,
                    MovedTiles = movedTiles,
                    AddedTiles = addedTiles
                });

                matchedPositions = _matchResolver.FindPositionsToClear(board);
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

                    int type = PickRandomType(noMatchTypes);
                    board[x, y] = new Tile(type);
                }
            }

            RerollLeftoverMatches(board);

            return board;
        }
        
        private void RerollLeftoverMatches(Board board)
        {
            for (int attempt = 0; attempt < MaxBoardRerolls; attempt++)
            {
                List<Match> matches = _matchResolver.FindMatches(board);
                if (matches.Count == 0) return;

                for (int i = 0; i < matches.Count; i++)
                {
                    Vector2Int position = matches[i].Origin;

                    List<int> otherTypes = new(_tileTypes);
                    otherTypes.Remove(board[position.x, position.y].Type);
                    if (otherTypes.Count == 0) continue;

                    board[position.x, position.y] = new Tile(PickRandomType(otherTypes));
                }
            }
        }

        private int PickRandomType(List<int> candidates)
        {
            float totalWeight = 0f;
            for (int i = 0; i < candidates.Count; i++)
            {
                totalWeight += _tileTypeConfig.GetWeight(candidates[i]);
            }
            
            if (totalWeight <= 0f) return candidates[Random.Range(0, candidates.Count)];

            float roll = Random.value * totalWeight;
            for (int i = 0; i < candidates.Count; i++)
            {
                roll -= _tileTypeConfig.GetWeight(candidates[i]);
                if (roll < 0f) return candidates[i];
            }

            return candidates[candidates.Count - 1];
        }

        private List<int> CollectTypes(Board board, List<Vector2Int> positions)
        {
            List<int> types = new(positions.Count);
            for (int i = 0; i < positions.Count; i++)
            {
                Vector2Int position = positions[i];
                types.Add(board[position.x, position.y].Type);
            }

            return types;
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

                    int type = PickRandomType(_tileTypes);
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
