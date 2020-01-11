using System;
using Eto.Drawing;
using Eto.Forms;

namespace GameboyEmulator.UI.Windows
{
    public class LaunchDialog : Dialog<LaunchDialogResult>
    {
        public LaunchDialog(LaunchDialogViewModel viewModel)
        {
            var layout = new DynamicLayout
            {
                DefaultSpacing = new Size(20, 10),
                Padding = new Padding(20, 10)
            };

            // Row 1: Boot ROM File Picker
            var bootromPickLabel = new Label() { Text = "Pick boot ROM:", VerticalAlignment = VerticalAlignment.Center };
            var bootromFileTextBox = new TextBox() { Width = 300 };
            bootromFileTextBox.TextBinding.BindDataContext<LaunchDialogViewModel>(v => v.BootromFile);
            var bootromFilePicker = new Button() { Text = "...", Width = 30 };
            bootromFilePicker.Click += (s, a) =>
            {
                var dialog = new OpenFileDialog();
                var result = dialog.ShowDialog(this);
                viewModel.BootromFile = dialog.FileName;
            };

            layout.AddRow(bootromPickLabel, bootromFileTextBox, bootromFilePicker);

            // Row 2: ROM File Picker
            var romPickLabel = new Label() { Text = "Pick ROM:", VerticalAlignment = VerticalAlignment.Center };
            var romFileTextBox = new TextBox() { Width = 300 };
            romFileTextBox.TextBinding.BindDataContext<LaunchDialogViewModel>(v => v.RomFile);
            var romFilePicker = new Button() { Text = "...", Width = 30 };
            romFilePicker.Click += (s, a) =>
            {
                var dialog = new OpenFileDialog();
                var result = dialog.ShowDialog(this);
                viewModel.RomFile = dialog.FileName;
            };

            layout.AddRow(romPickLabel, romFileTextBox, romFilePicker);

            // Row 3: Launch button
            var launchButton = new Button() { Text = "Launch" };
            launchButton.Click += (s, a) => OnLaunchClick(viewModel);

            layout.AddRow(launchButton);

            Content = layout;
            DataContext = viewModel;
        }

        private void OnLaunchClick(LaunchDialogViewModel viewModel)
        {
            Close(new LaunchDialogResult
            {
                BootromFile = viewModel.BootromFile,
                RomFile = viewModel.RomFile
            });
        }
    }
}
