using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using OpenTK.Mathematics;

namespace SpaceOpera.View.GameSetup
{
    public class GameSetupScreen : IRenderable
    {
        IController ComponentController { get; }

        private IUiElement? _form;
        private Vector3 _bounds;

        public GameSetupScreen(IController componentController)
        {
            ComponentController = componentController;
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            _form?.Draw(target, context);
        }

        public void Initialize()
        {
            ComponentController.Bind(this);
        }

        public void ResizeContext(Vector3 bounds)
        {
            _bounds = bounds;
            if (_form != null)
            {
                _form.Position = 0.5f * (bounds - _form.Size);
            }
        }

        public void SetForm(IUiElement? form)
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
    }
}
