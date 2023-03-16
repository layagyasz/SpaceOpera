using SpaceOpera.Core.Advanceable;

namespace SpaceOpera.Core
{
    public class StarCalendar : IUpdateable
    {
        private long _millis;

        public StarCalendar(long millis)
        {
            _millis = millis;
        }

        public void Update(long delta)
        {
            _millis += delta;
        }

        public long GetDayOfMonth()
        {
            return _millis / 1000 % 30 + 1;
        }

        public long GetMillis()
        {
            return _millis;
        }

        public long GetMonthOfYear()
        {
            return _millis / 30000 % 12 + 1;
        }

        public long GetYear()
        {
            return _millis / 360000;
        }

        public override string ToString()
        {
            return string.Format("{0:00}/{1:00}/{2}", GetMonthOfYear(), GetDayOfMonth(), GetYear());
        }
    }
}