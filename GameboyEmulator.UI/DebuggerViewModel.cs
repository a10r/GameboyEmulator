using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using GameboyEmulator.Core.Debug;
using GameboyEmulator.Core.Emulation;
using GameboyEmulator.Core.Processor;

namespace GameboyEmulator.UI
{
    public class DebuggerViewModel : INotifyPropertyChanged
    {
        private readonly IMachineState _state;
        private readonly IEmulationControl _emulationControl;
        private string _disassembedProgramText;
        private bool _emulationIsRunning;

        public DebuggerViewModel(IMachineState state, IEmulationControl emulationControl)
        {
            _state = state;
            _emulationControl = emulationControl;
            Refresh();
        }

        public bool EmulationIsRunning
        {
            get { return _emulationIsRunning; }
            set
            {
                _emulationIsRunning = value;
                OnPropertyChanged();
            }
        }

        public string DisassembedProgramText
        {
            get { return _disassembedProgramText; }
            set
            {
                _disassembedProgramText = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        private string DisassembleAndFormat(int instructionCount)
        {
            var sb = new StringBuilder();
            sb.Append("{\\rtf1 ");

            var currentAddress = _state.Registers.PC.Value;
            for (var i = 0; i < instructionCount; i++)
            {
                var lookahead = _state.Memory.InstructionAt(currentAddress);
                var instruction = Disassembler.DisassembleInstruction(lookahead);
                sb.Append($"{{\\b 0x{currentAddress:X4}:}} {instruction.Text}");
                sb.Append(" \\line ");
                currentAddress += (ushort) instruction.Length;
            }

            sb.Append("}");

            return sb.ToString();
        }

        public void StartEmulation()
        {
            _emulationControl.Running = true;
            Refresh();
        }

        public void HaltEmulation()
        {
            _emulationControl.Running = false;
            Refresh();
        }

        public void Step()
        {
            _emulationControl.Step();
            Refresh();
        }

        public void Refresh()
        {
            DisassembedProgramText = DisassembleAndFormat(10);
            EmulationIsRunning = _emulationControl.Running;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}