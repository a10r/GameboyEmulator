using GameboyEmulator.Core.Utils;

namespace GameboyEmulator.Core.Memory
{
    public class BoolPointer : IRegister<bool>
    {
        private readonly IRegister<byte> _targetRegister;
        private readonly int _bitIndex;

        public bool Value
        {
            get { return _targetRegister.GetBit(_bitIndex); }
            set { _targetRegister.SetBit(_bitIndex, value); }
        }

        public BoolPointer(IRegister<byte> targetRegister, int bitIndex)
        {
            _targetRegister = targetRegister;
            _bitIndex = bitIndex;
        }
    }
}
