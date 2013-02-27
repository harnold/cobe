using System;
using System.Collections;

namespace COBE {

    [Serializable]
    public enum Datatype {
        CharValue, ShortValue, IntValue, LongValue,
        UShortValue, UIntValue, ULongValue,
        AddressValue, BitValue, FloatValue, DoubleValue
    }

    [Serializable]
    public enum StorageClass {
        MemoryValue, RegisterValue, ConstantValue
    }

    [Serializable]
    public class ValueNode: Graph.Node, ICloneable {

        private Datatype datatype;
        private StorageClass storageClass;
        private bool outputFlag = false;

        public bool OutputFlag {
            get { return outputFlag; }
            set { outputFlag = value; }
        }

        public Datatype Datatype {
            get { return datatype; }
        }

        public StorageClass StorageClass {
            get { return storageClass; }
        }

        public OperationNode ProducingOperation {
            get { return (OperationNode) InNodes[0]; }
        }

        public IList ConsumingOperations {
            get { return OutNodes; }
        }

        public InstructionNode ProducingInstruction {
            get { return (InstructionNode) InNodes[0]; }
        }

        public IList ConsumingInstructions {
            get { return OutNodes; }
        }

        public ValueNode(Datatype datatype, StorageClass storageClass) {
            this.datatype = datatype;
            this.storageClass = storageClass;
        }

        public bool IsInputValue() {
            return InNodes.Count == 0;
        }

        public bool IsOutputValue() {
            return (OutNodes.Count == 0) || (outputFlag == true);
        }

        public virtual bool IsCoverableWith(ValueNode patternValue) {
            return IsCoverableWithDatatype(patternValue.Datatype) &&
                   IsCoverableWithStorageClass(patternValue.StorageClass);
        }

        protected virtual bool IsCoverableWithDatatype(Datatype datatype) {
            return this.Datatype == datatype;
        }

        protected virtual bool IsCoverableWithStorageClass(
            StorageClass storageClass) {
            return this.StorageClass == storageClass;
        }

        public virtual bool Cover(
            ValueNode patternValue, CoveringInfo coveringInfo) {

            bool result = false;

            if (IsCoverableWith(patternValue)) {

                if (!IsInputValue() && !patternValue.IsInputValue()) {

                    if (patternValue.OutNodes.Count == 0 ||
                        ConsumingOperations.Count <= 1 && !OutputFlag) {

                        if (ProducingOperation.Cover(
                                patternValue.ProducingOperation,
                                coveringInfo)) {

                            result = true;
                        }
                    }

                } else if (patternValue.IsInputValue()) {
                    coveringInfo.OperandValues.Add(this);
                    result = true;
                }

                if (result == true)
                    coveringInfo.CoveredValues.Add(this);
            }

            return result;
        }

        public virtual object Clone() {
            return new ValueNode(datatype, storageClass);
        }
    }

    [Serializable]
    public class RegisterValueNode: ValueNode {

        public RegisterValueNode(Datatype type)
            : base(type, StorageClass.RegisterValue) {
        }

        public override object Clone() {
            return new RegisterValueNode(Datatype);
        }
    }

    [Serializable]
    public class CharRegisterNode: RegisterValueNode {

        public CharRegisterNode()
            : base(Datatype.CharValue) {
        }

        public override object Clone() {
            return new CharRegisterNode();
        }
    }

    [Serializable]
    public class ShortRegisterNode: RegisterValueNode {

        public ShortRegisterNode()
            : base(Datatype.ShortValue) {
        }

        public override object Clone() {
            return new ShortRegisterNode();
        }
    }

    [Serializable]
    public class IntRegisterNode: RegisterValueNode {

        public IntRegisterNode()
            : base(Datatype.IntValue) {
        }

        public override object Clone() {
            return new IntRegisterNode();
        }
    }

    [Serializable]
    public class LongRegisterNode: RegisterValueNode {

        public LongRegisterNode()
            : base(Datatype.LongValue) {
        }

        public override object Clone() {
            return new LongRegisterNode();
        }
    }

    [Serializable]
    public class UShortRegisterNode: RegisterValueNode {

        public UShortRegisterNode()
            : base(Datatype.UShortValue) {
        }

        public override object Clone() {
            return new UShortRegisterNode();
        }
    }

    [Serializable]
    public class UIntRegisterNode: RegisterValueNode {

        public UIntRegisterNode()
            : base(Datatype.UIntValue) {
        }

        public override object Clone() {
            return new UIntRegisterNode();
        }
    }

    [Serializable]
    public class ULongRegisterNode: RegisterValueNode {

