using Eto.Drawing;
using Eto.Forms;
using GameboyEmulator.Core.Memory;
using GameboyEmulator.UI.Controls;

namespace GameboyEmulator.UI
{
    public class DebugWindow : Form
    {
        private readonly RegisterFieldControl _registerFieldControl;

        public DebugWindow(DebugWindowViewModel viewModel)
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

            disassemblerTextBox.TextBinding.BindDataContext<DebugWindowViewModel>(m => m.DisassembedProgramText);
            disassemblerTextBox.BindDataContext(t => t.BackgroundColor,
                Binding.Property<bool>(nameof(DebugWindowViewModel.EmulationIsRunning))
                    .Convert(m => m ? Colors.DarkGray : Colors.White));

            layout.Add(disassemblerTextBox);

            layout.BeginVertical();

            layout.BeginHorizontal();

            layout.BeginVertical();

            var runHaltButton = new Button();
            runHaltButton.TextBinding.BindDataContext(
                Binding.Property<bool>(nameof(DebugWindowViewModel.EmulationIsRunning))
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
                Binding.Property((DebugWindowViewModel m) => m.EmulationIsRunning).Convert(v => !v));
            layout.Add(stepButton);

            layout.Add(null);
            
            _registerFieldControl = new RegisterFieldControl(viewModel.State.Registers);
            layout.Add(_registerFieldControl);

            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "State")
                {
                    _registerFieldControl.UpdateBindings(BindingUpdateMode.Destination);
                }
            };

            layout.EndVertical();

            layout.EndBeginHorizontal();
            
            layout.EndBeginVertical();

            var tilesetView = new ImageView();
            tilesetView.BindDataContext(v => v.Image, (DebugWindowViewModel vm) => vm.Tileset, DualBindingMode.OneWay);
            layout.AddColumn(tilesetView, new Label { Text = "Tileset" });

            layout.EndBeginVertical();

            var tilemap0View = new ImageView();
            tilemap0View.BindDataContext(v => v.Image, (DebugWindowViewModel vm) => vm.Tilemap0, DualBindingMode.OneWay);
            layout.AddColumn(tilemap0View, new Label { Text = "Tilemap #0" });

            layout.EndBeginVertical();

            var tilemap1View = new ImageView();
            tilemap1View.BindDataContext(v => v.Image, (DebugWindowViewModel vm) => vm.Tilemap1, DualBindingMode.OneWay);
            layout.AddColumn(tilemap1View, new Label { Text = "Tilemap #1" });

            layout.EndVertical();

            layout.EndHorizontal();

            layout.EndVertical();

            Content = layout;
            viewModel.Refresh();
        }
    }
}