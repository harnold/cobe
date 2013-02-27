using System;
using System.Collections;

namespace COBE {

    [Serializable]
    public class MachineDescription {

        private IList instructions;
        private IList registers;
        private IList memories;
        private int executionUnits;

        public IList Instructions {
            get { return instructions; }
        }

        public IList Registers {
            get { return registers; }
        }

        public IList Memories {
            get { return memories; }
        }

        public int ExecutionUnits {
            get { return executionUnits; }
            set { executionUnits = value; }
        }

        public MachineDescription() {
            instructions = new ArrayList();
            registers = new ArrayList();
            memories = new ArrayList();
        }

        public IList GetTransferInstructions() {

            IList transferInstructions = new ArrayList();

            foreach (Instruction i in instructions) {

                OperationNode op = i.Pattern.ResultValue.ProducingOperation;

                if (op.OperationType == OperationType.MoveOp &&
                    i.Pattern.Nodes.Count == 3) {

                    transferInstructions.Add(i);
                }
            }

            return transferInstructions;
        }
    }
}
