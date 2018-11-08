using GameboyEmulator.Core.IO;

namespace GameboyEmulator.Core.Emulation
{
    public interface IEmulationControl
    {
        long ElapsedCycles { get; }
        bool Running { get; set; }
        void Step();

        IButtonState Buttons { get; }
    }
}
