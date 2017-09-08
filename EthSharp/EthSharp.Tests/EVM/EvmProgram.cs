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
                case EvmInstruction.ADD:
                    Stack.Push((UInt256)Stack.Pop() + (UInt256) Stack.Pop());
                    break;
                default:
                    throw new Exception("Invalid Opcode");
            }
        }
    }
}