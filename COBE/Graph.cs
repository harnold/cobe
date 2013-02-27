using System;
using System.Collections;

namespace COBE {

    [Serializable]
    public class Graph {

        [Serializable]
        public class Node {

            private IList inNodes;
            private IList outNodes;

            public IList InNodes {
                get { return inNodes; }
            }

            public IList OutNodes {
                get { return outNodes; }
            }

            public Node() {
                inNodes = new ArrayList();
                outNodes = new ArrayList();
            }
        }

        private IList nodes;

        public IList Nodes {
            get { return nodes; }
        }

        public Graph() {
            nodes = new ArrayList();
        }

        public void AddNode(Node node) {
            nodes.Add(node);
        }

        public void RemoveNode(Node node) {

            nodes.Remove(node);

            foreach (Node n in node.InNodes)
                n.OutNodes.Remove(node);

            foreach (Node n in node.OutNodes)
                n.InNodes.Remove(node);
        }

        public void AddEdge(Node startNode, Node endNode) {
            startNode.OutNodes.Add(endNode);
            endNode.InNodes.Add(startNode);
        }

        public void RemoveEdge(Node startNode, Node endNode) {
            startNode.OutNodes.Remove(endNode);
            endNode.InNodes.Remove(startNode);
        }

        public void ReplaceEdgeStart(
            Node oldStartNode, Node endNode, Node newStartNode) {

            oldStartNode.OutNodes.Remove(endNode);

            int endIndex = endNode.InNodes.IndexOf(oldStartNode);
            endNode.InNodes[endIndex] = newStartNode;
            newStartNode.OutNodes.Add(endNode);
        }
    }
}
