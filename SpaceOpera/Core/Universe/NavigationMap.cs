using Cardamom.Collections;
using Cardamom.Utilities;
using SpaceOpera.Core.Voronoi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            public Movement(INavigable Origin, INavigable Destination, NavigableEdgeType Type, float Distance)
            {
                this.Origin = Origin;
                this.Destination = Destination;
                this.Type = Type;
                this.Distance = Distance;
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
            public INavigable SystemNode { get; }
            public double OrbitDistance { get; }
            public StellarBody StellarBody { get; }
            public List<NavigableEdge> Edges { get; } = new List<NavigableEdge>();

            // Used by A*
            public NavigableEdge Parent { get; set; }
            public float Distance { get; set; } = float.PositiveInfinity;

            public NavigableNode(
                INavigable Navigable,
                StarSystem StarSystem, 
                INavigable SystemNode,
                double OrbitDistance, 
                StellarBody StellarBody)
            {
                this.Navigable = Navigable;
                this.StarSystem = StarSystem;
                this.SystemNode = SystemNode;
                this.OrbitDistance = OrbitDistance;
                this.StellarBody = StellarBody;
            }
        }

        class NavigableEdge
        {
            public NavigableNode Start { get; }
            public NavigableNode End { get; }
            public NavigableEdgeType Type { get; }
            public float Distance { get; }

            public NavigableEdge(NavigableNode Start, NavigableNode End, NavigableEdgeType Type, float Distance)
            {
                this.Start = Start;
                this.End = End;
                this.Type = Type;
                this.Distance = Distance;
            }
        }

        private readonly Dictionary<INavigable, NavigableNode> _Nodes = new Dictionary<INavigable, NavigableNode>();

        public NavigationMap(Galaxy Galaxy)
        {
            foreach (var system in Galaxy.Systems)
            {
                Populate(system);
            }
        }

        public Stack<Movement> FindPath(INavigable Start, INavigable End, EnumSet<NavigableEdgeType> AllowedEdges)
        {
            PriorityQueue<NavigableNode, double> open = new PriorityQueue<NavigableNode, double>();
            HashSet<NavigableNode> openSet = new HashSet<NavigableNode>();
            HashSet<NavigableNode> closedSet = new HashSet<NavigableNode>();

            var startNode = _Nodes[Start];
            var endNode = _Nodes[End];
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
                    if (AllowedEdges.Contains(edge.Type))
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
                        current.Parent.Start.Navigable,
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

        public StarSystem GetStarSystem(INavigable Navigable)
        {
            return _Nodes[Navigable].StarSystem;
        }

        public INavigable GetSystemNode(INavigable Navigable)
        {
            return _Nodes[Navigable].SystemNode;
        }

        public HashSet<INavigable> GetLocalOrbitNodes(LocalOrbitRegion LocalOrbit, EnumSet<NavigableNodeType> Types)
        {
            var nodes = new HashSet<INavigable>();
            nodes.Add(LocalOrbit);
            if (Types.Contains(NavigableNodeType.Space))
            {
                foreach (var node in LocalOrbit.StellarBody.OrbitRegions)
                {
                    nodes.Add(node);
                }
            }
            if (Types.Contains(NavigableNodeType.Ground) || Types.Contains(NavigableNodeType.Sea))
            {
                foreach (var node in LocalOrbit.StellarBody.Regions.SelectMany(x => x.SubRegions))
                {
                    if (Types.Contains(node.NavigableNodeType))
                    {
                        nodes.Add(node);
                    }
                }
            }
            return nodes;
        }

        public HashSet<INavigable> GetSystemNodes(StarSystem StarSystem, EnumSet<NavigableNodeType> Types)
        {
            var nodes = new HashSet<INavigable>();
            if (Types.Contains(NavigableNodeType.Space))
            {
                foreach (var node in StarSystem.OrbitalRegions)
                {
                    nodes.Add(node);
                    nodes.Add(node.LocalOrbit);
                    foreach (var stellarBodyNode in node.LocalOrbit.StellarBody.OrbitRegions)
                    {
                        nodes.Add(stellarBodyNode);
                    }
                }
                foreach (var node in StarSystem.Transits)
                {
                    nodes.Add(node.Value);
                }
            }
            if (Types.Contains(NavigableNodeType.Ground) || Types.Contains(NavigableNodeType.Sea))
            {
                foreach (
                    var node in StarSystem.OrbitalRegions.SelectMany(
                        x => x.LocalOrbit.StellarBody.Regions).SelectMany(x => x.SubRegions))
                {
                    if (Types.Contains(node.NavigableNodeType))
                    {
                        nodes.Add(node);
                    }
                }
            }
            return nodes;
        }

        public bool IsSystemNode(INavigable Node, StarSystem StarSystem)
        {
            if (Node is SolarOrbitRegion solarOrbit)
            {
                return StarSystem.OrbitalRegions.Contains(Node);
            }
            if (Node is LocalOrbitRegion localOrbit)
            {
                return StarSystem.OrbitalRegions.Any(x => x.LocalOrbit == localOrbit);
            }
            if (Node is TransitRegion transit)
            {
                return StarSystem.Transits.Values.Contains(transit);
            }
            return false;
        }

        private NavigableNode GetOrCreateNode(
            INavigable Navigable, 
            StarSystem StarSystem,
            INavigable SystemNode, 
            double OrbitDistance, 
            StellarBody StellarBody)
        {
            if (_Nodes.TryGetValue(Navigable, out NavigableNode node))
            {
                return node;
            }
            else
            {
                node = new NavigableNode(Navigable, StarSystem, SystemNode, OrbitDistance, StellarBody);
                _Nodes.Add(Navigable, node);
                return node;
            }
        }

        private double GetStarSystemDistance(StarSystem Left, StarSystem Right)
        {
            return Constants.LY_TO_AU * MathUtils.Distance(Left.Position, Right.Position);
        }

        private double HeuristicDistance(NavigableNode Left, NavigableNode Right)
        {
            double distance = 0;
            if (Left.StellarBody == Right.StellarBody && Left.StellarBody != null)
            {
                return MathUtils.ArcLength(
                    ((StellarBodySubRegion)Left.Navigable).Center, 
                    ((StellarBodySubRegion)Right.Navigable).Center, 
                    Left.StellarBody.Radius);
            }
            else
            {
                if (Left.StellarBody != null)
                {
                    distance += Left.StellarBody.GetGeosynchronousOrbitAltitude();
                }
                if (Right.StellarBody != null)
                {
                    distance += Right.StellarBody.GetGeosynchronousOrbitAltitude();
                }
            }
            if (Left.StarSystem != Right.StarSystem)
            {
                distance += GetStarSystemDistance(Left.StarSystem, Right.StarSystem);
                distance += (Left.StarSystem.TransitLimit - Left.OrbitDistance);
                distance += (Right.StarSystem.TransitLimit - Right.OrbitDistance);
            }
            else
            {
                distance += Math.Abs(Left.OrbitDistance - Right.OrbitDistance);
            }
            return distance;
        }

        private void Populate(StarSystem StarSystem)
        {
            for (int i=0; i<StarSystem.OrbitalRegions.Count;++i)
            {
                var geosynchronousOrbitAltitude = (float)StarSystem.Orbiters[i].GetGeosynchronousOrbitAltitude();
                var highOrbitAltitude = (float)StarSystem.Orbiters[i].GetHighOrbitAltitude();
                var averageDistance = StarSystem.OrbitalRegions[i].LocalOrbit.StellarBody.Orbit.GetAverageDistance();
                var circumference = StarSystem.OrbitalRegions[i].LocalOrbit.StellarBody.Orbit.GetCircumference();

                var solarOrbitNode = 
                    GetOrCreateNode(
                        StarSystem.OrbitalRegions[i], StarSystem, StarSystem.OrbitalRegions[i], averageDistance, null);
                var localOrbit = StarSystem.OrbitalRegions[i].LocalOrbit;
                var localOrbitNode = 
                    GetOrCreateNode(localOrbit, StarSystem, localOrbit, averageDistance, null);

                var orbitDistance = .25f * circumference;
                solarOrbitNode.Edges.Add(
                    new NavigableEdge(solarOrbitNode, localOrbitNode, NavigableEdgeType.Space, orbitDistance));
                localOrbitNode.Edges.Add(
                    new NavigableEdge(localOrbitNode, solarOrbitNode, NavigableEdgeType.Space, orbitDistance));

                foreach (var groundRegion in StarSystem.Orbiters[i].Regions.SelectMany(x => x.SubRegions))
                {
                    if (groundRegion.Biome.IsTraversable)
                    {
                        foreach (var neighborRegion in groundRegion.Neighbors.Where(x => x.Biome.IsTraversable))
                        {
                            var groundNode =
                                GetOrCreateNode(
                                    groundRegion, StarSystem, localOrbit, averageDistance, StarSystem.Orbiters[i]);
                            var neighborNode =
                                GetOrCreateNode(
                                    neighborRegion, StarSystem, localOrbit, averageDistance, StarSystem.Orbiters[i]);
                            groundNode.Edges.Add(
                                new NavigableEdge(
                                    groundNode,
                                    neighborNode,
                                    NavigableEdgeType.Ground,
                                    (float)MathUtils.ArcLength(
                                        groundRegion.Center, neighborRegion.Center, StarSystem.Orbiters[i].Radius)));
                        }
                    }
                }

                foreach (var orbitRegion in StarSystem.Orbiters[i].OrbitRegions)
                {
                    var orbitNode = GetOrCreateNode(orbitRegion, StarSystem, localOrbit, averageDistance, null);
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
                                    groundRegion, StarSystem, localOrbit, averageDistance, StarSystem.Orbiters[i]);
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
                        StarSystem.OrbitalRegions[i - 1].LocalOrbit.StellarBody.Orbit.GetAverageDistance();
                    solarOrbitNode.Edges.Add(
                        new NavigableEdge(
                            solarOrbitNode,
                            GetOrCreateNode(
                                StarSystem.OrbitalRegions[i - 1], 
                                StarSystem, 
                                StarSystem.OrbitalRegions[i - 1],
                                otherOrbitDistance, 
                                null), 
                            NavigableEdgeType.Space, 
                            averageDistance - otherOrbitDistance));
                }
                if (i < StarSystem.OrbitalRegions.Count - 1)
                {
                    var otherOrbitDistance =
                        StarSystem.OrbitalRegions[i + 1].LocalOrbit.StellarBody.Orbit.GetAverageDistance();
                    solarOrbitNode.Edges.Add(
                        new NavigableEdge(
                            solarOrbitNode,
                            GetOrCreateNode(
                                StarSystem.OrbitalRegions[i + 1],
                                StarSystem, 
                                StarSystem.OrbitalRegions[i + 1],
                                otherOrbitDistance,
                                null),
                            NavigableEdgeType.Space,
                            otherOrbitDistance - averageDistance));
                }
            }
            var transits = StarSystem.Transits.Values;
            for (int i=0; i<transits.Count; ++i)
            {
                var node = GetOrCreateNode(transits[i], StarSystem, transits[i], StarSystem.TransitLimit, null);
                var other = transits[i].TransitSystem.Transits.Values.First(x => x.TransitSystem == StarSystem);
                var otherNode =
                    GetOrCreateNode(
                        other, 
                        transits[i].TransitSystem,
                        other,
                        transits[i].TransitSystem.TransitLimit, 
                        null);
                node.Edges.Add(
                    new NavigableEdge(
                        node,
                        otherNode,
                        NavigableEdgeType.Jump,
                        (float)GetStarSystemDistance(StarSystem, transits[i].TransitSystem)));
                if (transits.Count > 1)
                {
                    node.Edges.Add(
                        new NavigableEdge(
                            node,
                            GetOrCreateNode(
                                transits[(i + transits.Count - 1) % transits.Count], 
                                StarSystem,
                                transits[(i + transits.Count - 1) % transits.Count],
                                StarSystem.TransitLimit,
                                null),
                            NavigableEdgeType.Space,
                            (float)MathUtils.ArcLength(
                                transits[i].TransitSystem.Position - StarSystem.Position,
                                transits[(i + transits.Count - 1) % transits.Count].TransitSystem.Position
                                - StarSystem.Position,
                                StarSystem.TransitLimit)));
                    node.Edges.Add(
                        new NavigableEdge(
                            node,
                            GetOrCreateNode(
                                transits[(i + 1) % transits.Count],
                                StarSystem,
                                transits[(i + 1) % transits.Count],
                                StarSystem.TransitLimit,
                                null),
                            NavigableEdgeType.Space,
                            (float)MathUtils.ArcLength(
                                transits[i].TransitSystem.Position - StarSystem.Position,
                                transits[(i + 1) % transits.Count].TransitSystem.Position - StarSystem.Position,
                                StarSystem.TransitLimit)));
                }
                if (StarSystem.OrbitalRegions.Count > 0)
                {
                    var outerOrbit = StarSystem.OrbitalRegions[StarSystem.OrbitalRegions.Count - 1];
                    var orbitDistance = outerOrbit.LocalOrbit.StellarBody.Orbit.GetAverageDistance();
                    var circumference = outerOrbit.LocalOrbit.StellarBody.Orbit.GetCircumference();
                    var distance = StarSystem.TransitLimit - orbitDistance + 0.25f * circumference;
                    var outerOrbitNode = GetOrCreateNode(outerOrbit, StarSystem, outerOrbit, orbitDistance, null);
                    outerOrbitNode.Edges.Add(
                        new NavigableEdge(outerOrbitNode, node, NavigableEdgeType.Space, distance));
                    node.Edges.Add(new NavigableEdge(node, outerOrbitNode, NavigableEdgeType.Space, distance));
                }
            }
        }
    }
}