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
    public class EvolutionaryHillClimbing : GenericSolver
    {
        private Problem _problem;
        private Chromosome[] _population;
        private Chromosome _initialValue = null;

        private static Random _rand = new Random();

        private int _noNeighbors = Settings.nn;
        private int _noIterations = Settings.ni;

        public void SetInitialValue(Chromosome initial)
        {
            _initialValue = initial;
        }

        public override void Solve(Problem problem)
        {
            if (_initialValue == null)
                throw new Exception("You must provide a starting point for the search");

            _problem = problem;

            _population = new Chromosome[_noNeighbors];

            bool searchOver = false;
            int generations = 0;
            Chromosome best = new Chromosome(_initialValue);

            while (!searchOver)
            {
                _population[0] = best;
                _population[0].ComputeFitness(_problem);

                // generating population by mutation
                for (int i = 1; i < _noNeighbors; i++)
                {
                    _population[i] = new Chromosome(best);

                    double r = _rand.NextDouble();

                    if (r < Settings.pmg)
                        Mutation.PerformMutation(_population[i], _problem.Parameters); // mutatie gaussiana
                    else if (r < Settings.pmg + Settings.pmr) // mutatie prin resetare
                    {
                        for (int j = 0; j < _population[i].NoGenes; j++)
                        {
                            if (_rand.NextDouble() < Settings.pm)
                                _population[i].RealGenes[j] = _problem.Parameters.EncodingInfo.MinValues[j] + ExtendedRandom.NextUniform() * (_problem.Parameters.EncodingInfo.MaxValues[j] - _problem.Parameters.EncodingInfo.MinValues[j]);
                        }
                    }
                    else
                    {
                        int i1 = _rand.Next(_population[i].RealGenes.Length);
                        int i2 = _rand.Next(_population[i].RealGenes.Length);
                        if (_population[i].RealGenes[i1] >= 2 && _population[i].RealGenes[i2] >= 2)
                        {
                            _population[i].RealGenes[i1] -= 1;
                            _population[i].RealGenes[i2] += 1;
                        }
                    }

                    _population[i].ComputeFitness(_problem);
                }

                best = Selection.GetBest(_population);
                generations++;

                if (generations > _noIterations)
                {
                    _solution = best;
                    searchOver = true;
                }
            }
        }
    }
}