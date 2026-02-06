using System.Collections.Generic;

namespace Game.Gameplay.Character.Intents
{
    /// <summary>
    /// Buffer for player intents.
    /// Systems can add/remove/query intents.
    /// </summary>
    public interface IIntentBuffer
    {
        /// <summary>
        /// Add or update an intent. Only one intent per type is stored.
        /// </summary>
        void Set<T>(T intent) where T : struct, IIntent;

        /// <summary>
        /// Remove an intent by type.
        /// </summary>
        void Remove<T>() where T : struct, IIntent;

        /// <summary>
        /// Check if an intent of type exists.
        /// </summary>
        bool Has<T>() where T : struct, IIntent;

        /// <summary>
        /// Get an intent by type. Returns default if not found.
        /// </summary>
        T Get<T>() where T : struct, IIntent;

        /// <summary>
        /// Try to get an intent by type.
        /// </summary>
        bool TryGet<T>(out T intent) where T : struct, IIntent;

        /// <summary>
        /// Get all current intents.
        /// </summary>
        IEnumerable<IIntent> GetAll();

        /// <summary>
        /// Clear all intents.
        /// </summary>
        void Clear();
    }
}
