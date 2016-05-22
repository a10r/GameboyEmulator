using System;

namespace GameboyEmulator.Core.Memory
{
    public class MemorySink : IMemoryBlock
    {
        public int Size => 0;

        public byte this[int address]
        {
            get
            {
                throw new InvalidOperationException("Illegal read.");
            }

            set
            {
                throw new InvalidOperationException("Illegal write.");
            }
        }
    }
}
