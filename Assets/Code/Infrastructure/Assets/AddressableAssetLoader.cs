using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core.Logging;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;

namespace Game.Infrastructure.Assets
{
    /// <summary>
    /// Addressables-based asset loader implementation.
    /// </summary>
    public sealed class AddressableAssetLoader : IAssetLoader
    {
        private const string LogTag = "AssetLoader";

        private readonly ILogService _log;
        private readonly Dictionary<Object, AsyncOperationHandle> _handles = new();

        [Inject]
        public AddressableAssetLoader(ILogService log)
        {
            _log = log;
        }

        public async UniTask<T> LoadAsync<T>(string address, CancellationToken cancellation = default)
            where T : Object
        {
            var handle = Addressables.LoadAssetAsync<T>(address);
            await handle.ToUniTask(cancellationToken: cancellation);

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _handles[handle.Result] = handle;
                return handle.Result;
            }

            _log.Error(LogTag, $"Failed to load asset: {address}");
            return null;
        }

        public async UniTask<T> LoadAsync<T>(AssetReference reference, CancellationToken cancellation = default)
            where T : Object
        {
            var handle = reference.LoadAssetAsync<T>();
            await handle.ToUniTask(cancellationToken: cancellation);

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _handles[handle.Result] = handle;
                return handle.Result;
            }

            _log.Error(LogTag, $"Failed to load asset reference: {reference.RuntimeKey}");
            return null;
        }

        public void Release<T>(T asset) where T : Object
        {
            if (asset == null) return;

            if (_handles.TryGetValue(asset, out var handle))
            {
                Addressables.Release(handle);
                _handles.Remove(asset);
            }
        }

        public async UniTask PreloadAsync(string[] addresses, CancellationToken cancellation = default)
        {
            var tasks = new List<UniTask>();

            foreach (var address in addresses)
            {
                tasks.Add(LoadAsync<Object>(address, cancellation).AsUniTask());
            }

            await UniTask.WhenAll(tasks);
        }

        public async UniTask<GameObject> InstantiateAsync(
            AssetReference reference,
            Vector3 position,
            Quaternion rotation,
            Transform parent = null,
            CancellationToken cancellation = default)
        {
            var handle = Addressables.InstantiateAsync(reference, position, rotation, parent);
            await handle.ToUniTask(cancellationToken: cancellation);

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return handle.Result;
            }

            _log.Error(LogTag, $"Failed to instantiate asset reference: {reference.RuntimeKey}");
            return null;
        }

        public void ReleaseInstance(GameObject instance)
        {
            if (instance == null) return;

            Addressables.ReleaseInstance(instance);
        }
    }
}
