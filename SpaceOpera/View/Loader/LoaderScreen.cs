using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using OpenTK.Mathematics;
using SpaceOpera.Controller.Loader;
using SpaceOpera.Core.Loader;

namespace SpaceOpera.View.Loader
{
    public class LoaderScreen : GraphicsResource, IDynamic, IScreen
    {
        private static readonly long s_RefreshTime = 100;

        public EventHandler<EventArgs>? Refreshed { get; set; }

        public IController Controller { get; }
        public LoaderComponent Loader { get; }

        private long _time;

        private LoaderScreen(IController controller, LoaderComponent loader)
        {
            Controller = controller;
            Loader = loader;
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            Loader.Draw(target, context);
        }

        public void Initialize()
        {
            Controller.Bind(this);
            Loader.Initialize();
        }

        public void Refresh()
        {
            Loader.Refresh();
            Refreshed?.Invoke(this, EventArgs.Empty);
        }

        public void ResizeContext(Vector3 bounds)
        {
            Loader.Position = 0.5f * (bounds - Loader.Size);
        }

        public void Update(long delta)
        {
            _time += delta;
            if (_time > s_RefreshTime)
            {
                _time = 0;
                Refresh();
            }

            Loader.Update(delta);
        }

        protected override void DisposeImpl()
        {
            Controller.Unbind();
            Loader.Dispose();
        }

        public static LoaderScreen Create(
            UiElementFactory uiElementFactory, ILoaderTask loaderTask, LoaderStatus loaderStatus)
        {
            return new LoaderScreen(
                new LoaderController(loaderTask), new LoaderComponent(uiElementFactory, loaderStatus));
        }
    }
}
