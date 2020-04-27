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
    public class Settings
    {
        public static int minval = 1;
        public static int maxval = 15;
        public static int kmax = maxval;
        public static int s0 = 50;
        public static int ngen = 1000;
        public static double f = 0.25;
        public static int lf = 4;
        public static int lf1 = 3;
        public static int nn = 20;
        public static int ni = 20;
        public static double sigma = 2;
        public static double pmg = 0.25;
        public static double pmr = 0.25;
        public static double pmp = 0.5;
        public static double pm = 0.2;
        public static double alpha = 0.5;
    }
}