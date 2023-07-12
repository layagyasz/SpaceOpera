﻿using Cardamom.Trackers;

namespace SpaceOpera.Core.Loader
{
    public class LoaderStatus
    {
        public IPool Progress { get; }

        private readonly Dictionary<object, LoaderStatusSegment> _segments;
        private readonly List<string> _status = new();
        private readonly int _logLength;

        public LoaderStatus(IEnumerable<object> segments, int logLength)
        {
            _segments = segments.ToDictionary(x => x, _ => new LoaderStatusSegment(logLength));
            _logLength = logLength;
            Progress = 
                new VirtualPool(
                    () => _segments.Sum(x => x.Value.Progress.MaxAmount > 0 ? x.Value.Progress.PercentFull() : 0),
                    () => _segments.Count);
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
            if (_status.Count > _logLength)
            {
                _status.RemoveRange(_logLength, _status.Count - _logLength);
            }
            _segments[segment].SetStatus(status);
        }
    }
}
