using Cardamom.Mathematics.Geometry;
using OpenTK.Mathematics;

namespace SpaceOpera.View
{
    public static class Utils
    {
        public struct WideSegment
        {
            public Vector3 NearLeft { get; set; }
            public Vector3 NearRight { get; set; }
            public Vector3 FarLeft { get; set; }
            public Vector3 FarRight { get; set; }

            public WideSegment(Vector3 nearLeft,  Vector3 nearRight, Vector3 farLeft, Vector3 farRight)
            {
                NearLeft = nearLeft;
                NearRight = nearRight;
                FarLeft = farLeft;
                FarRight = farRight;
            }
        }

        public static WideSegment CreateSegment(Ray3 ray, float length, Vector3 axis, float width)
        {
            var far = ray.Get(length);
            var p = Vector3.Cross(ray.Direction, axis);
            p *= 0.5f * width;
            return new(ray.Point + p, ray.Point - p, far + p, far - p);
        }

        public static WideSegment CreateSegment(Vector3 near, Vector3 far, Vector3 axis, float width)
        {
            var d = far - near;
            float l = d.Length;
            d.Normalize();
            return CreateSegment(new(near, d), l, axis, width);
        }
    }
}
