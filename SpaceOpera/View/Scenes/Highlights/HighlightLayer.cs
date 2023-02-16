using Cardamom.Collections;
using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Mathematics;
using System.Collections.Immutable;

namespace SpaceOpera.View.Scenes.Highlights
{
    public class HighlightLayer<TSubRegion> : IRenderable where TSubRegion : notnull
    {
        class SingleHighlightLayer : GraphicsResource, IRenderable
        {
            public ICompositeHighlight Highlight { get; }
       
            private readonly ISet<TSubRegion> _range;
            private readonly Dictionary<TSubRegion, SpaceSubRegionBounds> _boundsMap;
            private readonly float _borderWidth;
            private readonly RenderShader _shader;

            private readonly Dictionary<IHighlight, SpaceRegionView> _highlights = new();

            private SingleHighlightLayer(
                ICompositeHighlight highlight,
                ISet<TSubRegion> range, 
                Dictionary<TSubRegion, SpaceSubRegionBounds> boundsMap,
                float borderWidth, 
                RenderShader shader)
            {
                Highlight = highlight;
                _range = range;
                _boundsMap = boundsMap;
                _borderWidth = borderWidth;
                _shader = shader;
            }

            public static SingleHighlightLayer Create(
                ICompositeHighlight highlight,
                ISet<TSubRegion> range,
                Dictionary<TSubRegion, SpaceSubRegionBounds> boundsMap, 
                float borderWidth, 
                RenderShader shader)
            {
                var layer = new SingleHighlightLayer(highlight, range, boundsMap, borderWidth, shader);
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

            protected override void DisposeImpl()
            {
                foreach (var highlight in _highlights.Values)
                {
                    highlight.Dispose();
                }
                _highlights.Clear();
            }

            private SpaceRegionView ComputeHighlight(IHighlight highlight)
            {
                return SpaceRegionView.Create(
                    _shader, 
                    _range.Where(x => highlight.Contains(x)).Select(x => _boundsMap[x]).ToImmutableHashSet(),
                    highlight.BorderColor, 
                    highlight.Color, 
                    _borderWidth);
            }

            private void HandleHighlightUpdate(object? sender, EventArgs e)
            {
                var highlight = (IHighlight)sender!;
                if (_highlights.TryGetValue(highlight, out var current))
                {
                    current.Dispose();
                }
                _highlights[highlight] = ComputeHighlight(highlight);
            }
        }

        private readonly ISet<TSubRegion> _range;
        private readonly Dictionary<TSubRegion, SpaceSubRegionBounds> _boundsMap;
        private readonly float _borderWidth;
        private readonly RenderShader _shader;

        private readonly EnumMap<HighlightLayerName, SingleHighlightLayer> _layers = new();

        public HighlightLayer(
            IEnumerable<TSubRegion> range, 
            Dictionary<TSubRegion, SpaceSubRegionBounds> boundsMap,
            float borderWidth, 
            RenderShader shader)
        {
            _range = range.ToImmutableHashSet();
            _boundsMap = boundsMap;
            _borderWidth = borderWidth;
            _shader = shader;
        }

        public void Initialize() { }

        public void ResizeContext(Vector3 bounds) { }

        public void ClearLayer(HighlightLayerName layer)
        {
            if (_layers.TryGetValue(layer, out var highlight))
            {
                highlight.Dispose();
                highlight.Unhook();
                _layers.Remove(layer);
            }
        }

        public void SetLayer(HighlightLayerName layer, ICompositeHighlight highlight)
        {
            ClearLayer(layer);
            _layers[layer] = SingleHighlightLayer.Create(highlight, _range, _boundsMap, _borderWidth, _shader);
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