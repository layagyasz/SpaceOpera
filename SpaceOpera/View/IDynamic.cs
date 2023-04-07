namespace SpaceOpera.View
{
    public interface IDynamic
    {
        EventHandler<EventArgs>? Refreshed { get; set; }
        void Refresh();
    }
}
