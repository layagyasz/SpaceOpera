using Cardamom.Spatial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera
{
    struct CSpherical
    {
        public float Radius { get; set; }
        public float Theta { get; set; }
        public float Phi { get; set; }

        public CSpherical(float Radius, float Theta, float Phi)
        {
            this.Radius = Radius;
            this.Theta = Theta;
            this.Phi = Phi;
        }

        public Vector4f ToCartesian()
        {
            return new Vector4f(
                (float)(Radius * Math.Cos(Phi) * Math.Sin(Theta)), 
                (float)(Radius * Math.Sin(Phi) * Math.Sin(Theta)), 
                (float)(Radius * Math.Cos(Theta)));
        }

        public static CSpherical FromCartesian(Vector4f Vector)
        {
            return new CSpherical(
                (float)Math.Sqrt(Vector.X * Vector.X + Vector.Y * Vector.Y + Vector.Z * Vector.Z), 
                (float)Math.Atan2(Math.Sqrt(Vector.X * Vector.X + Vector.Y * Vector.Y), Vector.Z),
                (float)Math.Atan2(Vector.Y, Vector.X));
        }

        public static CSpherical operator *(float M, CSpherical C)
        {
            return new CSpherical(M * C.Radius, C.Theta, C.Phi);
        }

        public override string ToString()
        {
            return string.Format("[CSpherical: Radius={0}, Theta={1}, Phi={2}]", Radius, Theta, Phi);
        }
    }
}