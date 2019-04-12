using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using GameboyEmulator.Core.Debugger;
using GameboyEmulator.Core.IO;
using GameboyEmulator.Core.Memory;
using GameboyEmulator.Core.Processor;
using GameboyEmulator.Core.Utils;
using GameboyEmulator.Core.Video;
using GameboyEmulator.Core.Cartridge;

namespace GameboyEmulator.Core.Emulation
{
    public class EmulationEngine : IEmulationControl
    {
        private readonly LcdController _lcdController;
        private readonly TextWriter _logger;
        private readonly IMachineState _loggingState;

        public IButtonState Buttons { get; }

        public EmulationEngine()
        {
            _logger = Console.Out;

            var vram = new MemoryBlock(8192);
            var oam = new MemoryBlock(160);
            var io = new AddressableRegisterField(256);

            // TODO: replace this with the real registers
            for (var i = 0; i < io.Size; i++)
            {
                io.Add(i, new Register<byte>());
            }
            
            var @if = new Register<byte>();
            var ie = new Register<byte>();

            var vblankInterrupt = new InterruptTrigger(new BoolPointer(@if, 0));
            var lcdStatusInterrupt = new InterruptTrigger(new BoolPointer(@if, 1));
            var buttonPressInterrupt = new InterruptTrigger(new BoolPointer(@if, 4));

            var p1 = new ButtonInputRegister(buttonPressInterrupt);
            Buttons = p1;

            var lcdc = new LcdControlRegister();
            var lcdcLogger = new LoggingRegister<byte>(lcdc, "lcdc", _logger, logReads: false);
            var stat = new LcdStatusRegister();
            var statLogger = new LoggingRegister<byte>(stat, "stat", _logger, logReads: false);
            var scy = new Register<byte>();
            var scx = new Register<byte>();
            var ly = new Register<byte>();
            
            var lyLogger = new LoggingRegister<byte>(ly, "ly", _logger, logReads: false);
            var lyc = new Register<byte>();

            var bgp = new Register<byte>();
            var obp0 = new Register<byte>();
            var obp1 = new Register<byte>();

            var bootromEnable = new Register<byte>();

            io.Add(0x00, p1);

            var serialLog = new StreamWriter("C:/Users/Andreas/Dropbox/DMG/serial_log.txt", true);
            var lastSerialByte = 0;

            io.Add(0x01, new LambdaRegister<byte>(b => lastSerialByte = b)); // SB serial transfer
            io.Add(0x02, new LambdaRegister<byte>(b => { if (b == 0x81) serialLog.Write(Convert.ToChar(lastSerialByte)); serialLog.Flush(); })); // SC serial clock

            io.Add(0x40, lcdc);
            io.Add(0x41, stat);
            io.Add(0x42, scy);
            io.Add(0x43, scx);
            io.Add(0x44, lyLogger);
            io.Add(0x45, lyc);
            io.Add(0x47, bgp);
            io.Add(0x48, obp0);
            io.Add(0x49, obp1);
            io.Add(0x50, bootromEnable);

            io.Add(0x0F, @if);
            io.Add(0xFF, ie);
            
            //MemoryBlock.LoadFromFile("C:/Users/Andreas/Dropbox/DMG/ttt.gb"),
            //MemoryBlock.LoadFromFile("C:/Users/Andreas/Dropbox/DMG/DrMario.gb"),
            //MemoryBlock.LoadFromFile("C:/Users/Andreas/Dropbox/DMG/gb-test-roms/cpu_instrs/cpu_instrs.gb"),
            //MemoryBlock.LoadFromFile("C:/Users/Andreas/Dropbox/DMG/gb-test-roms/cpu_instrs/individual/02-interrupts.gb"),
            var cartridge = CartridgeLoader.FromFile("C:/Users/Andreas/Dropbox/DMG/PokemonRed.gb");

            var memoryMap = new TopLevelMemoryMap(
                new ShadowedMemoryBlock(
                    MemoryBlock.LoadFromFile("C:/Users/Andreas/Dropbox/DMG/DMG_ROM.bin"),
                    cartridge.ROM,
                    new BoolPointer(bootromEnable, 0)
                    ),
                cartridge.RAM,
                vram,
                new MemoryBlock(8192), // internal ram
                oam,
                io);

            io.Add(0x46, new OamDmaController(memoryMap));

            State = new MachineState(new RegisterField(), memoryMap);
            _loggingState = new MachineState(State.Registers,
                new LoggingMemoryBlock(State.Memory, _logger));
            _lcdController = new LcdController(lcdc, stat, scx, scy, ly, lyc, bgp,
                vram, oam, obp0, obp1, vblankInterrupt, lcdStatusInterrupt);

            //SkipBootrom();

            //_trace = new StreamWriter($"C:/Users/Andreas/Desktop/trace_{DateTime.Now.ToFileTime()}.txt");
        }

