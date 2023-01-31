using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Mathematics;

namespace SpaceOpera.Views.StellarBodyViews
{
    public class StellarBodyModel : IRenderable
    {
        private readonly Model<VertexLit3> _surfaceModel;

        public StellarBodyModel(Model<VertexLit3> surfaceModel)
        {
            _surfaceModel = surfaceModel;
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
        }

        public void Update(long delta)
        {
            _surfaceModel.Update(delta);
        }
    }
}
