using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Designs
{
    class Segment
    {
        public SegmentTemplate Template { get; }
        public SegmentConfiguration Configuration { get; private set; }

        private readonly MultiMap<DesignSlot, IComponent> _Components;

        public Segment(
            SegmentTemplate Template, SegmentConfiguration Configuration, MultiMap<DesignSlot, IComponent> Components)
        {
            this.Template = Template;
            this.Configuration = Configuration;
            _Components = new MultiMap<DesignSlot, IComponent>(Components);
        }

        public MultiMap<DesignSlot, IComponent> GetComponents()
        {
            var result = new MultiMap<DesignSlot, IComponent>(_Components);
            if (Configuration.IntrinsicComponent != null)
            {
                result.Add(new DesignSlot(), Configuration.IntrinsicComponent);
            }
            return result;
        }

        public bool Validate()
        {
            if (!Template.ConfigurationOptions.Contains(Configuration))
            {
                return false;
            }
            foreach (var slot in _Components)
            {
                if (slot.Key.Count != slot.Value.Count())
                {
                    return false;
                }
                foreach (var component in slot.Value)
                {
                    if (!component.FitsSlot(slot.Key))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}