using System;
using UniSwitcher.Infra;

namespace UniSwitcher.Domain
{
    /// <summary>
    /// Interface for Data Loader
    /// If any data is passed on scene switching, these methods will be called from <see cref="BootStrapper"/>.
    /// </summary>
    public interface IDataLoader
    {
        /// <summary>
        /// Receive and load the data, and pass it to other GameObjects if needed.
        /// </summary>
        /// <param name="data"></param>
        void Load(ISceneData data);

        /// <summary>
        /// Method to validate the data received.
        /// This method should be used to catch what you never want to see in production.
        /// Thus it is usually enough that you check the type of <see cref="ISceneData"/>,
        /// but you are allowed to put extra checks as needed.
        /// </summary>
        /// <param name="data"></param>
        bool Validate(ISceneData data);

        /// <summary>
        /// This is called when <see cref="Load"/> throws any exceptions.
        /// Can be used to gracefully fail.
        /// If called, <see cref="IsHeld"/> will be ignored completely.
        /// </summary>
        /// <param name="e"></param>
        void OnFailure(Exception e);

        /// <summary>
        /// If for some reason you <see cref="Load"/>ed data but want to delay the transition
        /// (e.g. you fired an async operation in <see cref="Load"/>,) return true.
        /// As long as it returns true, the game will NOT transition out to the destination scene.
        /// Make sure this eventually returns false, or you'll face soft locks.
        /// </summary>
        /// <returns></returns>
        /// <remarks>NEVER throw anything.</remarks>
        bool IsHeld();
    }
}