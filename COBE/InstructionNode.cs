using System;
using System.Collections;

namespace COBE {

    [Serializable]
    public class InstructionNode: Graph.Node, ICloneable {

        private Instruction instruction;

        public ValueNode ResultValue {
            get { return (ValueNode) OutNodes[0]; }
        }

        public IList OperandValues {
            get { return InNodes; }
        }

        public Instruction Instruction {
            get { return instruction; }
        }

        public InstructionNode(Instruction instruction) {
            this.instruction = instruction;
        }

        public virtual object Clone() {
            return new InstructionNode(instruction);
        }
    }
}
