using SpaceOpera.Core.Advanceable;

namespace SpaceOpera.Core
{
    public class StarCalendar : ITickable
    {
        public int StarDate { get; private set; }

        public StarCalendar(int startDate)
        {
            StarDate = startDate;
        }

        public void Tick()
        {
            StarDate++;
        }

        public int GetDayOfMonth()
        {
            return StarDate % 30;
        }

        public int GetMonthOfYear()
        {
            return (StarDate / 30) % 12;
        }

        public int GetYear()
        {
            return StarDate / 360;
        }
    }
}