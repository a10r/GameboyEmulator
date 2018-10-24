using System;
using System.IO;
using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;
using GameboyEmulator.Core.Emulation;

namespace GameboyEmulator.UI
{
    public class MainWindow : Form
    {
        public MainWindow()
        {
            var emulator = new EmulationEngine { Running = true };
            Task.Run(() => emulator.Run());

            Task.Run(async () =>
            {
                long last = emulator.ElapsedCycles;
                while (true)
                {
                    await Task.Delay(1000);
                    var elapsed = emulator.ElapsedCycles - last;
                    if (emulator.Running)
                    {
                        Console.WriteLine($"{(float)elapsed / 1000000} MHz");
                    }
                    last = emulator.ElapsedCycles;
                }
            });

            var imageView = new ImageView();

            emulator.FrameSource.NewFrame += (sender, args) =>
            {
                Application.Instance.AsyncInvoke(() =>
                {
                    var stream = new MemoryStream();
                    //args.Frame.Save(stream);
                    var bitmap = new Bitmap(new Size(160, 144), PixelFormat.Format24bppRgb);
                    var bitmapData = bitmap.Lock();

                    for (int x = 0; x < 160; x++)
                    {
                        for (int y = 0; y < 144; y++)
                        {
                            var pixel = args.Frame.Data[x, y];
                            bitmapData.SetPixel(x, y, Color.FromArgb(pixel.R, pixel.G, pixel.B, 255));
                        }
                    }

                    bitmapData.Dispose();

                    imageView.Image = bitmap;
                });
            };

            Content = new Panel
            {
                Content = imageView,
                Size = new Size(160, 144)
            };

            Resizable = false;

            new DebugWindow(new DebuggerViewModel(emulator.State, emulator)).Show();
        }
    }
}
