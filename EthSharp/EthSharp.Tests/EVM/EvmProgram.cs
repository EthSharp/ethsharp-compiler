using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using EthSharp;
using EthSharp.Compiler;
using EthSharp.ContractDevelopment;

namespace EthSharp.Tests.VM
{
    public class EvmProgram
    {
        public Stack<object> Stack { get; set; }
        public bool Stopped { get; set; }
        public IList<byte> ByteCode { get; private set; }
        public int Position { get; set; }

        public EvmProgram(IList<byte> byteCode)
        {
            ByteCode = byteCode;
        }

        public void Run()
        {
            while (!Stopped)
            {
                MakeStep();
            }
        }

        public void MakeStep()
        {
            switch ((EvmInstruction)ByteCode[Position])
            {
                case EvmInstruction.STOP:
                    Stopped = true;
                    break;
                case EvmInstruction.ADD:
                    Stack.Push((UInt256)Stack.Pop() + (UInt256)Stack.Pop());
                    break;
                case EvmInstruction.MUL:
                    Stack.Push((UInt256)Stack.Pop() * (UInt256)Stack.Pop());
                    break;
                case EvmInstruction.SUB:
                    Stack.Push((UInt256)Stack.Pop() - (UInt256)Stack.Pop()); // need to check order!
                    break;
                case EvmInstruction.DIV:
                    Stack.Push((UInt256)Stack.Pop() / (UInt256)Stack.Pop()); // need to check order!
                    break;
                case EvmInstruction.SDIV:
                case EvmInstruction.MOD:
                case EvmInstruction.SMOD:
                case EvmInstruction.ADDMOD:
                case EvmInstruction.MULMOD:
                case EvmInstruction.EXP:
                case EvmInstruction.SIGNEXTEND:
                    throw new NotImplementedException();
                case EvmInstruction.LT:
                case EvmInstruction.GT:
                case EvmInstruction.SLT:
                case EvmInstruction.SGT:
                case EvmInstruction.EQ:
                case EvmInstruction.ISZERO:
                case EvmInstruction.AND:
                case EvmInstruction.OR:
                case EvmInstruction.XOR:
                case EvmInstruction.NOT:
                case EvmInstruction.BYTE:
                    throw new NotImplementedException();
                case EvmInstruction.KECCAK256:
                    throw new NotImplementedException();
                case EvmInstruction.ADDRESS:
                case EvmInstruction.BALANCE:
                case EvmInstruction.ORIGIN:
                case EvmInstruction.CALLER:
                case EvmInstruction.CALLVALUE:
                case EvmInstruction.CALLDATALOAD:
                case EvmInstruction.CALLDATASIZE:
                case EvmInstruction.CALLDATACOPY:
                case EvmInstruction.CODESIZE:
                case EvmInstruction.CODECOPY:
                case EvmInstruction.GASPRICE:
                case EvmInstruction.EXTCODESIZE:
                case EvmInstruction.EXTCODECOPY:
                case EvmInstruction.RETURNDATASIZE:
                case EvmInstruction.RETURNDATACOPY:
                case EvmInstruction.BLOCKHASH:
                case EvmInstruction.COINBASE:
                case EvmInstruction.TIMESTAMP:
                case EvmInstruction.NUMBER:
                case EvmInstruction.DIFFICULTY:
                case EvmInstruction.GASLIMIT:
                case EvmInstruction.JUMPTO:
                case EvmInstruction.JUMPIF:
                case EvmInstruction.JUMPV:
                case EvmInstruction.JUMPSUB:
                case EvmInstruction.JUMPSUBV:
                case EvmInstruction.RETURNSUB:
                case EvmInstruction.POP:
                case EvmInstruction.MLOAD:
                case EvmInstruction.MSTORE:
                case EvmInstruction.MSTORE8:
                case EvmInstruction.SLOAD:
                case EvmInstruction.SSTORE:
                case EvmInstruction.JUMP:
                case EvmInstruction.JUMPI:
                case EvmInstruction.PC:
                case EvmInstruction.MSIZE:
                case EvmInstruction.GAS:
                case EvmInstruction.JUMPDEST:
                case EvmInstruction.BEGINSUB:
                case EvmInstruction.BEGINDATA:
                case EvmInstruction.PUSH1:
                case EvmInstruction.PUSH2:
                case EvmInstruction.PUSH3:
                case EvmInstruction.PUSH4:
                case EvmInstruction.PUSH5:
                case EvmInstruction.PUSH6:
                case EvmInstruction.PUSH7:
                case EvmInstruction.PUSH8:
                case EvmInstruction.PUSH9:
                case EvmInstruction.PUSH10:
                case EvmInstruction.PUSH11:
                case EvmInstruction.PUSH12:
                case EvmInstruction.PUSH13:
                case EvmInstruction.PUSH14:
                case EvmInstruction.PUSH15:
                case EvmInstruction.PUSH16:
                case EvmInstruction.PUSH17:
                case EvmInstruction.PUSH18:
                case EvmInstruction.PUSH19:
                case EvmInstruction.PUSH20:
                case EvmInstruction.PUSH21:
                case EvmInstruction.PUSH22:
                case EvmInstruction.PUSH23:
                case EvmInstruction.PUSH24:
                case EvmInstruction.PUSH25:
                case EvmInstruction.PUSH26:
                case EvmInstruction.PUSH27:
                case EvmInstruction.PUSH28:
                case EvmInstruction.PUSH29:
                case EvmInstruction.PUSH30:
                case EvmInstruction.PUSH31:
                case EvmInstruction.PUSH32:
                case EvmInstruction.DUP1:
                case EvmInstruction.DUP2:
                case EvmInstruction.DUP3:
                case EvmInstruction.DUP4:
                case EvmInstruction.DUP5:
                case EvmInstruction.DUP6:
                case EvmInstruction.DUP7:
                case EvmInstruction.DUP8:
                case EvmInstruction.DUP9:
                case EvmInstruction.DUP10:
                case EvmInstruction.DUP11:
                case EvmInstruction.DUP12:
                case EvmInstruction.DUP13:
                case EvmInstruction.DUP14:
                case EvmInstruction.DUP15:
                case EvmInstruction.DUP16:
                case EvmInstruction.SWAP1:
                case EvmInstruction.SWAP2:
                case EvmInstruction.SWAP3:
                case EvmInstruction.SWAP4:
                case EvmInstruction.SWAP5:
                case EvmInstruction.SWAP6:
                case EvmInstruction.SWAP7:
                case EvmInstruction.SWAP8:
                case EvmInstruction.SWAP9:
                case EvmInstruction.SWAP10:
                case EvmInstruction.SWAP11:
                case EvmInstruction.SWAP12:
                case EvmInstruction.SWAP13:
                case EvmInstruction.SWAP14:
                case EvmInstruction.SWAP15:
                case EvmInstruction.SWAP16:
                case EvmInstruction.LOG0:
                case EvmInstruction.LOG1:
                case EvmInstruction.LOG2:
                case EvmInstruction.LOG3:
                case EvmInstruction.LOG4:
                case EvmInstruction.CREATE:
                case EvmInstruction.CALL:
                case EvmInstruction.CALLCODE:
                case EvmInstruction.RETURN:
                case EvmInstruction.DELEGATECALL:
                case EvmInstruction.STATICCALL:
                case EvmInstruction.CREATE2:
                case EvmInstruction.REVERT:
                case EvmInstruction.INVALID:
                case EvmInstruction.SELFDESTRUCT:
                default:
                    throw new Exception("Invalid Opcode");
            }
        }
    }
}