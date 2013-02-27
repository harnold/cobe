using System.Collections;

namespace COBE.Samples {

    public class C3xLikeVLIW: IMachineDescriptionProvider {

        private MachineDescription machineDescription;
        private IList machineRegisters;
        private IList machineInstructions;

        private RegisterSet GR, AR;

        public MachineDescription CreateMachineDescription() {

            machineDescription = new MachineDescription();

            CreateMachineRegisters();
            CreateMachineInstructions();

            machineDescription = new MachineDescription();

            foreach (Register reg in machineRegisters)
                machineDescription.Registers.Add(reg);

            foreach (Instruction instr in machineInstructions)
                machineDescription.Instructions.Add(instr);

            machineDescription.ExecutionUnits = 8;

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

            mi.Add(Utilities.CreateLeftTernaryInstruction(
                    "ADDF *$0, $1, $r",
                    new LoadOperationNode(), new AddOperationNode(), 0,
                    new FloatRegisterNode(), GR,
                    new AddressRegisterNode(), AR,
                    new FloatRegisterNode(), GR,
                    Datatype.FloatValue));

            mi.Add(Utilities.CreateRightTernaryInstruction(
                    "ADDF $0, *$1, $r",
                    new LoadOperationNode(), new AddOperationNode(), 0,
                    new FloatRegisterNode(), GR,
                    new FloatRegisterNode(), GR,
                    new AddressRegisterNode(), AR,
                    Datatype.FloatValue));

            mi.Add(Utilities.CreateQuadInstruction(
                    "ADDF *$0, *$1, $r",
                    new LoadOperationNode(), new LoadOperationNode(),
                    new AddOperationNode(), 0,
                    new FloatRegisterNode(), GR,
                    new AddressRegisterNode(), AR,
                    new AddressRegisterNode(), AR,
                    Datatype.FloatValue, Datatype.FloatValue));

            mi.Add(Utilities.CreateBinaryInstruction(
                    "MPYF $0, $1, $r",
                    new MulOperationNode(), 1,
                    new FloatRegisterNode(), GR,
                    new FloatRegisterNode(), GR,
                    new FloatRegisterNode(), GR));

            mi.Add(Utilities.CreateLeftTernaryInstruction(
                    "MPYF *$0, $1, $r",
                    new LoadOperationNode(), new MulOperationNode(), 1,
                    new FloatRegisterNode(), GR,
                    new AddressRegisterNode(), AR,
                    new FloatRegisterNode(), GR,
                    Datatype.FloatValue));

            mi.Add(Utilities.CreateRightTernaryInstruction(
                    "MPYF $0, *$1, $r",
                    new LoadOperationNode(), new MulOperationNode(), 1,
                    new FloatRegisterNode(), GR,
                    new FloatRegisterNode(), GR,
                    new AddressRegisterNode(), AR,
                    Datatype.FloatValue));

            mi.Add(Utilities.CreateQuadInstruction(
                    "MPYF *$0, *$1, $r",
                    new LoadOperationNode(), new LoadOperationNode(),
                    new MulOperationNode(), 1,
                    new FloatRegisterNode(), GR,
                    new AddressRegisterNode(), AR,
                    new AddressRegisterNode(), AR,
                    Datatype.FloatValue, Datatype.FloatValue));

            mi.Add(Utilities.CreateUnaryInstruction(
                    "LDF *$0, $r",
                    new LoadOperationNode(), 0,
                    new FloatRegisterNode(), GR,
                    new AddressRegisterNode(), AR));

            mi.Add(Utilities.CreateUnaryInstruction(
                    "LDF $0, $r",
                    new LoadOperationNode(), 0,
                    new FloatRegisterNode(), GR,
                    new FloatMemoryNode(null), null));

            mi.Add(Utilities.CreateUnaryInstruction(
                    "LDF $0, $r",
                    new MoveOperationNode(), 0,
                    new FloatRegisterNode(), GR,
                    new FloatRegisterNode(), GR));

            mi.Add(Utilities.CreateUnaryInstruction(
                    "LDI $0, $r",
                    new MoveOperationNode(), 0,
                    new IntRegisterNode(), AR,
                    new IntRegisterNode(), AR));

            mi.Add(Utilities.CreateBinaryInstruction(
                    "STF $0, *$1",
                    new StoreOperationNode(), 0,
                    new FloatMemoryNode(null), null,
                    new FloatRegisterNode(), GR,
                    new AddressRegisterNode(), AR));

            mi.Add(Utilities.CreateBinaryInstruction(
                    "STF $0, $1",
                    new StoreOperationNode(), 0,
                    new FloatMemoryNode(null), null,
                    new FloatRegisterNode(), GR,
                    new FloatMemoryNode(null), null));

            mi.Add(Utilities.CreateUnaryInstruction(
                    "LDF *$0, $r",
                    new LoadOperationNode(), 1,
                    new FloatRegisterNode(), GR,
                    new AddressRegisterNode(), AR));

            mi.Add(Utilities.CreateUnaryInstruction(
                    "LDF $0, $r",
                    new LoadOperationNode(), 1,
                    new FloatRegisterNode(), GR,
                    new FloatMemoryNode(null), null));

            mi.Add(Utilities.CreateUnaryInstruction(
                    "LDF $0, $r",
                    new MoveOperationNode(), 1,
                    new FloatRegisterNode(), GR,
                    new FloatRegisterNode(), GR));

            mi.Add(Utilities.CreateUnaryInstruction(
                    "LDI $0, $r",
                    new MoveOperationNode(), 1,
                    new IntRegisterNode(), AR,
                    new IntRegisterNode(), AR));

            mi.Add(Utilities.CreateBinaryInstruction(
                    "STF $0, *$1",
                    new StoreOperationNode(), 1,
                    new FloatMemoryNode(null), null,
                    new FloatRegisterNode(), GR,
                    new AddressRegisterNode(), AR));

            mi.Add(Utilities.CreateBinaryInstruction(
                    "STF $0, $1",
                    new StoreOperationNode(), 1,
                    new FloatMemoryNode(null), null,
                    new FloatRegisterNode(), GR,
                    new FloatMemoryNode(null), null));

            CreateAddressIncInstructions(mi, 2);
            CreateAddressIncInstructions(mi, 3);

            mi.Add(Utilities.CreateBinaryInstruction(
                    "ADDF $0, $1, $r",
                    new AddOperationNode(), 4,
                    new FloatRegisterNode(), GR,
                    new FloatRegisterNode(), GR,
                    new FloatRegisterNode(), GR));

            mi.Add(Utilities.CreateLeftTernaryInstruction(
                    "ADDF *$0, $1, $r",
                    new LoadOperationNode(), new AddOperationNode(), 4,
                    new FloatRegisterNode(), GR,
                    new AddressRegisterNode(), AR,
                    new FloatRegisterNode(), GR,
                    Datatype.FloatValue));

            mi.Add(Utilities.CreateRightTernaryInstruction(
                    "ADDF $0, *$1, $r",
                    new LoadOperationNode(), new AddOperationNode(), 4,
                    new FloatRegisterNode(), GR,
                    new FloatRegisterNode(), GR,
                    new AddressRegisterNode(), AR,
                    Datatype.FloatValue));

            mi.Add(Utilities.CreateQuadInstruction(
                    "ADDF *$0, *$1, $r",
                    new LoadOperationNode(), new LoadOperationNode(),
                    new AddOperationNode(), 4,
                    new FloatRegisterNode(), GR,
                    new AddressRegisterNode(), AR,
                    new AddressRegisterNode(), AR,
                    Datatype.FloatValue, Datatype.FloatValue));

            mi.Add(Utilities.CreateBinaryInstruction(
                    "MPYF $0, $1, $r",
                    new MulOperationNode(), 5,
                    new FloatRegisterNode(), GR,
                    new FloatRegisterNode(), GR,
                    new FloatRegisterNode(), GR));

            mi.Add(Utilities.CreateLeftTernaryInstruction(
                    "MPYF *$0, $1, $r",
                    new LoadOperationNode(), new MulOperationNode(), 5,
                    new FloatRegisterNode(), GR,
                    new AddressRegisterNode(), AR,
                    new FloatRegisterNode(), GR,
                    Datatype.FloatValue));

            mi.Add(Utilities.CreateRightTernaryInstruction(
                    "MPYF $0, *$1, $r",
                    new LoadOperationNode(), new MulOperationNode(), 5,
                    new FloatRegisterNode(), GR,
                    new FloatRegisterNode(), GR,
                    new AddressRegisterNode(), AR,
                    Datatype.FloatValue));

            mi.Add(Utilities.CreateQuadInstruction(
                    "MPYF *$0, *$1, $r",
                    new LoadOperationNode(), new LoadOperationNode(),
                    new MulOperationNode(), 5,
                    new FloatRegisterNode(), GR,
                    new AddressRegisterNode(), AR,
                    new AddressRegisterNode(), AR,
                    Datatype.FloatValue, Datatype.FloatValue));

            mi.Add(Utilities.CreateUnaryInstruction(
                    "LDF *$0, $r",
                    new LoadOperationNode(), 4,
                    new FloatRegisterNode(), GR,
                    new AddressRegisterNode(), AR));

            mi.Add(Utilities.CreateUnaryInstruction(
                    "LDF $0, $r",
                    new LoadOperationNode(), 4,
                    new FloatRegisterNode(), GR,
                    new FloatMemoryNode(null), null));

            mi.Add(Utilities.CreateUnaryInstruction(
                    "LDF $0, $r",
                    new MoveOperationNode(), 4,
                    new FloatRegisterNode(), GR,
                    new FloatRegisterNode(), GR));

            mi.Add(Utilities.CreateUnaryInstruction(
                    "LDI $0, $r",
                    new MoveOperationNode(), 4,
                    new IntRegisterNode(), AR,
                    new IntRegisterNode(), AR));

            mi.Add(Utilities.CreateBinaryInstruction(
                    "STF $0, *$1",
                    new StoreOperationNode(), 4,
                    new FloatMemoryNode(null), null,
                    new FloatRegisterNode(), GR,
                    new AddressRegisterNode(), AR));

            mi.Add(Utilities.CreateBinaryInstruction(
                    "STF $0, $1",
                    new StoreOperationNode(), 4,
                    new FloatMemoryNode(null), null,
                    new FloatRegisterNode(), GR,
                    new FloatMemoryNode(null), null));

            mi.Add(Utilities.CreateUnaryInstruction(
                    "LDF *$0, $r",
                    new LoadOperationNode(), 5,
                    new FloatRegisterNode(), GR,
                    new AddressRegisterNode(), AR));

            mi.Add(Utilities.CreateUnaryInstruction(
                    "LDF $0, $r",
                    new LoadOperationNode(), 5,
                    new FloatRegisterNode(), GR,
                    new FloatMemoryNode(null), null));

            mi.Add(Utilities.CreateUnaryInstruction(
                    "LDF $0, $r",
                    new MoveOperationNode(), 5,
                    new FloatRegisterNode(), GR,
                    new FloatRegisterNode(), GR));

            mi.Add(Utilities.CreateUnaryInstruction(
                    "LDI $0, $r",
                    new MoveOperationNode(), 5,
                    new IntRegisterNode(), AR,
                    new IntRegisterNode(), AR));

            mi.Add(Utilities.CreateBinaryInstruction(
                    "STF $0, *$1",
                    new StoreOperationNode(), 5,
                    new FloatMemoryNode(null), null,
                    new FloatRegisterNode(), GR,
                    new AddressRegisterNode(), AR));

            mi.Add(Utilities.CreateBinaryInstruction(
                    "STF $0, $1",
                    new StoreOperationNode(), 5,
                    new FloatMemoryNode(null), null,
                    new FloatRegisterNode(), GR,
                    new FloatMemoryNode(null), null));

            CreateAddressIncInstructions(mi, 6);
            CreateAddressIncInstructions(mi, 7);

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

            for (int i = 0; i < 16; i++) {
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
