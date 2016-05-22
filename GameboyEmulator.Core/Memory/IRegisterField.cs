namespace GameboyEmulator.Core.Memory
{
    public interface IRegisterField
    {
        IFlags Flags { get; }
        IRegister<byte> A { get; }
        IRegister<byte> B { get; }
        IRegister<byte> C { get; }
        IRegister<byte> D { get; }
        IRegister<byte> E { get; }
        IRegister<byte> F { get; }
        IRegister<byte> H { get; }
        IRegister<byte> L { get; }
        IRegister<ushort> AF { get; }
        IRegister<ushort> BC { get; }
        IRegister<ushort> DE { get; }
        IRegister<ushort> HL { get; }
        IRegister<ushort> SP { get; }
        IRegister<ushort> PC { get; }
    }
}
