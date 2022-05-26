using System;
using Cysharp.Threading.Tasks;
using UniSwitcher.Infra;

namespace UniSwitcher.Domain
{
    /// <summary>
    /// Interface for Scene entry point
    /// If an implementation exists in the destination scene, <see cref="BootStrapper"/> will call it
    /// <para>
    /// The implementations should get the data passed injected through the type of the data,
    /// but consider optional injection for testing purposes.
    /// The entrypoint for a scene will not fire if Play Mode starts from that scene.
    /// </para>
    /// </summary>
    public interface ISceneEntryPoint
    {
        /// <summary>
        /// This is the entry point. Decode data, pass it to other GameObjects, do whatever you want.
        /// Implement this method as an async method. When this method finishes running, the transition will end.
        /// </summary>
        UniTask Fire();

        /// <summary>
        /// Method to validate the data received.
        /// This method should be used to catch what you never want to see in production,
        /// because this will totally stop the loading process.
        /// </summary>
        bool Validate();

        /// <summary>
        /// This is called when <see cref="Fire"/> throws any exceptions.
        /// Can be used to gracefully fail.
        /// </summary>
        /// <param name="e"></param>
        void OnFailure(Exception e);
    }
}