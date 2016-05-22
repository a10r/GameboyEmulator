using GameboyEmulator.Core;
using GameboyEmulator.Core.Memory;
using NUnit.Framework;

namespace GameboyEmulator.Tests.Core
{
    public class PairedRegisterTest
    {
        private IRegister<byte> _low;
        private IRegister<byte> _high;
        private PairedRegister _sut;

        [SetUp]
        public void SetUp()
        {
            _low = new Register<byte>();
            _high = new Register<byte>();
            _sut = new PairedRegister(_high, _low);
        }

        [Test]
        public void WriteIsPropagated()
        {
            _sut.Value = 0xABCD;
            Assert.AreEqual(0xAB, _high.Value);
            Assert.AreEqual(0xCD, _low.Value);
        }

        [Test]
        public void ReadIsPropagated()
        {
            _low.Value = 0xCD;
            _high.Value = 0xAB;
            Assert.AreEqual(0xABCD, _sut.Value);
        }
    }
}
