using System;
using System.IO;
using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;
using GameboyEmulator.Core.Emulation;
using GameboyEmulator.UI.Util;

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
                    imageView.Image = args.Frame.ToEtoBitmap();
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
