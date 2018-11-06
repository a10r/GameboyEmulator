using System;
using System.Diagnostics;
using System.Drawing;
using GameboyEmulator.Core.Memory;
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
        private readonly Bitmap _framebuffer;

        private readonly LcdControlRegister _lcdc;
        private readonly LcdStatusRegister _stat;
        private LcdMode _currentMode = LcdMode.OamSearch; // TODO incorporate this into stat register code

        private readonly IRegister<byte> _scx;
        private readonly IRegister<byte> _scy;
        private readonly IRegister<byte> _scanline; // ly
        private readonly IRegister<byte> _bgp; // BG palette data
        private readonly IMemoryBlock _vram;
        private readonly IMemoryBlock _oam;
        private readonly IRegister<byte> _if; // TODO: only take one flag
        private int _counter;

        private long _globalCounter;
        private long _lastVblank;
        

        public LcdController(
            LcdControlRegister lcdc, 
            LcdStatusRegister stat,
            IRegister<byte> scx,
            IRegister<byte> scy,
            IRegister<byte> ly,
            IRegister<byte> bgp,
            IMemoryBlock vram, 
            IMemoryBlock oam,
            IRegister<byte> @if)
        {
            Debug.Assert(vram.Size == 8192);
            Debug.Assert(oam.Size == 160);

            _framebuffer = new Bitmap(160, 144);
            _lcdc = lcdc;
            _stat = stat;
            _scx = scx;
            _scy = scy;
            _scanline = ly;
            _bgp = bgp;
            _vram = vram;
            _oam = oam;
            _if = @if;
        }

        // TODO: Make this more efficient. Currently one clock per call. 
        // TODO: Timing is not accurate. (Depends on sprite count etc)
        public int Tick()
        {
            if (!_lcdc.LcdEnable.Value) return 1;

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
                        _scanline.Value++;
                        if (_scanline.Value == 144) // TODO maybe 143???
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
                        _scanline.Value++;
                        _counter = 0; // TODO re-check
                    }
                    if (_scanline.Value == 154)
                    {
                        _scanline.Value = 0;
                        ChangeMode(LcdMode.OamSearch);
                    }
                    break;
            }

            _stat.Mode = _currentMode; // TODO
            return 1;
        }

        private void ChangeMode(LcdMode newMode)
        {
            _currentMode = newMode;
            _counter = 0;

            if (newMode == LcdMode.HorizontalBlank)
            {
                RenderScanline(_scanline.Value);
                // TODO: HBlank interrupt
                _if.SetBit(1, true);
            }
            else if (newMode == LcdMode.VerticalBlank)
            {
                //Console.WriteLine($"Vblank after {_globalCounter - _lastVblank} cycles.");
                _lastVblank = _globalCounter;
                

                OnCompletedFrame();
                // TODO: VBlank interrupt
                _if.SetBit(0, true);
            }
        }

        private void RenderScanline(int i)
        {
            if (i >= 144) Console.WriteLine($"[debug] Scanline {i}");

            var shades = new[]
            {
                new Pixel(255, 255, 255),
                new Pixel(192, 192, 192),
                new Pixel(96, 96, 96),
                new Pixel(0, 0, 0),
            };

            var tilemapOffset = _lcdc.BackgroundTilemap.Value ? 0x1C00 : 0x1800;
            var tilesetOffset = _lcdc.BackgroundTileset.Value ? 0x0000 : 0x0800;

            // Note: scanline and scroll values are pixel based, not tile based
            var globalRow = (i + _scy.Value) % 256;

            var mapY = globalRow >> 3; // static for scanline!
            var mapX = _scx.Value >> 3; // TODO wrapping
            var tileIndex = (int)_vram[tilemapOffset + mapY * 32 + mapX];
            if (_lcdc.BackgroundTileset.Value == false && tileIndex >= 128)
            {
                tileIndex -= 256; // TODO test if this works
            }
            var tileY = globalRow & 0b111; // static for scanline!
            var tileX = _scx.Value & 0b111;
            
            for (var x = 0; x < 160; x++)
            {
                var lower = _vram[tilesetOffset + tileIndex * 16 + tileY * 2];
                var upper = _vram[tilesetOffset + tileIndex * 16 + tileY * 2 + 1];
                    
                var shade = (upper.GetBit(7-tileX) ? 2 : 0) + (lower.GetBit(7-tileX) ? 1 : 0);

                // map tile shade through BG palette
                _framebuffer[x, i] = shades[((0b11 << (2*shade)) & _bgp.Value) >> (2*shade)];

                tileX++;

                if (tileX == 8)
                {
                    tileX = 0;
                    mapX++;
                    tileIndex = _vram[tilemapOffset + mapY * 32 + mapX];
                    if (_lcdc.BackgroundTileset.Value == false && tileIndex >= 128)
                    {
                        tileIndex -= 256; // TODO test if this works
                    }
                }
            }
        }

        public event EventHandler<FrameEventArgs> NewFrame;

        private void OnCompletedFrame()
        {
            NewFrame?.Invoke(this, new FrameEventArgs {Frame = _framebuffer});
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
