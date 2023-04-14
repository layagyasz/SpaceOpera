using Cardamom.Collections;
using OpenTK.Mathematics;

namespace SpaceOpera.Core.Universe
{
    public class NavigationMap
    {
        public class Movement
        {
            public INavigable Origin { get; }
            public INavigable Destination { get; }
            public NavigableEdgeType Type { get; }
            public float Distance { get; }

            public Movement(INavigable origin, INavigable destination, NavigableEdgeType type, float distance)
            {
                Origin = origin;
                Destination = destination;
                Type = type;
                Distance = distance;
            }

            public override string ToString()
            {
                return string.Format(
                    "[Movement: Origin={0}, Destination={1}, Type={2}, Distance={3}",
                    Origin.Name, 
                    Destination.Name, 
                    Type, 
                    Distance);
            }
        }

        class NavigableNode
        {
            public INavigable Navigable { get; }
            public StarSystem StarSystem { get; }
            public SolarOrbitRegion? Orbit { get; }
            public float OrbitDistance { get; }
            public StellarBody? StellarBody { get; }
            public List<NavigableEdge> Edges { get; } = new();

            // Used by A*
            public NavigableEdge? Parent { get; set; }
            public float Distance { get; set; } = float.PositiveInfinity;

            public NavigableNode(
                INavigable navigable,
                StarSystem starSystem, 
                SolarOrbitRegion? orbit,
                float orbitDistance, 
                StellarBody? stellarBody)
            {
                Navigable = navigable;
                StarSystem = starSystem;
                Orbit = orbit;
                OrbitDistance = orbitDistance;
                StellarBody = stellarBody;
            }
        }

        class NavigableEdge
        {
            public NavigableNode Start { get; }
            public NavigableNode End { get; }
            public NavigableEdgeType Type { get; }
            public float Distance { get; }

            public NavigableEdge(NavigableNode start, NavigableNode end, NavigableEdgeType type, float distance)
            {
                Start = start;
                End = end;
                Type = type;
                Distance = distance;
            }
        }

        private readonly Dictionary<INavigable, NavigableNode> _nodes = new();

        private NavigationMap(Galaxy galaxy)
        {
            foreach (var system in galaxy.Systems)
            {
                Populate(system);
            }
        }

        public static NavigationMap Create(Galaxy galaxy)
        {
            return new(galaxy);
        }

        public Stack<Movement> FindPath(INavigable start, INavigable end, EnumSet<NavigableEdgeType> allowedEdges)
        {
            Heap<NavigableNode, double> open = new();
            HashSet<NavigableNode> openSet = new();
            HashSet<NavigableNode> closedSet = new();

            var startNode = _nodes[start];
            var endNode = _nodes[end];
            startNode.Distance = 0;
            open.Push(startNode, 0);
            openSet.Add(startNode);
            closedSet.Add(startNode);

            NavigableNode current;
            while (open.Count > 0)
            {
                current = open.Pop();
                if (current == endNode)
                {
                    break;
                }
                openSet.Remove(current);
                foreach (var edge in current.Edges)
                {
                    if (allowedEdges.Contains(edge.Type))
                    {
                        var distance = current.Distance + edge.Distance;
                        if (distance < edge.End.Distance)
                        {
                            closedSet.Add(edge.End);
                            if (openSet.Contains(edge.End))
                            {
                                open.Remove(edge.End);
                            }
                            else
                            {
                                openSet.Add(edge.End);
                            }
                            edge.End.Distance = distance;
                            edge.End.Parent = edge;
                            open.Push(edge.End, distance + HeuristicDistance(edge.End, endNode));
                        }
                    }
                }
            }

            var path = new Stack<Movement>();
            current = endNode;
            while (current != startNode)
            {
                path.Push(
                    new Movement(
                        current.Parent!.Start.Navigable,
                        current.Navigable,
                        current.Parent.Type, 
                        current.Parent.Distance));
                current = current.Parent.Start;
            }

            foreach (var node in closedSet)
            {
                node.Distance = float.PositiveInfinity;
                node.Parent = null;
            }

            return path;
        }

        public int GetSize()
        {
            return _nodes.Count;
        }

