﻿using System;
using System.Diagnostics;
using GameboyEmulator.Core.Memory;
using GameboyEmulator.Core.Processor;
using GameboyEmulator.Core.Utils;

namespace GameboyEmulator.Core.Video
{
    public enum LcdMode
    {
        OamSearch = 2, // mode 10
        DataTransfer = 3, // mode 11
        HorizontalBlank = 0, // mode 00
        VerticalBlank = 1, // mode 01
    }

    public class LcdController : IFrameSource
    {
        private Bitmap _framebuffer;

        private readonly LcdControlRegister _lcdc;
        private readonly LcdStatusRegister _stat;
        private LcdMode _currentMode = LcdMode.OamSearch; // TODO incorporate this into stat register code

        private readonly IRegister<byte> _scx; // scroll x
        private readonly IRegister<byte> _scy; // scroll y
        private readonly IRegister<byte> _ly; // current scanline
        private readonly IRegister<byte> _lyc; // scanline compare
        private readonly IRegister<byte> _bgp; // BG palette
        private readonly IRegister<byte> _obp0; // OBJ palette 0
        private readonly IRegister<byte> _obp1; // OBJ palette 1
        private readonly IRegister<byte> _wy; // Window y position (0~143)
        private readonly IRegister<byte> _wx; // Window x position minus 7 (7~166)
        private readonly IMemoryBlock _vram;
        private readonly IMemoryBlock _oam;

        private readonly IInterruptTrigger _vblankInterrupt;
        private readonly IInterruptTrigger _lcdStatusInterrupt;

        private int _counter;

        private long _globalCounter;
        private long _lastVblank;


        public LcdController(
            LcdControlRegister lcdc,
            LcdStatusRegister stat,
            IRegister<byte> scx,
            IRegister<byte> scy,
            IRegister<byte> ly,
            IRegister<byte> lyc,
            IRegister<byte> bgp,
            IMemoryBlock vram,
            IMemoryBlock oam,
            IRegister<byte> obp0,
            IRegister<byte> obp1,
            IRegister<byte> wy,
            IRegister<byte> wx,
            IInterruptTrigger vblankInterrupt,
            IInterruptTrigger lcdStatusInterrupt)
        {
            Debug.Assert(vram.Size == 8192);
            Debug.Assert(oam.Size == 160);

            _framebuffer = new Bitmap(160, 144);
            _lcdc = lcdc;
            _stat = stat;
            _scx = scx;
            _scy = scy;
            _ly = ly;
            _lyc = lyc;
            _bgp = bgp;
            _vram = vram;
            _oam = oam;
            _obp0 = obp0;
            _obp1 = obp1;
            _vblankInterrupt = vblankInterrupt;
            _lcdStatusInterrupt = lcdStatusInterrupt;
            _wy = wy;
            _wx = wx;
        }

        // TODO: Make this more efficient. Currently one clock per call. 
        // TODO: Timing is not accurate. (Depends on sprite count etc)
        public int Tick()
        {
            if (!_lcdc.LcdEnabled.Value) return 1;

            _counter++;

            _globalCounter++;

            switch (_currentMode)
            {
                case LcdMode.OamSearch:
                    if (_counter >= 80)
                    {
                        ChangeMode(LcdMode.DataTransfer);
                    }
                    break;
                case LcdMode.DataTransfer:
                    if (_counter >= 172)
                    {
                        ChangeMode(LcdMode.HorizontalBlank);
                    }
                    break;
                case LcdMode.HorizontalBlank:
                    if (_counter >= 204)
                    {
                        _ly.Value++;
                        if (_ly.Value == 144) // TODO maybe 143???
                        {
                            ChangeMode(LcdMode.VerticalBlank);
                        }
                        else
                        {
                            ChangeMode(LcdMode.OamSearch);
                        }
                    }
                    break;
                case LcdMode.VerticalBlank:
                    // TODO this is wrong, maybe?
                    if (_counter >= 456)
                    {
                        _ly.Value++;
                        _counter = 0; // TODO re-check
                    }
                    if (_ly.Value == 154)
                    {
                        _ly.Value = 0;
                        ChangeMode(LcdMode.OamSearch);
                    }
                    break;
            }

            if (_ly.Value == _lyc.Value && _stat.ScanlineCoincidenceInterruptEnabled.Value)
            {
                _lcdStatusInterrupt.Trigger();
            }

            _stat.Mode = _currentMode; // TODO
            return 1;
        }

