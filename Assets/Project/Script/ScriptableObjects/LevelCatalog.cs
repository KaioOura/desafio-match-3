using System.Collections.Generic;
using UnityEngine;

namespace Gazeus.DesafioMatch3.ScriptableObjects
{
    [CreateAssetMenu(fileName = "LevelCatalog", menuName = "Gameplay/LevelCatalog")]
    public class LevelCatalog : ScriptableObject
    {
        [SerializeField] private List<LevelConfig> _levels = new();

        public IReadOnlyList<LevelConfig> Levels => _levels;
    }
}
