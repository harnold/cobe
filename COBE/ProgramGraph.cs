using System;
using System.Collections;

namespace COBE {

    [Serializable]
    public class CoveringInfo {

        private Instruction instruction;
        private IList coveredValues;
        private IList coveredOperations;
        private ValueNode resultValue;
        private IList operandValues;

        public CoveringInfo(Instruction instruction) {
            this.instruction       = instruction;
            this.coveredValues     = new ArrayList();
            this.coveredOperations = new ArrayList();
            this.operandValues     = new ArrayList();
        }

        public Instruction Instruction {
            get { return instruction; }
        }

        public IList CoveredValues {
            get { return coveredValues; }
        }

        public IList CoveredOperations {
            get { return coveredOperations; }
        }

        public ValueNode ResultValue {
            get { return resultValue; }
            set { resultValue = value; }
        }

        public IList OperandValues {
            get { return operandValues; }
        }
    }

    [Serializable]
    public class ProgramGraph: OperationValueGraph {

        private IList inputValues;
        private IList outputValues;

        private IDictionary preassignedRegisters;
        private IDictionary cyclicDependencies;

        public IDictionary PreassignedRegisters {
            get { return preassignedRegisters; }
        }

        public IDictionary CyclicDependencies {
            get { return cyclicDependencies; }
        }

        public IList InputValues {
            get { return inputValues; }
        }

        public IList OutputValues {
            get { return outputValues; }
        }

        public ProgramGraph() {

            inputValues = new ArrayList();
            outputValues = new ArrayList();

            preassignedRegisters = new Hashtable();
            cyclicDependencies = new Hashtable();
        }

        public virtual void InitInputValues() {

            inputValues.Clear();

            foreach (Graph.Node n in Nodes) {
                if (n.InNodes.Count == 0) {
                    inputValues.Add(n);
                }
            }
        }

        public virtual void InitOutputValues() {

            outputValues.Clear();

            foreach (Graph.Node n in Nodes) {
                if (n.OutNodes.Count == 0)
                    outputValues.Add(n);
            }
        }

        public virtual IList GetComputedValues() {

            IList computedValues = new ArrayList();

            foreach (Graph.Node n in Nodes) {

                if (n is ValueNode) {
                    ValueNode v = (ValueNode) n;
                    if (!v.IsInputValue())
                        computedValues.Add(v);
                }
            }

            return computedValues;
        }

        public virtual IDictionary CreateCoveringDesc(IList instructions) {

            IDictionary coveringDesc = new Hashtable();
            IList computedValues = GetComputedValues();

            foreach (ValueNode val in computedValues) {

                IList coveringDescForValue =
                    CreateCoveringDescForValue(val, instructions);

                coveringDesc[val] = coveringDescForValue;
            }

            return coveringDesc;
        }

        protected virtual IList CreateCoveringDescForValue(
            ValueNode val, IList instructions) {

            IList coveringDescForValue = new ArrayList();

            foreach (Instruction instr in instructions) {

                CoveringInfo coveringInfo = new CoveringInfo(instr);

                if (val.Cover(instr.Pattern.ResultValue, coveringInfo)) {
                    coveringInfo.ResultValue = val;
                    coveringDescForValue.Add(coveringInfo);
                }
            }

            return coveringDescForValue;
        }

    }
}
