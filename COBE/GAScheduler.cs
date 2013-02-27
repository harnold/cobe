using System;
using System.Collections;
using System.IO;
using GA;

namespace COBE {

    [Serializable]
    public class GASchedulerConfiguration {

        public int PopulationSize = 50;
        public double ScalingFactor = 2.0;
        public int MaxGenerations = 1000;
        public int MonitoredGenerations = 100;
        public bool RandomInitialization = false;

        public double PCrossover = 0.5;
        public double PScheduleCrossSwap = 0.5;
        public double PScheduleCompaction = 0.2;
        public double PRegisterMutation = 0.2;
        public double PScheduleMutation = 0.2;

        public bool ComputeObjectiveStatistics = true;
        public bool ComputeFitnessStatistics = false;
        public bool ComputeGenomeDiversity = false;

        public override string ToString() {

            StringWriter s = new StringWriter();

            s.WriteLine("PopulationSize       = " + PopulationSize);
            s.WriteLine("ScalingFactor        = " + ScalingFactor);
            s.WriteLine("MaxGenerations       = " + MaxGenerations);
            s.WriteLine("MonitoredGenerations = " + MonitoredGenerations);
            s.WriteLine("RandomInitialization = " + RandomInitialization);
            s.WriteLine("PCrossover           = " + PCrossover);
            s.WriteLine("PScheduleCrossSwap   = " + PScheduleCrossSwap);
            s.WriteLine("PScheduleCompaction  = " + PScheduleCompaction);
            s.WriteLine("PRegisterMutation    = " + PRegisterMutation);
            s.WriteLine("PScheduleMutation    = " + PScheduleMutation);

            return s.ToString();
        }
    }

    [Serializable]
    public class GAScheduler {

        public delegate void EventHandler(GAScheduler sender);

        public event EventHandler GenerationComputed;
        public event EventHandler Initialized;
        public event EventHandler Done;

        private MachineDescription machineDescription;
        private InstructionGraph instructionGraph;

        private IList instructionNodes;
        private IList[] instructionsOnExUnit;
        private IList valueNodes;
        private IList registerValues;

        private SchedulingGenomePopulation population;
        private SimpleGA ga;

        private double[] bestObjectiveMonitor;

        private IDictionary dependencies;

        private GACodeGenerator codeGenerator;
        private GASchedulerConfiguration configuration;

        public GASchedulerConfiguration Configuration {
            get { return configuration; }
        }

        public IDictionary Dependencies {
            get { return dependencies; }
        }

        public SchedulingGenome BestGenome {
            get { return (SchedulingGenome) BestIndividual.Genome; }
        }

        public Individual BestIndividual {
            get { return population.Individuals[0]; }
        }

        public int PopulationSize {
            get { return configuration.PopulationSize; }
            set { configuration.PopulationSize = value; }
        }

        public double ScalingFactor {
            get { return configuration.ScalingFactor; }
            set { configuration.ScalingFactor = value; }
        }

        public int MaxGenerations {
            get { return configuration.MaxGenerations; }
            set { configuration.MaxGenerations = value; }
        }

        public int MonitoredGenerations {
            get { return configuration.MonitoredGenerations; }
            set { configuration.MonitoredGenerations = value; }
        }

        public double PCrossover {
            get { return configuration.PCrossover; }
            set {
                configuration.PCrossover = value;
                ga.PCrossover = value;
            }
        }

        public double PScheduleCrossSwap {
            get { return configuration.PScheduleCrossSwap; }
            set {
                configuration.PScheduleCrossSwap = value;
                SchedulingGenomeMutation sgm =
                    (SchedulingGenomeMutation) population.MutationOperator;
                sgm.PScheduleCrossSwap = value;
            }
        }

        public double PScheduleCompaction {
            get { return configuration.PScheduleCompaction; }
            set {
                configuration.PScheduleCompaction = value;
                SchedulingGenomeMutation sgm =
                    (SchedulingGenomeMutation) population.MutationOperator;
                sgm.PScheduleCompaction = value;
            }
        }

        public double PScheduleMutation {
            get { return configuration.PScheduleMutation; }
            set {
                configuration.PScheduleMutation = value;
                SchedulingGenomeMutation sgm =
                    (SchedulingGenomeMutation) population.MutationOperator;
                sgm.PScheduleMutation = value;
            }
        }

        public double PRegisterMutation {
            get { return configuration.PRegisterMutation; }
            set {
                configuration.PRegisterMutation = value;
                SchedulingGenomeMutation sgm =
                    (SchedulingGenomeMutation) population.MutationOperator;
                sgm.PRegisterMutation = value;
            }
        }

        public SchedulingGenomePopulation Population {
            get { return population; }
        }

        public SimpleGA GA {
            get { return ga; }
        }

        public MachineDescription MachineDescription {
            get { return machineDescription; }
        }

        public InstructionGraph InstructionGraph {
            get { return instructionGraph; }
        }

        public IList InstructionNodes {
            get { return instructionNodes; }
        }

        public IList[] InstructionsOnExUnit {
            get { return instructionsOnExUnit; }
        }

        public IList ValueNodes {
            get { return valueNodes; }
        }

        public IList RegisterValues {
            get { return registerValues; }
        }

        public GAScheduler(GACodeGenerator codeGenerator) {
            this.codeGenerator = codeGenerator;
            this.configuration = codeGenerator.SchedulerConfiguration;
        }

