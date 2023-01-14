using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Universe
{
    class StationaryOrbitRegion : INavigable
    {
        public string Name { get; }
        public NavigableNodeType NavigableNodeType => NavigableNodeType.Space;
        public List<StellarBodySubRegion> SubRegions { get; }

        public StationaryOrbitRegion(string Name, IEnumerable<StellarBodySubRegion> SubRegions)
        {
            this.Name = Name;
            this.SubRegions = SubRegions.ToList();
        }
    }
}