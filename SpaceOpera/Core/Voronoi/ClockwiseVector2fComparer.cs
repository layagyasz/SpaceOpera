using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Voronoi
{
    class ClockwiseVector2fComparer : IComparer<Vector2f>
    {
        public Vector2f Center { get; }

        public ClockwiseVector2fComparer(Vector2f Center)
        {
            this.Center = Center;
        }

        public int Compare(Vector2f A, Vector2f B)
        {
            Vector2f a = A - Center;
            Vector2f b = B - Center;

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