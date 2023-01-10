using SpaceOpera.Core.Advanceable;
using SpaceOpera.Core.Advancement;
using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Military;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Politics
{
    class Faction : ITickable
    {
        public string Name { get; }
        public Banner Banner { get; }
        public NameGenerator NameGenerator { get; }

        private EnumMap<FactionAttribute, float> _Attributes;


        private AdvancementManager _AdvancementManager = new AdvancementManager();

        public Faction(
            string Name, Banner Banner, EnumMap<FactionAttribute, float> BaseAttributes, NameGenerator NameGenerator)
        {
            this.Name = Name;
            this.Banner = Banner;
            this.NameGenerator = NameGenerator;

            this._Attributes = new EnumMap<FactionAttribute, float>(BaseAttributes);
        }

        public void AddResearch(IMaterial Research, float Quantity)
        {
            _AdvancementManager.AddResearch(Research, Quantity);
        }

        public float GetFleetCommand()
        {
            return _Attributes[FactionAttribute.FLEET_COMMAND];
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

        public bool HasPrerequisiteResearch(IResearchable Researchable)
        {
            return _AdvancementManager.HasPrerequisiteResearch(Researchable);
        }

        public void Tick()
        {
            _AdvancementManager.Tick();
        }
    }
}