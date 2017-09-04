using System;
using System.Collections.Generic;
using System.Linq;
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
            EvmByteCode ret = new EvmByteCode();

            Dictionary<int, List<int>> jumpFromLocations = new Dictionary<int, List<int>>();
            Dictionary<int, int> tagLocations = new Dictionary<int, int>();
            foreach (var item in Items)
            {
                switch (item.Type)
                {
                    case AssemblyItemType.Operation:
                        ret.ByteCode.Add((byte) item.Instruction);
                        break;
                    case AssemblyItemType.Push:
                    {
                        int length = item.Data.ByteLength;
                        var sizedPushInstruction = EvmInstruction.PUSH1 - 1 + length;
                        
                        ret.ByteCode.Add((byte)sizedPushInstruction);
                        ret.ByteCode.AddRange(item.Data.ToByteArrayBE().Skip(32-length).Take(length));
                        break;
                    }
                    case AssemblyItemType.PushTag:
                    {
                        ret.ByteCode.Add((byte)EvmInstruction.PUSH1);
                        if (jumpFromLocations.ContainsKey(item.Data.ToInt()))
                        {
                            jumpFromLocations[item.Data.ToInt()].Add(ret.ByteCode.Count);
                        }
                        else
                        {
                            jumpFromLocations[item.Data.ToInt()] = new List<int> { ret.ByteCode.Count };
                        }
                        ret.ByteCode.Add((byte) 0); //This will get replaced with the location to jump to!
                        break;
                    }
                    case AssemblyItemType.Tag:
                        tagLocations.Add(item.Data.ToInt(), ret.ByteCode.Count);
                        ret.ByteCode.Add((byte)EvmInstruction.JUMPDEST);
                        break;
                    default:
                        throw new NotImplementedException(); // TODO: Exception handling
                }
            }

            //Update all jumps to correct location
            foreach (var tag in tagLocations)
            {
                int tagId = tag.Key;
                int tagLocation = tag.Value;
                foreach (var jumpFrom in jumpFromLocations[tagId])
                {
                    ret.ByteCode[jumpFrom] = (byte)tagLocation;
                }
            }

            return ret;
        }

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
    }
}
