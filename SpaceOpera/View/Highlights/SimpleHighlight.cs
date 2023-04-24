namespace SpaceOpera.View.Highlights
{
    public class SimpleHighlight : ICompositeHighlight
    {
        public EventHandler<ValueEventArgs<IHighlight>>? HighlightAdded { get; set; }

        IHighlight Highlight { get; }

        private SimpleHighlight(IHighlight highlight)
        {
            Highlight = highlight;
        }

        public static SimpleHighlight Wrap(IHighlight highlight)
        {
            return new SimpleHighlight(highlight);
        }

        public IEnumerable<IHighlight> GetHighlights()
        {
            yield return Highlight;
        }

        public void Unhook()
        {
            Highlight.Unhook();
        }
    }
}