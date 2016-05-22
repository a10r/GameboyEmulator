namespace GameboyEmulator.Core.Memory
{
    public class Stack : IStack
    {
        private readonly IMemoryBlock _memory;
        private readonly IRegister<ushort> _stackPointer;

        public Stack(IMemoryBlock memory, IRegister<ushort> stackPointer)
        {
            _memory = memory;
            _stackPointer = stackPointer;
        }

        public byte Pop()
        {
            var val = _memory[_stackPointer.Value];
            _stackPointer.Value++;
            return val;
        }

        public void Push(byte value)
        {
            _stackPointer.Value--;
            _memory[_stackPointer.Value] = value;
        }
    }
}
