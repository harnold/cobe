using System.Collections;

namespace COBE.Samples {

    public class M56000LikeDsp: IMachineDescriptionProvider {

        private MachineDescription machineDescription;
        private IList machineRegisters;
        private IList machineInstructions;

        private RegisterSet X, Y, A, R, XYA, XA, YA, RX, RY;

        public MachineDescription CreateMachineDescription() {

            machineDescription = new MachineDescription();

            CreateMachineRegisters();
            CreateMachineInstructions();

            machineDescription = new MachineDescription();

            foreach (Register reg in machineRegisters)
                machineDescription.Registers.Add(reg);

            foreach (Instruction instr in machineInstructions)
                machineDescription.Instructions.Add(instr);

            machineDescription.ExecutionUnits = 5;

            return machineDescription;
        }

        protected void CreateMachineInstructions() {

            IList mi = new ArrayList();

            mi.Add(Utilities.CreateBinaryInstruction(
                    "ADDF $0, $1, $r",
                    new AddOperationNode(), 0,
                    new FloatRegisterNode(), A,
                    new FloatRegisterNode(), XYA,
                    new FloatRegisterNode(), XYA));

            mi.Add(Utilities.CreateBinaryInstruction(
                    "MPYF $0, $1, $r",
                    new MulOperationNode(), 0,
                    new FloatRegisterNode(), A,
                    new FloatRegisterNode(), XYA,
                    new FloatRegisterNode(), XYA));

            mi.Add(Utilities.CreateLeftTernaryInstruction(
                    "MACF $0, $1, $2, $r",
                    new MulOperationNode(), new AddOperationNode(), 0,
                    new FloatRegisterNode(), A,
                    new FloatRegisterNode(), XYA,
                    new FloatRegisterNode(), XYA,
                    new FloatRegisterNode(), A));

            mi.Add(Utilities.CreateRightTernaryInstruction(
                    "MACF $1, $2, $0, $r",
                    new MulOperationNode(), new AddOperationNode(), 0,
                    new FloatRegisterNode(), A,
                    new FloatRegisterNode(), A,
                    new FloatRegisterNode(), XYA,
                    new FloatRegisterNode(), XYA));

            mi.Add(Utilities.CreateUnaryInstruction(
                    "LOADF *$0, $r",
                    new LoadOperationNode(), 1,
                    new FloatRegisterNode(), XA,
                    new AddressRegisterNode(), RX));

            mi.Add(Utilities.CreateUnaryInstruction(
                    "LOADF $0, $r",
                    new LoadOperationNode(), 1,
                    new FloatRegisterNode(), XA,
                    new FloatMemoryNode(null), null));

            mi.Add(Utilities.CreateBinaryInstruction(
                    "STOREF $0, *$1",
                    new StoreOperationNode(), 1,
                    new FloatMemoryNode(null), null,
                    new FloatRegisterNode(), XA,
                    new AddressRegisterNode(), RX));

            mi.Add(Utilities.CreateBinaryInstruction(
                    "STOREF $0, $1",
                    new StoreOperationNode(), 1,
                    new FloatMemoryNode(null), null,
                    new FloatRegisterNode(), XA,
                    new FloatMemoryNode(null), null));

            mi.Add(Utilities.CreateUnaryInstruction(
                    "LOADF *$0, $r",
                    new LoadOperationNode(), 2,
                    new FloatRegisterNode(), YA,
                    new AddressRegisterNode(), RY));

            mi.Add(Utilities.CreateUnaryInstruction(
                    "LOADF $0, $r",
                    new LoadOperationNode(), 2,
                    new FloatRegisterNode(), YA,
                    new FloatMemoryNode(null), null));

            mi.Add(Utilities.CreateBinaryInstruction(
                    "STOREF $0, *$1",
                    new StoreOperationNode(), 2,
                    new FloatMemoryNode(null), null,
                    new FloatRegisterNode(), YA,
                    new AddressRegisterNode(), RY));

            mi.Add(Utilities.CreateBinaryInstruction(
                    "STOREF $0, $1",
                    new StoreOperationNode(), 2,
                    new FloatMemoryNode(null), null,
                    new FloatRegisterNode(), YA,
                    new FloatMemoryNode(null), null));

            mi.Add(Utilities.CreateUnaryInstruction(
                    "MOVEF $0, $r",
                    new MoveOperationNode(), 1,
                    new FloatRegisterNode(), XYA,
                    new FloatRegisterNode(), XYA));

            mi.Add(Utilities.CreateUnaryInstruction(
                    "MOVEF $0, $r",
                    new MoveOperationNode(), 2,
                    new FloatRegisterNode(), XYA,
                    new FloatRegisterNode(), XYA));

            mi.Add(Utilities.CreateUnaryInstruction(
                    "MOVE $0, $r",
                    new MoveOperationNode(), 1,
                    new AddressRegisterNode(), R,
                    new AddressRegisterNode(), R));

            mi.Add(Utilities.CreateUnaryInstruction(
                    "MOVE $0, $r",
                    new MoveOperationNode(), 2,
                    new AddressRegisterNode(), R,
                    new AddressRegisterNode(), R));

            CreateAddressIncInstructions(mi, 3);
            CreateAddressIncInstructions(mi, 4);

            machineInstructions = mi;
        }

        protected void CreateAddressIncInstructions(IList mi, int eu) {

            for (int i = 0; i < R.Count; i++) {

                RegisterSet rs = new RegisterSet();

                rs.Add(R[i]);

                mi.Add(Utilities.CreateBinaryRightConstantInstruction(
                        "$0++",
                        new ConstOperationNode(), new AddOperationNode(), eu,
                        new AddressRegisterNode(), rs,
                        new AddressRegisterNode(), rs,
                        new IntConstantNode(4), null));
                }
            }

        protected void CreateMachineRegisters() {

            X = new RegisterSet();
            Y = new RegisterSet();
            A = new RegisterSet();

            RX = new RegisterSet();
            RY = new RegisterSet();

            for (int i = 0; i < 4; i++) {
                X.Add(new Register("X" + i, Datatype.FloatValue));
                Y.Add(new Register("Y" + i, Datatype.FloatValue));
                A.Add(new Register("A" + i, Datatype.FloatValue));
            }

            for (int i = 0; i < 8; i++) {

                Register r = new Register("R" + i, Datatype.AddressValue);

                if (i < 4)
                    RX.Add(r);
                else
                    RY.Add(r);
            }

            R   = RX + RY;
            XA  = X + A;
            YA  = Y + A;
            XYA = X + Y + A;

            machineRegisters = new ArrayList();

            foreach (Register r in X.Registers)
                machineRegisters.Add(r);

            foreach (Register r in Y.Registers)
                machineRegisters.Add(r);

            foreach (Register r in R.Registers)
                machineRegisters.Add(r);

        }
    }
}
