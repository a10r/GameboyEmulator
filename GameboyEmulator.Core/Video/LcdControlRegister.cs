using GameboyEmulator.Core.Memory;

namespace GameboyEmulator.Core.Video
{
    // TODO maybe switch to IReadonlyRegister, or just use a bool getter?
    public class LcdControlRegister : IRegister<byte>
    {
        public byte Value { get; set; }

        public IRegister<bool> BackgroundEnabled { get; }
        public IRegister<bool> SpritesEnabled { get; }

        /// <summary>
        /// If true, sprite size is 8x16. If false, it is 8x8.
        /// </summary>
        public IRegister<bool> LargeSpritesEnabled { get; }

        public IRegister<bool> BackgroundTilemap { get; }
        public IRegister<bool> BackgroundTileset { get; }
        public IRegister<bool> WindowEnabled { get; }
        public IRegister<bool> LcdEnabled { get; }

        // TODO: window code area selection flag

        public LcdControlRegister()
        {
            BackgroundEnabled = new BoolPointer(this, 0);
            SpritesEnabled = new BoolPointer(this, 1);
            LargeSpritesEnabled = new BoolPointer(this, 2);
            BackgroundTilemap = new BoolPointer(this, 3);
            BackgroundTileset = new BoolPointer(this, 4);
            WindowEnabled = new BoolPointer(this, 5);
            LcdEnabled = new BoolPointer(this, 7);
        }
    }
}
