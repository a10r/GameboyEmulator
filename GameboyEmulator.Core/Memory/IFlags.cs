namespace GameboyEmulator.Core.Memory
{
    public interface IFlags
    {
        bool Zero { get; set; }
        bool Subtract { get; set; }
        bool HalfCarry { get; set; }
        bool Carry { get; set; }
    }
}