using SpaceOpera.Core.Designs;

namespace SpaceOpera.Core.Military
{
    public readonly struct Armor
    {
        public float Thickness { get; }
        public float Protection { get; }
        public float Coverage { get; }

        public Armor(float thickness, float protection, float coverage)
        {
            Thickness = thickness;
            Protection = protection;
            Coverage = coverage;
        }

        public static Armor FromComponent(IComponent component)
        {
            return new Armor(
                component.GetAttribute(ComponentAttribute.ArmorThickness),
                component.GetAttribute(ComponentAttribute.ArmorProtection), 
                component.GetAttribute(ComponentAttribute.ArmorCoverage));
        }

        public static Armor Combine(IEnumerable<Armor> armors)
        {
            return new Armor(
                armors.Select(x => x.Thickness).DefaultIfEmpty(0).Sum(),
                armors.Select(x => x.Protection).DefaultIfEmpty(0).Sum(), 
                armors.Select(x => x.Coverage).DefaultIfEmpty(0).Sum());
        }
    }
}