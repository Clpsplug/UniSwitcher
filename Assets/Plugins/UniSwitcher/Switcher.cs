using System;
using Cysharp.Threading.Tasks;
using UniSwitcher.Domain;
using UniSwitcher.Infra;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Zenject;

namespace UniSwitcher
{
    /// <summary>
    /// Base <see cref="MonoBehaviour"/> for changing scene
    /// </summary>
    public abstract class Switcher : MonoBehaviour
    {
        /// <summary>
        /// Assign to activate the progress bar on transition.
        /// </summary>
        [SerializeField] [Tooltip("Can be null. If your scene have one, assign it here.")]
        protected ProgressDisplayController sceneProgressBarController = default;

#pragma warning disable 649
        [Inject] private IBootStrapper _bootStrapper;
        [Inject] private ISceneLoader _sceneLoader;
        /// <summary>
        /// Transition background
        /// </summary>
        [Inject] protected ITransitionBackgroundController TransitionBackgroundController;
#pragma warning restore 649

        /// <summary>
        /// If you need to display the progress yourself, add the updating method as delegate here.
        /// </summary>
        /// <param name="delegate"></param>
        protected void AddSceneProgressDelegate(OnProgressUpdateDelegate @delegate)
        {
            _sceneLoader.AddProgressUpdateDelegate(@delegate);
        }

        /// <summary>
        /// Forces the transition object to be in <see cref="TransitionState.Wait"/> state.
        /// </summary>
        protected void ForceTransitionWait()
        {
            TransitionBackgroundController.ForceTransitionWait();
        }

        /// <summary>
        /// Start the configuration as "Total scene change."
        /// You can directly pass this into <see cref="PerformSceneTransition{T}"/>.
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        protected virtual SceneTransitionConfiguration<object> ChangeScene(IScene scene)
        {
            return SceneTransitionConfiguration<object>.StartConfiguration(scene, false);
        }

        /// <summary>
        /// Start the configuration as "Total scene change," and attach data.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected virtual SceneTransitionConfiguration<T> ChangeScene<T>(IScene scene, T data)
        {
            return SceneTransitionConfiguration<T>.StartConfiguration(scene, false).AttachData(data);
        }

        /// <summary>
        /// Start the configuration as "Additive scene change."
        /// You can directly pass this into <see cref="PerformSceneTransition{T}"/>.
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        protected virtual SceneTransitionConfiguration<object> AddScene(IScene scene)
        {
            return SceneTransitionConfiguration<object>.StartConfiguration(scene, true);
        }

        /// <summary>
        /// Start the configuration as "Additive scene change," and attach data.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected virtual SceneTransitionConfiguration<T> AddScene<T>(IScene scene, T data)
        {
            return SceneTransitionConfiguration<T>.StartConfiguration(scene, true).AttachData(data);
        }

