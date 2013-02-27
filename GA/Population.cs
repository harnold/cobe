using System;

namespace GA {

    public interface IScalingOperator {
        void Scale(Population population);
    }

    public interface ISelectionOperator {
        void Update(Population population);
        Individual Select();
    }

    [Serializable]
    public class Population {

        private IScalingOperator scalingOperator;
        private ISelectionOperator selectionOperator;
        private IMutationOperator mutationOperator;
        private ICrossoverOperator crossoverOperator;
        private IGenomeEvaluator genomeEvaluator;

        private Individual[] individuals;

        private GeneticAlgorithm ga;

        private double genomeDiversity;

        private double fitnessSum;
        private double fitnessMean;
        private double fitnessMax;
        private double fitnessMin;
        private double fitnessDeviation;
        private double fitnessVariance;

        private double objectiveSum;
        private double objectiveMean;
        private double objectiveMax;
        private double objectiveMin;
        private double objectiveDeviation;
        private double objectiveVariance;

        public double GenomeDiversity {
            get { return genomeDiversity; }
        }

        public double FitnessSum {
            get { return fitnessSum; }
        }

        public double FitnessMean {
            get { return fitnessMean; }
        }

        public double FitnessMax {
            get { return fitnessMax; }
        }

        public double FitnessMin {
            get { return fitnessMin; }
        }

        public double FitnessDeviation {
            get { return fitnessDeviation; }
        }

        public double FitnessVariance {
            get { return fitnessVariance; }
        }

        public double ObjectiveSum {
            get { return objectiveSum; }
        }

        public double ObjectiveMean {
            get { return objectiveMean; }
        }

        public double ObjectiveMax {
            get { return objectiveMax; }
        }

        public double ObjectiveMin {
            get { return objectiveMin; }
        }

        public double ObjectiveDeviation {
            get { return objectiveDeviation; }
        }

        public double ObjectiveVariance {
            get { return objectiveVariance; }
        }

        public Individual[] Individuals {
            get { return individuals; }
            set { individuals = value; }
        }

        public GeneticAlgorithm GA {
            get { return ga; }
            set { ga = value; }
        }

        public int Size {
            get { return individuals.Length; }
        }

        public IMutationOperator MutationOperator {
            get { return mutationOperator; }
            set { mutationOperator = value; }
        }

        public ICrossoverOperator CrossoverOperator {
            get { return crossoverOperator; }
            set { crossoverOperator = value; }
        }

        public IScalingOperator ScalingOperator {
            get { return scalingOperator; }
            set { scalingOperator = value; }
        }

        public ISelectionOperator SelectionOperator {
            get { return selectionOperator; }
            set { selectionOperator = value; }
        }

        public IGenomeEvaluator GenomeEvaluator {
            get { return genomeEvaluator; }
            set { genomeEvaluator = value; }
        }

        public Population(int populationSize) {
            this.individuals = new Individual[populationSize];
        }

        public virtual void Initialize() {

            foreach (Individual i in Individuals)
                i.Genome.Initialize();
        }

        public virtual void Evaluate() {

            for (int i = 0; i < Size; i++) {
                Individuals[i].Objective =
                    GenomeEvaluator.Evaluate(Individuals[i].Genome);
            }
        }

        public virtual void Scale() {
            ScalingOperator.Scale(this);
        }

        public virtual Individual Select() {
            return SelectionOperator.Select();
        }

        public virtual void Cross(IGenome genome1, IGenome genome2) {
            CrossoverOperator.Cross(genome1, genome2);
        }

        public virtual void Mutate(IGenome genome, double pMutation) {
            MutationOperator.Mutate(genome, pMutation);
        }

        public virtual void Sort() {
            Array.Sort(Individuals);
        }

        public virtual void ComputeObjectiveStatistics() {

            objectiveSum = 0.0;
            objectiveMax = Double.NegativeInfinity;
            objectiveMin = Double.PositiveInfinity;

            foreach (Individual i in Individuals) {

                objectiveSum += i.Objective;

                if (i.Objective > objectiveMax)
                    objectiveMax = i.Objective;

                if (i.Objective < objectiveMin)
                    objectiveMin = i.Objective;
            }

            objectiveMean = objectiveSum / Size;

            double objectiveDevSum = 0.0;

            foreach (Individual i in Individuals) {
                double objectiveDiff = i.Objective - objectiveMean;
                objectiveDevSum += objectiveDiff * objectiveDiff;
            }

            objectiveVariance = objectiveDevSum / (Size);
            objectiveDeviation = Math.Sqrt(objectiveVariance);
        }

        public virtual void ComputeFitnessStatistics() {

            fitnessSum = 0.0;
            fitnessMax = Double.NegativeInfinity;
            fitnessMin = Double.PositiveInfinity;

            foreach (Individual i in Individuals) {

                fitnessSum += i.Fitness;

                if (i.Fitness > fitnessMax)
                    fitnessMax = i.Fitness;

                if (i.Fitness < fitnessMin)
                    fitnessMin = i.Fitness;
            }

            fitnessMean = fitnessSum / Size;

            double fitnessDevSum   = 0.0;

            foreach (Individual i in Individuals) {
                double fitnessDiff = i.Fitness - fitnessMean;
                fitnessDevSum += fitnessDiff * fitnessDiff;
            }

            fitnessVariance = fitnessDevSum / (Size);
            fitnessDeviation = Math.Sqrt(fitnessVariance);
        }

        public virtual void ComputeGenomeDiversity() {

            double[,] diff = new double[Size, Size];

            for (int i = 0; i < Size; i++) {
                for (int j = i + 1; j < Size; j++) {
                    diff[i, j] = Individuals[i].Genome.DifferenceTo(
                                 Individuals[j].Genome);
                }
            }

            double divSum = 0.0;

            for (int i = 0; i < Size; i++) {
                for (int j = i + 1; j < Size; j++) {
                    divSum += diff[i, j];
                }
            }

            genomeDiversity = 2 * divSum / (Size * (Size - 1));
        }
    }
}