        public StarSystem GetStarSystem(INavigable navigable)
        {
            return _nodes[navigable].StarSystem;
        }

        public StellarBody? GetStellarBody(INavigable navigable)
        {
            return _nodes[navigable].StellarBody;
        }

        public SolarOrbitRegion? GetOrbit(INavigable navigable)
        {
            return _nodes[navigable].Orbit;
        }

        public static HashSet<INavigable> GetLocalOrbitNodes(
            LocalOrbitRegion localOrbit, EnumSet<NavigableNodeType> types)
        {
            var nodes = new HashSet<INavigable>
            {
                localOrbit
            };
            if (types.Contains(NavigableNodeType.Space))
            {
                foreach (var node in localOrbit.StellarBody.OrbitRegions)
                {
                    nodes.Add(node);
                }
            }
            if (types.Contains(NavigableNodeType.Ground) || types.Contains(NavigableNodeType.Sea))
            {
                foreach (var node in localOrbit.StellarBody.Regions.SelectMany(x => x.SubRegions))
                {
                    if (types.Contains(node.NavigableNodeType))
                    {
                        nodes.Add(node);
                    }
                }
            }
            return nodes;
        }

        public static HashSet<INavigable> GetSystemNodes(StarSystem starSystem, EnumSet<NavigableNodeType> types)
        {
            var nodes = new HashSet<INavigable>();
            if (types.Contains(NavigableNodeType.Space))
            {
                foreach (var node in starSystem.OrbitalRegions)
                {
                    nodes.Add(node);
                    nodes.Add(node.LocalOrbit);
                    foreach (var stellarBodyNode in node.LocalOrbit.StellarBody.OrbitRegions)
                    {
                        nodes.Add(stellarBodyNode);
                    }
                }
                foreach (var node in starSystem.Transits)
                {
                    nodes.Add(node.Value);
                }
            }
            if (types.Contains(NavigableNodeType.Ground) || types.Contains(NavigableNodeType.Sea))
            {
                foreach (
                    var node in starSystem.OrbitalRegions.SelectMany(
                        x => x.LocalOrbit.StellarBody.Regions).SelectMany(x => x.SubRegions))
                {
                    if (types.Contains(node.NavigableNodeType))
                    {
                        nodes.Add(node);
                    }
                }
            }
            return nodes;
        }

        public static bool IsSystemNode(INavigable node, StarSystem starSystem)
        {
            if (node is SolarOrbitRegion solarOrbit)
            {
                return starSystem.OrbitalRegions.Contains(node);
            }
            if (node is LocalOrbitRegion localOrbit)
            {
                return starSystem.OrbitalRegions.Any(x => x.LocalOrbit == localOrbit);
            }
            if (node is TransitRegion transit)
            {
                return starSystem.Transits.ContainsValue(transit);
            }
            return false;
        }

        private NavigableNode GetOrCreateNode(
            INavigable navigable, 
            StarSystem starSystem,
            SolarOrbitRegion? orbit, 
            float orbitDistance, 
            StellarBody? stellarBody)
        {
            if (_nodes.TryGetValue(navigable, out var node))
            {
                return node;
            }
            else
            {
                node = new NavigableNode(navigable, starSystem, orbit, orbitDistance, stellarBody);
                _nodes.Add(navigable, node);
                return node;
            }
        }

        private static float GetStarSystemDistance(StarSystem left, StarSystem right)
        {
            return Constants.AstralUnitPerLightYear * Vector3.Distance(left.Position, right.Position);
        }

        private static float HeuristicDistance(NavigableNode left, NavigableNode right)
        {
            float distance = 0;
            if (left.StellarBody == right.StellarBody && left.StellarBody != null)
            {
                return MathUtils.ArcLength(
                    ((StellarBodySubRegion)left.Navigable).Center, 
                    ((StellarBodySubRegion)right.Navigable).Center, 
                    left.StellarBody.Radius);
            }
            else
            {
                if (left.StellarBody != null)
                {
                    distance += left.StellarBody.GetGeosynchronousOrbitAltitude();
                }
                if (right.StellarBody != null)
                {
                    distance += right.StellarBody.GetGeosynchronousOrbitAltitude();
                }
            }
            if (left.StarSystem != right.StarSystem)
            {
                distance += GetStarSystemDistance(left.StarSystem, right.StarSystem);
                distance += left.StarSystem.TransitLimit - left.OrbitDistance;
                distance += right.StarSystem.TransitLimit - right.OrbitDistance;
            }
            else
            {
                distance += Math.Abs(left.OrbitDistance - right.OrbitDistance);
            }
            return distance;
        }

