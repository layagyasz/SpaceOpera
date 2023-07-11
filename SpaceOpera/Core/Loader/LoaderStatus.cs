using Cardamom.Trackers;

namespace SpaceOpera.Core.Loader
{
    public class LoaderStatus
    {
        public IPool Progress { get; }

        private readonly Dictionary<object, LoaderStatusSegment> _segments;
        private readonly List<string> _status = new();

        public LoaderStatus(IEnumerable<object> segments)
        {
            _segments = segments.ToDictionary(x => x, _ => new LoaderStatusSegment());
            Progress = 
                new VirtualPool(() => _segments.Count, () => _segments.Sum(x => x.Value.Progress.PercentFull()));
        }

        public void AddWork(object segment, int amount)
        {
            _segments[segment].Progress.ChangeMax(amount);
        }

        public void DoWork(object segment)
        {
            _segments[segment].Progress.Change(1);
        }

        public IEnumerable<string> GetStatus()
        {
            return _status;
        }

        public void SetStatus(object segment, string status)
        {
            _status.Insert(0, status);
            _segments[segment].SetStatus(status);
        }
    }
}
