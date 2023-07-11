using Cardamom.Utils.Suppliers;

namespace SpaceOpera.Core.Loader
{
    public class FuncLoaderTask<T> : ILoaderTask
    {
        private readonly Func<T> _func;
        private readonly Promise<T> _promise = new();

        public FuncLoaderTask(Func<T> func)
        {
            _func = func;
        }

        public Promise<T> GetPromise()
        {
            return _promise;
        }

        public bool IsDone()
        {
            return _promise.HasValue();
        }

        public void Perform()
        {
            _promise.Set(_func.Invoke());
        }
    }
}
