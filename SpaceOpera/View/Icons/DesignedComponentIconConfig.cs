using Cardamom.Collections;
using OpenTK.Mathematics;
using SpaceOpera.Core.Designs;
using SpaceOpera.View.BannerViews;
using System.Text.Json.Serialization;

namespace SpaceOpera.View.Icons
{
    public class DesignedComponentIconConfig
    {
        public struct ColorConfig
        {
            public BannerColor FromFaction { get; set; }
            public ComponentType FromComponent { get; set; }
            public Color4 Constant { get; set; }
        }

        [JsonDerivedType(typeof(ComponentLayerConfig), "FromComponent")]
        [JsonDerivedType(typeof(TagLayerConfig), "FromTag")]
        [JsonDerivedType(typeof(SizeLayerConfig), "FromSize")]
        [JsonDerivedType(typeof(StaticLayerConfig), "Static")]
        public interface ILayerConfig
        {
            ColorConfig Color { get; }
            IEnumerable<IconLayer> CreateLayers(
                DesignedComponent component, Color4 color, IconFactory iconFactory);
        }

        public class ComponentLayerConfig : ILayerConfig
        {
            public ColorConfig Color { get; set; }
            public ComponentType Component { get; set; }
            public bool IsInfo { get; set; }

            public IEnumerable<IconLayer> CreateLayers(
                DesignedComponent component, Color4 color, IconFactory iconFactory)
            {
                var c = 
                    component.Components.Where(x => x.Component.Slot.Type == Component).FirstOrDefault()!.Component;
                return iconFactory.GetDefinition(c)
                    .Where(x => !x.IsInfo).Select(x => x.WithColor(color)).Select(x => x.WithInfo(IsInfo));
            }
        }

        public class StaticLayerConfig : ILayerConfig
        {
            public ColorConfig Color { get; set; }
            public List<string> Textures { get; set; } = new();
            public bool IsInfo { get; set; }

            public IEnumerable<IconLayer> CreateLayers(
                DesignedComponent component, Color4 color, IconFactory iconFactory)
            {
                foreach (var texture in Textures)
                {
                    yield return new(null, color, texture, IsInfo);
                }
            }
        }

        public class SizeLayerConfig : ILayerConfig
        {
            public class SizeLayerOption
            {
                public EnumSet<ComponentSize> Sizes { get; set; } = new();
                public List<string> Textures { get; set; } = new();
            }

            public ColorConfig Color { get; set; }
            public List<SizeLayerOption> Options { get; set; } = new();
            public bool IsInfo { get; set; }

            public IEnumerable<IconLayer> CreateLayers(
                DesignedComponent component, Color4 color, IconFactory iconFactory)
            {
                foreach (var option in Options)
                {
                    if (option.Sizes.Contains(component.Slot.Size))
                    {
                        foreach (var texture in option.Textures)
                        {
                            yield return new(null, color, texture, IsInfo);
                        }
                        break;
                    }
                }
            }
        }

        public class TagLayerConfig : ILayerConfig
        {
            public class TagLayerOption
            {
                public EnumSet<ComponentTag> Tags { get; set; } = new();
                public List<string> Textures { get; set; } = new();
            }

            public ColorConfig Color { get; set; }
            public List<TagLayerOption> Options { get; set; } = new();
            public bool IsInfo { get; set; }

            public IEnumerable<IconLayer> CreateLayers(
                DesignedComponent component, Color4 color, IconFactory iconFactory)
            {
                foreach (var option in Options)
                {
                    if (option.Tags.IsSubsetOf(component.Tags.Keys))
                    {
                        foreach (var texture in option.Textures)
                        {
                            yield return new(null, color, texture, IsInfo);
                        }
                        break;
                    }
                }
            }
        }

        public ComponentType ComponentType { get; set; }
        public List<ILayerConfig> Layers { get; set; } = new();

        public IEnumerable<IconLayer> CreateDefinition(
            DesignedComponent component, BannerColorSet factionColor, IconFactory iconFactory)
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
            ColorConfig config, DesignedComponent component, BannerColorSet factionColor, IconFactory iconFactory)
        {
            if (config.FromFaction != BannerColor.None)
            {
                return factionColor.Get(config.FromFaction);
            }
            if (config.FromComponent != ComponentType.Unknown)
            {
                return iconFactory.GetDefinition(
                    component.Components.Where(
                        x => x.Component.Slot.Type == config.FromComponent).FirstOrDefault()!.Component)
                    .Select(x => x.Color)
                    .First();
            }
            return config.Constant;
        }
    }
}
