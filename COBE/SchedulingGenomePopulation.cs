using System;
using System.Collections;
using GA;

namespace COBE {

    [Serializable]
    public class SchedulingGenomePopulation: Population {

        GAScheduler scheduler;
        SchedulingGenome initGenome;
        int nInit;

        public GAScheduler Scheduler {
            get { return scheduler; }
        }

        public SchedulingGenomePopulation(int size, GAScheduler scheduler)
            : base(size) {

            this.scheduler = scheduler;
        }

        public SchedulingGenomePopulation(
            int size, GAScheduler scheduler, SchedulingGenome initGenome,
            int nInit): base(size) {

            this.scheduler = scheduler;
            this.initGenome = initGenome;
            this.nInit = nInit;
        }

        public override void Initialize() {

            for (int i = 0; i < nInit; i++)
                Individuals[i].Genome = (SchedulingGenome) initGenome.Clone();

            for (int i = nInit; i < Size; i++) {
                Individuals[i].Genome = new SchedulingGenome(this);
                Individuals[i].Genome.Initialize();
            }
        }
    }
}

