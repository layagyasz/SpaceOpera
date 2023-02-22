namespace SpaceOpera.View.Common.Highlights
{
    public interface ICompositeHighlight
    {
        public EventHandler<ElementEventArgs<IHighlight>>? OnHighlightAdded { get; set; }
        IEnumerable<IHighlight> GetHighlights();
    }
}
