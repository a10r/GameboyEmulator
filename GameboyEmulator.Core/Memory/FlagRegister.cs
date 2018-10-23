using GameboyEmulator.Core.Utils;
using System;

namespace GameboyEmulator.Core.Memory
{
    public class FlagRegister : IFlags, IRegister<byte>
    {
        public bool Zero { get; set; } // Z
        public bool Subtract { get; set; } // N
        public bool HalfCarry { get; set; } // H
        public bool Carry { get; set; } // CY

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
                // It's possible to do raw writes to the flag register, e.g. through instruction 0xF1.
                Zero = value.GetBit(7);
                Subtract = value.GetBit(6);
                HalfCarry = value.GetBit(5);
                Carry = value.GetBit(4);
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
