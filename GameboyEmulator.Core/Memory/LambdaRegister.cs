using System;

namespace GameboyEmulator.Core.Memory
{
    public class LambdaRegister<T> : IRegister<T>
    {
        private readonly Func<T> _read;
        private readonly Action<T> _write;

        public T Value { get => _read(); set => _write(value); }

        public LambdaRegister(Action<T> write, Func<T> read)
        {
            _write = write;
            _read = read;
        }

        public LambdaRegister(Action<T> write)
            : this(write, () => default)
        { }
    }
}
