namespace Game.Gameplay.Character.Actions
{
    public interface IActionBuffer
    {
        void Set<T>(T action) where T : struct, IAction;
        void Remove<T>() where T : struct, IAction;
        bool Has<T>() where T : struct, IAction;
        T Get<T>() where T : struct, IAction;
        bool TryGet<T>(out T action) where T : struct, IAction;
        void Clear();
    }
}
