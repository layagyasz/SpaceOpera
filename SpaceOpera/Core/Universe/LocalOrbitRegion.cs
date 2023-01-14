using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Universe
{
    class LocalOrbitRegion : INavigable
    {
        public string Name => string.Format("Local {0} Orbit", StellarBody.Name);
        public NavigableNodeType NavigableNodeType => NavigableNodeType.Space;
        public StellarBody StellarBody { get; }

        public LocalOrbitRegion(StellarBody StellarBody)
        {
            this.StellarBody = StellarBody;
        }
    }
}