namespace GameboyEmulator.Core.Memory
{
    public class ShadowedMemoryBlock : IMemoryBlock
    {
        private readonly IMemoryBlock _top;
        private readonly IMemoryBlock _bottom;

        public byte this[int address]
        {
            get
            {
                if (address < _top.Size)
                {
                    return _top[address];
                }
                return _bottom[address];
            }

            set
            {
                if (address < _top.Size)
                {
                    _top[address] = value;
                }
                else
                {
                    _bottom[address] = value;
                }
            }
        }

        public int Size => _bottom.Size;

        public ShadowedMemoryBlock(IMemoryBlock top, IMemoryBlock bottom)
        {
            _top = top;
            _bottom = bottom;
        }


    }
}
