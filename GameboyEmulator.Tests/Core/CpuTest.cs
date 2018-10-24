using GameboyEmulator.Core.Debugger;
using GameboyEmulator.Core.Emulation;
using GameboyEmulator.Core.Processor;
using GameboyEmulator.Core.Utils;
using GameboyEmulator.Tests.Util;
using NUnit.Framework;

namespace GameboyEmulator.Tests.Core
{
    [TestFixture]
    public class CpuTest
    {
        private const ushort INITIAL_SP = 0xCFFF;
        private const ushort INITIAL_PC = 0x0100;

        private IMachineState _machine; 

        [SetUp]
        public void SetupMachine()
        {
            _machine = new CpuTestMachine();
            _machine.Registers.SP.Value = INITIAL_SP;
            _machine.Registers.PC.Value = INITIAL_PC;
        }

        public void LoadProgram(int baseOffset, params byte[] instructions)
        {
            for (int i = 0; i < instructions.Length; i++)
                _machine.Memory[baseOffset + i] = instructions[i];
        }

        public void Run(int instructionCount)
        {
            for (int i = 0; i < instructionCount; i++)
            {
                var addr = _machine.Registers.PC.Value;
                var instr = Disassembler.DisassembleInstruction(InstructionLookahead.Passive(_machine)).Text;
                TestContext.Out.WriteLine($"#{i} 0x{addr:X4}: {instr}");

                Cpu.ExecuteNextInstruction(_machine);
            }
        }

        [Test]
        public void Call()
        {
            ushort returnAddress = INITIAL_PC + 3;
            LoadProgram(INITIAL_PC, 0xCD, 0x12, 0x34); // CALL 0x3412
            Run(1);
            Assert.AreEqual(_machine.Registers.PC.Value, 0x3412);
            Assert.AreEqual(_machine.Registers.SP.Value, INITIAL_SP - 2);
            Assert.AreEqual(_machine.Memory[INITIAL_SP - 1], returnAddress.GetHigh());
            Assert.AreEqual(_machine.Memory[INITIAL_SP - 2], returnAddress.GetLow());
        }

        [TestCase(0xC4, false, false)] // NZ
        [TestCase(0xCC, true, false)] // Z
        [TestCase(0xD4, false, false)] // NC
        [TestCase(0xDC, false, true)] // C
        public void ConditionalCall_Pass(byte opcode, bool zeroFlag, bool carryFlag)
        {
            _machine.Registers.Flags.Zero = zeroFlag;
            _machine.Registers.Flags.Carry = carryFlag;

            ushort returnAddress = INITIAL_PC + 3;
            LoadProgram(INITIAL_PC, 
                opcode, 0x12, 0x34); // CALL ??, 0x3412
            Run(1);
            Assert.AreEqual(_machine.Registers.PC.Value, 0x3412);
            Assert.AreEqual(_machine.Registers.SP.Value, INITIAL_SP - 2);
            Assert.AreEqual(_machine.Memory[INITIAL_SP - 1], returnAddress.GetHigh());
            Assert.AreEqual(_machine.Memory[INITIAL_SP - 2], returnAddress.GetLow());
        }

        [TestCase(0xC4, true, false)] // NZ
        [TestCase(0xCC, false, false)] // Z
        [TestCase(0xD4, false, true)] // NC
        [TestCase(0xDC, false, false)] // C
        public void ConditionalCall_Fail(byte opcode, bool zeroFlag, bool carryFlag)
        {
            _machine.Registers.Flags.Zero = zeroFlag;
            _machine.Registers.Flags.Carry = carryFlag;

            ushort returnAddress = INITIAL_PC + 3;
            LoadProgram(INITIAL_PC,
                opcode, 0x12, 0x34); // CALL ??, 0x3412
            Run(1);
            Assert.AreEqual(_machine.Registers.PC.Value, returnAddress);
            Assert.AreEqual(_machine.Registers.SP.Value, INITIAL_SP);
        }

        [Test]
        public void CallAndRet()
        {
            ushort returnAddress = INITIAL_PC + 3;
            LoadProgram(INITIAL_PC, 0xCD, 0x00, 0x10); // CALL 0x1000
            LoadProgram(0x1000, 0xC9); // RET
            Run(2);
            Assert.AreEqual(_machine.Registers.PC.Value, returnAddress);
            Assert.AreEqual(_machine.Registers.SP.Value, INITIAL_SP);
        }

        [Test]
        public void JumpHL()
        {
            _machine.Registers.HL.Value = 0x1000;
            LoadProgram(INITIAL_PC, 0xe9); // JP (HL)
            Run(1);
            Assert.AreEqual(_machine.Registers.PC.Value, 0x1000);
        }
    }
}
