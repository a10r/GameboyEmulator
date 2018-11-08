using GameboyEmulator.Core.Memory;
using GameboyEmulator.Core.Utils;

namespace GameboyEmulator.Core.Video
{
    public class LcdStatusRegister : IRegister<byte>
    {
        // TODO maybe just use a bool getter
        public IReadonlyRegister<bool> ScanlineCoincidenceInterruptEnabled { get; }
        public IReadonlyRegister<bool> OamSearchInterruptEnabled { get; }
        public IReadonlyRegister<bool> VBlankInterruptEnabled { get; }
        public IReadonlyRegister<bool> HBlankInterruptEnabled { get; }

        public LcdMode Mode
        {
            get
            {
                return (LcdMode) (Value & 0b11);
            }
            set
            {
                var raw = (byte) value;
                this.SetBit(0, raw.GetBit(0));
                this.SetBit(1, raw.GetBit(1));
            }
        }

        // TODO bit 2!

        public byte Value { get; set; }

        public LcdStatusRegister()
        {
            ScanlineCoincidenceInterruptEnabled = new BoolPointer(this, 6);
            OamSearchInterruptEnabled = new BoolPointer(this, 5);
            VBlankInterruptEnabled = new BoolPointer(this, 4); // TODO why tho?
            HBlankInterruptEnabled = new BoolPointer(this, 3);
        }
    }
}