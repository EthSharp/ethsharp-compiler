namespace EthSharp.Compiler
{
    public class EthSharpAssemblyItem
    {
        public enum JumpType
        {
            Ordinary,
            IntoFunction,
            OutOfFunction
        }

        public AssemblyItemType Type { get; private set; }

        //private SourceLocation _sourceLocation;
        public EvmInstruction Instruction { get; private set; }
        public int Data { get; set; }
        public JumpType ItemJumpType { get; set; } = JumpType.Ordinary;

        //NOTE: We are ignoring sourceLocation for now

        public EthSharpAssemblyItem(int push) : this(AssemblyItemType.Push, push) {}

        public EthSharpAssemblyItem(EvmInstruction i)
        {
            Type = AssemblyItemType.Operation;
            Instruction = i;
        }

        public EthSharpAssemblyItem(AssemblyItemType type, int data)
        {
            Type = type;
            if (type == AssemblyItemType.Operation)
                Instruction = (EvmInstruction) data; //this is silly - should never get here.
            else
                Data = data;
        }
    }

    public enum AssemblyItemType
    {
        UndefinedItem,
        Operation,
        Push,
        PushString,
        PushTag,
        PushSub,
        PushSubSize,
        PushProgramSize,
        Tag,
        PushData,
        PushLibraryAddress
    };
}
