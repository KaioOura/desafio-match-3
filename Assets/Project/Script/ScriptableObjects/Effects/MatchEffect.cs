using Gazeus.DesafioMatch3.Models;
using UnityEngine;

namespace Gazeus.DesafioMatch3.ScriptableObjects.Effects
{
    // A new mechanic means inheriting from here and creating an asset. The matched tiles
    // already go without help: here we add what the effect takes BESIDES them.
    public abstract class MatchEffect : ScriptableObject
    {
        public abstract void Apply(Match match, Board board, ClearedTiles cleared);
    }
}
