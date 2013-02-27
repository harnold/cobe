using System;
using System.Collections;
using System.IO;
using GA;

namespace COBE {

    [Serializable]
    public class GAInstructionSelectorConfiguration {

        public int PopulationSize = 50;
        public double ScalingFactor = 2.0;
        public int MaxGenerations = 100;
        public int MonitoredGenerations = 10;

        public double PCrossover = 0.6;
        public double PMutation = 0.1;

        public bool ComputeObjectiveStatistics = true;
        public bool ComputeFitnessStatistics = false;
        public bool ComputeGenomeDiversity = false;

        public override string ToString() {

            StringWriter s = new StringWriter();

            s.WriteLine("PopulationSize       = " + PopulationSize);
            s.WriteLine("ScalingFactor        = " + ScalingFactor);
            s.WriteLine("MaxGenerations       = " + MaxGenerations);
            s.WriteLine("MonitoredGenerations = " + MonitoredGenerations);
            s.WriteLine("PCrossover           = " + PCrossover);
            s.WriteLine("PMutation            = " + PMutation);

            return s.ToString();
        }
    }

    [Serializable]
    public class GAInstructionSelector {

        public delegate void EventHandler(GAInstructionSelector sender);

        public event EventHandler GenerationComputed;
        public event EventHandler Initialized;
        public event EventHandler Done;

        private MachineDescription machineDescription;
        private IList transferInstructions;
        private ProgramGraph programGraph;
        private IDictionary coveringDesc;
        private IList operationNodes;
        private IList valueNodes;

        private SelectionGenomePopulation population;
        private SimpleGA ga;

        private double[] bestObjectiveMonitor;

        private GAScheduler scheduler;
        private GACodeGenerator codeGenerator;
        private GAInstructionSelectorConfiguration configuration;

        public GAScheduler Scheduler {
            get { return scheduler; }
        }

        public GAInstructionSelectorConfiguration Configuration {
            get { return configuration; }
        }

        public SelectionGenome BestGenome {
            get { return (SelectionGenome) BestIndividual.Genome; }
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

        public double PMutation {
            get { return configuration.PMutation; }
            set {
                configuration.PMutation = value;
                ga.PMutation = value;
            }
        }

        public SelectionGenomePopulation Population {
            get { return population; }
        }

        public SimpleGA GA {
            get { return ga; }
        }

        public IList OperationNodes {
            get { return operationNodes; }
        }

        public IList ValueNodes {
            get { return valueNodes; }
        }

        public MachineDescription MachineDescription {
            get { return machineDescription; }
        }

        public IList TransferInstructions {
            get { return transferInstructions; }
        }

        public ProgramGraph ProgramGraph {
            get { return programGraph; }
        }

        public IDictionary CoveringDesc {
            get { return coveringDesc; }
        }

        public GAInstructionSelector(GACodeGenerator codeGenerator) {
            this.codeGenerator = codeGenerator;
            configuration = codeGenerator.SelectorConfiguration;
            scheduler = new GAScheduler(codeGenerator);
        }

        public virtual void Initialize(
            MachineDescription machineDescription,
            ProgramGraph programGraph) {

            this.machineDescription = machineDescription;
            this.programGraph = programGraph;

            transferInstructions = machineDescription.GetTransferInstructions();
            operationNodes = programGraph.GetOperationNodes();
            valueNodes = programGraph.GetValueNodes();

            coveringDesc =
                programGraph.CreateCoveringDesc(machineDescription.Instructions);

            population = new SelectionGenomePopulation(PopulationSize, this);

            population.ScalingOperator   = new LinearScaling(ScalingFactor, true);
            population.SelectionOperator = new RouletteWheelSelection();
            population.MutationOperator  = new SelectionGenomeMutation();
            population.CrossoverOperator = new SelectionGenomeCrossover();
            population.GenomeEvaluator   = new SelectionGenomeEvaluator(scheduler);

            ga = new SimpleGA(population);

            ga.PMutation = configuration.PMutation;
            ga.PCrossover = configuration.PCrossover;

            ga.ComputeObjectiveStatistics =
                configuration.ComputeObjectiveStatistics;
            ga.ComputeFitnessStatistics =
                configuration.ComputeFitnessStatistics;
            ga.ComputeGenomeDiversity =
                configuration.ComputeGenomeDiversity;

            ga.GenerationComputed +=
                new SimpleGA.EventHandler(UpdateGAMonitor);

            bestObjectiveMonitor = new double[MonitoredGenerations];

            if (Initialized != null)
                Initialized(this);
        }

        public virtual bool Optimize(
            out InstructionGraph instructionGraph,
            out RegisterAssignment registerAssignment,
            out InstructionSchedule instructionSchedule) {

            Run();

            if (Done != null)
                Done(this);

            if (BestGenome.IsValid) {
                instructionGraph    = BestGenome.InstructionGraph;
                registerAssignment  = BestGenome.RegisterAssignment;
                instructionSchedule = BestGenome.InstructionSchedule;
                return true;

            } else {
                instructionGraph    = null;
                instructionSchedule = null;
                registerAssignment  = null;
                return false;
            }
        }

        protected virtual void Run() {

            bool done = false;

            while (!done) {

                ga.ComputeNextGeneration();

                if (GenerationComputed != null)
                    GenerationComputed(this);

                if (ga.Generation > MaxGenerations)
                    done = true;

                if (BestGenome.IsValid) {

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
        }

        protected virtual void UpdateGAMonitor(SimpleGA ga) {

            int indexOfCurrentGen =
                ga.Generation % MonitoredGenerations;

            bestObjectiveMonitor[indexOfCurrentGen] =
                BestIndividual.Objective;
        }

    }
}
