using System.ComponentModel;
using System.Threading;
using UniSwitcher.Infra;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace UniSwitcher.Domain
{
    /// <summary>
    /// Interface for underlying scene loading mechanism
    /// </summary>
    public interface ISceneLoader
    {
        /// <summary>
        /// Load scene without delay
        /// </summary>
        /// <param name="target">Target scene</param>
        /// <param name="isAdditive">true to perform additive load</param>
        /// <param name="sceneData"></param>
        void LoadScene<T>(IScene target, bool isAdditive, T sceneData);

        /// <summary>
        /// Load scene after set amount of time
        /// </summary>
        /// <param name="target">Target scene</param>
        /// <param name="time">Time (in seconds)</param>
        /// <param name="isAdditive">true to perform additive load</param>
        /// <param name="sceneData"></param>
        /// <param name="cancellationToken">Pass token to enable load cancelling</param>
        void LoadSceneWithDelay<T>(IScene target, float time, bool isAdditive, T sceneData,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Unloads scene
        /// </summary>
        /// <param name="target">Target scene</param>
        /// <remarks>Will not work when the scene isn't loaded additively</remarks>
        void UnloadScene(IScene target);

        /// <summary>
        /// Get current scene as path
        /// </summary>
        /// <returns></returns>
        string GetCurrentScene();

        /// <summary>
        /// Get current scene as Unity Scene
        /// </summary>
        /// <returns></returns>
        Scene GetCurrentSceneAsScene();

        /// <summary>
        /// Check if the target scene is loaded
        /// </summary>
        /// <param name="target">Target scene</param>
        /// <returns>true if loading is completed. When false, do not start looking for stuffs.</returns>
        bool IsLoaded(IScene target);

        /// <summary>
        /// Adds extra scene load progress update delegate.
        /// </summary>
        /// <param name="d"></param>
        void AddProgressUpdateDelegate(OnProgressUpdateDelegate d);

        /// <summary>
        /// Totally removes progress update delegate.
        /// </summary>
        void ResetProgressUpdateDelegate();

        /// <summary>
        /// Adds <see cref="SceneManager.activeSceneChanged"/> delegate.
        /// </summary>
        /// <param name="action"></param>
        void AddChangeDelegate(UnityAction<Scene, Scene> action);

        /// <summary>
        /// Adds <see cref="SceneManager.sceneLoaded"/> delegate.
        /// </summary>
        /// <param name="action"></param>
        void AddSceneLoadedDelegate(UnityAction<Scene, LoadSceneMode> action);

        /// <summary>
        /// Removes <see cref="SceneManager.activeSceneChanged"/> delegate.
        /// </summary>
        /// <param name="action"></param>
        void RemoveChangeDelegate(UnityAction<Scene, Scene> action);

        /// <summary>
        /// Removes <see cref="SceneManager.sceneLoaded"/> delegate.
        /// </summary>
        /// <param name="action"></param>
        void RemoveSceneLoadedDelegate(UnityAction<Scene, LoadSceneMode> action);
    }
}