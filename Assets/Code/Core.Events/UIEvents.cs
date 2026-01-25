namespace Game.Core.Events
{
    /// <summary>
    /// UI navigation event.
    /// </summary>
    public readonly struct UINavigationEvent
    {
        public string ScreenId { get; }
        public bool IsBack { get; }

        public UINavigationEvent(string screenId, bool isBack = false)
        {
            ScreenId = screenId;
            IsBack = isBack;
        }
    }

    /// <summary>
    /// UI visibility changed event.
    /// </summary>
    public readonly struct UIVisibilityChangedEvent
    {
        public string ElementId { get; }
        public bool IsVisible { get; }

        public UIVisibilityChangedEvent(string elementId, bool isVisible)
        {
            ElementId = elementId;
            IsVisible = isVisible;
        }
    }
}
