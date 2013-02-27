using System;

namespace GA {

    [Serializable]
    public class NoScaling: IScalingOperator {

        public void Scale(Population population) {
            for (int i = 0; i < population.Size; i++) {
                population.Individuals[i].Fitness =
                    population.Individuals[i].Objective;
            }
        }
    }

    [Serializable]
    public class LinearScaling: IScalingOperator {

        private double k;
        private bool truncate;

        public LinearScaling(double k, bool truncate) {
            this.k = k;
            this.truncate = truncate;
        }

        public void Scale(Population population) {

            Individual[] individuals = population.Individuals;
            double mean = population.ObjectiveMean;
            double min  = population.ObjectiveMin;
            double max  = population.ObjectiveMax;

            double slope = 1.0;

            if (population.GA.Maximize) {
                if (max > mean)
                    slope = (k * mean - mean) / (max - mean);
            } else {
                if (mean > min)
                    slope = (1/k * mean - mean) / (min - mean);
            }

            if (truncate) {

                for (int i = 0; i < population.Size; i++) {
                    individuals[i].Fitness =
                        slope * (individuals[i].Objective - mean) + mean;
                    if (individuals[i].Fitness < 0.0)
                        individuals[i].Fitness = 0.0;
                }

            } else {

                for (int i = 0; i < population.Size; i++) {
                    individuals[i].Fitness =
                        slope * (individuals[i].Objective - mean) + mean;
                }
            }
        }
    }
}
