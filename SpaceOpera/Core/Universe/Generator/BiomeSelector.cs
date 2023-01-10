using Cardamom.Spatial;
using SpaceOpera.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Universe.Generator
{
    class BiomeSelector
    {
        [JsonConverter(typeof(EnumMapJsonConverter<Expression.ParameterName, Expression>))]
        public EnumMap<Expression.ParameterName, Expression> BiomeParameters { get; set; }
        public List<BiomeOption> BiomeOptions { get; set; }

        public void Seed(Random Random)
        {
            foreach (var field in BiomeParameters.Values)
            {
                if (field != null)
                {
                    field.Seed(Random);
                }
            }
        }

        public Biome Select(Vector4f PositionCartesian, CSpherical PositionSpherical)
        {
            float[] parameters = new float[Enum.GetValues(typeof(Expression.ParameterName)).Length];
            foreach (var parameter in BiomeParameters)
            {
                parameters[(int)parameter.Key] = 
                    parameter.Value.Evaluate(parameters, PositionCartesian, PositionSpherical);
            }
            return BiomeOptions.First(x => x.Satisfies(parameters)).Biome;
        }
    }
}