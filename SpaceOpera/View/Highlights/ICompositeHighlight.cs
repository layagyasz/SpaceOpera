namespace SpaceOpera.View.Highlights
{
    public interface ICompositeHighlight
    {
        EventHandler<ValueEventArgs<IHighlight>>? OnHighlightAdded { get; set; }
        IEnumerable<IHighlight> GetHighlights();
        void Unhook();
    }
}
