using Cardamom.Graphics;
using Cardamom.Utils.Suppliers;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace SpaceOpera.Core.Loader
{
    public class LoaderThread : GraphicsResource
    {
        private readonly Queue<ILoaderTask> _tasks = new();
        private readonly Thread _thread;
        private readonly NativeWindow _window;

        public LoaderThread()
        {
            _thread = new Thread(WorkThread);
            _window = new NativeWindow(
                new NativeWindowSettings()
                {
                    Flags = ContextFlags.Offscreen
                });
        }

        public void QueueTask(ILoaderTask task)
        {
            Monitor.Enter(_tasks);
            _tasks.Enqueue(task);
            Monitor.Pulse(_tasks);
            Monitor.Exit(_tasks);
        }

        public Promise<T> Load<T>(Func<T> loaderFn)
        {
            var task = new FuncLoaderTask<T>(loaderFn);
            QueueTask(task);
            return task.GetPromise();
        }

        public void Start()
        {
            _thread.Start();
        }

        protected override void DisposeImpl()
        {
            _window.Dispose();
        }

        private void WorkThread()
        {
            _window.MakeCurrent();
            while (true)
            {
                Monitor.Enter(_tasks);
                if (_tasks.Count > 0)
                {
                    var task  = _tasks.Dequeue();
                    task.Perform();
                }
                else
                {
                    Monitor.Wait(_tasks);
                }
                Monitor.Exit(_tasks);
            }
        }
    }
}
