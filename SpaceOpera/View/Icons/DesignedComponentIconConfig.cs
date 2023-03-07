using Cardamom.Collections;
using OpenTK.Mathematics;
using SpaceOpera.Core.Designs;
using System.Text.Json.Serialization;

namespace SpaceOpera.View.Icons
{
    public class DesignedComponentIconConfig
    {
        public struct ColorConfig
        {
            public bool FromFaction { get; set; }
            public ComponentType FromComponent { get; set; }
            public Color4 Constant { get; set; }
        }

        [JsonDerivedType(typeof(ComponentLayerConfig), "FromComponent")]
        [JsonDerivedType(typeof(TagLayerConfig), "FromTag")]
        public interface ILayerConfig
        {
            ColorConfig Color { get; }
            IEnumerable<IconLayer.Definition> CreateLayers(
                DesignedComponent component, Color4 color, IconFactory iconFactory);
        }

        public class ComponentLayerConfig : ILayerConfig
        {
            public ColorConfig Color { get; set; }
            public ComponentType Component { get; set; }

            public IEnumerable<IconLayer.Definition> CreateLayers(
                DesignedComponent component, Color4 color, IconFactory iconFactory)
            {
                var c = component.Components.Where(x => x.Slot.Type.Contains(Component)).FirstOrDefault()!.Component;
                return iconFactory.GetDefinition(c).Select(x => x.WithColor(color));
            }
        }

        public class TagLayerConfig : ILayerConfig
        {
            public class TagLayerOption
            {
                public EnumSet<ComponentTag> Tags { get; set; } = new();
                public string Texture { get; set; } = string.Empty;
            }

            public ColorConfig Color { get; set; }
            public List<TagLayerOption> Options { get; set; } = new();

            public IEnumerable<IconLayer.Definition> CreateLayers(
                DesignedComponent component, Color4 color, IconFactory iconFactory)
            {
                foreach (var option in Options)
                {
                    if (option.Tags.Count == 0 || option.Tags.IsSubsetOf(component.Tags))
                    {
                        yield return new(color, option.Texture);
                        break;
                    }
                }
            }
        }

        public ComponentType ComponentType { get; set; }
        public List<ILayerConfig> Layers { get; set; } = new();

        public IEnumerable<IconLayer.Definition> CreateDefinition(
            DesignedComponent component, Color4 factionColor, IconFactory iconFactory)
        {
            foreach (var layer in Layers)
            {
                foreach (var c in layer.CreateLayers(
                    component, SelectColor(layer.Color, component, factionColor, iconFactory), iconFactory))
                {
                    yield return c;
                }
            }
        }

        private static Color4 SelectColor(
            ColorConfig config, DesignedComponent component, Color4 factionColor, IconFactory iconFactory)
        {
            if (config.FromFaction)
            {
                return factionColor;
            }
            if (config.FromComponent != ComponentType.Unknown)
            {
                return iconFactory.GetDefinition(
                    component.Components.Where(
                        x => x.Slot.Type.Contains(config.FromComponent)).FirstOrDefault()!.Component)
                    .Select(x => x.Color)
                    .First();
            }
            return config.Constant;
        }
    }
}
