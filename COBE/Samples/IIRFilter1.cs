using System.Collections;

namespace COBE.Samples {

    public class IIRFilter1: IProgramGraphProvider {

        public ProgramGraph CreateUnrolledProgramGraph(int k) {

            ProgramGraph g = new ProgramGraph();

            ValueNode[] xp = new ValueNode[k + 1];
            ValueNode[] yp = new ValueNode[k + 1];
            ValueNode[] y  = new ValueNode[k + 1];
            ValueNode[] x  = new ValueNode[k + 1];

            for (int i = 0; i < k + 1; i++) {

                xp[i] = new TestRegisterNode("xp[" + i + "]", Datatype.AddressValue);
                yp[i] = new TestRegisterNode("yp[" + i + "]", Datatype.AddressValue);

                y[i] = new TestRegisterNode("y[" + i + "]", Datatype.FloatValue);
                x[i] = new TestRegisterNode("x[" + i + "]", Datatype.FloatValue);

                g.AddNode(xp[i]);
                g.AddNode(yp[i]);
                g.AddNode(x[i]);
                g.AddNode(y[i]);
            }

            ValueNode pc0 = new FloatMemoryNode("c0");
            ValueNode pc1 = new FloatMemoryNode("c1");
            ValueNode pc2 = new FloatMemoryNode("c2");

            OperationNode lc0 = new LoadOperationNode();
            OperationNode lc1 = new LoadOperationNode();
            OperationNode lc2 = new LoadOperationNode();

            ValueNode c0 = new FloatRegisterNode();
            ValueNode c1 = new FloatRegisterNode();
            ValueNode c2 = new FloatRegisterNode();

            g.AddNode(pc0); g.AddNode(pc1); g.AddNode(pc2);
            g.AddNode(lc0); g.AddNode(lc1); g.AddNode(lc2);
            g.AddNode(c0);  g.AddNode(c1);  g.AddNode(c2);

            g.AddEdge(pc0, lc0); g.AddEdge(lc0, c0);
            g.AddEdge(pc1, lc1); g.AddEdge(lc1, c1);
            g.AddEdge(pc2, lc2); g.AddEdge(lc2, c2);

            for (int i = 0; i < k; i++) {

                CreateSingleLoop(
                    g, xp[i], yp[i], x[i], y[i], c0, c1, c2,
                    xp[i + 1], yp[i + 1], x[i + 1], y[i + 1]);
            }

            g.InitInputValues();
            g.InitOutputValues();

            g.OutputValues.Add(x[k]);
            g.OutputValues.Add(y[k]);

            x[k].OutputFlag = true;
            y[k].OutputFlag = true;

            g.CyclicDependencies[xp[0]] = xp[k];
            g.CyclicDependencies[xp[k]] = xp[0];

            g.CyclicDependencies[yp[0]] = yp[k];
            g.CyclicDependencies[yp[k]] = yp[0];

            g.CyclicDependencies[y[0]] = y[k];
            g.CyclicDependencies[y[k]] = y[0];

            g.CyclicDependencies[x[0]] = x[k];
            g.CyclicDependencies[x[k]] = x[0];

            return g;
        }

        public ProgramGraph CreateProgramGraph() {
            return CreateUnrolledProgramGraph(1);
        }

        protected void CreateSingleLoop(
            ProgramGraph g, ValueNode xp, ValueNode yp, ValueNode x, ValueNode y,
            ValueNode c0, ValueNode c1, ValueNode c2, ValueNode xp1, ValueNode yp1,
            ValueNode x1, ValueNode y1) {

            OperationNode[] o = new OperationNode[11];

            o[0]  = new StoreOperationNode();
            o[1]  = new AddOperationNode();
            o[2]  = new AddOperationNode();
            o[3]  = new ConstOperationNode();
            o[4]  = new AddOperationNode();
            o[5]  = new MulOperationNode();
            o[6]  = new MulOperationNode();
            o[7]  = new MulOperationNode();
            o[8]  = new LoadOperationNode();
            o[9]  = new AddOperationNode();
            o[10] = new ConstOperationNode();

            foreach (OperationNode op in o)
                g.AddNode(op);

            ValueNode[] v = new ValueNode[9];

            v[0]  = new FloatMemoryNode(null);
            v[1]  = new IntRegisterNode();
            v[2]  = new FloatRegisterNode();
            v[3]  = new FloatRegisterNode();
            v[4]  = new IntConstantNode(4);
            v[5]  = new FloatRegisterNode();
            v[6]  = new FloatRegisterNode();
            v[7]  = new IntRegisterNode();
            v[8]  = new IntConstantNode(4);

            foreach (ValueNode val in v)
                g.AddNode(val);

            g.AddEdge(y1, o[0]);
            g.AddEdge(yp, o[0]);
            g.AddEdge(o[0], v[0]);
            g.AddEdge(yp, o[1]);
            g.AddEdge(v[1], o[1]);
            g.AddEdge(o[1], yp1);
            g.AddEdge(v[2], o[2]);
            g.AddEdge(v[3], o[2]);
            g.AddEdge(o[2], y1);
            g.AddEdge(v[4], o[3]);
            g.AddEdge(o[3], v[1]);
            g.AddEdge(v[5], o[4]);
            g.AddEdge(v[6], o[4]);
            g.AddEdge(o[4], v[2]);
            g.AddEdge(c2, o[5]);
            g.AddEdge(y, o[5]);
            g.AddEdge(o[5], v[3]);
            g.AddEdge(c1, o[6]);
            g.AddEdge(x, o[6]);
            g.AddEdge(o[6], v[5]);
            g.AddEdge(c0, o[7]);
            g.AddEdge(x1, o[7]);
            g.AddEdge(o[7], v[6]);
            g.AddEdge(xp, o[8]);
            g.AddEdge(o[8], x1);
            g.AddEdge(xp, o[9]);
            g.AddEdge(v[7], o[9]);
            g.AddEdge(o[9], xp1);
            g.AddEdge(v[8], o[10]);
            g.AddEdge(o[10], v[7]);
        }
    }
}
