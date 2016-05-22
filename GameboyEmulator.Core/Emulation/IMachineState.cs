using GameboyEmulator.Core.Memory;

namespace GameboyEmulator.Core.Emulation
{
    public interface IMachineState
    {
        IRegisterField Registers { get; }
        IMemoryBlock Memory { get; }
        IStack Stack { get; }
    }
}
