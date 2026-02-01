namespace Game.Core.Configuration
{
    public interface IConfigSection : IDisplayName
    {
        string Key { get; }
    }
}
