using SpaceOpera.Core.Military;

namespace SpaceOpera.Core.Politics
{
    public class FleetIntelligence
    {
        class SingleFleetIntelligence
        {
            public double SpottingProgress { get; set; }
        }

        private readonly Dictionary<Fleet, SingleFleetIntelligence> _fleetIntelligence = new();

        public bool IsSpotted(Fleet fleet)
        {
            _fleetIntelligence.TryGetValue(fleet, out var intel);
            return intel != null && Math.Abs(intel.SpottingProgress - 1) < double.Epsilon;
        }

        public void Spot(Fleet fleet, double progress)
        {
            _fleetIntelligence.TryGetValue(fleet, out var intel);
            if (intel == null)
            {
                intel = new SingleFleetIntelligence();
                _fleetIntelligence.Add(fleet, intel);
            }
            intel.SpottingProgress = Math.Min(1, intel.SpottingProgress + progress);
        }
    }
}