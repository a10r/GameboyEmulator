using System;
using GameboyEmulator.Core.Memory;
using GameboyEmulator.Core.Utils;

namespace GameboyEmulator.Core.Cartridge
{
    /// <summary>
    /// Cartridge with a MBC1 bank switching controller.
    /// </summary>
    public class MBC1Cartridge : ICartridge
    {
        private enum MBC1Mode
        {
            ROM16Mbit_RAM8KByte,
            ROM4Mbit_RAM32KByte
        }

        private MBC1Mode _mode = MBC1Mode.ROM16Mbit_RAM8KByte;
        private readonly byte[] _romData;
        private int _selectedRomBank = 1;

        private readonly byte[] _ramData;
        private int _selectedRamBank = 0;
        private bool _ramWriteEnabled = false;

        public IMemoryBlock ROM { get; }
        public IMemoryBlock RAM { get; }

        public MBC1Cartridge(byte[] romData)
        {
            _romData = romData;
            // 32 KByte of RAM (TODO actual available RAM depends on concrete cartridge?)
            _ramData = new byte[32768];
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
            // Select ROM bank
            else if (address >= 0x2000 && address < 0x4000)
            {
                // Bits 6 and 7 are selected in the block below.
                var selection = value & 0b0001_1111;
                if (selection == 0)
                {
                    selection = 1;
                }
                _selectedRomBank = selection;
            }
            // Select RAM banks or upper ROM bank lines (depending on mode)
            else if (address >= 0x4000 && address < 0x6000)
            {
                var selection = value & 0b11;
                // Note: 8Kbyte is the full range accessible within 0xA000 and 0xBFFF. 
                // --> No RAM bank switching in that mode.
                if (_mode == MBC1Mode.ROM16Mbit_RAM8KByte)
                {
                    _selectedRomBank = (_selectedRomBank & 0b0001_1111) | (selection << 5);
                }
                else
                {
                    _selectedRamBank = selection;
                }
            }
            // Lowest bit selects memory mode
            else if (address >= 0x6000 && address < 0x8000)
            {
                if (value.GetBit(0))
                {
                    _mode = MBC1Mode.ROM4Mbit_RAM32KByte;
                }
                else
                {
                    _mode = MBC1Mode.ROM16Mbit_RAM8KByte;
                }
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
