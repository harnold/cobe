using System;
using System.Collections;

namespace COBE {

    [Serializable]
    public class AssemblyGenerator {

        private RegisterAssignment registerAssignment;

        public AssemblyGenerator(RegisterAssignment registerAssignment) {
            this.registerAssignment = registerAssignment;
        }

        public string GetAssembly(InstructionNode iNode) {

            string result = "";
            string iMnemonic = iNode.Instruction.Mnemonic;

            int i = 0;

            while (i < iMnemonic.Length) {

                if (iMnemonic[i] == '$') {

                    string opStr = iMnemonic.Substring(++i, 1);

                    if (opStr == "r") {
                        result += GetAssemblyForValue(iNode.ResultValue);
                    } else {
                        int opIndex = Int32.Parse(opStr);
                        result += GetAssemblyForValue(
                            (ValueNode) iNode.OperandValues[opIndex]);
                    }
                } else {
                    result += iMnemonic[i];
                }

                ++i;
            }

            return result;
        }

        protected string GetAssemblyForValue(ValueNode vNode) {

            if (vNode is RegisterValueNode)
                return registerAssignment[vNode].Mnemonic;
            else if (vNode is MemoryValueNode)
                return vNode.ToString();
            else if (vNode is ConstantValueNode)
                return vNode.ToString();
            else
                return null;
        }
    }
}
