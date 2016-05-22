using System.IO;

namespace GameboyEmulator.Core.Memory
{
    public class MemoryBlock : IMemoryBlock
    {
        private readonly byte[] _memory;

        public MemoryBlock(int size)
        {
            _memory = new byte[size];
            Size = size;
        }

        public MemoryBlock(int size, byte[] initial)
        {
            _memory = new byte[size];
            Size = size;
            initial.CopyTo(_memory, 0);
        }

        public MemoryBlock(byte[] initial) : this(initial.Length, initial)
        { }

        public int Size { get; private set; }

        public byte this[int address]
        {
            get
            {
                return _memory[address];
            }

            set
            {
                _memory[address] = value;
            }
        }

        public static IMemoryBlock LoadFromFile(string path)
        {
            return new MemoryBlock(File.ReadAllBytes(path));
        }
    }
}
