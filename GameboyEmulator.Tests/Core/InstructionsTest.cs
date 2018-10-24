using GameboyEmulator.Core.Memory;
using GameboyEmulator.Core.Processor;
using NUnit.Framework;

namespace GameboyEmulator.Tests.Core
{
    public class InstructionsTest
    {
        [TestCase(0x00, 0x00, 0x00, false, false)]
        [TestCase(0x80, 0x80, 0x00, false, true)]
        [TestCase(0x3A, 0xC6, 0x00, true, true)]
        public void Add(byte a, byte b, byte expected,
            bool expectedH, bool expectedC)
        {
            var expectedFlags = new FlagRegister(expected == 0, false, expectedH, expectedC);
            var reg = new Register<byte> { Value = a };
            var flags = new FlagRegister();
            Instructions.Add(reg, b, flags);
            Assert.AreEqual(expected, reg.Value);
            AssertFlags(expectedFlags, flags);
        }

        [TestCase(0x3E, 0x3E, 0x00, false, false)]
        [TestCase(0x3E, 0x0F, 0x2F, true, false)]
        [TestCase(0x3E, 0x40, 0xFE, false, true)]
        [TestCase(0x00, 0x80, 0x80, false, true)]
        public void Sub(byte a, byte b, byte expected,
            bool expectedH, bool expectedC)
        {
            var expectedFlags = new FlagRegister(expected == 0, true, expectedH, expectedC);
            var reg = new Register<byte> { Value = a };
            var flags = new FlagRegister();
            Instructions.Subtract(reg, b, flags);
            Assert.AreEqual(expected, reg.Value);
            AssertFlags(expectedFlags, flags);
        }

        [TestCase(0xFF, 0xFF, 0xFF)]
        [TestCase(0xF0, 0x0F, 0x00)]
        public void And(byte a, byte b, byte expected)
        {
            var expectedFlags = new FlagRegister(expected == 0, false, true, false);
            var reg = new Register<byte> { Value = a };
            var flags = new FlagRegister();
            Instructions.And(reg, b, flags);
            Assert.AreEqual(expected, reg.Value);
            AssertFlags(expectedFlags, flags);
        }

        [TestCase(0xFF, 0xFF, 0xFF)]
        [TestCase(0xF0, 0x0F, 0xFF)]
        [TestCase(0x00, 0x00, 0x00)]
        public void Or(byte a, byte b, byte expected)
        {
            var expectedFlags = new FlagRegister(expected == 0, false, false, false);
            var reg = new Register<byte> { Value = a };
            var flags = new FlagRegister();
            Instructions.Or(reg, b, flags);
            Assert.AreEqual(expected, reg.Value);
            AssertFlags(expectedFlags, flags);
        }

        [TestCase(0xFF, 0xFF, 0x00)]
        [TestCase(0xF0, 0x0F, 0xFF)]
        [TestCase(0x00, 0x00, 0x00)]
        public void Xor(byte a, byte b, byte expected)
        {
            var expectedFlags = new FlagRegister(expected == 0, false, false, false);
            var reg = new Register<byte> { Value = a };
            var flags = new FlagRegister();
            Instructions.Xor(reg, b, flags);
            Assert.AreEqual(expected, reg.Value);
            AssertFlags(expectedFlags, flags);
        }

        [TestCase(0x0A, 0xA0)]
        [TestCase(0xA0, 0x0A)]
        [TestCase(0x00, 0x00)]
        public void SwapNibbles(byte a, byte expected)
        {
            var expectedFlags = new FlagRegister(expected == 0, false, false, false);
            var reg = new Register<byte> { Value = a };
            var flags = new FlagRegister();
            Instructions.SwapNibbles(reg, flags);
            Assert.AreEqual(expected, reg.Value);
            AssertFlags(expectedFlags, flags);
        }

        [TestCase(0xAA, 0x55)]
        [TestCase(0xFF, 0x00)]
        public void Complement(byte a, byte expected)
        {
            var expectedFlags = new FlagRegister(false, true, true, false);
            var reg = new Register<byte> { Value = a };
            var flags = new FlagRegister();
            Instructions.Complement(reg, flags);
            Assert.AreEqual(expected, reg.Value);
            AssertFlags(expectedFlags, flags);
        }

        [TestCase(0x00, 0x00, false)]
        [TestCase(0x01, 0x02, false)]
        [TestCase(0xFF, 0xFF, true)]
        [TestCase(0x80, 0x01, true)]
        public void RotateLeft(byte a, byte expected, bool expectedC)
        {
            var expectedFlags = new FlagRegister(expected == 0, false, false, expectedC);
            var reg = new Register<byte> { Value = a };
            var flags = new FlagRegister();
            Instructions.RotateLeft(reg, flags);
            Assert.AreEqual(expected, reg.Value);
            AssertFlags(expectedFlags, flags);
        }

