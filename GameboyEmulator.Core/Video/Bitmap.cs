using System;
using System.Runtime.InteropServices;

namespace GameboyEmulator.Core.Video
{
    // TODO maybe this would be faster if it was aligned?
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 3)]
    public struct Pixel
    {
        [FieldOffset(0)]
        public byte R;

        [FieldOffset(1)]
        public byte G;

        [FieldOffset(2)]
        public byte B;

        public Pixel(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public Pixel(byte grayScale)
            : this(grayScale, grayScale, grayScale)
        {
        }
    }
    
    public class Bitmap
    {
        // Memory layout: 1st dimension = rows; 2nd dimension = pixels within row; so essentially: height before width.
        // Origin is the top left.
        private Pixel[,] _data;

        public int Width { get; }
        public int Height { get; }

        // Dimensions when accessing pixels are flipped for convenience.
        public ref Pixel this[int x, int y] => ref _data[y, x];

        public Bitmap(int width, int height)
        {
            _data = new Pixel[height, width];
            Width = width;
            Height = height;
        }

        public void Populate(Func<int, int, Pixel> populator)
        {
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    this[x, y] = populator(x, y);
                }
            }
        }

        public PinnedBitmapData Pinned(BitmapOrientation orientation = BitmapOrientation.Normal)
        {
            var data = (Pixel[,])_data.Clone();

            // TODO remove this hack once the OpenGL display is more sophisticated that DrawPixels
            if (orientation == BitmapOrientation.FlipY)
            {
                for (var y = 0; y < Height; y++)
                {
                    for (var x = 0; x < Width; x++)
                    {
                        data[y, x] = _data[Height - y - 1, x];
                    }
                }
            }

            return new PinnedBitmapData(data);
        }

        public Bitmap Clone()
        {
            return (Bitmap)MemberwiseClone();
        }
    }

    public enum BitmapOrientation
    {
        Normal,
        FlipY
    }

    public class PinnedBitmapData : IDisposable
    {
        private GCHandle _handle;

        internal PinnedBitmapData(Pixel[,] pixelData)
        {
            _handle = GCHandle.Alloc(pixelData, GCHandleType.Pinned);
        }

        public IntPtr Pointer => _handle.AddrOfPinnedObject();

        ~PinnedBitmapData()
        {
            Dispose();
        }

        public void Dispose()
        {
            _handle.Free();
            GC.SuppressFinalize(this);
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
