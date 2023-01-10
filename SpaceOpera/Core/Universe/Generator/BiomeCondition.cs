using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Universe.Generator
{
    class BiomeCondition
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Expression.ParameterName ParameterName { get; set; }
        public float Minimum { get; set; }
        public float Maximum { get; set; }

        public bool Satisfies(float[] Parameters)
        {
            float p = Math.Min(Math.Max(Parameters[(int)ParameterName], 0), 1);
            return p >= Minimum && p <= Maximum;
        }
    }
}