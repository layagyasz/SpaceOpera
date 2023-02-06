using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Mathematics;

namespace SpaceOpera.Views.StellarBodyViews
{
    public class StellarBodyModel : IRenderable
    {
        private readonly Model<VertexLit3> _surfaceModel;
        private readonly VertexBuffer<VertexLit3> _atmosphereModel;
        private readonly RenderShader _atmosphereShader;

        public StellarBodyModel(
            Model<VertexLit3> surfaceModel, VertexBuffer<VertexLit3> atmosphereModel, RenderShader atmosphereShader)
        {
            _surfaceModel = surfaceModel;
            _atmosphereModel = atmosphereModel;
            _atmosphereShader = atmosphereShader;
        }

        public void Initialize()
        {
            _surfaceModel.Initialize();
        }

        public void ResizeContext(Vector3 bounds)
        {
            _surfaceModel.ResizeContext(bounds);
        }

        public void Draw(RenderTarget target, UiContext context)
        {
            _surfaceModel.Draw(target, context);
            target.Draw(
                _atmosphereModel, 
                0,
                _atmosphereModel.Length, 
                new RenderResources(BlendMode.Alpha, _atmosphereShader) { EnableDepthTest = false });
        }

        public void Update(long delta)
        {
            _surfaceModel.Update(delta);
        }
    }
}
