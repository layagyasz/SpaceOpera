namespace SpaceOpera.View.Game.Highlights
{
    public interface ICompositeHighlight
    {
        EventHandler<IHighlight>? HighlightAdded { get; set; }
        IEnumerable<IHighlight> GetHighlights();
        void Hook(object domain);
        void Unhook(object domain);
    }
}
