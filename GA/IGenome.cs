using System;

namespace GA {

    public interface IMutationOperator {
        void Mutate(IGenome genome, double pMutation);
    }

    public interface ICrossoverOperator {
        void Cross(IGenome genome1, IGenome genome2);
    }

    public interface IGenomeEvaluator {
        double Evaluate(IGenome genome);
    }

    public interface IGenome: ICloneable {
        void Initialize();
        void Copy(IGenome otherGenome);
        double DifferenceTo(IGenome otherGenome);
    }
}
