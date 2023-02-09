using Cardamom.Graphing;
using Cardamom.Mathematics;
using Cardamom.Utils.Generators.Samplers;
using DelaunayTriangulator;
using OpenTK.Mathematics;

namespace SpaceOpera.Core.Universe.Generator
{
    public class GalaxyGenerator
    {
        private static readonly ISampler s_YSampler = new NormalSampler(0, 100);

        public float Radius { get; set; }
        public uint Arms { get; set; }
        public float Rotation { get; set; }
        public float CoreRadius { get; set; }
        public int StarCount { get; set; }
        public float TransitDensity { get; set; }
        public StarSystemGenerator? StarSystemGenerator { get; set; }

        class SystemWrapper : IGraphNode
        {
            public StarSystem System { get; }
            private readonly List<IGraphEdge> _edges = new();

            public SystemWrapper(StarSystem system)
            {
                System = system;
            }

            public IEnumerable<IGraphEdge> GetEdges()
            {
                return _edges;
            }

            public void SetNeighbors(IEnumerable<SystemWrapper> wrappers)
            {
                _edges.AddRange(
                    wrappers.Select(
                        x => new DefaultGraphEdge(this, x, Vector3.Distance(System.Position, x.System.Position))));
                System.SetNeighbors(wrappers.Select(x => x.System));
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

            Triangulator triangulator = new();
            List<Triad> triads = triangulator.Triangulation(vertices);
            VoronoiGrapher.VoronoiNeighborsResult result = VoronoiGrapher.GetNeighbors(vertices, triads);

            List<SystemWrapper> systemWrappers = new();
            for (int i=0; i< StarCount; ++i)
            {
                systemWrappers.Add(
                    new SystemWrapper(
                        StarSystemGenerator!.Generate(
                            random, new Vector3(vertices[i].x, s_YSampler.Generate(random), vertices[i].y))));
            }
            for (int i=0; i < StarCount;++i)
            {
                var neighbors = result.Neighbors[i].Where(x => x >= 0).Select(x => systemWrappers[x]).ToList();
                systemWrappers[i].SetNeighbors(neighbors);
            }
            foreach (var transit in MinimalSpanningTree.Compute(systemWrappers))
            {
                var start = (SystemWrapper)transit.Start;
                var end = (SystemWrapper)transit.End;
                start.System.AddTransit(end.System);
                end.System.AddTransit(start.System);
            }
            var distances = systemWrappers.SelectMany(x => x.GetEdges().Select(y => y.Cost)).ToList();
            var mean = distances.Average();
            var stdDev = MathUtils.StandardDeviation(distances, mean);
            var closed = new HashSet<SystemWrapper>();
            foreach (var systemWrapper in systemWrappers)
            {
                closed.Add(systemWrapper);
                foreach (var edge in systemWrapper.GetEdges())
                {
                    var neighbor = (SystemWrapper)edge.End;
                    if (closed.Contains(neighbor) 
                        || systemWrapper.System.Transits.Values.Any(x => x.TransitSystem == neighbor.System))
                    {
                        continue;
                    }
                    if (random.NextDouble() 
                        < TransitDensityFn(TransitDensity, (edge.Cost - mean) / stdDev)) {
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