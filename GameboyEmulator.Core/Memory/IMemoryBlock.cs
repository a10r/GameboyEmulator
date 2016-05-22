namespace GameboyEmulator.Core.Memory
{
    public interface IMemoryBlock
    {
        int Size { get; }
        byte this[int address] { get; set; }
    }
}
