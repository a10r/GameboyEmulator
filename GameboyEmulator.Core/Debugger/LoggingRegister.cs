using GameboyEmulator.Core.Memory;
using System;
using System.IO;

namespace GameboyEmulator.Core.Debugger
{
    public class LoggingRegister<T> : IRegister<T>
    {
        private readonly IRegister<T> _baseRegister;
        private readonly string _registerName;
        private readonly TextWriter _logger;
        private readonly bool _logReads;
        private readonly bool _logWrites;

        public LoggingRegister(IRegister<T> baseRegister, string registerName, TextWriter logger, bool logReads = true, bool logWrites = true)
        {
            _baseRegister = baseRegister;
            _registerName = registerName;
            _logger = logger;
            _logReads = logReads;
            _logWrites = logWrites;
        }

        public T Value
        {
            get
            {
                if (_logReads)
                {
                    _logger.WriteLine($"<- {_registerName} ~ 0x{_baseRegister.Value:X2} / {_baseRegister.Value}");
                }
                return _baseRegister.Value;
            }
            set
            {
                if (_logWrites)
                {
                    _logger.WriteLine($"{_registerName} <- 0x{value:X2} / {value.ToBinaryString()} / {value}; (old value: {_baseRegister.Value:X2} / {value.ToBinaryString()} / {_baseRegister.Value})");
                }
                _baseRegister.Value = value;
            }
        }

        public override string ToString() => _baseRegister.ToString();
    }
}
