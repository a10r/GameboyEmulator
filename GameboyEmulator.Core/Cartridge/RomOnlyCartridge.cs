using GameboyEmulator.Core.Memory;
using System.Diagnostics;

namespace GameboyEmulator.Core.Cartridge
{
    /// <summary>
    /// This cartridge has no memory banking.
    /// </summary>
    internal class RomOnlyCartridge : ICartridge
    {
        public IMemoryBlock ROM { get; }

        public IMemoryBlock RAM { get; }

        public RomOnlyCartridge(byte[] romData)
        {
            Debug.Assert(romData.Length == 32768);
            ROM = new MemoryBlock(romData);
            // TODO apparently some small carts DO have a (non-switchable) RAM
            RAM = new MemorySink();
        }
    }
}
