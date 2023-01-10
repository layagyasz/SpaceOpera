using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera
{
    class Range
    {
        public float Minimum { get; set; }
        public float Maximum { get; set; }

        public Range()
        {
            this.Minimum = float.NegativeInfinity;
            this.Maximum = float.PositiveInfinity;
        }

        public Range(float Minimum, float Maximum)
        {
            this.Minimum = Minimum;
            this.Maximum = Maximum;
        }

        public bool Contains(float Value)
        {
            return Value > Minimum && Value < Maximum;
        }

        public static Range Intersection(Range Left, Range Right)
        {
            return new Range(Math.Max(Left.Minimum, Right.Minimum), Math.Min(Left.Maximum, Right.Maximum));
        }

        public static Range Union(Range Left, Range Right)
        {
            return new Range(Math.Min(Left.Minimum, Right.Minimum), Math.Max(Left.Maximum, Right.Maximum));
        }
    }
}