using System.Collections.Generic;
using Gazeus.DesafioMatch3.Core;
using Gazeus.DesafioMatch3.Models;
using UnityEngine;

namespace Gazeus.DesafioMatch3.ScriptableObjects.Matching
{
    [CreateAssetMenu(fileName = "SquareMatchDetector", menuName = "Gameplay/Matching/Square")]
    public class SquareMatchDetector : MatchDetector
    {
        [Tooltip("Block side, in tiles. 2 makes the classic 2x2 square.")]
        [SerializeField] private int _size = 2;

        public override void Detect(BoardScan scan, List<Match> matches)
        {
            Board board = scan.Board;
            int size = Mathf.Max(2, _size);

            for (int y = 0; y + size <= board.Height; y++)
            {
                for (int x = 0; x + size <= board.Width; x++)
                {
                    if (IsUniformBlock(board, x, y, size)) matches.Add(CreateSquare(board, x, y, size));
                }
            }
        }

        private bool IsUniformBlock(Board board, int originX, int originY, int size)
        {
            if (board[originX, originY].IsEmpty) return false;

            int type = board[originX, originY].Type;

            for (int y = originY; y < originY + size; y++)
            {
                for (int x = originX; x < originX + size; x++)
                {
                    if (board[x, y].Type != type) return false;
                }
            }

            return true;
        }
        
        private Match CreateSquare(Board board, int originX, int originY, int size)
        {
            List<Vector2Int> positions = new(size * size);

            for (int y = originY; y < originY + size; y++)
            {
                for (int x = originX; x < originX + size; x++)
                {
                    positions.Add(new Vector2Int(x, y));
                }
            }
            
            return new Match(board[originX, originY].Type, positions, positions[0], MatchAxis.Both);
        }
    }
}