        public ULongRegisterNode()
            : base(Datatype.ULongValue) {
        }

        public override object Clone() {
            return new ULongRegisterNode();
        }
    }

    [Serializable]
    public class AddressRegisterNode: RegisterValueNode {

        public AddressRegisterNode()
            : base(Datatype.AddressValue) {
        }

        public override object Clone() {
            return new AddressRegisterNode();
        }
    }

    [Serializable]
    public class BitRegisterNode: RegisterValueNode {

        public BitRegisterNode()
            : base(Datatype.BitValue) {
        }

        public override object Clone() {
            return new BitRegisterNode();
        }
    }

    [Serializable]
    public class FloatRegisterNode: RegisterValueNode {

        public FloatRegisterNode()
            : base(Datatype.FloatValue) {
        }

        public override object Clone() {
            return new FloatRegisterNode();
        }
    }

    [Serializable]
    public class DoubleRegisterNode: RegisterValueNode {

        public DoubleRegisterNode()
            : base(Datatype.DoubleValue) {
        }

        public override object Clone() {
            return new DoubleRegisterNode();
        }
    }

    [Serializable]
    public class MemoryValueNode: ValueNode {

        private string id;

        public string Id {
            get { return id; }
        }

        public MemoryValueNode(Datatype type, string id)
            : base(type, StorageClass.MemoryValue) {
            this.id = id;
        }

        public override object Clone() {
            return new MemoryValueNode(Datatype, id);
        }

        public override string ToString() {
            return id;
        }
    }

    [Serializable]
    public class ConstantValueNode: ValueNode {

        public ConstantValueNode(Datatype type)
            : base(type, StorageClass.ConstantValue) {
        }

        public override object Clone() {
            return new ConstantValueNode(Datatype);
        }
    }

    [Serializable]
    public class CharConstantNode: ConstantValueNode {

        private byte value;

        public byte Value {
            get { return value; }
        }

        public CharConstantNode(byte value)
            : base(Datatype.CharValue) {
            this.value = value;
        }

        public override object Clone() {
            return new CharConstantNode(value);
        }

        public override string ToString() {
            return value.ToString();
        }
    }

    [Serializable]
    public class ShortConstantNode: ConstantValueNode {

        private short value;

        public short Value {
            get { return value; }
        }

        public ShortConstantNode(short value)
            : base(Datatype.ShortValue) {
            this.value = value;
        }

        public override object Clone() {
            return new ShortConstantNode(value);
        }

        public override string ToString() {
            return value.ToString();
        }
    }

    [Serializable]
    public class IntConstantNode: ConstantValueNode {

        private int value;

        public int Value {
            get { return value; }
        }

        public IntConstantNode(int value)
            : base(Datatype.IntValue) {
            this.value = value;
        }

        public override object Clone() {
            return new IntConstantNode(value);
        }

        public override string ToString() {
            return value.ToString();
        }
    }

    [Serializable]
    public class LongConstantNode: ConstantValueNode {

        private long value;

        public long Value {
            get { return value; }
        }

        public LongConstantNode(long value)
            : base(Datatype.LongValue) {
            this.value = value;
        }

        public override object Clone() {
            return new LongConstantNode(value);
        }

        public override string ToString() {
            return value.ToString();
        }
    }

    [Serializable]
    public class UShortConstantNode: ConstantValueNode {

        private ushort value;

        public ushort Value {
            get { return value; }
        }

        public UShortConstantNode(ushort value)
            : base(Datatype.UShortValue) {
            this.value = value;
        }

        public override object Clone() {
            return new UShortConstantNode(value);
        }

        public override string ToString() {
            return value.ToString();
        }
    }

    [Serializable]
    public class UIntConstantNode: ConstantValueNode {

        private uint value;

        public uint Value {
            get { return value; }
        }

        public UIntConstantNode(uint value)
            : base(Datatype.UIntValue) {
            this.value = value;
        }

        public override object Clone() {
            return new UIntConstantNode(value);
        }

        public override string ToString() {
            return value.ToString();
        }
    }

    [Serializable]
    public class ULongConstantNode: ConstantValueNode {

        private ulong value;

        public ulong Value {
            get { return value; }
        }

        public ULongConstantNode(ulong value)
            : base(Datatype.ULongValue) {
            this.value = value;
        }

        public override object Clone() {
            return new ULongConstantNode(value);
        }

        public override string ToString() {
            return value.ToString();
        }
    }

    [Serializable]
    public class AddressConstantNode: ConstantValueNode {

        private ulong value;

