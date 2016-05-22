using GameboyEmulator.Core.Utils;

namespace GameboyEmulator.Core.Memory
{
    public class PairedRegister : IRegister<ushort>
    {
        private readonly IRegister<byte> _high;
        private readonly IRegister<byte> _low;

        public ushort Value
        {
            get
            {
                return BitUtils.Combine(_low.Value, _high.Value);
            }

            set
            {
                _high.Value = value.GetHigh();
                _low.Value = value.GetLow();
            }
        }

        public PairedRegister(IRegister<byte> high, IRegister<byte> low)
        {
            _high = high;
            _low = low;
        }

        public override string ToString()
            => $"PReg16({Value}; 0x{Value:X4})";
    }
}