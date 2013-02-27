using System;
using System.Collections;
using System.IO;
using COBE.Samples;

namespace COBE.Tester {

    public class GACodeGeneratorTester {

        private MachineDescription machineDescription;
        private ProgramGraph programGraph;

        private InstructionGraph instructionGraph;
        private RegisterAssignment registerAssignment;
        private InstructionSchedule schedule;

        private GACodeGenerator codeGenerator;
        private GAInstructionSelector iSelector;
        private GAScheduler scheduler;

        private TextWriter textWriter;
        private TextWriter dataWriter;

        private int schedulerDumpsEvery = 100;
        private int selectorDumpsEvery  = 1;

        public int SchedulerDumpsEvery {
            get { return schedulerDumpsEvery; }
            set { schedulerDumpsEvery = value; }
        }

        public int SelectorDumpsEvery {
            get { return selectorDumpsEvery; }
            set { selectorDumpsEvery = value; }
        }

        public GACodeGenerator CodeGenerator {
            get { return codeGenerator; }
            set { codeGenerator = value; }
        }

        public GACodeGeneratorTester(
            IMachineDescriptionProvider mdp,
            IProgramGraphProvider pgp, int k,
            GAInstructionSelectorConfiguration isConfig,
            GASchedulerConfiguration scConfig,
            TextWriter textWriter,
            TextWriter dataWriter) {

            machineDescription = mdp.CreateMachineDescription();
            programGraph = pgp.CreateUnrolledProgramGraph(k);

            codeGenerator = new GACodeGenerator(isConfig, scConfig);
            iSelector = codeGenerator.InstructionSelector;
            scheduler = codeGenerator.Scheduler;

            this.textWriter = textWriter;
            this.dataWriter = dataWriter;
        }

        public void Run() {

            codeGenerator.InstructionSelector.Initialized +=
                new GAInstructionSelector.EventHandler(GenerationComputed);

            codeGenerator.InstructionSelector.GenerationComputed +=
                new GAInstructionSelector.EventHandler(GenerationComputed);

            codeGenerator.Scheduler.GenerationComputed +=
                new GAScheduler.EventHandler(GenerationComputed);

            DateTime t1 = DateTime.Now;

            codeGenerator.Initialize(machineDescription, programGraph);

            bool result = codeGenerator.Optimize(out instructionGraph,
                                                 out registerAssignment,
                                                 out schedule);

            DateTime t2 = DateTime.Now;

            textWriter.WriteLine("Optimization time: " + (t2 - t1));
            textWriter.WriteLine();

            textWriter.WriteLine("Instruction selector configuration: ");
            textWriter.WriteLine();
            textWriter.WriteLine(iSelector.Configuration);
            textWriter.WriteLine();
            textWriter.WriteLine("Instruction scheduler configuration: ");
            textWriter.WriteLine();
            textWriter.WriteLine(scheduler.Configuration);
            textWriter.WriteLine();

            PrintSelectorStatistics();

            textWriter.WriteLine();

            if (result == true) {
                PrintSchedule();
                PrintRegisterAssignment();
            } else {
                textWriter.WriteLine(
                "No solution found.");
            }
        }

        protected void PrintRegisterAssignment() {

            textWriter.WriteLine();

            foreach (ValueNode vNode in instructionGraph.GetValueNodes()) {
                if (vNode is TestRegisterNode) {
                    TestRegisterNode testNode = (TestRegisterNode) vNode;
                    Register reg = registerAssignment[testNode];
                    textWriter.WriteLine(testNode.Id + ": " + reg.Mnemonic);
                }
            }
        }

        protected void GenerationComputed(GAInstructionSelector iSelector) {
            if (iSelector.GA.Generation % selectorDumpsEvery == 0)
                PrintSelectorStatistics();
        }

        protected void GenerationComputed(GAScheduler scheduler) {
            if (scheduler.GA.Generation % schedulerDumpsEvery == 0)
                PrintSchedulerStatistics();
        }

        protected void PrintSchedulerStatistics() {

            textWriter.WriteLine("Scheduler Generation " + scheduler.GA.Generation + ":");
            textWriter.WriteLine();

            SchedulingGenomePopulation pop = scheduler.Population;

            textWriter.WriteLine("Population.ObjectiveMin    = " + pop.ObjectiveMin);
            textWriter.WriteLine("Population.ObjectiveMax    = " + pop.ObjectiveMax);
            textWriter.WriteLine("Population.ObjectiveMean   = " + pop.ObjectiveMean);
            textWriter.WriteLine("Population.ObjectiveVar    = " + pop.ObjectiveVariance);
            textWriter.WriteLine("Population.GenomeDiversity = " + pop.GenomeDiversity);
            textWriter.WriteLine();
            textWriter.WriteLine("BestGenome.IsValid = " + scheduler.BestGenome.IsValid);
            textWriter.WriteLine("BestGenome.ViolatedRegisterConstraints   = " + scheduler.BestGenome.ViolatedRegisterConstraints);
            textWriter.WriteLine("BestGenome.ViolatedSchedulingConstraints = " + scheduler.BestGenome.ViolatedSchedulingConstraints);
            textWriter.WriteLine();

            dataWriter.WriteLine(
                "SC" + "; " +
                scheduler.GA.Generation + "; " +
                pop.ObjectiveMin + "; " +
                pop.ObjectiveMax + "; " +
                pop.ObjectiveMean + "; " +
                pop.ObjectiveVariance + "; " +
                pop.GenomeDiversity + "; " +
                scheduler.BestGenome.IsValid + "; " +
                scheduler.BestGenome.ViolatedRegisterConstraints + "; " +
                scheduler.BestGenome.ViolatedSchedulingConstraints);
        }

        protected void PrintSelectorStatistics() {

            textWriter.WriteLine("Instruction Selector Generation " + iSelector.GA.Generation + ":");
            textWriter.WriteLine();

            SelectionGenomePopulation pop = iSelector.Population;

            textWriter.WriteLine("Population.ObjectiveMin    = " + pop.ObjectiveMin);
            textWriter.WriteLine("Population.ObjectiveMax    = " + pop.ObjectiveMax);
            textWriter.WriteLine("Population.ObjectiveMean   = " + pop.ObjectiveMean);
            textWriter.WriteLine("Population.ObjectiveVar    = " + pop.ObjectiveVariance);
            textWriter.WriteLine("Population.GenomeDiversity = " + pop.GenomeDiversity);
            textWriter.WriteLine();
            textWriter.WriteLine("BestGenome.IsValid = " + iSelector.BestGenome.IsValid);
            textWriter.WriteLine();

            dataWriter.WriteLine(
                "IS" + "; " +
                iSelector.GA.Generation + "; " +
                pop.ObjectiveMin + "; " +
                pop.ObjectiveMax + "; " +
                pop.ObjectiveMean + "; " +
                pop.ObjectiveVariance + "; " +
                pop.GenomeDiversity + "; " +
                iSelector.BestGenome.IsValid);
        }

        protected void PrintSchedule() {

            AssemblyGenerator asmg = new AssemblyGenerator(registerAssignment);

            for (int i = 0; i < schedule.Steps; i++) {

                textWriter.Write("[" + i + "] ");

                InstructionNode[] istep = schedule[i];

                for (int j = 0; j < istep.Length; j++) {

                    InstructionNode inode = istep[j];

                    if (inode != null) {
                        textWriter.Write(asmg.GetAssembly(inode));
                        textWriter.Write(" | ");
                    } else {
                        textWriter.Write(" | ");
                    }
                }

                textWriter.WriteLine();
            }

            textWriter.WriteLine();
        }
    }
}