        public ulong Value {
            get { return value; }
        }

        public AddressConstantNode(ulong value)
            : base(Datatype.AddressValue) {
            this.value = value;
        }

        public override object Clone() {
            return new AddressConstantNode(value);
        }

        public override string ToString() {
            return value.ToString();
        }
    }

    [Serializable]
    public class BitConstantNode: ConstantValueNode {

        private bool value;

        public bool Value {
            get { return value; }
        }

        public BitConstantNode(bool value)
            : base(Datatype.BitValue) {
            this.value = value;
        }

        public override object Clone() {
            return new BitConstantNode(value);
        }

        public override string ToString() {
            return value.ToString();
        }
    }

    [Serializable]
    public class FloatConstantNode: ConstantValueNode {

        private float value;

        public float Value {
            get { return value; }
        }

        public FloatConstantNode(float value)
            : base(Datatype.FloatValue) {
            this.value = value;
        }

        public override object Clone() {
            return new FloatConstantNode(value);
        }

        public override string ToString() {
            return value.ToString();
        }
    }

    [Serializable]
    public class DoubleConstantNode: ConstantValueNode {

        private double value;

        public double Value {
            get { return value; }
        }

        public DoubleConstantNode(double value)
            : base(Datatype.DoubleValue) {
            this.value = value;
        }

        public override object Clone() {
            return new DoubleConstantNode(value);
        }

        public override string ToString() {
            return value.ToString();
        }
    }

    [Serializable]
    public class CharMemoryNode: MemoryValueNode {

        public CharMemoryNode(string id)
            : base(Datatype.CharValue, id) {
        }

        public override object Clone() {
            return new CharMemoryNode(Id);
        }
    }

    [Serializable]
    public class ShortMemoryNode: MemoryValueNode {

        public ShortMemoryNode(string id)
            : base(Datatype.ShortValue, id) {
        }

        public override object Clone() {
            return new ShortMemoryNode(Id);
        }
    }

    [Serializable]
    public class IntMemoryNode: MemoryValueNode {

        public IntMemoryNode(string id)
            : base(Datatype.IntValue, id) {
        }

        public override object Clone() {
            return new IntMemoryNode(Id);
        }
    }

    [Serializable]
    public class LongMemoryNode: MemoryValueNode {

        public LongMemoryNode(string id)
            : base(Datatype.LongValue, id) {
        }

        public override object Clone() {
            return new LongMemoryNode(Id);
        }
    }

    [Serializable]
    public class UShortMemoryNode: MemoryValueNode {

        public UShortMemoryNode(string id)
            : base(Datatype.UShortValue, id) {
        }

        public override object Clone() {
            return new UShortMemoryNode(Id);
        }
    }

    [Serializable]
    public class UIntMemoryNode: MemoryValueNode {

        public UIntMemoryNode(string id)
            : base(Datatype.UIntValue, id) {
        }

        public override object Clone() {
            return new UIntMemoryNode(Id);
        }
    }

    [Serializable]
    public class ULongMemoryNode: MemoryValueNode {

        public ULongMemoryNode(string id)
            : base(Datatype.ULongValue, id) {
        }

        public override object Clone() {
            return new ULongMemoryNode(Id);
        }
    }

    [Serializable]
    public class AddressMemoryNode: MemoryValueNode {

        public AddressMemoryNode(string id)
            : base(Datatype.AddressValue, id) {
        }

        public override object Clone() {
            return new AddressMemoryNode(Id);
        }
    }

    [Serializable]
    public class BitMemoryNode: MemoryValueNode {

        public BitMemoryNode(string id)
            : base(Datatype.BitValue, id) {
        }

        public override object Clone() {
            return new BitMemoryNode(Id);
        }
    }

    [Serializable]
    public class FloatMemoryNode: MemoryValueNode {

        public FloatMemoryNode(string id)
            : base(Datatype.FloatValue, id) {
        }

        public override object Clone() {
            return new FloatMemoryNode(Id);
        }
    }

    [Serializable]
    public class DoubleMemoryNode: MemoryValueNode {

        public DoubleMemoryNode(string id)
            : base(Datatype.DoubleValue, id) {
        }

        public override object Clone() {
            return new DoubleMemoryNode(Id);
        }
    }

    public class TestRegisterNode: RegisterValueNode {

        private string id;

        public string Id {
            get { return id; }
        }

        public TestRegisterNode(string id, Datatype type)
            : base(type) {
            this.id = id;
        }

        public override object Clone() {
            return new TestRegisterNode(Id, Datatype);
        }
    }

}
