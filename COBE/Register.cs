using System;
using System.Collections;

namespace COBE {

    [Serializable]
    public class Register {

        private string   mnemonic;
        private Datatype datatype;

        public string Mnemonic {
            get { return mnemonic; }
        }

        public Datatype Datatype {
            get { return datatype; }
        }

        public Register(string mnemonic, Datatype datatype) {
            this.mnemonic = mnemonic;
            this.datatype = datatype;
        }
    }

    [Serializable]
    public class RegisterSet: ICloneable {

        private IList registers;

        public Register this [int index] {
            get { return (Register) registers[index]; }
        }

        public IList Registers {
            get { return registers; }
        }

        public int Count {
            get { return registers.Count; }
        }

        public RegisterSet() {
            this.registers = new ArrayList();
        }

        public virtual void Add(Register reg) {
            registers.Add(reg);
        }

        public virtual void Remove(Register reg) {
            registers.Remove(reg);
        }

        public virtual void Clear() {
            registers.Clear();
        }

        public virtual bool Contains(Register reg) {
            return registers.Contains(reg);
        }

        public virtual object Clone() {

            RegisterSet clone = new RegisterSet();

            foreach (Register reg in registers)
                clone.registers.Add(reg);

            return clone;
        }

        public static RegisterSet Union(RegisterSet s1, RegisterSet s2) {

            RegisterSet result = new RegisterSet();

            foreach (Register reg in s1.registers)
                result.registers.Add(reg);

            foreach (Register reg in s2.registers) {
                if (!result.Contains(reg))
                    result.registers.Add(reg);
            }

            return result;
        }

        public static RegisterSet Intersection(RegisterSet s1, RegisterSet s2) {

            RegisterSet result = new RegisterSet();

            foreach (Register reg in s1.registers) {
                if (s2.registers.Contains(reg))
                    result.registers.Add(reg);
            }

            return result;
        }

        public static RegisterSet Difference(RegisterSet s1, RegisterSet s2) {

            RegisterSet result = new RegisterSet();

            foreach (Register reg in s1.registers) {
                if (!s2.registers.Contains(reg))
                    result.registers.Add(reg);
            }

            return result;
        }

        public static RegisterSet operator +(RegisterSet s1, RegisterSet s2) {
            return Union(s1, s2);
        }

        public static RegisterSet operator *(RegisterSet s1, RegisterSet s2) {
            return Intersection(s1, s2);
        }

        public static RegisterSet operator -(RegisterSet s1, RegisterSet s2) {
            return Difference(s1, s2);
        }
    }
}
