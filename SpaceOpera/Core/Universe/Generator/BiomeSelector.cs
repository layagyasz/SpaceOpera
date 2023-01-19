using Cardamom.Collections;
using Cardamom.Mathematics.Coordinates;
using OpenTK.Mathematics;

namespace SpaceOpera.Core.Universe.Generator
{
    public class BiomeSelector
    {
        public EnumMap<Expression.ParameterName, Expression> BiomeParameters { get; set; } = new();
        public List<BiomeOption> BiomeOptions { get; set; } = new();

        public void Seed(Random random)
        {
            foreach (var field in BiomeParameters.Values)
            {
                if (field != null)
                {
                    field.Seed(random);
                }
            }
        }

        public Biome Select(Vector3 positionCartesian, Spherical3 positionSpherical)
        {
            float[] parameters = new float[Enum.GetValues(typeof(Expression.ParameterName)).Length];
            foreach (var parameter in BiomeParameters)
            {
                parameters[(int)parameter.Key] = 
                    parameter.Value.Evaluate(parameters, positionCartesian, positionSpherical);
            }
            return BiomeOptions.First(x => x.Satisfies(parameters)).Biome;
        }
    }
}