using VContainer;

namespace Game.Core.Modules
{
    /// <summary>
    /// Interface for service modules that configure DI container.
    /// Implemented by ScriptableObjects to define reusable service registrations.
    /// </summary>
    public interface IServicesModule
    {
        void Configure(IContainerBuilder builder);
    }
}
