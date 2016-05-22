using System;

namespace GameboyEmulator.Core.Memory
{
    public class FlagRegister : IFlags, IRegister<byte>
    {
        public bool Zero { get; set; }
        public bool Subtract { get; set; }
        public bool HalfCarry { get; set; }
        public bool Carry { get; set; }

        public byte Value
        {
            get
            {
                byte v = 0;
                if (Zero) v |= 1 << 7;
                if (Subtract) v |= 1 << 6;
                if (HalfCarry) v |= 1 << 5;
                if (Carry) v |= 1 << 4;
                return v;
            }

            set
            {
                throw new InvalidOperationException("Cannot set raw value of the flag register.");
            }
        }

        public FlagRegister() { }

        public FlagRegister(bool zero, bool subtract, bool halfCarry, bool carry)
        {
            Zero = zero;
            Subtract = subtract;
            HalfCarry = halfCarry;
            Carry = carry;
        }

        public override string ToString()
            => $"Z = {Zero}; N = {Subtract}; H = {HalfCarry}; C = {Carry}";
    }
}
