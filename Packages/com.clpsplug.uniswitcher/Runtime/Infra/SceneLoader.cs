using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniSwitcher.Domain;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Zenject;
#if UGS_ANALYTICS
using Unity.Services.Core;
using Unity.Services.Analytics;
#endif

namespace UniSwitcher.Infra
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="progress"></param>
    public delegate void OnProgressUpdateDelegate(float progress);

    /// <summary>
    /// <see cref="ISceneLoader"/> implementation
    /// One instance per <see cref="Switcher"/>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Important Caveat: Loading the same scene multiple times additively is not supported.
    /// It might break load status detection.
    /// </para>
    /// <para>
    /// It is totally fine to single-load the same scene.
    /// </para>
    /// </remarks>
    public class SceneLoader : MonoBehaviour, ISceneLoader
    {
        private string _currentScene;

        private OnProgressUpdateDelegate _progressUpdateDelegates;

        /// <summary>
        /// Dictionary holding all scenes (noted by path) are loaded.
        /// </summary>
        private Dictionary<string, bool> _loaded;

#pragma warning disable 0649
        [Inject] private ZenjectSceneLoader _sceneLoader;
#pragma warning restore 0649

        [Inject]
        private void Construct()
        {
            _loaded = new Dictionary<string, bool>();

            for (var i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                _loaded.Add(SceneUtility.GetScenePathByBuildIndex(i), false);
            }

            _currentScene = SceneManager.GetActiveScene().path;
            _loaded[_currentScene] = true;
        }

        /// <inheritdoc cref="ISceneLoader.LoadScene{T}"/>
        public void LoadScene<T>(IScene target, bool isAdditive, T sceneData)
        {
            SetStateToLoading(target, isAdditive);
            LoadSceneAsync(target, isAdditive, sceneData).Forget(Debug.LogException);
        }

        /// <inheritdoc cref="ISceneLoader.LoadSceneWithDelay{T}"/>
        public void LoadSceneWithDelay<T>(IScene target, float time, bool isAdditive, T sceneData,
            CancellationToken cancellationToken = default)
        {
            SetStateToLoading(target, isAdditive);
            DelayMethod(time, () => { LoadScene(target, isAdditive, sceneData); }, cancellationToken).Forget(e =>
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
            UnloadSceneAsync(target).Forget(Debug.LogException);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// Under-the-hood loading
        /// </summary>
        /// <param name="target"></param>
        /// <param name="isAdditive"></param>
        /// <param name="sceneData"></param>
        /// <returns>UniTask</returns>
        private async UniTask LoadSceneAsync<T>(IScene target, bool isAdditive, T sceneData)
        {
            if (IsLoaded(target) && isAdditive)
            {
                Debug.LogWarning(
                    "UniSwitcher currently does not fully support additively loading the same scene multiple times. Unloading functionality may break."
                );
            }

            var loadedLevel = _sceneLoader.LoadSceneAsync(
                target.RawValue,
                isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single,
                c =>
                {
                    c.Bind<StartedFromAnotherSceneMarker>().AsSingle();
                    c.Bind<T>().FromInstance(sceneData).AsSingle();
                }
            );
            while (!loadedLevel.isDone)
            {
                _progressUpdateDelegates?.Invoke(loadedLevel.progress);
                await UniTask.Yield();
            }

            SceneManager.SetActiveScene(SceneManager.GetSceneByPath(target.RawValue));
            SetLoaded(target, true);
            _currentScene = target.RawValue;

#if UNITY_ANALYTICS
            if (Analytics.enabled)
            {
                switch (target)
                {
                    case IReportable r:
                        if (!r.DoNotReport())
                        {
                            AnalyticsEvent.ScreenVisit(target.RawValue);
                        }

                        break;
                    default:
                        if (!target.SuppressEvent) {
                            // Analytics is enabled, but the scene definition did not implement IReportable and thus event was not sent.
                            if (Analytics.enabled) {
                                Debug.LogWarning(
                                    $"Heads up! This project have Unity Analytics enabled, but your Scene definition ({target.GetType().Name}) does not implement IReportable; the event was not sent.\n"
                                    + "If you want UniSwitcher to report 'Screen Visit' events, please implement IReportable to avoid unexpected rate limiting by UA.\n"
                                    + "If you don't, override 'BaseScene.SuppressEvent' in your Scene definition and set it to true.\n"
                                    + "For more information, refer to https://github.com/Clpsplug/UniSwitcher/wiki/Unity-Analytics for details."
                                ); 
                            }
                        }

                        break;
                }
            }
#endif
#if UGS_ANALYTICS
            if (!string.IsNullOrEmpty(target.ScreenVisitEventName) && !string.IsNullOrEmpty(target.ScreenVisitEventParameterName))
            {
                switch (target)
                {
                    case IReportable r:
                        if (!r.DoNotReport())
                        {
                            SendScreenVisit(target);
                        }

                        break;
                    default:
                        if (!target.SuppressEvent)
                        {
                            Debug.LogWarning(
                                $"Heads up! Your Scene definition ({target.GetType().Name}) does not implement IReportable; the event was not sent to prevent unexpectedly using up your event quota.\n"
                                + $"Since you have overridden both {nameof(target.ScreenVisitEventName)} and {nameof(target.ScreenVisitEventParameterName)}, you probably intend to use UGS Analytics.\n"
                                + "Please implement IReportable to start sending the events.\n"
                                + "For more information, refer to https://github.com/Clpsplug/UniSwitcher/wiki/UGS-Analytics for details."
                            );
                        }

                        break;
                }
            }
            else
            {
                if (!target.SuppressEvent)
                {
                    Debug.LogWarning(
                        $"Heads up! You have UGS Analytics package in this project, but your screen definition does not implement either (or both) of {nameof(target.ScreenVisitEventName)} or {nameof(target.ScreenVisitEventParameterName)}.\n"
                        + $"If you want UniSwitcher to report 'Screen Visit' events, please override both and implement IReportable into your Scene definition ({target.GetType().Name}.)\n"
                        + $"If you don't, but you still want to include UGS Analytics, please override {nameof(target.SuppressEvent)} and set it to true.\n"
                        + $"If you didn't mean to use Analytics in your project, consider removing com.unity.services.analytics package from this project."
                    );
                }
            }
#endif
        }

#if UGS_ANALYTICS
        private void SendScreenVisit(IScene target)
        {
            AnalyticsService.Instance.CustomData(
                target.ScreenVisitEventName, 
    new Dictionary<string,object> {
                    {target.ScreenVisitEventParameterName, target.RawValue},
                }
            );
        }
#endif
        
        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// Under-the-hood unloading
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private async UniTask UnloadSceneAsync(IScene target)
        {
            try
            {
                var unloadedLevel = SceneManager.UnloadSceneAsync(
                    target.RawValue
                );

                if (!IsLoaded(target))
                {
                    Debug.LogWarning(
                        $"Target {target.RawValue} does not look loaded."
                    );
                }

                while (!unloadedLevel.isDone)
                {
                    _progressUpdateDelegates?.Invoke(unloadedLevel.progress);
                    await UniTask.Yield();
                }
            }
            catch (ArgumentException)
            {
                Debug.LogWarning("Target unload failure detected.");
            }

            SetLoaded(target, false);
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
                return _loaded[target.RawValue];
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        private void SetLoaded(IScene target, bool loaded)
        {
            _loaded[target.RawValue] = loaded;
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
                _loaded[destination.RawValue] = false;
            }
        }
    }
}