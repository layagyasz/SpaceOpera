using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Universe
{
    class SolarOrbitRegion : INavigable
    {
        public string Name => string.Format("{0} Orbit", LocalOrbit.StellarBody.Name);
        public NavigableNodeType NavigableNodeType => NavigableNodeType.Space;
        public LocalOrbitRegion LocalOrbit { get; }

        public SolarOrbitRegion(LocalOrbitRegion LocalOrbit)
        {
            this.LocalOrbit = LocalOrbit;
        }
    }
}