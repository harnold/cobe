using System;

namespace GA {

    [Serializable]
    public class UniformSelection: ISelectionOperator {

        protected Population population;

        public virtual void Update(Population population) {
            this.population = population;
        }

        public virtual Individual Select() {
            int selectedIndex = Random.Next(0, population.Size);
            return population.Individuals[selectedIndex];
        }
    }

    [Serializable]
    public class RandomTournamentSelection: UniformSelection {

        public override Individual Select() {

            Individual i1 = base.Select();
            Individual i2 = base.Select();

            if (population.GA.Maximize)
                return (i1.Fitness > i2.Fitness) ? i1 : i2;
            else
                return (i1.Fitness < i2.Fitness) ? i1 : I2;
        }
    }

    [Serializable]
    public class RouletteWheelSelection: ISelectionOperator {

        protected Population population;
        protected double[] partialSums;

        public virtual void Update(Population population) {

            this.population = population;

            if (partialSums == null || population.Size != partialSums.Length)
                partialSums = new double[population.Size];

            if (population.GA.Maximize) {

                partialSums[0] =
                    population.Individuals[0].Fitness / population.FitnessSum;

                for (int i = 1; i < population.Size; i++) {
                    partialSums[i] = partialSums[i-1]
                        + (population.Individuals[i].Fitness
                           / population.FitnessSum);
                }

            } else {

                double inverseFitnessSum = 0.0;

                for (int i = 0; i < population.Size; i++)
                    inverseFitnessSum += 1.0 / population.Individuals[i].Fitness;

                partialSums[0] =
                    1.0 / (population.Individuals[0].Fitness * inverseFitnessSum);

                for (int i = 1; i < population.Size; i++) {
                    partialSums[i] = partialSums[i-1]
                        + (1.0 / (population.Individuals[i].Fitness
                                  * inverseFitnessSum));
                }
            }
        }

        public virtual Individual Select() {

            int i;
            int lower = 0;
            int upper = population.Size - 1;

            double cutoff = Random.NextDouble();

            while (upper >= lower) {

                i = lower + (upper - lower) / 2;

                if (partialSums[i] < cutoff)
                    lower = i + 1;
                else
                    upper = i - 1;
            }

            lower = Math.Min(population.Size - 1, lower);
            lower = Math.Max(0, lower);

            return population.Individuals[lower];
        }
    }

    [Serializable]
    public class TournamentSelection: RouletteWheelSelection {

        public override Individual Select() {

            Individual i1 = base.Select();
            Individual i2 = base.Select();

            if (population.GA.Maximize)
                return (i1.Fitness > i2.Fitness) ? i1 : i2;
            else
                return (i1.Fitness < i2.Fitness) ? i1 : i2;
        }
    }
}
