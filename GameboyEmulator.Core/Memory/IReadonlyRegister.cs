namespace GameboyEmulator.Core.Memory
{
    public interface IReadonlyRegister<T>
    {
        T Value { get; }
    }
}
