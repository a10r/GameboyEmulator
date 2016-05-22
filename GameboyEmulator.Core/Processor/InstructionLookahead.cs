using System.Collections.Generic;
using GameboyEmulator.Core.Emulation;
using GameboyEmulator.Core.Memory;

namespace GameboyEmulator.Core.Processor
{
    public static class InstructionLookahead
    {
        public static IEnumerator<byte> Passive(IMachineState state)
        {
            var pc = state.Registers.PC.Value;
            while (true)
            {
                yield return state.Memory[pc++];
            }
        }

        public static IEnumerator<byte> InstructionAt(this IMemoryBlock memory, ushort address)
        {
            while (true)
            {
                yield return memory[address++];
            }
        }
    }
}
