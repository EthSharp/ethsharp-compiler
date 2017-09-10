using System;
using System.Linq;
using System.Text;
using HashLib;

namespace EthSharp.Hashing
{
    public static class HashHelper
    {
        public static HashResult Keccak256(string input)
        {
            var hasher = HashFactory.Crypto.SHA3.CreateKeccak256();
            var toHash = Encoding.UTF8.GetBytes(input);
            return hasher.ComputeBytes(toHash);
        }

        public static HashResult Keccak256For32Bytes(string input)
        {
            var hasher = HashFactory.Crypto.SHA3.CreateKeccak256();
            var toHash = Encoding.UTF8.GetBytes(input);
            if (toHash.Length > 32)
                throw new Exception("The string wouldn't have fit into bytes32 in the first place");

            if (toHash.Length < 32)
            {
                var newHash = new byte[32];
                toHash.CopyTo(newHash, 0);
                toHash = newHash;
            }

            return hasher.ComputeBytes(toHash.ToArray());
        }
    }
}