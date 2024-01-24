using Cardamom;
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
                        .Where(_templates.ContainsKey)
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
                    if (!nodes[node].Any())
                    {
                        open.Enqueue(node);
                        nodes.Remove(node);
                    }
                }
            }
            return result;
        }

        public DesignTemplate GetTemplate(ComponentType componentType)
        {
            return _templates[componentType];
        }

        public DesignConfiguration CreateSeries(
            AutoDesignerParameters parameters, IEnumerable<IComponent> availableComponents, Random random)
        {
            var template = _templates[parameters.Type];
            return new DesignConfiguration(
                template,
                template.Segments.Select(x => DesignSegment(x, parameters.Fitness, availableComponents, random)));
        }

        public static DesignConfiguration ContinueSeries(
            DesignSeries series, IEnumerable<IComponent> availableComponents, Random random)
        {
            return DesignWithSegmentConfigurations(
                series.GetDesignTemplate(), series.GetSegmentConfiguration(), availableComponents, random);
        }

        private static Segment DesignSegment(
            SegmentTemplate template, 
            DesignFitness fitness, 
            IEnumerable<IComponent> availableComponents,
            Random random)
        {
            return
                template.ConfigurationOptions
                    .Select(x => DesignSegmentWithConfiguration(template, x, fitness, availableComponents, random))
                    .ArgMaxRandomlySelecting(fitness.Get, random)!;
        }

        private static Segment DesignSegmentWithConfiguration(
            SegmentTemplate template,
            SegmentConfiguration configuration,
            DesignFitness fitness,
            IEnumerable<IComponent> availableComponents, 
            Random random)
        {
            var components = new MultiMap<DesignSlot, IComponent>();
            foreach (var slot in configuration.Slots)
            {
                var component =
                    availableComponents.Where(x => x.FitsSlot(slot)).ArgMaxRandomlySelecting(fitness.Get, random);
                Precondition.Check(component != null);
                components.Add(slot, Enumerable.Repeat(component!, slot.Count));
            }
            return new Segment(template, configuration, components);
        }

        private static DesignConfiguration DesignWithSegmentConfigurations(
            DesignTemplate template,
            Dictionary<SegmentTemplate, SegmentConfiguration> segmentConfigurations,
            IEnumerable<IComponent> availableComponents, 
            Random random)
        {
            var components =
                segmentConfigurations
                    .SelectMany(x => x.Value.Slots)
                    .Select(x => x)
                    .Distinct()
                    .ToDictionary(x => x, x => availableComponents.Where(y => y.FitsSlot(x)).ToList());
            var selectedComponents = components.ToDictionary(x => x.Key, x => x.Value[random.Next(0, x.Value.Count)]);

            var segments = new List<Segment>();
            foreach (var segmentConfiguration in segmentConfigurations)
            {
                var componentMap = new MultiMap<DesignSlot, IComponent>();
                foreach (var slot in segmentConfiguration.Value.Slots)
                {
                    componentMap.Add(slot, Enumerable.Repeat(selectedComponents[slot], slot.Count));
                }
                segments.Add(new Segment(segmentConfiguration.Key, segmentConfiguration.Value, componentMap));
            }

            return new DesignConfiguration(template, segments);
        }
    }
}