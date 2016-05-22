using Eto;
using Eto.Drawing;
using Eto.Forms;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using System;
using System.Threading;
using System.Threading.Tasks;
using GameboyEmulator.Core.Emulation;

namespace GameboyEmulator.UI
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Style.Add<TextBox>("data-field", box => box.Font = new Font("monospace", 10));
            Style.Add<RichTextArea>("data-field", area => area.Font = new Font("monospace", 10));
            
            var app = new Application();
            app.Run(new MainWindow());
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
