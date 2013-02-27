using System;
using System.Collections;
using GA;

namespace COBE {

    [Serializable]
    public class SchedulingGenome: IGenome {

        [Serializable]
        public class ValueInfo {
            public Register Register;
            public int Production;
            public int LastUsage;
        }

        [Serializable]
        public class InstructionInfo {
            public int SchedulingStep;
        }

        private InstructionNode[,] schedule;
        private IDictionary instructionInfos;
        private IDictionary valueInfos;

        private bool isValid;
        private int scheduleLength;
        private int violatedSchedulingConstraints;
        private int violatedRegisterConstraints;

        private int generationOfBirth;

        private SchedulingGenomePopulation population;
        private GAScheduler scheduler;

        public SchedulingGenomePopulation Population {
            get { return population; }
            set { population = value; }
        }

        public InstructionNode[,] Schedule {
            get { return schedule; }
        }

        public IDictionary InstructionInfos {
            get { return instructionInfos; }
        }

        public IDictionary ValueInfos {
            get { return valueInfos; }
        }

        public int GenerationOfBirth {
            get { return generationOfBirth; }
            set { generationOfBirth = value; }
        }

        public bool IsValid {
            get { return isValid; }
            set { isValid = value; }
        }

        public int ScheduleLength {
            get { return scheduleLength; }
            set { scheduleLength = value; }
        }

        public int ViolatedRegisterConstraints {
            get { return violatedRegisterConstraints; }
            set { violatedRegisterConstraints = value; }
        }

        public int ViolatedSchedulingConstraints {
            get { return violatedSchedulingConstraints; }
            set { violatedSchedulingConstraints = value; }
        }

        public InstructionGraph InstructionGraph {
            get { return scheduler.InstructionGraph; }
        }

        public IList InstructionNodes {
            get { return scheduler.InstructionNodes; }
        }

        public IList[] InstructionsOnExUnit {
            get { return scheduler.InstructionsOnExUnit; }
        }

        public IDictionary Dependencies {
            get { return scheduler.Dependencies; }
        }

        public IList RegisterValues {
            get { return scheduler.RegisterValues; }
        }

        public MachineDescription MachineDescription {
            get { return scheduler.MachineDescription; }
        }

        public IDictionary CyclicDependencies {
            get { return InstructionGraph.CyclicDependencies; }
        }

        public SchedulingGenome(SchedulingGenomePopulation population) {

            this.population = population;
            this.scheduler = population.Scheduler;

            int maxSchedulingSteps = InstructionNodes.Count;

            schedule = new InstructionNode[
                maxSchedulingSteps, MachineDescription.ExecutionUnits];

            instructionInfos = new Hashtable(InstructionNodes.Count);

            foreach (InstructionNode iNode in InstructionNodes)
                instructionInfos[iNode] = new InstructionInfo();

            valueInfos = new Hashtable(RegisterValues.Count);

            foreach (ValueNode vNode in RegisterValues)
                valueInfos[vNode] = new ValueInfo();

            isValid = false;

            generationOfBirth = population.GA.Generation;
        }

        public virtual RegisterAssignment GetRegisterAssignment() {

            RegisterAssignment registerAssignment =
                new RegisterAssignment();

            foreach (ValueNode rValue in RegisterValues)
                registerAssignment[rValue] = GetValueInfo(rValue).Register;

            return registerAssignment;
        }

        public virtual InstructionSchedule GetInstructionSchedule() {

            InstructionSchedule instructionSchedule =
                new InstructionSchedule();

            for (int i = 0; i < ScheduleLength; i++) {

                InstructionNode[] step = new InstructionNode[
                    MachineDescription.ExecutionUnits];

                for (int j = 0; j < step.Length; j++)
                    step[j] = Schedule[i, j];

                instructionSchedule.Instructions.Add(step);
            }

            return instructionSchedule;
        }

        public ValueInfo GetValueInfo(ValueNode v) {
            return (ValueInfo) valueInfos[v];
        }

        public InstructionInfo GetInstructionInfo(InstructionNode i) {
            return (InstructionInfo) instructionInfos[i];
        }

        public virtual void Initialize() {
            InitializeSchedule();
            InitializeRegisters();
        }