        protected virtual void InitializePopulation() {

            SchedulingGenomeMutation sgm = new SchedulingGenomeMutation();
            sgm.PScheduleCrossSwap  = configuration.PScheduleCrossSwap;
            sgm.PScheduleCompaction = configuration.PScheduleCompaction;
            sgm.PScheduleMutation   = configuration.PScheduleMutation;
            sgm.PRegisterMutation   = configuration.PRegisterMutation;

            SchedulingGenomeCrossover sgc = new SchedulingGenomeCrossover();

            population.ScalingOperator   = new LinearScaling(ScalingFactor, true);
            population.SelectionOperator = new TournamentSelection();
            population.MutationOperator  = sgm;
            population.CrossoverOperator = sgc;
            population.GenomeEvaluator   = new SchedulingGenomeEvaluator();
        }

        protected virtual void InitializeGA() {

            ga = new SimpleGA(population);

            ga.PCrossover = configuration.PCrossover;

            ga.ComputeObjectiveStatistics =
                configuration.ComputeObjectiveStatistics;
            ga.ComputeFitnessStatistics =
                configuration.ComputeFitnessStatistics;
            ga.ComputeGenomeDiversity =
                configuration.ComputeGenomeDiversity;

            ga.GenerationComputed +=
                new SimpleGA.EventHandler(UpdateGAMonitor);
        }

        protected virtual void InitializeBase(
            MachineDescription machineDescription,
            InstructionGraph instructionGraph) {

            this.machineDescription = machineDescription;
            this.instructionGraph = instructionGraph;

            instructionNodes = instructionGraph.GetInstructionNodes();

            instructionsOnExUnit =
                new IList[machineDescription.ExecutionUnits];

            for (int i = 0; i < machineDescription.ExecutionUnits; i++) {
                instructionsOnExUnit[i] =
                    instructionGraph.GetInstructionNodesOnExUnit(i);
            }

            valueNodes = instructionGraph.GetValueNodes();

            registerValues = new ArrayList();

            foreach (ValueNode vNode in valueNodes) {
                if (vNode is RegisterValueNode)
                    registerValues.Add(vNode);
            }
        }

        public virtual void Initialize(
            MachineDescription machineDescription,
            InstructionGraph instructionGraph) {

            InitializeBase(machineDescription, instructionGraph);
            InitializeDependencies();

            population = new SchedulingGenomePopulation(PopulationSize, this);

            InitializePopulation();
            InitializeGA();

            bestObjectiveMonitor = new double[MonitoredGenerations];

            if (Initialized != null)
                Initialized(this);
        }

        public virtual void Initialize(
            MachineDescription machineDescription,
            InstructionGraph instructionGraph,
            SchedulingGenome initGenome) {

            InitializeBase(machineDescription, instructionGraph);
            InitializeDependencies();

            population = new SchedulingGenomePopulation(
                PopulationSize, this, initGenome, 1);

            InitializePopulation();
            InitializeGA();

            bestObjectiveMonitor = new double[MonitoredGenerations];

            if (Initialized != null)
                Initialized(this);
        }

        public virtual bool Optimize(
            out RegisterAssignment registerAssignment,
            out InstructionSchedule instructionSchedule) {

            Run();

            if (Done != null)
                Done(this);

            registerAssignment = BestGenome.GetRegisterAssignment();
            instructionSchedule = BestGenome.GetInstructionSchedule();

            return BestGenome.IsValid;
        }

        protected virtual void Run() {

            bool done = false;

            while (!done) {

                ga.ComputeNextGeneration();

                if (GenerationComputed != null)
                    GenerationComputed(this);

                if (ga.Generation > MaxGenerations)
                    done = true;

                int indexOfCurrentGen =
                    ga.Generation % MonitoredGenerations;
                int indexOfFirstGen =
                    (indexOfCurrentGen + 1) % MonitoredGenerations;

                if (ga.Generation > MonitoredGenerations &&
                    bestObjectiveMonitor[indexOfCurrentGen]
                        == bestObjectiveMonitor[indexOfFirstGen]) {

                    done = true;
                }
            }
        }

        protected virtual void UpdateGAMonitor(SimpleGA ga) {

            int indexOfCurrentGen =
                ga.Generation % MonitoredGenerations;

            bestObjectiveMonitor[indexOfCurrentGen] =
                BestIndividual.Objective;
        }

        protected virtual void InitializeDependencies() {

            dependencies = new Hashtable(instructionNodes.Count);

            foreach (InstructionNode instr in instructionNodes)
                dependencies[instr] = new ArrayList();

            IDictionary dependenciesComputed =
                new Hashtable(instructionNodes.Count);

            foreach (InstructionNode instr in instructionNodes)
                dependenciesComputed[instr] = false;

            foreach (InstructionNode instr in instructionNodes)
                InitializeDependenciesForInstruction(instr, dependenciesComputed);
        }

        protected virtual void InitializeDependenciesForInstruction(
            InstructionNode instr,
            IDictionary dependenciesComputed) {

            IList instrDependencies = (IList) dependencies[instr];

            foreach (ValueNode opValue in instr.OperandValues) {

                if (!opValue.IsInputValue()) {

                    InstructionNode opInstr = opValue.ProducingInstruction;

                    if (!((bool) dependenciesComputed[opInstr])) {
                        InitializeDependenciesForInstruction(
                            opInstr, dependenciesComputed);
                    }

                    IList opDependencies = (IList) dependencies[opInstr];

                    foreach (InstructionNode depInstr in opDependencies) {
                        if (!instrDependencies.Contains(depInstr))
                            instrDependencies.Add(depInstr);
                    }

                    instrDependencies.Add(opInstr);
                }
            }

            dependenciesComputed[instr] = true;
        }
    }
}