        // Debug
        public TextWriter _trace;
        private IDictionary<ushort, Instruction> _logUnique = new Dictionary<ushort, Instruction>();
        private IDictionary<ushort, int> _logHits = new Dictionary<ushort, int>();

        public IMachineState State { get; }

        public IFrameSource FrameSource => _lcdController;

        public long ElapsedCycles { get; private set; }

        public bool Running { get; set; }

        private long _c;


        private void DumpLog()
        {
            foreach (var item in _logUnique)
            {
                var bytes = string.Join(" ", Enumerable.Range(0, item.Value.Length).Select(i => State.Memory[item.Key + i].ToString("X2")));
                _logger.WriteLine($"0x{item.Key:X4}: {bytes,-10} {item.Value.Text} ({_logHits[item.Key]}x)");
            }
        }

        public void Step()
        {
            //if (State.Memory[0xFF50].GetBit(0)) // Bootrom disabled
            //{
            //    //var nextInstr = Disassembler.DisassembleInstruction(InstructionLookahead.Passive(State));
            //    //_log[State.Registers.PC.Value] = nextInstr;
            //    //if (!_logHits.ContainsKey(State.Registers.PC.Value)) _logHits[State.Registers.PC.Value] = 0;
            //    //_logHits[State.Registers.PC.Value]++;

            //    _trace.WriteLine(DebugUtils.Trace(State));

            //    if (State.Registers.PC.Value == 0x00F0)
            //    {
            //        _trace.Flush();
            //        Environment.Exit(0);
            //    }
            //}

            if (State.Stopped)
            {
                Console.WriteLine("Stopped!");
            }

            if (State.Halted)
            {
                _lcdController.Tick();
                ElapsedCycles += 1;
            }
            else
            {
                var cycles = Cpu.ExecuteNextInstruction(State);
                ElapsedCycles += cycles;

                for (var i = 0; i < cycles; i++)
                {
                    _lcdController.Tick();
                }
            }

            // Handle interrupts
            var firedInterrupts = (byte)(State.Memory[0xFF0F] & State.Memory[0xFFFF]);
            if (State.InterruptMasterEnable && firedInterrupts != 0)
            {
                // Save PC
                // TODO: push 16 bit values onto the stack in one call
                var pc = State.Registers.PC.Value;
                State.Stack.Push(pc.GetHigh());
                State.Stack.Push(pc.GetLow());

                State.InterruptMasterEnable = false;
                
                //_logger.WriteLine($"Servicing interrupt ... {State.Memory[0xFF0F].ToBinaryString()} {State.Memory[0xFFFF].ToBinaryString()} {firedInterrupts.ToBinaryString()} {ElapsedCycles - _c}");
                _c = ElapsedCycles;

                if (firedInterrupts.GetBit(0)) // VBlank
                {
                    State.Registers.PC.Value = 0x0040;
                    State.Memory[0xFF0F] = State.Memory[0xFF0F].SetBit(0, false);
                }
                else if (firedInterrupts.GetBit(1)) // LCD status
                {
                    State.Registers.PC.Value = 0x0048;
                    State.Memory[0xFF0F] = State.Memory[0xFF0F].SetBit(1, false);
                }
                else if (firedInterrupts.GetBit(2)) // Timer
                {
                    State.Registers.PC.Value = 0x0050;
                    State.Memory[0xFF0F] = State.Memory[0xFF0F].SetBit(2, false);
                }
                else if (firedInterrupts.GetBit(3)) // Serial link
                {
                    State.Registers.PC.Value = 0x0058;
                    State.Memory[0xFF0F] = State.Memory[0xFF0F].SetBit(3, false);
                }
                else if (firedInterrupts.GetBit(4)) // Keypad press
                {
                    State.Registers.PC.Value = 0x0060;
                    State.Memory[0xFF0F] = State.Memory[0xFF0F].SetBit(4, false);
                }

                State.Halted = false;
            }
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
                    var instr = Disassembler.DisassembleInstruction(InstructionLookahead.Passive(State));
                    Console.WriteLine($"Exception at instruction: 0x{State.Registers.PC.Value:X2} {instr.Text}");
                    _logger.WriteLine(e.ToString());
                }
            }

            //State.Registers.PC.Value = 0; // reset

            while (!Running)
            {
                Thread.Sleep(1000);
            }

            goto runLoop;
        }

        public void SkipBootrom()
        {
            // TODO
            State.Memory[0xFF40] = 0x91; // turn on lcd
            State.Memory[0xFF47] = 0xFC; // bg palette
            State.Memory[0xFF50] = 1; // turn off bootrom
            State.Registers.PC.Value = 0x100;
        }
    }
}