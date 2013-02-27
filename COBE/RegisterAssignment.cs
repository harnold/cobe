using System;
using System.Collections;

namespace COBE {

    [Serializable]
    public class RegisterAssignment {

        private IDictionary registers;

        public Register this [ValueNode valueNode] {
            get { return (Register) registers[valueNode]; }
            set { registers[valueNode] = value; }
        }

        public RegisterAssignment() {
            registers = new Hashtable();
        }

        public virtual RegisterAssignment Clone(IDictionary valueMap) {

            RegisterAssignment clone = new RegisterAssignment();

            foreach (ValueNode val in registers.Keys) {
                ValueNode cloneVal = (ValueNode) valueMap[val];
                clone.registers[cloneVal] = registers[val];
            }

            return clone;
        }

        public virtual object Clone() {

            RegisterAssignment clone = new RegisterAssignment();

            foreach (ValueNode val in registers.Keys)
                clone.registers[val] = registers[val];

            return clone;
        }
    }
}

