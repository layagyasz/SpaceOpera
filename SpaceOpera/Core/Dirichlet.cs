using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core
{
    class Dirichlet
    {
        public static double[] GetDistribution(Random Random, double[] Weights)
        {
            double[] values = new double[Weights.Length];
            double total = 0;
            for (int i=0;i<values.Length;++i)
            {
                values[i] = Gamma.Sample(Random, Weights[i], 1);
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