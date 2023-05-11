using Cardamom.Trackers;
using SpaceOpera.Core.Advanceable;
using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Economics;

namespace SpaceOpera.Core.Advancement
{
    public class FactionAdvancementManager : ITickable
    {
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
            return component.Prerequisites.All(HasPrerequisiteResearch);
        }

        public bool HasPrerequisiteResearch(Recipe recipe)
        {
            return recipe.Prerequisites.All(HasPrerequisiteResearch);
        }

        public bool HasPrerequisiteResearch(IAdvancement advancement)
        {
            return advancement.Prerequisites == null
                || advancement.Prerequisites.Count() == 0 
                || advancement.Prerequisites.All(_researchedAdvancements.Contains);
        }

        public void AddResearch(IMaterial Material, float Quantity)
        {
            _backlogProgress.Add(Material, Quantity);
        }

        public Pool GetResearchProgress(IAdvancement advancement)
        {
            return GetOrCreateProgress(advancement);
        }

        public IEnumerable<AdvancementSlot> GetAdvancementSlots()
        {
            return _advancementSlots;
        }

        public void Tick()
        {
            Dictionary<IMaterial, int> counts = 
                _advancementSlots
                    .Where(x => x.Advancement != null)
                    .GroupBy(x => x.Advancement?.Type!.Research!)
                    .ToDictionary(x => x.Key, x => x.Count());
            foreach (var advancement in _advancementSlots)
            {
                if (advancement.Advancement == null)
                {
                    continue;
                }
                var material = advancement.Advancement?.Type!.Research!;
                AddProgress(advancement.Advancement!, _backlogProgress[material] / counts[material]);
            }

            _backlogProgress.Clear();
        }

        public void AddProgress(IAdvancement advancement, float progress)
        {
            var pool = GetOrCreateProgress(advancement);
            pool.Change(progress);
            if (pool.IsFull())
            {
                _researchedAdvancements.Add(advancement);
            }
        }

        private Pool GetOrCreateProgress(IAdvancement advancement)
        {
            if (!_advancementProgress.TryGetValue(advancement, out var progress))
            {
                progress = new(advancement.Cost);
                _advancementProgress.Add(advancement, progress);
            }
            return progress;
        }
    }
}