using Cardamom.Collections;
using Cardamom.Trackers;
using SpaceOpera.Core.Advanceable;
using SpaceOpera.Core.Advancement;
using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Economics;

namespace SpaceOpera.Core.Politics
{
    public class Faction : ITickable
    {
        public string Name { get; }
        public Banner Banner { get; }
        public NameGenerator NameGenerator { get; }

        private readonly EnumMap<FactionAttribute, float> _attributes;


        private readonly AdvancementManager _advancementManager = new();

        public Faction(
            string name, Banner banner, EnumMap<FactionAttribute, float> baseAttributes, NameGenerator nameGenerator)
        {
            Name = name;
            Banner = banner;
            NameGenerator = nameGenerator;

            _attributes = new EnumMap<FactionAttribute, float>(baseAttributes);
        }

        public void AddResearch(IMaterial research, float quantity)
        {
            _advancementManager.AddResearch(research, quantity);
        }

        public float GetFleetCommand()
        {
            return _attributes[FactionAttribute.FleetCommand];
        }

        public Pool GetResearchProgress(IAdvancement Advancement)
        {
            return _advancementManager.GetResearchProgress(Advancement);
        }

        public IEnumerable<IAdvancement> GetResearchedAdvancements()
        {
            return _advancementManager.GetResearchedAdvancements();
        }

        public IEnumerable<AdvancementSlot> GetAdvancementSlots()
        {
            return _advancementManager.GetAdvancementSlots();
        }

        public bool HasPrerequisiteResearch(IAdvancement advancement)
        {
            return _advancementManager.HasPrerequisiteResearch(advancement);
        }

        public bool HasPrerequisiteResearch(IComponent component)
        {
            return component.Prerequisites.All(HasPrerequisiteResearch);
        }

        public bool HasPrerequisiteResearch(Recipe recipe)
        {
            return recipe.Prerequisites.All(HasPrerequisiteResearch);
        }

        public void Tick()
        {
            _advancementManager.Tick();
        }
    }
}