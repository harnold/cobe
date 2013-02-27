using System;
using System.Collections;

namespace COBE {

    [Serializable]
    public enum OperationType {
        AddOp, AddcOp, SubOp, SubbOp,
        MulOp, DivOp, ModOp, NegOp,
        AndOp, OrOp, XorOp, NotOp, ShlOp, ShrOp,
        ConstOp, ConvOp, MoveOp, LoadOp, StoreOp
    }

    public struct OperationInfo {

        public OperationInfo(
            OperationType type, bool isCommutative, bool isBinary) {

            this.type = type;
            this.isCommutative = isCommutative;
            this.isBinary = isBinary;
        }

        public static bool IsCommutative(OperationType type) {
            return operationInfo[(int) type].isCommutative;
        }

        public static bool IsBinary(OperationType type) {
            return operationInfo[(int) type].isBinary;
        }

        public static bool IsUnary(OperationType type) {
            return !operationInfo[(int) type].isBinary;
        }

        private static readonly OperationInfo[] operationInfo = {
            new OperationInfo(OperationType.AddOp,   true,  true),
            new OperationInfo(OperationType.AddcOp,  true,  true),
            new OperationInfo(OperationType.SubOp,   false, true),
            new OperationInfo(OperationType.SubbOp,  false, true),
            new OperationInfo(OperationType.MulOp,   true,  true),
            new OperationInfo(OperationType.DivOp,   false, true),
            new OperationInfo(OperationType.ModOp,   false, true),
            new OperationInfo(OperationType.NegOp,   false, false),
            new OperationInfo(OperationType.AndOp,   true,  true),
            new OperationInfo(OperationType.OrOp,    true,  true),
            new OperationInfo(OperationType.XorOp,   true,  true),
            new OperationInfo(OperationType.NotOp,   false, false),
            new OperationInfo(OperationType.ShlOp,   false, true),
            new OperationInfo(OperationType.ShrOp,   false, true),
            new OperationInfo(OperationType.ConstOp, false, false),
            new OperationInfo(OperationType.ConvOp,  false, false),
            new OperationInfo(OperationType.MoveOp,  false, false),
            new OperationInfo(OperationType.LoadOp,  false, false),
            new OperationInfo(OperationType.StoreOp, false, true)
        };

        private OperationType type;
        private bool isCommutative;
        private bool isBinary;
    }

    [Serializable]
    public abstract class OperationNode: Graph.Node, ICloneable {

        private OperationType operationType;

        public OperationType OperationType {
            get { return operationType; }
        }

        public ValueNode ResultValue {
            get { return (ValueNode) OutNodes[0]; }
        }

        public IList OperandValues {
            get { return InNodes; }
        }

        public OperationNode(OperationType operationType) {
            this.operationType = operationType;
        }

        public virtual bool IsCoverableWith(OperationNode patternOp) {
            return this.OperationType == patternOp.OperationType;
        }

        public abstract object Clone();

        public abstract bool Cover(
            OperationNode patternOp, CoveringInfo coveringInfo);
    }

    [Serializable]
    public class UnaryOperationNode: OperationNode {

        public ValueNode OperandValue {
            get { return (ValueNode) OperandValues[0]; }
        }

        public UnaryOperationNode(OperationType operationType)
            : base(operationType) {
        }

        public override bool Cover(
            OperationNode patternOp, CoveringInfo coveringInfo) {

            bool result = false;

            if (IsCoverableWith(patternOp)) {

                UnaryOperationNode unaryPatternOp = (UnaryOperationNode) patternOp;

                if (OperandValue.Cover(unaryPatternOp.OperandValue, coveringInfo)) {
                    coveringInfo.CoveredOperations.Add(this);
                    result = true;
                }
            }

            return result;
        }

        public override object Clone() {
            return new UnaryOperationNode(OperationType);
        }

    }

    [Serializable]
    public class BinaryOperationNode: OperationNode {

        public ValueNode LeftOperandValue {
            get { return (ValueNode) OperandValues[0]; }
        }

        public ValueNode RightOperandValue {
            get { return (ValueNode) OperandValues[1]; }
        }

        public BinaryOperationNode(OperationType operationType)
            : base(operationType) {
        }

        public override bool Cover(
            OperationNode patternOp, CoveringInfo coveringInfo) {

            bool result = false;

            if (IsCoverableWith(patternOp)) {

                BinaryOperationNode binaryPatternOp =
                    (BinaryOperationNode) patternOp;

                if (LeftOperandValue.Cover(binaryPatternOp.LeftOperandValue,
                                           coveringInfo) &&
                    RightOperandValue.Cover(binaryPatternOp.RightOperandValue,
                                            coveringInfo)) {

                    coveringInfo.CoveredOperations.Add(this);
                    result = true;
                }
            }

            return result;
        }

