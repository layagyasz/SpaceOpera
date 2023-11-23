using Cardamom.Collections;
using Cardamom.Graphics;
using Cardamom.Logging;
using Cardamom.Utils.Suppliers;
using Cardamom.Window;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace SpaceOpera.Core.Loader
{
    public class Loader : GraphicsResource
    {
        interface IWorker : IDisposable
        {
            int GetTaskCount();
            void QueueTask(ILoaderTask task);
            void Start();
        }

        class Worker : IWorker
        {
            private readonly ILogger _logger;
            private readonly Queue<ILoaderTask> _tasks = new();
            private readonly Thread _thread;

            public Worker(ILogger logger)
            {
                _logger = logger.ForType(typeof(Worker));
                _thread = new Thread(WorkThread);
            }

            public virtual void Dispose() { }

            public int GetTaskCount()
            {
                return _tasks.Count;
            }

            public void QueueTask(ILoaderTask task)
            {
                Monitor.Enter(_tasks);
                _logger.AtInfo().With(_thread.ManagedThreadId).Log($"Received task {task.GetHashCode()}");
                _tasks.Enqueue(task);
                Monitor.Pulse(_tasks);
                Monitor.Exit(_tasks);
            }

            public void Start()
            {
                _thread.Start();
            }

            private void WorkThread()
            {
                WorkThreadImpl();
            }

            protected virtual void WorkThreadImpl()
            {
                while (true)
                {
                    ILoaderTask task;
                    Monitor.Enter(_tasks);
                    if (_tasks.Count > 0)
                    {
                        task = _tasks.Dequeue();
                    }
                    else
                    {
                        Monitor.Wait(_tasks);
                        task = _tasks.Dequeue();
                    }
                    Monitor.Exit(_tasks);
                    task.Perform();
                }
            }
        }

        class GLWorker : Worker
        {
            private readonly NativeWindow _window;

            public GLWorker(RenderWindow parentWindow, ILogger logger)
                : base(logger)
            {
                _window = new NativeWindow(
                    new NativeWindowSettings()
                    {
                        SharedContext = parentWindow.GetContext(),
                        Flags = ContextFlags.Offscreen,
                        StartVisible = false
                    });
            }

            public override void Dispose()
            {
                _window.Dispose();
            }

            protected override void WorkThreadImpl()
            {
                _window.MakeCurrent();
                base.WorkThreadImpl();
            }
        }

        private readonly ILogger _logger;
        private readonly List<IWorker> _workers = new();
        private readonly List<IWorker> _glWorkers = new();

        public Loader(RenderWindow parentWindow, int numWorkers, int numGLWorkers, ILogger logger)
        {
            _logger = logger.ForType(typeof(Loader));
            for (int i=0; i<numWorkers; ++i)
            {
                _workers.Add(new Worker(logger));
            }
            for (int i=0; i<numGLWorkers; ++i)
            {
                var worker = new GLWorker(parentWindow, logger);
                _workers.Add(worker);
                _glWorkers.Add(worker);
            }
            parentWindow.MakeCurrent();
        }

        public void QueueTask(ILoaderTask task)
        {
            if (task.IsGL)
            {
                _glWorkers.ArgMin(x => x.GetTaskCount())!.QueueTask(task);
            }
            else
            {
                _workers.ArgMin(x => x.GetTaskCount())!.QueueTask(task);
            }
        }

        public Promise<T> LoadGL<T>(Func<T> loaderFn)
        {
            return Load(loaderFn, /* isGL= */ true);
        }

        public Promise<T> Load<T>(Func<T> loaderFn)
        {
            return Load(loaderFn, /* isGL= */ false);
        }

        public Promise<T> Load<T>(Func<T> loaderFn, bool isGL)
        {
            var task = new FuncLoaderTask<T>(loaderFn, isGL);
            QueueTask(task);
            return task.GetPromise();
        }

        public void Start()
        {
            foreach (var worker in _workers)
            {
                worker.Start();
            }
            _logger.AtInfo().Log($"Started {_workers.Count} workers");
        }

        protected override void DisposeImpl()
        {
            foreach (var worker in _workers)
            {
                worker.Dispose();
            }
            _workers.Clear();
        }
    }
}
