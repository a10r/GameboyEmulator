namespace GameboyEmulator.Core.Processor
{
    // TODO: HIGHLY TENTATIVE

    public abstract class InstructionPrimitives<TReturn>
    {
        public abstract TReturn NOP();
        public abstract TReturn LD_R8_R8(RegisterName target, RegisterName source);
        public abstract TReturn LD_R8_D8(RegisterName target, byte immediate);
        public abstract TReturn LD_R8_pHL(RegisterName target);
        public abstract TReturn LD_pHL_R8(RegisterName source);
        public abstract TReturn LD_pHL_D8(RegisterName source);
        //public abstract TReturn Ld_A_AddrBC();
        //public abstract TReturn Ld_A_AddrDE();
        //public abstract TReturn Ld_A_AddrC();
        //public abstract TReturn Ld_AddrC_A();
        //public abstract TReturn Ld_A_AddrImm();
        //public abstract TReturn Ld_AddImm_A();
        //public abstract TReturn Ld_A_Imm16();
    }
}