        public virtual void UpdateLifetimeIntervals() {

            foreach (ValueInfo vi in valueInfos.Values) {
                vi.Production = -1;
                vi.LastUsage = -1;
            }

            foreach (InstructionNode iNode in InstructionNodes) {

                int schedulingStep = GetInstructionInfo(iNode).SchedulingStep;

                if (iNode.ResultValue is RegisterValueNode) {
                    ValueInfo vi  = GetValueInfo(iNode.ResultValue);
                    vi.Production = schedulingStep;
                }

                foreach (ValueNode vNode in iNode.OperandValues) {
                    if (vNode is RegisterValueNode) {
                        ValueInfo vi = GetValueInfo(vNode);
                        if (vi.LastUsage < schedulingStep - 1)
                            vi.LastUsage = schedulingStep - 1;
                    }
                }
            }

            foreach (ValueNode val in InstructionGraph.OutputValues) {
                if (val is RegisterValueNode) {
                    ValueInfo vi = GetValueInfo(val);
                    vi.LastUsage = schedule.GetLength(0);
                }
            }
        }

        protected virtual void InitializeRegisters() {

            foreach (ValueNode val in RegisterValues) {

                ValueInfo vi = GetValueInfo(val);

                if (vi.Register == null) {

                    RegisterSet assignableRegisters =
                        (RegisterSet) InstructionGraph.AssignableRegisters[val];

                    int regIndex = GA.Random.Next(assignableRegisters.Count);
                    vi.Register = (Register) assignableRegisters[regIndex];

                    ValueNode depVal = (ValueNode) CyclicDependencies[val];

                    if (depVal != null) {
                        ValueInfo depVi = GetValueInfo(depVal);
                        depVi.Register = vi.Register;
                    }
                }
            }
        }

        protected virtual void InitializeSchedule() {

            if (scheduler.Configuration.RandomInitialization) {

                int[] index = new int[schedule.GetLength(1)];

                foreach (InstructionNode iNode in InstructionNodes) {
                    int exUnit = iNode.Instruction.ExecutionUnit;
                    schedule[index[exUnit], exUnit] = iNode;
                    GetInstructionInfo(iNode).SchedulingStep = index[exUnit];
                    index[exUnit]++;
                }

                for (int i = 0; i < 10 * InstructionNodes.Count; i++) {
                    int eu = GA.Random.Next(schedule.GetLength(1));
                    int t1 = GA.Random.Next(schedule.GetLength(0));
                    int t2 = GA.Random.Next(schedule.GetLength(0));
                    SwapInstructions(t1, t2, eu);
                }

            } else {

                IDictionary instrScheduled = new Hashtable();

                foreach (InstructionNode instr in InstructionNodes)
                    instrScheduled[instr] = false;

                int t = 0;

                while (t < InstructionNodes.Count) {

                    IList readyInstr = GetReadyInstructions(instrScheduled);

                    InstructionNode instr =
                        (InstructionNode)
                        readyInstr[GA.Random.Next(readyInstr.Count)];

                    int eu = instr.Instruction.ExecutionUnit;

                    schedule[t, eu] = instr;
                    GetInstructionInfo(instr).SchedulingStep = t;
                    instrScheduled[instr] = true;

                    t++;
                }
            }
        }

        protected IList GetReadyInstructions(IDictionary instrScheduled) {

            IList readyInstr = new ArrayList();

            foreach (InstructionNode instr in InstructionNodes) {

                if (!(bool) instrScheduled[instr]) {

                    bool allOperandsScheduled = true;

                    foreach (ValueNode opVal in instr.OperandValues) {

                        if (!opVal.IsInputValue()) {

                            InstructionNode opInstr = opVal.ProducingInstruction;
                            bool opScheduled = (bool) instrScheduled[opInstr];

                            if (!opScheduled) {
                                allOperandsScheduled = false;
                                break;
                            }
                        }
                    }

                    if (allOperandsScheduled)
                        readyInstr.Add(instr);
                }
            }

            return readyInstr;
        }

        public virtual void ScheduleInstructionForStep(
            InstructionNode iNode, int step) {

            schedule[ step, iNode.Instruction.ExecutionUnit ] = iNode;
            GetInstructionInfo(iNode).SchedulingStep = step;
        }

        public virtual void SwapInstructions(int t1, int t2, int eu) {

            InstructionNode i1 = schedule[t1, eu];
            InstructionNode i2 = schedule[t2, eu];

            schedule[t1, eu] = i2;
            schedule[t2, eu] = i1;

            if (i1 != null)
                GetInstructionInfo(i1).SchedulingStep = t2;

            if (i2 != null)
                GetInstructionInfo(i2).SchedulingStep = t1;
        }

