using Cardamom;
using Cardamom.Trackers;
using SpaceOpera.Core.Advanceable;
using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Advancement
{
    public class FactionAdvancementManager : ITickable
    {
        public Faction Faction { get; }

        private readonly List<IAdvancement> _researchOptions = new();
        private readonly HashSet<IAdvancement> _researchedAdvancements = new();
        private readonly Dictionary<IAdvancement, Pool> _advancementProgress = new();

        private readonly List<AdvancementSlot> _advancementSlots = 
            new()
            {
                new AdvancementSlot(0),
                new AdvancementSlot(1),
                new AdvancementSlot(2)
            };

        private readonly MultiQuantity<IMaterial> _backlogProgress = new();

        public FactionAdvancementManager(Faction faction)
        {
            Faction = faction;
        }

        public IEnumerable<IAdvancement> GetResearchedAdvancements()
        {
            return _researchedAdvancements;
        }

        public void Research(IAdvancement advancement)
        {
            _researchedAdvancements.Add(advancement);
        }

        public bool HasPrerequisiteResearch(IComponent component)
        {
            return component.Prerequisites.All(HasResearched);
        }

        public bool HasPrerequisiteResearch(Recipe recipe)
        {
            return recipe.Prerequisites.All(HasResearched);
        }

        public bool HasPrerequisiteResearch(IAdvancement advancement)
        {
            return advancement.Prerequisites.All(HasResearched);
        }

        public bool IsResearching(IAdvancement advancement)
        {
            return GetAdvancementSlots().Any(x => x.Advancement == advancement);
        }

        public void AddResearch(IMaterial Material, float Quantity)
        {
            _backlogProgress.Add(Material, Quantity);
        }

        public Pool GetResearchProgress(IAdvancement advancement)
        {
            return GetOrCreateProgress(advancement, addIfMissing: false);
        }

        public IEnumerable<AdvancementSlot> GetAdvancementSlots()
        {
            return _advancementSlots;
        }

        public IEnumerable<IAdvancement> GetResearchInProgress()
        {
            return _advancementProgress.Keys.Concat(
                _advancementSlots.Select(x => x.Advancement).Where(x => x != null)).Cast<IAdvancement>().Distinct();
        }

        public IEnumerable<IAdvancement> GetResearchOptions()
        {
            return _researchOptions;
        }

        public void SetResearchOptions(IEnumerable<IAdvancement> options)
        {
            _researchOptions.Clear();
            _researchOptions.AddRange(options);
        }

        public void Tick()
        {
            Dictionary<IMaterial, int> counts = 
                _advancementSlots
                    .Where(x => x.Advancement != null)
                    .GroupBy(x => x.Advancement!.Type!)
                    .ToDictionary(x => x.Key, x => x.Count());
            foreach (var advancement in _advancementSlots)
            {
                if (advancement.Advancement == null)
                {
                    continue;
                }
                var material = advancement.Advancement?.Type!;
                AddProgress(advancement.Advancement!, _backlogProgress[material] / counts[material]);
            }

            _backlogProgress.Clear();
        }

        public void AddProgress(IAdvancement advancement, float progress)
        {
            if (HasResearched(advancement))
            {
                return;
            }
            var pool = GetOrCreateProgress(advancement, addIfMissing: true);
            pool.Change(progress);
            if (pool.IsFull())
            {
                _researchedAdvancements.Add(advancement);
                _advancementProgress.Remove(advancement);
            }
        }

        private Pool GetOrCreateProgress(IAdvancement advancement, bool addIfMissing)
        {
            if (!_advancementProgress.TryGetValue(advancement, out var progress))
            {
                progress = new(advancement.Cost, startFull: false);
                if (addIfMissing)
                {
                    _advancementProgress.Add(advancement, progress);
                }
            }
            return progress;
        }

        private bool HasResearched(IAdvancement advancement)
        {
            return _researchedAdvancements.Contains(advancement);
        }
    }
}