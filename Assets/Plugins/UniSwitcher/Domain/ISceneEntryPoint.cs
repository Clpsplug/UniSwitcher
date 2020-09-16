using System;
using UniSwitcher.Infra;

namespace UniSwitcher.Domain
{
    /// <summary>
    /// Interface for Scene entry point
    /// If an implementation exists in the destination scene, <see cref="BootStrapper"/> will call it
    /// <para>
    /// The implementations should get <see cref="ISceneData"/> injected, but consider optional injection for testing purposes.
    /// </para>
    /// </summary>
    public interface ISceneEntryPoint
    {
        /// <summary>
        /// Load the data injected through Zenject, and pass it to other GameObjects if needed.
        /// </summary>
        void Fire();

        /// <summary>
        /// Method to validate the data received.
        /// This method should be used to catch what you never want to see in production,
        /// because this will totally stop the loading process.
        /// </summary>
        bool Validate();

        /// <summary>
        /// This is called when <see cref="Fire"/> throws any exceptions.
        /// Can be used to gracefully fail.
        /// If called, <see cref="IsHeld"/> will be ignored completely.
        /// </summary>
        /// <param name="e"></param>
        void OnFailure(Exception e);

        /// <summary>
        /// If for some reason you <see cref="Fire"/>d the entry point but want to delay the transition
        /// (e.g. you fired an async operation in <see cref="Fire"/>,) return true.
        /// As long as it returns true, the game will NOT transition out to the destination scene.
        /// Make sure this eventually returns false, or you'll face soft locks.
        /// </summary>
        /// <returns></returns>
        /// <remarks>NEVER throw anything.</remarks>
        bool IsHeld();
    }
}