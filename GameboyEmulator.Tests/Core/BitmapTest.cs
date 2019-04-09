using GameboyEmulator.Core.Video;
using NUnit.Framework;
using System.Runtime.InteropServices;

namespace GameboyEmulator.Tests.Core
{
    public class BitmapTest
    {
        private Bitmap CreateTestBitmap()
        {
            var testBitmap = new Bitmap(2, 2);
            testBitmap.Populate((x, y) => new Pixel((byte)x, (byte)y, 255));
            return testBitmap;
        }

        [Test]
        public void BitmapPinningIsStable()
        {
            var bitmap = CreateTestBitmap();
            byte[] buf = new byte[bitmap.Height * bitmap.Width * Marshal.SizeOf<Pixel>()];

            using (var pinned = bitmap.Pinned())
            {
                var ptr = pinned.Pointer;
                Marshal.Copy(ptr, buf, 0, buf.Length);
            }

            Assert.AreEqual(4 * 3, buf.Length);
            // Output should be packed pixels, row-oriented.
            var expected = new byte[] { 0, 0, 255, 1, 0, 255, 0, 1, 255, 1, 1, 255 };
            CollectionAssert.AreEqual(expected, buf);
        }
    }
}
