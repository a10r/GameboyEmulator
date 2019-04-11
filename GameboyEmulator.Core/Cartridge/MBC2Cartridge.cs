using System;
using GameboyEmulator.Core.Memory;

namespace GameboyEmulator.Core.Cartridge
{
    /// <summary>
    /// A cartridge with an MBC2 controller.
    /// 
    /// It has up to 256 KBytes of ROM and a built-in RAM of 512 x 4 bits.
    /// </summary>
    public class MBC2Cartridge : ICartridge
    {
        private readonly byte[] _romData;
        private int _selectedRomBank = 1;
        private readonly byte[] _ramData;
        private bool _ramWriteEnabled = false;

        public IMemoryBlock ROM { get; }
        public IMemoryBlock RAM { get; }
        
        public MBC2Cartridge(byte[] romData)
        {
            _romData = romData;
            // MBC2 has 512 x 4 bits of RAM on addresses 0xA000-0xA1FF.
            _ramData = new byte[512];
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
            if (address <= 0x0FFF)
            {
                _ramWriteEnabled = value == 0x0A;
            }
            else if (address >= 0x2100 && address < 0x2200)
            {
                var selection = value & 0b0000_1111;
                if (selection == 0)
                {
                    selection = 1;
                }
                _selectedRomBank = selection;
            }
        }

        private byte ReadRam(int address)
        {
            if (address >= 0x200)
            {
                throw new ArgumentException("Invalid read address for MBC2.");
            }

            return _ramData[address];
        }

        private void WriteRam(int address, byte value)
        {
            if (address >= 0x200)
            {
                throw new ArgumentException("Invalid write address for MBC2.");
            }

            // This MBC can only store the lower 4 bits.
            _ramData[address] = (byte)(value & 0b1111);
        }
    }
}
