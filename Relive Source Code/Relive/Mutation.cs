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
    public class Mutation
    {
        private static MutationInformation _info;

        public static void PerformMutation(Chromosome child, Parameters parameters)
        {
            _info = parameters.MutationInfo;

            switch (_info.Type)
            {
                case MutationType.Resetting:
                    if (parameters.EncodingInfo.Encoding == EncodingType.Permutation)
                        throw new Exception("Use resetting mutation for binary and permutation encoding");
                    if (parameters.EncodingInfo.Encoding == EncodingType.Binary)
                        ResetBinary(child);
                    else // if (parameters.EncodingInfo.Encoding == EncodingType.Permutation)
                        ResetReal(child, parameters);
                    break;

                case MutationType.Gaussian:
                    if (parameters.EncodingInfo.Encoding != EncodingType.RealValued)
                        throw new Exception("Gaussian mutation implemented only for real valued encoding");
                    GaussianReal(child, parameters);
                    break;

                case MutationType.Exchange:
                    if (parameters.EncodingInfo.Encoding != EncodingType.Permutation)
                        throw new Exception("Exchange mutation implemented only for permutation encoding");
                    ExchangePermutation(child);
                    break;
            }
        }

        private static void ExchangePermutation(Chromosome child)
        {
            // implemented for permutation encoding

            for (int i = 0; i < child.NoGenes; i++)
            {
                int len = child.PermutationGenes[i].Length;

                if (ExtendedRandom.NextUniform() < _info.Probability)
                {
                    // generate exchange points
                    int p1 = (int)(ExtendedRandom.NextUniform() * len);
                    int p2 = p1;

                    while (p2 == p1)
                        p2 = (int)(ExtendedRandom.NextUniform() * len);

                    int auxi = child.PermutationGenes[i][p1];
                    child.PermutationGenes[i][p1] = child.PermutationGenes[i][p2];
                    child.PermutationGenes[i][p2] = auxi;
                }
            }
        }

        private static void GaussianReal(Chromosome child, Parameters parameters)
        {
            // implemented for real valued encoding

            for (int i = 0; i < child.NoGenes; i++)
            {
                if (ExtendedRandom.NextUniform() < _info.Probability)
                {
                    child.RealGenes[i] = ExtendedRandom.NextNormal(child.RealGenes[i], _info.StandardDeviation);
                    if (child.RealGenes[i] < parameters.EncodingInfo.MinValues[i])
                        child.RealGenes[i] = parameters.EncodingInfo.MinValues[i];
                    if (child.RealGenes[i] > parameters.EncodingInfo.MaxValues[i])
                        child.RealGenes[i] = parameters.EncodingInfo.MaxValues[i];
                }
            }
        }

        private static void ResetReal(Chromosome child, Parameters parameters)
        {
            // implemented for real valued encoding

            for (int i = 0; i < child.NoGenes; i++)
            {
                if (ExtendedRandom.NextUniform() < _info.Probability)
                {
                    child.RealGenes[i] = parameters.EncodingInfo.MinValues[i] + ExtendedRandom.NextUniform() *
                        (parameters.EncodingInfo.MaxValues[i] - parameters.EncodingInfo.MinValues[i]);
                }
            }
        }

        private static void ResetBinary(Chromosome child)
        {
            // implemented for binary encoding

            for (int i = 0; i < child.NoGenes; i++)
            {
                string newGene = "";
                for (int j = 0; j < child.BinaryGenes[i].Length; j++)
                {
                    if (ExtendedRandom.NextUniform() < _info.Probability)
                    {
                        if (child.BinaryGenes[i][j] == '0')
                            newGene += "1";
                        else
                            newGene += "0";
                    }
                    else
                        newGene += child.BinaryGenes[i][j];
                }
                child.BinaryGenes[i] = newGene;
            }
        }
    }
}