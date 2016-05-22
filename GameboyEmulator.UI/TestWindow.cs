using Eto.Drawing;
using Eto.Forms;
using GameboyEmulator.Core.Memory;
using GameboyEmulator.UI.Controls;

namespace GameboyEmulator.UI
{
    public class TestWindow : Form
    {
        public TestWindow()
        {
            var font = new Font("monospace", 10);

            Title = "GameboyEmulator Alpha";
            ClientSize = new Size(500, 800);
            Content = new TableLayout
            {
                Spacing = new Size(5, 5),
                Padding = new Padding(10, 10),
                Rows =
                {
                    new TableRow
                    {
                        Cells =
                        {
                            new Label { Text = "Something", VerticalAlignment = VerticalAlignment.Center },
                            new TextBox
                            {
                                Text = "0xFF21",
                                ToolTip = "0xFF21 huhuhuhuuhu",
                                Enabled = false,
                                Font = new Font("monospace", 10),
                            },
                        }
                    },
                    new NumericUpDown
                    {
                        MinValue = 0,
                        Increment = 0.25f,
                        DecimalPlaces = 2,
                    },
                    new Button
                    {
                        Text = "Step",
                        Command = new Command((o, a) => new OpenFileDialog().ShowDialog(this))
                    },
                    new DropDown
                    {
                        SelectedIndex = 0,
                        Items =
                        {
                            new ListItem { Text = "a" },
                            new ListItem { Text = "b" },
                        }
                    },
                    new ListBox
                    {
                        Items =
                        {
                            new ListItem { Text = "a" },
                            new ListItem { Text = "b" },
                        }
                    },
                    new Slider { SnapToTick = true },
                    new ProgressBar {Indeterminate = true},
                    new TabControl {Pages =
                        {
                            new TabPage { Text = "A", Content = new Label { Text = "Page A" } },
                            new TabPage { Text = "B", Content = new Label { Text = "Page B" } },
                        }
                    },
                    new RegisterFieldControl(new RegisterField()),
                    null
                }
            };
            Resizable = false;
            Menu = new MenuBar();
            Menu.Items.Add(new ButtonMenuItem {
                Text = "File",
                Items =
                {
                    new ButtonMenuItem { Text = "Open" }
                }
            });
        }
    }
}
