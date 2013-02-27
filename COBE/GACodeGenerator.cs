using System;
using System.Collections;

namespace COBE {

    [Serializable]
    public class GACodeGenerator {

        private GAInstructionSelectorConfiguration selectorConfiguration;
        private GASchedulerConfiguration schedulerConfiguration;

        private GAInstructionSelector selector;
        private GAScheduler scheduler;

        public GAInstructionSelector InstructionSelector {
            get { return selector; }
        }

        public GAScheduler Scheduler {
            get { return scheduler; }
        }

        public GAInstructionSelectorConfiguration SelectorConfiguration {
            get { return selectorConfiguration; }
        }

        public GASchedulerConfiguration SchedulerConfiguration {
            get { return schedulerConfiguration; }
        }

        public GACodeGenerator(
            GAInstructionSelectorConfiguration selectorConfiguration,
            GASchedulerConfiguration schedulerConfiguration) {

            this.selectorConfiguration = selectorConfiguration;
            this.schedulerConfiguration = schedulerConfiguration;

            selector = new GAInstructionSelector(this);
            scheduler = selector.Scheduler;
        }

        public virtual void Initialize(
            MachineDescription machineDescription,
            ProgramGraph programGraph) {

            selector.Initialize(machineDescription, programGraph);
        }

        public virtual bool Optimize(
            out InstructionGraph instructionGraph,
            out RegisterAssignment registerAssignment,
            out InstructionSchedule instructionSchedule) {

            return selector.Optimize(out instructionGraph,
                                     out registerAssignment,
                                     out instructionSchedule);
        }

    }
}
