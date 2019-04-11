using GameboyEmulator.Core.Memory;

namespace GameboyEmulator.Core.Emulation
{
    internal class MachineState : IMachineState
    {

        public IRegisterField Registers { get; }
        public IMemoryBlock Memory { get; }
        public IStack Stack { get; }
        public bool InterruptMasterEnable { get; set; }
        
        private bool _halted;
        public bool Halted
        {
            get => _halted;
            set
            {
                // TODO debug outputs --> remove later.
                //if (value) System.Console.WriteLine("CPU halted.");
                //else System.Console.WriteLine("CPU un-halted.");
                _halted = value;
            }
        }
        public bool Stopped { get; set; }

        public MachineState(IRegisterField registers, IMemoryBlock memory)
        {
            Registers = registers;
            Memory = memory;
            Stack = new Stack(memory, registers.SP);
        }
    }
}
