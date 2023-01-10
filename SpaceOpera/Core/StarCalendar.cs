using SpaceOpera.Core.Advanceable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core
{
    class StarCalendar : ITickable
    {
        public int StarDate { get; private set; }

        public StarCalendar(int StartDate)
        {
            StarDate = StartDate;
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