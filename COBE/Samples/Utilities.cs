using System;
using System.Collections;

namespace COBE.Samples {
    
    public class Utilities {
    
        public static Instruction CreateUnaryInstruction(
            string mnemonic, OperationNode op, int exUnit,
            ValueNode vres, RegisterSet vresRegs,
            ValueNode v1, RegisterSet v1Regs) {            
            
            InstructionPattern ip = new InstructionPattern();
            
            ip.AddNode(op);
            ip.AddNode(vres);
            ip.AddNode(v1);
            ip.AddEdge(op, vres);
            ip.AddEdge(v1, op);
            
            ip.OperandValues.Add(v1);
            ip.ResultValue = vres;
            
            Instruction i = new Instruction(mnemonic, ip);
            
            i.ExecutionUnit = exUnit;
            i.ResultRegisters = vresRegs;
            i.OperandsRegisters[0] = v1Regs;
            
            return i;
        }
        
        public static Instruction CreateLeftTernaryInstruction(
            string mnemonic, OperationNode op1, OperationNode op2, int exUnit,
            ValueNode vres, RegisterSet vresRegs,
            ValueNode v1, RegisterSet v1Regs,
            ValueNode v2, RegisterSet v2Regs,
            ValueNode v3, RegisterSet v3Regs) {
            
            InstructionPattern ip = new InstructionPattern();
                
            ip.AddNode(op1);
            ip.AddNode(op2);
            ip.AddNode(vres);
            ip.AddNode(v1);
            ip.AddNode(v2);
            ip.AddNode(v3);
            
            ValueNode iv1 = new RegisterValueNode(v1.Datatype);
            
            ip.AddNode(iv1);
                
            ip.AddEdge(v1, op1);
            ip.AddEdge(v2, op1);
            ip.AddEdge(op1, iv1);
            ip.AddEdge(iv1, op2);
            ip.AddEdge(v3, op2);
            ip.AddEdge(op2, vres);
            
            ip.OperandValues.Add(v1);
            ip.OperandValues.Add(v2);
            ip.OperandValues.Add(v3);
            ip.ResultValue = vres;
        
            Instruction i = new Instruction(mnemonic, ip);
            
            i.ExecutionUnit = exUnit;
            i.ResultRegisters = vresRegs;
            i.OperandsRegisters[0] = v1Regs;
            i.OperandsRegisters[1] = v2Regs;
            i.OperandsRegisters[2] = v3Regs;
            
            return i;
        }
            
        public static Instruction CreateLeftTernaryInstruction(
            string mnemonic, OperationNode op1, OperationNode op2, int exUnit,
            ValueNode vres, RegisterSet vresRegs,
            ValueNode v1, RegisterSet v1Regs,
            ValueNode v2, RegisterSet v2Regs,
            Datatype iv1Type) {
            
            InstructionPattern ip = new InstructionPattern();
                
            ip.AddNode(op1);
            ip.AddNode(op2);
            ip.AddNode(vres);
            ip.AddNode(v1);
            ip.AddNode(v2);
            
            ValueNode iv1 = new RegisterValueNode(iv1Type);
            
            ip.AddNode(iv1);
                
            ip.AddEdge(v1, op1);
            ip.AddEdge(op1, iv1);
            ip.AddEdge(iv1, op2);
            ip.AddEdge(v2, op2);
            ip.AddEdge(op2, vres);
            
            ip.OperandValues.Add(v1);
            ip.OperandValues.Add(v2);
            ip.ResultValue = vres;
        
            Instruction i = new Instruction(mnemonic, ip);
            
            i.ExecutionUnit = exUnit;
            i.ResultRegisters = vresRegs;
            i.OperandsRegisters[0] = v1Regs;
            i.OperandsRegisters[1] = v2Regs;
            
            return i;
        }
        
        public static Instruction CreateRightTernaryInstruction(
            string mnemonic, OperationNode op1, OperationNode op2, int exUnit,
            ValueNode vres, RegisterSet vresRegs,
            ValueNode v1, RegisterSet v1Regs,
            ValueNode v2, RegisterSet v2Regs,
            ValueNode v3, RegisterSet v3Regs) {
            
            InstructionPattern ip = new InstructionPattern();
                
            ip.AddNode(op1);
            ip.AddNode(op2);
            ip.AddNode(vres);
            ip.AddNode(v1);
            ip.AddNode(v2);
            ip.AddNode(v3);
            
            ValueNode iv1 = new RegisterValueNode(v2.Datatype);
            
            ip.AddNode(iv1);
                
            ip.AddEdge(v2, op1);
            ip.AddEdge(v3, op1);
            ip.AddEdge(op1, iv1);
            ip.AddEdge(v1, op2);
            ip.AddEdge(iv1, op2);
            ip.AddEdge(op2, vres);
            
            ip.OperandValues.Add(v1);
            ip.OperandValues.Add(v2);
            ip.OperandValues.Add(v3);
            ip.ResultValue = vres;
        
            Instruction i = new Instruction(mnemonic, ip);
            
            i.ExecutionUnit = exUnit;
            i.ResultRegisters = vresRegs;
            i.OperandsRegisters[0] = v1Regs;
            i.OperandsRegisters[1] = v2Regs;
            i.OperandsRegisters[2] = v3Regs;
            
            return i;
        }
            
