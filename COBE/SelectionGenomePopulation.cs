using System;
using System.Collections;
using GA;

namespace COBE {

    [Serializable]
    public class SelectionGenomePopulation: Population {

        private GAInstructionSelector instructionSelector;

        public GAInstructionSelector InstructionSelector {
            get { return instructionSelector; }
        }

        public MachineDescription MachineDescription {
            get { return instructionSelector.MachineDescription; }
        }

        public IList TransferInstructions {
            get { return instructionSelector.TransferInstructions; }
        }

        public ProgramGraph ProgramGraph {
            get { return instructionSelector.ProgramGraph; }
        }

        public IList OperationNodes {
            get { return instructionSelector.OperationNodes; }
        }

        public IList ValueNodes {
            get { return instructionSelector.ValueNodes; }
        }

        public IDictionary CoveringDesc {
            get  { return instructionSelector.CoveringDesc; }
        }

        public SelectionGenomePopulation(
            int size, GAInstructionSelector instructionSelector): base(size) {

            this.instructionSelector = instructionSelector;
        }

        public override void Initialize() {

            for (int i = 0; i < Size; i++)
                Individuals[i].Genome = new SelectionGenome(this);

            base.Initialize();
        }
    }
}
