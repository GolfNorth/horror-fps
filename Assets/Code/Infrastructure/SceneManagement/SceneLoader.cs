using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core.Logging;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using VContainer;

namespace Game.Infrastructure.SceneManagement
{
    /// <summary>
    /// Scene loader implementation supporting both built-in and Addressable scenes.
    /// </summary>
    public sealed class SceneLoader : ISceneLoader
    {
        private const string LogTag = "SceneLoader";

        private readonly ILogService _log;

        [Inject]
        public SceneLoader(ILogService log)
        {
            _log = log;
        }

        public async UniTask LoadSceneAsync(
            string sceneName,
            LoadSceneMode mode = LoadSceneMode.Single,
            CancellationToken cancellation = default)
        {
            await LoadSceneAsync(sceneName, mode, null, cancellation);
        }

        public async UniTask LoadSceneAsync(
            string sceneName,
            LoadSceneMode mode,
            IProgress<float> progress,
            CancellationToken cancellation = default)
        {
            var operation = SceneManager.LoadSceneAsync(sceneName, mode);

            if (operation == null)
            {
                _log.Error(LogTag, $"Failed to load scene: {sceneName}");
                return;
            }

            operation.allowSceneActivation = false;

            while (operation.progress < 0.9f)
            {
                cancellation.ThrowIfCancellationRequested();
                progress?.Report(operation.progress);
                await UniTask.Yield(cancellation);
            }

            progress?.Report(1f);
            operation.allowSceneActivation = true;

            await UniTask.WaitUntil(() => operation.isDone, cancellationToken: cancellation);

            _log.Info(LogTag, $"Loaded scene: {sceneName}");
        }

        public async UniTask<SceneInstance> LoadAddressableSceneAsync(
            string address,
            LoadSceneMode mode = LoadSceneMode.Single,
            CancellationToken cancellation = default)
        {
            return await LoadAddressableSceneAsync(address, mode, null, cancellation);
        }

        public async UniTask<SceneInstance> LoadAddressableSceneAsync(
            string address,
            LoadSceneMode mode,
            IProgress<float> progress,
            CancellationToken cancellation = default)
        {
            var handle = Addressables.LoadSceneAsync(address, mode);

            while (!handle.IsDone)
            {
                cancellation.ThrowIfCancellationRequested();
                progress?.Report(handle.PercentComplete);
                await UniTask.Yield(cancellation);
            }

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                _log.Error(LogTag, $"Failed to load addressable scene: {address}");
                return default;
            }

            progress?.Report(1f);
            _log.Info(LogTag, $"Loaded addressable scene: {address}");

            return handle.Result;
        }

        public async UniTask UnloadSceneAsync(string sceneName, CancellationToken cancellation = default)
        {
            var operation = SceneManager.UnloadSceneAsync(sceneName);

            if (operation == null)
            {
                _log.Error(LogTag, $"Failed to unload scene: {sceneName}");
                return;
            }

            await UniTask.WaitUntil(() => operation.isDone, cancellationToken: cancellation);

            _log.Info(LogTag, $"Unloaded scene: {sceneName}");
        }

        public async UniTask UnloadAddressableSceneAsync(SceneInstance sceneInstance, CancellationToken cancellation = default)
        {
            var handle = Addressables.UnloadSceneAsync(sceneInstance);

            await UniTask.WaitUntil(() => handle.IsDone, cancellationToken: cancellation);

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                _log.Error(LogTag, "Failed to unload addressable scene");
                return;
            }

            _log.Info(LogTag, "Unloaded addressable scene");
        }
    }
}
