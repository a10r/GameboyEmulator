using GameboyEmulator.Core.Memory;

namespace GameboyEmulator.Core.Cartridge
{
    /// <summary>
    /// Cartridge with MBC5 controller. 
    /// 
    /// Supports CGB double-speed mode. Can use up to 64 MBits of ROM and 1 MBit of RAM.
    /// </summary>
    public class MBC5Cartridge : ICartridge
    {
        private readonly byte[] _romData;
        // 9 bit ROM bank number.
        private int _selectedRomBank = 1;

        private readonly byte[] _ramData;
        private int _selectedRamBank = 0;
        private bool _ramWriteEnabled = false;

        public IMemoryBlock ROM { get; }
        public IMemoryBlock RAM { get; }

        public MBC5Cartridge(byte[] romData)
        {
            _romData = romData;
            // Has maximum of 16 banks of 8KBytes each.
            _ramData = new byte[8192 * 16];
            ROM = new LambdaMemoryBlock(32768, ReadRom, WriteRom);
            RAM = new LambdaMemoryBlock(8192, ReadRam, WriteRam);
        }
        
        private byte ReadRom(int address)
        {
            // First bank is not switchable.
            if (address < 0x4000)
            {
                return _romData[address];
            }

            var effectiveAddress = address - 0x4000 + (_selectedRomBank * 0x4000);
            return _romData[effectiveAddress];
        }

        private void WriteRom(int address, byte value)
        {
            // Enable/disable RAM write access.
            if (address < 0x2000)
            {
                _ramWriteEnabled = value == 0x0A;
            }
            // TODO what happens if bank number 0 is selected?
            // Select lower 8 bits of ROM bank number.
            else if (address >= 0x2000 && address < 0x3000)
            {
                _selectedRomBank = (_selectedRomBank & 0b1_0000_0000) | value;
            }
            // Select 9th bit of ROM bank number.
            else if (address >= 0x3000 && address < 0x4000)
            {
                // TODO manual says 10th bit is not ignored -- why?
                // Might be a mistake since the MBC5+Rumble page marks it ignored.
                _selectedRomBank = (_selectedRomBank & 0b1111_1111) | ((value & 0b1) << 8);
            }
            // Select RAM bank.
            else if (address >= 0x4000 && address < 0x6000)
            {
                // TODO MBC5+Rumble uses bit 3 as motor control; only bit 0 and 1 are used 
                // for ram bank selection.
                _selectedRamBank = value & 0b1111;
            }
        }

        private int EffectiveRamAddress(int address)
        {
            return address + (_selectedRamBank * 0x2000);
        }

        private byte ReadRam(int address)
        {
            return _ramData[EffectiveRamAddress(address)];
        }

        private void WriteRam(int address, byte value)
        {
            if (_ramWriteEnabled)
            {
                _ramData[EffectiveRamAddress(address)] = value;
            }
        }
    }
}
