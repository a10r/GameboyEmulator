using System;
using System.IO;
using System.Linq;
using GameboyEmulator.Core.Emulation;
using GameboyEmulator.Core.Memory;
using GameboyEmulator.Core.Processor;

namespace GameboyEmulator.Core.Debugger
{
    public static class DebugUtils
    {
        public static void Dump(this IMemoryBlock memory, TextWriter output)
        {
            for (int i = 0; i < memory.Size; i++)
            {
                if (i % 16 == 0)
                {
                    output.Write($"0x{i:X4} : ");
                }

                output.Write($"{memory[i]:X2} ");

                if (i%16 != 15) continue;

                output.Write(": ");
                for (int j = i - 15; j <= i; j++)
                {
                    output.Write(memory[j].ToDisplayableChar());
                    output.Write(" ");
                }

                output.WriteLine();
            }

            output.Flush();
        }

        public static char ToDisplayableChar(this byte b)
        {
            return b > 0xA0 || (b > 0x1F && b < 0x80) ? (char)b : '.';
        }

        public static string Trace(IMachineState state)
        {
            var pc = state.Registers.PC.Value;
            var disassembledInstr = Disassembler.DisassembleInstruction(InstructionLookahead.Passive(state));
            var bytes = string.Join(" ", Enumerable.Range(0, disassembledInstr.Length).Select(i => state.Memory[pc + i].ToString("X2")));

            var hl = state.Memory[state.Registers.HL.Value];
            return $"0x{pc:X4}: {bytes,-10} {disassembledInstr.Text,-15} {state.Registers.ToString()}; (HL)={hl:X2}";
        }

        public static string ToBinaryString<T>(this T data)
        {
            switch (data)
            {
                case byte b:
                    return $"0b{Convert.ToString(b, 2).PadLeft(8, '0').Insert(4, "_")}";
                case ushort s:
                    return $"0b{Convert.ToString(s, 2).PadLeft(16, '0').Insert(4, "_").Insert(8, "_").Insert(12, "_")}";
                default:
                    return "0b????_????";
            }
        }
    }
}
