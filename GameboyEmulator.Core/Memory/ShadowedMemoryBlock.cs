namespace GameboyEmulator.Core.Memory
{
    public class ShadowedMemoryBlock : IMemoryBlock
    {
        private readonly IMemoryBlock _top;
        private readonly IMemoryBlock _bottom;
        private readonly IRegister<bool> _disableTop;

        public byte this[int address]
        {
            get
            {
                if (address >= _top.Size || _disableTop.Value == true)
                {
                    return _bottom[address];
                }
                else
                {
                    return _top[address];
                }
            }

            set
            {
                if (address >= _top.Size || _disableTop.Value == true)
                {
                    _bottom[address] = value;
                }
                else
                {
                    _top[address] = value;
                }
            }
        }

        public int Size => _bottom.Size;

        public ShadowedMemoryBlock(IMemoryBlock top, IMemoryBlock bottom, IRegister<bool> disableTop)
        {
            _top = top;
            _bottom = bottom;
            _disableTop = disableTop;
        }


    }
}
