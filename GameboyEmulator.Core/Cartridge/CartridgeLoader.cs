using System;
using System.IO;

namespace GameboyEmulator.Core.Cartridge
{
    public static class CartridgeLoader
    {
        public static ICartridge FromFile(string romFile)
        {
            var data = File.ReadAllBytes(romFile);

            var cartridgeType = data[0x0147];

            switch (cartridgeType)
            {
                case 0x00: // ROM only
                    return new RomOnlyCartridge(data);

                case 0x01: // MBC1
                case 0x02: // MBC1 + RAM
                case 0x03: // MBC1 + RAM + BATTERY
                    return new MBC1Cartridge(data);

                case 0x05: // MBC2
                case 0x06: // MBC2 + BATTERY
                    return new MBC2Cartridge(data);

                case 0x08: // ROM + BATTERY
                case 0x09: // ROM + RAM + BATTERY
                    // TODO what is this?
                    return new RomOnlyCartridge(data);

                case 0x0B: // MMM01
                case 0x0C: // MMM01 + RAM
                case 0x0D: // MMM01 + RAM + BATTERY
                    // This one is really rare apparently.
                    throw new NotSupportedException("MMM01 controller not supported.");

                case 0x0F: // MBC3 + TIMER + BATTERY
                case 0x10: // MBC3 + TIMER + RAM + BATTERY
                case 0x11: // MBC3
                case 0x12: // MBC3 + RAM
                case 0x13: // MBC3 + RAM + BATTERY
                    return new MBC3Cartridge(data);

                case 0x15: // MBC4
                case 0x16: // MBC4 + RAM
                case 0x17: // MBC4 + RAM + BATTERY
                    // What is this? It's not even in the manual.
                    throw new NotSupportedException("MBC4 controller not supported.");

                case 0x19: // MBC5
                case 0x1A: // MBC5 + RAM
                case 0x1B: // MBC5 + RAM + BATTERY
                case 0x1C: // MBC5 + RUMBLE
                case 0x1D: // MBC5 + RUMBLE + RAM
                case 0x1E: // MBC5 + RUMBLE + RAM + BATTERY
                    // TODO rumble not supported yet.
                    return new MBC5Cartridge(data);

                // These are not im the manual either.
                case 0x20: // MBC6
                case 0x22: // MBC7 + SENSOR + RUMBLE + RAM + BATTERY
                    
                case 0xFC: // POCKET CAMERA
                case 0xFD: // BANDAI TAMA5
                case 0xFE: // HuC3
                case 0xFF: // HuC1 + RAM + BATTERY
                    throw new NotSupportedException("Special cartridge type not supported.");

                default:
                    throw new NotSupportedException("Unsupported cartridge type.");
            };

            //var romSize = data[0x0148];
            //var ramSize = data[0x0149];
        }
    }
}
