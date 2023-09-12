using Cardamom.Collections;
using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Mathematics;
using SpaceOpera.View.Game.Common;

namespace SpaceOpera.View.Game.Highlights
{
    public class HighlightLayer<TSubRegion> : GraphicsResource, IRenderable
        where TSubRegion : notnull
    {
        class SingleHighlightLayer : GraphicsResource, IRenderable
        {
            public ICompositeHighlight Highlight { get; }

            private readonly BoundsAndRegionKey[] _range;
            private readonly float _borderWidth;
            private readonly HighlightShaders _shaders;

            private readonly HashSet<IHighlight> _updated = new();
            private readonly Dictionary<IHighlight, IRenderable> _highlights = new();

            private SingleHighlightLayer(
                ICompositeHighlight highlight,
                BoundsAndRegionKey[] range,
                float borderWidth,
                HighlightShaders shaders)
            {
                Highlight = highlight;
                _range = range;
                _borderWidth = borderWidth;
                _shaders = shaders;
            }

            public static SingleHighlightLayer Create(
                ICompositeHighlight highlight,
                BoundsAndRegionKey[] range,
                float borderWidth,
                HighlightShaders shaders)
            {
                var layer = new SingleHighlightLayer(highlight, range, borderWidth, shaders);
                foreach (var h in highlight.GetHighlights())
                {
                    h.Updated += layer.HandleUpdate;
                    layer._highlights.Add(h, h.CreateHighlight<TSubRegion>(shaders, range, borderWidth));
                }
                return layer;
            }

            public void Initialize() { }

            public void ResizeContext(Vector3 bounds) { }

            public void Draw(IRenderTarget target, IUiContext context)
            {
                foreach (var highlight in _highlights.Values)
                {
                    highlight.Draw(target, context);
                }
            }

            public void Update(long delta)
            {
                lock (_updated)
                {
                    foreach (var highlight in _updated)
                    {
                        UpdateHighlight(highlight);
                    }
                    _updated.Clear();
                }
            }

            protected override void DisposeImpl()
            {
                foreach (var highlight in _highlights)
                {
                    highlight.Key.Updated -= HandleUpdate;
                    if (highlight.Value is IDisposable disposable)
                    {
                       disposable.Dispose();
                    }
                }
                _highlights.Clear();
            }

            private void UpdateHighlight(IHighlight highlight)
            {
                if (_highlights.TryGetValue(highlight, out var current))
                {
                    if (current is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
                _highlights[highlight] = highlight.CreateHighlight<TSubRegion>(_shaders, _range, _borderWidth);
            }

            private void HandleUpdate(object? sender, EventArgs e)
            {
                lock(_updated)
                {
                    _updated.Add((IHighlight)sender!);
                }
            }
        }

        private readonly BoundsAndRegionKey[] _range;
        private readonly float _borderWidth;
        private readonly Matrix4 _position;
        private readonly HighlightShaders _shaders;

        private readonly EnumMap<HighlightLayerName, SingleHighlightLayer> _layers = new();

        private HighlightLayer(
            BoundsAndRegionKey[] range,
            float borderWidth,
            Matrix4 position,
            HighlightShaders shaders)
        {
            _range = range;
            _borderWidth = borderWidth;
            _position = position;
            _shaders = shaders;
        }

        public static HighlightLayer<TSubRegion> Create<TRegion>(
            IEnumerable<TRegion> range,
            Func<TRegion, IEnumerable<TSubRegion>> regionMapFn,
            Dictionary<TSubRegion, SpaceSubRegionBounds> boundsMap,
            float borderWidth,
            Matrix4 position,
            HighlightShaders shaders)
            where TRegion : notnull
        {
            List<BoundsAndRegionKey> r = new();
            foreach (var region in range)
            {
                foreach (var subRegion in regionMapFn(region))
                {
                    r.Add(new(region, boundsMap[subRegion]));
                }
            }
            return new(r.ToArray(), borderWidth, position, shaders);
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
                _layers.Remove(layer);
            }
        }

        public void SetLayer(HighlightLayerName layer, ICompositeHighlight? highlight)
        {
            ClearLayer(layer);
            if (highlight != null)
            {
                _layers[layer] = SingleHighlightLayer.Create(highlight, _range, _borderWidth, _shaders);
            }
        }

        public void Draw(IRenderTarget target, IUiContext context)
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

        public void Update(long delta)
        {
            foreach (var layer in _layers.Values)
            {
                layer.Update(delta);
            }
        }
    }
}