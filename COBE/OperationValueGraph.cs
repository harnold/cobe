using System;
using System.Collections;

namespace COBE {

    [Serializable]
    public class OperationValueGraph: Graph {

        public IList GetValueNodes() {

            IList valueNodes = new ArrayList();

            foreach (Graph.Node n in Nodes) {
                if (n is ValueNode)
                    valueNodes.Add(n);
            }

            return valueNodes;
        }

        public IList GetOperationNodes() {

            IList opNodes = new ArrayList();

            foreach (Graph.Node n in Nodes) {
                if (n is OperationNode)
                    opNodes.Add(n);
            }

            return opNodes;
        }

    }
}
