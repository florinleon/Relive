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

using Reliability;
using Relive;
using System;
using System.IO;

namespace ReliveExamples
{
    public class Program
    {
        private static void Main(string[] args)
        {
            OptimizationProblem p = new OptimizationProblem();
            ReliveAlgorithm solver = new ReliveAlgorithm();
            solver.Solve(p);

            int[] k = new int[p.n];
            for (int i = 0; i < solver.Solution.NoGenes; i++)
                k[i] = (int)solver.Solution.RealGenes[i];

            StreamWriter sw = new StreamWriter("result.txt");

            Console.WriteLine("Maximizing reliability\r\n");

            double cost = 0;
            for (int i = 0; i < solver.Solution.NoGenes; i++)
                cost += p.c[i] * k[i];

            Console.Write("Solution:\r\n");

            for (int i = 0; i < solver.Solution.NoGenes; i++)
            {
                Console.Write($"{k[i]}, ");
                sw.Write($"{k[i]}, ");
            }

            Console.WriteLine($"\r\nCost: {cost:F0}\r\nReliability: {solver.Solution.Fitness:F4}\r\n");
            sw.WriteLine($"\r\nCost: {cost:F0}\r\nReliability: {solver.Solution.Fitness:F4}\r\n");

            double eff = p.ComputeEfficiency(k);
            Console.WriteLine($"Efficiency: {eff:F2}\r\n");
            sw.WriteLine($"Efficiency: {eff:F2}\r\n");

            sw.Close();
        }
    }
}