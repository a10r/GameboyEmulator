using System;

namespace GameboyEmulator.Core.Memory
{
    // TODO implement 160 cycle delay!
    public class OamDmaController : IRegister<byte>
    {
        public byte Value
        {
            get => 0x42; // Doesn't matter.
            set => TransferData(value);
        }

        private readonly IMemoryBlock _systemMemory;

        public OamDmaController(IMemoryBlock systemMemory)
        {
            _systemMemory = systemMemory;
        }
        
        private void TransferData(byte sourceHigh)
        {
            var sourceBase = sourceHigh << 8;

            // Transfers 160 bytes
            for (int i = 0; i < 160; i++)
            {
                _systemMemory[0xFE00 + i] = _systemMemory[sourceBase + i];
            }
        }
    }
}
