namespace Game.Core
{
    /// <summary>
    /// Base interface for all objects that have a display name for Editor.
    /// </summary>
    public interface IDisplayName
    {
#if UNITY_EDITOR
        string DisplayName { get; }
#endif
    }
}