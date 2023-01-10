using SpaceOpera.Core.Advanceable;
using SpaceOpera.Core.Economics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Advancement
{
    class AdvancementManager : ITickable
    {
        private readonly HashSet<IAdvancement> _ResearchedAdvancements = new HashSet<IAdvancement>();

        private readonly Dictionary<IAdvancement, Pool> _AdvancementProgress = new Dictionary<IAdvancement, Pool>();
        private readonly List<AdvancementSlot> _AdvancementSlots = 
            new List<AdvancementSlot>()
            {
                new AdvancementSlot(0),
                new AdvancementSlot(1),
                new AdvancementSlot(2)
            };

        private readonly MultiQuantity<IMaterial> _BacklogProgress = new MultiQuantity<IMaterial>();

        public IEnumerable<IAdvancement> GetResearchedAdvancements()
        {
            return _ResearchedAdvancements;
        }

        public void Research(IAdvancement Advancement)
        {
            _ResearchedAdvancements.Add(Advancement);
        }

        public bool HasPrerequisiteResearch(IResearchable Researchable)
        {
            return Researchable.Prerequisites == null
                || Researchable.Prerequisites.Count() == 0 
                || Researchable.Prerequisites.All(_ResearchedAdvancements.Contains);
        }

        public void AddResearch(IMaterial Material, float Quantity)
        {
            _BacklogProgress.Add(Material, Quantity);
        }

        public Pool GetResearchProgress(IAdvancement Advancement)
        {
            return GetOrCreateProgress(Advancement);
        }

        public IEnumerable<AdvancementSlot> GetAdvancementSlots()
        {
            return _AdvancementSlots;
        }

        public void Tick()
        {
            Dictionary<IMaterial, int> counts = 
                _AdvancementSlots
                .Where(x => x.Advancement != null)
                .GroupBy(x => x.Advancement.Type.Research)
                .ToDictionary(x => x.Key, x => x.Count());
            foreach (var advancement in _AdvancementSlots)
            {
                if (advancement.Advancement == null)
                {
                    continue;
                }
                var material = advancement.Advancement.Type.Research;
               AddProgress(advancement.Advancement, _BacklogProgress[material] / counts[material]);
            }

            _BacklogProgress.Clear();
        }

        public void AddProgress(IAdvancement Advancement, float Progress)
        {
            var progress = GetOrCreateProgress(Advancement);
            progress.ChangeAmount(Progress);
            if (progress.IsFull())
            {
                _ResearchedAdvancements.Add(Advancement);
            }
        }

        private Pool GetOrCreateProgress(IAdvancement Advancement)
        {
            _AdvancementProgress.TryGetValue(Advancement, out Pool progress);
            if (progress == null)
            {
                progress = new Pool(Advancement.Cost);
                _AdvancementProgress.Add(Advancement, progress);
            }
            return progress;
        }
    }
}