using GameboyEmulator.Core.Memory;
using GameboyEmulator.Core.Utils;

namespace GameboyEmulator.Core.Video
{
    public class LcdStatusRegister : IRegister<byte>
    {
        public LcdMode Mode
        {
            get
            {
                return (LcdMode) ((Value.GetBit(1) ? 2 : 0) + (Value.GetBit(0) ? 1 : 0));
            }
            set
            {
                var raw = (byte) value;
                this.SetBit(0, raw.GetBit(0));
                this.SetBit(1, raw.GetBit(1));
            }
        }

        // TODO: match + interrupt

        public byte Value { get; set; }
    }
}