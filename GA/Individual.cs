using System;

namespace GA {

    [Serializable]
    public struct Individual: IComparable {

        private IGenome genome;
        private double objective;
        private double fitness;

        public IGenome Genome {
            get { return genome; }
            set { genome = value; }
        }

        public double Objective {
            get { return objective; }
            set { objective = value; }
        }

        public double Fitness {
            get { return fitness; }
            set { fitness = value; }
        }

        public int CompareTo(object obj) {

            if (!(obj is Individual))
                throw new ArgumentException();

            Individual other = (Individual) obj;

            if (this.Objective < other.Objective)
                return -1;
            else if (this.Objective > other.Objective)
                return 1;
            else
                return 0;
        }

        public void Copy(Individual otherInd) {
            Genome.Copy(otherInd.Genome);
            Objective = otherInd.Objective;
            Fitness = otherInd.Fitness;
        }
    }
}
