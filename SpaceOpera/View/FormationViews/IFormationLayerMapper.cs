using OpenTK.Mathematics;
using SpaceOpera.Core;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.View.FormationViews
{
    public interface IFormationLayerMapper<T> where T : notnull
    {
        (object, T) MapToBucket(INavigable node);
        Vector3 MapToPin(T bucket);

        public class GalaxyMapper : IFormationLayerMapper<StarSystem>
        {
            private readonly World? _world;
            private readonly Galaxy _galaxy;
            private readonly float _scale;

            public GalaxyMapper(World? world, Galaxy galaxy, float scale)
            {
                _world = world;
                _galaxy = galaxy;
                _scale = scale;
            }

            public (object, StarSystem) MapToBucket(INavigable node)
            {
                return (_galaxy, _world!.NavigationMap.GetStarSystem(node));
            }

            public Vector3 MapToPin(StarSystem bucket)
            {
                return _scale * bucket.Position;
            }
        }

        public class SubSystemRigMapper : IFormationLayerMapper<object>
        {
            private static readonly float s_LocalOrbitY = -0.1f;
            private static readonly float s_SolarOrbitY = -0.25f;

            private readonly World? _world;
            private readonly StarSystem _starSystem;
            private readonly float _scale;
            private readonly Dictionary<INavigable, Vector3> _transitPins;

            public SubSystemRigMapper(
                World? world, StarSystem starSystem, float scale, Dictionary<INavigable, Vector3> transitPins)
            {
                _world = world;
                _starSystem = starSystem;
                _scale = scale;
                _transitPins = transitPins;
            }

            public (object, object) MapToBucket(INavigable node)
            {
                if (node is TransitRegion)
                {
                    return (_starSystem, node);
                }
                var layer = _world!.NavigationMap.GetOrbit(node)!;
                if (node is LocalOrbitRegion || node is SolarOrbitRegion)
                {
                    return (layer, node);
                }
                if (node is StationaryOrbitRegion orbit)
                {
                    return (layer, _world!.NavigationMap.GetOrbit(orbit)!.LocalOrbit.StellarBody);
                }
                return (layer, _world!.NavigationMap.GetStellarBody(node)!);
            }

            public Vector3 MapToPin(object bucket)
            {
                if (bucket is TransitRegion region)
                {
                    return _transitPins[region];
                }
                if (bucket is SolarOrbitRegion)
                {
                    return _scale * new Vector3(0, s_SolarOrbitY, 0);
                }
                if (bucket is LocalOrbitRegion)
                {
                    return _scale * new Vector3(0, s_LocalOrbitY, 0);
                }
                if (bucket is StellarBody || bucket is TransitRegion)
                {
                    return new();
                }
                throw new ArgumentException($"Unsupported bucket type: [{bucket.GetType()}]");
            }
        }
    }
}
