using System.IO;
using GameboyEmulator.Core.Memory;

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
    }
}
