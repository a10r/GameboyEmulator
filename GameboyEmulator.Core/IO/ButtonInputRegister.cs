using GameboyEmulator.Core.Debugger;
using GameboyEmulator.Core.Memory;
using GameboyEmulator.Core.Processor;
using GameboyEmulator.Core.Utils;

namespace GameboyEmulator.Core.IO
{
    // NOTE switching between key groups techically takes a few cycles
    // NOTE 0: keypress; 1: no keypress (opposite of IButtonState interface)
    public class ButtonInputRegister : IRegister<byte>, IButtonState
    {
        private readonly IInterruptTrigger _buttonPressInterrupt;

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

        public bool Right   { get => !_dpad.GetBit(0); set { _dpad.SetBitByRef(0, !value); ButtonStateChanged(); } }
        public bool Left    { get => !_dpad.GetBit(1); set { _dpad.SetBitByRef(1, !value); ButtonStateChanged(); } }
        public bool Up      { get => !_dpad.GetBit(2); set { _dpad.SetBitByRef(2, !value); ButtonStateChanged(); } }
        public bool Down    { get => !_dpad.GetBit(3); set { _dpad.SetBitByRef(3, !value); ButtonStateChanged(); } }
        
        public bool A       { get => !_other.GetBit(0); set { _other.SetBitByRef(0, !value); ButtonStateChanged(); } }
        public bool B       { get => !_other.GetBit(1); set { _other.SetBitByRef(1, !value); ButtonStateChanged(); } }
        public bool Select  { get => !_other.GetBit(2); set { _other.SetBitByRef(2, !value); ButtonStateChanged(); } }
        public bool Start   { get => !_other.GetBit(3); set { _other.SetBitByRef(3, !value); ButtonStateChanged(); } }

        private void ButtonStateChanged()
        {
            _buttonPressInterrupt.Trigger();
        }

        public ButtonInputRegister(IInterruptTrigger buttonPressInterrupt)
        {
            _buttonPressInterrupt = buttonPressInterrupt;
        }

        public override string ToString() => $"Switch: {_enableDPad}; DPad: {_dpad.ToBinaryString()}; Other: {_other.ToBinaryString()}";
    }

    /// <summary>
    /// Represents the "pressed" state of buttons. True means that the 
    /// button is pressed, false means released.
    /// </summary>
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