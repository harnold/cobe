using System;
using System.Collections;

namespace COBE {

    [Serializable]
    public class InstructionValueGraph: Graph {

        public virtual IList GetValueNodes() {

            IList valueNodes = new ArrayList();

            foreach (Graph.Node n in Nodes) {
                if (n is ValueNode)
                    valueNodes.Add(n);
            }

            return valueNodes;
        }

        public virtual IList GetInstructionNodes() {

            IList instrNodes = new ArrayList();

            foreach (Graph.Node n in Nodes) {
                if (n is InstructionNode)
                    instrNodes.Add(n);
            }

            return instrNodes;
        }

        public virtual IList GetInstructionNodesOnExUnit(int exUnit) {

            IList instrNodes = new ArrayList();

            foreach (Graph.Node n in Nodes) {

                if (n is InstructionNode) {
                    InstructionNode i = (InstructionNode) n;
                    if (i.Instruction.ExecutionUnit == exUnit)
                        instrNodes.Add(i);
                }
            }

            return instrNodes;
        }
    }
}
