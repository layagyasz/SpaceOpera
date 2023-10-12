namespace SpaceOpera.View.Game.Highlights
{
    public class SimpleHighlight : ICompositeHighlight
    {
        public EventHandler<IHighlight>? HighlightAdded { get; set; }

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

        public void Hook(object domain)
        {
            Highlight.Hook(domain);
        }

        public void Unhook(object domain)
        {
            Highlight.Unhook(domain);
        }
    }
}