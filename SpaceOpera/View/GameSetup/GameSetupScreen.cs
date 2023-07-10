using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using OpenTK.Mathematics;

namespace SpaceOpera.View.GameSetup
{
    public class GameSetupScreen : GraphicsResource, IScreen
    {
        public IController Controller { get; }

        private IUiComponent? _form;
        private Vector3 _bounds;

        public GameSetupScreen(IController controller)
        {
            Controller = controller;
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            _form?.Draw(target, context);
        }

        public IUiComponent GetForm()
        {
            return _form!;
        }

        public void Initialize()
        {
            Controller.Bind(this);
        }

        public void ResizeContext(Vector3 bounds)
        {
            _bounds = bounds;
            if (_form != null)
            {
                _form.Position = 0.5f * (bounds - _form.Size);
            }
        }

        public void SetForm(IUiComponent? form)
        {
            _form?.Dispose();
            _form = form;
            if (form != null)
            {
                form.Initialize();
                form.Position = 0.5f * (_bounds - form.Size);
            }
        }

        public void Update(long delta)
        {
            _form?.Update(delta);
        }

        protected override void DisposeImpl()
        {
            Controller.Unbind();
            _form?.Dispose();
        }
    }
}
