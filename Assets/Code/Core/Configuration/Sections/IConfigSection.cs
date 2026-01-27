namespace Game.Core.Configuration
{
    public interface IConfigSection
    {
        string Key { get; }
        string DisplayName { get; }
    }
}
