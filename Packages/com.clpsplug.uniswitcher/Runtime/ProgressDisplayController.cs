using Cysharp.Threading.Tasks;
using UniSwitcher.Infra;
using UnityEngine;

namespace UniSwitcher
{
    /// <summary>
    /// Base class for implementing a component to display progress bars.
    /// </summary>
    public abstract class ProgressDisplayController : MonoBehaviour
    {
        /// <summary>
        /// Display the progress. This method already conforms to <see cref="OnProgressUpdateDelegate"/>.
        /// </summary>
        /// <param name="progress"></param>
        public abstract void SetProgress(float progress);

        /// <summary>
        /// Shows the progress bar.
        /// </summary>
        /// <param name="reset">True if you want the progress to revert to zero.</param>
        public abstract void Enable(bool reset = true);

        /// <summary>
        /// Hides the progress bar.
        /// </summary>
        public abstract void Disable();

        /// <summary>
        /// Apply <see cref="MonoBehaviour.DontDestroyOnLoad"/> on switching over the scene.
        /// </summary>
        /// <remarks>Be careful not to lose references when you call this yourself.</remarks>
        public abstract void SetDDoL();

        /// <summary>
        /// Hides the progress bar, but also "destroys this progress bar" when done and it's also an async operation.
        /// Note that this only instructs the progress bar to destroy itself. You can immediately destroy it here,
        /// but you can also defer the destruction (e.g. after the closing animation ends.)
        /// </summary>
        /// <remarks>This is intentionally an async operation just in case exceptions are thrown.</remarks>
        public abstract UniTask Close();
    }
}