        public virtual double DifferenceTo(IGenome otherGenome) {

            SchedulingGenome other = (SchedulingGenome) otherGenome;

            double d = 0.0;

            foreach (InstructionNode iNode in InstructionNodes) {

                InstructionInfo info = GetInstructionInfo(iNode);
                InstructionInfo otherInfo = other.GetInstructionInfo(iNode);

                if (info.SchedulingStep != otherInfo.SchedulingStep)
                    d += 1.0;
            }

            foreach (ValueNode vNode in RegisterValues) {

                ValueInfo info = GetValueInfo(vNode);
                ValueInfo otherInfo = other.GetValueInfo(vNode);

                if (info.Register != otherInfo.Register)
                    d += 1.0;
            }

            return d / (InstructionNodes.Count + RegisterValues.Count);
        }

        public virtual void Copy(IGenome otherGenome) {

            SchedulingGenome other = (SchedulingGenome) otherGenome;

            population = other.population;
            scheduler = other.scheduler;

            schedule = new InstructionNode[
                other.schedule.GetLength(0),
                other.schedule.GetLength(1)];

            for (int i = 0; i < schedule.GetLength(0); i++)
                for (int j = 0; j < schedule.GetLength(1); j++)
                    schedule[i, j] = other.schedule[i, j];

            valueInfos = new Hashtable();

            foreach (ValueNode vNode in other.ValueInfos.Keys) {

                ValueInfo info = new ValueInfo();
                ValueInfo otherInfo = other.GetValueInfo(vNode);

                info.Register = otherInfo.Register;
                info.Production = otherInfo.Production;
                info.LastUsage = otherInfo.LastUsage;
                valueInfos[vNode] = info;
            }

            instructionInfos = new Hashtable();

            foreach (InstructionNode iNode in other.InstructionInfos.Keys) {

                InstructionInfo info = new InstructionInfo();
                InstructionInfo otherInfo = other.GetInstructionInfo(iNode);

                info.SchedulingStep = otherInfo.SchedulingStep;
                instructionInfos[iNode] = info;
            }

            isValid = other.isValid;
            scheduleLength = other.scheduleLength;
            violatedSchedulingConstraints = other.violatedSchedulingConstraints;
            violatedRegisterConstraints = other.violatedRegisterConstraints;
            generationOfBirth = other.generationOfBirth;
        }

        public virtual void Copy(
            SchedulingGenome other,
            IDictionary instructionMap,
            IDictionary valueMap) {

            population = other.population;
            scheduler = other.scheduler;

            schedule = new InstructionNode[
                other.schedule.GetLength(0),
                other.schedule.GetLength(1)];

            for (int i = 0; i < schedule.GetLength(0); i++) {
                for (int j = 0; j < schedule.GetLength(1); j++) {

                    InstructionNode iNode = other.schedule[i, j];

                    if (iNode != null) {
                        schedule[i, j] =
                            (InstructionNode) instructionMap[other.schedule[i, j]];
                    } else {
                        schedule[i, j] = null;
                    }
                }
            }

            valueInfos = new Hashtable();

            foreach (ValueNode vNode in other.ValueInfos.Keys) {

                ValueInfo info = new ValueInfo();
                ValueInfo otherInfo = other.GetValueInfo(vNode);

                info.Register = otherInfo.Register;
                info.Production = otherInfo.Production;
                info.LastUsage = otherInfo.LastUsage;
                valueInfos[valueMap[vNode]] = info;
            }

            instructionInfos = new Hashtable();

            foreach (InstructionNode iNode in other.InstructionInfos.Keys) {

                InstructionInfo info = new InstructionInfo();
                InstructionInfo otherInfo = other.GetInstructionInfo(iNode);

                info.SchedulingStep = otherInfo.SchedulingStep;
                instructionInfos[instructionMap[iNode]] = info;
            }

            isValid = other.isValid;
            scheduleLength = other.scheduleLength;
            violatedSchedulingConstraints = other.violatedSchedulingConstraints;
            violatedRegisterConstraints = other.violatedRegisterConstraints;
            generationOfBirth = other.generationOfBirth;
        }

        public virtual object Clone() {
            SchedulingGenome clone = new SchedulingGenome();
            clone.Copy(this);
            return clone;
        }

        protected SchedulingGenome() {
        }

        public virtual SchedulingGenome Clone(
            IDictionary instructionMap, IDictionary valueMap) {

            SchedulingGenome clone = new SchedulingGenome();
            clone.Copy(this, instructionMap, valueMap);
            return clone;
        }
    }
}
