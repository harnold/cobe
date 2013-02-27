using System;
using System.Collections;
using GA;

namespace COBE {

    [Serializable]
    public class SchedulingGenomeEvaluator: IGenomeEvaluator {

        public virtual double Evaluate(IGenome genome) {

            SchedulingGenome sGenome = (SchedulingGenome) genome;

            sGenome.UpdateLifetimeIntervals();

            sGenome.ViolatedRegisterConstraints =
                GetViolatedRegisterConstraints(sGenome);
            sGenome.ViolatedSchedulingConstraints =
                GetViolatedSchedulingConstraints(sGenome);

            if (sGenome.ViolatedRegisterConstraints == 0 &&
                sGenome.ViolatedSchedulingConstraints == 0) {

                sGenome.IsValid = true;
            }

            int maxSchedulingStep = 0;

            foreach (InstructionNode instr in sGenome.InstructionNodes) {

                int instrStep = sGenome.GetInstructionInfo(instr).SchedulingStep;

                if (maxSchedulingStep < instrStep)
                    maxSchedulingStep = instrStep;
            }

            sGenome.ScheduleLength = maxSchedulingStep + 1;

            double c = sGenome.ViolatedRegisterConstraints
                     + sGenome.ViolatedSchedulingConstraints;

            int age = sGenome.Population.GA.Generation
                    - sGenome.GenerationOfBirth;

            c = (2 * c) / Math.Max(1, 2 - age);

            return (sGenome.ScheduleLength) * (1 + c*c);
        }

        protected virtual int GetViolatedRegisterConstraints(
            SchedulingGenome genome) {

            IList[] activeValues = GetActiveValues(genome);

            int violatedConstraints = 0;

            for (int step = 0; step < activeValues.Length; step++) {
                for (int i = 0; i < activeValues[step].Count - 1; i++) {

                    ValueNode vi = (ValueNode) activeValues[step][i];
                    Register  ri = genome.GetValueInfo(vi).Register;

                    for (int j = i + 1; j < activeValues[step].Count; j++) {

                        ValueNode vj = (ValueNode) activeValues[step][j];
                        Register rj = genome.GetValueInfo(vj).Register;

                        if (ri == rj)
                            violatedConstraints += 1;
                    }
                }
            }

            return violatedConstraints;
        }

        protected virtual IList[] GetActiveValues(SchedulingGenome genome) {

            IList[] activeValues = new IList[genome.Schedule.GetLength(0)];

            for (int i = 0; i < genome.Schedule.GetLength(0); i++) {

                activeValues[i] = new ArrayList();

                foreach (ValueNode rValue in genome.RegisterValues) {

                    int production = genome.GetValueInfo(rValue).Production;
                    int lastUsage  = genome.GetValueInfo(rValue).LastUsage;

                    if (production <= i && i <= lastUsage)
                        activeValues[i].Add(rValue);
                }
            }

            return activeValues;
        }

        protected virtual int GetViolatedSchedulingConstraints(
            SchedulingGenome genome) {

            int violatedConstraints = 0;

            foreach (InstructionNode instr in genome.InstructionNodes) {

                int instrStep = genome.GetInstructionInfo(instr).SchedulingStep;
                IList instrDependencies = (IList) genome.Dependencies[instr];

                foreach (InstructionNode depInstr in instrDependencies) {

                    int depInstrStep =
                        genome.GetInstructionInfo(depInstr).SchedulingStep;

                    if (instrStep <= depInstrStep)
                        violatedConstraints += 1;
                }

                /*
                foreach (ValueNode depVal in instr.OperandValues) {

                    if (!depVal.IsInputValue()) {

                        InstructionNode depInstr =
                            depVal.ProducingInstruction;

                        int instrStep =
                            genome.GetInstructionInfo(instr).SchedulingStep;
                        int depInstrStep =
                            genome.GetInstructionInfo(depInstr).SchedulingStep;

                        if (depInstrStep >= instrStep)
                            violatedConstraints += depInstrStep - instrStep + 1;
                    }
                }
                */
            }

            return violatedConstraints;
        }
    }
}

