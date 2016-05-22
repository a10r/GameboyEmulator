using System;
using System.Drawing;
using System.Drawing.Imaging;
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
        private readonly Framebuffer _framebuffer;

        private readonly LcdControlRegister _lcdc;
        private readonly LcdStatusRegister _stat;

        private readonly IRegister<byte> _scx;
        private readonly IRegister<byte> _scy;
        private readonly IRegister<byte> _scanline;
        private readonly IMemoryBlock _vram;
        private readonly IMemoryBlock _oam;
        
        private int _counter;
        
        public LcdController(LcdControlRegister lcdc, 
            LcdStatusRegister stat,
            IRegister<byte> scx,
            IRegister<byte> scy,
            IRegister<byte> ly,
            IMemoryBlock vram, 
            IMemoryBlock oam)
        {
            System.Diagnostics.Debug.Assert(vram.Size == 8192);
            System.Diagnostics.Debug.Assert(oam.Size == 120);
            _framebuffer = new Framebuffer(160, 144);
            _lcdc = lcdc;
            _stat = stat;
            _scx = scx;
            _scy = scy;
            _scanline = ly;
            _vram = vram;
            _oam = oam;
        }

        // TODO: Make this more efficient. Currently one clock per call. 
        // TODO: Timing is not accurate. (Depends on sprite count etc)
        public int Tick()
        {
            if (!_lcdc.LcdEnable.Value) return 0;

            _counter++;

            switch (_stat.Mode)
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
                        if (_scanline.Value == 144)
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
                    if (_counter >= 456)
                    {
                        _scanline.Value++;
                    }
                    if (_scanline.Value == 154)
                    {
                        _scanline.Value = 0;
                        ChangeMode(LcdMode.OamSearch);
                    }
                    break;
            }

            return 1;
        }

        private void ChangeMode(LcdMode newMode)
        {
            _stat.Mode = newMode;
            _counter = 0;

            if (newMode == LcdMode.HorizontalBlank)
            {
                RenderScanline(_scanline.Value);
                // TODO: HBlank interrupt
            }
            else if (newMode == LcdMode.VerticalBlank)
            {
                OnCompletedFrame();
                // TODO: VBlank interrupt
            }
        }

        private int _frameCount = 0;

        private void __DummyRenderScanline(int y)
        {
            if (y == 0) _frameCount++;
            
            var grayscale = (_frameCount + y) % 255;
            var color = Color.FromArgb(grayscale, grayscale, grayscale);
            
            // Slow as fk
            //_framebuffer.SetPixel(j, i, color);

            //var data = _framebuffer.LockBits(new Rectangle(0, 0, _framebuffer.Width, _framebuffer.Height),
            //    ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            //var stride = data.Stride;
            //unsafe
            //{
            //    var ptr = (byte*)data.Scan0;
            //    for (var x = 0; x < 160; x++)
            //    {
            //        var pixelOffset = x * 3 + y * stride;
            //        ptr[pixelOffset] = color.R;
            //        ptr[pixelOffset + 1] = color.G;
            //        ptr[pixelOffset + 2] = color.B;
            //    }
            //}

            //_framebuffer.UnlockBits(data);
        }
        
        private void RenderScanline(int i)
        {
            //__DummyRenderScanline(i);

            var tilemapOffset = _lcdc.BackgroundTilemap.Value ? 0x1C00 : 0x1800;
            var tilesetOffset = _lcdc.BackgroundTileset.Value ? 0x0000 : 0x1000;

            var globalRow = i + _scy.Value;

            var mapY = globalRow >> 3; // static!
            var mapX = _scx.Value >> 3;
            var tileIndex = _vram[tilemapOffset + mapY * 32 + mapX];
            var tileY = globalRow & 7; // static!
            var tileX = _scx.Value & 7;
            
            for (var x = 0; x < 160; x++)
            {
                var lower = _vram[tilesetOffset + tileIndex * 16 + tileY * 2];
                var upper = _vram[tilesetOffset + tileIndex * 16 + tileY * 2 + 1];
                    
                var shade = (upper.GetBit(7-tileX) ? 2 : 0) + (lower.GetBit(7-tileX) ? 1 : 0);

                // TODO: use proper palette
                shade = 4 - shade;
                var color = Color.FromArgb(shade*60, shade*60, shade*60);
                
                _framebuffer.Data[x, i].R = color.R;
                _framebuffer.Data[x, i].G = color.G;
                _framebuffer.Data[x, i].B = color.B;

                tileX++;

                if (tileX == 8)
                {
                    tileX = 0;
                    mapX++;
                    tileIndex = _vram[tilemapOffset + mapY * 32 + mapX];
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
        public Framebuffer Frame { get; set; }
    }
}
