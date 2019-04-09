using Eto;
using Eto.Drawing;
using Eto.Forms;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using System;
using System.Threading;
using System.Threading.Tasks;
using GameboyEmulator.Core.Emulation;
using OpenTK.Input;

namespace GameboyEmulator.UI
{
    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            Style.Add<TextBox>("data-field", box => box.Font = new Font("monospace", 10));
            Style.Add<RichTextArea>("data-field", area => area.Font = new Font("monospace", 10));
            
            var emulator = new EmulationEngine { Running = true };
            Task.Run(() => emulator.Run());

            var emuWindow = new OpenGLEmulationWindow();

            emulator.FrameSource.NewFrame += (s, a) => emuWindow.SetNextFrame(a.Frame);

            emuWindow.KeyUp += (s, a) =>
            {
                switch (a.Key)
                {
                    case Key.Up: emulator.Buttons.Up = false; break;
                    case Key.Down: emulator.Buttons.Down = false; break;
                    case Key.Left: emulator.Buttons.Left = false; break;
                    case Key.Right: emulator.Buttons.Right = false; break;
                    case Key.X: emulator.Buttons.A = false; break;
                    case Key.C: emulator.Buttons.B = false; break;
                    case Key.V: emulator.Buttons.Start = false; break;
                    case Key.B: emulator.Buttons.Select = false; break;
                }
            };

            emuWindow.KeyDown += (s, a) =>
            {
                switch (a.Key)
                {
                    case Key.Up: emulator.Buttons.Up = true; break;
                    case Key.Down: emulator.Buttons.Down = true; break;
                    case Key.Left: emulator.Buttons.Left = true; break;
                    case Key.Right: emulator.Buttons.Right = true; break;
                    case Key.X: emulator.Buttons.A = true; break;
                    case Key.C: emulator.Buttons.B = true; break;
                    case Key.V: emulator.Buttons.Start = true; break;
                    case Key.B: emulator.Buttons.Select = true; break;
                }
            };

            var app = new Application();
            app.Attach();
            var debugViewModel = new DebuggerViewModel(emulator.State, emulator);
            var debugWindow = new DebugWindow(debugViewModel);
            debugWindow.Show();

            var frameCount = 0;
            emulator.FrameSource.NewFrame += (s, a) => frameCount++;
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

            using (emuWindow)
            {
                emuWindow.Run(60);
            }
        }

        static void AudioTest()
        {
            var context = new AudioContext();

            var buffer = AL.GenBuffer();

            var freq = 50;
            var amplitude = 0.2f;
            var seconds = 2.0f;
            var sampleRate = 44100;
            var bufferSize = (int)(seconds * sampleRate);

            var rand = new Random();
            short randVal = 0;
            var samples = new short[bufferSize];
            for (int i = 0; i < bufferSize; i++)
            {
                if (i % freq == 0)
                    randVal = (short)rand.Next();

                samples[i] = (short)(amplitude * randVal);
            }

            AL.BufferData(buffer, ALFormat.Mono16, samples, sizeof(short) * bufferSize, sampleRate);

            var source = AL.GenSource();
            AL.Source(source, ALSourcei.Buffer, buffer);
            AL.Source(source, ALSourceb.Looping, true);
            AL.SourcePlay(source);

            var e = AL.GetError();

            Thread.Sleep(10000);

            AL.SourceStop(source);
            AL.DeleteBuffer(buffer);
            AL.DeleteSource(source);

            context.Dispose();
        }
    }
}
