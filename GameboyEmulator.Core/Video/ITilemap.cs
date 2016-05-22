using GameboyEmulator.Core.Memory;

namespace GameboyEmulator.Core.Video
{
    public interface IVideoRam
    {
        ITilemap Tilemap { get; }
    }

    public interface ITilemap
    {
        ITile this[int x, int y] { get; }
    }

    public interface ITile
    {
        int this[int x, int y] { get; }
    }

    public class VideoRam : IVideoRam
    {
        private readonly IRegister<bool> _tilemapSelect;
        private readonly IRegister<bool> _tilesetSelect;
        private readonly IMemoryBlock _vram;

        public VideoRam(IMemoryBlock vram, IRegister<bool> tilemapSelect, IRegister<bool> tilesetSelect)
        {
            _vram = vram;
            _tilemapSelect = tilemapSelect;
            _tilesetSelect = tilesetSelect;
        }

        public ITilemap Tilemap { get; }
    }

    public class Tilemap : ITilemap
    {
        public Tilemap()
        {
            
        }

        public ITile this[int x, int y]
        {
            get { throw new System.NotImplementedException(); }
        }
    }

    public class Tile : ITile
    {
        public int this[int x, int y]
        {
            get { throw new System.NotImplementedException(); }
        }

        public Tile()
        {
            
        }
    }
}