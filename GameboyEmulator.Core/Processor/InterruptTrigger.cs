using GameboyEmulator.Core.Memory;

namespace GameboyEmulator.Core.Processor
{
    public class InterruptTrigger : IInterruptTrigger
    {
        private readonly IRegister<bool> _ifBit;

        public InterruptTrigger(IRegister<bool> ifBit)
        {
            _ifBit = ifBit;
        }

        public void Trigger()
        {
            _ifBit.Value = true;
        }
    }
}
