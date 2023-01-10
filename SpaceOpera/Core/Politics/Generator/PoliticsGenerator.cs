using Cardamom.Graphing;
using Cardamom.Utilities;
using SpaceOpera.Core.Universe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Politics.Generator
{
    class PoliticsGenerator
    {
        class FactionWrapper : DijkstraRegion<RegionWrapper>
        {
            public Faction Faction { get; }
            public RegionWrapper Center { get; }
            public double StartDistance => 0;

            public HashSet<RegionWrapper> Regions { get; } = new HashSet<RegionWrapper>();

            public FactionWrapper(Faction Faction, RegionWrapper Center)
            {
                this.Faction = Faction;
                this.Center = Center;
            }

            public void Add(RegionWrapper Child)
            {
                Regions.Add(Child);
            }
        }

        class RegionWrapper : Pathable<RegionWrapper>
        {
            public bool Passable => true;
            public StellarBody StellarBody { get; }
            public StellarBodyRegion Region { get; }
            public List<RegionEdgeWrapper> Edges { get; } = new List<RegionEdgeWrapper>();
            public MultiQuantity<Culture> CultureWeights { get; } = new MultiQuantity<Culture>();

            public RegionWrapper(StellarBody StellarBody, StellarBodyRegion Region)
            {
                this.StellarBody = StellarBody;
                this.Region = Region;
            }

            public IEnumerable<RegionWrapper> Neighbors()
            {
                return Edges.Select(x => x.End);
            }

            public double DistanceTo(RegionWrapper Neighbor)
            {
                return Edges.First(x => x.End == Neighbor).Distance;
            }

            public double HeuristicDistanceTo(RegionWrapper Other)
            {
                return DistanceTo(Other);
            }
        }

        class RegionEdgeWrapper
        {
            public RegionWrapper End { get; }
            public float Distance { get; }
            public float Falloff { get; }

            public RegionEdgeWrapper(RegionWrapper End, float Distance, float Falloff)
            {
                this.End = End;
                this.Distance = Distance;
                this.Falloff = Falloff;
            }
        }

        public CultureGenerator Culture { get; set; }
        public BannerGenerator Banner { get; set; }
        public FactionGenerator Faction { get; set; }
        public DesignGenerator Design { get; set; }
        public FleetGenerator Fleet { get; set; }
        public float BaseLinkChance { get; set; }
        public uint Cultures { get; set; }
        public uint States { get; set; }

        public void Generate(World World, Culture PlayerCulture, Faction PlayerFaction, Random Random)
        {
            var nodes = new Dictionary<StellarBodyRegion, RegionWrapper>();
            var homeRegions = new WeightedVector<RegionWrapper>();
            foreach (var system in World.Galaxy.Systems)
            {
                var regionOptions = new WeightedVector<RegionWrapper>();
                foreach (var body in system.Orbiters.Concat(
                    system.Transits.Values.SelectMany(x => x.TransitSystem.Orbiters)))
                {
                    foreach (var region in body.Regions)
                    {
                        if (region.DominantBiome.IsTraversable)
                        {
                            var node = GetOrCreateNode(nodes, body, region);
                            regionOptions.Add(GetLinkChance(region), node);
                        }
                    }
                }

                for (int i=0; i < system.Orbiters.Count; ++i)
                {
                    var body = system.Orbiters[i];
                    foreach (var region in body.Regions)
                    {
                        var node = GetOrCreateNode(nodes, body, region);
                        if (region.DominantBiome.IsTraversable)
                        {
                            homeRegions.Add(GetHabitability(region), node);
                        }
                        foreach (var neighbor in region.GetNeighbors())
                        {
                            var neighborNode = GetOrCreateNode(nodes, body, neighbor);
                            float distance = GetDistance(region, neighbor);
                            float falloff = GetFalloff(region, neighbor);
                            node.Edges.Add(new RegionEdgeWrapper(neighborNode, distance, falloff));
                            neighborNode.Edges.Add(new RegionEdgeWrapper(node, distance, falloff));
                        }
                        if (region.DominantBiome.IsTraversable && Random.NextDouble() < GetLinkChance(region))
                        {
                            var jump = regionOptions[Random.NextDouble()];
                            float distance = GetDistance(region, jump.Region);
                            float falloff = GetFalloff(region, jump.Region);
                            node.Edges.Add(new RegionEdgeWrapper(jump, distance, falloff));
                            jump.Edges.Add(new RegionEdgeWrapper(node, distance, falloff));
                        }
                    }

                    if (i < system.Orbiters.Count - 1)
                    {
                        AddRandomEdge(body, system.Orbiters[i + 1], nodes, Random);
                    }
                    else
                    {
                        foreach (var neighbor in system.Transits.Values.Select(x => x.TransitSystem))
                        {
                            if (neighbor.Orbiters.Count > 0)
                            {
                                AddRandomEdge(body, neighbor.Orbiters.Last(), nodes, Random);
                            }
                        }
                    }
                }
            }

            var cultures = new List<Culture>();
            var playerHomeRegion = homeRegions[Random.Next(0, homeRegions.Count)];
            PlaceCulture(PlayerCulture, playerHomeRegion);
            var chosenHomeRegions = new List<RegionWrapper>();
            for (int i = 0; i < Cultures; ++i)
            {
                var culture = Culture.Generate(Random);
                cultures.Add(culture);
                var homeRegion = homeRegions[Random.NextDouble()];
                chosenHomeRegions.Add(homeRegion);
                PlaceCulture(culture, homeRegion);
            }
            World.AddAllCultures(cultures);

            var states = new List<FactionWrapper>();
            var playerStateWrapper = new FactionWrapper(PlayerFaction, playerHomeRegion);
            states.Add(playerStateWrapper);
            var closedRegions = new HashSet<RegionWrapper>();
            closedRegions.Add(playerHomeRegion);
            var statePool = new DijkstraPool<RegionWrapper>();
            statePool.Drop(playerStateWrapper);

            var banners = Banner.GenerateUnique(States, Random).ToList();
            for (int i = 0; i < States; ++i)
            {
                var homeRegion = i < chosenHomeRegions.Count ? chosenHomeRegions[i] : homeRegions[Random.NextDouble()];
                while (closedRegions.Contains(homeRegion) || homeRegion.CultureWeights.Count == 0)
                {
                    homeRegion = homeRegions[Random.NextDouble()];
                }
                closedRegions.Add(homeRegion);
                var stateWrapper =
                    new FactionWrapper(
                        Faction.Generate(
                            homeRegion.CultureWeights.ArgMax(x => x.Value).Key, banners[i], Random), homeRegion);
                states.Add(stateWrapper);
                statePool.Drop(stateWrapper);
            }
            World.AddAllFactions(states.Select(x => x.Faction));
            statePool.Resolve();

            foreach (var state in states)
            {
                Design.Generate(World, state.Faction, Random);
                foreach (var region in state.Regions)
                {
                    region.Region.SetName(state.Faction.NameGenerator.GenerateNameFor(region.Region, Random));
                    region.Region.SetSovereign(state.Faction);
                }
                var hq = 
                    World.Galaxy.Systems
                        .SelectMany(x => x.Orbiters)
                        .SelectMany(x => x.OrbitRegions)
                        .Where(x => x.SubRegions.Contains(state.Regions.First().Region.Center))
                        .FirstOrDefault();
                Fleet.Generate(World, state.Faction, hq, Random);
            }

            foreach (var system in World.Galaxy.Systems)
            {
                var dominantFaction = 
                    system.Orbiters.SelectMany(x => x.Regions).GroupBy(x => x.Sovereign).ArgMax(x => x.Count())?.Key;
                if (dominantFaction != null)
                {
                    system.Star.SetName(dominantFaction.NameGenerator.GenerateNameForStar(Random));
                    system.SetName(dominantFaction.NameGenerator.GenerateNameFor(system, Random));

                    foreach (var stellarBody in system.Orbiters)
                    {
                        var stellarBodyDominantFaction = 
                            stellarBody.Regions.GroupBy(x => x.Sovereign).ArgMax(x => x.Count()).Key 
                            ?? dominantFaction;
                        stellarBody.SetName(
                            stellarBodyDominantFaction.NameGenerator.GenerateNameFor(stellarBody, system, Random));
                    }
                }
            }
        }

        private void AddRandomEdge(
            StellarBody Left, StellarBody Right, Dictionary<StellarBodyRegion, RegionWrapper> Wrappers, Random Random)
        {
            var region = Left.Regions[Random.Next(0, Left.Regions.Count)];
            var jump = Right.Regions[Random.Next(0, Right.Regions.Count)];

            var node = GetOrCreateNode(Wrappers, Left, region);
            var jumpNode = GetOrCreateNode(Wrappers, Right, jump);

            float distance = GetDistance(region, jump);
            float falloff = GetFalloff(region, jump);
            node.Edges.Add(new RegionEdgeWrapper(jumpNode, distance, falloff));
            jumpNode.Edges.Add(new RegionEdgeWrapper(node, distance, falloff));
        }

        private void PlaceCulture(Culture Culture, RegionWrapper HomeRegion)
        {
            var queue = new PriorityQueue<RegionWrapper, float>();
            var closed = new HashSet<RegionWrapper>();
            queue.Push(HomeRegion, 0);
            HomeRegion.CultureWeights.Add(Culture, 1);
            while (queue.Count > 0)
            {
                var current = queue.Pop();
                closed.Add(current);
                foreach (var edge in current.Edges)
                {
                    if (!closed.Contains(edge.End))
                    {
                        var weight = current.CultureWeights.Get(Culture) * edge.Falloff;
                        if (edge.End.CultureWeights.Get(Culture) < weight)
                        {
                            edge.End.CultureWeights[Culture] = weight;
                            queue.Remove(edge.End);
                            queue.Push(edge.End, weight);
                        }
                    }
                }
            }
        }

        private float GetHabitability(StellarBodyRegion Region)
        {
            if (Region.DominantBiome.IsHabitable)
            {
                return 1;
            }
            else return 0.01f;
        }

        private float GetFalloff(StellarBodyRegion Left, StellarBodyRegion Right)
        {
            return 0.9f;
        }

        private float GetDistance(StellarBodyRegion Left, StellarBodyRegion Right)
        {
            return 1;
        }

        private float GetLinkChance(StellarBodyRegion Region)
        {
            float chance = BaseLinkChance;
            if (!Region.DominantBiome.IsHabitable)
            {
                chance *= 0.1f;
            }
            return chance;
        }

        private RegionWrapper GetOrCreateNode(
            Dictionary<StellarBodyRegion, RegionWrapper> Wrappers, StellarBody StellarBody, StellarBodyRegion Region)
        {
            RegionWrapper node;
            if (Wrappers.TryGetValue(Region, out node))
            {
                return node;
            }
            else
            {
                node = new RegionWrapper(StellarBody, Region);
                Wrappers.Add(Region, node);
                return node;
            }
        }
    }
}