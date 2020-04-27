/**************************************************************************
 *                                                                        *
 *  Description: RELIVE optimization algorithm                            *
 *  Website:     https://github.com/florinleon/Relive                     *
 *  Copyright:   (c) 2020, Florin Leon                                    *
 *                                                                        *
 *  This program is free software; you can redistribute it and/or modify  *
 *  it under the terms of the GNU General Public License as published by  *
 *  the Free Software Foundation. This program is distributed in the      *
 *  hope that it will be useful, but WITHOUT ANY WARRANTY; without even   *
 *  the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR   *
 *  PURPOSE. See the GNU General Public License for more details.         *
 *                                                                        *
 **************************************************************************/

using System;

namespace Relive
{
    internal class ExtendedRandom
    {
        private const double _oneOverRootTwoPi = 0.398942280401433;
        private static Random _UniformRandom = new Random();

        /// <summary>
        /// Probability density for a standard Gaussian distribution
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static double NormalDensity(double x)
        {
            return _oneOverRootTwoPi * Math.Exp(-x * x / 2);
        }

        /// <summary>
        /// The InverseCumulativeNormal function via the Beasley-Springer/Moro approximation
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        private static double InverseCumulativeNormal(double u)
        {
            double[] a = new double[] {  2.50662823884,
                        -18.61500062529,
                         41.39119773534,
                        -25.44106049637};

            double[] b = new double[] {-8.47351093090,
                        23.08336743743,
                       -21.06224101826,
                         3.13082909833};

            double[] c = new double[] {0.3374754822726147,
                        0.9761690190917186,
                        0.1607979714918209,
                        0.0276438810333863,
                        0.0038405729373609,
                        0.0003951896511919,
                        0.0000321767881768,
                        0.0000002888167364,
                        0.0000003960315187};

            double x = u - 0.5;
            double r;

            if (Math.Abs(x) < 0.42) // Beasley-Springer
            {
                double y = x * x;
                r = x * (((a[3] * y + a[2]) * y + a[1]) * y + a[0]) /
                        ((((b[3] * y + b[2]) * y + b[1]) * y + b[0]) * y + 1.0);
            }
            else // Moro
            {
                r = u;
                if (x > 0.0)
                    r = 1.0 - u;
                r = Math.Log(-Math.Log(r));
                r = c[0] + r * (c[1] + r * (c[2] + r * (c[3] + r * (c[4] + r * (c[5] + r * (c[6] +
                        r * (c[7] + r * c[8])))))));
                if (x < 0.0)
                    r = -r;
            }

            return r;
        }

        public static double NextNormal(double mean, double stdev)
        {
            double uniform = 0;
            while (uniform == 0 || uniform == 1)
                uniform = _UniformRandom.NextDouble();

            double standardNormal = InverseCumulativeNormal(uniform);
            return stdev * standardNormal + mean;
        }

        public static double NextUniform()
        {
            return _UniformRandom.NextDouble();
        }
    }
}