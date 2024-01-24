using Cardamom.Trackers;
using SpaceOpera.Core.Designs;

namespace SpaceOpera.Core.Military
{
    public class BattalionTemplate : DesignedComponent, IFormationTemplate
    {
        public MultiCount<Unit> Composition { get; }

        public BattalionTemplate(
            string name, ComponentSlot slot, IEnumerable<ComponentAndWeight> components, MultiCount<ComponentTag> tags)
            : base(name, slot, components, tags)
        {
            Composition = MaterialCost
                .Where(x => x.Key is Unit).ToMultiCount(x => (Unit)x.Key, x => (int)x.Value.GetTotal());
        }
    }
}