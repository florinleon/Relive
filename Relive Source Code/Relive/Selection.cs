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

namespace Relive
{
    public class Selection
    {
        private static int[] _ranks = null;

        public static Chromosome PerformSelection(Chromosome[] population, SelectionInformation info)
        {
            switch (info.Type)
            {
                case SelectionType.Tournament:
                    return Tournament(population, info.TournamentSize);

                case SelectionType.RouletteWheel:
                    return Roulette(population);

                case SelectionType.Rank:
                    return Rank(population, info.AlphaRank, info.BetaRank);
            }

            return null;
        }

        private static Chromosome Tournament(Chromosome[] population, int tournamentSize)
        {
            //bool[] selected = new bool[population.Length];
            //for (int i=0; i<population.Length; i++)
            //    selected[i] = false;

            /*for (int i = 0; i < tournamentSize; i++)
            {
                bool ok = false;
                while (!ok)
                {
                    int crtSel = (int) (ExtendedRandom.NextUniform() * population.Length);
                    if (!selected[crtSel])
                    {
                        selected[crtSel] = true;
                        ok = true;
                    }
                }
            }*/

            //double bestFitness = double.MinValue;
            //int bestIndex = -1;

            //for (int i = 0; i < population.Length; i++)
            //{
            //    if (selected[i])
            //    {
            //        if (population[i].Fitness > bestFitness)
            //        {
            //            bestFitness = population[i].Fitness;
            //            bestIndex = i;
            //        }
            //    }
            //}

            int[] selectedOrder = Sorting.RandomRankVector(population.Length);
            double bestFitness = double.MinValue;
            int bestIndex = -1;

            for (int i = 0; i < tournamentSize; i++)
            {
                if (population[selectedOrder[i]].Fitness > bestFitness)
                {
                    bestFitness = population[selectedOrder[i]].Fitness;
                    bestIndex = i;
                }
            }

            return population[bestIndex];
        }

        private static Chromosome Roulette(Chromosome[] population)
        {
            double partsum = 0, sum = 0;
            double worstFitness = double.MaxValue;

            for (int i = 0; i < population.Length; i++)
            {
                sum += population[i].Fitness;
                if (worstFitness > population[i].Fitness)
                    worstFitness = population[i].Fitness;
            }

            // fitness scaling -> F(x) = f(x) - worst
            sum -= worstFitness * population.Length;

            double r = ExtendedRandom.NextUniform() * sum;

            for (int i = 0; i < population.Length; i++)
            {
                partsum += (population[i].Fitness - worstFitness);
                if (partsum >= r)
                    return population[i];
            }

            return null;
        }

        public static void SortPopulation(Chromosome[] population)
        {
            int size = population.Length;

            int[] no = new int[size];
            Chromosome[] popBackup = new Chromosome[size];
            int i = 0;

            for (i = 0; i < size; i++)
            {
                no[i] = i;
                popBackup[i] = new Chromosome(population[i]);
            }

            int dist = 0, j = 0, auxi = 0;
            Chromosome auxc = null;

            for (dist = size / 2; dist > 0; dist /= 2)
                for (i = dist; i < size; i++)
                    for (j = i - dist; j >= 0 && popBackup[j].Fitness < popBackup[j + dist].Fitness; j -= dist)
                    {
                        auxi = no[j]; no[j] = no[j + dist]; no[j + dist] = auxi;
                        auxc = popBackup[j]; popBackup[j] = popBackup[j + dist]; popBackup[j + dist] = auxc;
                    }

            _ranks = new int[size];
            for (i = 0; i < size; i++)
                _ranks[no[i]] = size - i;
        }

        public static Chromosome[] GetBestHalfOfPopulation(Chromosome[] population)
        {
            int size = population.Length;

            Chromosome[] popBackup = new Chromosome[size];
            int i = 0;

            for (i = 0; i < size; i++)
                popBackup[i] = new Chromosome(population[i]);

            int dist = 0, j = 0;
            Chromosome auxc = null;

            for (dist = size / 2; dist > 0; dist /= 2)
                for (i = dist; i < size; i++)
                    for (j = i - dist; j >= 0 && popBackup[j].Fitness < popBackup[j + dist].Fitness; j -= dist)
                    {
                        auxc = popBackup[j]; popBackup[j] = popBackup[j + dist]; popBackup[j + dist] = auxc;
                    }

            Chromosome[] bestHalf = new Chromosome[size / 2];
            for (i = 0; i < size / 2; i++)
                bestHalf[i] = popBackup[i];
            return bestHalf;
        }

        private static Chromosome Rank(Chromosome[] population, double alpha, double beta)
        {
            // assumes that the population has been previously sorted
            // each chromosome has the rank computed in "ranks"

            double partsum = 0;
            double rankSum = population.Length * (population.Length + 1) / 2.0;
            double r = ExtendedRandom.NextUniform() * rankSum;

            for (int i = 0; i < population.Length; i++)
            {
                partsum += _ranks[i];
                if (partsum > r)
                    return population[i];
            }

            return null;
        }

        public static Chromosome[] GetElite(Chromosome[] population, int elitism)
        {
            if (elitism <= 0)
                return null;

            Chromosome[] elite = new Chromosome[elitism];

            if (elitism == 1) // return the best
            {
                double maxFitness = population[0].Fitness;
                int maxIndex = 0;

                for (int i = 1; i < population.Length; i++)
                {
                    if (population[i].Fitness > maxFitness)
                    {
                        maxFitness = population[i].Fitness;
                        maxIndex = i;
                    }
                }

                elite[0] = population[maxIndex];
            }
            else
            {
                Chromosome[] popBackup = new Chromosome[population.Length];
                for (int i = 0; i < population.Length; i++)
                    popBackup[i] = new Chromosome(population[i]);

                int dist = 0, j = 0;
                Chromosome auxc = null;
                int size = population.Length;

                // sorting population descending by fitness
                for (dist = size / 2; dist > 0; dist /= 2)
                    for (int i = dist; i < size; i++)
                        for (j = i - dist; j >= 0 && popBackup[j].Fitness < popBackup[j + dist].Fitness; j -= dist)
                        {
                            auxc = popBackup[j]; popBackup[j] = popBackup[j + dist]; popBackup[j + dist] = auxc;
                        }

                for (int i = 0; i < elitism; i++)
                    elite[i] = popBackup[i];
            }

            return elite;
        }

        public static Chromosome GetBest(Chromosome[] population)
        {
            double maxFitness = population[0].Fitness;
            int maxIndex = 0;

            for (int i = 1; i < population.Length; i++)
            {
                if (population[i].Fitness > maxFitness)
                {
                    maxFitness = population[i].Fitness;
                    maxIndex = i;
                }
            }

            return population[maxIndex];
        }
    }
}