using System.Collections.Generic;
using EthSharp.ContractDevelopment;

namespace EthSharp.Compiler
{
    public class EthSharpCompilerContext
    {
        private int _tagCount;
        private int _storageTracker;
        public EthSharpAssembly Assembly { get; set; }
        public Dictionary<string,int> StorageIdentifiers { get; set; }

        public EthSharpCompilerContext()
        {
            Assembly = new EthSharpAssembly();
            StorageIdentifiers = new Dictionary<string, int>();
            _tagCount = 1;
            _storageTracker = 0;
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

        public void AssignNewStorageLocation(string identifier)
        {
            StorageIdentifiers.Add(identifier, _storageTracker++);
        }

    }
}
