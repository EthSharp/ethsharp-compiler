using System;
using System.Collections.Generic;
using EthSharp.ContractDevelopment;

namespace EthSharp.Compiler
{
    public class EthSharpAssembly
    {
        public List<EthSharpAssemblyItem> Items { get; private set; }

        public EthSharpAssembly()
        {
            Items = new List<EthSharpAssemblyItem>();
        }

        public EvmByteCode Assemble()
        {
            // TODO: Handle longer pushes for larger data.
            // i.e. use PUSH2-PUSHXX instead of jsut PUSH1
            EvmByteCode ret = new EvmByteCode();
            foreach (var item in Items)
            {
                Console.WriteLine(item.Data.ByteLength);
                switch (item.Type)
                {
                    case AssemblyItemType.Operation:
                        ret.ByteCode.Add((byte) item.Instruction);
                        break;
                    case AssemblyItemType.PushString:
                    {
                        ret.ByteCode.Add((byte) EvmInstruction.PUSH32);
                        uint ii = 0;
                        throw new NotImplementedException();
                    }
                    case AssemblyItemType.Push:
                    {
                        ret.ByteCode.Add((byte)EvmInstruction.PUSH1);
                        ret.ByteCode.Add((byte)item.Data.ToInt());
                        break;
                    }
                    case AssemblyItemType.PushTag:
                    {
                        ret.ByteCode.Add((byte)EvmInstruction.PUSH1);
                        ret.ByteCode.Add((byte) 0); // I think we don't actually have to add 0 - maybe solidity is just doing this to extend size of bytecode (which is array)
                        break;
                    }
                    case AssemblyItemType.PushData:
                        throw new NotImplementedException();
                    case AssemblyItemType.PushSub:
                        ret.ByteCode.Add((byte)EvmInstruction.PUSH1);
                        ret.ByteCode.Add((byte)0);
                        break;
                    case AssemblyItemType.PushSubSize:
                    {

                        //auto s = m_subs.at(size_t(i.data()))->assemble().bytecode.size();
                        //i.setPushedValue(u256(s));
                        //byte b = max<unsigned>(1, dev::bytesRequired(s));
                        //ret.bytecode.push_back((byte)Instruction::PUSH1 - 1 + b);
                        //ret.bytecode.resize(ret.bytecode.size() + b);
                        //bytesRef byr(&ret.bytecode.back() + 1 - b, b);
                        //toBigEndian(s, byr);
                        //break;
                        throw new NotImplementedException();
                    }
                    case AssemblyItemType.PushProgramSize:
                    {
                        throw new NotImplementedException();
                    }
                    case AssemblyItemType.PushLibraryAddress:
                        throw new NotImplementedException();
                    case AssemblyItemType.Tag:
                        //assertThrow(i.data() != 0, AssemblyException, "");
                        //assertThrow(i.splitForeignPushTag().first == size_t(-1), AssemblyException, "Foreign tag.");
                        //assertThrow(ret.bytecode.size() < 0xffffffffL, AssemblyException, "Tag too large.");
                        //m_tagPositionsInBytecode[size_t(i.data())] = ret.bytecode.size();
                        //ret.bytecode.push_back((byte)Instruction::JUMPDEST);
                        throw new NotImplementedException();
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            throw new NotImplementedException();
        }

        //public EthSharpAssemblyItem NewData(byte[] value)
        //{
        //    //AssemblyItem newData(bytes const& _data) { h256 h(dev::keccak256(asString(_data))); m_data[h] = _data; return AssemblyItem(PushData, h);

        //    throw new NotImplementedException();
        //}

        public void Append(EthSharpAssemblyItem assemblyItem)
        {
            Items.Add(assemblyItem);
        }

        public void Append(UInt256 value)
        {
            Items.Add(new EthSharpAssemblyItem(value));
        }

        public void Append(EvmInstruction instruction)
        {
            Items.Add(new EthSharpAssemblyItem(instruction));
        }

        //public void Append(byte[] value)
        //{
        //    Items.Add(NewData(value));
        //}
    }
}
