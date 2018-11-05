using Eto.Drawing;

namespace GameboyEmulator.UI.Util
{
    public static class BitmapUtils
    {
        public static Bitmap ToEtoBitmap(this Core.Video.Bitmap coreBitmap)
        {
            var bitmap = new Bitmap(new Size(coreBitmap.Width, coreBitmap.Height), PixelFormat.Format24bppRgb);
            var bitmapData = bitmap.Lock();

            for (int x = 0; x < coreBitmap.Width; x++)
            {
                for (int y = 0; y < coreBitmap.Height; y++)
                {
                    var pixel = coreBitmap[x, y];
                    bitmapData.SetPixel(x, y, Color.FromArgb(pixel.R, pixel.G, pixel.B, 255));
                }
            }

            bitmapData.Dispose();
            return bitmap;
        }
    }
}
