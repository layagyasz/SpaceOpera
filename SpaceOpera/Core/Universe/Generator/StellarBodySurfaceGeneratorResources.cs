using Cardamom.Graphics;
using Cardamom.ImageProcessing.Pipelines;
using OpenTK.Mathematics;

namespace SpaceOpera.Core.Universe.Generator
{
    public class StellarBodySurfaceGeneratorResources : GraphicsResource
    {
        private static readonly Color4 s_DefaultColor = new(0.5f, 0.5f, 1, 1);

        public int Resolution { get; }

        private ICanvasProvider? _canvases;

        public StellarBodySurfaceGeneratorResources(int resolution)
        {
            Resolution = resolution;
            _canvases = new CachingCanvasProvider(new(resolution, resolution), s_DefaultColor);
        }

        public static StellarBodySurfaceGeneratorResources CreateForGenerator()
        {
            return new(64);
        }

        public static StellarBodySurfaceGeneratorResources CreateHighRes()
        {
            return new(2048);
        }

        public static StellarBodySurfaceGeneratorResources CreateLowRes()
        {
            return new(256);
        }

        protected override void DisposeImpl()
        {
            _canvases!.Dispose();
            _canvases = null;
        }

        public ICanvasProvider GetCanvasProvider()
        {
            return _canvases!;
        }
    }
}
