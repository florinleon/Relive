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
    public class Sorting
    {
        public static int[] ShellSort(int[] vector, bool ascending)
        {
            int dist = 0, i = 0, j = 0;
            int aux;
            int count = vector.Length;

            int[] nvect = new int[count];
            for (i = 0; i < count; i++)
                nvect[i] = vector[i];

            if (ascending)
            {
                for (dist = count / 2; dist > 0; dist /= 2)
                    for (i = dist; i < count; i++)
                        for (j = i - dist; j >= 0 && nvect[j] > nvect[j + dist]; j -= dist)
                        {
                            aux = nvect[j];
                            nvect[j] = nvect[j + dist];
                            nvect[j + dist] = aux;
                        }
            }
            else
            {
                for (dist = count / 2; dist > 0; dist /= 2)
                    for (i = dist; i < count; i++)
                        for (j = i - dist; j >= 0 && nvect[j] < nvect[j + dist]; j -= dist)
                        {
                            aux = nvect[j];
                            nvect[j] = nvect[j + dist];
                            nvect[j + dist] = aux;
                        }
            }

            return nvect;
        }

        public static double[] ShellSort(double[] vector, bool ascending)
        {
            int dist = 0, i = 0, j = 0;
            double aux;
            int count = vector.Length;

            double[] nvect = new double[count];
            for (i = 0; i < count; i++)
                nvect[i] = vector[i];

            if (ascending)
            {
                for (dist = count / 2; dist > 0; dist /= 2)
                    for (i = dist; i < count; i++)
                        for (j = i - dist; j >= 0 && nvect[j] > nvect[j + dist]; j -= dist)
                        {
                            aux = nvect[j];
                            nvect[j] = nvect[j + dist];
                            nvect[j + dist] = aux;
                        }
            }
            else
            {
                for (dist = count / 2; dist > 0; dist /= 2)
                    for (i = dist; i < count; i++)
                        for (j = i - dist; j >= 0 && nvect[j] < nvect[j + dist]; j -= dist)
                        {
                            aux = nvect[j];
                            nvect[j] = nvect[j + dist];
                            nvect[j + dist] = aux;
                        }
            }

            return nvect;
        }

        public static string[] ShellSort(string[] vector, bool ascending)
        {
            int dist = 0, i = 0, j = 0;
            string aux;
            int count = vector.Length;

            string[] nvect = new string[count];
            for (i = 0; i < count; i++)
                nvect[i] = vector[i];

            if (ascending)
            {
                for (dist = count / 2; dist > 0; dist /= 2)
                    for (i = dist; i < count; i++)
                        for (j = i - dist; j >= 0 && nvect[j].CompareTo(nvect[j + dist]) > 0; j -= dist)
                        {
                            aux = nvect[j];
                            nvect[j] = nvect[j + dist];
                            nvect[j + dist] = aux;
                        }
            }
            else
            {
                for (dist = count / 2; dist > 0; dist /= 2)
                    for (i = dist; i < count; i++)
                        for (j = i - dist; j >= 0 && nvect[j].CompareTo(nvect[j + dist]) < 0; j -= dist)
                        {
                            aux = nvect[j];
                            nvect[j] = nvect[j + dist];
                            nvect[j + dist] = aux;
                        }
            }

            return nvect;
        }

        public static int[] RandomRankVector(int size)
        {
            int[] nrand = new int[size];
            int[] no = new int[size];

            Random r = new Random();
            int i = 0;

            for (i = 0; i < size; i++)
            {
                nrand[i] = r.Next(10000);
                no[i] = i;
            }

            int dist = 0, j = 0, auxi = 0;

            for (dist = size / 2; dist > 0; dist /= 2)
                for (i = dist; i < size; i++)
                    for (j = i - dist; j >= 0 && nrand[j] < nrand[j + dist]; j -= dist)
                    {
                        auxi = no[j]; no[j] = no[j + dist]; no[j + dist] = auxi;
                        auxi = nrand[j]; nrand[j] = nrand[j + dist]; nrand[j + dist] = auxi;
                    }

            return no;
        }

        public static int[] RankVector(int[] vector, bool ascending)
        {
            int size = vector.Length;

            int[] no = new int[size];
            int[] tvect = new int[size];
            int i = 0;

            for (i = 0; i < size; i++)
            {
                no[i] = i;
                tvect[i] = vector[i];
            }

            int dist = 0, j = 0, auxi = 0;

            if (ascending)
            {
                for (dist = size / 2; dist > 0; dist /= 2)
                    for (i = dist; i < size; i++)
                        for (j = i - dist; j >= 0 && tvect[j] > tvect[j + dist]; j -= dist)
                        {
                            auxi = no[j]; no[j] = no[j + dist]; no[j + dist] = auxi;
                            auxi = tvect[j]; tvect[j] = tvect[j + dist]; tvect[j + dist] = auxi;
                        }
            }
            else
            {
                for (dist = size / 2; dist > 0; dist /= 2)
                    for (i = dist; i < size; i++)
                        for (j = i - dist; j >= 0 && tvect[j] < tvect[j + dist]; j -= dist)
                        {
                            auxi = no[j]; no[j] = no[j + dist]; no[j + dist] = auxi;
                            auxi = tvect[j]; tvect[j] = tvect[j + dist]; tvect[j + dist] = auxi;
                        }
            }

            int[] ranks = new int[size];
            for (i = 0; i < size; i++)
                ranks[no[i]] = i;

            return ranks;
        }

        public static int[] RankVector(double[] vector, bool ascending)
        {
            int size = vector.Length;

            int[] no = new int[size];
            double[] tvect = new double[size];
            int i = 0;

            for (i = 0; i < size; i++)
            {
                no[i] = i;
                tvect[i] = vector[i];
            }

            int dist = 0, j = 0, auxi = 0;
            double auxd = 0;

            if (ascending)
            {
                for (dist = size / 2; dist > 0; dist /= 2)
                    for (i = dist; i < size; i++)
                        for (j = i - dist; j >= 0 && tvect[j] > tvect[j + dist]; j -= dist)
                        {
                            auxi = no[j]; no[j] = no[j + dist]; no[j + dist] = auxi;
                            auxd = tvect[j]; tvect[j] = tvect[j + dist]; tvect[j + dist] = auxd;
                        }
            }
            else
            {
                for (dist = size / 2; dist > 0; dist /= 2)
                    for (i = dist; i < size; i++)
                        for (j = i - dist; j >= 0 && tvect[j] < tvect[j + dist]; j -= dist)
                        {
                            auxi = no[j]; no[j] = no[j + dist]; no[j + dist] = auxi;
                            auxd = tvect[j]; tvect[j] = tvect[j + dist]; tvect[j + dist] = auxd;
                        }
            }

            int[] ranks = new int[size];
            for (i = 0; i < size; i++)
                ranks[no[i]] = i;

            return ranks;
        }

        public static int[] RankVector(string[] vector, bool ascending)
        {
            int size = vector.Length;

            int[] no = new int[size];
            string[] tvect = new string[size];
            int i = 0;

            for (i = 0; i < size; i++)
            {
                no[i] = i;
                tvect[i] = vector[i];
            }

            int dist = 0, j = 0, auxi = 0;
            string auxs;

            if (ascending)
            {
                for (dist = size / 2; dist > 0; dist /= 2)
                    for (i = dist; i < size; i++)
                        for (j = i - dist; j >= 0 && tvect[j].CompareTo(tvect[j + dist]) > 0; j -= dist)
                        {
                            auxi = no[j]; no[j] = no[j + dist]; no[j + dist] = auxi;
                            auxs = tvect[j]; tvect[j] = tvect[j + dist]; tvect[j + dist] = auxs;
                        }
            }
            else
            {
                for (dist = size / 2; dist > 0; dist /= 2)
                    for (i = dist; i < size; i++)
                        for (j = i - dist; j >= 0 && tvect[j].CompareTo(tvect[j + dist]) < 0; j -= dist)
                        {
                            auxi = no[j]; no[j] = no[j + dist]; no[j + dist] = auxi;
                            auxs = tvect[j]; tvect[j] = tvect[j + dist]; tvect[j + dist] = auxs;
                        }
            }

            int[] ranks = new int[size];
            for (i = 0; i < size; i++)
                ranks[no[i]] = i;

            return ranks;
        }
    }
}