using UnityEngine;

namespace Gazeus.DesafioMatch3.ScriptableObjects
{
    [CreateAssetMenu(fileName = "TileTypeDefinition", menuName = "Gameplay/TileTypeDefinition")]
    public class TileTypeDefinition : ScriptableObject
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _points = 10;
        [SerializeField] private float _weight = 1f;

        public GameObject Prefab => _prefab;
        public int Points => _points;
        public float Weight => Mathf.Max(0f, _weight);
    }
}
