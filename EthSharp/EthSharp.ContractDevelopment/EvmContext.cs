using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthSharp.ContractDevelopment
{
    public static class EvmContext
    {
        public static Block CurrentBlock { get; set; }

        public static Message Message { get; set; }

        public static string Keccak256()
        {
            throw  new NotImplementedException();
        }
    }
}
