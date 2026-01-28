namespace Game.Core.Ticking
{
    /// <summary>
    /// Common tick priority values.
    /// Lower values execute first.
    /// </summary>
    public static class TickPriorities
    {
        public const int First = -1000;
        public const int PreInput = -100;
        public const int Input = 0;
        public const int PostInput = 100;
        public const int PrePhysics = 200;
        public const int Physics = 300;
        public const int PostPhysics = 400;
        public const int PreAnimation = 500;
        public const int Animation = 600;
        public const int PostAnimation = 700;
        public const int PreRender = 800;
        public const int Render = 900;
        public const int Last = 1000;
    }
}
