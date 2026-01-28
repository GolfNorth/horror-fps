namespace Game.Core.Ticking
{
    public interface ITickService
    {
        void Register(object target);
        void Unregister(object target);
    }
}
