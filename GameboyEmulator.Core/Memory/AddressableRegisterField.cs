using System.Collections.Generic;

namespace GameboyEmulator.Core.Memory
{
    public class AddressableRegisterField : IMemoryBlock
    {
        private readonly IRegister<byte>[] _registers;

        public int Size { get; }

        public AddressableRegisterField(int size)
        {
            _registers = new IRegister<byte>[size];
            Size = size;
        }

        public byte this[int address]
        {
            get { return _registers[address].Value; }
            set { _registers[address].Value = value; }
        }

        public void Add(int address, IRegister<byte> register)
        {
            _registers[address] = register;
        }
    }
}
