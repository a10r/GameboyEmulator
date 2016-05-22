using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;
using GameboyEmulator.Core.Emulation;

namespace GameboyEmulator.UI.Controls
{
    public class EmulationControl : TableLayout
    {
        private IEmulationControl _emulationControl;
        private readonly Button _runHaltButton;
        private Button _stepButton;

        public EmulationControl(IEmulationControl emulationControl)
        {
            _emulationControl = emulationControl;

            _runHaltButton = new Button
            {
                Text = emulationControl.Running ? "Halt" : "Run",
                Command = new Command((s, a) =>
                {
                    if (_emulationControl.Running)
                    {
                        _emulationControl.Running = false;
                        _stepButton.Enabled = true;
                        _runHaltButton.Text = "Run";
                    }
                    else
                    {
                        _emulationControl.Running = true;
                        _stepButton.Enabled = false;
                        _runHaltButton.Text = "Halt";
                    }
                })
            };

            _stepButton = new Button
            {
                Text = "Step",
                Enabled = !emulationControl.Running,
                Command = new Command((s, a) =>
                {
                    _emulationControl.Step();
                })
            };

            Rows.Add(_runHaltButton);
            Rows.Add(_stepButton);
            Rows.Add(null);
            
            Spacing = new Size(6, 4);
            Padding = new Padding(6, 4);
        }
    }
}
