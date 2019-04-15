using GameboyEmulator.Core.Memory;
using GameboyEmulator.Core.Processor;
using GameboyEmulator.Core.Utils;

namespace GameboyEmulator.Core.Timer
{
    public class TimerController
    {
        /// <summary>
        /// Divider register, increments at a fixed rate of 16,384 Hz (once every 256 clocks).
        /// </summary>
        public IRegister<byte> DIV { get; }

        /// <summary>
        /// Counter register. Holds the current counter value, once it overflows, an 
        /// interrupt is generated.
        /// </summary>
        public IRegister<byte> TIMA { get; }

        /// <summary>
        /// Timer modulo register. Its value is loaded into TIMA once it overflows.
        /// </summary>
        public IRegister<byte> TMA { get; }

        /// <summary>
        /// Timer controller register. Bits 0 and 1 select the clock divider. 
        /// Bit 2 starts and stops the timer. Bits 3-7 are unused.
        /// </summary>
        public IRegister<byte> TAC { get; }

        private readonly IInterruptTrigger _timerInterrupt;

        private byte _rawTacValue; // for caching
        private bool _timerEnabled; // TAC bit 2
        private int _timerDivider; // selected by TAC bits 0 and 1

        private byte _div;
        private int _divCounter;

        private byte _timer;
        private int _timerCounter;
        
        public TimerController(IInterruptTrigger interrupt)
        {
            _timerInterrupt = interrupt;
            DIV = new LambdaRegister<byte>(_ => _div = 0, () => _div);
            // TODO what happens on TIMA write?
            TIMA = new LambdaRegister<byte>(_ => { }, () => _timer);
            TMA = new Register<byte>();
            TAC = new LambdaRegister<byte>(WriteTAC, ReadTAC);
        }

        private void WriteTAC(byte value)
        {
            _rawTacValue = value;
            _timerEnabled = value.GetBit(2);

            var div = value & 0b11;
            if (div == 0) _timerDivider = 1024;
            else if (div == 1) _timerDivider = 16;
            else if (div == 2) _timerDivider = 64;
            else if (div == 3) _timerDivider = 256;
        }

        private byte ReadTAC()
        {
            return _rawTacValue;
        }

        // Expected to be called at the rate of the base clock of 4,194,304 Hz.
        public void Tick()
        {
            // DIV counter
            _divCounter++;
            if (_divCounter == 256)
            {
                _divCounter = 0;
                _div++;
            }

            // TIMA counter
            if (!_timerEnabled)
            {
                return;
            }

            _timerCounter++;
            if (_timerCounter == _timerDivider)
            {
                _timerCounter = 0;
                _timer++;

                // overflow happened
                if (_timer == 0)
                {
                    _timerInterrupt.Trigger();
                    _timer = TMA.Value;
                }
            }
        }
    }
}
