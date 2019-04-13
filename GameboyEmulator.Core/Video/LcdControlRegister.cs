using GameboyEmulator.Core.Memory;

namespace GameboyEmulator.Core.Video
{
    // TODO maybe switch to IReadonlyRegister, or just use bool getters?
    public class LcdControlRegister : IRegister<byte>
    {
        public byte Value { get; set; }

        /// <summary>
        /// LCDC bit 0: If true, background is drawn.
        /// </summary>
        public IRegister<bool> BackgroundEnabled { get; }

        /// <summary>
        /// LCDC bit 1: If true, sprites are drawn.
        /// </summary>
        public IRegister<bool> SpritesEnabled { get; }

        /// <summary>
        /// LCDC bit 2: If true, sprite size is 8x16. If false, it is 8x8.
        /// </summary>
        public IRegister<bool> LargeSpritesEnabled { get; }

        /// <summary>
        /// LCDC bit 3: If false, BG tile map is 0x9800 ~ 0x9BFF. If true, BG tile map is 0x9C00 ~ 0x9FFF.
        /// </summary>
        public IRegister<bool> BackgroundTilemap { get; }

        /// <summary>
        /// LCDC bit 4: If false, tile set for BG and window is 0x8800 ~ 0x97FF. If true, it is 0x8000 ~ 0x8FFF.
        /// (Note that the overlap is by design.)
        /// </summary>
        public IRegister<bool> BackgroundTileset { get; }

        /// <summary>
        /// LCDC bit 5. If true, window is drawn based on WX and WY registers.
        /// </summary>
        public IRegister<bool> WindowEnabled { get; }

        /// <summary>
        /// LCDC bit 6: If false, window tile map is 0x9800 ~ 0x9BFF. If true, window tile map is 0x9C00 ~ 0x9FFF.
        /// </summary>
        public IRegister<bool> WindowTilemap { get; }

        /// <summary>
        /// LCDC bit 7: If true, LCD is enabled. If false, nothing is displayed at all.
        /// </summary>
        // TODO is this implemented properly?
        public IRegister<bool> LcdEnabled { get; }
        
        public LcdControlRegister()
        {
            BackgroundEnabled = new BoolPointer(this, 0);
            SpritesEnabled = new BoolPointer(this, 1);
            LargeSpritesEnabled = new BoolPointer(this, 2);
            BackgroundTilemap = new BoolPointer(this, 3);
            BackgroundTileset = new BoolPointer(this, 4);
            WindowEnabled = new BoolPointer(this, 5);
            WindowTilemap = new BoolPointer(this, 6);
            LcdEnabled = new BoolPointer(this, 7);
        }
    }
}
