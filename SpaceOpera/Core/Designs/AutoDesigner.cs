using Cardamom.Collections;

namespace SpaceOpera.Core.Designs
{
    public class AutoDesigner
    {
        private readonly EnumMap<ComponentType, DesignTemplate> _templates;

        public AutoDesigner(IEnumerable<DesignTemplate> templates)
        {
            _templates = templates.ToEnumMap(x => x.Type, x => x);
        }

        public List<ComponentType> GetDesignOrder()
        {
            var nodes = new MultiMap<ComponentType, ComponentType>();
            var open = new Queue<ComponentType>();
            foreach (var template in  _templates)
            {
                var parents =
                    template.Value.Segments
                        .SelectMany(x => x.ConfigurationOptions)
                        .SelectMany(x => x.Slots)
                        .SelectMany(x => x.Type)
                        .Where(x => _templates.ContainsKey(x))
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
            AutoDesignerParameters parameters, IEnumerable<IComponent> availableComponents, Random random)
        {
            var template = _templates[parameters.Type];
            return new DesignConfiguration(
                template, template.Segments.Select(x => DesignSegment(x, parameters, availableComponents, random)));
        }

        public DesignConfiguration ContinueSeries(
            DesignSeries series, IEnumerable<IComponent> availableComponents, Random random)
        {
            return DesignWithSegmentConfigurations(
                series.GetDesignTemplate(), series.GetSegmentConfiguration(), availableComponents, random);
        }

        private Segment DesignSegment(
            SegmentTemplate template, 
            AutoDesignerParameters parameters, 
            IEnumerable<IComponent> availableComponents,
            Random Random)
        {
            return
                template.ConfigurationOptions
                    .Select(x => DesignSegmentWithConfiguration(template, x, parameters, availableComponents, Random))
                    .ArgMaxRandomlySelecting(parameters.GetFitness, Random);
        }

        private Segment DesignSegmentWithConfiguration(
            SegmentTemplate template,
            SegmentConfiguration configuration,
            AutoDesignerParameters parameters,
            IEnumerable<IComponent> availableComponents, 
            Random Random)
        {
            var components = new MultiMap<DesignSlot, IComponent>();
            foreach (var slot in configuration.Slots)
            {
                components.Add(
                    slot, 
                    Enumerable.Repeat(
                        availableComponents
                            .Where(x => x.FitsSlot(slot)).ArgMaxRandomlySelecting(parameters.GetFitness, Random),
                        slot.Count));
            }
            return new Segment(template, configuration, components);
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