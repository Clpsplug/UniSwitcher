using System;
using Cysharp.Threading.Tasks;
using UniSwitcher.Domain;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UniSwitcher.Infra
{
    /// <summary>
    /// A class that finds <see cref="IDataLoader"/> in the scene and pass the data for initialization
    /// </summary>
    public class BootStrapper : IBootStrapper
    {
        /// <inheritdoc cref="IBootStrapper.Pass"/>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentException"></exception>
        public async UniTask Pass(ISceneData data)
        {
            var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            GameObject dataLoaderObject = null;
            foreach (var rootObject in rootObjects)
            {
                var component = rootObject.GetComponent<IDataLoader>();
                if (component != null)
                {
                    dataLoaderObject = rootObject;
                    break;
                }
            }

            if (dataLoaderObject == null)
            {
                Debug.LogError(
                    $"Cannot find any Data Loader in the active scene, '{SceneManager.GetActiveScene().name}'!\n" +
                    "Did you mean to accept data in this scene? Did you check the correct scene is fully loaded?");
                throw new Exception("Data passed but no data loader is located.");
            }

            var dataLoader = dataLoaderObject.GetComponent<IDataLoader>();
            if (!dataLoader.Validate(data))
            {
                Debug.LogError(
                    "Validator failed the value! Are you sure you are passing a correct value? Did you check if your scene is loaded first?");
                throw new ArgumentException("Data loader validator failure.");
            }

            try
            {
                dataLoader.Load(data);
            }
            catch (Exception e)
            {
                Debug.LogError(
                    "Data loader threw exception on load - initialization failure! Trace follows this message.\n" +
                    $"The data loader object is named {dataLoaderObject.name}");
                Debug.LogException(e);
                dataLoader.OnFailure(e);
                return;
            }

            // Wait for lift...
            while (dataLoader.IsHeld())
            {
                await UniTask.Yield();
            }
        }
    }
}