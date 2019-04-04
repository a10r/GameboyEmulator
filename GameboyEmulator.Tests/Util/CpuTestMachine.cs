using GameboyEmulator.Core.Emulation;
using GameboyEmulator.Core.Memory;

namespace GameboyEmulator.Tests.Util
{
    /// <summary>
    /// Simplified machine for testing CPU instructions only.
    /// </summary>
    public class CpuTestMachine : IMachineState
    {
        public IRegisterField Registers { get; set; }

        public IMemoryBlock Memory { get; set; }

        public IStack Stack { get; set; }

        public bool InterruptMasterEnable { get; set; }
        public bool Halted { get; set; }
        public bool Stopped { get; set; }

        public CpuTestMachine()
        {
            Registers = new RegisterField();
            Memory = new MemoryBlock(0x10000);
            Stack = new Stack(Memory, Registers.SP);
        }
    }
}
