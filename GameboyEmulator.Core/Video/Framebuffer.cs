using System.IO;

namespace GameboyEmulator.Core.Video
{
    public struct Pixel
    {
        public byte R;
        public byte G;
        public byte B;
    }

    public class Framebuffer
    {
        public Pixel[,] Data { get; }

        public Framebuffer(int width, int height)
        {
            Data = new Pixel[width, height];
        }

        public void Save(Stream targetStream)
        {
            foreach (var pixel in Data)
            {
                targetStream.WriteByte(pixel.R);
                targetStream.WriteByte(pixel.G);
                targetStream.WriteByte(pixel.B);
            }
            targetStream.Flush();
        }
    }
}
