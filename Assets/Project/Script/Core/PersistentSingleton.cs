using UnityEngine;

namespace Gazeus.DesafioMatch3.Core
{
    public abstract class PersistentSingleton<T> : MonoBehaviour where T : PersistentSingleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;

                _instance = FindFirstObjectByType<T>();
                if (_instance != null) return _instance;

                _instance = new GameObject(typeof(T).Name).AddComponent<T>();

                return _instance;
            }
        }

        #region Unity
        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = (T)this;
            DontDestroyOnLoad(gameObject);
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this) _instance = null;
        }
        #endregion
    }
}
