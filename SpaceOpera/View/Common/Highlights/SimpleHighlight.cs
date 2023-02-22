namespace SpaceOpera.View.Common.Highlights
{
    public class SimpleHighlight : ICompositeHighlight
    {
        public EventHandler<ElementEventArgs<IHighlight>>? OnHighlightAdded { get; set; }

        IHighlight Highlight { get; }

        private SimpleHighlight(IHighlight highlight)
        {
            this.Highlight = highlight;
        }

        public static SimpleHighlight Wrap(IHighlight highlight)
        {
            return new SimpleHighlight(highlight);
        }

        public IEnumerable<IHighlight> GetHighlights()
        {
            yield return Highlight;
        }
    }
}