        private void Populate(StarSystem starSystem)
        {
            for (int i=0; i<starSystem.OrbitalRegions.Count;++i)
            {
                var geosynchronousOrbitAltitude = (float)starSystem.Orbiters[i].GetGeosynchronousOrbitAltitude();
                var highOrbitAltitude = (float)starSystem.Orbiters[i].GetHighOrbitAltitude();
                var averageDistance = starSystem.OrbitalRegions[i].LocalOrbit.StellarBody.Orbit.GetAverageDistance();
                var circumference = starSystem.OrbitalRegions[i].LocalOrbit.StellarBody.Orbit.GetCircumference();

                var orbit = starSystem.OrbitalRegions[i];
                var solarOrbitNode = 
                    GetOrCreateNode(orbit, starSystem, orbit, averageDistance, null);
                var localOrbitNode = 
                    GetOrCreateNode(orbit.LocalOrbit, starSystem, orbit, averageDistance, null);

                var orbitDistance = .25f * circumference;
                solarOrbitNode.Edges.Add(
                    new NavigableEdge(solarOrbitNode, localOrbitNode, NavigableEdgeType.Space, orbitDistance));
                localOrbitNode.Edges.Add(
                    new NavigableEdge(localOrbitNode, solarOrbitNode, NavigableEdgeType.Space, orbitDistance));

                foreach (var groundRegion in starSystem.Orbiters[i].Regions.SelectMany(x => x.SubRegions))
                {
                    if (groundRegion.Biome.IsTraversable)
                    {
                        foreach (var neighborRegion in groundRegion.Neighbors!.Where(x => x.Biome.IsTraversable))
                        {
                            var groundNode =
                                GetOrCreateNode(
                                    groundRegion, starSystem, orbit, averageDistance, starSystem.Orbiters[i]);
                            var neighborNode =
                                GetOrCreateNode(
                                    neighborRegion, starSystem, orbit, averageDistance, starSystem.Orbiters[i]);
                            groundNode.Edges.Add(
                                new NavigableEdge(
                                    groundNode,
                                    neighborNode,
                                    NavigableEdgeType.Ground,
                                    (float)MathUtils.ArcLength(
                                        groundRegion.Center, neighborRegion.Center, starSystem.Orbiters[i].Radius)));
                        }
                    }
                }

                foreach (var orbitRegion in starSystem.Orbiters[i].OrbitRegions)
                {
                    var orbitNode = GetOrCreateNode(orbitRegion, starSystem, orbit, averageDistance, null);
                    localOrbitNode.Edges.Add(
                        new NavigableEdge(
                            localOrbitNode, 
                            orbitNode,
                            NavigableEdgeType.Space,
                            highOrbitAltitude - geosynchronousOrbitAltitude));
                    orbitNode.Edges.Add(
                        new NavigableEdge(
                            orbitNode,
                            localOrbitNode,
                            NavigableEdgeType.Space, 
                            highOrbitAltitude - geosynchronousOrbitAltitude));

                    foreach (var groundRegion in orbitRegion.SubRegions)
                    {
                        if (groundRegion.Biome.IsTraversable)
                        {
                            var groundNode = 
                                GetOrCreateNode(
                                    groundRegion, starSystem, orbit, averageDistance, starSystem.Orbiters[i]);
                            orbitNode.Edges.Add(
                                new NavigableEdge(
                                    orbitNode, 
                                    groundNode,
                                    NavigableEdgeType.SpaceGround, 
                                    geosynchronousOrbitAltitude));
                            groundNode.Edges.Add(
                                new NavigableEdge(
                                    groundNode,
                                    orbitNode, 
                                    NavigableEdgeType.GroundSpace,
                                    geosynchronousOrbitAltitude));
                        }
                    }
                }
                if (i > 0)
                {
                    var otherOrbitDistance =
                        starSystem.OrbitalRegions[i - 1].LocalOrbit.StellarBody.Orbit.GetAverageDistance();
                    solarOrbitNode.Edges.Add(
                        new NavigableEdge(
                            solarOrbitNode,
                            GetOrCreateNode(
                                starSystem.OrbitalRegions[i - 1], 
                                starSystem, 
                                starSystem.OrbitalRegions[i - 1],
                                otherOrbitDistance, 
                                null), 
                            NavigableEdgeType.Space, 
                            averageDistance - otherOrbitDistance));
                }
                if (i < starSystem.OrbitalRegions.Count - 1)
                {
                    var otherOrbitDistance =
                        starSystem.OrbitalRegions[i + 1].LocalOrbit.StellarBody.Orbit.GetAverageDistance();
                    solarOrbitNode.Edges.Add(
                        new NavigableEdge(
                            solarOrbitNode,
                            GetOrCreateNode(
                                starSystem.OrbitalRegions[i + 1],
                                starSystem, 
                                starSystem.OrbitalRegions[i + 1],
                                otherOrbitDistance,
                                null),
                            NavigableEdgeType.Space,
                            otherOrbitDistance - averageDistance));
                }
            }
            var transits = starSystem.Transits.Values;
            for (int i=0; i<transits.Count; ++i)
            {
                var node = GetOrCreateNode(transits[i], starSystem, null, starSystem.TransitLimit, null);
                var other = transits[i].TransitSystem.Transits.Values.First(x => x.TransitSystem == starSystem);
                var otherNode =
                    GetOrCreateNode(
                        other, 
                        transits[i].TransitSystem,
                        null,
                        transits[i].TransitSystem.TransitLimit, 
                        null);
                node.Edges.Add(
                    new NavigableEdge(
                        node,
                        otherNode,
                        NavigableEdgeType.Jump,
                        (float)GetStarSystemDistance(starSystem, transits[i].TransitSystem)));
                if (transits.Count > 1)
                {
                    node.Edges.Add(
                        new NavigableEdge(
                            node,
                            GetOrCreateNode(
                                transits[(i + transits.Count - 1) % transits.Count], 
                                starSystem,
                                null,
                                starSystem.TransitLimit,
                                null),
                            NavigableEdgeType.Space,
                            (float)MathUtils.ArcLength(
                                transits[i].TransitSystem.Position - starSystem.Position,
                                transits[(i + transits.Count - 1) % transits.Count].TransitSystem.Position
                                - starSystem.Position,
                                starSystem.TransitLimit)));
                    node.Edges.Add(
                        new NavigableEdge(
                            node,
                            GetOrCreateNode(
                                transits[(i + 1) % transits.Count],
                                starSystem,
                                null,
                                starSystem.TransitLimit,
                                null),
                            NavigableEdgeType.Space,
                            (float)MathUtils.ArcLength(
                                transits[i].TransitSystem.Position - starSystem.Position,
                                transits[(i + 1) % transits.Count].TransitSystem.Position - starSystem.Position,
                                starSystem.TransitLimit)));
                }
                if (starSystem.OrbitalRegions.Count > 0)
                {
                    var outerOrbit = starSystem.OrbitalRegions[^1];
                    var orbitDistance = outerOrbit.LocalOrbit.StellarBody.Orbit.GetAverageDistance();
                    var circumference = outerOrbit.LocalOrbit.StellarBody.Orbit.GetCircumference();
                    var distance = starSystem.TransitLimit - orbitDistance + 0.25f * circumference;
                    var outerOrbitNode = GetOrCreateNode(outerOrbit, starSystem, outerOrbit, orbitDistance, null);
                    outerOrbitNode.Edges.Add(
                        new NavigableEdge(outerOrbitNode, node, NavigableEdgeType.Space, distance));
                    node.Edges.Add(new NavigableEdge(node, outerOrbitNode, NavigableEdgeType.Space, distance));
                }
            }
        }
    }
}