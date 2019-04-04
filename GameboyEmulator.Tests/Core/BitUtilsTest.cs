using GameboyEmulator.Core.Utils;
using NUnit.Framework;

namespace GameboyEmulator.Tests.Core
{
    public class BitUtilsTest
    {
        [TestCase(0xAA, 0, false)]
        [TestCase(0xAA, 7, true)]
        public void GetBit(byte a, byte index, bool expected)
        {
            Assert.AreEqual(expected, a.GetBit(index));
        }

        [TestCase((ushort)0xAAAA, 0, false)]
        [TestCase((ushort)0xAAAA, 15, true)]
        public void GetBit(ushort a, byte index, bool expected)
        {
            Assert.AreEqual(expected, a.GetBit(index));
        }

        [TestCase((ushort)0xABCD, 0xCD)]
        [TestCase((ushort)0x0001, 0x01)]
        public void GetLow(ushort val, byte expected)
        {
            Assert.AreEqual(expected, val.GetLow());
        }

        [TestCase((ushort)0xABCD, 0xAB)]
        [TestCase((ushort)0x0001, 0x00)]
        public void GetHigh(ushort val, byte expected)
        {
            Assert.AreEqual(expected, val.GetHigh());
        }

        [TestCase(0xAB, 0xCD, (ushort)0xABCD)]
        [TestCase(0x00, 0x00, (ushort)0x0000)]
        public void Combine(byte high, byte low, ushort expected)
        {
            Assert.AreEqual(expected, BitUtils.Combine(low, high));
        }
    }
}
