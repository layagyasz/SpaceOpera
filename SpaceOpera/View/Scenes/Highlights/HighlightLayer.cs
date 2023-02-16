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
            private readonly Matrix4 _outlineTransform;
            private readonly RenderShader _outlineShader;
            private readonly Matrix4 _fillTransform;
            private readonly RenderShader _fillShader;

            private readonly Dictionary<IHighlight, SpaceRegionView> _highlights = new();

            private SingleHighlightLayer(
                ICompositeHighlight highlight,
                ISet<TSubRegion> range, 
                Dictionary<TSubRegion, SpaceSubRegionBounds> boundsMap,
                float borderWidth, 
                Matrix4 outlineTransform,
                RenderShader outlineShader,
                Matrix4 fillTransform,
                RenderShader fillShader)
            {
                Highlight = highlight;
                _range = range;
                _boundsMap = boundsMap;
                _borderWidth = borderWidth;
                _outlineTransform = outlineTransform;
                _outlineShader = outlineShader;
                _fillTransform = fillTransform;
                _fillShader = fillShader;
            }

            public static SingleHighlightLayer Create(
                ICompositeHighlight highlight,
                ISet<TSubRegion> range,
                Dictionary<TSubRegion, SpaceSubRegionBounds> boundsMap, 
                float borderWidth, 
                Matrix4 outlineTransform,
                RenderShader outlineShader,
                Matrix4 fillTransform,
                RenderShader fillShader)
            {
                var layer = 
                    new SingleHighlightLayer(
                        highlight, 
                        range, 
                        boundsMap, 
                        borderWidth, 
                        outlineTransform,
                        outlineShader,
                        fillTransform,
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
                    _outlineTransform,
                    _outlineShader,
                    _fillTransform,
                    _fillShader,
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
        private readonly RenderShader _outlineShader;
        private readonly RenderShader _fillShader;
        private readonly EnumMap<HighlightLayerName, Matrix4> _outlineTransforms;
        private readonly EnumMap<HighlightLayerName, Matrix4> _fillTransforms;

        private readonly EnumMap<HighlightLayerName, SingleHighlightLayer> _layers = new();

        public HighlightLayer(
            IEnumerable<TSubRegion> range,
            Dictionary<TSubRegion, SpaceSubRegionBounds> boundsMap,
            float borderWidth,
            EnumMap<HighlightLayerName, Matrix4> outlineTransforms,
            RenderShader outlineShader,
            EnumMap<HighlightLayerName, Matrix4> fillTransforms,
            RenderShader fillShader)
        {
            _range = range.ToImmutableHashSet();
            _boundsMap = boundsMap;
            _borderWidth = borderWidth;
            _outlineShader = outlineShader;
            _fillShader = fillShader;
            _outlineTransforms = outlineTransforms;
            _fillTransforms = fillTransforms;
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
                    _boundsMap, 
                    _borderWidth,
                    _outlineTransforms[layer], 
                    _outlineShader,
                    _fillTransforms[layer], 
                    _fillShader);
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