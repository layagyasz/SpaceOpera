using SpaceOpera.Core.Military;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Politics
{
    class FleetIntelligence
    {
        class SingleFleetIntelligence
        {
            public double SpottingProgress { get; set; }
        }

        private readonly Dictionary<Fleet, SingleFleetIntelligence> _FleetIntelligence = 
            new Dictionary<Fleet, SingleFleetIntelligence>();

        public bool IsSpotted(Fleet Fleet)
        {
            _FleetIntelligence.TryGetValue(Fleet, out SingleFleetIntelligence intel);
            return intel != null && Math.Abs(intel.SpottingProgress - 1) < double.Epsilon;
        }

        public void Spot(Fleet Fleet, double Progress)
        {
            _FleetIntelligence.TryGetValue(Fleet, out SingleFleetIntelligence intel);
            if (intel == null)
            {
                intel = new SingleFleetIntelligence();
                _FleetIntelligence.Add(Fleet, intel);
            }
            intel.SpottingProgress = Math.Min(1, intel.SpottingProgress + Progress);
        }
    }
}