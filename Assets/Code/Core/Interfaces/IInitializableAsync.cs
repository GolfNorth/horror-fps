using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Core
{
    /// <summary>
    /// Interface for services requiring async initialization.
    /// </summary>
    public interface IInitializableAsync
    {
        UniTask InitializeAsync(CancellationToken cancellation = default);
    }
}
