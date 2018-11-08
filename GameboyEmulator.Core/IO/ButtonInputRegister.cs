using GameboyEmulator.Core.Debugger;
using GameboyEmulator.Core.Memory;
using GameboyEmulator.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameboyEmulator.Core.IO
{
    // TODO button press interrupt
    // NOTE switching between key groups techically takes a few cycles
    // NOTE 0: keypress; 1: no keypress
    public class ButtonInputRegister : IRegister<byte>, IButtonState
    {
        // True -> DPad; False -> A/B/Start/Select
        private bool _enableDPad;

        private byte _dpad = 0b1111;
        private byte _other = 0b1111;

        public byte Value
        {
            get
            {
                if (_enableDPad)
                {
                    return _dpad;
                }
                else
                {
                    return _other;
                }
            }

            set
            {
                if (value.GetBit(5))
                {
                    _enableDPad = true;
                }
                else if (value.GetBit(4))
                {
                    _enableDPad = false;
                }
            }
        }

        public bool Right   { get => !_dpad.GetBit(0); set => _dpad.SetBitByRef(0, !value); }
        public bool Left    { get => !_dpad.GetBit(1); set => _dpad.SetBitByRef(1, !value); }
        public bool Up      { get => !_dpad.GetBit(2); set => _dpad.SetBitByRef(2, !value); }
        public bool Down    { get => !_dpad.GetBit(3); set => _dpad.SetBitByRef(3, !value); }
        
        public bool A       { get => !_other.GetBit(0); set => _other.SetBitByRef(0, !value); }
        public bool B       { get => !_other.GetBit(1); set => _other.SetBitByRef(1, !value); }
        public bool Select  { get => !_other.GetBit(2); set => _other.SetBitByRef(2, !value); }
        public bool Start   { get => !_other.GetBit(3); set => _other.SetBitByRef(3, !value); }

        public override string ToString() => $"Switch: {_enableDPad}; DPad: {_dpad.ToBinaryString()}; Other: {_other.ToBinaryString()}";
    }

    public interface IButtonState
    {
        bool Right { get; set; }
        bool Left { get; set; }
        bool Up { get; set; }
        bool Down { get; set; }

        bool A { get; set; }
        bool B { get; set; }
        bool Select { get; set; }
        bool Start { get; set; }
    }
}