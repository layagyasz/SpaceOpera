using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Designs
{
    class SimpleComponentDesigner : IComponentDesigner
    {
        public List<IComponent> AvailableComponents { get; }
        public Random Random { get; }

        public SimpleComponentDesigner(IEnumerable<IComponent> AvailableComponents, Random Random)
        {
            this.AvailableComponents = AvailableComponents.ToList();
            this.Random = Random;
        }

        public DesignConfiguration Design(DesignTemplate Template)
        {
            var segments = new List<Segment>();
            foreach (var segmentTemplate in Template.Segments)
            {
                var segmentConfiguration = segmentTemplate.ConfigurationOptions.First();
                var components = new MultiMap<DesignSlot, IComponent>();
                foreach (var slot in segmentConfiguration.Slots)
                {
                    var validComponents = AvailableComponents.Where(x => x.FitsSlot(slot)).ToList();
                    var component = validComponents[Random.Next(0, validComponents.Count)];
                    for (int i = 0; i < slot.Count; ++i)
                    {
                        components.Add(slot, component);
                    }
                }
                segments.Add(new Segment(segmentTemplate, segmentConfiguration, components));
            }
            var design = new DesignConfiguration(Template, segments);
            return design;
        }
    }
}