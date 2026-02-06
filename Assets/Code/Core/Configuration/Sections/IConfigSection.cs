namespace Game.Core.Configuration
{
    public interface IConfigSection
    {
        string Key { get; }
#if UNITY_EDITOR
        string Label { get; }
#endif
    }
}
