using Cardamom.Trackers;
using SpaceOpera.Core.Designs;

namespace SpaceOpera.Core.Military
{
    public class DivisionTemplate : DesignedComponent, IFormationTemplate
    {
        public float Command => GetAttribute(ComponentAttribute.Command);
        public MultiCount<Unit> Composition { get; }

        public DivisionTemplate(
            string name, ComponentSlot slot, IEnumerable<ComponentAndSlot> components, MultiCount<ComponentTag> tags)
            : base(name, slot, components, tags)
        {
            Composition = MaterialCost
                .Where(x => x.Key is Unit).ToMultiCount(x => (Unit)x.Key, x => (int)x.Value.GetTotal());
        }
    }
}