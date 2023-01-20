using MathNet.Numerics.Distributions;

namespace SpaceOpera.Core
{
    public class Dirichlet
    {
        public static double[] GetDistribution(Random random, double[] weights)
        {
            double[] values = new double[weights.Length];
            double total = 0;
            for (int i=0;i<values.Length;++i)
            {
                values[i] = Gamma.Sample(random, weights[i], 1);
                total += values[i];
            }
            for (int i=0; i<values.Length;++i)
            {
                values[i] /= total;
            }
            return values;
        }
    }
}