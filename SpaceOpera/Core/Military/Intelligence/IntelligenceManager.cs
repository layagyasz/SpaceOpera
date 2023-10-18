using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Military.Intelligence
{
    public class IntelligenceManager
    {
        private readonly Dictionary<Faction, FactionIntelligence> _intel = new();

        public void Add(Faction faction)
        {
            _intel.Add(faction, new());
        }

        public FactionIntelligence Get(Faction faction)
        {
            return _intel[faction];
        }
    }
}
