using Cardamom.Utils.Suppliers.Promises;

namespace SpaceOpera.Core.Loader
{
    public abstract class LoaderTaskNode<T> : ILoaderTask
    {
        public bool IsGL { get; }

        private readonly List<ILoaderTask> _children = new();

        protected readonly RemotePromise<T> _promise = new RemotePromise<T>();

        protected LoaderTaskNode(bool isGL)
        {
            IsGL = isGL;
        }

        public void AddChild(ILoaderTask node)
        {
            _children.Add(node);
        }

        public IPromise<T> GetPromise()
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
