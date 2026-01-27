using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Core.DI
{
    /// <summary>
    /// Automatically injects dependencies using a specific LifetimeScope type.
    /// Similar to VContainer's parentReference pattern.
    /// Waits for the container to be ready before injecting.
    /// </summary>
    public class AutoInjectWithScope : MonoBehaviour
    {
        [SerializeField] private ParentReference _scopeReference;
        [SerializeField] private bool _includeChildren = true;

        private CancellationTokenSource _cts;

        private void Awake()
        {
            _cts = new CancellationTokenSource();
            InjectAsync(_cts.Token).Forget();
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }

        private async UniTaskVoid InjectAsync(CancellationToken cancellation)
        {
            var scope = FindScopeByType();
            if (scope == null)
            {
                Debug.LogWarning($"[AutoInject] Could not find scope of type '{_scopeReference.TypeName}'", this);
                return;
            }

            // Wait for container to be built
            await UniTask.WaitUntil(() => scope.Container != null, cancellationToken: cancellation);

            if (_includeChildren)
            {
                scope.Container.InjectGameObject(gameObject);
            }
            else
            {
                InjectSingleObject(scope.Container);
            }
        }

        private LifetimeScope FindScopeByType()
        {
            if (string.IsNullOrEmpty(_scopeReference.TypeName))
            {
                return FindAnyActiveScope();
            }

            var scopes = FindObjectsByType<LifetimeScope>(FindObjectsSortMode.None);
            foreach (var scope in scopes)
            {
                if (MatchesTypeName(scope.GetType()))
                {
                    return scope;
                }
            }

            return null;
        }

        private bool MatchesTypeName(Type type)
        {
            return type.Name == _scopeReference.TypeName || type.FullName == _scopeReference.TypeName;
        }

        private static LifetimeScope FindAnyActiveScope()
        {
            var scopes = FindObjectsByType<LifetimeScope>(FindObjectsSortMode.None);
            foreach (var scope in scopes)
            {
                if (scope.Container != null)
                {
                    return scope;
                }
            }
            return scopes.First();
        }

        private void InjectSingleObject(IObjectResolver resolver)
        {
            var components = GetComponents<MonoBehaviour>();
            foreach (var component in components)
            {
                if (component != this)
                {
                    resolver.Inject(component);
                }
            }
        }
    }
}
