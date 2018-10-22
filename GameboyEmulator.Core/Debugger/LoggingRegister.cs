using GameboyEmulator.Core.Memory;
using System.IO;

namespace GameboyEmulator.Core.Debugger
{
    public class LoggingRegister<T> : IRegister<T>
    {
        private readonly IRegister<T> _baseRegister;
        private readonly string _registerName;
        private readonly TextWriter _logger;

        public LoggingRegister(IRegister<T> baseRegister, string registerName, TextWriter logger)
        {
            _baseRegister = baseRegister;
            _registerName = registerName;
            _logger = logger;
        }

        public T Value
        {
            get
            {
                _logger.WriteLine($"<- {_registerName} ~ 0x{_baseRegister.Value:X2} / {_baseRegister.Value}");
                return _baseRegister.Value;
            }
            set
            {
                _logger.WriteLine($"{_registerName} <- 0x{value:X2} / {value}; (old value: {_baseRegister.Value:X2} / {_baseRegister.Value})");
                _baseRegister.Value = value;
            }
        }

        public override string ToString() => _baseRegister.ToString();
    }
}
