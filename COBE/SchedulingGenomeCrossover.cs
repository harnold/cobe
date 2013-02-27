using System;
using System.Collections;
using GA;

namespace COBE {

    [Serializable]
    public class SchedulingGenomeCrossover: ICrossoverOperator {

        public virtual void Cross(IGenome genome1, IGenome genome2) {

            SchedulingGenome sGenome1 = (SchedulingGenome) genome1;
            SchedulingGenome sGenome2 = (SchedulingGenome) genome2;

            PositionExchangeCrossover(sGenome1, sGenome2);

            sGenome1.GenerationOfBirth = sGenome1.Population.GA.Generation;
            sGenome2.GenerationOfBirth = sGenome2.Population.GA.Generation;
        }

        protected virtual void RegisterCrossover(
            SchedulingGenome genome1, SchedulingGenome genome2) {

            int nCross = (int) (GA.Random.NextGaussian()
                                * (genome1.RegisterValues.Count / 6)
                                + (genome1.RegisterValues.Count / 2));

            nCross = Math.Min(nCross, genome1.RegisterValues.Count);
            nCross = Math.Max(nCross, 0);

            for (int i = 0; i < nCross; i++) {

                ValueNode val = (ValueNode) genome1.RegisterValues[
                    GA.Random.Next(genome1.RegisterValues.Count)];

                Register r1 = genome1.GetValueInfo(val).Register;
                Register r2 = genome2.GetValueInfo(val).Register;

                genome1.GetValueInfo(val).Register = r2;
                genome2.GetValueInfo(val).Register = r1;

                ValueNode depVal = (ValueNode) genome1.CyclicDependencies[val];

                if (depVal != null) {
                    genome1.GetValueInfo(depVal).Register = r2;
                    genome2.GetValueInfo(depVal).Register = r1;
                }
            }
        }

        protected virtual void ExecutionUnitExchangeCrossover(
            SchedulingGenome genome1, SchedulingGenome genome2) {

            int executionUnits = genome1.MachineDescription.ExecutionUnits;

            int nCross = (int) (GA.Random.NextGaussian()
                                * (executionUnits / 6)
                                + (executionUnits / 2));

            nCross = Math.Min(nCross, executionUnits);
            nCross = Math.Max(nCross, 0);

            for (int i = 0; i < nCross; i++) {

                int exUnit = GA.Random.Next(
                    genome1.MachineDescription.ExecutionUnits);

                for (int j = 0; j < genome1.Schedule.GetLength(0); j++) {

                    InstructionNode i1 = genome1.Schedule[j, i];
                    InstructionNode i2 = genome2.Schedule[j, i];

                    genome1.Schedule[j, i] = i2;
                    genome2.Schedule[j, i] = i1;
                }
            }

        }

        protected virtual void RelativeOrderCrossoverForExUnit(
            SchedulingGenome genome1, SchedulingGenome genome2,
            int exUnit, IList instructionsOnExUnit) {

            int nCross = (int) (GA.Random.NextGaussian()
                                * (instructionsOnExUnit.Count / 6)
                                + (instructionsOnExUnit.Count / 2));

            nCross = Math.Min(nCross, instructionsOnExUnit.Count);
            nCross = Math.Max(nCross, 0);

            InstructionNode[] selectedInstr1 = new InstructionNode[nCross];
            InstructionNode[] selectedInstr2 = new InstructionNode[nCross];

            int[] instrSteps1 = new int[nCross];
            int[] instrSteps2 = new int[nCross];

            for (int i = 0; i < nCross; i++) {

                int selectedInstrIndex =
                    GA.Random.Next(instructionsOnExUnit.Count);

                InstructionNode selectedInstr =
                    (InstructionNode) instructionsOnExUnit[selectedInstrIndex];

                selectedInstr1[i] = selectedInstr;
                selectedInstr2[i] = selectedInstr;

                instrSteps1[i] = genome1.GetInstructionInfo(
                    selectedInstr).SchedulingStep;

                instrSteps2[i] = genome2.GetInstructionInfo(
                    selectedInstr).SchedulingStep;
            }

            Array.Sort(instrSteps1, selectedInstr2);
            Array.Sort(instrSteps2, selectedInstr1);

            for (int i = 0; i < nCross; i++) {
                genome1.ScheduleInstructionForStep(
                    selectedInstr1[i], instrSteps1[i]);
                genome2.ScheduleInstructionForStep(
                    selectedInstr2[i], instrSteps2[i]);
            }
        }

        protected virtual void RelativeOrderCrossover(
            SchedulingGenome genome1, SchedulingGenome genome2) {

            for (int i = 0; i < genome1.MachineDescription.ExecutionUnits; i++) {

                IList instructionsOnExUnit = genome1.InstructionsOnExUnit[i];

                if (instructionsOnExUnit.Count > 0) {
                    RelativeOrderCrossoverForExUnit(
                        genome1, genome2, i, instructionsOnExUnit);
                }
            }
        }

        protected virtual void PositionExchangeCrossover(
            SchedulingGenome genome1, SchedulingGenome genome2) {

            int nCross = (int) (GA.Random.NextGaussian()
                                * (genome1.InstructionNodes.Count / 6)
                                + (genome1.InstructionNodes.Count / 2));

            nCross = Math.Min(nCross, genome1.InstructionNodes.Count);
            nCross = Math.Max(nCross, 0);

            for (int i = 0; i < nCross; i++) {

                int iNodeIndex =
                    GA.Random.Next(genome1.InstructionNodes.Count);

                InstructionNode iNode =
                    (InstructionNode) genome1.InstructionNodes[iNodeIndex];

                int eu = iNode.Instruction.ExecutionUnit;
                int t1 = genome1.GetInstructionInfo(iNode).SchedulingStep;
                int t2 = genome2.GetInstructionInfo(iNode).SchedulingStep;

                genome1.SwapInstructions(t1, t2, eu);
                genome2.SwapInstructions(t1, t2, eu);

                if (iNode.ResultValue is RegisterValueNode) {

                    Register r1 = genome1.GetValueInfo(iNode.ResultValue).Register;
                    Register r2 = genome2.GetValueInfo(iNode.ResultValue).Register;

                    genome1.GetValueInfo(iNode.ResultValue).Register = r2;
                    genome2.GetValueInfo(iNode.ResultValue).Register = r1;

                    ValueNode depVal =
                        (ValueNode) genome1.CyclicDependencies[iNode.ResultValue];

                    if (depVal != null) {

                        Register dr1 = genome1.GetValueInfo(depVal).Register;
                        Register dr2 = genome2.GetValueInfo(depVal).Register;

                        genome1.GetValueInfo(depVal).Register = dr2;
                        genome2.GetValueInfo(depVal).Register = dr1;
                    }
                }
            }
        }
    }
}
