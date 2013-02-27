using System;

namespace GA {

    [Serializable]
    public abstract class GeneticAlgorithm {

        public abstract void Initialize();
        public abstract void ComputeNextGeneration();

        private bool maximize = false;
        private int generation;

        public int Generation {
            get { return generation; }
            set { generation = value; }
        }
        public bool Maximize {
            get { return maximize; }
            set { maximize = value; }
        }

        public bool Minimize {
            get { return !maximize; }
            set { maximize = !value; }
        }
    }
}
