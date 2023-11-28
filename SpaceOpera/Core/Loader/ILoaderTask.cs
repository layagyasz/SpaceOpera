namespace SpaceOpera.Core.Loader
{
    public interface ILoaderTask
    {
        bool IsGL { get; }
        bool IsDone();
        bool IsReady();
        IEnumerable<ILoaderTask> GetParents();
        IEnumerable<ILoaderTask> GetChildren();
        void Notify(ILoaderTask parent);
        void Perform();
    }
}
