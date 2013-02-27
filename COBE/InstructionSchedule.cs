using System;
using System.Collections;

namespace COBE {

    [Serializable]
    public class InstructionSchedule {

        private IList instructions;

        public int Steps {
            get { return instructions.Count; }
        }

        public IList Instructions {
            get { return instructions; }
        }

        public InstructionNode[] this [int step] {
            get { return (InstructionNode[]) instructions[step]; }
        }

        public InstructionSchedule() {
            instructions = new ArrayList();
        }

        public virtual InstructionSchedule Clone(IDictionary instructionMap) {

            InstructionSchedule clone = new InstructionSchedule();

            foreach (InstructionNode[] step in instructions) {

                InstructionNode[] cloneStep = new InstructionNode[step.Length];

                for (int i = 0; i < step.Length; i++) {
                    if (step[i] != null)
                        cloneStep[i] = (InstructionNode) instructionMap[step[i]];
                    else
                        cloneStep[i] = null;
                }

                clone.instructions.Add(cloneStep);
            }

            return clone;
        }
    }
}
