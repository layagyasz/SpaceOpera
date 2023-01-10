using SpaceOpera.Core.Advanceable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Military
{
    class FleetManager
    {
        private readonly Dictionary<Fleet, FleetDriver> _Drivers = new Dictionary<Fleet, FleetDriver>();

        public void AddFleet(Fleet Fleet)
        {
            _Drivers.Add(Fleet, new FleetDriver(Fleet));
        }

        public FleetDriver GetDriver(Fleet Fleet)
        {
            return _Drivers[Fleet];
        }

        public void Tick(World World)
        {
            var context = new SpaceOperaContext(World);
            foreach (var fleet in _Drivers.Values)
            {
                fleet.Tick(context);
            }
        }
    }
}