using System.Collections;

namespace COBE.Samples {

    public class SampleDsp1: IMachineDescriptionProvider {

        private MachineDescription machineDescription;
        private IList machineRegisters;
        private IList machineInstructions;

        private RegisterSet GR, AR;

        public MachineDescription CreateMachineDescription() {

            CreateMachineRegisters();
            CreateMachineInstructions();

            machineDescription = new MachineDescription();

            foreach (Register reg in machineRegisters)
                machineDescription.Registers.Add(reg);

            foreach (Instruction instr in machineInstructions)
                machineDescription.Instructions.Add(instr);

            machineDescription.ExecutionUnits = 6;

            return machineDescription;
        }

        protected void CreateMachineInstructions() {

            IList mi = new ArrayList();

            mi.Add(Utilities.CreateBinaryInstruction(
                    "ADDF $0, $1, $r",
                    new AddOperationNode(), 0,
                    new FloatRegisterNode(), GR,
                    new FloatRegisterNode(), GR,
                    new FloatRegisterNode(), GR));

            mi.Add(Utilities.CreateBinaryInstruction(
                    "MULF $0, $1, $r",
                    new MulOperationNode(), 0,
                    new FloatRegisterNode(), GR,
                    new FloatRegisterNode(), GR,
                    new FloatRegisterNode(), GR));

            mi.Add(Utilities.CreateLeftTernaryInstruction(
                    "MACF $0, $1, $2, $r",
                    new MulOperationNode(), new AddOperationNode(), 0,
                    new FloatRegisterNode(), GR,
                    new FloatRegisterNode(), GR,
                    new FloatRegisterNode(), GR,
                    new FloatRegisterNode(), GR));

            mi.Add(Utilities.CreateRightTernaryInstruction(
                    "MACF $1, $2, $0, $r",
                    new MulOperationNode(), new AddOperationNode(), 0,
                    new FloatRegisterNode(), GR,
                    new FloatRegisterNode(), GR,
                    new FloatRegisterNode(), GR,
                    new FloatRegisterNode(), GR));

            mi.Add(Utilities.CreateBinaryInstruction(
                    "ADDF $0, $1, $r",
                    new AddOperationNode(), 1,
                    new FloatRegisterNode(), GR,
                    new FloatRegisterNode(), GR,
                    new FloatRegisterNode(), GR));

            mi.Add(Utilities.CreateUnaryInstruction(
                    "LOADF *$0, $r",
                    new LoadOperationNode(), 2,
                    new FloatRegisterNode(), GR,
                    new AddressRegisterNode(), AR));

            mi.Add(Utilities.CreateUnaryInstruction(
                    "LOADF $0, $r",
                    new LoadOperationNode(), 2,
                    new FloatRegisterNode(), GR,
                    new FloatMemoryNode(null), null));

            mi.Add(Utilities.CreateBinaryInstruction(
                    "STOREF $0, *$1",
                    new StoreOperationNode(), 2,
                    new FloatMemoryNode(null), null,
                    new FloatRegisterNode(), GR,
                    new AddressRegisterNode(), AR));

            mi.Add(Utilities.CreateBinaryInstruction(
                    "STOREF $0, $1",
                    new StoreOperationNode(), 2,
                    new FloatMemoryNode(null), null,
                    new FloatRegisterNode(), GR,
                    new FloatMemoryNode(null), null));

            mi.Add(Utilities.CreateUnaryInstruction(
                    "LOADF *$0, $r",
                    new LoadOperationNode(), 4,
                    new FloatRegisterNode(), GR,
                    new AddressRegisterNode(), AR));

            mi.Add(Utilities.CreateUnaryInstruction(
                    "LOADF $0, $r",
                    new LoadOperationNode(), 4,
                    new FloatRegisterNode(), GR,
                    new FloatMemoryNode(null), null));

            mi.Add(Utilities.CreateBinaryInstruction(
                    "STOREF $0, *$1",
                    new StoreOperationNode(), 4,
                    new FloatMemoryNode(null), null,
                    new FloatRegisterNode(), GR,
                    new AddressRegisterNode(), AR));

            mi.Add(Utilities.CreateBinaryInstruction(
                    "STOREF $0, $1",
                    new StoreOperationNode(), 4,
                    new FloatMemoryNode(null), null,
                    new FloatRegisterNode(), GR,
                    new FloatMemoryNode(null), null));

            CreateAddressIncInstructions(mi, 3);
            CreateAddressIncInstructions(mi, 5);

            machineInstructions = mi;
        }

        protected void CreateAddressIncInstructions(IList mi, int eu) {

            for (int i = 0; i < AR.Count; i++) {

                RegisterSet rs = new RegisterSet();

                rs.Add(AR[i]);

                mi.Add(Utilities.CreateBinaryRightConstantInstruction(
                        "$0++",
                        new ConstOperationNode(), new AddOperationNode(), eu,
                        new AddressRegisterNode(), rs,
                        new AddressRegisterNode(), rs,
                        new IntConstantNode(4), null));
            }
        }

        protected void CreateMachineRegisters() {

            GR = new RegisterSet();
            AR = new RegisterSet();

            for (int i = 0; i < 8; i++) {
                GR.Add(new Register("GR" + i, Datatype.FloatValue));
                AR.Add(new Register("AR" + i, Datatype.AddressValue));
            }

            machineRegisters = new ArrayList();

            foreach (Register r in GR.Registers)
                machineRegisters.Add(r);

            foreach (Register r in AR.Registers)
                machineRegisters.Add(r);
        }
    }
}
