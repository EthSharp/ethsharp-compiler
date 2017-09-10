/*
 * From: https://github.com/pmlyon/BitSharp/tree/master/BitSharp.Common
 * 
 * Unlicensed
 */

using System;
using System.Data.HashFunction;
using System.Globalization;
using System.Numerics;

namespace EthSharp.ContractDevelopment
{
    public class UInt256 : IComparable<UInt256>
    {
        public static UInt256 Zero { get; } = new UInt256(new byte[0]);

        // parts are big-endian
        private const int width = 4;
        private readonly ulong[] parts;
        private readonly int hashCode;


        // Really not optimised - we need to improve when this is used in future. 
        // At the moment is going to check this and then re call ToByteArray
        public int ByteLength
        {
            get
            {
                var bytes = this.ToByteArrayBE();
                int offset = 0;
                while (bytes[offset] == 0)
                {
                    if (offset == 31)
                        break;
                    offset++;
                }
                return 32 - offset;
            }
        }

        public UInt256(byte[] value)
        {
            // length must be <= 32, or 33 with the last byte set to 0 to indicate the number is positive
            if (value.Length > 32 && !(value.Length == 33 && value[32] == 0))
                throw new ArgumentOutOfRangeException(nameof(value));

            if (value.Length < 32)
                Array.Resize(ref value, 32);

            InnerInit(value, 0, out parts, out hashCode);
        }

        public UInt256(byte[] value, int offset)
        {
            if (value.Length < offset + 32)
                throw new ArgumentOutOfRangeException(nameof(offset));

            InnerInit(value, offset, out parts, out hashCode);
        }

        private UInt256(ulong[] parts)
        {
            this.parts = parts;

            InnerInit(out hashCode);
        }

        private void InnerInit(byte[] buffer, int offset, out ulong[] parts, out int hashCode)
        {
            // convert parts and store
            parts = new ulong[width];
            offset += 32;
            for (var i = 0; i < width; i++)
            {
                offset -= 8;
                parts[i] = Bits.ToUInt64(buffer, offset);
            }



            InnerInit(out hashCode);
        }

        private void InnerInit(out int hashCode)
        {
            var hashBytes = ToByteArray();
            hashCode = Bits.ToInt32(new xxHash(32).ComputeHash(hashBytes));
        }

        public UInt256(int value)
            : this(Bits.GetBytes(value))
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException();
        }

