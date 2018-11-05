using System.IO;

namespace GameboyEmulator.Core.Video
{
    public struct Pixel
    {
        public byte R;
        public byte G;
        public byte B;

        public Pixel(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }
    }

    public class Bitmap
    {
        private Pixel[,] _data;
        public int Width { get; }
        public int Height { get; }

        public ref Pixel this[int x, int y] => ref _data[x, y];

        public Bitmap(int width, int height)
        {
            _data = new Pixel[width, height];
            Width = width;
            Height = height;
        }

        public void Save(Stream targetStream)
        {
            foreach (var pixel in _data)
            {
                targetStream.WriteByte(pixel.R);
                targetStream.WriteByte(pixel.G);
                targetStream.WriteByte(pixel.B);
            }
            targetStream.Flush();
        }
    }
}
