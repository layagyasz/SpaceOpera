using Cardamom.Trackers;
using SpaceOpera.Core.Economics;

namespace SpaceOpera.Core.Universe
{
    public class Atmosphere
    {
        public MultiQuantity<IMaterial> Composition { get; }
        public float Radius { get; }
        public float Density { get; }
        public float Falloff { get; }

        public Atmosphere(MultiQuantity<IMaterial> composition, float radius, float density, float falloff)
        {
            Composition = composition;
            Radius = radius;
            Density = density;
            Falloff = falloff;
        }
    }
}
