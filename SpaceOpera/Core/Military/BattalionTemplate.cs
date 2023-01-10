using Cardamom.Utilities;
using SpaceOpera.Core.Designs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Military
{
    class BattalionTemplate : DesignedComponent, IFormationTemplate
    {
        public MultiCount<Unit> Composition { get; }

        public BattalionTemplate(
            string Name, ComponentSlot Slot, IEnumerable<ComponentAndSlot> Components, IEnumerable<ComponentTag> Tags)
            : base(Name, Slot, Components, Tags)
        {
            Composition = MaterialCost
                .Where(x => x.Key is Unit).ToMultiCount(x => (Unit)x.Key, x => (int)x.Value.GetTotal());
        }
    }
}