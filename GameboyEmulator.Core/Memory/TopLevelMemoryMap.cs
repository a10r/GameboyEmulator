using System;
using System.Diagnostics;

namespace GameboyEmulator.Core.Memory
{
    public class TopLevelMemoryMap : IMemoryBlock
    {
        private readonly IMemoryBlock _cartridgeRom;
        private readonly IMemoryBlock _cartridgeRam;
        private readonly IMemoryBlock _vram;
        private readonly IMemoryBlock _internalRam;
        private readonly IMemoryBlock _oam;
        private readonly IMemoryBlock _ioPorts;

        public int Size => 65536;

        public byte this[int address]
        {
            get
            {
                if (address < 0x8000)
                {
                    return _cartridgeRom[address];
                }
                if (address < 0xA000)
                {
                    // TODO: GBC uses bank switching here!
                    return _vram[address - 0x8000];
                }
                if (address < 0xC000)
                {
                    return _cartridgeRam[address - 0xA000];
                }
                if (address < 0xE000)
                {
                    // TODO: GBC has switchable banks here!
                    return _internalRam[address - 0xC000];
                }
                if (address < 0xFE00)
                {
                    // Manual says access is prohibited, but actual hardware
                    // seems to mirror internal RAM here.
                    return _internalRam[address - 0xE000];
                }
                if (address < 0xFEA0)
                {
                    return _oam[address - 0xFE00];
                }
                if (address < 0xFF00)
                {
                    // Unmapped memory region. Hardware always returns 0xFF.
                    return 0xFF;
                }
                if (address <= 0xFFFF)
                {
                    return _ioPorts[address - 0xFF00];
                }
                throw new InvalidOperationException("Invalid address.");
            }

            set
            {
                if (address < 0x8000)
                {
                    _cartridgeRom[address] = value;
                }
                else if (address < 0xA000)
                {
                    _vram[address - 0x8000] = value;
                }
                else if (address < 0xC000)
                {
                    _cartridgeRam[address - 0xA000] = value;
                }
                else if (address < 0xE000)
                {
                    _internalRam[address - 0xC000] = value;
                }
                else if (address < 0xFE00)
                {
                    // Manual says access is prohibited, but actual hardware
                    // seems to mirror internal RAM here.
                    _internalRam[address - 0xE000] = value;
                }
                else if (address < 0xFEA0)
                {
                    _oam[address - 0xFE00] = value;
                }
                else if (address < 0xFF00)
                {
                    // Unmapped memory region. Nothing happens.
                }
                else if (address <= 0xFFFF)
                {
                    _ioPorts[address - 0xFF00] = value;
                }
                else
                {
                    throw new InvalidOperationException("Invalid address.");
                }
            }
        }

        public TopLevelMemoryMap(IMemoryBlock cartridgeRom,
            IMemoryBlock cartridgeRam,
            IMemoryBlock vram,
            IMemoryBlock internalRam,
            IMemoryBlock oam,
            IMemoryBlock ioPorts)
        {
            Debug.Assert(vram.Size == 8192);
            Debug.Assert(oam.Size == 160);

            _cartridgeRom = cartridgeRom;
            _cartridgeRam = cartridgeRam;
            _vram = vram;
            _internalRam = internalRam;
            _oam = oam;
            _ioPorts = ioPorts;
        }
    }
}