        public static Instruction CreateRightTernaryInstruction(
            string mnemonic, OperationNode op1, OperationNode op2, int exUnit,
            ValueNode vres, RegisterSet vresRegs,
            ValueNode v1, RegisterSet v1Regs,
            ValueNode v2, RegisterSet v2Regs,
            Datatype iv1Type) {
            
            InstructionPattern ip = new InstructionPattern();
                
            ip.AddNode(op1);
            ip.AddNode(op2);
            ip.AddNode(vres);
            ip.AddNode(v1);
            ip.AddNode(v2);
            
            ValueNode iv1 = new RegisterValueNode(iv1Type);
            
            ip.AddNode(iv1);
                
            ip.AddEdge(v2, op1);
            ip.AddEdge(op1, iv1);
            ip.AddEdge(v1, op2);
            ip.AddEdge(iv1, op2);
            ip.AddEdge(op2, vres);
            
            ip.OperandValues.Add(v1);
            ip.OperandValues.Add(v2);
            ip.ResultValue = vres;
        
            Instruction i = new Instruction(mnemonic, ip);
            
            i.ExecutionUnit = exUnit;
            i.ResultRegisters = vresRegs;
            i.OperandsRegisters[0] = v1Regs;
            i.OperandsRegisters[1] = v2Regs;
            
            return i;
        }
        
        public static Instruction CreateQuadInstruction(
            string mnemonic, OperationNode op1, OperationNode op2,
            OperationNode op3, int exUnit,
            ValueNode vres, RegisterSet vresRegs,
            ValueNode v1, RegisterSet v1Regs,
            ValueNode v2, RegisterSet v2Regs,
            Datatype iv1Type, Datatype iv2Type) {
        
            InstructionPattern ip = new InstructionPattern();
                
            ip.AddNode(op1);
            ip.AddNode(op2);
            ip.AddNode(op3);
            ip.AddNode(vres);
            ip.AddNode(v1);
            ip.AddNode(v2);
            
            ValueNode iv1 = new RegisterValueNode(iv1Type);
            ValueNode iv2 = new RegisterValueNode(iv2Type);
            
            ip.AddNode(iv1);
            ip.AddNode(iv2);
                
            ip.AddEdge(v1, op1);
            ip.AddEdge(v2, op2);
            ip.AddEdge(op1, iv1);
            ip.AddEdge(op2, iv2);
            ip.AddEdge(iv1, op3);
            ip.AddEdge(iv2, op3);
            ip.AddEdge(op3, vres);
            
            ip.OperandValues.Add(v1);
            ip.OperandValues.Add(v2);
            ip.ResultValue = vres;
        
            Instruction i = new Instruction(mnemonic, ip);
            
            i.ExecutionUnit = exUnit;
            i.ResultRegisters = vresRegs;
            i.OperandsRegisters[0] = v1Regs;
            i.OperandsRegisters[1] = v2Regs;
            
            return i;
        }
        
        public static Instruction CreateBinaryRightConstantInstruction(
            string mnemonic, OperationNode op1, OperationNode op2, int exUnit,
            ValueNode vres, RegisterSet vresRegs,
            ValueNode v1, RegisterSet v1Regs,
            ValueNode v2, RegisterSet v2Regs) {
            
            InstructionPattern ip = new InstructionPattern();
                
            ip.AddNode(op1);
            ip.AddNode(op2);
            ip.AddNode(vres);
            ip.AddNode(v1);
            ip.AddNode(v2);
            
            ValueNode iv1 = new RegisterValueNode(v2.Datatype);
            
            ip.AddNode(iv1);
                
            ip.AddEdge(v2, op1);
            ip.AddEdge(op1, iv1);
            ip.AddEdge(v1, op2);
            ip.AddEdge(iv1, op2);
            ip.AddEdge(op2, vres);
            
            ip.OperandValues.Add(v1);
            ip.OperandValues.Add(v2);
            ip.ResultValue = vres;
        
            Instruction i = new Instruction(mnemonic, ip);
            
            i.ExecutionUnit = exUnit;
            i.ResultRegisters = vresRegs;
            i.OperandsRegisters[0] = v1Regs;
            i.OperandsRegisters[1] = v2Regs;
            
            return i;
        }
            
        public static Instruction CreateBinaryInstruction(
            string mnemonic, OperationNode op, int exUnit,
            ValueNode vres, RegisterSet vresRegs,
            ValueNode v1, RegisterSet v1Regs,
            ValueNode v2, RegisterSet v2Regs) {
            
            InstructionPattern ip = new InstructionPattern();
            
            ip.AddNode(op);
            ip.AddNode(vres);
            ip.AddNode(v1);
            ip.AddNode(v2);
            ip.AddEdge(op, vres);
            ip.AddEdge(v1, op);
            ip.AddEdge(v2, op);
            
            ip.OperandValues.Add(v1);
            ip.OperandValues.Add(v2);
            ip.ResultValue = vres;
            
            Instruction i = new Instruction(mnemonic, ip);
            
            i.ExecutionUnit = exUnit;
            i.ResultRegisters = vresRegs;
            i.OperandsRegisters[0] = v1Regs;
            i.OperandsRegisters[1] = v2Regs;
            
            return i;
        }
    }
}

