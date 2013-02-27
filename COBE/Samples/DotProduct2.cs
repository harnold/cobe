using System.Collections;

namespace COBE.Samples {

    public class DotProduct2: IProgramGraphProvider {

        public ProgramGraph CreateUnrolledProgramGraph(int k) {

            ProgramGraph g = new ProgramGraph();

            ValueNode[] ap = new ValueNode[k + 1];
            ValueNode[] bp = new ValueNode[k + 1];
            ValueNode[] s  = new ValueNode[k + 1];

            ValueNode[] cp = new ValueNode[k + 1];
            ValueNode[] dp = new ValueNode[k + 1];
            ValueNode[] t  = new ValueNode[k + 1];

            for (int i = 0; i < k + 1; i++) {

                ap[i] = new TestRegisterNode("a[" + i + "]", Datatype.AddressValue);
                bp[i] = new TestRegisterNode("b[" + i + "]", Datatype.AddressValue);
                s[i]  = new TestRegisterNode("s[" + i + "]", Datatype.FloatValue);

                g.AddNode(ap[i]);
                g.AddNode(bp[i]);
                g.AddNode(s[i]);

                cp[i] = new TestRegisterNode("c[" + i + "]", Datatype.AddressValue);
                dp[i] = new TestRegisterNode("d[" + i + "]", Datatype.AddressValue);
                t[i]  = new TestRegisterNode("t[" + i + "]", Datatype.FloatValue);

                g.AddNode(cp[i]);
                g.AddNode(dp[i]);
                g.AddNode(t[i]);
            }

            for (int i = 0; i < k; i++) {

                CreateSingleLoop(
                    g, ap[i], bp[i], s[i], ap[i + 1], bp[i + 1], s[i + 1]);

                CreateSingleLoop(
                    g, cp[i], dp[i], t[i], cp[i + 1], dp[i + 1], t[i + 1]);
            }

            g.InitInputValues();
            g.InitOutputValues();

            g.CyclicDependencies[s[0]] = s[k];
            g.CyclicDependencies[s[k]] = s[0];

            g.CyclicDependencies[ap[0]] = ap[k];
            g.CyclicDependencies[ap[k]] = ap[0];

            g.CyclicDependencies[bp[0]] = bp[k];
            g.CyclicDependencies[bp[k]] = bp[0];

            g.CyclicDependencies[t[0]] = t[k];
            g.CyclicDependencies[t[k]] = t[0];

            g.CyclicDependencies[cp[0]] = cp[k];
            g.CyclicDependencies[cp[k]] = cp[0];

            g.CyclicDependencies[dp[0]] = dp[k];
            g.CyclicDependencies[dp[k]] = dp[0];

            return g;
        }

        public ProgramGraph CreateProgramGraph() {
            return CreateUnrolledProgramGraph(1);
        }

        protected void CreateSingleLoop(
            ProgramGraph g, ValueNode ap, ValueNode bp, ValueNode s,
            ValueNode ap1, ValueNode bp1, ValueNode s1) {

            OperationNode[] o = new OperationNode[8];

            o[0] = new AddOperationNode();
            o[1] = new MulOperationNode();
            o[2] = new LoadOperationNode();
            o[3] = new AddOperationNode();
            o[4] = new LoadOperationNode();
            o[5] = new AddOperationNode();
            o[6] = new ConstOperationNode();
            o[7] = new ConstOperationNode();

            foreach (OperationNode op in o)
                g.AddNode(op);

            ValueNode[] v = new ValueNode[7];

            v[0] = new FloatRegisterNode();
            v[1] = new FloatRegisterNode();
            v[2] = new FloatRegisterNode();
            v[3] = new IntRegisterNode();
            v[4] = new IntRegisterNode();
            v[5] = new IntConstantNode(4);
            v[6] = new IntConstantNode(4);

            foreach (ValueNode val in v)
                g.AddNode(val);

            g.AddEdge(v[0], o[0]);
            g.AddEdge(s, o[0]);
            g.AddEdge(o[0], s1);
            g.AddEdge(v[1], o[1]);
            g.AddEdge(v[2], o[1]);
            g.AddEdge(o[1], v[0]);
            g.AddEdge(ap, o[2]);
            g.AddEdge(o[2], v[1]);
            g.AddEdge(ap, o[3]);
            g.AddEdge(v[3], o[3]);
            g.AddEdge(o[3], ap1);
            g.AddEdge(bp, o[4]);
            g.AddEdge(o[4], v[2]);
            g.AddEdge(bp, o[5]);
            g.AddEdge(v[4], o[5]);
            g.AddEdge(o[5], bp1);
            g.AddEdge(v[5], o[6]);
            g.AddEdge(o[6], v[3]);
            g.AddEdge(v[6], o[7]);
            g.AddEdge(o[7], v[4]);
        }
    }
}
