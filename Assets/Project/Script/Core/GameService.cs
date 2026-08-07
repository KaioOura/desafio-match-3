using System;
using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.ScriptableObjects;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Core
{
    public class GameService
    {
        private const int MaxRedrawAttempts = 20;
        private const int MaxBoardAttempts = 5;

        private readonly TileTypeConfig _tileTypeConfig;
        private readonly MatchResolver _matchResolver;
        private readonly List<int> _drawCandidates = new();

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
            for (int attempt = 0; attempt < MaxBoardAttempts; attempt++)
            {
                Board board = DrawBoard(width, height);
                RedrawMatchedTiles(board, null);

                if (!_matchResolver.HasAnyMatch(board)) return board;
            }

            throw new InvalidOperationException(
                $"Could not build a {width}x{height} board free of matches out of {_tileTypes.Count} tile " +
                "types. Add types to the TileTypeConfig, loosen the rules in the SpecialMatchConfig, " +
                "or use a smaller board.");
        }

        private Board DrawBoard(int width, int height)
        {
            Board board = new(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    board[x, y] = new Tile(PickRandomType(_tileTypes));
                }
            }

            return board;
        }
        
        private void RedrawMatchedTiles(Board board, bool[,] redrawable)
        {
            for (int attempt = 0; attempt < MaxRedrawAttempts; attempt++)
            {
                List<Match> matches = _matchResolver.FindMatches(board);
                if (matches.Count == 0) return;

                bool[,] redrawnThisPass = new bool[board.Width, board.Height];

                bool redrewAny = false;
                for (int i = 0; i < matches.Count; i++)
                {
                    if (TryRedrawOneTile(board, matches[i], redrawable, redrawnThisPass)) redrewAny = true;
                }

                if (!redrewAny) return;
            }
        }

        private bool TryRedrawOneTile(Board board, Match match, bool[,] redrawable, bool[,] redrawnThisPass)
        {
            for (int i = 0; i < match.Size; i++)
            {
                Vector2Int position = match.Positions[i];

                if (redrawnThisPass[position.x, position.y]) continue;
                if (redrawable != null && !redrawable[position.x, position.y]) continue;

                _drawCandidates.Clear();
                _drawCandidates.AddRange(_tileTypes);
                _drawCandidates.Remove(board[position.x, position.y].Type);
                if (_drawCandidates.Count == 0) return false;

                board[position.x, position.y] = new Tile(PickRandomType(_drawCandidates));
                redrawnThisPass[position.x, position.y] = true;

                return true;
            }

            return false;
        }

        private int PickRandomType(List<int> candidates)
        {
            float totalWeight = 0f;
            for (int i = 0; i < candidates.Count; i++)
            {
                totalWeight += _tileTypeConfig.GetWeight(candidates[i]);
            }
            
            if (totalWeight <= 0f) return candidates[UnityEngine.Random.Range(0, candidates.Count)];

            float roll = UnityEngine.Random.value * totalWeight;
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
            bool[,] drawnNow = new bool[board.Width, board.Height];

            for (int y = board.Height - 1; y >= 0; y--)
            {
                for (int x = board.Width - 1; x >= 0; x--)
                {
                    if (!board[x, y].IsEmpty) continue;

                    board[x, y] = new Tile(PickRandomType(_tileTypes));
                    drawnNow[x, y] = true;
                }
            }

            RedrawMatchedTiles(board, drawnNow);

            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    if (!drawnNow[x, y]) continue;

                    addedTiles.Add(new AddedTileInfo
                    {
                        Position = new Vector2Int(x, y),
                        Type = board[x, y].Type
                    });
                }
            }
        }
    }
}
