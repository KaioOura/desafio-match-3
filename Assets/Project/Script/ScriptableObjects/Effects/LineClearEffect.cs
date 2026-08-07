using Gazeus.DesafioMatch3.Models;
using UnityEngine;

namespace Gazeus.DesafioMatch3.ScriptableObjects.Effects
{
    // Clears the lines crossing the match Origin. On a run that's its own line; on a
    // corner the row and the column make a cross.
    [CreateAssetMenu(fileName = "LineClearEffect", menuName = "Gameplay/Effects/Line Clear")]
    public class LineClearEffect : MatchEffect
    {
        public override void Apply(Match match, Board board, ClearedTiles cleared)
        {
            if (match.Axis != MatchAxis.Column)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    cleared.Add(x, match.Origin.y);
                }
            }

            if (match.Axis != MatchAxis.Row)
            {
                for (int y = 0; y < board.Height; y++)
                {
                    cleared.Add(match.Origin.x, y);
                }
            }
        }
    }
}
