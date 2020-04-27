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
    public class Crossover
    {
        private static CrossoverInformation _info;

        public static Chromosome PerformCrossover(Chromosome mother, Chromosome father, Parameters parameters)
        {
            _info = parameters.CrossoverInfo;

            switch (_info.Type)
            {
                case CrossoverType.OnePoint:
                    if (parameters.EncodingInfo.Encoding == EncodingType.RealValued)
                        throw new Exception("Use arithmetic crossover for real valued encoding");
                    if (parameters.EncodingInfo.Encoding == EncodingType.Binary)
                        return OnePointBinary(mother, father);
                    else // if (parameters.EncodingInfo.Encoding == EncodingType.Permutation)
                        return OnePointPermutation(mother, father);

                case CrossoverType.MultiplePoint:
                    if (parameters.EncodingInfo.Encoding != EncodingType.Binary)
                        throw new Exception("Multiple point crossover implemented only for binary encoding");
                    return MultiplePoint(mother, father);

                case CrossoverType.Uniform:
                    if (parameters.EncodingInfo.Encoding != EncodingType.Binary)
                        throw new Exception("Uniform crossover implemented only for binary encoding");
                    return MultiplePoint(mother, father);

                case CrossoverType.Arithmetic:
                    if (parameters.EncodingInfo.Encoding != EncodingType.RealValued)
                        throw new Exception("Arithmetic crossover implemented only for real valued encoding");
                    return Arithmetic(mother, father);

                case CrossoverType.ArithmeticInteger:
                    if (parameters.EncodingInfo.Encoding != EncodingType.RealValued)
                        throw new Exception("Arithmetic crossover implemented only for real valued encoding");
                    return ArithmeticInteger(mother, father);
            }

            return null;
        }

        private static Chromosome OnePointBinary(Chromosome mother, Chromosome father)
        {
            // implemented for binary encoding

            Chromosome child = new Chromosome(mother); // for array allocation

            for (int i = 0; i < child.NoGenes; i++)
            {
                int len = child.BinaryGenes[i].Length;
                int pivot = (int)(ExtendedRandom.NextUniform() * len);

                if (ExtendedRandom.NextUniform() < _info.Probability)
                {
                    child.BinaryGenes[i] = mother.BinaryGenes[i].Substring(0, pivot) + father.BinaryGenes[i].Substring(pivot);
                }
                else // no crossover, copy one parent
                {
                    if (ExtendedRandom.NextUniform() < 0.5)
                        child.BinaryGenes[i] = mother.BinaryGenes[i];
                    else
                        child.BinaryGenes[i] = father.BinaryGenes[i];
                }
            }

            return child;
        }

        private static Chromosome OnePointPermutation(Chromosome mother, Chromosome father)
        {
            // implemented for permutation encoding

            Chromosome child = new Chromosome(mother); // for array allocation

            for (int i = 0; i < child.NoGenes; i++)
            {
                int len = child.PermutationGenes[i].Length;
                int pivot = (int)(ExtendedRandom.NextUniform() * len);

                if (ExtendedRandom.NextUniform() < _info.Probability)
                {
                    List<int> values = new List<int>();
                    int j = 0;
                    for (; j < pivot; j++)
                    {
                        // child.PermutationGenes[i][j] = mother.PermutationGenes[i][j];
                        values.Add(mother.PermutationGenes[i][j]);
                    }

                    // add all the values in the father, in order, except those already added from the mother
                    int k = 0;
                    while (j < len)
                    {
                        if (values.Contains(father.PermutationGenes[i][k]))
                            k++;
                        else
                        {
                            child.PermutationGenes[i][j] = father.PermutationGenes[i][k];
                            j++; k++;
                        }
                    }
                }
                else  // no crossover, copy one parent
                {
                    if (ExtendedRandom.NextUniform() < 0.5)
                        for (int j = 0; j < len; j++)
                            child.PermutationGenes[i][j] = mother.PermutationGenes[i][j];
                    else
                        for (int j = 0; j < len; j++)
                            child.PermutationGenes[i][j] = father.PermutationGenes[i][j];
                }
            }

            return child;
        }

        private static Chromosome MultiplePoint(Chromosome mother, Chromosome father)
        {
            // implemented for binary encoding

            Chromosome child = new Chromosome(mother); // for array allocation

            for (int i = 0; i < child.NoGenes; i++)
            {
                int len = child.BinaryGenes[i].Length;

                if (ExtendedRandom.NextUniform() < _info.Probability)
                {
                    // generate crossover points
                    bool[] selected = new bool[len];
                    for (int j = 0; j < len; j++)
                        selected[j] = false;

                    for (int j = 0; j < _info.NoPoints; j++)
                    {
                        bool ok = false;
                        while (!ok)
                        {
                            int crtSel = (int)(ExtendedRandom.NextUniform() * len);
                            if (!selected[crtSel])
                            {
                                selected[crtSel] = true;
                                ok = true;
                            }
                        }
                    }

                    bool selMother = true; // switch between mother genes and father genes

                    string newGene = "";
                    for (int j = 0; j < len; j++)
                    {
                        if (selected[j])
                            selMother = !selMother;

                        if (selMother)
                            newGene += mother.BinaryGenes[i][j];
                        else
                            newGene += father.BinaryGenes[i][j];
                    }
                    child.BinaryGenes[i] = newGene;
                }
                else // no crossover, copy one parent
                {
                    if (ExtendedRandom.NextUniform() < 0.5)
                        child.BinaryGenes[i] = mother.BinaryGenes[i];
                    else
                        child.BinaryGenes[i] = father.BinaryGenes[i];
                }
            }

            return child;
        }

        private static Chromosome Uniform(Chromosome mother, Chromosome father)
        {
            // implemented for binary encoding

            Chromosome child = new Chromosome(mother); // for array allocation

            for (int i = 0; i < child.NoGenes; i++)
            {
                int len = child.BinaryGenes[i].Length;
                int pivot = (int)(ExtendedRandom.NextUniform() * len);

                if (ExtendedRandom.NextUniform() < _info.Probability)
                {
                    string newGene = "";
                    for (int j = 0; j < len; j++)
                    {
                        if (ExtendedRandom.NextUniform() < 0.5)
                            newGene += mother.BinaryGenes[i][j];
                        else
                            newGene += father.BinaryGenes[i][j];
                    }
                    child.BinaryGenes[i] = newGene;
                }
                else // no crossover, copy one parent
                {
                    if (ExtendedRandom.NextUniform() < 0.5)
                        child.BinaryGenes[i] = mother.BinaryGenes[i];
                    else
                        child.BinaryGenes[i] = father.BinaryGenes[i];
                }
            }

            return child;
        }

        private static Chromosome Arithmetic(Chromosome mother, Chromosome father)
        {
            Chromosome child = new Chromosome(mother);

            if (ExtendedRandom.NextUniform() < _info.Probability)
            {
                for (int i = 0; i < child.NoGenes; i++)
                {
                    double point = ExtendedRandom.NextUniform();
                    child.RealGenes[i] = point * mother.RealGenes[i] + (1 - point) * father.RealGenes[i];
                }
            }

            return child;
        }

        private static Chromosome ArithmeticInteger(Chromosome mother, Chromosome father)
        {
            Chromosome child = new Chromosome(mother);

            if (ExtendedRandom.NextUniform() < _info.Probability)
            {
                for (int i = 0; i < child.NoGenes; i++)
                {
                    if (ExtendedRandom.NextUniform() < 0.5)
                        child.RealGenes[i] = father.RealGenes[i];
                }
            }

            return child;
        }
    }
}