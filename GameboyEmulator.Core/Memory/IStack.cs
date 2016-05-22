namespace GameboyEmulator.Core.Memory
{
    public interface IStack
    {
        void Push(byte value);
        byte Pop();
    }
}