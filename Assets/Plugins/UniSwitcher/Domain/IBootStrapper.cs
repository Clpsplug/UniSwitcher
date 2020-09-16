using Cysharp.Threading.Tasks;
using UniSwitcher.Infra;

namespace UniSwitcher.Domain
{
    /// <summary>
    /// Injection interface for <see cref="BootStrapper"/>
    /// </summary>
    public interface IBootStrapper
    {
        /// <summary>
        /// Fires the entry point of <see cref="ISceneEntryPoint"/> that exists in the next scene.
        /// </summary>
        /// <param name="data">Data to pass.</param>
        /// <remarks>Ensure that <see cref="ISceneLoader.IsLoaded"/> returns true before calling it.</remarks>
        UniTask TriggerEntryPoint();
    }
}