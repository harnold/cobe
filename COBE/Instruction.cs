using System;
using System.Collections;

namespace COBE {

    [Serializable]
    public class Instruction {

        private string mnemonic;
        private int executionUnit;
        private InstructionPattern pattern;
        private RegisterSet[] operandsRegisters;
        private RegisterSet resultRegisters;

        public string Mnemonic {
            get { return mnemonic; }
        }

        public int NumberOfOperands {
            get { return pattern.OperandValues.Count; }
        }

        public InstructionPattern Pattern {
            get { return pattern; }
        }

        public int ExecutionUnit {
            get { return executionUnit; }
            set { executionUnit = value; }
        }

        public int Cycles {
            get { return 1; }
        }

        public RegisterSet[] OperandsRegisters {
            get { return operandsRegisters; }
        }

        public RegisterSet ResultRegisters {
            get { return resultRegisters; }
            set { resultRegisters = value; }
        }

        public Instruction(string mnemonic, InstructionPattern pattern) {
            this.mnemonic = mnemonic;
            this.pattern = pattern;
            operandsRegisters = new RegisterSet[pattern.OperandValues.Count];
        }
    }
}
