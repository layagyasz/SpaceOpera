using Cardamom.Graphing;
using Cardamom.Planar;
using Cardamom.Spatial;
using Cardamom.Utilities;
using DelaunayTriangulator;
using SFML.System;
using SFML.Window;
using SpaceOpera.Core.Voronoi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Universe.Generator
{
    class StellarBodyGenerator: IKeyed
    {
        class RegionWrapper : DijkstraRegion<SubRegionWrapper>
        {
            public SubRegionWrapper Center { get; }
            public double StartDistance { get; } = 0;
            public List<SubRegionWrapper> Children { get; } = new List<SubRegionWrapper>();

            public RegionWrapper(SubRegionWrapper Center)
            {
                this.Center = Center;
            }

            public void Add(SubRegionWrapper Child)
            {
                Children.Add(Child);
            }

            public override string ToString()
            {
                return Center.ToString();
            }
        }

        class SubRegionWrapper : Pathable<SubRegionWrapper>
        {
            public StellarBodySubRegion Region { get; }
            public List<SubRegionWrapper> NeighborWrappers { get; } = new List<SubRegionWrapper>();
            public bool Passable { get; } = true;

            public SubRegionWrapper(StellarBodySubRegion Region)
            {
                this.Region = Region;
            }

            public IEnumerable<SubRegionWrapper> Neighbors()
            {
                return NeighborWrappers;
            }

            public double DistanceTo(SubRegionWrapper OtherRegion)
            {
                int biomeCost = this.Region.Biome == OtherRegion.Region.Biome ? 0 : 4;
                return 1 + biomeCost;
            }

            public double HeuristicDistanceTo(SubRegionWrapper OtherRegion)
            {
                throw new NotImplementedException();
            }

            public override string ToString()
            {
                return Region.ToString();
            }
        }

        public string Key { get; set; }
        public string Name { get; set; }
        public Sampler RadiusSampler { get; set; }
        public float Density { get; set; }
        public float RegionDensity { get; set; }
        public float SubRegionDensity { get; set; }
        public uint StructureNodeDensity { get; set; }
        public BiomeSelector BiomeSelector { get; set; }
        public AtmosphereGenerator AtmosphereGenerator { get; set; }

        public StellarBody Generate(Random Random, Orbit Orbit)
        {
            while(true)
            {
                try
                {
                    return GenerateAux(Random, Orbit);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    continue;
                }
            }
        }

        private StellarBody GenerateAux(Random Random, Orbit Orbit)
        { 
            double radius = RadiusSampler.Sample(Random);
            int subRegionCount = (int)Math.Ceiling(4 * SubRegionDensity * Math.PI * radius * radius);

            List<Vector4f> centers = new List<Vector4f>();
            for (int i = 0; i < subRegionCount - 1; ++i)
            {
                float z = (float)(2 * Random.NextDouble() - 1);
                float r = (float)(Math.Sqrt(1 - z * z));
                float theta = (float)(2 * Math.PI * Random.NextDouble());
                centers.Add(new Vector4f(r * (float)Math.Cos(theta), r * (float)Math.Sin(theta), z));
            }
            centers.Sort((x, y) => -x.Z.CompareTo(y.Z));

            IProjection<Vector2f, Vector4f> projection = new Projection.StereographicProjection();
            List<Vertex> vertices = new List<Vertex>();
            foreach (var center in centers)
            {
                Vector2f projected = projection.Project(center);
                vertices.Add(new Vertex(projected.X, projected.Y));
            }

            List<Triad> triads = VoronoiGrapher.GetTriangulation(vertices);
            VoronoiGrapher.NeighborsResult result = VoronoiGrapher.GetNeighbors(vertices, triads);
            result.Neighbors.Add(result.EdgeIndices);
            centers.Add(new Vector4f(0, 0, 1));

            BiomeSelector.Seed(Random);
            List<SubRegionWrapper> subRegionWrappers = new List<SubRegionWrapper>();
            for (int i=0;i<subRegionCount;++i)
            {
                Vector4f center = centers[i] * (float)radius;
                CSpherical centerSpherical = CSpherical.FromCartesian(center);
                Biome biome = BiomeSelector.Select(center, centerSpherical);
                var subRegion = new StellarBodySubRegion(i, center, centerSpherical, biome);
                subRegionWrappers.Add(new SubRegionWrapper(subRegion));
            }
            
            for(int i=0; i<subRegionCount;++i)
            {
                subRegionWrappers[i].Region.SetNeighbors(
                    result.Neighbors[i]
                    .Select(x => subRegionWrappers[x >= 0 ? x : subRegionCount - 1].Region));
                subRegionWrappers[i].NeighborWrappers.AddRange(
                    result.Neighbors[i].Select(x => subRegionWrappers[x >= 0 ? x : subRegionCount - 1]));
            }

            int regionCount = (int)Math.Ceiling(4 * RegionDensity * Math.PI * radius * radius);
            DijkstraPool<SubRegionWrapper> dijkstraPool = new DijkstraPool<SubRegionWrapper>();
            List<RegionWrapper> regionWrappers = new List<RegionWrapper>();
            List<int> cores = new List<int>();
            for (int i=0; i <regionCount; ++i)
            {
                int core;
                do
                {
                    core = Random.Next(0, subRegionCount);
                }
                while (cores.Contains(core) || subRegionWrappers[core].NeighborWrappers.Count < 4);
                cores.Add(core);
                var region = new RegionWrapper(subRegionWrappers[core]);
                regionWrappers.Add(region);
                dijkstraPool.Drop(region);
            }
            dijkstraPool.Resolve();

            foreach (var region in regionWrappers.ToList())
            {
                var partitioned =
                    region.Children
                        .Where(x => x.Region.Biome.IsTraversable != region.Center.Region.Biome.IsTraversable)
                        .ToList();
                if (partitioned.Count > 0)
                {
                    region.Children.RemoveAll(partitioned.Contains);
                    var newWrapper = 
                        new RegionWrapper(
                            partitioned.ArgMax(
                                x => -MathUtils.ArcLength(region.Center.Region.Center, x.Region.Center, radius)));
                    newWrapper.Children.AddRange(partitioned);
                    regionWrappers.Add(newWrapper);
                }
            }
            if (regionWrappers.Count > regionCount)
            {
                regionWrappers.Sort(
                    Comparer<RegionWrapper>.Create((x, y) => x.Children.Count.CompareTo(y.Children.Count)));
                var regionsToRemove = regionWrappers.Count - regionCount;
                while (regionsToRemove > 0)
                {
                    var region = regionWrappers[0];
                    var regionToCombine =
                        regionWrappers.FirstOrDefault(
                            x => x != region 
                                && region.Center.Region.Biome.IsTraversable == x.Center.Region.Biome.IsTraversable 
                                && region.Children.SelectMany(y => y.NeighborWrappers).Any(region.Children.Contains));
                    if (regionToCombine != null)
                    {
                        regionToCombine.Children.AddRange(region.Children);
                        region.Children.Clear();
                        --regionsToRemove;
                    }
                    else
                    {
                        regionWrappers.Add(region);
                    }
                    regionWrappers.RemoveAt(0);
                }
            }

            List<StellarBodyRegion> regions = new List<StellarBodyRegion>();
            for (int i = 0; i < regionCount;++i)
            {
                var region = 
                    new StellarBodyRegion(
                        regionWrappers[i].Center.Region, regionWrappers[i].Children.Select(x => x.Region));
                region.AddStructureNodes((uint)(StructureNodeDensity * regionWrappers[i].Children.Count));
                regions.Add(region);
                foreach (var subRegion in regionWrappers[i].Children)
                {
                    subRegion.Region.SetParentRegion(region);
                }
            }

            int atmosphericRegionCount = (int)(2 * Math.PI * radius * AtmosphereGenerator.RegionDensity) + 1;
            List<List<StellarBodySubRegion>> atmosphereRegionMembers = new List<List<StellarBodySubRegion>>();
            for (int i=0; i<atmosphericRegionCount; ++i)
            {
                atmosphereRegionMembers.Add(new List<StellarBodySubRegion>());
            }
            foreach (var subRegion in subRegionWrappers)
            {
                double angle = ((subRegion.Region.SphericalCenter.Phi + 2 * Math.PI) % (2 * Math.PI)) / (2 * Math.PI);
                int atmosphereRegion = (int)(angle * atmosphericRegionCount);
                atmosphereRegionMembers[atmosphereRegion].Add(subRegion.Region);
            }

            var orbitRegions = new List<StationaryOrbitRegion>();
            for (int i=0; i< atmosphericRegionCount; ++i)
            {
                var region = new StationaryOrbitRegion(string.Format("Atmosphere {0}", i), atmosphereRegionMembers[i]);
                orbitRegions.Add(region);
            }

            return new StellarBody(
                Name,
                radius,
                4 * Density * Math.PI * radius * radius * radius / 3,
                Orbit,
                AtmosphereGenerator.Generate(Random), 
                regions, 
                orbitRegions);
        }
    }
}