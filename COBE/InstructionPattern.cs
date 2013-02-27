using System;
using System.Collections;

namespace COBE {

    [Serializable]
    public class InstructionPattern: OperationValueGraph {

        private ValueNode resultValue;
        private IList operandValues;

        public ValueNode ResultValue {
            get { return resultValue; }
            set { resultValue = value; }
        }

        public IList OperandValues {
            get { return operandValues; }
        }

        public InstructionPattern() {
            operandValues = new ArrayList();
        }
    }
}
