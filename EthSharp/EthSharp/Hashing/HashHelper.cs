using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HashLib;

namespace EthSharp.Hashing
{
    public static class HashHelper
    {
        public static HashResult Keccak256(string input)
        {
            var hasher = HashFactory.Crypto.SHA3.CreateKeccak256();
            var toHash = Encoding.UTF8.GetBytes(input).ToList();
            return hasher.ComputeBytes(toHash.ToArray());
        }

        public static HashResult Keccak256For32Bytes(string input)
        {
            var hasher = HashFactory.Crypto.SHA3.CreateKeccak256();
            var toHash = Encoding.UTF8.GetBytes(input).ToList();
            if (toHash.Count > 32)
                throw new Exception("The string wouldn't have fit into bytes32 in the first place");
            while (toHash.Count < 32)
                toHash.Add(new byte());
            return hasher.ComputeBytes(toHash.ToArray());
        }
    }
}
