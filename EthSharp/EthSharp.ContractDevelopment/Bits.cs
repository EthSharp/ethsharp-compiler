using System;
using System.Linq;
using EthSharp.ContractDevelopment;

namespace EthSharp.ContractDevelopment
{
    public static class Bits
    {
        private static readonly bool isLE = BitConverter.IsLittleEndian;

        public static byte[] GetBytes(Int16 value)
        {
            return Order(BitConverter.GetBytes(value));
        }

        public static byte[] GetBytes(UInt16 value)
        {
            return Order(BitConverter.GetBytes(value));
        }

        public static byte[] GetBytes(Int32 value)
        {
            return Order(BitConverter.GetBytes(value));
        }

        public static byte[] GetBytes(UInt32 value)
        {
            return Order(BitConverter.GetBytes(value));
        }

        public static byte[] GetBytes(Int64 value)
        {
            return Order(BitConverter.GetBytes(value));
        }

        public static byte[] GetBytes(UInt64 value)
        {
            return Order(BitConverter.GetBytes(value));
        }

        public static byte[] GetBytes(UInt256 value)
        {
            return value.ToByteArray();
        }

        public static string ToString(byte[] value, int startIndex = 0)
        {
            return BitConverter.ToString(Order(value), startIndex);
        }

        public static UInt16 ToUInt16(byte[] value, int startIndex = 0)
        {
            return BitConverter.ToUInt16(Order(value), startIndex);
        }

        public static Int32 ToInt32(byte[] value, int startIndex = 0)
        {
            return BitConverter.ToInt32(Order(value), startIndex);
        }

        public static UInt32 ToUInt32(byte[] value, int startIndex = 0)
        {
            return BitConverter.ToUInt32(Order(value), startIndex);
        }

        public static Int64 ToInt64(byte[] value, int startIndex = 0)
        {
            return BitConverter.ToInt64(Order(value), startIndex);
        }

        public static UInt64 ToUInt64(byte[] value, int startIndex = 0)
        {
            return BitConverter.ToUInt64(Order(value), startIndex);
        }

        public static long ToInt64BE(byte[] buffer, int offset = 0)
        {
            var value = 0L;

            value |= (long)buffer[offset++] << 56;
            value |= (long)buffer[offset++] << 48;
            value |= (long)buffer[offset++] << 40;
            value |= (long)buffer[offset++] << 32;
            value |= (long)buffer[offset++] << 24;
            value |= (long)buffer[offset++] << 16;
            value |= (long)buffer[offset++] << 8;
            value |= (long)buffer[offset++];

            return value;
        }

        public static ulong ToUInt64BE(byte[] buffer, int offset = 0)
        {
            return unchecked((ulong)ToInt64BE(buffer, offset));
        }

        public static UInt256 ToUInt256(byte[] value, int startIndex = 0)
        {
            return new UInt256(value, startIndex);
        }

        public static byte[] Order(byte[] value)
        {
            return isLE ? value : value.Reverse().ToArray();
        }

        public static void EncodeBool(bool value, byte[] buffer, int offset = 0)
        {
            buffer[offset++] = (byte)(value ? 1 : 0);
        }

        public static void EncodeInt16(short value, byte[] buffer, int offset = 0)
        {
            unchecked
            {
                buffer[offset++] = (byte)(value);
                buffer[offset++] = (byte)(value >> 8);
            }
        }

        public static void EncodeUInt16(ushort value, byte[] buffer, int offset = 0)
        {
            EncodeInt16(unchecked((short)value), buffer, offset);
        }

        public static void EncodeInt32(int value, byte[] buffer, int offset = 0)
        {
            unchecked
            {
                buffer[offset++] = (byte)(value);
                buffer[offset++] = (byte)(value >> 8);
                buffer[offset++] = (byte)(value >> 16);
                buffer[offset++] = (byte)(value >> 24);
            }
        }

        public static void EncodeUInt32(uint value, byte[] buffer, int offset = 0)
        {
            EncodeInt32(unchecked((int)value), buffer, offset);
        }

        public static void EncodeInt32BE(int value, byte[] buffer, int offset = 0)
        {
            unchecked
            {
                buffer[offset++] = (byte)(value >> 24);
                buffer[offset++] = (byte)(value >> 16);
                buffer[offset++] = (byte)(value >> 8);
                buffer[offset++] = (byte)(value);
            }
        }

        public static void EncodeUInt32BE(uint value, byte[] buffer, int offset = 0)
        {
            EncodeInt32BE(unchecked((int)value), buffer, offset);
        }

        public static void EncodeInt64(long value, byte[] buffer, int offset = 0)
        {
            unchecked
            {
                buffer[offset++] = (byte)(value);
                buffer[offset++] = (byte)(value >> 8);
                buffer[offset++] = (byte)(value >> 16);
                buffer[offset++] = (byte)(value >> 24);
                buffer[offset++] = (byte)(value >> 32);
                buffer[offset++] = (byte)(value >> 40);
                buffer[offset++] = (byte)(value >> 48);
                buffer[offset++] = (byte)(value >> 56);
            }
        }

        public static void EncodeUInt64(ulong value, byte[] buffer, int offset)
        {
            EncodeInt64(unchecked((long)value), buffer, offset);
        }

        public static void EncodeInt64BE(long value, byte[] buffer, int offset)
        {
            unchecked
            {
                buffer[offset++] = (byte)(value >> 56);
                buffer[offset++] = (byte)(value >> 48);
                buffer[offset++] = (byte)(value >> 40);
                buffer[offset++] = (byte)(value >> 32);
                buffer[offset++] = (byte)(value >> 24);
                buffer[offset++] = (byte)(value >> 16);
                buffer[offset++] = (byte)(value >> 8);
                buffer[offset++] = (byte)(value);
            }
        }

        public static void EncodeUInt64BE(ulong value, byte[] buffer, int offset)
        {
            EncodeInt64BE(unchecked((long)value), buffer, offset);
        }
    }
}
