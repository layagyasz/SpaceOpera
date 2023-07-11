namespace SpaceOpera.Core.Loader
{
    public interface ILoaderTask
    {
        bool IsDone();
        void Perform();
    }
}
