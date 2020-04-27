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
    public enum EncodingType { Binary, RealValued, Permutation };

    public enum CrossoverType { OnePoint, MultiplePoint, Uniform, Arithmetic, ArithmeticInteger };

    public enum MutationType { Resetting, Gaussian, Exchange };

    public enum SelectionType { RouletteWheel, Rank, Tournament };

    public enum StoppingType { Generations, Convergence };

    public class EncodingInformation
    {
        private int _noGenes;
        private EncodingType _encoding;
        private int[] _noBits = null; // binary
        private double[] _minValues = null, _maxValues = null; // real valued
        private int[] _noValues = null; // permutation

        #region Properties

        public int NoGenes
        {
            get { return _noGenes; }
            //set { _noGenes = value; }
        }

        public EncodingType Encoding
        {
            get { return _encoding; }
            //set { _encoding = value; }
        }

        public int[] NoBits
        {
            get { return _noBits; }
            //set { _noBits = value; }
        }

        public double[] MinValues
        {
            get { return _minValues; }
            //set { _minValues = value; }
        }

        public double[] MaxValues
        {
            get { return _maxValues; }
            //set { _maxValues = value; }
        }

        public int[] NoValues
        {
            get { return _noValues; }
            //set { _noValues = value; }
        }

        #endregion Properties

        public EncodingInformation(int noGenes, EncodingType encodingType)
        {
            _noGenes = noGenes;
            _encoding = encodingType;

            switch (encodingType) // only one set will be used at a time...
            {
                case EncodingType.Binary:
                    _noBits = new int[noGenes];
                    break;

                case EncodingType.RealValued:
                    _minValues = new double[noGenes];
                    _maxValues = new double[noGenes];
                    break;

                case EncodingType.Permutation:
                    _noValues = new int[noGenes];
                    break;
            }
        }
    }

    public class SelectionInformation
    {
        public SelectionType Type;
        public int Elitism;
        public int TournamentSize;
        public double AlphaRank, BetaRank;
    }

    public class CrossoverInformation
    {
        public CrossoverType Type;
        public double Probability;
        public int NoPoints;
    }

    public class MutationInformation
    {
        public MutationType Type;
        public double Probability;
        public double StandardDeviation;
    }

    public class StoppingInformation
    {
        public StoppingType Type;
        public int MaxGenerations;
        public double DiversityFactor; // ratio between best fitness and average fitness
    }

    public class Parameters
    {
        private int _populationSize;
        private EncodingInformation _encodingInfo;
        private SelectionInformation _selectionInfo;
        private CrossoverInformation _crossoverInfo;
        private MutationInformation _mutationInfo;
        private StoppingInformation _stoppingInfo;

        #region Properties

        public int PopulationSize
        {
            get { return _populationSize; }
            set { _populationSize = value; }
        }

        public EncodingInformation EncodingInfo
        {
            get { return _encodingInfo; }
            set { _encodingInfo = value; }
        }

        public SelectionInformation SelectionInfo
        {
            get { return _selectionInfo; }
            set { _selectionInfo = value; }
        }

        public CrossoverInformation CrossoverInfo
        {
            get { return _crossoverInfo; }
            set { _crossoverInfo = value; }
        }

        public MutationInformation MutationInfo
        {
            get { return _mutationInfo; }
            set { _mutationInfo = value; }
        }

        public StoppingInformation StoppingInfo
        {
            get { return _stoppingInfo; }
            set { _stoppingInfo = value; }
        }

        #endregion Properties

        public Parameters(int popSize, EncodingInformation encoding, SelectionInformation selection,
            CrossoverInformation crossover, MutationInformation mutation, StoppingInformation stopping)
        {
            _populationSize = popSize;
            _encodingInfo = encoding;
            _selectionInfo = selection;
            _crossoverInfo = crossover;
            _mutationInfo = mutation;
            _stoppingInfo = stopping;
        }
    }
}