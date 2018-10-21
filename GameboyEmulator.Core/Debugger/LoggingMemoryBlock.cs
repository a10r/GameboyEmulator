using System.IO;
using GameboyEmulator.Core.Memory;

namespace GameboyEmulator.Core.Debugger
{
    public class LoggingMemoryBlock : IMemoryBlock
    {
        private readonly IMemoryBlock _sub;
        private readonly TextWriter _logger; 

        public byte this[int address]
        {
            get
            {
                _logger.WriteLine($"<- mem[0x{address:X4}] ~ 0x{_sub[address]:X2}");
                return _sub[address];
            }

            set
            {
                _logger.WriteLine($"mem[0x{address:X4}] <- 0x{value:X2}");
                _sub[address] = value;
            }
        }

        public int Size => _sub.Size;

        public LoggingMemoryBlock(IMemoryBlock memory, TextWriter logger)
        {
            _sub = memory;
            _logger = logger;
        }
    }
}
