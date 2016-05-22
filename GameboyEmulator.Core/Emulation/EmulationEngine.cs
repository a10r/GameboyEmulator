using System.IO;
using System.Threading;
using GameboyEmulator.Core.Debug;
using GameboyEmulator.Core.Memory;
using GameboyEmulator.Core.Processor;
using GameboyEmulator.Core.Video;

namespace GameboyEmulator.Core.Emulation
{
    public class EmulationEngine : IEmulationControl
    {
        private readonly LcdController _lcdController;
        private readonly TextWriter _logger;
        private readonly IMachineState _loggingState;

        public EmulationEngine()
        {
            var vram = new MemoryBlock(8192);
            var oam = new MemoryBlock(120);
            var io = new AddressableRegisterField(256);

            // TODO: replace this with the real registers
            for (var i = 0; i < io.Size; i++)
            {
                io.Add(i, new Register<byte>());
            }

            var lcdc = new LcdControlRegister();
            var stat = new LcdStatusRegister();
            var scy = new Register<byte>();
            var scx = new Register<byte>();
            var ly = new Register<byte>();

            io.Add(0x40, lcdc);
            io.Add(0x41, stat);
            io.Add(0x42, scy);
            io.Add(0x43, scx);
            io.Add(0x44, ly);

            var memoryMap = new TopLevelMemoryMap(
                new ShadowedMemoryBlock(
                    MemoryBlock.LoadFromFile("C:/Users/Andreas/Dropbox/DMG_ROM.bin"),
                    MemoryBlock.LoadFromFile("C:/Users/Andreas/Dropbox/Tetris.gb")
                    ),
                new MemorySink(),
                vram,
                new MemoryBlock(8192),
                oam,
                io);

            _logger = TextWriter.Null;
            State = new MachineState(new RegisterField(), memoryMap);
            _loggingState = new MachineState(State.Registers,
                new LoggingMemoryBlock(State.Memory, _logger));
            _lcdController = new LcdController(lcdc, stat, scx, scy, ly,
                vram,
                oam);
        }

        public IMachineState State { get; }

        public IFrameSource FrameSource => _lcdController;

        public long ElapsedCycles { get; private set; }

        public bool Running { get; set; }

        public void Step()
        {
            //var nextInstr = Disassembler.DisassembleInstruction(InstructionLookahead.Passive(_state));

            //_logger.WriteLine();
            //_logger.WriteLine($"--- PC = 0x{_state.Registers.PC.Value:X4}; {nextInstr.Text} ---");

            var cycles = Cpu.ExecuteNextInstruction(State);
            ElapsedCycles += cycles;

            for (var i = 0; i < cycles; i++)
            {
                _lcdController.Tick();
            }

            //_logger.WriteLine(_state.Registers.ToString());
        }

        public void Run()
        {
            runLoop:
            while (Running && State.Registers.PC.Value < 0x100)
            {
                Step();
            }

            while (!Running)
            {
                Thread.Sleep(1000);
            }

            goto runLoop;
        }
    }
}