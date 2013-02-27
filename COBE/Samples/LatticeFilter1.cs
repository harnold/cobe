using System.Collections;

namespace COBE.Samples {

    public class LatticeFilter1: IProgramGraphProvider {

        public ProgramGraph CreateUnrolledProgramGraph(int k) {

            ProgramGraph g = new ProgramGraph();

            ValueNode[] px = new ValueNode[k + 1];
            ValueNode[] py = new ValueNode[k + 1];

            for (int i = 0; i < k + 1; i++) {

                px[i] = new TestRegisterNode("x[" + i + "]", Datatype.AddressValue);
                py[i] = new TestRegisterNode("y[" + i + "]", Datatype.AddressValue);

                g.AddNode(px[i]);
                g.AddNode(py[i]);
            }

            ValueNode[] x = new ValueNode[k];
            ValueNode[] y = new ValueNode[k];

            for (int i = 0; i < k; i++) {

                x[i] = new FloatRegisterNode();
                y[i] = new FloatRegisterNode();

                g.AddNode(x[i]);
                g.AddNode(y[i]);
            }

            ValueNode[] x0 = new ValueNode[k + 1];
            ValueNode[] x1 = new ValueNode[k + 1];

            for (int i = 0; i < k + 1; i++) {

                x0[i] = new TestRegisterNode("x0[" + i + "]", Datatype.FloatValue);
                x1[i] = new TestRegisterNode("x1[" + i + "]", Datatype.FloatValue);

                g.AddNode(x0[i]);
                g.AddNode(x1[i]);
            }

            ValueNode pc0 = new FloatMemoryNode("c0");
            ValueNode pc1 = new FloatMemoryNode("c1");
            ValueNode pc2 = new FloatMemoryNode("c2");
            ValueNode pc3 = new FloatMemoryNode("c3");
            ValueNode pc4 = new FloatMemoryNode("c4");

            OperationNode lc0 = new LoadOperationNode();
            OperationNode lc1 = new LoadOperationNode();
            OperationNode lc2 = new LoadOperationNode();
            OperationNode lc3 = new LoadOperationNode();
            OperationNode lc4 = new LoadOperationNode();

            ValueNode c0 = new FloatRegisterNode();
            ValueNode c1 = new FloatRegisterNode();
            ValueNode c2 = new FloatRegisterNode();
            ValueNode c3 = new FloatRegisterNode();
            ValueNode c4 = new FloatRegisterNode();

            g.AddNode(pc0); g.AddNode(pc1); g.AddNode(pc2);
            g.AddNode(pc3); g.AddNode(pc4);

            g.AddNode(lc0); g.AddNode(lc1); g.AddNode(lc2);
            g.AddNode(lc3); g.AddNode(lc4);

            g.AddNode(c0); g.AddNode(c1); g.AddNode(c2);
            g.AddNode(c3); g.AddNode(c4);

            g.AddEdge(pc0, lc0); g.AddEdge(lc0, c0);
            g.AddEdge(pc1, lc1); g.AddEdge(lc1, c1);
            g.AddEdge(pc2, lc2); g.AddEdge(lc2, c2);
            g.AddEdge(pc3, lc3); g.AddEdge(lc3, c3);
            g.AddEdge(pc4, lc4); g.AddEdge(lc4, c4);

            for (int i = 0; i < k; i++) {

                CreateLoadIncGraph(
                    g, x[i], px[i], px[i + 1], 4);

                CreateStoreIncGraph(
                    g, y[i], py[i], py[i + 1], 4);

                CreateSingleFilterLoop(
                    g, x[i], x0[i], x1[i], c0, c1, c2, c3, c4,
                    x0[i + 1], x1[i + 1], y[i]);
            }

            g.InitInputValues();
            g.InitOutputValues();

            g.CyclicDependencies[x0[0]] = x0[k];
            g.CyclicDependencies[x0[k]] = x0[0];

            g.CyclicDependencies[x1[0]] = x1[k];
            g.CyclicDependencies[x1[k]] = x1[0];

            g.CyclicDependencies[px[0]] = px[k];
            g.CyclicDependencies[px[k]] = px[0];

            g.CyclicDependencies[py[0]] = py[k];
            g.CyclicDependencies[py[k]] = py[0];

            return g;
        }

        public ProgramGraph CreateProgramGraph() {
            return CreateUnrolledProgramGraph(1);
        }

