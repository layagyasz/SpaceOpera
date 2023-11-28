using Cardamom.Utils.Suppliers;

namespace SpaceOpera.Core.Loader
{
    public abstract class LoaderTaskNode<T> : ILoaderTask
    {
        public bool IsGL { get; }

        private readonly List<ILoaderTask> _children = new();

        protected readonly Promise<T> _promise = new();

        protected LoaderTaskNode(bool isGL)
        {
            IsGL = isGL;
        }

        public void AddChild(ILoaderTask node)
        {
            _children.Add(node);
        }

        public Promise<T> GetPromise()
        {
            return _promise;
        }

        public bool IsDone()
        {
            return _promise.HasValue();
        }

        public IEnumerable<ILoaderTask> GetChildren()
        {
            return _children;
        }

        public abstract IEnumerable<ILoaderTask> GetParents();

        public abstract bool IsReady();

        public abstract void Notify(ILoaderTask parent);

        public abstract void Perform();
    }
}
