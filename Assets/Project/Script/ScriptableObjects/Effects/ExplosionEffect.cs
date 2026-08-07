using Gazeus.DesafioMatch3.Models;
using UnityEngine;

namespace Gazeus.DesafioMatch3.ScriptableObjects.Effects
{
    [CreateAssetMenu(fileName = "ExplosionEffect", menuName = "Gameplay/Effects/Explosion")]
    public class ExplosionEffect : MatchEffect
    {
        [Tooltip("Side of the destroyed area, in tiles: 3 makes a 3x3 around the origin. " +
                 "An even value rounds down, so the area stays centered.")]
        [SerializeField] private int _size = 3;

        private int Radius => Mathf.Max(0, (_size - 1) / 2);

        public override void Apply(Match match, Board board, ClearedTiles cleared)
        {
            int radius = Radius;

            for (int y = match.Origin.y - radius; y <= match.Origin.y + radius; y++)
            {
                for (int x = match.Origin.x - radius; x <= match.Origin.x + radius; x++)
                {
                    if (board.Contains(x, y)) cleared.Add(x, y);
                }
            }
        }
    }
}
