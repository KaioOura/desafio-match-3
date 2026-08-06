using UnityEngine;

namespace Gazeus.DesafioMatch3.ScriptableObjects
{
    [CreateAssetMenu(fileName = "TileTypeConfig", menuName = "Gameplay/TileTypeConfig")]
    public class TileTypeConfig : ScriptableObject
    {
        [SerializeField] private TileTypeDefinition[] _tileTypes;

        public int Count => _tileTypes.Length;

        public GameObject GetPrefab(int type)
        {
            TileTypeDefinition definition = GetDefinition(type);

            return definition == null ? null : definition.Prefab;
        }

        public int GetPoints(int type)
        {
            TileTypeDefinition definition = GetDefinition(type);

            return definition == null ? 0 : definition.Points;
        }

        public float GetWeight(int type)
        {
            TileTypeDefinition definition = GetDefinition(type);

            return definition == null ? 0f : definition.Weight;
        }

        private TileTypeDefinition GetDefinition(int type)
        {
            if (type < 0 || type >= _tileTypes.Length) return null;

            return _tileTypes[type];
        }
    }
}
