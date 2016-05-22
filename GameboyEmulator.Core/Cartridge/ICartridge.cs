using GameboyEmulator.Core.Memory;

namespace GameboyEmulator.Core.Cartridge
{
    public interface ICartridge
    {
        // TODO: MBC type, ROM name?, battery
        int RomBankCount { get; }
        int RamBankCount { get; }

        IMemoryBlock Ram { get; }
        IMemoryBlock Rom { get; }
    }
}
