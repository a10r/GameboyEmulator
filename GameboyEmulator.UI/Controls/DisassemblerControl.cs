using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Forms;
using Eto.Drawing;
using GameboyEmulator.Core.Emulation;
using GameboyEmulator.Core.Processor;
using GameboyEmulator.Core.Debug;

namespace GameboyEmulator.UI.Controls
{
    public class DisassemblerControl : Panel
    {
        private IMachineState _state;
        private RichTextArea _textArea;

        public DisassemblerControl(IMachineState state)
        {
            // TODO: maybe I don't need the ENTIRE state
            _state = state;

            _textArea = new RichTextArea
            {
                //BackgroundColor = Color.FromRgb(0),
                //TextColor = Color.FromGrayscale(1, 1),
                Font = new Font("Monospace", 10),
                Width = 300,
                ReadOnly = true,
            };

            //var stream = new MemoryStream();
            //var sw = new StreamWriter(stream);
            //sw.WriteLine("{\\rtf1 Hi {\\b Hi } \\line \\par asdf }");
            //sw.Flush();

            var stream = DisassembleAndFormat(100);

            _textArea.Buffer.Load(stream, RichTextAreaFormat.Rtf);

            Content = _textArea;
        }

        private Stream DisassembleAndFormat(int instructionCount)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write("{\\rtf1 ");

            var currentAddress = _state.Registers.PC.Value;
            for (var i = 0; i < instructionCount; i++)
            {
                var lookahead = _state.Memory.InstructionAt(currentAddress);
                var instruction = Disassembler.DisassembleInstruction(lookahead);
                writer.Write($"{{\\b 0x{currentAddress:X4}:}} {instruction.Text}");
                writer.Write(" \\line ");
                currentAddress += (ushort)instruction.Length;
            }

            writer.Write("}");
            writer.Flush();

            return stream;
        }
    }
}
