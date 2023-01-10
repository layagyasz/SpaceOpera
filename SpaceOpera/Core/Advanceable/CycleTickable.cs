using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Advanceable
{
    class CycleTickable : ITickable
    {
        public int CycleLength { get; }

        private ITickable _Tickable;
        private int _Progress;

        public CycleTickable(ITickable Tickable, int CycleLength)
        {
            this.CycleLength = CycleLength;
            _Tickable = Tickable;
        }

        public void Tick()
        {
            if (_Progress++ == CycleLength)
            {
                _Progress = 0;
                _Tickable.Tick();
            }
        }
    }
}