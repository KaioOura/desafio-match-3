using System.Collections.Generic;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Core.Pooling
{
    public class PrefabPoolRegistry
    {
        private readonly Dictionary<GameObject, PrefabPool> _poolByInstance = new();
        private readonly Dictionary<GameObject, PrefabPool> _poolByPrefab = new();
        private readonly Transform _root;

        public PrefabPoolRegistry(Transform owner, string rootName = "[Pool]")
        {
            _root = PrefabPool.CreateRoot(owner, rootName);
        }

        public GameObject Get(GameObject prefab, Transform parent)
        {
            PrefabPool pool = GetPool(prefab);

            GameObject instance = pool.Get(parent);
            _poolByInstance[instance] = pool;

            return instance;
        }

        public void Release(GameObject instance)
        {
            if (!_poolByInstance.TryGetValue(instance, out PrefabPool pool))
            {
                Debug.LogWarning($"{instance.name} did not come from this registry; destroying it instead of leaking.", instance);
                Object.Destroy(instance);

                return;
            }

            pool.Release(instance);
        }

        public void Clear()
        {
            foreach (PrefabPool pool in _poolByPrefab.Values)
            {
                pool.Clear();
            }

            _poolByInstance.Clear();
        }

        private PrefabPool GetPool(GameObject prefab)
        {
            if (_poolByPrefab.TryGetValue(prefab, out PrefabPool pool)) return pool;

            pool = new PrefabPool(prefab, PrefabPool.CreateRoot(_root, prefab.name));
            _poolByPrefab[prefab] = pool;

            return pool;
        }
    }
}