        public override object Clone() {
            return new BinaryOperationNode(OperationType);
        }
    }

    [Serializable]
    public class AddOperationNode: BinaryOperationNode {

        public AddOperationNode()
            : base(OperationType.AddOp) { }

        public override object Clone() {
            return new AddOperationNode();
        }
    }

    [Serializable]
    public class AddcOperationNode: BinaryOperationNode {

        public AddcOperationNode()
            : base(OperationType.AddcOp) { }

        public override object Clone() {
            return new AddcOperationNode();
        }
    }

    [Serializable]
    public class SubOperationNode: BinaryOperationNode {

        public SubOperationNode()
            : base(OperationType.SubOp) { }

        public override object Clone() {
            return new SubOperationNode();
        }
    }

    [Serializable]
    public class SubbOperationNode: BinaryOperationNode {

        public SubbOperationNode()
            : base(OperationType.SubbOp) { }

        public override object Clone() {
            return new SubbOperationNode();
        }
    }

    [Serializable]
    public class MulOperationNode: BinaryOperationNode {

        public MulOperationNode()
            : base(OperationType.MulOp) { }

        public override object Clone() {
            return new MulOperationNode();
        }
    }

    [Serializable]
    public class DivOperationNode: BinaryOperationNode {

        public DivOperationNode()
            : base(OperationType.DivOp) { }

        public override object Clone() {
            return new DivOperationNode();
        }
    }

    [Serializable]
    public class ModOperationNode: BinaryOperationNode {

        public ModOperationNode()
            : base(OperationType.ModOp) { }

        public override object Clone() {
            return new ModOperationNode();
        }
    }

    [Serializable]
    public class NegOperationNode: UnaryOperationNode {

        public NegOperationNode()
            : base(OperationType.NegOp) { }

        public override object Clone() {
            return new NegOperationNode();
        }
    }

    [Serializable]
    public class AndOperationNode: BinaryOperationNode {

        public AndOperationNode()
            : base(OperationType.AndOp) { }

        public override object Clone() {
            return new AndOperationNode();
        }
    }

    [Serializable]
    public class OrOperationNode: BinaryOperationNode {

        public OrOperationNode()
            : base(OperationType.OrOp) { }

        public override object Clone() {
            return new OrOperationNode();
        }
    }

    [Serializable]
    public class XorOperationNode: BinaryOperationNode {

        public XorOperationNode()
            : base(OperationType.XorOp) { }

        public override object Clone() {
            return new XorOperationNode();
        }
    }

    [Serializable]
    public class NotOperationNode: UnaryOperationNode {

        public NotOperationNode()
            : base(OperationType.NotOp) { }

        public override object Clone() {
            return new NotOperationNode();
        }
    }

    [Serializable]
    public class ShlOperationNode: BinaryOperationNode {

        public ShlOperationNode()
            : base(OperationType.ShlOp) { }

        public override object Clone() {
            return new ShlOperationNode();
        }
    }

    [Serializable]
    public class ShrOperationNode: BinaryOperationNode {

        public ShrOperationNode()
            : base(OperationType.ShrOp) { }

        public override object Clone() {
            return new ShrOperationNode();
        }
    }

    [Serializable]
    public class ConstOperationNode: UnaryOperationNode {

        public ConstOperationNode()
            : base(OperationType.ConstOp) { }

        public override object Clone() {
            return new ConstOperationNode();
        }
    }

    [Serializable]
    public class ConvOperationNode: UnaryOperationNode {

        public ConvOperationNode()
            : base(OperationType.ConvOp) { }

        public override object Clone() {
            return new ConvOperationNode();
        }
    }

    [Serializable]
    public class MoveOperationNode: UnaryOperationNode {

        public MoveOperationNode()
            : base(OperationType.MoveOp) { }

        public override object Clone() {
            return new MoveOperationNode();
        }
    }

    [Serializable]
    public class LoadOperationNode: UnaryOperationNode {

        public LoadOperationNode()
            : base(OperationType.LoadOp) { }

        public override object Clone() {
            return new LoadOperationNode();
        }
    }

    [Serializable]
    public class StoreOperationNode: BinaryOperationNode {

        public StoreOperationNode()
            : base(OperationType.StoreOp) { }

        public override object Clone() {
            return new StoreOperationNode();
        }
    }
}
