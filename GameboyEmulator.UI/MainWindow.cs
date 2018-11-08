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

            // Debug window
            var debugViewModel = new DebuggerViewModel(emulator.State, emulator);
            var debugWindow = new DebugWindow(debugViewModel);
            debugWindow.Show();

            var frameCount = 0;

            Task.Run(async () =>
            {
                long last = emulator.ElapsedCycles;
                while (true)
                {
                    await Task.Delay(1000);
                    var elapsed = emulator.ElapsedCycles - last;
                    if (emulator.Running)
                    {
                        var newTitle = $"Debugger ({(float)elapsed / 1000000} MHz, {frameCount} fps)";
                        Application.Instance.AsyncInvoke(() => 
                        {
                            debugWindow.Title = newTitle;
                        });
                    }
                    last = emulator.ElapsedCycles;
                    frameCount = 0;
                }
            });

            var imageView = new ImageView();

            emulator.FrameSource.NewFrame += (sender, args) =>
            {
                frameCount++;
                Application.Instance.AsyncInvoke(() =>
                {
                    imageView.Image = args.Frame.ToEtoBitmap();
                    
                    debugViewModel.Refresh();
                });
            };

            Content = new Panel
            {
                Content = imageView,
                Size = new Size(160, 144)
            };

            Resizable = false;
        }
    }
}