        /// <summary>
        /// Performs scene transition.
        /// </summary>
        /// <param name="config">Using <see cref="ChangeScene"/> or <see cref="AddScene"/> is recommended.</param>
        /// <returns></returns>
        protected async UniTask PerformSceneTransition<T>(SceneTransitionConfiguration<T> config)
        {
            if (config.PerformTransition && TransitionBackgroundController == null)
            {
                Debug.LogWarning(
                    "Scene change / addition with a transition requested, " +
                    "but there is no transition background attached.\n" +
                    "Did you assign one in UniSwitcherInstaller?"
                );
            }

            if (config.PerformTransition && TransitionBackgroundController != null)
            {
                while (TransitionBackgroundController.GetTransitionState() != TransitionState.Ready)
                {
                    await UniTask.Yield();
                }
            }

            if (!config.IsAdditive)
            {
                DontDestroyOnLoad(this);
            }

            if (!config.SuppressProgressBar && sceneProgressBarController != null)
            {
                sceneProgressBarController.Enable();
                sceneProgressBarController.SetDDoL();
            }

            if (config.Delay <= 0.0001f)
            {
                // No delay or negligible delay
                if (config.PerformTransition && TransitionBackgroundController != null)
                {
                    TransitionBackgroundController.TriggerTransitionIn(config);
                    await UniTask.Delay(TimeSpan.FromSeconds(0.1));
                    while (TransitionBackgroundController.GetTransitionState() == TransitionState.In)
                    {
                        await UniTask.Yield();
                    }
                }

                _sceneLoader.LoadScene(config.DestinationScene, config.IsAdditive,
                    config.DataToTransfer);
            }
            else
            {
                _sceneLoader.LoadSceneWithDelay(config.DestinationScene, config.Delay, config.IsAdditive,
                    config.DataToTransfer);
                if (config.PerformTransition && TransitionBackgroundController != null)
                {
                    // In case of delayed transition, trigger transition 1 second before scene change
                    TransitionBackgroundController.TriggerTransitionIn(config);
                    await UniTask.Delay(TimeSpan.FromSeconds(config.Delay - 1));
                    while (TransitionBackgroundController.GetTransitionState() == TransitionState.In)
                    {
                        await UniTask.Yield();
                    }
                }
            }

            while (!_sceneLoader.IsLoaded(config.DestinationScene) ||
                   (TransitionBackgroundController != null && config.PerformTransition &&
                    !TransitionBackgroundController.IsTransitionAllowed()))
            {
                await UniTask.Yield();
            }

            TransitionBackgroundController?.DetectMainCamera();

            var entryPointTask = _bootStrapper.TriggerEntryPoint();
            while (entryPointTask.Status == UniTaskStatus.Pending)
            {
                await UniTask.Yield();
            }

            if (!config.SuppressProgressBar && sceneProgressBarController != null)
            {
                sceneProgressBarController.SetProgress(1f);
                if (config.IsAdditive)
                {
                    sceneProgressBarController.Disable();
                }
                else
                {
                    sceneProgressBarController.Close().Forget(Debug.LogException);
                }
            }

            if (config.PerformTransition)
            {
                TransitionBackgroundController?.TriggerTransitionOut(config);
            }

            if (!config.IsAdditive)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// This method will not complete until the transition ends.
        /// As soon as the animation ends, the loop immediately breaks, completing the method.
        /// </summary>
        /// <returns></returns>
        protected async UniTask WaitForTransitionReady()
        {
            while (TransitionBackgroundController.GetTransitionState() != TransitionState.Ready)
            {
                await UniTask.Yield();
            }
        }

        /// <summary>
        /// This method will not complete until the <see cref="target"/> scene unloads.
        /// As soon as the target scene is unloaded (and gets ready for reload,)
        /// the loop immediately breaks, completing the method.
        /// </summary>
        /// <param name="target"><see cref="IScene"/> to wait for its unload</param>
        /// <returns></returns>
        /// <remarks>If the scene is unloaded to start with, this method will immediately return.</remarks>
        protected async UniTask WaitForSceneUnload(IScene target)
        {
            while (_sceneLoader.IsLoaded(target))
            {
                await UniTask.Yield();
            }
        }

        /// <summary>
        /// Unload additive scene. Will fail if this is the only scene currently loaded
        /// </summary>
        /// <param name="target"></param>
        protected void UnloadScene(IScene target)
        {
            _sceneLoader.UnloadScene(target);
        }

        /// <summary>
        /// Adds <see cref="SceneManager.activeSceneChanged"/> delegate.
        /// </summary>
        /// <param name="delegate"></param>
        protected void AddChangeSceneDelegate(UnityAction<Scene, Scene> @delegate)
        {
            _sceneLoader.AddChangeDelegate(@delegate);
        }

        /// <summary>
        /// Removes <see cref="SceneManager.activeSceneChanged"/> delegate.
        /// </summary>
        /// <param name="delegate"></param>
        protected void RemoveChangeSceneDelegate(UnityAction<Scene, Scene> @delegate)
        {
            _sceneLoader.RemoveChangeDelegate(@delegate);
        }

        /// <summary>
        /// Adds <see cref="SceneManager.sceneLoaded"/> delegate.
        /// </summary>
        /// <param name="delegate"></param>
        protected void AddSceneLoadedDelegate(UnityAction<Scene, LoadSceneMode> @delegate)
        {
            _sceneLoader.AddSceneLoadedDelegate(@delegate);
        }

        /// <summary>
        /// Removes <see cref="SceneManager.sceneLoaded"/> delegate.
        /// </summary>
        /// <param name="delegate"></param>
        protected void RemoveSceneLoadedDelegate(UnityAction<Scene, LoadSceneMode> @delegate)
        {
            _sceneLoader.RemoveSceneLoadedDelegate(@delegate);
        }

        /// <summary>
        /// Things to do before <see cref="Switcher"/>'s <see cref="OnDestroy"/> do things
        /// </summary>
        protected virtual void WillBeDestroyed()
        {
            /* no-op */
        }
        
        /// <summary>
        /// DO NOT OVERRIDE. Use <see cref="WillBeDestroyed"/> or <see cref="WasDestroyed"/>
        /// </summary>
        /// <remarks>
        /// Overriding this is a bad idea because you'll need to remember to call 'this' <see cref="OnDestroy"/>.
        /// </remarks>
        private void OnDestroy()
        {
            WillBeDestroyed();
            _sceneLoader.ResetProgressUpdateDelegate();
            WasDestroyed();
        }

        /// <summary>
        /// Things to do after <see cref="Switcher"/>'s <see cref="OnDestroy"/> did its things
        /// </summary>
        protected virtual void WasDestroyed()
        {
            /* no-op */
        }

        /// <summary>
        /// Configuration for scene transition
        /// </summary>
        /// <remarks>
        /// The point of this class is to be able to be used as a 'builder' class.
        /// You MAY extend this class to customize the configuration, but you SHOULD mark such classes 'sealed'
        /// to avoid any chaos and SHOULD NOT make more than one of them.
        /// </remarks>
        public class SceneTransitionConfiguration<T> // XXX: if no data, then specify object. Void not possible
        {
            /// <summary>
            /// Destination
            /// </summary>
            /// <remarks>Required</remarks>
            public IScene DestinationScene { get; }

            /// <summary>
            /// Additive Load or not?
            /// </summary>
            /// <remarks>Default: false</remarks>
            public bool IsAdditive { get; }

            /// <summary>
            /// Data to transfer to the next scene
            /// </summary>
            /// <remarks>Optional, default: null</remarks>
            public T DataToTransfer { get; private set; }

            /// <summary>
            /// True if you wish to do a transition animation
            /// </summary>
            /// <remarks>Default: false</remarks>
            public bool PerformTransition { get; private set; }

            /// <summary>
            /// time to defer transition in seconds
            /// </summary>
            /// <remarks>Default: 0.0f</remarks>
            public float Delay { get; private set; }

            /// <summary>
            /// If the progress bar should not be shown, true
            /// </summary>
            public bool SuppressProgressBar { get; private set; }

            private SceneTransitionConfiguration(IScene scene, bool additively)
            {
                DestinationScene = scene;
                IsAdditive = additively;
            }

            /// <summary>
            /// Constructor that just copies the original. This is used when extending this configuration.
            /// </summary>
            /// <param name="original"></param>
            protected SceneTransitionConfiguration(SceneTransitionConfiguration<T> original)
            {
                DestinationScene = original.DestinationScene;
                IsAdditive = original.IsAdditive;
                DataToTransfer = original.DataToTransfer;
                PerformTransition = original.PerformTransition;
                Delay = original.Delay;
                SuppressProgressBar = original.SuppressProgressBar;
            }

            /// <summary>
            /// Create a new configuration
            /// </summary>
            /// <param name="scene">destination</param>
            /// <param name="additively"></param>
            /// <returns><see cref="SceneTransitionConfiguration{T}"/></returns>
            public static SceneTransitionConfiguration<T> StartConfiguration(IScene scene, bool additively)
            {
                return new SceneTransitionConfiguration<T>(scene, additively);
            }

            /// <summary>
            /// Set data to pass to the next scene
            /// </summary>
            /// <param name="data">Object to pass as the data</param>
            /// <returns><see cref="SceneTransitionConfiguration{T}"/></returns>
            internal SceneTransitionConfiguration<T> AttachData(T data)
            {
                DataToTransfer = data;
                return this;
            }

            /// <summary>
            /// Enable transition animation
            /// </summary>
            /// <returns><see cref="SceneTransitionConfiguration{T}"/></returns>
            public SceneTransitionConfiguration<T> WithTransitionEffect()
            {
                PerformTransition = true;
                return this;
            }

            /// <summary>
            /// Hide the progress bar even if it exists in the scene and assigned to <see cref="Switcher"/>.
            /// </summary>
            /// <returns></returns>
            public SceneTransitionConfiguration<T> HideProgressBar()
            {
                SuppressProgressBar = true;
                return this;
            }

            /// <summary>
            /// Set time before transition
            /// </summary>
            /// <param name="seconds"></param>
            /// <returns><see cref="SceneTransitionConfiguration{T}"/></returns>
            public SceneTransitionConfiguration<T> After(float seconds)
            {
                Delay = seconds;
                return this;
            }
        }
    }
}