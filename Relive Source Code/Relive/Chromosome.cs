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
    public class Chromosome
    {
        private int _noGenes;
        private string[] _binaryGenes;
        private double[] _realGenes;
        private int[][] _permutationGenes;
        private double _fitness;

        public int Age;

        #region Properties

        public int NoGenes
        {
            get { return _noGenes; }
            //set { _noGenes = value; }
        }

        public string[] BinaryGenes
        {
            get { return _binaryGenes; }
            set { _binaryGenes = value; }
        }

        public double[] RealGenes
        {
            get { return _realGenes; }
            set { _realGenes = value; }
        }

        public int[][] PermutationGenes
        {
            get { return _permutationGenes; }
            set { _permutationGenes = value; }
        }

        public double Fitness
        {
            get { return _fitness; }
            //set { _fitness = value; }
        }

        #endregion Properties

        public Chromosome(Parameters parameters, bool createRandomValues)
        {
            Age = 0;

            _noGenes = parameters.EncodingInfo.NoGenes;

            _binaryGenes = null;
            _realGenes = null;
            _permutationGenes = null;

            switch (parameters.EncodingInfo.Encoding) // only one set will be used at a time...
            {
                case EncodingType.Binary:
                    _binaryGenes = new string[_noGenes];
                    if (createRandomValues)
                    {
                        for (int i = 0; i < _noGenes; i++)
                        {
                            _binaryGenes[i] = "";
                            for (int j = 0; j < parameters.EncodingInfo.NoBits[i]; j++)
                            {
                                if (ExtendedRandom.NextUniform() < 0.5)
                                    _binaryGenes[i] += "0";
                                else
                                    _binaryGenes[i] += "1";
                            }
                        }
                    }
                    break;

                case EncodingType.RealValued:
                    _realGenes = new double[_noGenes];
                    if (createRandomValues)
                    {
                        for (int i = 0; i < _noGenes; i++)
                        {
                            _realGenes[i] = parameters.EncodingInfo.MinValues[i] + ExtendedRandom.NextUniform() *
                                (parameters.EncodingInfo.MaxValues[i] - parameters.EncodingInfo.MinValues[i]);
                        }
                    }
                    break;

                case EncodingType.Permutation:
                    _permutationGenes = new int[_noGenes][];
                    if (createRandomValues)
                    {
                        for (int i = 0; i < _noGenes; i++)
                        {
                            _permutationGenes[i] = GeneratePermutation(parameters.EncodingInfo.NoValues[i]);
                        }
                    }
                    break;
            }
        }

        public Chromosome(Chromosome c)
        {
            Age = c.Age;

            _noGenes = c._noGenes;
            _fitness = c._fitness;

            // binary
            if (c._binaryGenes == null)
                _binaryGenes = null;
            else
            {
                _binaryGenes = new string[c._binaryGenes.Length];
                for (int i = 0; i < c._binaryGenes.Length; i++)
                    _binaryGenes[i] = c._binaryGenes[i];
            }

            // real-valued
            if (c._realGenes == null)
                _realGenes = null;
            else
            {
                _realGenes = new double[c._realGenes.Length];
                for (int i = 0; i < c._realGenes.Length; i++)
                    _realGenes[i] = c._realGenes[i];
            }

            // permutation
            if (c._permutationGenes == null)
                _permutationGenes = null;
            else
            {
                _permutationGenes = new int[c._permutationGenes.Length][];
                for (int i = 0; i < c._permutationGenes.Length; i++)
                {
                    if (c._permutationGenes[i] == null)
                        _permutationGenes[i] = null;
                    else
                    {
                        _permutationGenes[i] = new int[c._permutationGenes[i].Length];
                        for (int j = 0; j < c._permutationGenes[i].Length; j++)
                            _permutationGenes[i][j] = c._permutationGenes[i][j];
                    }
                }
            }
        }

        public void ComputeFitness(Problem problem)
        {
            _fitness = problem.FitnessFunction(this);
        }

        private int[] GeneratePermutation(int size)
        {
            double[] nrand = new double[size];
            int[] no = new int[size];

            int i = 0;

            for (i = 0; i < size; i++)
            {
                nrand[i] = ExtendedRandom.NextUniform();
                no[i] = i;
            }

            int dist = 0, j = 0, auxi = 0;
            double auxd = 0;

            for (dist = size / 2; dist > 0; dist /= 2)
                for (i = dist; i < size; i++)
                    for (j = i - dist; j >= 0 && nrand[j] < nrand[j + dist]; j -= dist)
                    {
                        auxi = no[j]; no[j] = no[j + dist]; no[j + dist] = auxi;
                        auxd = nrand[j]; nrand[j] = nrand[j + dist]; nrand[j + dist] = auxd;
                    }

            return no;
        }
    }
}