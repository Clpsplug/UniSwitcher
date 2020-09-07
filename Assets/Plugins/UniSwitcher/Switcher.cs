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
    public abstract class Switcher : MonoBehaviour
    {
        [SerializeField] [Tooltip("Can be null. If your scene have one, assign it here.")]
        protected ProgressDisplayController sceneProgressBarController = default;

#pragma warning disable 649
        [Inject] private IBootStrapper _bootStrapper;
        [Inject] private ISceneLoader _sceneLoader;
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
        /// You can directly pass this into <see cref="PerformSceneTransition"/>.
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        protected SceneTransitionConfiguration ChangeScene(IScene scene)
        {
            return SceneTransitionConfiguration.StartConfiguration(scene, false);
        }

        /// <summary>
        /// Start the configuration as "Additive scene change."
        /// You can directly pass this into <see cref="PerformSceneTransition"/>.
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        protected SceneTransitionConfiguration AddScene(IScene scene)
        {
            return SceneTransitionConfiguration.StartConfiguration(scene, true);
        }

        /// <summary>
        /// Performs scene transition.
        /// </summary>
        /// <param name="config">Using <see cref="ChangeScene"/> or <see cref="AddScene"/> is recommended.</param>
        /// <returns></returns>
        protected async UniTask PerformSceneTransition(SceneTransitionConfiguration config)
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

            if (!config.SupressProgressBar && sceneProgressBarController != null)
            {
                sceneProgressBarController.Enable();
                sceneProgressBarController.SetDDoL();
            }

            if (config.Delay <= 0.0001f)
            {
                // No delay or negligible delay
                if (config.PerformTransition && TransitionBackgroundController != null)
                {
                    TransitionBackgroundController.TriggerTransitionIn();
                    await UniTask.Delay(TimeSpan.FromSeconds(0.1));
                    while (TransitionBackgroundController.GetTransitionState() == TransitionState.In)
                    {
                        await UniTask.Yield();
                    }
                }

                _sceneLoader.LoadScene(config.DestinationScene, config.IsAdditive);
            }
            else
            {
                _sceneLoader.LoadSceneWithDelay(config.DestinationScene, config.Delay, config.IsAdditive);
                if (config.PerformTransition && TransitionBackgroundController != null)
                {
                    // In case of delayed transition, trigger transition 1 second before scene change
                    TransitionBackgroundController.TriggerTransitionIn();
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
            if (config.DataToTransfer != null)
            {
                var passTask = _bootStrapper.Pass(config.DataToTransfer);
                while (passTask.Status == UniTaskStatus.Pending)
                {
                    await UniTask.Yield();
                }
            }

            if (!config.SupressProgressBar && sceneProgressBarController != null)
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
                TransitionBackgroundController?.TriggerTransitionOut();
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

        protected void UnloadScene(IScene target)
        {
            _sceneLoader.UnloadScene(target);
        }

        protected void AddChangeSceneDelegate(UnityAction<Scene, Scene> @delegate)
        {
            _sceneLoader.AddChangeDelegate(@delegate);
        }

        protected void RemoveChangeSceneDelegate(UnityAction<Scene, Scene> @delegate)
        {
            _sceneLoader.RemoveChangeDelegate(@delegate);
        }

        protected void AddSceneLoadedDelegate(UnityAction<Scene, LoadSceneMode> @delegate)
        {
            _sceneLoader.AddSceneLoadedDelegate(@delegate);
        }

        protected void RemoveSceneLoadedDelegate(UnityAction<Scene, LoadSceneMode> @delegate)
        {
            _sceneLoader.RemoveSceneLoadedDelegate(@delegate);
        }

        /// <summary>
        /// overrideできるようにはしているが、baseも呼ぶべき。
        /// </summary>
        protected virtual void OnDestroy()
        {
            _sceneLoader.ResetProgressUpdateDelegate();
        }
    }

    /// <summary>
    /// シーン遷移の設定オブジェクト
    /// </summary>
    public class SceneTransitionConfiguration
    {
        /// <summary>
        /// 遷移先のシーン
        /// </summary>
        /// <remarks>Required</remarks>
        public readonly IScene DestinationScene;

        /// <summary>
        /// /// Additive Loadを行うか
        /// /// </summary>
        /// /// <remarks>Default: false</remarks>
        public bool IsAdditive;

        /// <summary>
        /// 遷移先のシーンに渡したいデータ
        /// /// </summary>
        /// /// <remarks>Optional, default: null</remarks>
        public ISceneData DataToTransfer;

        /// <summary>
        /// /// トランジションを行うか
        /// /// </summary>
        /// /// <remarks>Default: false</remarks>
        public bool PerformTransition;

        /// <summary>
        /// /// シーン遷移を遅らせる長さ(秒)
        /// /// </summary>
        /// /// <remarks>Default: 0.0f</remarks>
        public float Delay;

        public bool SupressProgressBar;

        private SceneTransitionConfiguration(IScene scene, bool additively)
        {
            DestinationScene = scene;
            IsAdditive = additively;
        }

        /// <summary>
        /// 新しく設定オブジェクトを生成
        /// </summary>
        /// <param name="scene">遷移先のシーン</param>
        /// <param name="additively"></param>
        /// <returns><see cref="SceneTransitionConfiguration"/></returns>
        public static SceneTransitionConfiguration StartConfiguration(IScene scene, bool additively)
        {
            return new SceneTransitionConfiguration(scene, additively);
        }

        /// <summary>
        ///  遷移先に渡すデータを指定する
        /// </summary>
        ///  <param name="data"><see cref="ISceneData"/>を実装したオブジェクト</param>
        /// <returns><see cref="SceneTransitionConfiguration"/></returns>
        public SceneTransitionConfiguration AttachData(ISceneData data)
        {
            DataToTransfer = data;
            return this;
        }

        /// <summary>
        /// トランジションを行うようにする
        /// </summary>
        /// <returns><see cref="SceneTransitionConfiguration"/></returns>
        public SceneTransitionConfiguration WithTransitionEffect()
        {
            PerformTransition = true;
            return this;
        }

        public SceneTransitionConfiguration HideProgressBar()
        {
            SupressProgressBar = true;
            return this;
        }

        /// <summary>
        /// 遷移までの長さを指定する
        /// </summary>
        /// <param name="seconds">秒単位</param>
        /// <returns><see cref="SceneTransitionConfiguration"/></returns>
        public SceneTransitionConfiguration After(float seconds)
        {
            Delay = seconds;
            return this;
        }
    }
}