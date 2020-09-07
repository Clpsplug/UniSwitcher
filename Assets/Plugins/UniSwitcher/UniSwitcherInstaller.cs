using UniSwitcher.Domain;
using UniSwitcher.Infra;
using UnityEngine;
using Zenject;

namespace UniSwitcher
{
    public class UniSwitcherInstaller : MonoInstaller
    {
        public GameObject sceneLoaderPrefab;
        public GameObject transitionBackgroundPrefab;

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