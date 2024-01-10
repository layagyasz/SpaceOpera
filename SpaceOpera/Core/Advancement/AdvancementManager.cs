using SpaceOpera.Core.Advanceable;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Politics;
using System.Collections.Immutable;

namespace SpaceOpera.Core.Advancement
{
    public class AdvancementManager : ITickable
    {
        public ImmutableList<IMaterial> ResearchTypes { get; }

        private readonly Random _random;
        private readonly List<IAdvancement> _advancements = new();
        private readonly Dictionary<Faction, FactionAdvancementManager> _factionManagers = new();

        public AdvancementManager(IEnumerable<IMaterial> researchTypes, Random random)
        {
            ResearchTypes = researchTypes.ToImmutableList();
            _random = random;
        }

        public void AddAdvancement(IAdvancement advancement)
        {
            _advancements.Add(advancement);
        }

        public void AddFaction(Faction faction)
        {
            _factionManagers.Add(faction, new(faction));
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
                SetResearchOptions(factionManager);
            }
        }

        private void SetResearchOptions(FactionAdvancementManager manager)
        {
            var options = manager.GetResearchInProgress().ToList();
            int optionCount = manager.Faction.GetResearchOptions();
            List<IAdvancement> researchable = 
                optionCount > options.Count ? GetResearchableAdvancements(manager).ToList() : new();
            while (options.Count < optionCount)
            {
                var choice = researchable[_random.Next(researchable.Count)];
                if (!options.Contains(choice))
                {
                    options.Add(choice);
                }
            }
            manager.SetResearchOptions(options);
        }

        private IEnumerable<IAdvancement> GetResearchableAdvancements(FactionAdvancementManager manager)
        {
            foreach (var advancement in _advancements)
            {
                if (manager.HasPrerequisiteResearch(advancement))
                {
                    yield return advancement;
                }
            }
        }
    }
}
