using Cardamom.Collections;
using Cardamom.Trackers;
using SpaceOpera.Core.Advanceable;
using SpaceOpera.Core.Advancement;
using SpaceOpera.Core.Economics;

namespace SpaceOpera.Core.Politics
{
    public class Faction : ITickable
    {
        public string Name { get; }
        public Banner Banner { get; }
        public NameGenerator NameGenerator { get; }

        private EnumMap<FactionAttribute, float> _Attributes;


        private AdvancementManager _AdvancementManager = new AdvancementManager();

        public Faction(
            string name, Banner banner, EnumMap<FactionAttribute, float> baseAttributes, NameGenerator nameGenerator)
        {
            Name = name;
            Banner = banner;
            NameGenerator = nameGenerator;

            _Attributes = new EnumMap<FactionAttribute, float>(baseAttributes);
        }

        public void AddResearch(IMaterial research, float quantity)
        {
            _AdvancementManager.AddResearch(research, quantity);
        }

        public float GetFleetCommand()
        {
            return _Attributes[FactionAttribute.FleetCommand];
        }

        public Pool GetResearchProgress(IAdvancement Advancement)
        {
            return _AdvancementManager.GetResearchProgress(Advancement);
        }

        public IEnumerable<IAdvancement> GetResearchedAdvancements()
        {
            return _AdvancementManager.GetResearchedAdvancements();
        }

        public IEnumerable<AdvancementSlot> GetAdvancementSlots()
        {
            return _AdvancementManager.GetAdvancementSlots();
        }

        public bool HasPrerequisiteResearch(IAdvancement advancement)
        {
            return _AdvancementManager.HasPrerequisiteResearch(advancement);
        }

        public void Tick()
        {
            _AdvancementManager.Tick();
        }
    }
}