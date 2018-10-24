namespace GameboyEmulator.Core.Processor
{
    public enum RegisterName
    {
        A = 7,
        B = 0,
        C = 1,
        D = 2,
        E = 3,
        H = 4,
        L = 5
    }

    public enum PairedRegisterName
    {
        BC = 0,
        DE = 1,
        HL = 2,
        AF = 3
    }

    public enum SpecialRegisterName
    {
        BC = 0,
        DE = 1,
        HL = 2,
        SP = 3
    }
}