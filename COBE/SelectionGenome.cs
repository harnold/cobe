using System;
using System.Collections;
using GA;

namespace COBE {

    [Serializable]
    public class SelectionGenome: IGenome {

        private SelectionGenomePopulation population;

        private IDictionary selection;

        private bool isValid  = false;
        private bool modified = true;
        private double objective;

        private InstructionSchedule instructionSchedule;
        private RegisterAssignment registerAssignment;
        private InstructionGraph instructionGraph;

        /*
        private SchedulingGenome bestSchedulingGenome;

        public SchedulingGenome BestSchedulingGenome {
            get { return bestSchedulingGenome; }
            set { bestSchedulingGenome = value; }
        }
        */

        public InstructionSchedule InstructionSchedule {
            get { return instructionSchedule; }
            set { instructionSchedule = value; }
        }

        public RegisterAssignment RegisterAssignment {
            get { return registerAssignment; }
            set { registerAssignment = value; }
        }

        public InstructionGraph InstructionGraph {
            get { return instructionGraph; }
            set { instructionGraph = value; }
        }

        public double Objective {
            get { return objective; }
            set { objective = value; }
        }

        public bool Modified {
            get { return modified; }
            set { modified = value; }
        }

        public bool IsValid {
            get { return isValid; }
            set { isValid = value; }
        }

        public SelectionGenomePopulation Population {
            get { return population; }
        }

        public MachineDescription MachineDescription {
            get { return population.MachineDescription; }
        }

        public IList TransferInstructions {
            get { return population.TransferInstructions; }
        }

        public ProgramGraph ProgramGraph {
            get { return population.ProgramGraph; }
        }

        public IList OperationNodes {
            get { return population.OperationNodes; }
        }

        public IList ValueNodes {
            get { return population.ValueNodes; }
        }

        public IDictionary CoveringDesc {
            get { return population.CoveringDesc; }
        }

        public IDictionary Selection {
            get { return selection; }
        }

        public SelectionGenome(SelectionGenomePopulation population) {
            this.population = population;
            selection = new Hashtable();
        }

        public virtual void Initialize() {

            IDictionary valueInitialized = new Hashtable();

            foreach (ValueNode val in ValueNodes)
                valueInitialized[val] = false;

            foreach (ValueNode val in ProgramGraph.OutputValues)
                InitializeValue(val, valueInitialized);

            instructionSchedule = null;
            registerAssignment = null;
            instructionGraph = null;

            /*
            bestSchedulingGenome = null;
            */
        }

        protected virtual void InitializeValue(
            ValueNode val, IDictionary valueInitialized) {

            if (!((bool) valueInitialized[val])) {

                valueInitialized[val] = true;

                if (!val.IsInputValue()) {

                    IList coveringDesc = (IList) CoveringDesc[val];

                    CoveringInfo ci = (CoveringInfo)
                        coveringDesc[GA.Random.Next(coveringDesc.Count)];

                    foreach (OperationNode op in ci.CoveredOperations)
                        selection[op] = ci;

                    foreach (ValueNode opVal in ci.OperandValues)
                        InitializeValue(opVal, valueInitialized);
                }
            }

        }

        public virtual void Copy(IGenome otherGenome) {

            SelectionGenome other = (SelectionGenome) otherGenome;

            population = other.population;
            isValid = other.isValid;
            modified = other.modified;
            objective = other.objective;

            if (other.instructionGraph != null) {

                IDictionary instructionMap;
                IDictionary valueMap;

                instructionGraph =
                    other.instructionGraph.Clone(
                        out instructionMap, out valueMap);

                if (other.registerAssignment != null &&
                    other.instructionSchedule != null) {

                    registerAssignment =
                        other.registerAssignment.Clone(valueMap);

                    instructionSchedule =
                        other.instructionSchedule.Clone(instructionMap);

                    /*
                    bestSchedulingGenome =
                        other.bestSchedulingGenome.Clone(
                            instructionMap, valueMap);
                    */

                } else {
                    registerAssignment = null;
                    instructionSchedule = null;

                    /*
                    bestSchedulingGenome = null;
                    */
                }

            } else {
                instructionGraph = null;
            }

            foreach (OperationNode op in OperationNodes)
                selection[op] = other.selection[op];
        }

