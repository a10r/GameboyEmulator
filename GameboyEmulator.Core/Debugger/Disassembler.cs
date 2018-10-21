using System;
using System.Collections.Generic;
using GameboyEmulator.Core.Processor;
using GameboyEmulator.Core.Utils;

namespace GameboyEmulator.Core.Debugger
{
    public struct Instruction
    {
        public string Text { get; private set; }
        public int Length { get; private set; }

        public Instruction(string text, int length)
        {
            Text = text;
            Length = length;
        }
    }

    public static class Disassembler
    {
        // TODO: Case statement is the same as in Cpu. Unify them somehow to make it
        // less ugly.
        public static Instruction DisassembleInstruction(IEnumerator<byte> lookahead)
        {
            var count = 0;
            Func<byte> next = () =>
            {
                lookahead.MoveNext();
                count++;
                return lookahead.Current;
            };

            var opcode = next();


            switch (opcode)
            {
                #region NOP

                case 0x00:
                    return new Instruction("NOP", count);

                #endregion

                #region LD reg, reg'

                case 0x40:
                case 0x41:
                case 0x42:
                case 0x43:
                case 0x44:
                case 0x45:
                case 0x47:

                case 0x48:
                case 0x49:
                case 0x4A:
                case 0x4B:
                case 0x4C:
                case 0x4D:
                case 0x4F:

                case 0x50:
                case 0x51:
                case 0x52:
                case 0x53:
                case 0x54:
                case 0x55:
                case 0x57:

                case 0x58:
                case 0x59:
                case 0x5A:
                case 0x5B:
                case 0x5C:
                case 0x5D:
                case 0x5F:

                case 0x60:
                case 0x61:
                case 0x62:
                case 0x63:
                case 0x64:
                case 0x65:
                case 0x67:

                case 0x68:
                case 0x69:
                case 0x6A:
                case 0x6B:
                case 0x6C:
                case 0x6D:
                case 0x6F:

                case 0x7F:
                case 0x78:
                case 0x79:
                case 0x7A:
                case 0x7B:
                case 0x7C:
                case 0x7D:
                    {
                        var regTarget = Enum.GetName(typeof(RegisterName), (opcode & 0x38) >> 3);
                        var regSource = Enum.GetName(typeof(RegisterName), opcode & 0x07);

                        return new Instruction($"LD {regTarget}, {regSource}", count);
                    }

                #endregion

                #region LD reg, imm

                case 0x06:
                case 0x0E:
                case 0x16:
                case 0x1E:
                case 0x26:
                case 0x2E:
                case 0x3E:
                    {
                        var regTarget = Enum.GetName(typeof(RegisterName), (opcode & 0x38) >> 3);
                        return new Instruction($"LD {regTarget}, 0x{next():X2}", count);
                    }

                #endregion

                #region LD reg, (HL)

                case 0x7E:
                case 0x46:
                case 0x4E:
                case 0x56:
                case 0x5E:
                case 0x66:
                case 0x6E:
                    {
                        var regTarget = Enum.GetName(typeof(RegisterName), (opcode & 0x38) >> 3);
                        return new Instruction($"LD {regTarget}, (HL)", count);
                    }

                #endregion

                #region LD (HL), reg

                case 0x70:
                case 0x71:
                case 0x72:
                case 0x73:
                case 0x74:
                case 0x75:
                case 0x77:
                    {
                        var regSource = Enum.GetName(typeof(RegisterName), opcode & 0x07);
                        return new Instruction($"LD (HL), {regSource}", count);
                    }

                #endregion

                #region LD (HL), imm

                case 0x36:
                    {
                        return new Instruction($"LD (HL), 0x{next():X2}", count);
                    }

                #endregion

                #region LD A, (BC)

                case 0x0A:
                    {
                        return new Instruction("LD A, (BC)", count);
                    }

                #endregion

                #region LD A, (DE)

                case 0x1A:
                    {
                        return new Instruction("LD A, (DE)", count);
                    }

                #endregion

                #region LD A, (C)

                case 0xF2:
                    {
                        return new Instruction("LD A, (0xFF00 + C)", count);
                    }

                #endregion

                #region LD (C), A

                case 0xE2:
                    {
                        return new Instruction("LD (0xFF00 + C), A", count);
                    }

                #endregion

                #region LD A, (n)

                case 0xF0:
                    {
                        return new Instruction($"LD A, (0xFF{next():X2})", count);
                    }

                #endregion

                #region LD (n), A

                case 0xE0:
                    {
                        return new Instruction($"LD (0xFF{next():X2}), A", count);
                    }

                #endregion

                #region LD A, (nn)

                case 0xFA:
                    {
                        return new Instruction($"LD A, (0x{BitUtils.Combine(next(), next()):X4})", count);
                    }

                #endregion

                #region LD (nn), A

                case 0xEA:
                    {
                        return new Instruction($"LD (0x{BitUtils.Combine(next(), next()):X4}), A", count);
                    }

                #endregion

                #region LD A, (HL+)

                case 0x2A:
                    {
                        return new Instruction("LD A, (HL+)", count);
                    }

                #endregion

                #region LD A, (HL-)

                case 0x3A:
                    {
                        return new Instruction("LD A, (HL-)", count);
                    }

                #endregion

                #region LD (BC), A

                case 0x02:
                    {
                        return new Instruction("LD (BC), A", count);
                    }

                #endregion

                #region LD (DE), A

                case 0x12:
                    {
                        return new Instruction("LD (DE), A", count);
                    }

                #endregion

                #region LD (HL+), A

                case 0x22:
                    {
                        return new Instruction("LD (HL+), A", count);
                    }

                #endregion

                #region LD (HL-), A

                case 0x32:
                    {
                        return new Instruction("LD (HL-), A", count);
                    }

                #endregion

                #region LD reg16, imm16

                case 0x01:
                case 0x11:
                case 0x21:
                case 0x31:
                    {
                        var regTarget = Enum.GetName(typeof(SpecialRegisterName), (opcode & 0x30) >> 4);
                        return new Instruction($"LD {regTarget}, 0x{BitUtils.Combine(next(), next()):X4}", count);
                    }

                #endregion

                #region LD SP, HL

                case 0xF9:
                    {
                        return new Instruction("LD SP, HL", count);
                    }

                #endregion

                #region PUSH qq

                case 0xF5:
                case 0xC5:
                case 0xD5:
                case 0xE5:
                    {
                        var regTarget = Enum.GetName(typeof(SpecialRegisterName), (opcode & 0x30) >> 4);
                        return new Instruction($"PUSH {regTarget}", count);
                    }

                #endregion

                #region POP qq

                case 0xF1:
                case 0xC1:
                case 0xD1:
                case 0xE1:
                    {
                        var regTarget = Enum.GetName(typeof(SpecialRegisterName), (opcode & 0x30) >> 4);
                        return new Instruction($"POP {regTarget}", count);
                    }

                #endregion

                #region LDHL SP, e

                case 0xF8:
                    {
                        var e = next();
                        //Instructions.Load(m.Registers.HL, (ushort)(m.Registers.SP.Value + e - 128));
                        return new Instruction("LDHL SP, e", count);
                    }

                #endregion

                #region LD (nn), SP

                case 0x08:
                    {
                        return new Instruction($"LD (0x{BitUtils.Combine(next(), next()):X4}), SP", count);
                    }

                #endregion

                #region ADD A, reg

                case 0x87:
                case 0x80:
                case 0x81:
                case 0x82:
                case 0x83:
                case 0x84:
                case 0x85:
                    {
                        var regSource = Enum.GetName(typeof(RegisterName), opcode & 0x7);
                        return new Instruction($"ADD A, {regSource}", count);
                    }

                #endregion

                #region ADD A, (HL)

                case 0x86:
                    {
                        return new Instruction("ADD A, (HL)", count);
                    }

                #endregion

                #region ADD A, imm

                case 0xC6:
                    {
                        return new Instruction($"ADD A, 0x{next():X2}", count);
                    }

                #endregion

                #region ADC A, reg

                case 0x8F:
                case 0x88:
                case 0x89:
                case 0x8A:
                case 0x8B:
                case 0x8C:
                case 0x8D:
                    {
                        var regSource = Enum.GetName(typeof(RegisterName), opcode & 0x7);
                        return new Instruction($"ADC A, {regSource}", count);
                    }

                #endregion

                #region ADC A, (HL)

                case 0x8E:
                    {
                        return new Instruction("ADC A, (HL)", count);
                    }

                #endregion

                #region ADC A, imm

                case 0xCE:
                    {
                        return new Instruction($"ADC A, 0x{next():X2}", count);
                    }

                #endregion

                #region SUB A, reg

                case 0x97:
                case 0x90:
                case 0x91:
                case 0x92:
                case 0x93:
                case 0x94:
                case 0x95:
                    {
                        var regSource = Enum.GetName(typeof(RegisterName), opcode & 0x7);
                        return new Instruction($"SUB A, {regSource}", count);
                    }

                #endregion

                #region SUB A, (HL)

                case 0x96:
                    {
                        return new Instruction("SUB A, (HL)", count);
                    }

                #endregion

                #region SUB A, imm

                case 0xD6:
                    {
                        var immediate = next();
                        return new Instruction($"SUB A, 0x{immediate:X2}", count);
                    }

                #endregion

                #region SBC A, reg

                case 0x9F:
                case 0x98:
                case 0x99:
                case 0x9A:
                case 0x9B:
                case 0x9C:
                case 0x9D:
                    {
                        var regSource = Enum.GetName(typeof(RegisterName), opcode & 0x7);
                        return new Instruction($"SBC A, {regSource}", count);
                    }

                #endregion

                #region SBC A, (HL)

                case 0x9E:
                    {
                        return new Instruction("SBC A, (HL)", count);
                    }

                #endregion

                #region SBC A, imm

                case 0xDE:
                    {
                        var immediate = next();
                        return new Instruction($"SBC A, 0x{immediate:X2}", count);
                    }

                #endregion

                #region AND A, reg

                case 0xA7:
                case 0xA0:
                case 0xA1:
                case 0xA2:
                case 0xA3:
                case 0xA4:
                case 0xA5:
                    {
                        var regSource = Enum.GetName(typeof(RegisterName), opcode & 0x7);
                        return new Instruction($"AND A, {regSource}", count);
                    }

                #endregion

                #region AND A, (HL)

                case 0xA6:
                    {
                        return new Instruction("AND A, (HL)", count);
                    }

                #endregion

                #region AND A, imm

                case 0xE6:
                    {
                        var immediate = next();
                        return new Instruction($"AND A, 0x{immediate:X2}", count);
                    }

                #endregion

                #region OR A, reg

                case 0xB7:
                case 0xB0:
                case 0xB1:
                case 0xB2:
                case 0xB3:
                case 0xB4:
                case 0xB5:
                    {
                        var regSource = Enum.GetName(typeof(RegisterName), opcode & 0x7);
                        return new Instruction($"OR A, {regSource}", count);
                    }

                #endregion

                #region OR A, (HL)

                case 0xB6:
                    {
                        return new Instruction("OR A, (HL)", count);
                    }

                #endregion

                #region OR A, imm

                case 0xF6:
                    {
                        var immediate = next();
                        return new Instruction($"OR A, 0x{immediate:X2}", count);
                    }

                #endregion

                #region XOR A, reg

                case 0xAF:
                case 0xA8:
                case 0xA9:
                case 0xAA:
                case 0xAB:
                case 0xAC:
                case 0xAD:
                    {
                        var regSource = Enum.GetName(typeof(RegisterName), opcode & 0x7);
                        return new Instruction($"XOR A, {regSource}", count);
                    }

                #endregion

                #region XOR A, (HL)

                case 0xAE:
                    {
                        return new Instruction("XOR A, (HL)", count);
                    }

                #endregion

                #region XOR A, imm

                case 0xEE:
                    {
                        var immediate = next();
                        return new Instruction($"XOR A, 0x{immediate:X2}", count);
                    }

                #endregion

                #region CP A, reg

                case 0xBF:
                case 0xB8:
                case 0xB9:
                case 0xBA:
                case 0xBB:
                case 0xBC:
                case 0xBD:
                    {
                        var regSource = Enum.GetName(typeof(RegisterName), opcode & 0x7);
                        return new Instruction($"CP A, {regSource}", count);
                    }

                #endregion

                #region CP A, (HL)

                case 0xBE:
                    {
                        return new Instruction("CP A, (HL)", count);
                    }

                #endregion

                #region CP A, imm

                case 0xFE:
                    {
                        var immediate = next();
                        return new Instruction($"CP A, 0x{immediate:X2}", count);
                    }

                #endregion

                #region INC reg

                case 0x3C:
                case 0x04:
                case 0x0C:
                case 0x14:
                case 0x1C:
                case 0x24:
                case 0x2C:
                    {
                        var regSource = Enum.GetName(typeof(RegisterName), (opcode & 0x38) >> 3);
                        return new Instruction($"INC {regSource}", count);
                    }

                #endregion

                #region INC (HL)

                case 0x34:
                    {
                        return new Instruction("INC (HL)", count);
                    }

                #endregion

                #region DEC reg

                case 0x3D:
                case 0x05:
                case 0x0D:
                case 0x15:
                case 0x1D:
                case 0x25:
                case 0x2D:
                    {
                        var regSource = Enum.GetName(typeof(RegisterName), (opcode & 0x38) >> 3);
                        return new Instruction($"DEC {regSource}", count);
                    }

                #endregion

                #region DEC (HL)

                case 0x35:
                    {
                        return new Instruction("DEC (HL)", count);
                    }

                #endregion

                #region ADD HL, reg16

                case 0x09:
                case 0x19:
                case 0x29:
                case 0x39:
                    {
                        var regSource = Enum.GetName(typeof(PairedRegisterName), (opcode & 0x30) >> 4);
                        return new Instruction($"ADD HL, {regSource}", count);
                    }

                #endregion

                // TODO

                #region ADD SP, e

                case 0xE8:
                    {
                        // ...
                        var e = next();
                        return new Instruction("ADD SP, e", count);
                    }

                #endregion

                #region INC reg16

                case 0x03:
                case 0x13:
                case 0x23:
                case 0x33:
                    {
                        var regSource = Enum.GetName(typeof(PairedRegisterName), (opcode & 0x30) >> 4);
                        return new Instruction($"INC {regSource}", count);
                    }

                #endregion

                #region DEC reg16

                case 0x0B:
                case 0x1B:
                case 0x2B:
                case 0x3B:
                    {
                        var regSource = Enum.GetName(typeof(PairedRegisterName), (opcode & 0x30) >> 4);
                        return new Instruction($"DEC {regSource}", count);
                    }

                #endregion

                #region RLCA

                case 0x07:
                    {
                        return new Instruction("RLCA", count);
                    }

                #endregion

                #region RLA

                case 0x17:
                    {
                        return new Instruction("RLA", count);
                    }

                #endregion

                #region RRCA

                case 0x0F:
                    {
                        return new Instruction("RRCA", count);
                    }

                #endregion

                #region RRA

                case 0x1F:
                    {
                        return new Instruction("RRA", count);
                    }

                #endregion

                case 0xCB:
                    {
                        return DisassembleCBInstruction(lookahead);
                    }

                #region JP imm16

                case 0xC3:
                    {
                        var immediate = BitUtils.Combine(next(), next());
                        return new Instruction($"JP 0x{immediate:X4}", count);
                    }

                #endregion

                #region JP cc, imm16

                case 0xC2:
                case 0xCA:
                case 0xD2:
                case 0xDA:
                    {
                        var immediate = BitUtils.Combine(next(), next());
                        var condition = Enum.GetName(typeof(JumpCondition), (opcode & 0x18) >> 3);
                        return new Instruction($"JP {condition}, {immediate}", count);
                    }

                #endregion

                // TODO: what is e-2??

                #region JR e

                case 0x18:
                    {
                        var e = next();
                        return new Instruction("JR e", count);
                    }

                #endregion

                // TODO: e

                #region JR cc, e

                case 0x20:
                case 0x28:
                case 0x30:
                case 0x38:
                    {
                        var condition = Enum.GetName(typeof(JumpCondition), (opcode & 0x18) >> 3);
                        var offset = (sbyte)next();
                        return new Instruction($"JR {condition}, {offset}", count);
                    }

                #endregion

                #region JP (HL)

                case 0xE9:
                    {
                        return new Instruction("JP (HL)", count);
                    }

                #endregion

                #region CALL imm16

                case 0xCD:
                    {
                        var immediate = BitUtils.Combine(next(), next());
                        return new Instruction($"CALL 0x{immediate:X4}", count);
                    }

                #endregion

                #region CALL cc, imm16

                case 0xC4:
                case 0xCC:
                case 0xD4:
                case 0xDC:
                    {
                        var immediate = BitUtils.Combine(next(), next());
                        var condition = Enum.GetName(typeof(JumpCondition), (opcode & 0x18) >> 3);
                        return new Instruction($"CALL {condition}, {immediate}", count);
                    }

                #endregion

                #region RET

                case 0xC9:
                    {
                        return new Instruction("RET", count);
                    }

                #endregion
                    

                #region RETI

                case 0xD9:
                    {
                        return new Instruction("RETI", count);
                    }

                #endregion
                    

                #region RET cc

                case 0xC0:
                case 0xC8:
                case 0xD0:
                case 0xD8:
                    {
                        var condition = Enum.GetName(typeof(JumpCondition), (opcode & 0x18) >> 3);
                        return new Instruction($"RET {condition}", count);
                    }

                #endregion

                #region RST t

                case 0xC7:
                case 0xCF:
                case 0xD7:
                case 0xDF:
                case 0xE7:
                case 0xEF:
                case 0xF7:
                case 0xFF:
                    {
                        // TODO
                        var t = next();
                        return new Instruction("RST t", count);
                    }

                #endregion

                #region DAA

                case 0x27:
                    {
                        return new Instruction("DAA", count);
                    }

                #endregion

                #region CPL

                case 0x2F:
                    {
                        return new Instruction("CPL", count);
                    }

                #endregion

                #region CCF

                case 0x3F:
                    {
                        return new Instruction("CCF", count);
                    }

                #endregion

                #region SCF

                case 0x37:
                    {
                        return new Instruction("SCF", count);
                    }

                #endregion

                #region DI

                case 0xF3:
                    {
                        return new Instruction("DI", count);
                    }

                #endregion

                #region EI

                case 0xFB:
                    {
                        return new Instruction("EI", count);
                    }

                #endregion

                #region HALT

                case 0x76:
                    {
                        return new Instruction("HALT", count);
                    }

                #endregion

                #region STOP

                case 0x10:
                    {
                        return new Instruction("STOP", count);
                    }

                #endregion

                default:
                    throw new InvalidOperationException("Invalid opcode.");
            }
        }

