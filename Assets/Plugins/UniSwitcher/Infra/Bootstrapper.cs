using System;
using Cysharp.Threading.Tasks;
using UniSwitcher.Domain;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UniSwitcher.Infra
{
    /// <summary>
    /// A class that finds <see cref="ISceneEntryPoint"/> in the scene and triggers the entry point
    /// </summary>
    public class BootStrapper : IBootStrapper
    {
        /// <inheritdoc cref="IBootStrapper.TriggerEntryPoint"/>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentException"></exception>
        public async UniTask TriggerEntryPoint()
        {
            var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            ISceneEntryPoint sceneEntryPoint = null;
            foreach (var rootObject in rootObjects)
            {
                sceneEntryPoint = rootObject.GetComponent<ISceneEntryPoint>();
                if (sceneEntryPoint != null)
                {
                    break;
                }
            }

            // If null at this point, there are no entry point.
            if (sceneEntryPoint == null) return;

            if (!sceneEntryPoint.Validate())
            {
                Debug.LogError(
                    "Validator failed the value! Are you sure you are passing a correct value? Did you check if your scene is loaded first?"
                );
                throw new ArgumentException("Data loader validator failure.");
            }

            try
            {
                await sceneEntryPoint.Fire();
            }
            catch (Exception e)
            {
                Debug.LogError(
                    "Data loader threw exception on load - initialization failure! Trace follows this message.\n");
                Debug.LogException(e);
                sceneEntryPoint.OnFailure(e);
            }
        }
    }
}