        public virtual object Clone() {
            SelectionGenome clone = new SelectionGenome(population);
            clone.Copy(this);
            return clone;
        }

        public virtual double DifferenceTo(IGenome otherGenome) {

            SelectionGenome other = (SelectionGenome) otherGenome;

            double d = 0.0;

            foreach (OperationNode op in OperationNodes) {
                if (selection[op] != other.selection[op])
                    d += 1.0;
            }

            return d / OperationNodes.Count;
        }

        public virtual void BuildInstructionGraph() {

            IDictionary valueProcessed = new Hashtable();

            foreach (ValueNode val in ValueNodes)
                valueProcessed[val] = false;

            instructionGraph = new InstructionGraph();
            IDictionary valueNodeMap = new Hashtable();

            foreach (ValueNode val in ProgramGraph.GetValueNodes()) {
                if (val.OutNodes.Count == 0) {
                    BuildInstructionGraphForValue(
                        val, valueProcessed, valueNodeMap);
                }
            }

            foreach (ValueNode val in ProgramGraph.InputValues)
                instructionGraph.InputValues.Add(valueNodeMap[val]);

            foreach (ValueNode val in ProgramGraph.OutputValues)
                instructionGraph.OutputValues.Add(valueNodeMap[val]);

            ComputeAssignableRegisters();

            foreach (ValueNode val in ProgramGraph.CyclicDependencies.Keys) {
                ValueNode depVal = (ValueNode)
                    ProgramGraph.CyclicDependencies[val];
                instructionGraph.CyclicDependencies[
                    valueNodeMap[val]] = valueNodeMap[depVal];
            }

            InsertCyclicTransferInstructions();
        }

        protected virtual void InsertCyclicTransferInstructions() {

            foreach (ValueNode inVal in instructionGraph.CyclicDependencies.Keys) {

                if (inVal.IsInputValue()) {

                    ValueNode outVal = (ValueNode)
                        instructionGraph.CyclicDependencies[inVal];

                    RegisterSet inValRegs = (RegisterSet)
                        instructionGraph.AssignableRegisters[inVal];

                    RegisterSet outValRegs = (RegisterSet)
                        instructionGraph.AssignableRegisters[outVal];

                    if ((inValRegs * outValRegs).Count == 0) {
                        InsertTransferInstructionBefore(
                            outVal, outValRegs[0], inValRegs[0],
                            outValRegs, inValRegs);
                    }
                }
            }
        }

        protected virtual IList GetTransferInstructions(
            Register fromReg, Register toReg) {

            IList applicableTransferInstr = new ArrayList();

            foreach (Instruction ti in TransferInstructions) {

                RegisterSet tiOutRegs = ti.ResultRegisters;
                RegisterSet tiInRegs = (RegisterSet) ti.OperandsRegisters[0];

                if (tiOutRegs.Contains(toReg) && tiInRegs.Contains(fromReg))
                    applicableTransferInstr.Add(ti);
            }

            return applicableTransferInstr;
        }

        protected virtual void InsertTransferInstructionBefore(
            ValueNode val, Register fromReg, Register toReg,
            RegisterSet fromSet, RegisterSet toSet) {

            IList applicableTransferInstr =
                GetTransferInstructions(fromReg, toReg);

            ValueNode newVal = new RegisterValueNode(val.Datatype);
            instructionGraph.AddNode(newVal);

            if (!val.IsInputValue()) {
                InstructionNode prodInstr = val.ProducingInstruction;
                instructionGraph.RemoveEdge(prodInstr, val);
                instructionGraph.AddEdge(prodInstr, newVal);
            }

            Instruction transferInstr =
                (Instruction) applicableTransferInstr[
                    GA.Random.Next(applicableTransferInstr.Count)];

            InstructionNode transferNode =
                new InstructionNode(transferInstr);

            instructionGraph.AddNode(transferNode);
            instructionGraph.AddEdge(newVal, transferNode);
            instructionGraph.AddEdge(transferNode, val);

            instructionGraph.AssignableRegisters[val] = toSet;
            instructionGraph.AssignableRegisters[newVal] = fromSet;
        }

