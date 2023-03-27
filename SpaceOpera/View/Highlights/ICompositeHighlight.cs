namespace SpaceOpera.View.Highlights
{
    public interface ICompositeHighlight
    {
        public EventHandler<ValueEventArgs<IHighlight>>? OnHighlightAdded { get; set; }
        IEnumerable<IHighlight> GetHighlights();
    }
}
