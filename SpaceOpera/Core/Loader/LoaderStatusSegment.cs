using Cardamom.Trackers;

namespace SpaceOpera.Core.Loader
{
    public class LoaderStatusSegment
    {
        public IntPool Progress { get; } = new(0);

        private readonly List<string> _status = new();
        private readonly int _logLength;

        public LoaderStatusSegment(int logLength)
        {
            _logLength = logLength;
        }

        public void SetStatus(string status)
        {
            _status.Insert(0, status);
            if (_status.Count > _logLength)
            {
                _status.RemoveRange(_logLength, _status.Count - _logLength);
            }
        }
    }
}
