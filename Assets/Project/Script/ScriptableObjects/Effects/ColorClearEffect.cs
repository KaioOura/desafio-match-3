using Gazeus.DesafioMatch3.Models;
using UnityEngine;

namespace Gazeus.DesafioMatch3.ScriptableObjects.Effects
{
    [CreateAssetMenu(fileName = "ColorClearEffect", menuName = "Gameplay/Effects/Color Clear")]
    public class ColorClearEffect : MatchEffect
    {
        public override void Apply(Match match, Board board, ClearedTiles cleared)
        {
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    if (board[x, y].Type == match.Type) cleared.Add(x, y);
                }
            }
        }
    }
}
