using GameboyEmulator.Core.Memory;

namespace GameboyEmulator.Core.Video
{
    public class LcdControlRegister : IRegister<byte>
    {
        public byte Value { get; set; }

        // TODO maybe switch to IReadonlyRegister?
        public IRegister<bool> BackgroundEnable { get; private set; }
        public IRegister<bool> SpriteEnable { get; private set; }

        // TODO: keep this as bool?
        public IRegister<bool> SpriteMode { get; private set; }
        public IRegister<bool> BackgroundTilemap { get; private set; }
        public IRegister<bool> BackgroundTileset { get; private set; }
        public IRegister<bool> WindowEnable { get; private set; }
        public IRegister<bool> LcdEnable { get; private set; }

        // TODO: window code area selection flag

        public LcdControlRegister()
        {
            BackgroundEnable = new BoolPointer(this, 0);
            SpriteEnable = new BoolPointer(this, 1);
            SpriteMode = new BoolPointer(this, 2);
            BackgroundTilemap = new BoolPointer(this, 3);
            BackgroundTileset = new BoolPointer(this, 4);
            WindowEnable = new BoolPointer(this, 5);
            LcdEnable = new BoolPointer(this, 7);
        }
    }
}
