using System;

namespace GA {

    [Serializable]
    public class Random {

        private static System.Random rng = new System.Random();

        private static bool gaussianCached = false;
        private static double gaussianValue;

        public static void Seed(int seed) {
            rng = new System.Random(seed);
        }

        public static int Next() {
            return rng.Next();
        }

        public static int Next(int maxValue) {
            return rng.Next(maxValue);
        }

        public static int Next(int minValue, int maxValue) {
            return rng.Next(minValue, maxValue);
        }

        public static double NextDouble() {
            return rng.NextDouble();
        }

        public static double NextGaussian() {

            double var1;
            double var2;
            double rsquare;

            if (gaussianCached) {
                gaussianCached = false;
                return gaussianValue;
            }

            do {
                var1 = 2.0 * NextDouble() - 1.0;
                var2 = 2.0 * NextDouble() - 1.0;
                rsquare = var1 * var1 + var2 * var2;

            } while (rsquare >= 1.0 || rsquare == 0.0);

            double val = -2.0 * Math.Log(rsquare) / rsquare;
            double factor = (val > 0.0) ? Math.Sqrt(val) : 0.0;

            gaussianValue  = var1 * factor;
            gaussianCached = true;

            return (var2 * factor);
        }
    }
}
