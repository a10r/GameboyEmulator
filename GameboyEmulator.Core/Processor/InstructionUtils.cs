using System;
using GameboyEmulator.Core.Memory;

namespace GameboyEmulator.Core.Processor
{
    public static class InstructionUtils
    {
        public static IRegister<byte> GetRegisterById(this IRegisterField regs, int id)
        {
            switch (id)
            {
                case 7:
                    return regs.A;
                case 0:
                    return regs.B;
                case 1:
                    return regs.C;
                case 2:
                    return regs.D;
                case 3:
                    return regs.E;
                case 4:
                    return regs.H;
                case 5:
                    return regs.L;
                default:
                    throw new ArgumentException("Invalid id.");
            }
        }

        public static IRegister<ushort> GetSpecialRegisterPairById(this IRegisterField regs, int id)
        {
            switch (id)
            {
                case 1:
                    return regs.DE;
                case 2:
                    return regs.HL;
                case 3:
                    return regs.SP;
                default:
                    throw new ArgumentException("Invalid id.");
            }
        }

        public static IRegister<ushort> GetGeneralRegisterPairById(this IRegisterField regs, int id)
        {
            switch (id)
            {
                case 0:
                    return regs.BC;
                case 1:
                    return regs.DE;
                case 2:
                    return regs.HL;
                case 3:
                    return regs.AF;
                default:
                    throw new ArgumentException("Invalid id.");
            }
        }
        
    }
}
