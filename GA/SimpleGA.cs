using System;

namespace GA {

    public class SimpleGA: GeneticAlgorithm {

        public delegate void EventHandler(SimpleGA sender);

        public event EventHandler GenerationComputed;

        private Population population;
        private Individual[] matingPool;

        private int numberOfElitists = 1;

        private double pMutation = 0.01;
        private double pCrossover = 0.6;

        private bool computeObjectiveStatistics = true;
        private bool computeFitnessStatistics = false;
        private bool computeGenomeDiversity = false;

        protected Individual[] MatingPool {
            get { return matingPool; }
            set { matingPool = value; }
        }

        public bool ComputeObjectiveStatistics {
            get { return computeObjectiveStatistics; }
            set { computeObjectiveStatistics = value; }
        }

        public bool ComputeFitnessStatistics {
            get { return computeFitnessStatistics; }
            set { computeFitnessStatistics = value; }
        }

        public bool ComputeGenomeDiversity {
            get { return computeGenomeDiversity; }
            set { computeGenomeDiversity = value; }
        }

        public Population Population {
            get { return population; }
            set { population = value; }
        }

        public int NumberOfElitists {
            get { return numberOfElitists; }
            set { numberOfElitists = value; }
        }

        public double PMutation {
            get { return pMutation; }
            set { pMutation = value; }
        }

        public double PCrossover {
            get { return pCrossover; }
            set { pCrossover = value; }
        }

        public SimpleGA(Population population) {

            this.population = population;
            this.matingPool = new Individual[population.Size];

            Initialize();
        }

        public override void Initialize() {

            Population.GA = this;
            Population.Initialize();

            for (int i = 0; i < Population.Size; i++) {
                MatingPool[i].Genome =
                    (IGenome) Population.Individuals[i].Genome.Clone();
            }

            EvaluateGeneration();
        }

        protected virtual void EvaluateGeneration() {

            Population.Evaluate();

            if (ComputeObjectiveStatistics)
                Population.ComputeObjectiveStatistics();

            Population.Scale();
            Population.Sort();

            if (ComputeFitnessStatistics)
                Population.ComputeFitnessStatistics();

            if (ComputeGenomeDiversity)
                Population.ComputeGenomeDiversity();
        }

        public override void ComputeNextGeneration() {

            CopyElitists();

            DoSelection();
            DoCrossover();
            DoMutation();

            Individual[] oldIndividuals = Population.Individuals;
            Population.Individuals = matingPool;
            matingPool = oldIndividuals;

            ++Generation;

            EvaluateGeneration();

            if (GenerationComputed != null)
                GenerationComputed(this);
        }

        protected virtual void CopyElitists() {

            int firstElitist;
            int lastElitist;

            if (Maximize == true) {
                firstElitist = Population.Size - NumberOfElitists;
                lastElitist = firstElitist + NumberOfElitists - 1;
            } else {
                firstElitist = 0;
                lastElitist = NumberOfElitists - 1;
            }

            for (int i = firstElitist, j = 0; i <= lastElitist; i++, j++)
                MatingPool[j].Copy(Population.Individuals[i]);
        }

        protected virtual void DoSelection() {

            Population.SelectionOperator.Update(Population);

            for (int i = NumberOfElitists; i < Population.Size; i++)
                MatingPool[i].Copy(Population.Select());
        }

        protected virtual void DoCrossover() {

            for (int i = NumberOfElitists; i < Population.Size; i++) {

                if (Random.NextDouble() < pCrossover) {

                    int j = Random.Next(NumberOfElitists, Population.Size);

                    IGenome genome1 = MatingPool[i].Genome;
                    IGenome genome2 = MatingPool[j].Genome;

                    Population.Cross(genome1, genome2);
                }
            }
        }

        protected virtual void DoMutation() {

            for (int i = NumberOfElitists; i < Population.Size; i++)
                Population.Mutate(MatingPool[i].Genome, pMutation);
        }
    }
}
