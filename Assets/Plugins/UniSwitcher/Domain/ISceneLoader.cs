using System.Threading;
using UniSwitcher.Infra;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace UniSwitcher.Domain
{
    public interface ISceneLoader
    {
        /// <summary>
        /// Load scene without delay
        /// </summary>
        /// <param name="target">Target scene</param>
        /// <param name="isAdditive">true to perform additive load</param>
        void LoadScene(IScene target, bool isAdditive = false);

        /// <summary>
        /// Load scene after set amount of time
        /// </summary>
        /// <param name="target">Target scene</param>
        /// <param name="time">Time (in seconds)</param>
        /// <param name="isAdditive">true to perform additive load</param>
        /// <param name="cancellationToken">Pass token to enable load cancelling</param>
        void LoadSceneWithDelay(IScene target, float time, bool isAdditive = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Unloads scene
        /// </summary>
        /// <param name="target">Target scene</param>
        /// <remarks>Will not work when the scene isn't loaded additively</remarks>
        void UnloadScene(IScene target);

        /// <summary>
        /// Get current scene as Scene enum
        /// </summary>
        /// <returns></returns>
        string GetCurrentScene();

        /// <summary>
        /// Get current scene as Scene
        /// </summary>
        /// <returns></returns>
        Scene GetCurrentSceneAsScene();

        /// <summary>
        /// Check if the target scene is loaded
        /// </summary>
        /// <param name="target">Target scene</param>
        /// <returns>true if loading is completed. When false, do not start looking for stuffs.</returns>
        bool IsLoaded(IScene target);

        void AddProgressUpdateDelegate(OnProgressUpdateDelegate d);
        void ResetProgressUpdateDelegate();
        void AddChangeDelegate(UnityAction<Scene, Scene> action);
        void AddSceneLoadedDelegate(UnityAction<Scene, LoadSceneMode> action);
        void RemoveChangeDelegate(UnityAction<Scene, Scene> action);
        void RemoveSceneLoadedDelegate(UnityAction<Scene, LoadSceneMode> action);
    }
}