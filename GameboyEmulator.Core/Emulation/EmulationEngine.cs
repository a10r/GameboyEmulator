using System;
using System.IO;
using System.Threading;
using GameboyEmulator.Core.Debugger;
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
                    MemoryBlock.LoadFromFile("C:/Users/Andreas/Dropbox/DMG/DMG_ROM.bin"),
                    MemoryBlock.LoadFromFile("C:/Users/Andreas/Dropbox/DMG/Tetris.gb")
                    ),
                new MemorySink(),
                vram,
                new MemoryBlock(8192),
                oam,
                io);

            _logger = Console.Out;
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
            var cycles = Cpu.ExecuteNextInstruction(State);
            ElapsedCycles += cycles;

            for (var i = 0; i < cycles; i++)
            {
                _lcdController.Tick();
            }

            //_logger.WriteLine(_state.Registers.ToString());
        }

        private void LogInstruction()
        {
            var nextInstr = Disassembler.DisassembleInstruction(InstructionLookahead.Passive(State));

            _logger.WriteLine();
            _logger.WriteLine($"--- PC = 0x{State.Registers.PC.Value:X4}; {nextInstr.Text} ---");

        }

        public void Run()
        {
            runLoop:
            while (Running)
            {
                //if (State.Registers.PC.Value > 0x100)
                //{
                //    LogInstruction();
                //}

                try
                {
                    Step();
                }
                catch (Exception e)
                {
                    _logger.WriteLine(e.ToString());
                }
            }

            Console.WriteLine("step over");

            //State.Registers.PC.Value = 0; // reset

            while (!Running)
            {
                Thread.Sleep(1000);
                Console.WriteLine("paused");
            }

            goto runLoop;
        }
    }
}