namespace GameboyEmulator.Core.Memory
{
    public interface IWriteonlyRegister<T>
    {
        T Value { set; }
    }
}
