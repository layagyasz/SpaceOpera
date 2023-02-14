using Cardamom.Collections;
using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Mathematics;
using System.Collections.Immutable;

namespace SpaceOpera.View.Scenes.Highlights
{
    public class HighlightLayer<TSubRegion> : IRenderable where TSubRegion : notnull
    {
        class SingleHighlightLayer : IRenderable
        {
            public ICompositeHighlight Highlight { get; }

            private readonly ISet<TSubRegion> _range;
            private readonly Func<IEnumerable<TSubRegion>, Color4, Color4, IRenderable> _highlightFn;

            private readonly Dictionary<IHighlight, IRenderable> _highlights = new();

            private SingleHighlightLayer(
                ICompositeHighlight vighlight, 
                ISet<TSubRegion> range, 
                Func<IEnumerable<TSubRegion>, Color4, Color4, IRenderable> highlightFn)
            {
                Highlight = vighlight;
                _range = range;
                _highlightFn = highlightFn;
            }

            public static SingleHighlightLayer Create(
                ICompositeHighlight highlight, 
                ISet<TSubRegion> range,
                Func<IEnumerable<TSubRegion>, Color4, Color4, IRenderable> highlightFn)
            {
                var layer = new SingleHighlightLayer(highlight, range, highlightFn);
                foreach (var h in highlight.GetHighlights())
                {
                    h.OnUpdated += layer.HandleHighlightUpdate;
                    layer._highlights.Add(h, layer.ComputeHighlight(h));
                }
                return layer;
            }

            public void Initialize() { }

            public void ResizeContext(Vector3 bounds) { }

            public void Draw(RenderTarget target, UiContext context)
            {
                foreach (var highlight in _highlights.Values)
                {
                    highlight.Draw(target, context);
                }
            }

            public void Unhook()
            {
                foreach (var highlight in _highlights.Keys)
                {
                    highlight.OnUpdated -= HandleHighlightUpdate;
                }
            }

            public void Update(long delta) { }

            private IRenderable ComputeHighlight(IHighlight highlight)
            {
                return _highlightFn(_range.Where(x => highlight.Contains(x)), highlight.BorderColor, highlight.Color);
            }

            private void HandleHighlightUpdate(object? sender, EventArgs e)
            {
                var highlight = (IHighlight)sender!;
                _highlights[highlight] = ComputeHighlight(highlight);
            }
        }

        private readonly ISet<TSubRegion> _range;
        private readonly Func<IEnumerable<TSubRegion>, Color4, Color4, IRenderable> _highlightFn;

        private readonly EnumMap<HighlightLayerName, SingleHighlightLayer> _layers = new();

        public HighlightLayer(
            IEnumerable<TSubRegion> range, Func<IEnumerable<TSubRegion>, Color4, Color4, IRenderable> highlightFn)
        {
            _range = range.ToImmutableHashSet();
            _highlightFn = highlightFn;
        }

        public void Initialize() { }

        public void ResizeContext(Vector3 bounds) { }

        public void ClearLayer(HighlightLayerName layer)
        {
            if (_layers.TryGetValue(layer, out var highlight))
            {
                highlight.Unhook();
                _layers.Remove(layer);
            }
        }

        public void SetLayer(HighlightLayerName layer, ICompositeHighlight highlight)
        {
            ClearLayer(layer);
            _layers[layer] = SingleHighlightLayer.Create(highlight, _range, _highlightFn);
        }

        public void Draw(RenderTarget target, UiContext context)
        {
            foreach (var layer in _layers.Values)
            {
                layer?.Draw(target, context);
            }
        }

        public void Unhook()
        {
            foreach (var layer in _layers.Keys.ToList())
            {
                ClearLayer(layer);
            }
        }

        public void Update(long delta) { }
    }
}