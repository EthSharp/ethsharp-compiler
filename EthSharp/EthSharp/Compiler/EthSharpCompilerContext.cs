using System.Collections.Generic;
using System.Linq;
using EthSharp.ContractDevelopment;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EthSharp.Compiler
{
    public class EthSharpCompilerContext
    {
        private int _tagCount;
        private int _storageTracker;
        public EthSharpAssembly Assembly { get; private set; }
        // variableName : storageLocation
        public Dictionary<string,int> StorageIdentifiers { get; private set; }
        // methodName : method
        public Dictionary<string, EthSharpAssemblyItem> MethodBlockEntryPoints { get; set; }

        public Queue<MethodDeclarationSyntax> MethodQueue { get; set; }


        public SyntaxTree Root { get; private set; }

        public ClassDeclarationSyntax RootClass => Root.GetRoot().ChildNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();

        public EthSharpCompilerContext(SyntaxTree root)
        {
            Assembly = new EthSharpAssembly();
            StorageIdentifiers = new Dictionary<string, int>();
            MethodBlockEntryPoints = new Dictionary<string, EthSharpAssemblyItem>();
            MethodQueue = new Queue<MethodDeclarationSyntax>();
            _tagCount = 1;
            _storageTracker = 0;
            Root = root;
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

        /// <summary>
        /// When we need to jump to a function we use this. If the method hasn't been built yet, it will be after this :)
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public UInt256 GetMethodTag(MethodDeclarationSyntax method)
        {
            string methodName = method.Identifier.Text;
            if (MethodBlockEntryPoints.ContainsKey(methodName))
                return MethodBlockEntryPoints[methodName].Data;

            UInt256 newTag = GetNewTag();
            MethodBlockEntryPoints.Add(methodName, new EthSharpAssemblyItem(AssemblyItemType.Tag, GetNewTag()));
            MethodQueue.Enqueue(method);
            return newTag;
        }

    }
}
