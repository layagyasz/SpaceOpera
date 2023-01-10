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
    class ClockwiseTriadComparer : IComparer<Triad>
    {
        public Vector2f Center { get; }
        public Func<Triad, Vector2f> CenterFn { get; }

        public ClockwiseTriadComparer(Vector2f Center, Func<Triad, Vector2f> CenterFn)
        {
            this.Center = Center;
            this.CenterFn = CenterFn;
        }

        public int Compare(Triad A, Triad B)
        {
            Vector2f a = CenterFn(A) - Center;
            Vector2f b = CenterFn(B) - Center;

            if (a.X >= 0 && b.X < 0)
            {
                return 1;
            }
            if (a.X < 0 && b.X >= 0)
            {
                return -1;
            }

            return a.X * b.Y - a.Y * b.X < 0 ? 1 : -1;
        }
    }
}