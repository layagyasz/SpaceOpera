using Cardamom.Ui.Controller;
using SpaceOpera.Core.Loader;
using SpaceOpera.View.Loader;

namespace SpaceOpera.Controller.Loader
{
    public class LoaderController : IController
    {
        public EventHandler<EventArgs>? Finished { get; set; }

        public ILoaderTask Task { get; }

        private LoaderScreen? _screen;

        public LoaderController(ILoaderTask task)
        {
            Task = task;
        }

        public void Bind(object @object)
        {
            _screen = (LoaderScreen)@object;
            _screen.Refreshed += HandleRefresh;
        }

        public void Unbind()
        {
            _screen!.Refreshed -= HandleRefresh;
        }

        private void HandleRefresh(object? sender, EventArgs e)
        {
            if (Task.IsDone())
            {
                Finished?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
