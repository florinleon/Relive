/**************************************************************************
 *                                                                        *
 *  Description: RELIVE optimization algorithm   
 *                                                                        *
 *  This algorithm and the reliability optimization problems were         *
 *  presented in the following paper:                                     *
 *  Florin Leon, Petru Cascaval, Costin Badica (2020). Optimization       *
 *  Methods for Redundancy Allocation in Large Systems,                   *
 *  Vietnam Journal of Computer Science, vol. 7, no. 3, pp. 1–19,         *
 *  DOI: 10.1142/S2196888820500165                                        *
 *                                                                        *
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

using Relive;
using System;
using System.Collections.Generic;

namespace Reliability
{
    public enum ReserveType { Active, Passive, Combined }

    public class OptimizationProblem : Problem
    {
        public int n;
        public double cmax, rmin;
        public double[] c, r; // cost, reliability
        public int[] rt; // reserve type
        public int popSize, noGen;

        private Random rand;
        private Dictionary<int, UInt64> factorial;

        public OptimizationProblem()
        {
            n = 50;
            r = new double[] { 0.974, 0.601, 0.928, 0.981, 0.961, 0.979, 0.972, 0.975, 0.978, 0.770, 0.931, 0.975, 0.873, 0.995, 0.999, 0.906, 0.999, 0.999, 0.995, 0.581, 0.890, 0.644, 0.990, 0.557, 0.971, 0.971, 0.936, 0.972, 0.978, 0.976, 0.972, 0.964, 0.728, 0.677, 0.954, 0.820, 0.995, 0.634, 0.943, 0.872, 0.979, 0.842, 0.999, 0.975, 0.830, 0.990, 0.988, 0.842, 0.756, 0.986 };
            c = new double[] { 8, 32, 44, 14, 25, 17, 26, 21, 49, 13, 21, 7, 47, 28, 2, 44, 4, 42, 42, 45, 22, 2, 15, 5, 25, 31, 44, 6, 20, 23, 18, 17, 28, 34, 4, 26, 23, 45, 49, 48, 20, 8, 10, 29, 50, 22, 44, 26, 42, 7 };
            rt = new int[] { 2, 0, 0, 2, 0, 2, 2, 2, 2, 1, 1, 2, 0, 2, 2, 0, 2, 0, 2, 0, 1, 0, 2, 0, 2, 2, 1, 2, 2, 2, 2, 0, 1, 1, 1, 0, 2, 0, 1, 1, 2, 0, 1, 2, 1, 2, 2, 1, 1, 2 };
            cmax = 4000;

            //=================================================================================

            SetSettings();
            InitFactorial();
            InitGA();
        }

        private static void SetSettings()
        {
            Settings.minval = 1;
            Settings.maxval = 15;
            Settings.kmax = 15;
            Settings.s0 = 50;
            Settings.ngen = 500;
            Settings.f = 0.25;
            Settings.lf = 4;
            Settings.lf1 = 3;
            Settings.nn = 20;
            Settings.ni = 20;
            Settings.sigma = 2;
            Settings.pmg = 0.25;
            Settings.pmr = 0.25;
            Settings.pmp = 0.5;
            Settings.pm = 0.2;
            Settings.alpha = 0.5;
        }

        private void InitFactorial()
        {
            rand = new Random();

            factorial = new Dictionary<int, UInt64>();

            UInt64 fact = 1;
            factorial[0] = 1;
            factorial[1] = 1;

            for (int i = 2; i <= Settings.maxval + 1; i++)
            {
                fact *= (UInt64)i;
                factorial[i] = fact;
            }
        }

        private void InitGA()
        {
            popSize = Settings.s0;
            noGen = Settings.ngen;

            EncodingInformation enc = new EncodingInformation(n, EncodingType.RealValued);
            for (int i = 0; i < n; i++)
            {
                enc.MinValues[i] = Settings.minval;
                enc.MaxValues[i] = Settings.maxval;
            }

            SelectionInformation sel = new SelectionInformation
            {
                Type = SelectionType.Tournament,
                TournamentSize = 2,
                Elitism = 1
            };

            CrossoverInformation cro = new CrossoverInformation
            {
                Type = CrossoverType.ArithmeticInteger,
                Probability = 0.9
            };

            MutationInformation mut = new MutationInformation
            {
                Type = MutationType.Gaussian,
                StandardDeviation = Settings.sigma,
                Probability = Settings.pm
            };

            StoppingInformation stop = new StoppingInformation
            {
                Type = StoppingType.Generations,
                MaxGenerations = noGen
            };

            _parameters = new Parameters(popSize, enc, sel, cro, mut, stop);
        }

        public double ComputeReliability(int[] k)
        {
            double reliability = 1;
            double[] rred = new double[n];

            double alpha = Settings.alpha;

            for (int i = 0; i < n; i++)
            {
                rred[i] = 0;

                if (k[i] == 1)
                {
                    rred[i] = r[i];
                }
                else
                    switch ((ReserveType)rt[i])
                    {
                        case ReserveType.Active:
                            rred[i] = 1.0 - Math.Pow(1.0 - r[i], k[i]);
                            break;

                        case ReserveType.Passive:
                            rred[i] = r[i];
                            double lambda_ti = -Math.Log(r[i]);
                            double sum = 0;
                            for (int j = 0; j < k[i]; j++)
                                sum += Math.Pow(lambda_ti, j) / (double)factorial[j];
                            rred[i] *= sum;
                            break;

                        case ReserveType.Combined:
                            if (k[i] == 2)
                            {
                                rred[i] = r[i] - Math.Pow(r[i], alpha) * (r[i] - 1);
                            }
                            else if (k[i] == 3)
                            {
                                double t1 = (1 + alpha) * (1 + alpha) / (alpha * alpha) * r[i];
                                double t2 = (1 + 2 * alpha) / (alpha * alpha);
                                double t3 = (1 + alpha) / alpha * Math.Log(r[i]);
                                double t4 = Math.Pow(r[i], 1 + alpha);
                                rred[i] = t1 - (t2 - t3) * t4;
                            }
                            else if (k[i] == 4)
                            {
                                double ln_ri = Math.Log(r[i]);
                                double t1 = (1 + alpha) * (1 + alpha) * (1 + alpha) / (alpha * alpha * alpha) * r[i];
                                double t2 = (1 + 3 * alpha + 3 * alpha * alpha) / (alpha * alpha * alpha);
                                double t3 = (1 + 3 * alpha + 2 * alpha * alpha) / (alpha * alpha) * ln_ri;
                                double t4 = (1 + alpha) * (1 + alpha) / (2 * alpha) * (ln_ri * ln_ri);
                                double t5 = Math.Pow(r[i], 1 + alpha);
                                rred[i] = t1 - (t2 - t3 + t4) * t5;
                            }
                            else
                                rred[i] = 1e-6; // not implemented, small reliability so that it is not selected
                            break;

                        default:
                            break;
                    }

                reliability *= rred[i];
            }

            return reliability;
        }

        public double ComputeCost(int[] k)
        {
            double cost = 0;
            for (int i = 0; i < n; i++)
                cost += c[i] * k[i];
            return cost;
        }

        public double ComputeEfficiency(int[] k)
        {
            int[] k1 = new int[n];
            for (int i = 0; i < n; i++)
                k1[i] = 1;

            double rsn = ComputeReliability(k1);
            double rsr = ComputeReliability(k);

            return (1.0 - rsn) / (1.0 - rsr);
        }

        public override double FitnessFunction(Chromosome chromosome)
        {
            int[] k = new int[n];

            for (int i = 0; i < n; i++)
            {
                chromosome.RealGenes[i] = (int)(0.5 + chromosome.RealGenes[i]);
                k[i] = (int)(chromosome.RealGenes[i]);
            }

            // repairing the chromosome if it does not satisfy the maximum cost

            double csr = 0;
            for (int i = 0; i < n; i++)
                csr += c[i] * k[i];

            while (csr > cmax)
            {
                int ri = GetHighestCost(k);
                k[ri]--;
                csr -= c[ri];
            }

            for (int i = 0; i < n; i++)
                chromosome.RealGenes[i] = k[i];

            return ComputeReliability(k);
        }

        private int GetHighestCost(int[] k)
        {
            double cx = 0;
            int kx = -1;

            for (int i = 0; i < k.Length; i++)
            {
                if (k[i] < 2)
                    continue;

                if (c[i] > cx)
                {
                    cx = c[i];
                    kx = i;
                }
            }

            return kx;
        }

        private int GetHighestReliabilitySubsystem(int[] k1)
        {
            int[] k2 = new int[k1.Length];

            int kx = -1;
            double rx = 0;

            for (int i = 0; i < k1.Length; i++)
            {
                if (k1[i] < 2)
                    continue;

                for (int j = 0; j < k1.Length; j++)
                {
                    if (i == j)
                        k2[j] = k1[j] - 1;
                    else
                        k2[j] = k1[j];

                    double r = ComputeReliability(k2);

                    if (r > rx)
                    {
                        rx = r;
                        kx = i;
                    }
                }
            }

            return kx;
        }
    }
}