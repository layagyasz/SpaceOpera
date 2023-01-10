using Cardamom.Planar;
using Cardamom.Utilities;
using DelaunayTriangulator;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Voronoi
{
    class VoronoiGrapher
    {
        public class NeighborsResult
        {
            public List<List<int>> Neighbors { get; set; }
            public List<int> EdgeIndices { get; set; }
        }

        public static Vector2f GetCircumcenter(Vector2f A, Vector2f B, Vector2f C)
        {
            var points = new List<Vertex>()
            {
                new Vertex(A.X, A.Y),
                new Vertex(B.X, B.Y),
                new Vertex(C.X, C.Y)
            };
            var triad = new Triad(0, 1, 2);
            triad.FindCircumcirclePrecisely(points);
            return new Vector2f(triad.circumcircleX, triad.circumcircleY);
        }

        public static List<Triad> GetTriangulation(List<Vertex> Vertices)
        {
            Triangulator triangulator = new Triangulator();
            return triangulator.Triangulation(Vertices);
        }

        public static NeighborsResult GetNeighbors(List<Vertex> Vertices, List<Triad> Triads)
        {
            List<WrapperNode> wrapperNodes = Vertices.Select(x => new WrapperNode()).ToList();
            foreach (var triad in Triads)
            {
                wrapperNodes[triad.a].Triads.Add(triad);
                wrapperNodes[triad.b].Triads.Add(triad);
                wrapperNodes[triad.c].Triads.Add(triad);
            }

            List<List<int>> allNeighbors = new List<List<int>>();
            List<int> edgeIndices = new List<int>();
            for (int i = 0; i < Vertices.Count; ++i)
            {
                List<int> neighbors = new List<int>();
                for (int j = 0; j < wrapperNodes[i].Triads.Count; ++j)
                {
                    Triad triad = wrapperNodes[i].Triads[j];
                    if (i == triad.a)
                    {
                        neighbors.Add(triad.b);
                        neighbors.Add(triad.c);
                        if (triad.ab == -1 || triad.ac == -1)
                        {
                            neighbors.Add(-1);
                            edgeIndices.Add(i);
                        }
                    }
                    else if (i == triad.b)
                    {
                        neighbors.Add(triad.a);
                        neighbors.Add(triad.c);
                        if (triad.ab == -1 || triad.bc == -1)
                        {
                            neighbors.Add(-1);
                            edgeIndices.Add(i);
                        }
                    }
                    else
                    {
                        neighbors.Add(triad.a);
                        neighbors.Add(triad.b);
                        if (triad.bc == -1 || triad.ac == -1)
                        {
                            neighbors.Add(-1);
                            edgeIndices.Add(i);
                        }
                    }
                }
                allNeighbors.Add(neighbors.Distinct().ToList());
            }

            return new NeighborsResult()
            {
                Neighbors = allNeighbors,
                EdgeIndices = edgeIndices.Distinct().ToList()
            };
        }

        private class WrapperNode
        {
            public List<Triad> Triads { get; } = new List<Triad>();
        }
    }
}