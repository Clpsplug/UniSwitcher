using UniSwitcher.Domain;
using UniSwitcher.Infra;
using UnityEngine;
using Zenject;

namespace UniSwitcher
{
    /// <summary>
    /// Zenject installer for UniSwitcher
    /// </summary>
    public class UniSwitcherInstaller : MonoInstaller
    {
        /// <summary>
        /// Must have <see cref="ISceneLoader"/> implementation attached.
        /// </summary>
        public GameObject sceneLoaderPrefab;
        
        /// <summary>
        /// Must have <see cref="ITransitionBackgroundController"/> implementation attached. Can be null.
        /// </summary>
        public GameObject transitionBackgroundPrefab;

        /// <inheritdoc cref="MonoInstaller.InstallBindings"/>
        public override void InstallBindings()
        {
            Container
                .Bind<IBootStrapper>().To<BootStrapper>().AsSingle();
            Container
                .Bind<ISceneLoader>()
                .FromComponentInNewPrefab(sceneLoaderPrefab)
                .AsSingle();
 
            // Transition background is optional.
            if (transitionBackgroundPrefab != null)
            {
                Container.Bind<ITransitionBackgroundController>()
                    .FromComponentInNewPrefab(transitionBackgroundPrefab)
                    .AsSingle();
            }
            else
            {
                Container.Bind<ITransitionBackgroundController>()
                    .FromInstance(null);
            }
        }
    }
}