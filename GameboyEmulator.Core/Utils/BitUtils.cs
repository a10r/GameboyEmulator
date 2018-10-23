using System;
using GameboyEmulator.Core.Memory;

namespace GameboyEmulator.Core.Utils
{
    public static class BitUtils
    {
        /// <summary>
        /// Returns a certain bit in a byte.
        /// </summary>
        /// <param name="index">Index of the bit to return. 0 = LSB; 7 = MSB.</param>
        public static bool GetBit(this byte b, int index)
        {
            if (index > 7 || index < 0)
                throw new ArgumentException("Invalid bit index.");
            return (b >> index & 0x1) == 1;
        }

        /// <summary>
        /// Returns a certain bit in a short.
        /// </summary>
        /// <param name="index">Index of the bit to return. 0 = LSB; 15 = MSB.</param>
        public static bool GetBit(this ushort s, int index)
        {
            if (index > 15 || index < 0)
                throw new ArgumentException("Invalid bit index.");
            return (s >> index & 0x1) == 1;
        }

        // Aliases
        public static bool GetBit(this IReadonlyRegister<byte> b, int index) => b.Value.GetBit(index);
        public static bool GetBit(this IReadonlyRegister<ushort> s, int index) => s.Value.GetBit(index);

        public static void SetBit(this IRegister<byte> b, int index, bool value)
        {
            if (value)
            {
                b.Value |= (byte)(1 << index);
            }
            else
            {
                b.Value &= (byte)(~(1 << index));
            }
        }

        public static void SetBit(ref this byte b, int index, bool value)
        {
            if (value)
            {
                b |= (byte)(1 << index);
            }
            else
            {
                b &= (byte)(~(1 << index));
            }
        }

        public static byte GetLow(this ushort s) => (byte)s;
        public static byte GetHigh(this ushort s) => (byte)(s >> 8);

        public static ushort Combine(byte low, byte high) => (ushort)(high << 8 | low);
    }
}
