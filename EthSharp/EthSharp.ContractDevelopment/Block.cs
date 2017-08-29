using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthSharp.ContractDevelopment
{
    public class Block
    {
        public Address Coinbase { get; set; }
        public UInt256 Number { get; set; }
        public string Hash { get; set; }
        public DateTime TimeStamp { get; set; }
        public UInt256 Difficulty { get; set; }
        public UInt256 GasLimit { get; set; }
    }
}
