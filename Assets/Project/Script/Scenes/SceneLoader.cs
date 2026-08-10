using System.Collections;
using DG.Tweening;
using Gazeus.DesafioMatch3.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gazeus.DesafioMatch3.Scenes
{
    public class SceneLoader : PersistentSingleton<SceneLoader>
    {
        [SerializeField] private string _loadingSceneName = SceneNames.Loading;
        [Tooltip("Shortest time the loading screen stays up, so quick loads do not flash.")]
        [SerializeField] private float _minimumDuration = 0.6f;

        private LoadingScreen _loadingScreen;

        public bool IsLoading { get; private set; }

        public void Load(string sceneName)
        {
            if (IsLoading)
            {
                Debug.LogWarning($"SceneLoader is already loading a scene, ignoring request for '{sceneName}'.", this);
                return;
            }

            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("SceneLoader received an empty scene name.", this);
                return;
            }

            StartCoroutine(LoadRoutine(sceneName));
        }

        private IEnumerator PrepareLoadingScreen()
        {
            Scene scene = SceneManager.GetSceneByName(_loadingSceneName);
            if (!scene.isLoaded)
            {
                yield return SceneManager.LoadSceneAsync(_loadingSceneName, LoadSceneMode.Additive);
                scene = SceneManager.GetSceneByName(_loadingSceneName);
            }

            _loadingScreen = FindLoadingScreen(scene);

            if (_loadingScreen == null)
            {
                Debug.LogError($"Scene '{_loadingSceneName}' has no LoadingScreen component.", this);
            }
        }

        private IEnumerator LoadRoutine(string sceneName)
        {
            IsLoading = true;

            if (_loadingScreen == null) yield return PrepareLoadingScreen();

            if (_loadingScreen == null)
            {
                IsLoading = false;
                yield break;
            }

            Scene previousScene = SceneManager.GetActiveScene();
            Scene loadingScene = _loadingScreen.gameObject.scene;

            _loadingScreen.SetProgress(0f);
            _loadingScreen.SetVisible(true);

            yield return WaitFor(_loadingScreen.FadeIn());

            float startTime = Time.unscaledTime;

            SceneManager.SetActiveScene(loadingScene);
            yield return SceneManager.UnloadSceneAsync(previousScene);
            yield return Resources.UnloadUnusedAssets();

            AsyncOperation load = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!load.isDone)
            {
                _loadingScreen.SetProgress(DisplayedProgress(load.progress, startTime));
                yield return null;
            }

            while (Time.unscaledTime - startTime < _minimumDuration)
            {
                _loadingScreen.SetProgress(DisplayedProgress(1f, startTime));
                yield return null;
            }

            _loadingScreen.SetProgress(1f);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

            yield return null;

            yield return WaitFor(_loadingScreen.FadeOut());
            _loadingScreen.SetVisible(false);

            IsLoading = false;
        }

        private static IEnumerator WaitFor(Tween tween)
        {
            bool completed = false;
            tween.onComplete += () => completed = true;

            while (!completed) yield return null;
        }

        private float DisplayedProgress(float loadProgress, float startTime)
        {
            if (_minimumDuration <= 0f) return loadProgress;

            return Mathf.Max(loadProgress, (Time.unscaledTime - startTime) / _minimumDuration);
        }

        private static LoadingScreen FindLoadingScreen(Scene scene)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                LoadingScreen screen = root.GetComponentInChildren<LoadingScreen>(true);
                if (screen != null) return screen;
            }

            return null;
        }
    }
}
