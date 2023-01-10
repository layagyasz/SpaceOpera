using Adansonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Military
{
    class SpaceOperaContext : SimpleContext
    {
        public class FleetContext : SpaceOperaContext
        {
            public FleetDriver Fleet { get; }

            internal FleetContext(World World, FleetDriver Fleet)
                : base(World)
            {
                this.Fleet = Fleet;
            }
        }

        public World World { get; }

        public SpaceOperaContext(World World)
        {
            this.World = World;
        }

        public FleetContext ForFleet(FleetDriver Fleet)
        {
            return new FleetContext(World, Fleet);
        }
    }
}
