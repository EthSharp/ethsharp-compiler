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
