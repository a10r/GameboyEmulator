﻿using Eto.Drawing;
using Eto.Forms;
using GameboyEmulator.Core;
using GameboyEmulator.Core.Memory;

namespace GameboyEmulator.UI.Controls
{
    public class RegisterFieldControl : TableLayout
    {
        private IRegisterField _registerField;

        public RegisterFieldControl(IRegisterField registers)
        {
            _registerField = registers;

            var reg8Table = new TableLayout
            {
                Spacing = new Size(6, 4)
            };

            reg8Table.Rows.Add(new TableRow
            {
                Cells =
                {
                    RegisterLabel("A"),
                    RegisterBox8(_registerField.A),
                    RegisterBox8(_registerField.F),
                    RegisterLabel("F"),
                },
            });

            reg8Table.Rows.Add(new TableRow
            {
                Cells =
                {
                    RegisterLabel("B"),
                    RegisterBox8(_registerField.B),
                    RegisterBox8(_registerField.C),
                    RegisterLabel("C"),
                },
            });

            reg8Table.Rows.Add(new TableRow
            {
                Cells =
                {
                    RegisterLabel("D"),
                    RegisterBox8(_registerField.D),
                    RegisterBox8(_registerField.E),
                    RegisterLabel("E"),
                },
            });

            reg8Table.Rows.Add(new TableRow
            {
                Cells =
                {
                    RegisterLabel("H"),
                    RegisterBox8(_registerField.H),
                    RegisterBox8(_registerField.L),
                    RegisterLabel("L"),
                },
            });

            var reg16Table = new TableLayout
            {
                Spacing = new Size(6, 4)
            };

            reg16Table.Rows.Add(new TableRow
            {
                Cells =
                {
                    RegisterLabel("PC"),
                    RegisterBox16(_registerField.PC),
                    new TableCell { ScaleWidth = true },
                },
            });

            reg16Table.Rows.Add(new TableRow
            {
                Cells =
                {
                    RegisterLabel("SP"),
                    RegisterBox16(_registerField.SP),
                    new TableCell { ScaleWidth = true },
                },
            });

            Rows.Add(reg8Table);
            Rows.Add(reg16Table);
            Rows.Add(new TableRow { ScaleHeight = true });

            Spacing = new Size(6, 4);
            Padding = new Padding(6, 4);
        }

        private Control RegisterLabel(string name)
        {
            return new Label
            {
                Text = name,
                VerticalAlignment = VerticalAlignment.Center
            };
        }

        private Control RegisterBox8(IRegister<byte> reg)
        {
            var box = new TextBox
            {
                Enabled = false,
                Text = "00",
                MaxLength = 2,
                Width = 26,
                Style = "data-field",
            };

            box.TextBinding.Bind(() => reg.Value.ToString("X2"));

            return box;
        }

        private Control RegisterBox16(IRegister<ushort> reg)
        {
            var box = new TextBox
            {
                Enabled = false,
                Text = "FFFF",
                MaxLength = 4,
                Width = 46,
                Style = "data-field",
            };

            box.TextBinding.Bind(() => reg.Value.ToString("X4"));

            return box;
        }
    }
}