        protected void CreateStoreIncGraph(
            OperationValueGraph g, ValueNode x, ValueNode p,
            ValueNode pi, int inc) {

            OperationNode storeOp = new StoreOperationNode();
            OperationNode addOp   = new AddOperationNode();
            OperationNode constOp = new ConstOperationNode();

            g.AddNode(storeOp);
            g.AddNode(addOp);
            g.AddNode(constOp);

            ValueNode m = new MemoryValueNode(x.Datatype, null);
            g.AddNode(m);

            ValueNode t = new IntRegisterNode();
            ValueNode i = new IntConstantNode(inc);

            g.AddNode(t);
            g.AddNode(i);

            g.AddEdge(x, storeOp);
            g.AddEdge(p, storeOp);
            g.AddEdge(storeOp, m);
            g.AddEdge(p, addOp);
            g.AddEdge(i, constOp);
            g.AddEdge(constOp, t);
            g.AddEdge(t, addOp);
            g.AddEdge(addOp, pi);
        }

        protected void CreateLoadIncGraph(
            OperationValueGraph g, ValueNode x, ValueNode p,
            ValueNode pi, int inc) {

            OperationNode loadOp  = new LoadOperationNode();
            OperationNode addOp   = new AddOperationNode();
            OperationNode constOp = new ConstOperationNode();

            g.AddNode(loadOp);
            g.AddNode(addOp);
            g.AddNode(constOp);

            ValueNode t = new IntRegisterNode();
            ValueNode i = new IntConstantNode(inc);

            g.AddNode(t);
            g.AddNode(i);

            g.AddEdge(p, loadOp);
            g.AddEdge(loadOp, x);
            g.AddEdge(p, addOp);
            g.AddEdge(i, constOp);
            g.AddEdge(constOp, t);
            g.AddEdge(t, addOp);
            g.AddEdge(addOp, pi);
        }

        protected void CreateSingleFilterLoop(
            ProgramGraph g, ValueNode x, ValueNode x0, ValueNode x1,
            ValueNode c0, ValueNode c1, ValueNode c2, ValueNode c3, ValueNode c4,
            ValueNode z0, ValueNode z1, ValueNode y) {

            OperationNode[] o = new OperationNode[13];

            o[0]  = new AddOperationNode();
            o[1]  = new MulOperationNode();
            o[2]  = new AddOperationNode();
            o[3]  = new AddOperationNode();
            o[4]  = new MulOperationNode();
            o[5]  = new AddOperationNode();
            o[6]  = new AddOperationNode();
            o[7]  = new AddOperationNode();
            o[8]  = new MulOperationNode();
            o[9]  = new MulOperationNode();
            o[10] = new MulOperationNode();
            o[11] = new AddOperationNode();
            o[12] = new AddOperationNode();

            foreach (OperationNode opNode in o)
                g.AddNode(opNode);

            ValueNode[] v = new ValueNode[10];

            for (int i = 0; i < v.Length; i++)
                v[i] = new FloatRegisterNode();

            foreach (ValueNode vNode in v)
                g.AddNode(vNode);

            g.AddEdge(x,     o[0]);
            g.AddEdge(x0,    o[0]);
            g.AddEdge(o[0],  v[0]);
            g.AddEdge(c0,    o[1]);
            g.AddEdge(v[0],  o[1]);
            g.AddEdge(o[1],  v[1]);
            g.AddEdge(x,     o[2]);
            g.AddEdge(v[1],  o[2]);
            g.AddEdge(o[2],  v[2]);
            g.AddEdge(v[2],  o[3]);
            g.AddEdge(x1,    o[3]);
            g.AddEdge(o[3],  v[3]);
            g.AddEdge(c1,    o[4]);
            g.AddEdge(v[3],  o[4]);
            g.AddEdge(o[4],  v[4]);
            g.AddEdge(v[2],  o[5]);
            g.AddEdge(v[4],  o[5]);
            g.AddEdge(o[5],  z1);
            g.AddEdge(v[1],  o[6]);
            g.AddEdge(x0,    o[6]);
            g.AddEdge(o[6],  v[5]);
            g.AddEdge(v[4],  o[7]);
            g.AddEdge(x1,    o[7]);
            g.AddEdge(o[7],  z0);
            g.AddEdge(v[5],  o[8]);
            g.AddEdge(c2,    o[8]);
            g.AddEdge(o[8],  v[6]);
            g.AddEdge(z0,    o[9]);
            g.AddEdge(c3,    o[9]);
            g.AddEdge(o[9],  v[7]);
            g.AddEdge(z1,    o[10]);
            g.AddEdge(c4,    o[10]);
            g.AddEdge(o[10], v[9]);
            g.AddEdge(v[6],  o[11]);
            g.AddEdge(v[7],  o[11]);
            g.AddEdge(o[11], v[8]);
            g.AddEdge(v[8],  o[12]);
            g.AddEdge(v[9],  o[12]);
            g.AddEdge(o[12], y);
        }
    }
}
