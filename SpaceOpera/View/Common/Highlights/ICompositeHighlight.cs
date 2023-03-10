namespace SpaceOpera.View.Common.Highlights
{
    public interface ICompositeHighlight
    {
        public EventHandler<ValueEventArgs<IHighlight>>? OnHighlightAdded { get; set; }
        IEnumerable<IHighlight> GetHighlights();
    }
}
