namespace EthSharp.Compiler
{
    public class EthSharpCompilerContext
    {
        public EthSharpAssembly Assembly { get; set; }

        public EthSharpCompilerContext()
        {
            Assembly = new EthSharpAssembly();
        }

        public void Append(int value)
        {
            Assembly.Append(value);
        }

        public void Append(EvmInstruction instruction)
        {
            Assembly.Append(instruction);
        }

        //public void Append(byte[] value)
        //{
        //    Assembly.Append(value);
        //} 

    }
}
