using GameboyEmulator.Core.Memory;

namespace GameboyEmulator.Core.Emulation
{
    internal class MachineState : IMachineState
    {
        public IRegisterField Registers { get; }
        public IMemoryBlock Memory { get; }
        public IStack Stack { get; }

        public MachineState(IRegisterField registers, IMemoryBlock memory)
        {
            Registers = registers;
            Memory = memory;
            Stack = new Stack(memory, registers.SP);
        }
    }
}
