using System;
using System.Collections;
using GA;

namespace COBE {

    [Serializable]
    public class SelectionGenomeCrossover: ICrossoverOperator {

        public virtual void Cross(IGenome genome1, IGenome genome2) {

            SelectionGenome sGenome1 = (SelectionGenome) genome1;
            SelectionGenome sGenome2 = (SelectionGenome) genome2;

            SelectionCrossover(sGenome1, sGenome2);

            sGenome1.InstructionGraph     = null;
            sGenome1.RegisterAssignment   = null;
            sGenome1.InstructionSchedule  = null;
            sGenome1.Modified = true;

            /*
            sGenome1.BestSchedulingGenome = null;
            */

            sGenome2.InstructionGraph     = null;
            sGenome2.RegisterAssignment   = null;
            sGenome2.InstructionSchedule  = null;
            sGenome2.Modified = true;

            /*
            sGenome2.BestSchedulingGenome = null;
            */
        }

        protected virtual void SelectionCrossover(
            SelectionGenome genome1, SelectionGenome genome2) {

            int depth = GetCrossoverDepth(genome1, genome2);
            ValueNode rootVal = GetRootValue(genome1, genome2);
            IList operationsToCross = new ArrayList();

            GetOperationsToCross(genome1.ProgramGraph, rootVal, depth,
                                 operationsToCross);
            ExtendOperationsToCross(genome1, genome2, operationsToCross);
            CrossSelection(genome1, genome2, operationsToCross);
        }

        protected virtual void CrossSelection(
            SelectionGenome genome1, SelectionGenome genome2,
            IList operationsToCross) {

            foreach (OperationNode op in operationsToCross) {
                CoveringInfo ci1 = (CoveringInfo) genome1.Selection[op];
                CoveringInfo ci2 = (CoveringInfo) genome2.Selection[op];
                genome1.Selection[op] = ci2;
                genome2.Selection[op] = ci1;
            }
        }

        protected virtual void ExtendOperationsToCross(
            SelectionGenome genome1, SelectionGenome genome2,
            IList operationsToCross) {

            IList operationsToAdd = new ArrayList();
            bool operationAdded = true;

            while (operationAdded) {

                operationAdded = false;

                foreach (OperationNode op in operationsToCross) {

                    CoveringInfo ci1 = (CoveringInfo) genome1.Selection[op];

                    foreach (OperationNode coveredOp in ci1.CoveredOperations) {

                        if (!operationsToCross.Contains(coveredOp) &&
                            !operationsToAdd.Contains(coveredOp)) {

                            operationsToAdd.Add(coveredOp);
                            operationAdded = true;
                        }
                    }

                    CoveringInfo ci2 = (CoveringInfo) genome2.Selection[op];

                    foreach (OperationNode coveredOp in ci2.CoveredOperations) {

                        if (!operationsToCross.Contains(coveredOp) &&
                            !operationsToAdd.Contains(coveredOp)) {

                            operationsToAdd.Add(coveredOp);
                            operationAdded = true;
                        }
                    }
                }
            }

            foreach (OperationNode op in operationsToAdd)
                operationsToCross.Add(op);
        }

        protected virtual int GetCrossoverDepth(
            SelectionGenome genome1, SelectionGenome genome2) {

            int nCross = (int) (Math.Abs(GA.Random.NextGaussian())
                                * (genome1.OperationNodes.Count - 1) / 3 + 1);

            nCross = Math.Min(nCross, genome1.OperationNodes.Count);

            return (int) Math.Log(nCross, 2) + 1;
        }

        protected virtual ValueNode GetRootValue(
            SelectionGenome genome1, SelectionGenome genome2) {

            ValueNode rootVal = null;
            OperationNode rootOp;
            CoveringInfo rootCi1;
            CoveringInfo rootCi2;

            bool foundRootOp = false;

            while (!foundRootOp) {

                rootOp = (OperationNode) genome1.OperationNodes[
                    GA.Random.Next(genome1.OperationNodes.Count)];

                rootCi1 = (CoveringInfo) genome1.Selection[rootOp];
                rootCi2 = (CoveringInfo) genome2.Selection[rootOp];

                if (rootCi1.ResultValue == rootCi2.ResultValue) {
                    rootVal = rootCi1.ResultValue;
                    foundRootOp = true;
                }
            }

            return rootVal;
        }

        protected virtual void GetOperationsToCross(
            ProgramGraph programGraph, ValueNode rootVal, int depth,
            IList operationsToCross) {

            if (depth > 0) {

                if (!rootVal.IsInputValue()) {

                    OperationNode op =
                        (OperationNode) rootVal.ProducingOperation;

                    if (!operationsToCross.Contains(op)) {
                        operationsToCross.Add(op);
                        foreach (ValueNode opVal in op.OperandValues) {
                            GetOperationsToCross(programGraph, opVal, depth - 1,
                                                 operationsToCross);
                        }
                    }
                }
            }
        }
    }
}
