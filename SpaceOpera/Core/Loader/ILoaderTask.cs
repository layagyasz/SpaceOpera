namespace SpaceOpera.Core.Loader
{
    public interface ILoaderTask
    {
        bool IsGL { get; }
        bool IsDone();
        void Perform();
    }
}
