using OpenTK.Mathematics;
using SpaceOpera.Core;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.View.FormationViews
{
    public interface IFormationLayerMapper<T> where T : notnull
    {
        bool Contains(T bucket);
        T MapToBucket(INavigable node);
        Vector3 MapToPin(T bucket);

        public class GalaxyMapper : IFormationLayerMapper<StarSystem>
        {
            private readonly World _world;
            private readonly float _scale;

            public GalaxyMapper(World world, float scale)
            {
                _world = world;
                _scale = scale;
            }

            public bool Contains(StarSystem bucket)
            {
                return true;
            }

            public StarSystem MapToBucket(INavigable node)
            {
                return _world.NavigationMap.GetStarSystem(node);
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

            private readonly World _world;
            private readonly SolarOrbitRegion _region;
            private readonly float _scale;

            public SubSystemRigMapper(World world, SolarOrbitRegion region, float scale)
            {
                _world = world;
                _region = region;
                _scale = scale;
            }

            public bool Contains(object bucket)
            {
                if (bucket is SolarOrbitRegion)
                {
                    return bucket == _region;
                }
                if (bucket is LocalOrbitRegion)
                {
                    return bucket == _region.LocalOrbit;
                }
                if (bucket is StellarBody)
                {
                    return bucket == _region.LocalOrbit.StellarBody;
                }
                return false;
            }

            public object MapToBucket(INavigable node)
            {
                if (node is LocalOrbitRegion || node is SolarOrbitRegion)
                {
                    return node;
                }
                if (node is StationaryOrbitRegion orbit)
                {
                    return _world.NavigationMap.GetOrbit(orbit)!.LocalOrbit.StellarBody;
                }
                return _world.NavigationMap.GetStellarBody(node)!;
            }

            public Vector3 MapToPin(object bucket)
            {
                if (bucket is SolarOrbitRegion)
                {
                    return _scale * new Vector3(0, s_SolarOrbitY, 0);
                }
                if (bucket is LocalOrbitRegion)
                {
                    return _scale * new Vector3(0, s_LocalOrbitY, 0);
                }
                if (bucket is StellarBody)
                {
                    return new();
                }
                throw new ArgumentException($"Unsupported bucket type: [{bucket.GetType()}]");
            }
        }
    }
}
