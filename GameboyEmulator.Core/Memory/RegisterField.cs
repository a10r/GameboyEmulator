namespace GameboyEmulator.Core.Memory
{
    public class RegisterField : IRegisterField
    {
        private readonly FlagRegister _flags;
        public IRegister<byte> A { get; }
        public IRegister<byte> B { get; }
        public IRegister<byte> C { get; }
        public IRegister<byte> D { get; }
        public IRegister<byte> E { get; }
        public IRegister<byte> F => _flags;
        public IRegister<byte> H { get; }
        public IRegister<byte> L { get; }
        public IRegister<ushort> AF { get; }
        public IRegister<ushort> BC { get; }
        public IRegister<ushort> DE { get; }
        public IRegister<ushort> HL { get; }
        public IRegister<ushort> SP { get; }
        public IRegister<ushort> PC { get; }
        public IFlags Flags => _flags;
        // TODO: add IME here?

        public RegisterField()
        {
            _flags = new FlagRegister();
            A = new Register<byte>();
            B = new Register<byte>();
            C = new Register<byte>();
            D = new Register<byte>();
            E = new Register<byte>();
            H = new Register<byte>();
            L = new Register<byte>();
            AF = new PairedRegister(A, F);
            BC = new PairedRegister(B, C);
            DE = new PairedRegister(D, E);
            HL = new PairedRegister(H, L);
            SP = new Register<ushort>();
            PC = new Register<ushort>();
        }

        public override string ToString() 
            => $"A={A.Value:X2}; F={F.Value:X2}; B={B.Value:X2}; C={C.Value:X2}; D={D.Value:X2}; E={E.Value:X2}; H={H.Value:X2}; L={L.Value:X2}; PC={PC.Value:X4}; SP={SP.Value:X4}";
    }
}
