namespace GameboyEmulator.Core.Memory
{
    public class Register<T> : IRegister<T>
    {
        public Register() { }

        public Register(T v)
        {
            Value = v;
        }

        public T Value { get; set; }

        public override string ToString() 
            => $"Reg<{typeof(T).Name}>({Value}; 0x{Value:X2})";
    }
}
