using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthSharp.ContractDevelopment
{
    public class Address
    {
        public UInt256 Balance { get; set; }

        public void Transfer(UInt256 toSend)
        {
            throw new NotImplementedException();
        }

        //Methods for send!
    }
}
