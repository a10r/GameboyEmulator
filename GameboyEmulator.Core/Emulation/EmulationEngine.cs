﻿using System;
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
using GameboyEmulator.Core.Timer;
using System.Diagnostics;

namespace GameboyEmulator.Core.Emulation
{
    public class EmulationEngine : IEmulationControl
    {
        private readonly LcdController _lcdController;
        private readonly TimerController _timerController;
        private readonly TextWriter _logger;
        private readonly IMachineState _loggingState;

        public IButtonState Buttons { get; }

        public EmulationEngine(string bootromFile, string romFile)
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
            var timerInterrupt = new InterruptTrigger(new BoolPointer(@if, 2));
            var buttonPressInterrupt = new InterruptTrigger(new BoolPointer(@if, 4));
            
            _timerController = new TimerController(timerInterrupt);

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
            
            var wy = new Register<byte>();
            var wx = new Register<byte>();

            var bootromEnable = new Register<byte>();

            io.Add(0x00, p1);

            var serialLog = new StreamWriter("C:/Users/Andreas/Dropbox/DMG/serial_log.txt", true);
            var lastSerialByte = 0;

            io.Add(0x01, new LambdaRegister<byte>(b => lastSerialByte = b)); // SB serial transfer
            io.Add(0x02, new LambdaRegister<byte>(b => { if (b == 0x81) serialLog.Write(Convert.ToChar(lastSerialByte)); serialLog.Flush(); })); // SC serial clock

            io.Add(0x04, _timerController.DIV);
            io.Add(0x05, _timerController.TIMA);
            io.Add(0x06, _timerController.TMA);
            io.Add(0x07, _timerController.TAC);

            io.Add(0x40, lcdc);
            io.Add(0x41, stat);
            io.Add(0x42, scy);
            io.Add(0x43, scx);
            io.Add(0x44, lyLogger);
            io.Add(0x45, lyc);
            io.Add(0x47, bgp);
            io.Add(0x48, obp0);
            io.Add(0x49, obp1);
            io.Add(0x4A, wy);
            io.Add(0x4B, wx);
            io.Add(0x50, bootromEnable);

            io.Add(0x0F, @if);
            io.Add(0xFF, ie);

            var cartridge = CartridgeLoader.FromFile(romFile);

            var memoryMap = new TopLevelMemoryMap(
                new ShadowedMemoryBlock(
                    MemoryBlock.LoadFromFile(bootromFile),
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
                vram, oam, obp0, obp1, wy, wx, vblankInterrupt, lcdStatusInterrupt);

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

        // Synchronization variables.
        private int _targetClock = 4194304;
        private int _syncsPerSecond = 4;
        private Stopwatch _stopwatch = new Stopwatch();

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
                // STOP mode is exited when a button is pressed.
                // IF bit 4 is set when a button is pressed, so we can use that.
                if (State.Memory[0xFF0F].GetBit(4))
                {
                    State.Stopped = false;
                }
                else
                {
                    return;
                }
            }

            if (State.Halted)
            {
                _lcdController.Tick();
                _timerController.Tick();
                ElapsedCycles += 1;
            }
            else
            {
                var cycles = Cpu.ExecuteNextInstruction(State);
                ElapsedCycles += cycles;

                for (var i = 0; i < cycles; i++)
                {
                    _lcdController.Tick();
                    _timerController.Tick();
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
            }

            // HALT mode is always exited regardless of the state of the IME
            if (firedInterrupts != 0)
            {
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

                _stopwatch.Restart();
                var lastElapsed = ElapsedCycles;
                var clockCounter = 0;
                var syncTarget = _targetClock / _syncsPerSecond; // TODO possibly inaccurate if there is a remainder!
                var phaseTime = 1000 / _syncsPerSecond;

                try
                {
                    Step();

                    clockCounter += (int)(ElapsedCycles - lastElapsed);
                    if (clockCounter >= syncTarget)
                    {
                        var elapsedTime = _stopwatch.ElapsedMilliseconds;
                        if (elapsedTime <= phaseTime)
                        {
                            Thread.Sleep((int)(phaseTime - elapsedTime));
                        }
                        else
                        {
                            _logger.WriteLine("Slowdown!");
                        }
                        clockCounter -= syncTarget;
                        _stopwatch.Restart();
                    }
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