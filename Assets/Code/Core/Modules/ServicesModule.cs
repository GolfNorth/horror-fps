using UnityEngine;
using VContainer;

namespace Game.Core.Modules
{
    /// <summary>
    /// Base ScriptableObject for service modules.
    /// Inherit to create reusable service configuration assets.
    /// </summary>
    public abstract class ServicesModule : ScriptableObject, IServicesModule
    {
        public abstract void Configure(IContainerBuilder builder);
    }
}
