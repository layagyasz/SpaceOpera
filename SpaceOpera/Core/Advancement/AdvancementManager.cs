using SpaceOpera.Core.Advanceable;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Politics;
using System.Collections.Immutable;

namespace SpaceOpera.Core.Advancement
{
    public class AdvancementManager : ITickable
    {
        public ImmutableList<IMaterial> Research { get; }

        private readonly Dictionary<Faction, FactionAdvancementManager> _factionManagers = new();

        public AdvancementManager(IEnumerable<IMaterial> research)
        {
            Research = research.ToImmutableList();
        }

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
