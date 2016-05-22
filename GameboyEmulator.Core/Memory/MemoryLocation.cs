namespace GameboyEmulator.Core.Memory
{
    public class MemoryLocation : IRegister<byte>
    {
        private readonly IMemoryBlock _memory;
        private readonly int _address;

        public byte Value
        {
            get
            {
                return _memory[_address];
            }

            set
            {
                _memory[_address] = value;
            }
        }

        public MemoryLocation(IMemoryBlock memory, int address)
        {
            _memory = memory;
            _address = address;
        }
    }
}