        private void ChangeMode(LcdMode newMode)
        {
            _currentMode = newMode;
            _counter = 0;

            if (newMode == LcdMode.OamSearch)
            {
                if (_stat.OamSearchInterruptEnabled.Value)
                {
                    _lcdStatusInterrupt.Trigger();
                }
            }
            else if (newMode == LcdMode.DataTransfer)
            {
                // Do nothing.
            }
            else if (newMode == LcdMode.HorizontalBlank)
            {
                RenderScanline(_ly.Value);

                if (_stat.HBlankInterruptEnabled.Value)
                {
                    _lcdStatusInterrupt.Trigger();
                }
            }
            else if (newMode == LcdMode.VerticalBlank)
            {
                //Console.WriteLine($"Vblank after {_globalCounter - _lastVblank} cycles.");
                _lastVblank = _globalCounter;


                OnCompletedFrame();

                _vblankInterrupt.Trigger();

                // TODO does vblank really have two interrupts?
                if (_stat.VBlankInterruptEnabled.Value)
                {
                    _vblankInterrupt.Trigger();
                }
            }
        }

        private int NormalizeTileIndex(int tileIndex)
        {
            if (!_lcdc.BackgroundTileset.Value && tileIndex < 128)
            {
                tileIndex += 256;
            }
            return tileIndex;
        }

        private int TileIndexFromMapPosition(IRegister<bool> tilemapSelect, int mapX, int mapY)
        {
            var tilemapOffset = tilemapSelect.Value ? 0x1C00 : 0x1800;
            var tileIndex = (int)_vram[tilemapOffset + mapY * 32 + mapX];
            return NormalizeTileIndex(tileIndex);
        }

        /// <summary>
        /// 4 shades, from white to black.
        /// </summary>
        private static Pixel[] MonochromeShades = new[]
            {
                new Pixel(255, 255, 255),
                new Pixel(192, 192, 192),
                new Pixel(96, 96, 96),
                new Pixel(0, 0, 0),
            };

        private Pixel MapShadeThroughPalette(int shade, byte palette) => MonochromeShades[((0b11 << (2 * shade)) & palette) >> (2 * shade)];

        private void RenderScanline(int i)
        {
            if (i >= 144) Console.WriteLine($"[debug] Scanline {i}");

            if (_lcdc.BackgroundEnabled.Value)
            {
                RenderBackgroundForScanline(i);
            }
            
            if (_lcdc.WindowEnabled.Value)
            {
                RenderWindowForScanline(i);
            }

            if (_lcdc.SpritesEnabled.Value)
            {
                RenderSpritesForScanline(i);
            }
        }

        // Stores raw shades of the BG layer for the current scanline.
        // This is needed for ensuring correct bg/sprite priority.
        private int[] _rawBackgroundLayer = new int[160];

        private void RenderBackgroundForScanline(int i)
        {
            // Note: scanline and scroll values are pixel based, not tile based
            var globalRow = (i + _scy.Value) % 256;

            var mapY = globalRow >> 3; // static for scanline!
            var mapX = _scx.Value >> 3;
            var tileIndex = TileIndexFromMapPosition(_lcdc.BackgroundTilemap, mapX, mapY);
            var tileY = globalRow & 0b111; // static for scanline!
            var tileX = _scx.Value & 0b111;

            for (var x = 0; x < 160; x++)
            {
                var lower = _vram[tileIndex * 16 + tileY * 2];
                var upper = _vram[tileIndex * 16 + tileY * 2 + 1];

                var shade = (upper.GetBit(7 - tileX) ? 2 : 0) + (lower.GetBit(7 - tileX) ? 1 : 0);

                // map tile shade through BG palette
                _framebuffer[x, i] = MapShadeThroughPalette(shade, _bgp.Value);
                _rawBackgroundLayer[x] = shade;

                tileX++;

                if (tileX == 8)
                {
                    tileX = 0;
                    mapX = (mapX + 1) % 32;
                    tileIndex = TileIndexFromMapPosition(_lcdc.BackgroundTilemap, mapX, mapY);
                }
            }
        }

