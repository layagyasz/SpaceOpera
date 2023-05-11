using SpaceOpera.Core.Advanceable;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Advancement
{
    public class AdvancementManager : ITickable
    {
        private readonly Dictionary<Faction, FactionAdvancementManager> _factionManagers = new();

        public void Add(Faction faction)
        {
            _factionManagers.Add(faction, new());
        }

        public FactionAdvancementManager Get(Faction faction)
        {
            return _factionManagers[faction];
        }

        public void Tick()
        {
            foreach (var factionManager in _factionManagers.Values)
            {
                factionManager.Tick();
            }
        }
    }
}
