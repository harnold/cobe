using System;
using System.Collections;

namespace COBE {

    public class InstructionGraph: InstructionValueGraph {

        private IList inputValues;
        private IList outputValues;
        private IDictionary assignableRegisters;
        private IDictionary cyclicDependencies;

        public IDictionary CyclicDependencies {
            get { return cyclicDependencies; }
        }

        public IList InputValues {
            get { return inputValues; }
        }

        public IList OutputValues {
            get { return outputValues; }
        }

        public IDictionary AssignableRegisters {
            get { return assignableRegisters; }
        }

        public InstructionGraph() {
            inputValues = new ArrayList();
            outputValues = new ArrayList();
            assignableRegisters = new Hashtable();
            cyclicDependencies = new Hashtable();
        }

        public virtual InstructionGraph Clone(
            out IDictionary instructionMap,
            out IDictionary valueMap) {

            InstructionGraph clone = new InstructionGraph();

            instructionMap = new Hashtable();
            valueMap = new Hashtable();

            IList instructionNodes = GetInstructionNodes();
            IList valueNodes = GetValueNodes();

            foreach (InstructionNode instr in instructionNodes) {
                InstructionNode cloneInstr = (InstructionNode) instr.Clone();
                clone.AddNode(cloneInstr);
                instructionMap[instr] = cloneInstr;
            }

            foreach (ValueNode val in valueNodes) {
                ValueNode cloneVal = (ValueNode) val.Clone();
                clone.AddNode(cloneVal);
                valueMap[val] = cloneVal;
            }

            foreach (ValueNode val in inputValues)
                clone.inputValues.Add(valueMap[val]);

            foreach (ValueNode val in outputValues)
                clone.outputValues.Add(valueMap[val]);

            foreach (ValueNode val in valueNodes) {

                if (val is RegisterValueNode) {

                    RegisterSet assignableRegs =
                        (RegisterSet) assignableRegisters[val];

                    clone.assignableRegisters[valueMap[val]] =
                        assignableRegs.Clone();
                }
            }

            IDictionary valueProcessed = new Hashtable();

            foreach (ValueNode val in valueNodes)
                valueProcessed[val] = false;

            foreach (ValueNode val in outputValues) {
                CloneEdgesForValue(val, clone, instructionMap, valueMap,
                                   valueProcessed);
            }

            foreach (ValueNode val in cyclicDependencies.Keys) {
                clone.cyclicDependencies[valueMap[val]] =
                    valueMap[cyclicDependencies[val]];
            }

            return clone;
        }

        protected virtual void CloneEdgesForValue(
            ValueNode val,
            InstructionGraph clone,
            IDictionary instructionMap,
            IDictionary valueMap,
            IDictionary valueProcessed) {

            if (!(bool) valueProcessed[val] && !val.IsInputValue()) {

                valueProcessed[val] = true;
                ValueNode cloneResValue = (ValueNode) valueMap[val];
                InstructionNode cloneInstr =
                    (InstructionNode) instructionMap[val.ProducingInstruction];

                clone.AddEdge(cloneInstr, cloneResValue);

                foreach (ValueNode opVal in val.ProducingInstruction.OperandValues) {
                    ValueNode cloneOpVal = (ValueNode) valueMap[opVal];
                    clone.AddEdge(cloneOpVal, cloneInstr);
                    CloneEdgesForValue(opVal, clone, instructionMap, valueMap,
                                       valueProcessed);
                }
            }
        }
    }
}
