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
            while (toHash.Count < 32)
                toHash.Add(new byte());
            return hasher.ComputeBytes(toHash.ToArray());
        }
    }
}
