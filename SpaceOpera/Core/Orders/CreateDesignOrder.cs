using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Politics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Orders
{
    class CreateDesignOrder : IImmediateOrder
    {
        public Faction Faction { get; }
        public DesignConfiguration Design { get; }

        public CreateDesignOrder(Faction Faction, DesignConfiguration Design)
        {
            this.Faction = Faction;
            this.Design = Design;
        }

        public ValidationFailureReason Validate()
        {
            return Design.Validate() ? ValidationFailureReason.NONE : ValidationFailureReason.INVALID_DESIGN;
        }

        public bool Execute(World World)
        {
            var design = World.GameData.DesignBuilder.Build(Design);
            World.AddDesign(design);
            World.AddLicense(new DesignLicense(Faction, design));
            return true;
        }
    }
}