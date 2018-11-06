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

    public static class BitmapExtensions
    {
        public static Bitmap Overlay(this Bitmap bottom, Bitmap top, int x, int y)
        {
            for (int iy = 0; iy < top.Height; iy++)
            {
                for (int ix = 0; ix < top.Width; ix++)
                {
                    bottom[x + ix, y + iy] = top[ix, iy];
                }
            }

            return bottom;
        }

        public static Bitmap DrawRectangle(this Bitmap b, int x, int y, int width, int height, Pixel color)
        {
            // top edge
            for (int ix = 0; ix < width; ix++)
            {
                b[(x + ix) % b.Width, y % b.Height] = color;
            }

            // bottom edge
            for (int ix = 0; ix < width; ix++)
            {
                b[(x + ix) % b.Width, (y + height) % b.Height] = color;
            }

            // left edge
            for (int iy = 0; iy < height; iy++)
            {
                b[x % b.Width, (y + iy) % b.Height] = color;
            }

            // right edge
            for (int iy = 0; iy < height; iy++)
            {
                b[(x + width) % b.Width, (y + iy) % b.Height] = color;
            }

            return b;
        }

        public static Bitmap DrawRectangle(this Bitmap b, int x, int y, int width, int height) 
            => b.DrawRectangle(x, y, width, height, new Pixel(0xFF, 0x00, 0x00));
    }
}
