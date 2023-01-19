using Cardamom.Graphing;
using DelaunayTriangulator;
using OpenTK.Mathematics;

namespace SpaceOpera.Core.Universe.Generator
{
    public class GalaxyGenerator
    {
        public float Radius { get; set; }
        public uint Arms { get; set; }
        public float Rotation { get; set; }
        public float CoreRadius { get; set; }
        public int StarCount { get; set; }
        public float TransitDensity { get; set; }
        public StarSystemGenerator? StarSystemGenerator { get; set; }

        class SystemWrapper : Pathable<SystemWrapper>
        {
            public StarSystem System { get; }
            public bool Passable { get; } = true;
            private HashSet<SystemWrapper> _neighborWrappers { get; } = new();

            public SystemWrapper(StarSystem system)
            {
                this.System = system;
            }

            public float DistanceTo(SystemWrapper node)
            {
                return HeuristicDistanceTo(node);
            }

            public float HeuristicDistanceTo(SystemWrapper node)
            {
                return Vector2.Distance(System.Position, node.System.Position);
            }

            public void SetNeighbors(IEnumerable<SystemWrapper> wrappers)
            {
                foreach (var wrapper in wrappers)
                {
                    _neighborWrappers.Add(wrapper);
                }
                System.SetNeighbors(wrappers.Select(x => x.System));
            }

            public IEnumerable<SystemWrapper> Neighbors()
            {
                return _neighborWrappers;
            }
        }

        public Galaxy Generate(Random random)
        {
            List<Vertex> vertices = new();
            int starId = 0;
            while (starId<StarCount)
            {
                float x = random.NextSingle() * 2 - 1;
                float y = random.NextSingle() * 2 - 1;
                float angle = MathF.Atan2(y, x);
                float radius = (float)Math.Sqrt(x * x + y * y);
                if (random.NextDouble() < StarDensityFn(Arms, Rotation, angle, radius))
                {
                    ++starId;
                    float modifiedRadius = radius * (Radius - CoreRadius) + CoreRadius;
                    vertices.Add(
                        new Vertex(modifiedRadius * MathF.Cos(angle), modifiedRadius * MathF.Sin(angle)));
                }
            }

            Triangulator triangulator = new Triangulator();
            List<Triad> triads = triangulator.Triangulation(vertices);
            VoronoiGrapher.VoronoiNeighborsResult result = VoronoiGrapher.GetNeighbors(vertices, triads);

            List<SystemWrapper> systemWrappers = new();
            for (int i=0; i< StarCount; ++i)
            {
                systemWrappers.Add(
                    new SystemWrapper(
                        StarSystemGenerator!.Generate(random, new Vector2(vertices[i].x, vertices[i].y))));
            }
            for (int i=0; i < StarCount;++i)
            {
                var neighbors = result.Neighbors[i].Where(x => x >= 0).Select(x => systemWrappers[x]).ToList();
                systemWrappers[i].SetNeighbors(neighbors);
            }
            var spanningTree = 
                new MinimalSpanning<SystemWrapper>(systemWrappers, x => x.Neighbors(), (x, y) => x.DistanceTo(y));
            foreach (var transit in spanningTree.GetEdges())
            {
                transit.Item1.System.AddTransit(transit.Item2.System);
                transit.Item2.System.AddTransit(transit.Item1.System);
            }
            var distances = systemWrappers.SelectMany(x => x.Neighbors().Select(y => (float)x.DistanceTo(y))).ToList();
            var mean = distances.Average();
            var stdDev = MathUtils.StandardDeviation(distances, mean);
            var closed = new HashSet<SystemWrapper>();
            foreach (var systemWrapper in systemWrappers)
            {
                closed.Add(systemWrapper);
                foreach (var neighbor in systemWrapper.Neighbors())
                {
                    if (closed.Contains(neighbor) 
                        || systemWrapper.System.Transits.Values.Any(x => x.TransitSystem == neighbor.System))
                    {
                        continue;
                    }
                    if (random.NextDouble() 
                        < TransitDensityFn(TransitDensity, (systemWrapper.DistanceTo(neighbor) - mean) / stdDev)) {
                        systemWrapper.System.AddTransit(neighbor.System);
                        neighbor.System.AddTransit(systemWrapper.System);
                    }
                }
            }

            return new Galaxy(Radius, systemWrappers.Select(x => x.System));
        }

        private static float StarDensityFn(uint arms, float rotation, float angle, float radius)
        {
            if (radius > 1)
            {
                return 0;
            }
            return .5f * Math.Max(
                (MathF.Cos(angle * arms + radius * rotation) + 1) * MathF.Sqrt(1 - radius), 1 - radius);
        }

        private static float TransitDensityFn(float density, float standardDeviations)
        {
            return 2 * density / (1 + (float)Math.Exp(20 * standardDeviations));
        }
    }
}