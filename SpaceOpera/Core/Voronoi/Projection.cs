using Cardamom.Spatial;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Voronoi
{
    class Projection
    {
        public class StereographicProjection : IProjection<Vector2f, Vector4f>
        {
            public Vector2f Project(Vector4f Vector)
            {
                return new Vector2f(Vector.X / (1 - Vector.Z), Vector.Y / (1 - Vector.Z));
            }

            public Vector4f Wrap(Vector2f Vector)
            {
                float denominator = 1 / (1 + Vector.X * Vector.X + Vector.Y * Vector.Y);
                return new Vector4f(
                    2 * denominator * Vector.X, 
                    2 * denominator * Vector.Y,
                    denominator * (Vector.X * Vector.X + Vector.Y * Vector.Y - 1));
            }
        }

        public class Inverse3dProjection : IProjection<Vector4f, Vector4f>
        {
            public Vector4f Project(Vector4f Vector)
            {
                return new Vector4f(-Vector.X, -Vector.Y, -Vector.Z);
            }

            public Vector4f Wrap(Vector4f Vector)
            {
                return Project(Vector);
            }
        }
    }
}