using System;
using System.Collections;
using GA;

namespace COBE {

    [Serializable]
    public class SelectionGenomeEvaluator: IGenomeEvaluator {

        private GAScheduler scheduler;

        public SelectionGenomeEvaluator(GAScheduler scheduler) {
            this.scheduler = scheduler;
        }

        public virtual double Evaluate(IGenome genome) {

            SelectionGenome sGenome = (SelectionGenome) genome;

            bool result;

            if (sGenome.Modified) {

                sGenome.Modified = false;
                sGenome.BuildInstructionGraph();

                /*
                int currentGen = sGenome.Population.GA.Generation;
                scheduler.MaxGenerations = (int) (100 * Math.Pow(2, currentGen));
                */

                scheduler.Initialize(sGenome.MachineDescription,
                                     sGenome.InstructionGraph);

                RegisterAssignment registerAssignment;
                InstructionSchedule instructionSchedule;

                result = scheduler.Optimize(out registerAssignment,
                                            out instructionSchedule);

                sGenome.IsValid = result;
                sGenome.RegisterAssignment = registerAssignment;
                sGenome.InstructionSchedule = instructionSchedule;
                sGenome.Objective = scheduler.BestIndividual.Objective;

                /*
                sGenome.BestSchedulingGenome = scheduler.BestGenome;
                */
            }

            /*
            else {
                scheduler.Initialize(sGenome.MachineDescription,
                                     sGenome.InstructionGraph,
                                     sGenome.BestSchedulingGenome);
            }
            */

            return sGenome.Objective;
        }
    }
}
