using Cardamom.Collections;
using Cardamom.Trackers;

namespace SpaceOpera.Core.Designs
{
    public class Segment
    {
        public SegmentTemplate Template { get; }
        public SegmentConfiguration Configuration { get; private set; }

        private readonly MultiMap<DesignSlot, IComponent> _components;

        public Segment(
            SegmentTemplate template, SegmentConfiguration configuration, MultiMap<DesignSlot, IComponent> components)
        {
            Template = template;
            Configuration = configuration;
            _components = new MultiMap<DesignSlot, IComponent>(components);
        }

        public MultiMap<DesignSlot, IComponent> GetComponents()
        {
            var result = new MultiMap<DesignSlot, IComponent>(_components);
            if (Configuration.IntrinsicComponent != null)
            {
                result.Add(new DesignSlot(), Configuration.IntrinsicComponent);
            }
            return result;
        }

        public MultiCount<ComponentTag> GetTags()
        {
            return Enumerable.Concat(
                Configuration.IntrinsicComponent?.Tags ?? new(),
                _components.SelectMany(x => x.Value.SelectMany(x => x.Tags)))
                .ToMultiCount(x => x.Key, x => x.Value);
        }

        public bool Validate()
        {
            if (!Template.ConfigurationOptions.Contains(Configuration))
            {
                return false;
            }
            foreach (var slot in _components)
            {
                if (slot.Key.Count != slot.Value.Count())
                {
                    return false;
                }
                foreach (var component in slot.Value)
                {
                    if (slot.Key.Accepts(component))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}