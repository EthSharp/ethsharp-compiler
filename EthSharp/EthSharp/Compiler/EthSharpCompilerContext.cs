using System.Collections.Generic;
using EthSharp.ContractDevelopment;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EthSharp.Compiler
{
    public class EthSharpCompilerContext
    {
        private int _tagCount;
        private int _storageTracker;
        public EthSharpAssembly Assembly { get; set; }
        // variableName : storageLocation
        public Dictionary<string,int> StorageIdentifiers { get; set; }
        // methodName : method
        public Dictionary<string, EthSharpAssemblyItem> MethodBlockEntryPoints { get; set; }

        public EthSharpCompilerContext()
        {
            Assembly = new EthSharpAssembly();
            StorageIdentifiers = new Dictionary<string, int>();
            MethodBlockEntryPoints = new Dictionary<string, EthSharpAssemblyItem>();
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
