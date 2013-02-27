using System;
using System.Collections;
using GA;

namespace COBE {

    [Serializable]
    public class SelectionGenomeMutation: IMutationOperator {

        public virtual void Mutate(IGenome genome, double pMutation) {

            SelectionGenome sGenome = (SelectionGenome) genome;

            if (GA.Random.NextDouble() < pMutation) {

                SelectionMutation(sGenome);

                sGenome.InstructionGraph = null;
                sGenome.RegisterAssignment = null;
                sGenome.InstructionSchedule = null;
                sGenome.Modified = true;

                /*
                sGenome.BestSchedulingGenome = null;
                */
            }
        }

        protected virtual void SelectionMutation(SelectionGenome genome) {

            int depth = GetMutationDepth(genome);

            OperationNode rootOp =
                (OperationNode) genome.OperationNodes[
                    GA.Random.Next(genome.OperationNodes.Count)];

            CoveringInfo rootCi = (CoveringInfo) genome.Selection[rootOp];
            ValueNode rootVal = rootCi.ResultValue;

            IList operationsToMutate = new ArrayList();

            GetOperationsToMutate(genome.ProgramGraph, rootVal, depth,
                                  operationsToMutate);
            ExtendOperationsToMutate(genome, operationsToMutate);
            MutateSelection(genome, operationsToMutate);
        }

        protected virtual void ExtendOperationsToMutate(
            SelectionGenome genome, IList operationsToMutate) {

            IList operationsToAdd = new ArrayList();

            foreach (OperationNode op in operationsToMutate) {

                CoveringInfo ci = (CoveringInfo) genome.Selection[op];

                foreach (OperationNode coveredOp in ci.CoveredOperations) {

                    if (!operationsToMutate.Contains(coveredOp) &&
                        !operationsToAdd.Contains(coveredOp)) {

                        operationsToAdd.Add(coveredOp);
                    }
                }
            }

            foreach (OperationNode op in operationsToAdd)
                operationsToMutate.Add(op);
        }

        protected virtual int GetMutationDepth(SelectionGenome genome) {

            int nMut = (int) (Math.Abs(GA.Random.NextGaussian())
                              * (genome.OperationNodes.Count - 1) / 3 + 1);

            nMut = Math.Min(nMut, genome.OperationNodes.Count);

            return (int) Math.Log(nMut, 2) + 1;
        }

        protected virtual void MutateSelection(
            SelectionGenome genome, IList operationsToMutate) {

            while (operationsToMutate.Count > 0) {

                OperationNode mutOp =
                    (OperationNode) operationsToMutate[0];

                IList coveringDesc =
                    (IList) genome.CoveringDesc[mutOp.ResultValue];

                CoveringInfo coveringInfo = null;
                bool coveringFound = false;

                while (!coveringFound) {

                    coveringInfo =
                        (CoveringInfo) coveringDesc[
                            GA.Random.Next(coveringDesc.Count)];

                    bool containsAllOps = true;

                    foreach (OperationNode op in coveringInfo.CoveredOperations) {
                        if (!operationsToMutate.Contains(op)) {
                            containsAllOps = false;
                            break;
                        }
                    }

                    coveringFound = containsAllOps;
                }

                foreach (OperationNode op in coveringInfo.CoveredOperations) {
                    genome.Selection[op] = coveringInfo;
                    operationsToMutate.Remove(op);
                }
            }

        }

        protected virtual void GetOperationsToMutate(
            ProgramGraph programGraph, ValueNode rootVal, int depth,
            IList operationsToMutate) {

            if (depth > 0) {

                if (!rootVal.IsInputValue()) {

                    OperationNode op =
                        (OperationNode) rootVal.ProducingOperation;

                    if (!operationsToMutate.Contains(op)) {
                        operationsToMutate.Add(op);
                        foreach (ValueNode opVal in op.OperandValues) {
                            GetOperationsToMutate(programGraph, opVal, depth - 1,
                                                  operationsToMutate);
                        }
                    }
                }
            }
        }
    }
}
