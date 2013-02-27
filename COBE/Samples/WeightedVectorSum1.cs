using System.Collections;

namespace COBE.Samples {

    public class WeightedVectorSum1: IProgramGraphProvider {

        public ProgramGraph CreateUnrolledProgramGraph(int k) {

            ProgramGraph g = new ProgramGraph();

            ValueNode[] ap = new ValueNode[k + 1];
            ValueNode[] bp = new ValueNode[k + 1];
            ValueNode[] cp = new ValueNode[k + 1];

            for (int i = 0; i < k + 1; i++) {

                ap[i] = new TestRegisterNode("a[" + i + "]", Datatype.AddressValue);
                bp[i] = new TestRegisterNode("b[" + i + "]", Datatype.AddressValue);
                cp[i] = new TestRegisterNode("c[" + i + "]", Datatype.AddressValue);

                g.AddNode(ap[i]);
                g.AddNode(bp[i]);
                g.AddNode(cp[i]);
            }

            ValueNode k0 = new TestRegisterNode("k0", Datatype.FloatValue);
            ValueNode k1 = new TestRegisterNode("k1", Datatype.FloatValue);

            g.AddNode(k0);
            g.AddNode(k1);

            for (int i = 0; i < k; i++) {

                CreateSingleLoop(
                    g, ap[i], bp[i], cp[i], k0, k1,
                    ap[i + 1], bp[i + 1], cp[i + 1]);
            }

            g.InitInputValues();
            g.InitOutputValues();

            g.OutputValues.Add(k0);
            g.OutputValues.Add(k1);

            g.CyclicDependencies[ap[0]] = ap[k];
            g.CyclicDependencies[ap[k]] = ap[0];

            g.CyclicDependencies[bp[0]] = bp[k];
            g.CyclicDependencies[bp[k]] = bp[0];

            g.CyclicDependencies[cp[0]] = cp[k];
            g.CyclicDependencies[cp[k]] = cp[0];

            return g;
        }

        public ProgramGraph CreateProgramGraph() {
            return CreateUnrolledProgramGraph(1);
        }

        protected void CreateSingleLoop(
            ProgramGraph g, ValueNode ap, ValueNode bp, ValueNode cp,
            ValueNode k0, ValueNode k1,
            ValueNode ap1, ValueNode bp1, ValueNode cp1) {

            OperationNode[] o = new OperationNode[12];

            o[0]  = new AddOperationNode();
            o[1]  = new MulOperationNode();
            o[2]  = new MulOperationNode();
            o[3]  = new LoadOperationNode();
            o[4]  = new AddOperationNode();
            o[5]  = new LoadOperationNode();
            o[6]  = new AddOperationNode();
            o[7]  = new ConstOperationNode();
            o[8]  = new ConstOperationNode();
            o[9]  = new StoreOperationNode();
            o[10] = new AddOperationNode();
            o[11] = new ConstOperationNode();

            foreach (OperationNode op in o)
                g.AddNode(op);

            ValueNode[] v = new ValueNode[12];

            v[0]  = new FloatRegisterNode();
            v[1]  = new FloatRegisterNode();
            v[2]  = new FloatRegisterNode();
            v[3]  = new FloatRegisterNode();
            v[4]  = new IntRegisterNode();
            v[5]  = new IntRegisterNode();
            v[6]  = new IntConstantNode(4);
            v[7]  = new IntConstantNode(4);
            v[8]  = new FloatRegisterNode();
            v[9]  = new FloatMemoryNode(null);
            v[10] = new IntRegisterNode();
            v[11] = new IntConstantNode(4);

            foreach (ValueNode val in v)
                g.AddNode(val);

            g.AddEdge(v[0], o[0]);
            g.AddEdge(v[1], o[0]);
            g.AddEdge(o[0], v[8]);
            g.AddEdge(v[2], o[1]);
            g.AddEdge(k0, o[1]);
            g.AddEdge(o[1], v[0]);
            g.AddEdge(v[3], o[2]);
            g.AddEdge(k1, o[2]);
            g.AddEdge(o[2], v[1]);
            g.AddEdge(ap, o[3]);
            g.AddEdge(o[3], v[2]);
            g.AddEdge(ap, o[4]);
            g.AddEdge(v[4], o[4]);
            g.AddEdge(o[4], ap1);
            g.AddEdge(bp, o[5]);
            g.AddEdge(o[5], v[3]);
            g.AddEdge(bp, o[6]);
            g.AddEdge(v[5], o[6]);
            g.AddEdge(o[6], bp1);
            g.AddEdge(v[6], o[7]);
            g.AddEdge(o[7], v[4]);
            g.AddEdge(v[7], o[8]);
            g.AddEdge(o[8], v[5]);
            g.AddEdge(v[8], o[9]);
            g.AddEdge(cp, o[9]);
            g.AddEdge(o[9], v[9]);
            g.AddEdge(cp, o[10]);
            g.AddEdge(v[10], o[10]);
            g.AddEdge(o[10], cp1);
            g.AddEdge(v[11], o[11]);
            g.AddEdge(o[11], v[10]);
        }
    }
}
