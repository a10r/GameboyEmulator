using System;
using System.Linq.Expressions;
using Eto.Drawing;
using Eto.Forms;
using GameboyEmulator.Core.Utils;

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
            layout.AddRow(FilePickerControls(viewModel, "Pick boot ROM:", vm => vm.BootromFile));

            // Row 2: ROM File Picker
            layout.AddRow(FilePickerControls(viewModel, "Pick ROM:", vm => vm.RomFile));

            // Row 3: Launch button
            var launchButton = new Button() { Text = "Launch" };
            launchButton.Click += (s, a) => OnLaunchClick(viewModel);

            layout.AddRow(launchButton);

            Content = layout;
            DataContext = viewModel;
        }

        private Control[] FilePickerControls(LaunchDialogViewModel viewModel, string label,
            Expression<Func<LaunchDialogViewModel, string>> propertyExpression)
        {
            var pickLabel = new Label() { Text = label, VerticalAlignment = VerticalAlignment.Center };
            var fileTextBox = new TextBox() { Width = 300 };
            fileTextBox.TextBinding.BindDataContext<LaunchDialogViewModel>(propertyExpression);
            var filePicker = new Button() { Text = "...", Width = 30 };
            filePicker.Click += (s, a) =>
            {
                var dialog = new OpenFileDialog();
                var result = dialog.ShowDialog(this);
                if (result == DialogResult.Ok)
                {
                    ReflectionUtils.SetProperty(viewModel, dialog.FileName, propertyExpression);
                }
            };

            return new Control[] { pickLabel, fileTextBox, filePicker };
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
