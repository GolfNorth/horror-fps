using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Game.Infrastructure.SceneManagement
{
    /// <summary>
    /// Scene loading abstraction supporting both built-in and Addressable scenes.
    /// </summary>
    public interface ISceneLoader
    {
        /// <summary>
        /// Load a scene by build index or name (built-in scenes).
        /// </summary>
        UniTask LoadSceneAsync(
            string sceneName,
            LoadSceneMode mode = LoadSceneMode.Single,
            CancellationToken cancellation = default);

        /// <summary>
        /// Load a scene with progress callback (built-in scenes).
        /// </summary>
        UniTask LoadSceneAsync(
            string sceneName,
            LoadSceneMode mode,
            IProgress<float> progress,
            CancellationToken cancellation = default);

        /// <summary>
        /// Load an Addressable scene by address.
        /// </summary>
        UniTask<SceneInstance> LoadAddressableSceneAsync(
            string address,
            LoadSceneMode mode = LoadSceneMode.Single,
            CancellationToken cancellation = default);

        /// <summary>
        /// Load an Addressable scene with progress callback.
        /// </summary>
        UniTask<SceneInstance> LoadAddressableSceneAsync(
            string address,
            LoadSceneMode mode,
            IProgress<float> progress,
            CancellationToken cancellation = default);

        /// <summary>
        /// Unload a scene by name (built-in scenes).
        /// </summary>
        UniTask UnloadSceneAsync(string sceneName, CancellationToken cancellation = default);

        /// <summary>
        /// Unload an Addressable scene.
        /// </summary>
        UniTask UnloadAddressableSceneAsync(SceneInstance sceneInstance, CancellationToken cancellation = default);
    }
}