        [TestCase(0x00, false, 0x00, false)]
        [TestCase(0x00, true, 0x01, false)]
        [TestCase(0x01, false, 0x02, false)]
        [TestCase(0x01, true, 0x03, false)]
        [TestCase(0xFF, false, 0xFE, true)]
        [TestCase(0x80, false, 0x00, true)]
        public void RotateLeftThroughCarry(byte a, bool initialC,
            byte expected, bool expectedC)
        {
            var expectedFlags = new FlagRegister(expected == 0, false, false, expectedC);
            var reg = new Register<byte> { Value = a };
            var flags = new FlagRegister { Carry = initialC };
            Instructions.RotateLeftThroughCarry(reg, flags);
            Assert.AreEqual(expected, reg.Value);
            AssertFlags(expectedFlags, flags);
        }

        [TestCase(0x00, 0x00, false)]
        [TestCase(0x01, 0x80, true)]
        [TestCase(0xFF, 0xFF, true)]
        [TestCase(0x80, 0x40, false)]
        public void RotateRight(byte a, byte expected, bool expectedC)
        {
            var expectedFlags = new FlagRegister(expected == 0, false, false, expectedC);
            var reg = new Register<byte> { Value = a };
            var flags = new FlagRegister();
            Instructions.RotateRight(reg, flags);
            Assert.AreEqual(expected, reg.Value);
            AssertFlags(expectedFlags, flags);
        }

        [TestCase(0x00, false, 0x00, false)]
        [TestCase(0x00, true, 0x80, false)]
        [TestCase(0x01, false, 0x00, true)]
        [TestCase(0x01, true, 0x80, true)]
        [TestCase(0xFF, false, 0x7F, true)]
        [TestCase(0xFF, true, 0xFF, true)]
        public void RotateRightThroughCarry(byte a, bool initialC,
            byte expected, bool expectedC)
        {
            var expectedFlags = new FlagRegister(expected == 0, false, false, expectedC);
            var reg = new Register<byte> { Value = a };
            var flags = new FlagRegister { Carry = initialC };
            Instructions.RotateRightThroughCarry(reg, flags);
            Assert.AreEqual(expected, reg.Value);
            AssertFlags(expectedFlags, flags);
        }

        [TestCase(0x00, 0x00, false)]
        [TestCase(0x01, 0x02, false)]
        [TestCase(0xFF, 0xFE, true)]
        [TestCase(0x80, 0x00, true)]
        public void ShiftLeft(byte a, byte expected, bool expectedC)
        {
            var expectedFlags = new FlagRegister(expected == 0, false, false, expectedC);
            var reg = new Register<byte> { Value = a };
            var flags = new FlagRegister();
            Instructions.ShiftLeft(reg, flags);
            Assert.AreEqual(expected, reg.Value);
            AssertFlags(expectedFlags, flags);
        }

        [TestCase(0x00, 0x00, false)]
        [TestCase(0x01, 0x00, true)]
        [TestCase(0xFF, 0xFF, true)]
        [TestCase(0xFE, 0xFF, false)]
        public void ShiftRightArithmetic(byte a, byte expected, bool expectedC)
        {
            var expectedFlags = new FlagRegister(expected == 0, false, false, expectedC);
            var reg = new Register<byte> { Value = a };
            var flags = new FlagRegister();
            Instructions.ShiftRightArithmetic(reg, flags);
            Assert.AreEqual(expected, reg.Value);
            AssertFlags(expectedFlags, flags);
        }

        [TestCase(0x00, 0x00, false)]
        [TestCase(0x01, 0x00, true)]
        [TestCase(0xFF, 0x7F, true)]
        [TestCase(0xFE, 0x7F, false)]
        public void ShiftRightLogical(byte a, byte expected, bool expectedC)
        {
            var expectedFlags = new FlagRegister(expected == 0, false, false, expectedC);
            var reg = new Register<byte> { Value = a };
            var flags = new FlagRegister();
            Instructions.ShiftRightLogical(reg, flags);
            Assert.AreEqual(expected, reg.Value);
            AssertFlags(expectedFlags, flags);
        }

        [TestCase(0xAA, 7, false)]
        [TestCase(0xAA, 6, true)]
        public void TestBit(byte a, byte index, bool expectedZ)
        {
            var expectedFlags = new FlagRegister(expectedZ, false, true, false);
            var reg = new Register<byte> { Value = a };
            var flags = new FlagRegister();
            Instructions.TestBit(reg, index, flags);
            AssertFlags(expectedFlags, flags);
        }

        [TestCase(0xAA, 7, false, 0x2A)]
        [TestCase(0xAA, 7, true, 0xAA)]
        [TestCase(0xAA, 6, false, 0xAA)]
        [TestCase(0xAA, 6, true, 0xEA)]
        public void SetBit(byte a, byte index, bool value, byte expected)
        {
            var reg = new Register<byte> { Value = a };
            Instructions.SetBit(reg, index, value);
            Assert.AreEqual(expected, reg.Value);
        }

        private void AssertFlags(IFlags expected, IFlags actual)
        {
            Assert.AreEqual(expected.Zero, actual.Zero);
            Assert.AreEqual(expected.Subtract, actual.Subtract);
            Assert.AreEqual(expected.HalfCarry, actual.HalfCarry);
            Assert.AreEqual(expected.Carry, actual.Carry);
        }
    }
}
