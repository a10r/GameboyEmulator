using GameboyEmulator.Core.Memory;

namespace GameboyEmulator.Core.Cartridge
{
    public interface ICartridge
    {
        IMemoryBlock ROM { get; }
        IMemoryBlock RAM { get; }
    }
}
