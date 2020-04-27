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
using System.Collections.Generic;

namespace Relive
{
    public class ReliveAlgorithm : GenericSolver
    {
        private Problem _problem;
        private Chromosome[] _population;
        private static Random _rand = new Random();

        public override void Solve(Problem problem)
        {
            _problem = problem;

            // generating the initial random population
            _population = new Chromosome[_problem.Parameters.PopulationSize];
            for (int i = 0; i < _problem.Parameters.PopulationSize; i++)
            {
                _population[i] = new Chromosome(_problem.Parameters, true)
                {
                    Age = Settings.lf + _rand.Next(Settings.lf)
                };
            }

            bool searchOver = false;
            int generations = 0;

            while (!searchOver)
            {
                for (int i = 0; i < _population.Length; i++)
                {
                    int age = _population[i].Age;
                    Chromosome c = PerformHC(_population[i]);
                    _population[i] = c;
                    _population[i].Age = age - 1;
                    _population[i].ComputeFitness(_problem);
                }

                Chromosome[] newPopulation = new Chromosome[_problem.Parameters.PopulationSize];

                if (_problem.Parameters.SelectionInfo.Elitism > 0)
                {
                    Chromosome[] elite = Selection.GetElite(_population, _problem.Parameters.SelectionInfo.Elitism);
                    for (int i = 0; i < _problem.Parameters.SelectionInfo.Elitism; i++)
                    {
                        newPopulation[i] = elite[i];
                        newPopulation[i].Age = Settings.lf;
                    }
                }

                //if (_problem.Parameters.SelectionInfo.Type == SelectionType.Rank)
                //    Selection.SortPopulation(_population);

                for (int i = problem.Parameters.SelectionInfo.Elitism; i < _problem.Parameters.PopulationSize; i++)
                {
                    // select
                    Chromosome mother = Selection.PerformSelection(_population, _problem.Parameters.SelectionInfo);
                    Chromosome father = Selection.PerformSelection(_population, _problem.Parameters.SelectionInfo);

                    // combine
                    Chromosome child = Crossover.PerformCrossover(mother, father, _problem.Parameters);

                    // mutate
                    Mutation.PerformMutation(child, _problem.Parameters);

                    child.ComputeFitness(_problem);
                    child.Age = Settings.lf + _rand.Next(Settings.lf);

                    newPopulation[i] = child;
                }

                List<Chromosome> mergedPop = new List<Chromosome>();

                for (int i = 0; i < _population.Length; i++)
                {
                    if (_population[i].Age > 0)
                        mergedPop.Add(_population[i]);
                }

                for (int i = 0; i < newPopulation.Length; i++)
                {
                    if (newPopulation[i].Age > 0)
                        mergedPop.Add(newPopulation[i]);
                }

                for (int i = 0; i < _problem.Parameters.PopulationSize * Settings.f; i++) // indivizi noi
                {
                    Chromosome c = new Chromosome(_problem.Parameters, true)
                    {
                        Age = Settings.lf1 + _rand.Next(Settings.lf1)
                    };
                    mergedPop.Add(c);
                }

                _population = new Chromosome[mergedPop.Count];
                for (int i = 0; i < _population.Length; i++)
                    _population[i] = mergedPop[i];

                generations++;
                Console.Write("Generation {0} / {1}: ", generations, _problem.Parameters.StoppingInfo.MaxGenerations);
                _solution = Selection.GetBest(_population);
                Console.WriteLine("{0:F4} {1}", _solution.Fitness, _population.Length);

                if (generations > _problem.Parameters.StoppingInfo.MaxGenerations)
                {
                    //_solution = Selection.GetBest(_population);
                    searchOver = true;
                }
            }
        }

        private Chromosome PerformHC(Chromosome c)
        {
            EvolutionaryHillClimbing ehc = new EvolutionaryHillClimbing();
            ehc.SetInitialValue(c);
            ehc.Solve(_problem);
            return ehc.Solution;
        }
    }
}