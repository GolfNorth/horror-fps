using UnityEngine;

namespace Game.Core.Configuration
{
    /// <summary>
    /// Base class for all game configuration ScriptableObjects.
    /// Each module should have its own config inheriting from this.
    /// </summary>
    public abstract class GameConfig : ScriptableObject
    {
#if UNITY_EDITOR
        [TextArea(3, 10)]
        [SerializeField]
        private string developerNotes;
#endif
    }
}
