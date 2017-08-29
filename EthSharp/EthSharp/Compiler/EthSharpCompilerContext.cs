using EthSharp.ContractDevelopment;

namespace EthSharp.Compiler
{
    public class EthSharpCompilerContext
    {
        private int _tagCount;
        public EthSharpAssembly Assembly { get; set; }

        public EthSharpCompilerContext()
        {
            Assembly = new EthSharpAssembly();
            _tagCount = 1;
        }

        public int GetNewTag()
        {
            return _tagCount++;
        }

        public void Append(UInt256 value)
        {
            Assembly.Append(value);
        }

        public void Append(EvmInstruction instruction)
        {
            Assembly.Append(instruction);
        }

        public void Append(EthSharpAssemblyItem item)
        {
            Assembly.Append(item);
        }

        //public void Append(byte[] value)
        //{
        //    Assembly.Append(value);
        //} 

    }
}
