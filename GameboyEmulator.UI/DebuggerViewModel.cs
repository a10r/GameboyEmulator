using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using EtoBitmap = Eto.Drawing.Bitmap;
using GameboyEmulator.Core.Debugger;
using GameboyEmulator.Core.Emulation;
using GameboyEmulator.Core.Memory;
using GameboyEmulator.Core.Processor;
using GameboyEmulator.UI.Util;
using GameboyEmulator.Core.Video;

namespace GameboyEmulator.UI
{
    public class DebuggerViewModel : INotifyPropertyChanged
    {
        private readonly IEmulationControl _emulationControl;
        private string _disassembedProgramText;
        private bool _emulationIsRunning;
        private EtoBitmap _tileset;
        private EtoBitmap _tilemap1;
        private EtoBitmap _tilemap2;

        public DebuggerViewModel(IMachineState state, IEmulationControl emulationControl)
        {
            State = state;
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

        public EtoBitmap Tileset
        {
            get => _tileset;
            set
            {
                _tileset = value;
                OnPropertyChanged();
            }
        }

        public EtoBitmap Tilemap0
        {
            get => _tilemap1;
            set
            {
                _tilemap1 = value;
                OnPropertyChanged();
            }
        }

        public EtoBitmap Tilemap1
        {
            get => _tilemap2;
            set
            {
                _tilemap2 = value;
                OnPropertyChanged();
            }
        }

        public IMachineState State { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        
        private string DisassembleAndFormat(int instructionCount)
        {
            var sb = new StringBuilder();
            sb.Append("{\\rtf1 ");

            var currentAddress = State.Registers.PC.Value;
            for (var i = 0; i < instructionCount; i++)
            {
                var lookahead = State.Memory.InstructionAt(currentAddress);
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
            DisassembedProgramText = DisassembleAndFormat(50);
            EmulationIsRunning = _emulationControl.Running;
            
            Tileset = LcdDebugUtils.RenderTileset(new MemoryPointer(State.Memory, 0x8000)).ToEtoBitmap();
            // TODO can be cleaned up maybe
            var tilesetSelect = new BoolPointer(new MemoryPointer(State.Memory, 0xFF40), 4);
            var scy = State.Memory[0xFF42];
            var scx = State.Memory[0xFF43];
            Tilemap0 = LcdDebugUtils.RenderTilemap(new MemoryPointer(State.Memory, 0x8000), new Register<bool>(false), tilesetSelect)
                .DrawRectangle(scx, scy, 160, 144).ToEtoBitmap();
            Tilemap1 = LcdDebugUtils.RenderTilemap(new MemoryPointer(State.Memory, 0x8000), new Register<bool>(true), tilesetSelect)
                .DrawRectangle(scx, scy, 160, 144).ToEtoBitmap();

            OnPropertyChanged("State");
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}