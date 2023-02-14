namespace SpaceOpera.View.Scenes.Highlights
{
    public interface ICompositeHighlight
    {
        public EventHandler<ElementEventArgs<IHighlight>>? OnHighlightAdded { get; set; }
        IEnumerable<IHighlight> GetHighlights();
    }
}
