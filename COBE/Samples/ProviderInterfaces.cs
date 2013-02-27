namespace COBE.Samples {

    public interface IProgramGraphProvider {
        ProgramGraph CreateProgramGraph();
        ProgramGraph CreateUnrolledProgramGraph(int k);
    }

    public interface IMachineDescriptionProvider {
        MachineDescription CreateMachineDescription();
    }
}
