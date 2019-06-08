namespace GameboyEmulator.Core.Memory
{
    public class MemoryLocation : IRegister<byte>, IMemoryBlock
    {
        private readonly IMemoryBlock _baseMemoryBlock;
        private readonly int _baseAddress;

        public byte this[int address] {
            get => _baseMemoryBlock[_baseAddress + address];
            set => _baseMemoryBlock[_baseAddress + address] = value;
        }

        public int Size => _baseMemoryBlock.Size - _baseAddress;

        public byte Value
        {
            get => _baseMemoryBlock[_baseAddress];
            set => _baseMemoryBlock[_baseAddress] = value;
        }

        public MemoryLocation(IMemoryBlock baseMemoryBlock, int address)
        {
            _baseMemoryBlock = baseMemoryBlock;
            _baseAddress = address;
        }
    }
}
