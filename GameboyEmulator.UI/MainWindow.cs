﻿using System;
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

            // Very basic key input handling
            KeyDown += (s, e) =>
            {
                switch (e.Key)
                {
                    case Keys.Up: emulator.Buttons.Up = true; break;
                    case Keys.Down: emulator.Buttons.Down = true; break;
                    case Keys.Left: emulator.Buttons.Left = true; break;
                    case Keys.Right: emulator.Buttons.Right = true; break;
                    case Keys.X: emulator.Buttons.A = true; break;
                    case Keys.C: emulator.Buttons.B = true; break;
                    case Keys.V: emulator.Buttons.Start = true; break;
                    case Keys.B: emulator.Buttons.Select = true; break;
                }
            };

            KeyUp += (s, e) =>
            {
                switch (e.Key)
                {
                    case Keys.Up: emulator.Buttons.Up = false; break;
                    case Keys.Down: emulator.Buttons.Down = false; break;
                    case Keys.Left: emulator.Buttons.Left = false; break;
                    case Keys.Right: emulator.Buttons.Right = false; break;
                    case Keys.X: emulator.Buttons.A = false; break;
                    case Keys.C: emulator.Buttons.B = false; break;
                    case Keys.V: emulator.Buttons.Start = false; break;
                    case Keys.B: emulator.Buttons.Select = false; break;
                }
            };

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