        public UInt256(long value)
            : this(Bits.GetBytes(value))
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException();
        }

        public UInt256(uint value)
            : this(Bits.GetBytes(value))
        { }

        public UInt256(ulong value)
            : this(Bits.GetBytes(value))
        { }

        public UInt256(BigInteger value)
            : this(value.ToByteArray())
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException();
        }

        public ulong[] Parts => new ulong[] { parts[3], parts[2], parts[1], parts[0] };
        public ulong Part1 => parts[0];
        public ulong Part2 => parts[1];
        public ulong Part3 => parts[2];
        public ulong Part4 => parts[3];

        public byte[] ToByteArray()
        {
            var buffer = new byte[32];
            ToByteArray(buffer);
            return buffer;
        }

        public void ToByteArray(byte[] buffer, int offset = 0)
        {
            for (var i = width - 1; i >= 0; i--)
            {
                Bits.EncodeUInt64(parts[i], buffer, offset);
                offset += 8;
            }
        }

        public byte[] ToByteArrayBE()
        {
            var buffer = new byte[32];
            ToByteArrayBE(buffer);
            return buffer;
        }

        public void ToByteArrayBE(byte[] buffer, int offset = 0)
        {
            for (var i = 0; i < width; i++)
            {
                Bits.EncodeUInt64BE(parts[i], buffer, offset);
                offset += 8;
            }
        }

        public static UInt256 FromByteArrayBE(byte[] buffer, int offset = 0)
        {
            unchecked
            {
                if (buffer.Length < offset + 32)
                    throw new ArgumentException();

                var parts = new ulong[width];
                for (var i = 0; i < width; i++)
                {
                    parts[i] = Bits.ToUInt64BE(buffer, offset);
                    offset += 8;
                }

                return new UInt256(parts);
            }
        }

        public BigInteger ToBigInteger()
        {
            // add a trailing zero so that value is always positive
            var buffer = new byte[33];
            ToByteArray(buffer);
            return new BigInteger(buffer);
        }

        public int ToInt()
        {
            return (int) ToBigInteger();
        }

        public int CompareTo(UInt256 other)
        {
            for (var i = 0; i < width; i++)
            {
                if (parts[i] < other.parts[i])
                    return -1;
                else if (parts[i] > other.parts[i])
                    return +1;
            }

            return 0;
        }

        // TODO doesn't compare against other numerics
        public override bool Equals(object obj)
        {
            if (!(obj is UInt256))
                return false;

            var other = (UInt256)obj;
            return this == other;
        }

        public override int GetHashCode() => hashCode;

        public override string ToString()
        {
            return Bits.ToString(this.ToByteArrayBE()).Replace("-", "").ToLower();
        }

        public static explicit operator BigInteger(UInt256 value)
        {
            return value.ToBigInteger();
        }

        public static explicit operator UInt256(byte value)
        {
            return new UInt256(value);
        }

        public static implicit operator UInt256(int value)
        {
            return new UInt256(value);
        }

        public static explicit operator UInt256(long value)
        {
            return new UInt256(value);
        }

        public static explicit operator UInt256(sbyte value)
        {
            return new UInt256(value);
        }

        public static explicit operator UInt256(short value)
        {
            return new UInt256(value);
        }

        public static explicit operator UInt256(uint value)
        {
            return new UInt256(value);
        }

        public static explicit operator UInt256(ulong value)
        {
            return new UInt256(value);
        }

        public static explicit operator UInt256(ushort value)
        {
            return new UInt256(value);
        }

        public static bool operator ==(UInt256 left, UInt256 right)
        {
            if (ReferenceEquals(left, right))
                return true;
            else if (ReferenceEquals(left, null) != ReferenceEquals(right, null))
                return false;

            return left.CompareTo(right) == 0;
        }

        public static bool operator !=(UInt256 left, UInt256 right)
        {
            return !(left == right);
        }

        public static bool operator <(UInt256 left, UInt256 right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(UInt256 left, UInt256 right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(UInt256 left, UInt256 right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(UInt256 left, UInt256 right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static UInt256 Parse(string value)
        {
            return new UInt256(BigInteger.Parse("0" + value).ToByteArray());
        }

        public static UInt256 Parse(string value, IFormatProvider provider)
        {
            return new UInt256(BigInteger.Parse("0" + value, provider).ToByteArray());
        }

        public static UInt256 Parse(string value, NumberStyles style)
        {
            return new UInt256(BigInteger.Parse("0" + value, style).ToByteArray());
        }

        public static UInt256 Parse(string value, NumberStyles style, IFormatProvider provider)
        {
            return new UInt256(BigInteger.Parse("0" + value, style, provider).ToByteArray());
        }

        public static UInt256 ParseHex(string value)
        {
            return new UInt256(BigInteger.Parse("0" + value, NumberStyles.HexNumber).ToByteArray());
        }

        public static UInt256 operator +(UInt256 left, UInt256 right)
        {
            return new UInt256(left.ToBigInteger() + right.ToBigInteger());
        }

        public static UInt256 operator -(UInt256 left, UInt256 right)
        {
            return new UInt256(left.ToBigInteger() - right.ToBigInteger());
        }

        public static UInt256 operator *(UInt256 left, uint right)
        {
            return new UInt256(left.ToBigInteger() * right);
        }

        public static UInt256 operator *(UInt256 left, UInt256 right)
        {
            return new UInt256(left.ToBigInteger() * right.ToBigInteger());
        }

        public static UInt256 operator /(UInt256 dividend, uint divisor)
        {
            return new UInt256(dividend.ToBigInteger() / divisor);
        }

        public static UInt256 operator /(UInt256 dividend, UInt256 divisor)
        {
            return new UInt256(dividend.ToBigInteger() / divisor.ToBigInteger());
        }

        public static UInt256 operator <<(UInt256 value, int shift)
        {
            return new UInt256(value.ToBigInteger() << shift);
        }

        public static UInt256 operator >>(UInt256 value, int shift)
        {
            return new UInt256(value.ToBigInteger() >> shift);
        }

        public static UInt256 operator ~(UInt256 value)
        {
            var parts = new ulong[width];
            for (var i = 0; i < width; i++)
                parts[i] = ~value.parts[i];

            return new UInt256(parts);
        }
    }
}
