using System;
using System.IO;
using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;
using GameboyEmulator.Core.Emulation;
using GameboyEmulator.Core.Video;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

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
                    Console.WriteLine($"{(float)elapsed / 1000000} MHz");
                    last = emulator.ElapsedCycles;
                }
            });

            var imageView = new ImageView();

            emulator.FrameSource.NewFrame += (sender, args) =>
            {
                Application.Instance.AsyncInvoke(() =>
                {
                    var stream = new MemoryStream();
                    args.Frame.Save(stream, ImageFormat.Bmp);
                    imageView.Image = new Bitmap(stream);
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