        // TODO caching mechanism to make this faster?
        // TODO "Up to 10 objects can be displayed on the same Y line."
        private void RenderSpritesForScanline(int i)
        {
            const int SPRITE_ENTRY_SIZE = 4;

            var globalRow = (i + _scy.Value) % 256;
            
            var spriteMaxHeight = _lcdc.LargeSpritesEnabled.Value ? 16 : 8;

            for (int spriteIdx = 0; spriteIdx < 40; spriteIdx++)
            {
                var spriteScreenY = _oam[SPRITE_ENTRY_SIZE * spriteIdx] - 16; // stored value is screen coordinate + 16
                var spriteActiveY = i - spriteScreenY;
                if (spriteActiveY < 0 || spriteActiveY >= spriteMaxHeight)
                {
                    continue; // Sprite invisible
                }

                var spriteScreenX = _oam[SPRITE_ENTRY_SIZE * spriteIdx + 1] - 8; // stored value is screen coordinate + 8
                if (spriteScreenX < 0 || spriteScreenX >= 160)
                {
                    continue; // Sprite invisible
                }

                var tileIndex = _oam[SPRITE_ENTRY_SIZE * spriteIdx + 2];
                var attributes = _oam[SPRITE_ENTRY_SIZE * spriteIdx + 3]; // DMG: lower 4 bits do nothing; CGB: bank/palette selection
                
                var paletteAttr = attributes.GetBit(4); // unused on CGB
                var xFlip = attributes.GetBit(5); // 1 = flipped horizontally
                var yFlip = attributes.GetBit(6); // 1 = flipped vertically
                var belowBg = attributes.GetBit(7); // 1 = sprite is below BG (except if BG shade is 0)

                var palette = paletteAttr ? _obp1 : _obp0;

                if (yFlip)
                {
                    spriteActiveY = (spriteMaxHeight - 1) - spriteActiveY;
                }

                for (int pixelCount = 0; pixelCount < 8; pixelCount++)
                {
                    int writeX;
                    if (xFlip)
                    {
                        writeX = spriteScreenX + 7 - pixelCount;
                    }
                    else
                    {
                        writeX = spriteScreenX + pixelCount;
                    }
                    
                    if (writeX < 0 || writeX >= 160)
                    {
                        continue;
                    }
                    
                    // Sprite drawn below BG
                    if (_rawBackgroundLayer[writeX] != 0 && belowBg)
                    {
                        continue;
                    }

                    var lower = _vram[tileIndex * 16 + spriteActiveY * 2];
                    var upper = _vram[tileIndex * 16 + spriteActiveY * 2 + 1];

                    var shade = (upper.GetBit(7 - pixelCount) ? 2 : 0) + (lower.GetBit(7 - pixelCount) ? 1 : 0);

                    // Color 0 == transparent
                    if (shade == 0)
                    {
                        continue;
                    }

                    // map tile shade through OBJ palette
                    _framebuffer[writeX, i] = MapShadeThroughPalette(shade, palette.Value);
                }
            }
        }
        
        private void RenderWindowForScanline(int i)
        {
            if (i < _wy.Value)
            {
                return;
            }

            var windowLeft = _wx.Value - 7;
            var mapY = (i - _wy.Value) >> 3; // static for scanline!
            var mapX = 0;
            var tileIndex = TileIndexFromMapPosition(_lcdc.WindowTilemap, mapX, mapY);
            var tileY = (i - _wy.Value) & 0b111; // static for scanline!
            var tileX = 0;

            for (var x = windowLeft; x < 160; x++)
            {
                var lower = _vram[tileIndex * 16 + tileY * 2];
                var upper = _vram[tileIndex * 16 + tileY * 2 + 1];

                var shade = (upper.GetBit(7 - tileX) ? 2 : 0) + (lower.GetBit(7 - tileX) ? 1 : 0);

                if (x >= 0)
                {
                    // map tile shade through BG palette
                    _framebuffer[x, i] = MapShadeThroughPalette(shade, _bgp.Value);
                    _rawBackgroundLayer[x] = shade;
                }

                tileX++;

                if (tileX == 8)
                {
                    tileX = 0;
                    mapX++;
                    tileIndex = TileIndexFromMapPosition(_lcdc.WindowTilemap, mapX, mapY);
                }
            }
        }

        public event EventHandler<FrameEventArgs> NewFrame;

        private void OnCompletedFrame()
        {
            NewFrame?.Invoke(this, new FrameEventArgs { Frame = _framebuffer });

            // New cleared framebuffer for next frame.
            _framebuffer = new Bitmap(160, 144);
        }
    }

    public interface IFrameSource
    {
        event EventHandler<FrameEventArgs> NewFrame;
    }

    public class FrameEventArgs : EventArgs
    {
        public Bitmap Frame { get; set; }
    }
}
