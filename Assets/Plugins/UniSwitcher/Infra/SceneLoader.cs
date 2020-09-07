using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniSwitcher.Domain;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace UniSwitcher.Infra
{
    public delegate void OnProgressUpdateDelegate(float progress);
    
    /// <summary>
    /// Scene Loader (Injected)
    /// One instance per <see cref="Switcher"/>
    /// </summary>
    /// <remarks>
    /// Important Caveat: Loading the same scene multiple times is not supported.
    /// It might break load status detection
    /// </remarks>
    public class SceneLoader : MonoBehaviour, ISceneLoader
    {
        private string _currentScene;

        private OnProgressUpdateDelegate _progressUpdateDelegates;

        /// <summary>
        /// Dictionary holding all scenes (noted by path) are loaded.
        /// </summary>
        private Dictionary<string, bool> _loaded;

        private void Awake()
        {
            _loaded = new Dictionary<string, bool>();

            for (var i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                _loaded.Add(SceneUtility.GetScenePathByBuildIndex(i), false);
            }
            
            _currentScene = SceneManager.GetActiveScene().path;
            _loaded[_currentScene] = true;
        }

        /// <inheritdoc cref="ISceneLoader.LoadScene"/>
        public void LoadScene(IScene target, bool isAdditive = false)
        {
            SetStateToLoading(target, isAdditive);
            LoadSceneCoroutine(target, isAdditive).Forget(Debug.LogException);
        }

        /// <inheritdoc cref="ISceneLoader.LoadSceneWithDelay"/>
        public void LoadSceneWithDelay(IScene target, float time, bool isAdditive = false,
            CancellationToken cancellationToken = default)
        {
            SetStateToLoading(target, isAdditive);
            DelayMethod(time, () => { LoadScene(target, isAdditive); }, cancellationToken).Forget(e =>
            {
                if (e is OperationCanceledException)
                {
                    return;
                }

                Debug.LogException(e);
            });
        }

        /// <inheritdoc cref="ISceneLoader.UnloadScene"/>
        public void UnloadScene(IScene target)
        {
            UnloadSceneCoroutine(target).Forget(Debug.LogException);
        }

        /// <summary>
        /// Under-the-hood loading
        /// </summary>
        /// <param name="target"></param>
        /// <param name="isAdditive"></param>
        /// <returns></returns>
        private async UniTask LoadSceneCoroutine(IScene target, bool isAdditive)
        {
            var loadedLevel = SceneManager.LoadSceneAsync(
                target.GetRawValue(),
                isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single
            );
            while (!loadedLevel.isDone)
            {
                _progressUpdateDelegates?.Invoke(loadedLevel.progress);
                await UniTask.Yield();
            }

            SceneManager.SetActiveScene(SceneManager.GetSceneByPath(target.GetRawValue()));
            _loaded[target.GetRawValue()] = true;
            _currentScene = target.GetRawValue();
        }

        private async UniTask UnloadSceneCoroutine(IScene target)
        {
            var unloadedLevel = SceneManager.UnloadSceneAsync(
                target.GetRawValue()
            );
            while (!unloadedLevel.isDone)
            {
                _progressUpdateDelegates?.Invoke(unloadedLevel.progress);
                await UniTask.Yield();
            }

            _loaded[target.GetRawValue()] = false;
            _currentScene = SceneManager.GetActiveScene().path;
        }

        /// <inheritdoc cref="ISceneLoader.GetCurrentScene"/>
        public string GetCurrentScene()
        {
            return _currentScene;
        }

        /// <inheritdoc cref="ISceneLoader.GetCurrentSceneAsScene"/>
        public Scene GetCurrentSceneAsScene()
        {
            return SceneManager.GetActiveScene();
        }

        /// <inheritdoc cref="ISceneLoader.IsLoaded"/>
        public bool IsLoaded(IScene target)
        {
            try
            {
                return _loaded[target.GetRawValue()];
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        /// <inheritdoc cref="ISceneLoader.AddProgressUpdateDelegate"/>
        public void AddProgressUpdateDelegate(OnProgressUpdateDelegate @delegate)
        {
            _progressUpdateDelegates += @delegate;
        }

        public void ResetProgressUpdateDelegate()
        {
            _progressUpdateDelegates = null;
        }

        public void AddChangeDelegate(UnityAction<Scene, Scene> action)
        {
            SceneManager.activeSceneChanged += action;
        }

        public void AddSceneLoadedDelegate(UnityAction<Scene, LoadSceneMode> action)
        {
            SceneManager.sceneLoaded += action;
        }

        public void RemoveChangeDelegate(UnityAction<Scene, Scene> action)
        {
            SceneManager.activeSceneChanged -= action;
        }

        public void RemoveSceneLoadedDelegate(UnityAction<Scene, LoadSceneMode> action)
        {
            SceneManager.sceneLoaded -= action;
        }

        private static async UniTask DelayMethod(float wait, Action action,
            CancellationToken cancellationToken = default)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(wait), cancellationToken: cancellationToken);
            action();
        }

        private void SetStateToLoading(IScene destination, bool isAdditive)
        {
            if (!isAdditive)
            {
                var loadedTemp = _loaded;
                // If single mode, everything gets unloaded, so everything is set as 'not loaded'
                foreach (var scene in loadedTemp.Keys.ToList())
                {
                    _loaded[scene] = false;
                }
            }
            else
            {
                // If additive mode, that scene only is set as 'not loaded'
                _loaded[destination.GetRawValue()] = false;
            }
        }
    }
}