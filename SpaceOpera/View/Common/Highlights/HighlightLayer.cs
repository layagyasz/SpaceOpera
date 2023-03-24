using Cardamom.Collections;
using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Mathematics;
using System.Collections.Immutable;

namespace SpaceOpera.View.Common.Highlights
{
    public class HighlightLayer<TRegion, TSubRegion> : GraphicsResource, IRenderable 
        where TRegion : notnull 
        where TSubRegion : notnull
    {
        class SingleHighlightLayer : GraphicsResource, IRenderable
        {
            public ICompositeHighlight Highlight { get; }
       
            private readonly ISet<TRegion> _range;
            private readonly Func<TRegion, IEnumerable<TSubRegion>> _regionMapFn;
            private readonly Dictionary<TSubRegion, SpaceSubRegionBounds> _boundsMap;
            private readonly float _borderWidth;
            private readonly RenderShader _outlineShader;
            private readonly RenderShader _fillShader;

            private readonly Dictionary<IHighlight, SpaceRegionView> _highlights = new();

            private SingleHighlightLayer(
                ICompositeHighlight highlight,
                ISet<TRegion> range,
                Func<TRegion, IEnumerable<TSubRegion>> regionMapFn,
                Dictionary<TSubRegion, SpaceSubRegionBounds> boundsMap,
                float borderWidth, 
                RenderShader outlineShader,
                RenderShader fillShader)
            {
                Highlight = highlight;
                _range = range;
                _regionMapFn = regionMapFn;
                _boundsMap = boundsMap;
                _borderWidth = borderWidth;
                _outlineShader = outlineShader;
                _fillShader = fillShader;
            }

            public static SingleHighlightLayer Create(
                ICompositeHighlight highlight,
                ISet<TRegion> range,
                Func<TRegion, IEnumerable<TSubRegion>> regionMapFn,
                Dictionary<TSubRegion, SpaceSubRegionBounds> boundsMap, 
                float borderWidth, 
                RenderShader outlineShader,
                RenderShader fillShader)
            {
                var layer = 
                    new SingleHighlightLayer(
                        highlight, 
                        range,
                        regionMapFn,
                        boundsMap, 
                        borderWidth, 
                        outlineShader,
                        fillShader);
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
                    _outlineShader,
                    _fillShader,
                    _range
                        .Where(x => highlight.Contains(x))
                        .SelectMany(_regionMapFn)
                        .Select(x => _boundsMap[x])
                        .ToImmutableHashSet(),
                    highlight.BorderColor, 
                    highlight.Color, 
                    _borderWidth * highlight.BorderWidth,
                    highlight.Merge);
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

        private readonly ISet<TRegion> _range;
        private readonly Func<TRegion, IEnumerable<TSubRegion>> _regionMapFn;
        private readonly Dictionary<TSubRegion, SpaceSubRegionBounds> _boundsMap;
        private readonly float _borderWidth;
        private readonly Matrix4 _position;
        private readonly RenderShader _outlineShader;
        private readonly RenderShader _fillShader;

        private readonly EnumMap<HighlightLayerName, SingleHighlightLayer> _layers = new();

        public HighlightLayer(
            IEnumerable<TRegion> range,
            Func<TRegion, IEnumerable<TSubRegion>> regionMapFn,
            Dictionary<TSubRegion, SpaceSubRegionBounds> boundsMap,
            float borderWidth,
            Matrix4 position,
            RenderShader outlineShader,
            RenderShader fillShader)
        {
            _range = range.ToImmutableHashSet();
            _regionMapFn = regionMapFn;
            _boundsMap = boundsMap;
            _borderWidth = borderWidth;
            _position = position;
            _outlineShader = outlineShader;
            _fillShader = fillShader;
        }

        protected override void DisposeImpl()
        {
            Unhook();
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
            _layers[layer] =
                SingleHighlightLayer.Create(
                    highlight,
                    _range,
                    _regionMapFn,
                    _boundsMap,
                    _borderWidth,
                    _outlineShader,
                    _fillShader);
        }

        public void Draw(RenderTarget target, UiContext context)
        {
            target.PushModelMatrix(_position);
            foreach (var layer in _layers.Values)
            {
                layer?.Draw(target, context);
            }
            target.PopModelMatrix();
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