        private static Instruction DisassembleCBInstruction(IEnumerator<byte> lookahead)
        {
            var count = 1;
            Func<byte> next = () =>
            {
                lookahead.MoveNext();
                count++;
                return lookahead.Current;
            };

            var opcode = next();

            switch (opcode)
            {
                #region RLC reg

                case 0x07:
                case 0x00:
                case 0x01:
                case 0x02:
                case 0x03:
                case 0x04:
                case 0x05:
                    {
                        //var reg = m.Registers.GetRegisterById(opcode & 0x7);
                        return new Instruction("RLC reg", count);
                    }

                #endregion

                #region RLC (HL)

                case 0x06:
                    {
                        return new Instruction("RLC (HL)", count);
                    }

                #endregion

                #region RL reg

                case 0x17:
                case 0x10:
                case 0x11:
                case 0x12:
                case 0x13:
                case 0x14:
                case 0x15:
                    {
                        //var reg = m.Registers.GetRegisterById(opcode & 0x7);
                        return new Instruction("RL reg", count);
                    }

                #endregion

                #region RL (HL)

                case 0x16:
                    {
                        return new Instruction("RL (HL)", count);
                    }

                #endregion

                #region RRC reg

                case 0x0F:
                case 0x08:
                case 0x09:
                case 0x0A:
                case 0x0B:
                case 0x0C:
                case 0x0D:
                    {
                        //var reg = m.Registers.GetRegisterById(opcode & 0x7);
                        return new Instruction("RRC reg", count);
                    }

                #endregion

                #region RRC (HL)

                case 0x0E:
                    {
                        return new Instruction("RRC (HL)", count);
                    }

                #endregion

                #region RR reg

                case 0x1F:
                case 0x18:
                case 0x19:
                case 0x1A:
                case 0x1B:
                case 0x1C:
                case 0x1D:
                    {
                        //var reg = m.Registers.GetRegisterById(opcode & 0x7);
                        return new Instruction("RR reg", count);
                    }

                #endregion

                #region RR (HL)

                case 0x1E:
                    {
                        return new Instruction("RR (HL)", count);
                    }

                #endregion

                #region SLA reg

                case 0x27:
                case 0x20:
                case 0x21:
                case 0x22:
                case 0x23:
                case 0x24:
                case 0x25:
                    {
                        //var reg = m.Registers.GetRegisterById(opcode & 0x7);
                        return new Instruction("SLA reg", count);
                    }

                #endregion

                #region SLA (HL)

                case 0x26:
                    {
                        return new Instruction("SLA (HL)", count);
                    }

                #endregion

                #region SRA reg

                case 0x2F:
                case 0x28:
                case 0x29:
                case 0x2A:
                case 0x2B:
                case 0x2C:
                case 0x2D:
                    {
                        //var reg = m.Registers.GetRegisterById(opcode & 0x7);
                        return new Instruction("SRA reg", count);
                    }

                #endregion

                #region SRA (HL)

                case 0x2E:
                    {
                        return new Instruction("SRA (HL)", count);
                    }

                #endregion

                #region SRL reg

                case 0x3F:
                case 0x38:
                case 0x39:
                case 0x3A:
                case 0x3B:
                case 0x3C:
                case 0x3D:
                    {
                        //var reg = m.Registers.GetRegisterById(opcode & 0x7);
                        return new Instruction("SRL reg", count);
                    }

                #endregion

                #region SRL (HL)

                case 0x3E:
                    {
                        return new Instruction("SRL (HL)", count);
                    }

                #endregion

                #region SWAP reg

                case 0x37:
                case 0x30:
                case 0x31:
                case 0x32:
                case 0x33:
                case 0x34:
                case 0x35:
                    {
                        //var reg = m.Registers.GetRegisterById(opcode & 0x7);
                        return new Instruction("SWAP reg", count);
                    }

                #endregion

                #region SWAP (HL)

                case 0x36:
                    {
                        return new Instruction("SWAP (HL)", count);
                    }

                #endregion

                #region BIT b, reg

                case 0x47:
                case 0x40:
                case 0x41:
                case 0x42:
                case 0x43:
                case 0x44:
                case 0x45:
                case 0x57:
                case 0x50:
                case 0x51:
                case 0x52:
                case 0x53:
                case 0x54:
                case 0x55:
                case 0x67:
                case 0x60:
                case 0x61:
                case 0x62:
                case 0x63:
                case 0x64:
                case 0x65:
                case 0x77:
                case 0x70:
                case 0x71:
                case 0x72:
                case 0x73:
                case 0x74:
                case 0x75:
                case 0x4F:
                case 0x48:
                case 0x49:
                case 0x4A:
                case 0x4B:
                case 0x4C:
                case 0x4D:
                case 0x5F:
                case 0x58:
                case 0x59:
                case 0x5A:
                case 0x5B:
                case 0x5C:
                case 0x5D:
                case 0x6F:
                case 0x68:
                case 0x69:
                case 0x6A:
                case 0x6B:
                case 0x6C:
                case 0x6D:
                case 0x7F:
                case 0x78:
                case 0x79:
                case 0x7A:
                case 0x7B:
                case 0x7C:
                case 0x7D:
                    {
                        var index = (opcode & 0x38) >> 3;
                        //var reg = m.Registers.GetRegisterById(opcode & 0x7);
                        return new Instruction($"BIT {index}, reg", count);
                    }

                #endregion

                #region BIT b, (HL)

                case 0x46:
                case 0x56:
                case 0x66:
                case 0x76:
                case 0x4E:
                case 0x5E:
                case 0x6E:
                case 0x7E:
                    {
                        var index = (opcode & 0x38) >> 3;
                        return new Instruction($"BIT {index}, reg", count);
                    }

                #endregion

                #region SET b, reg

                case 0xC7:
                case 0xC0:
                case 0xC1:
                case 0xC2:
                case 0xC3:
                case 0xC4:
                case 0xC5:
                case 0xD7:
                case 0xD0:
                case 0xD1:
                case 0xD2:
                case 0xD3:
                case 0xD4:
                case 0xD5:
                case 0xE7:
                case 0xE0:
                case 0xE1:
                case 0xE2:
                case 0xE3:
                case 0xE4:
                case 0xE5:
                case 0xF7:
                case 0xF0:
                case 0xF1:
                case 0xF2:
                case 0xF3:
                case 0xF4:
                case 0xF5:
                case 0xCF:
                case 0xC8:
                case 0xC9:
                case 0xCA:
                case 0xCB:
                case 0xCC:
                case 0xCD:
                case 0xDF:
                case 0xD8:
                case 0xD9:
                case 0xDA:
                case 0xDB:
                case 0xDC:
                case 0xDD:
                case 0xEF:
                case 0xE8:
                case 0xE9:
                case 0xEA:
                case 0xEB:
                case 0xEC:
                case 0xED:
                case 0xFF:
                case 0xF8:
                case 0xF9:
                case 0xFA:
                case 0xFB:
                case 0xFC:
                case 0xFD:
                    {
                        var index = (opcode & 0x38) >> 3;
                        //var reg = m.Registers.GetRegisterById(opcode & 0x7);
                        return new Instruction($"SET {index}, reg", count);
                    }

                #endregion

                #region SET b, (HL)

                case 0xC6:
                case 0xCE:
                case 0xD6:
                case 0xDE:
                case 0xE6:
                case 0xEE:
                case 0xF6:
                case 0xFE:
                    {
                        var index = (opcode & 0x38) >> 3;
                        return new Instruction($"SET {index}, (HL)", count);
                    }

                #endregion

                #region RES b, reg

                case 0x87:
                case 0x80:
                case 0x81:
                case 0x82:
                case 0x83:
                case 0x84:
                case 0x85:
                case 0x97:
                case 0x90:
                case 0x91:
                case 0x92:
                case 0x93:
                case 0x94:
                case 0x95:
                case 0xA7:
                case 0xA0:
                case 0xA1:
                case 0xA2:
                case 0xA3:
                case 0xA4:
                case 0xA5:
                case 0xB7:
                case 0xB0:
                case 0xB1:
                case 0xB2:
                case 0xB3:
                case 0xB4:
                case 0xB5:
                case 0x8F:
                case 0x88:
                case 0x89:
                case 0x8A:
                case 0x8B:
                case 0x8C:
                case 0x8D:
                case 0x9F:
                case 0x98:
                case 0x99:
                case 0x9A:
                case 0x9B:
                case 0x9C:
                case 0x9D:
                case 0xAF:
                case 0xA8:
                case 0xA9:
                case 0xAA:
                case 0xAB:
                case 0xAC:
                case 0xAD:
                case 0xBF:
                case 0xB8:
                case 0xB9:
                case 0xBA:
                case 0xBB:
                case 0xBC:
                case 0xBD:
                    {
                        var index = (opcode & 0x38) >> 3;
                        //var reg = m.Registers.GetRegisterById(opcode & 0x7);
                        return new Instruction($"RES {index}, reg", count);
                    }

                #endregion

                #region RES b, (HL)

                case 0x86:
                case 0x8E:
                case 0x96:
                case 0x9E:
                case 0xA6:
                case 0xAE:
                case 0xB6:
                case 0xBE:
                    {
                        var index = (opcode & 0x38) >> 3;
                        return new Instruction("RLC reg", count);
                    }

                #endregion

                default:
                    throw new InvalidOperationException("Invalid opcode.");
            }
        }
    }
}