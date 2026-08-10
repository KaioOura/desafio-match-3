using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.ScriptableObjects;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Core
{
    public class MatchResolver
    {
        private readonly SpecialMatchConfig _specialMatchConfig;
        private readonly List<Match> _matches = new();

        public MatchResolver(SpecialMatchConfig specialMatchConfig)
        {
            _specialMatchConfig = specialMatchConfig;
        }

        public bool HasAnyMatch(Board board)
        {
            return FindMatches(board).Count > 0;
        }
        
        public List<Match> FindMatches(Board board)
        {
            BoardScan scan = new(board);
            MatchRule[] rules = _specialMatchConfig.Rules;

            _matches.Clear();
            for (int i = 0; i < rules.Length; i++)
            {
                if (rules[i].Detector != null) rules[i].Detector.Detect(scan, _matches);
            }

            return _matches;
        }
        
        public List<Vector2Int> FindPositionsToClear(Board board)
        {
            return FindPositionsToClear(board, null);
        }
        
        public List<Vector2Int> FindPositionsToClear(Board board, Vector2Int? playerTile)
        {
            BoardScan scan = new(board);
            MatchRule[] rules = _specialMatchConfig.Rules;

            ClearedTiles cleared = new(board);
            bool[,] resolved = new bool[board.Width, board.Height];
            List<Match> matches = new();

            for (int i = 0; i < rules.Length; i++)
            {
                MatchRule rule = rules[i];
                if (rule.Detector == null) continue;

                matches.Clear();
                rule.Detector.Detect(scan, matches);

                for (int j = 0; j < matches.Count; j++)
                {
                    ApplyRule(rule, AnchorToPlayerTile(matches[j], playerTile), board, cleared, resolved);
                }
            }

            return cleared.Positions;
        }
        
        private Match AnchorToPlayerTile(Match match, Vector2Int? playerTile)
        {
            if (playerTile == null) return match;
            if (!match.Contains(playerTile.Value)) return match;

            return new Match(match.Type, match.Positions, playerTile.Value, match.Axis);
        }

        private void ApplyRule(MatchRule rule, Match match, Board board, ClearedTiles cleared, bool[,] resolved)
        {
            cleared.Add(match.Positions);

            if (rule.Effect == null) return;
            if (!_specialMatchConfig.StackEffects && IsResolved(match, resolved)) return;

            rule.Effect.Apply(match, board, cleared);
            MarkResolved(match, resolved);
        }
        
        private bool IsResolved(Match match, bool[,] resolved)
        {
            for (int i = 0; i < match.Size; i++)
            {
                Vector2Int position = match.Positions[i];
                if (resolved[position.x, position.y]) return true;
            }

            return false;
        }

        private void MarkResolved(Match match, bool[,] resolved)
        {
            for (int i = 0; i < match.Size; i++)
            {
                Vector2Int position = match.Positions[i];
                resolved[position.x, position.y] = true;
            }
        }
    }
}