        protected virtual void BuildInstructionGraphForValue(
            ValueNode val, IDictionary valueProcessed,
            IDictionary valueNodeMap) {

            if (!(bool) valueProcessed[val]) {

                ValueNode iVal = (ValueNode) val.Clone();
                instructionGraph.AddNode(iVal);

                valueProcessed[val] = true;
                valueNodeMap[val] = iVal;

                if (!val.IsInputValue()) {

                    CoveringInfo ci =
                        (CoveringInfo) selection[val.ProducingOperation];

                    InstructionNode instr =
                        new InstructionNode(ci.Instruction);

                    instructionGraph.AddNode(instr);
                    instructionGraph.AddEdge(instr, iVal);

                    foreach (ValueNode opVal in ci.OperandValues) {

                        BuildInstructionGraphForValue(
                            opVal, valueProcessed, valueNodeMap);

                        ValueNode iOpVal = (ValueNode) valueNodeMap[opVal];
                        instructionGraph.AddEdge(iOpVal, instr);
                    }
                }
            }
        }

        public virtual void ComputeAssignableRegisters() {

            instructionGraph.AssignableRegisters.Clear();
            Queue registerValues = new Queue();

            foreach (ValueNode val in instructionGraph.GetValueNodes()) {
                if (val is RegisterValueNode)
                    registerValues.Enqueue(val);
            }

            while (registerValues.Count > 0) {

                ValueNode val = (ValueNode) registerValues.Dequeue();
                RegisterSet assignableRegs;

                if (ProgramGraph.PreassignedRegisters[val] != null) {
                    assignableRegs = new RegisterSet();
                    assignableRegs.Add(
                        (Register) ProgramGraph.PreassignedRegisters[val]);

                } else {
                    assignableRegs =
                        ComputeAssignableRegistersForValue(val, registerValues);
                }

                instructionGraph.AssignableRegisters[val] = assignableRegs;
            }
        }

        protected virtual RegisterSet ComputeAssignableRegistersForValue(
            ValueNode val, Queue registerValues) {

            RegisterSet assignableRegs = new RegisterSet();

            if (!val.IsInputValue()) {

                InstructionNode prodInstr = val.ProducingInstruction;

                assignableRegs = assignableRegs
                    + prodInstr.Instruction.ResultRegisters;

            } else {

                InstructionNode consInstr0 =
                    (InstructionNode) val.ConsumingInstructions[0];

                int valIndex =
                    consInstr0.OperandValues.IndexOf(val);

                assignableRegs = assignableRegs
                    + consInstr0.Instruction.OperandsRegisters[valIndex];
            }

            if (val.OutNodes.Count > 0) {

                IList consumingInstructions = new ArrayList();

                foreach (InstructionNode consInstr in val.ConsumingInstructions)
                    consumingInstructions.Add(consInstr);

                foreach (InstructionNode consInstr in consumingInstructions) {

                    int valIndex =
                        consInstr.OperandValues.IndexOf(val);

                    RegisterSet consRegs =
                        consInstr.Instruction.OperandsRegisters[valIndex];

                    if ((assignableRegs * consRegs).Count == 0) {

                        ValueNode newVal =
                            InsertTransferInstruction(
                                val, consInstr, assignableRegs);

                        registerValues.Enqueue(val);
                        registerValues.Enqueue(newVal);

                    } else {
                        assignableRegs = assignableRegs * consRegs;
                    }
                }
            }

            return assignableRegs;
        }

        protected ValueNode InsertTransferInstruction(
            ValueNode val, InstructionNode consInstr,
            RegisterSet assignableRegs) {

            ValueNode newVal = new RegisterValueNode(val.Datatype);
            instructionGraph.AddNode(newVal);
            instructionGraph.ReplaceEdgeStart(val, consInstr, newVal);

            IList applicableTransferInstr = new ArrayList();

            foreach (Instruction instr in TransferInstructions) {
                RegisterSet instrRegs = instr.OperandsRegisters[0];
                if ((assignableRegs * instrRegs).Count > 0)
                    applicableTransferInstr.Add(instr);
            }

            int selectedIndex =
                GA.Random.Next(applicableTransferInstr.Count);

            Instruction selectedInstr =
                (Instruction) applicableTransferInstr[selectedIndex];

            InstructionNode transferInstr =
                new InstructionNode(selectedInstr);

            instructionGraph.AddNode(transferInstr);
            instructionGraph.AddEdge(val, transferInstr);
            instructionGraph.AddEdge(transferInstr, newVal);

            return newVal;
        }
    }
}
