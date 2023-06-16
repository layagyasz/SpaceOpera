using OpenTK.Mathematics;
using SpaceOpera.Core;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.View.Game.FormationViews
{
    public interface IFormationLayerMapper<T> where T : notnull
    {
        (object?, T) MapToBucket(INavigable node);
        Vector3 MapToPin(T bucket);
        float? GetOffset(T bucket);

        public class GalaxyMapper : IFormationLayerMapper<StarSystem>
        {
            private static readonly float s_GalaxyOffset = 32f;


            private readonly World? _world;
            private readonly Galaxy _galaxy;
            private readonly float _scale;

            public GalaxyMapper(World? world, Galaxy galaxy, float scale)
            {
                _world = world;
                _galaxy = galaxy;
                _scale = scale;
            }

            public (object?, StarSystem) MapToBucket(INavigable node)
            {
                return (_galaxy, _world!.NavigationMap.GetStarSystem(node));
            }

            public Vector3 MapToPin(StarSystem bucket)
            {
                return _scale * bucket.Position;
            }

            public float? GetOffset(StarSystem bucket)
            {
                return s_GalaxyOffset;
            }
        }

        public class StarSystemMapper : IFormationLayerMapper<object>
        {
            private static readonly float s_StarSystemOffset = 32f;

            private static readonly float s_LocalOrbitY = -0.1f;
            private static readonly float s_SolarOrbitY = -0.25f;

            private readonly World? _world;
            private readonly StarSystem _starSystem;
            private readonly float _scale;
            private readonly Dictionary<INavigable, Vector3> _transitPins;

            public StarSystemMapper(
                World? world, StarSystem starSystem, float scale, Dictionary<INavigable, Vector3> transitPins)
            {
                _world = world;
                _starSystem = starSystem;
                _scale = scale;
                _transitPins = transitPins;
            }

            public (object?, object) MapToBucket(INavigable node)
            {
                if (node is TransitRegion transit)
                {
                    if (_starSystem.Transits.ContainsValue(transit))
                    {
                        return (_starSystem, node);
                    }
                    return (null, node);
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
                    return _scale * _transitPins[region];
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

            public float? GetOffset(object bucket)
            {
                return s_StarSystemOffset;
            }
        }

        public class StellarBodyMapper : IFormationLayerMapper<INavigable>
        {
            private readonly World? _world;
            private readonly StellarBody _stellarBody;
            private readonly float _surfaceRadius;
            private readonly float _atmosphereRadius;

            public StellarBodyMapper(
                World? world, StellarBody stellarBody, float surfaceRadius, float atmosphereRadius)
            {
                _world = world;
                _stellarBody = stellarBody;
                _surfaceRadius = surfaceRadius;
                _atmosphereRadius = atmosphereRadius;
            }

            public (object?, INavigable) MapToBucket(INavigable node)
            {
                if (node is StationaryOrbitRegion orbit)
                {
                    if (_stellarBody.OrbitRegions.Contains(orbit))
                    {
                        return (_stellarBody, node);
                    }
                    return (null, node);
                }
                return (_world!.NavigationMap.GetStellarBody(node), node);
            }

            public Vector3 MapToPin(INavigable bucket)
            {
                if (bucket is StationaryOrbitRegion orbit)
                {
                    return _atmosphereRadius 
                        * new Vector3(new Vector3(MathF.Cos(orbit.Theta), 0, MathF.Sin(orbit.Theta)));
                }
                if (bucket is StellarBodySubRegion region)
                {
                    return _surfaceRadius * region.Center;
                }
                throw new ArgumentException($"Unsupported bucket type: [{bucket.GetType()}]");
            }

            public float? GetOffset(INavigable bucket)
            {
                return null;
            }
        }
    }
}
