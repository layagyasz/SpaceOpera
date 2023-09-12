using Cardamom.Graphics;

namespace SpaceOpera.View.Game.Highlights
{
    public class HighlightShaders
    {
        public RenderShader Fill { get; }
        public RenderShader Outline { get; }

        public HighlightShaders(RenderShader fill, RenderShader outline)
        {
            Fill = fill;
            Outline = outline;
        }
    }
}
