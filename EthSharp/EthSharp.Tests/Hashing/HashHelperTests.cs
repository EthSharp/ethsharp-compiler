using System;
using System.Linq;
using EthSharp.Hashing;
using Xunit;

namespace EthSharp.Tests.Hashing
{
    public class HashHelperTests
    {
        public static string ToString(byte[] data)
        {
            return string.Concat(data.Select(item => item.ToString("X2").ToLowerInvariant()));
        }


        // reference url for the expected values
        // https://ethereum.stackexchange.com/questions/550/which-cryptographic-hash-function-does-ethereum-use
        // http://emn178.github.io/online-tools/keccak_256.html
        [Theory]
        [InlineData("testing", "5f16f4c7f149ac4f9510d9cf8cf384038ad348b3bcdc01915f95de12df9d1b02")]
        [InlineData("other random string", "779a6e16b0707e30912ce551355d1b3d4b67a83bfb2a1a72e09496a4b6f0d624")]
        public void can_hash_using_Keccak256(string data, string hash)
        {
            var calculatedHash = HashHelper.Keccak256(data);

            var calculatedHashAsString = ToString(calculatedHash.GetBytes());

            Assert.Equal(hash, calculatedHashAsString);
        }

        [Fact]        
        public void can_hash_using_Keccak256For32Bytes()
        {
            var calculatedHash = HashHelper.Keccak256For32Bytes("testing");

            var calculatedHashAsString = ToString(calculatedHash.GetBytes());

            Assert.Equal("24498869e38e7823d6eb6ac2e75540490879e7c340216076aa4b3e5166b8d240", calculatedHashAsString);

            Assert.Throws<Exception>(() => HashHelper.Keccak256For32Bytes("this is a longer than 32 bytes string"));
        }
    }
}