namespace GameboyEmulator.Core.Memory
{
    public class Register<T> : IRegister<T>
    {
        public T Value { get; set; }

        public override string ToString() 
            => $"Reg<{typeof(T).Name}>({Value}; 0x{Value:X2})";
    }
}
