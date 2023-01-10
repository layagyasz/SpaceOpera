using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Designs
{
    class AutoDesigner
    {
        private readonly EnumMap<ComponentType, DesignTemplate> _Templates;

        public AutoDesigner(IEnumerable<DesignTemplate> Templates)
        {
            _Templates = Templates.ToEnumMap(x => x.Type, x => x);
        }

        public List<ComponentType> GetDesignOrder()
        {
            var nodes = new MultiMap<ComponentType, ComponentType>();
            var open = new Queue<ComponentType>();
            foreach (var template in  _Templates)
            {
                var parents =
                    template.Value.Segments
                        .SelectMany(x => x.ConfigurationOptions)
                        .SelectMany(x => x.Slots)
                        .SelectMany(x => x.Type)
                        .Where(x => _Templates.ContainsKey(x))
                        .ToHashSet();
                if (parents.Count == 0)
                {
                    open.Enqueue(template.Key);
                }
                else
                {
                    nodes.Add(template.Key, parents);
                }
            }

            var result = new List<ComponentType>();
            while (open.Count > 0)
            {
                var current = open.Dequeue();
                result.Add(current);
                nodes.Remove(current);
                foreach (var node in nodes.Keys.ToList())
                {
                    nodes.RemoveAll(node, x => x == current);
                    if (nodes[node].Count() == 0)
                    {
                        open.Enqueue(node);
                        nodes.Remove(node);
                    }
                }
            }
            return result;
        }

        public DesignConfiguration CreateSeries(
            AutoDesignerParameters Parameters, IEnumerable<IComponent> AvailableComponents, Random Random)
        {
            var template = _Templates[Parameters.Type];
            return new DesignConfiguration(
                template, template.Segments.Select(x => DesignSegment(x, Parameters, AvailableComponents, Random)));
        }

        public DesignConfiguration ContinueSeries(DesignSeries Series, IEnumerable<IComponent> AvailableComponents, Random Random)
        {
            return DesignWithSegmentConfigurations(
                Series.GetDesignTemplate(), Series.GetSegmentConfiguration(), AvailableComponents, Random);
        }

        private Segment DesignSegment(
            SegmentTemplate Template, 
            AutoDesignerParameters Parameters, 
            IEnumerable<IComponent> AvailableComponents,
            Random Random)
        {
            return
                Template.ConfigurationOptions
                    .Select(x => DesignSegmentWithConfiguration(Template, x, Parameters, AvailableComponents, Random))
                    .ArgMaxRandomlySelecting(Parameters.GetFitness, Random);
        }

        private Segment DesignSegmentWithConfiguration(
            SegmentTemplate Template,
            SegmentConfiguration Configuration,
            AutoDesignerParameters Parameters,
            IEnumerable<IComponent> AvailableComponents, 
            Random Random)
        {
            var components = new MultiMap<DesignSlot, IComponent>();
            foreach (var slot in Configuration.Slots)
            {
                components.Add(
                    slot, 
                    Enumerable.Repeat(
                        AvailableComponents
                            .Where(x => x.FitsSlot(slot)).ArgMaxRandomlySelecting(Parameters.GetFitness, Random),
                        slot.Count));
            }
            return new Segment(Template, Configuration, components);
        }

        private DesignConfiguration DesignWithSegmentConfigurations(
            DesignTemplate Template,
            Dictionary<SegmentTemplate, SegmentConfiguration> SegmentConfigurations,
            IEnumerable<IComponent> AvailableComponents, 
            Random Random)
        {
            var components =
                SegmentConfigurations
                    .SelectMany(x => x.Value.Slots)
                    .Select(x => x)
                    .Distinct()
                    .ToDictionary(x => x, x => AvailableComponents.Where(y => y.FitsSlot(x)).ToList());
            var selectedComponents = components.ToDictionary(x => x.Key, x => x.Value[Random.Next(0, x.Value.Count)]);

            var segments = new List<Segment>();
            foreach (var segmentConfiguration in SegmentConfigurations)
            {
                var componentMap = new MultiMap<DesignSlot, IComponent>();
                foreach (var slot in segmentConfiguration.Value.Slots)
                {
                    componentMap.Add(slot, Enumerable.Repeat(selectedComponents[slot], slot.Count));
                }
                segments.Add(new Segment(segmentConfiguration.Key, segmentConfiguration.Value, componentMap));
            }

            return new DesignConfiguration(Template, segments);
        }
    }
}