using System;

namespace GameboyEmulator.Core.Memory
{
    public class LambdaMemoryBlock : IMemoryBlock
    {
        private readonly Func<int, byte> _read;
        private readonly Action<int, byte> _write;

        public byte this[int address] {
            get => _read(address);
            set => _write(address, value);
        }

        public int Size { get; }
        
        public LambdaMemoryBlock(int size, Func<int, byte> read, Action<int, byte> write)
        {
            Size = size;
            _write = write;
            _read = read;
        }
    }
}
