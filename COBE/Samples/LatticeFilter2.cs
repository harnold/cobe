using System.Collections;

namespace COBE.Samples {

    public class LatticeFilter2: IProgramGraphProvider {

        public ProgramGraph CreateUnrolledProgramGraph(int k) {

            ProgramGraph g = new ProgramGraph();

            ValueNode[] px = new ValueNode[k];
            ValueNode[] x  = new ValueNode[k];

            ValueNode[] py = new ValueNode[k];
            ValueNode[] my = new ValueNode[k];
            ValueNode[] y  = new ValueNode[k];

            OperationNode[] lx = new OperationNode[k];
            OperationNode[] sy = new OperationNode[k];

            for (int i = 0; i < k; i++) {

                px[i] = new FloatMemoryNode("x[" + i + "]");
                lx[i] = new LoadOperationNode();
                x[i]  = new FloatRegisterNode();

                g.AddNode(px[i]);
                g.AddNode(lx[i]);
                g.AddNode(x[i]);

                g.AddEdge(px[i], lx[i]);
                g.AddEdge(lx[i], x[i]);

                y[i]  = new FloatRegisterNode();
                py[i] = new FloatMemoryNode("y[" + i + "]");
                sy[i] = new StoreOperationNode();
                my[i] = new FloatMemoryNode(null);

                g.AddNode(y[i]);
                g.AddNode(py[i]);
                g.AddNode(sy[i]);
                g.AddNode(my[i]);

                g.AddEdge(y[i], sy[i]);
                g.AddEdge(py[i], sy[i]);
                g.AddEdge(sy[i], my[i]);
            }

            ValueNode[] x0 = new ValueNode[k + 1];
            ValueNode[] x1 = new ValueNode[k + 1];

            for (int i = 0; i < k + 1; i++) {

                x0[i] = new FloatRegisterNode();
                x1[i] = new FloatRegisterNode();

                g.AddNode(x0[i]);
                g.AddNode(x1[i]);
            }

            ValueNode px00 = new FloatMemoryNode("x0[0]");
            ValueNode px10 = new FloatMemoryNode("x1[0]");

            OperationNode lx00 = new LoadOperationNode();
            OperationNode lx10 = new LoadOperationNode();

            g.AddNode(px00);
            g.AddNode(px10);
            g.AddNode(lx00);
            g.AddNode(lx10);

            g.AddEdge(px00, lx00);
            g.AddEdge(lx00, x0[0]);
            g.AddEdge(px10, lx10);
            g.AddEdge(lx10, x1[0]);

            ValueNode px0k = new FloatMemoryNode("x0[" + k + "]");
            ValueNode px1k = new FloatMemoryNode("x1[" + k + "]");

            ValueNode mx0k = new FloatMemoryNode(null);
            ValueNode mx1k = new FloatMemoryNode(null);

            OperationNode sx0k = new StoreOperationNode();
            OperationNode sx1k = new StoreOperationNode();

            g.AddNode(px0k);
            g.AddNode(px1k);
            g.AddNode(mx0k);
            g.AddNode(mx1k);
            g.AddNode(sx0k);
            g.AddNode(sx1k);

            g.AddEdge(x0[k], sx0k);
            g.AddEdge(px0k, sx0k);
            g.AddEdge(sx0k, mx0k);
            g.AddEdge(x1[k], sx1k);
            g.AddEdge(px1k, sx1k);
            g.AddEdge(sx1k, mx1k);

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

                CreateSingleFilterLoop(
                    g, x[i], x0[i], x1[i], c0, c1, c2, c3, c4,
                    x0[i + 1], x1[i + 1], y[i]);
            }

            g.InitInputValues();
            g.InitOutputValues();

            return g;
        }

        public ProgramGraph CreateProgramGraph() {
            return CreateUnrolledProgramGraph(1);
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
