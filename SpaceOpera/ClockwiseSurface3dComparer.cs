using Cardamom.Spatial;
using SFML.System;
using SFML.Window;
using SpaceOpera.Core.Voronoi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera
{
    class ClockwiseSurface3dComparer : IComparer<Vector4f>
    {
        ClockwiseVector2fComparer _Subcomparer = new ClockwiseVector2fComparer(new Vector2f());
        SpatialTransformMatrix _Matrix;

        public ClockwiseSurface3dComparer(CSpherical Center)
        {
            _Matrix = new SpatialTransformMatrix()
                .Rotate(Center.Phi - Math.PI / 2, SpatialTransformMatrix.Axis.Z)
                .Rotate(Center.Theta, SpatialTransformMatrix.Axis.X);
        }

        public int Compare(Vector4f A, Vector4f B)
        {
            Vector4f a = _Matrix * A;
            Vector4f b = _Matrix * B;
            return _Subcomparer.Compare(new Vector2f(a.X, a.Y), new Vector2f(b.X, b.Y));
        }
    }
}
