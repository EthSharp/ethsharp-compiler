using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthSharp.Compiler
{
    public class EvmByteCode
    {
        public List<byte> ByteCode { get; set; }

        public EvmByteCode()
        {
            ByteCode = new List<byte>();
        }
    }
}
