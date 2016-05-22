namespace GameboyEmulator.Core.Memory
{
    public interface IRegister<T> : IReadonlyRegister<T>, IWriteonlyRegister<T>
    {
        new T Value { get; set; }
    }
}
