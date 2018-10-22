using Eto.Drawing;
using Eto.Forms;
using GameboyEmulator.Core.Memory;
using GameboyEmulator.UI.Controls;

namespace GameboyEmulator.UI
{
    public class DebugWindow : Form
    {
        private readonly RegisterFieldControl _registerFieldControl;

        public DebugWindow(DebuggerViewModel viewModel)
        {
            Title = "Debugger";
            DataContext = viewModel;

            var layout = new DynamicLayout
            {
                DefaultSpacing = new Size(20, 10),
                Padding = new Padding(20, 10)
            };

            layout.BeginVertical();
            layout.BeginHorizontal();

            var disassemblerTextBox = new AutoLoadingRichTextArea
            {
                Font = new Font("monospace", 10),
                Width = 300,
                ReadOnly = true
            };

            disassemblerTextBox.TextBinding.BindDataContext<DebuggerViewModel>(m => m.DisassembedProgramText);
            disassemblerTextBox.BindDataContext(t => t.BackgroundColor,
                Binding.Property<bool>(nameof(DebuggerViewModel.EmulationIsRunning))
                    .Convert(m => m ? Colors.DarkGray : Colors.White));

            layout.Add(disassemblerTextBox);

            layout.BeginVertical();

            layout.BeginHorizontal();

            layout.BeginVertical();

            var runHaltButton = new Button();
            runHaltButton.TextBinding.BindDataContext(
                Binding.Property<bool>(nameof(DebuggerViewModel.EmulationIsRunning))
                    .Convert(b => b ? "Halt" : "Run"));
            runHaltButton.Command = new Command((s, a) =>
            {
                if (viewModel.EmulationIsRunning)
                {
                    viewModel.HaltEmulation();
                }
                else
                {
                    viewModel.StartEmulation();
                }
            });
            layout.Add(runHaltButton);

            var stepButton = new Button
            {
                Text = "Step",
                Command = new Command((s, a) => viewModel.Step())
            };
            stepButton.Bind(b => b.Enabled, viewModel,
                Binding.Property((DebuggerViewModel m) => m.EmulationIsRunning).Convert(v => !v));
            layout.Add(stepButton);

            layout.Add(null);

            layout.EndVertical();

            _registerFieldControl = new RegisterFieldControl(viewModel.State.Registers);
            layout.Add(_registerFieldControl);

            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "State")
                {
                    _registerFieldControl.UpdateBindings(BindingUpdateMode.Destination);
                }
            };

            layout.EndHorizontal();

            layout.EndBeginVertical();

            layout.Add(new TextArea
            {
                Text = "Log here ...",
                BackgroundColor = Color.FromRgb(0),
                TextColor = Color.FromGrayscale(1, 1),
                Font = new Font("monospace", 10),
                ReadOnly = true
            });

            layout.EndVertical();

            layout.EndHorizontal();

            layout.EndVertical();

            Content = layout;
            viewModel.Refresh();
        }
    }
}