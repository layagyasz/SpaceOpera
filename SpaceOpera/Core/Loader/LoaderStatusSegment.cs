using Cardamom.Trackers;

namespace SpaceOpera.Core.Loader
{
    public class LoaderStatusSegment
    {
        public IntPool Progress { get; } = new(0);

        private readonly List<string> _status = new();

        public void SetStatus(string status)
        {
            _status.Insert(0, status);
        }
    }
}
