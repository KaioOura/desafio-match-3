using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;

namespace Gazeus.DesafioMatch3.Core.Pooling
{
    public class PrefabPool
    {
#if UNITY_EDITOR
        private const bool CollectionCheck = true;
#else
        private const bool CollectionCheck = false;
#endif

        private readonly GameObject _prefab;
        private readonly ObjectPool<GameObject> _pool;
        private readonly Transform _root;

        public PrefabPool(GameObject prefab, Transform root, int defaultCapacity = 16, int maxSize = 128)
        {
            _prefab = prefab;
            _root = root;
            _pool = new ObjectPool<GameObject>(
                Create,
                OnGet,
                OnRelease,
                OnDestroyInstance,
                CollectionCheck,
                defaultCapacity,
                maxSize);
        }
        
        public static Transform CreateRoot(Transform owner, string name = "[Pool]")
        {
            GameObject root = new GameObject(name);
            root.transform.SetParent(owner, false);
            root.SetActive(false);

            return root.transform;
        }

        public GameObject Get(Transform parent)
        {
            GameObject instance = _pool.Get();
            instance.transform.SetParent(parent, false);

            return instance;
        }

        public void Release(GameObject instance)
        {
            _pool.Release(instance);
        }

        public void Clear()
        {
            _pool.Clear();
        }

        private GameObject Create()
        {
            GameObject instance = Object.Instantiate(_prefab, _root);
            instance.SetActive(false);

            return instance;
        }

        private void OnGet(GameObject instance)
        {
            instance.transform.localScale = Vector3.one;
            instance.SetActive(true);
        }

        private void OnRelease(GameObject instance)
        {
            instance.transform.DOKill();
            instance.SetActive(false);
            instance.transform.SetParent(_root, false);
        }

        private void OnDestroyInstance(GameObject instance)
        {
            Object.Destroy(instance);
        }
    }
}
