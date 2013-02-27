using System;
using System.Collections;
using GA;

namespace COBE {

    [Serializable]
    public class SchedulingGenomeMutation: IMutationOperator {

        private double pRegisterMutation;
        private double pScheduleCompaction;
        private double pScheduleMutation;
        private double pScheduleCrossSwap;

        public double PScheduleMutation {
            get { return pScheduleMutation; }
            set { pScheduleMutation = value; }
        }

        public double PRegisterMutation {
            get { return pRegisterMutation; }
            set { pRegisterMutation = value; }
        }

        public double PScheduleCompaction {
            get { return pScheduleCompaction; }
            set { pScheduleCompaction = value; }
        }

        public double PScheduleCrossSwap {
            get { return pScheduleCrossSwap; }
            set { pScheduleCrossSwap = value; }
        }

        public virtual void Mutate(IGenome genome, double pMutation) {

            SchedulingGenome sGenome = (SchedulingGenome) genome;

            bool mutated = false;

            if (GA.Random.NextDouble() < pScheduleMutation) {
                ScheduleMutation(sGenome);
                mutated = true;
            }

            if (GA.Random.NextDouble() < pRegisterMutation) {
                RegisterMutation(sGenome);
                mutated = true;
            }

            if (GA.Random.NextDouble() < pScheduleCrossSwap) {
                ScheduleCrossSwap(sGenome);
                mutated = true;
            }

            if (GA.Random.NextDouble() < pScheduleCompaction) {
                ScheduleCompaction(sGenome);
                mutated = true;
            }

            if (mutated)
                sGenome.GenerationOfBirth = sGenome.Population.GA.Generation;
        }

        public virtual void ScheduleCrossSwap(SchedulingGenome genome) {

            int nMut = (int) (Math.Abs(GA.Random.NextGaussian())
                              * (genome.InstructionNodes.Count - 1) / 3 + 1);

            nMut = Math.Min(nMut, genome.InstructionNodes.Count);

            for (int i = 0; i < nMut; i++) {

                int instrIndex1 =
                    GA.Random.Next(genome.InstructionNodes.Count);

                InstructionNode instr1 = (InstructionNode)
                    genome.InstructionNodes[instrIndex1];

                int instrIndex2 =
                    GA.Random.Next(genome.InstructionNodes.Count);

                InstructionNode instr2 = (InstructionNode)
                    genome.InstructionNodes[instrIndex2];

                IList instrDependencies1 =
                    (IList) genome.Dependencies[instr1];

                IList instrDependencies2 =
                    (IList) genome.Dependencies[instr2];

                int t1 = genome.GetInstructionInfo(instr1).SchedulingStep;
                int t2 = genome.GetInstructionInfo(instr2).SchedulingStep;

                if ((instrDependencies1.Contains(instr2) && t1 <= t2) ||
                    (instrDependencies2.Contains(instr1) && t2 <= t1)) {

                    int eu1 = instr1.Instruction.ExecutionUnit;
                    int eu2 = instr2.Instruction.ExecutionUnit;

                    if (eu1 != eu2) {
                        genome.SwapInstructions(t1, t2, eu1);
                        genome.SwapInstructions(t1, t2, eu2);
                    } else {
                        genome.SwapInstructions(t1, t2, eu1);
                    }
                }
            }
        }

        public virtual void ScheduleMutation(SchedulingGenome genome) {

            int nMut = (int) (Math.Abs(GA.Random.NextGaussian())
                              * (genome.InstructionNodes.Count - 1) / 3 + 1);

            nMut = Math.Min(nMut, genome.InstructionNodes.Count);

            for (int i = 0; i < nMut; i++) {

                int iNodeIndex =
                    GA.Random.Next(genome.InstructionNodes.Count);

                InstructionNode iNode =
                    (InstructionNode) genome.InstructionNodes[iNodeIndex];

                int exUnit = iNode.Instruction.ExecutionUnit;
                int t1 = genome.GetInstructionInfo(iNode).SchedulingStep;
                int t2 = GA.Random.Next(genome.Schedule.GetLength(0));

                genome.SwapInstructions(t1, t2, exUnit);
            }
        }

        public virtual void RegisterMutation(SchedulingGenome genome) {

            int nMut = (int) (Math.Abs(GA.Random.NextGaussian())
                              * (genome.RegisterValues.Count- 1) / 3 + 1);

            nMut = Math.Min(nMut, genome.RegisterValues.Count);

            for (int i = 0; i < nMut; i++) {

                int valIndex =
                    GA.Random.Next(genome.RegisterValues.Count);

                ValueNode val =
                    (ValueNode) genome.RegisterValues[valIndex];

                RegisterSet assignableRegs =
                    (RegisterSet) genome.InstructionGraph.AssignableRegisters[val];

                int regIndex = GA.Random.Next(assignableRegs.Count);

                genome.GetValueInfo(val).Register =
                    (Register) assignableRegs[regIndex];

                ValueNode depVal = (ValueNode) genome.CyclicDependencies[val];

                if (depVal != null) {
                    genome.GetValueInfo(depVal).Register =
                        (Register) assignableRegs[regIndex];
                }
            }
        }

        public virtual void ScheduleCompaction(SchedulingGenome genome) {

            int nMut = (int) (Math.Abs(GA.Random.NextGaussian())
                              * (genome.InstructionNodes.Count - 1) / 3 + 1);

            nMut = Math.Min(nMut, genome.InstructionNodes.Count);

            for (int i = 0; i < nMut; i++) {

                int iNodeIndex =
                    GA.Random.Next(genome.InstructionNodes.Count);

                InstructionNode iNode =
                    (InstructionNode) genome.InstructionNodes[iNodeIndex];

                int t = genome.GetInstructionInfo(iNode).SchedulingStep;

                if (t > 0) {

                    int exUnit = iNode.Instruction.ExecutionUnit;

                    if (genome.Schedule[t - 1, exUnit] == null)
                        genome.SwapInstructions(t, t - 1, exUnit);
                }
            }
        }
    }
}
