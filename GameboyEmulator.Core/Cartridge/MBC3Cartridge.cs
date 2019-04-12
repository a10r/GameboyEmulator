using GameboyEmulator.Core.Memory;
using System;

namespace GameboyEmulator.Core.Cartridge
{
    /// <summary>
    /// A cartridge with the MBC3 controller.
    /// 
    /// It has between 64 KBytes and 2 MBytes of ROM and 32 KBytes of RAM.
    /// It also has a built-in clock.
    /// </summary>
    public class MBC3Cartridge : ICartridge
    {
        private struct RTCData
        {
            public byte Seconds;
            public byte Minutes;
            public byte Hours;
            public byte DaysLo;
            public byte DaysHi;
        }

        private readonly byte[] _romData;
        private int _selectedRomBank = 1;
        private readonly byte[] _ramData;
        // Values 0-3: maps into RAM
        // Values 8-12: RTC components accessible in 0xA000 ~ 0xBFFF.
        private int _selectedRamBank = 0;
        private bool _ramWriteEnabled = false;
        private RTCData _latchedRTCData;

        public IMemoryBlock ROM { get; }
        public IMemoryBlock RAM { get; }

        public MBC3Cartridge(byte[] romData)
        {
            _romData = romData;
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
            // Enable or disable RAM write protection.
            if (address < 0x2000)
            {
                _ramWriteEnabled = value == 0x0A;
            }
            // Select ROM bank.
            else if (address >= 0x2000 && address < 0x4000)
            {
                var selection = value & 0b0001_1111;
                if (selection == 0)
                {
                    selection = 1;
                }
                _selectedRomBank = selection;
            }
            else if (address >= 0x4000 && address < 0x6000)
            {
                // Write values 0-3 are for selecting RAM bank.
                // Higher values are for selecting RTC.
                if (!(value <= 3) && !(value >= 8 && value <= 12))
                {
                    throw new ArgumentException("Invalid MBC3 ram/rtc selection");
                }

                _selectedRamBank = value;
            }
            // Latches clock data.
            else if (address >= 0x6000 && address < 0x8000)
            {
                // TODO implement proper internal counting
                var now = DateTime.Now;
                var rtcData = new RTCData();
                rtcData.Seconds = (byte)now.Second;
                rtcData.Minutes = (byte)now.Minute;
                rtcData.Hours = (byte)now.Hour;
                _latchedRTCData = rtcData;
            }
        }
        
        private int EffectiveRamAddress(int address)
        {
            return address + (_selectedRamBank * 0x2000);
        }

        private byte ReadRam(int address)
        {
            if (_selectedRamBank <= 3)
            {
                return _ramData[EffectiveRamAddress(address)];
            }
            else if (_selectedRamBank == 8)
            {
                return _latchedRTCData.Seconds;
            }
            else if (_selectedRamBank == 9)
            {
                return _latchedRTCData.Minutes;
            }
            else if (_selectedRamBank == 10)
            {
                return _latchedRTCData.Hours;
            }
            else if (_selectedRamBank == 11)
            {
                return _latchedRTCData.DaysLo;
            }
            else if (_selectedRamBank == 2)
            {
                return _latchedRTCData.DaysHi;
            }

            return 0;
        }

        private void WriteRam(int address, byte value)
        {
            if (_ramWriteEnabled)
            {
                if (_selectedRamBank <= 3)
                {
                    _ramData[EffectiveRamAddress(address)] = value;
                }
                else
                {
                    throw new NotImplementedException("Write into MBC3 clock counters.");
                }
            }
        }
    }
}
