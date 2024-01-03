using SpaceOpera.Core.Advanceable;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Politics;
using System.Collections.Immutable;

namespace SpaceOpera.Core.Advancement
{
    public class AdvancementManager : ITickable
    {
        public ImmutableList<IMaterial> ResearchTypes { get; }

        private readonly List<IAdvancement> _advancements = new();
        private readonly Dictionary<Faction, FactionAdvancementManager> _factionManagers = new();

        public AdvancementManager(IEnumerable<IMaterial> researchTypes)
        {
            ResearchTypes = researchTypes.ToImmutableList();
        }

        public void AddAdvancement(IAdvancement advancement)
        {
            _advancements.Add(advancement);
        }

        public void AddFaction(Faction faction)
        {
            _factionManagers.Add(faction, new());
        }

        public FactionAdvancementManager Get(Faction faction)
        {
            return _factionManagers[faction];
        }

        public IEnumerable<IAdvancement> GetResearchableAdvancements(Faction faction)
        {
            var manager = Get(faction);
            foreach (var advancement in _advancements)
            {
                if (manager.HasPrerequisiteResearch(advancement))
                {
                    yield return advancement;
                }
            }
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
