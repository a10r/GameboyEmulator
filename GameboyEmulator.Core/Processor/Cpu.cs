using System;
using GameboyEmulator.Core.Emulation;
using GameboyEmulator.Core.Memory;
using GameboyEmulator.Core.Utils;

namespace GameboyEmulator.Core.Processor
{
    public static class Cpu
    {
        /// <param name="m">Machine state before execution.</param>
        /// <returns>Elapsed clock cycles.</returns>
        public static int ExecuteNextInstruction(IMachineState m)
        {
            var opcode = NextInstructionByte(m);

            switch (opcode)
            {
                #region NOP
                case 0x00:
                    return 4;
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
                        var regTarget = m.Registers.GetRegisterById((opcode & 0x38) >> 3);
                        var regSource = m.Registers.GetRegisterById(opcode & 0x07);
                        Instructions.Load(regTarget, regSource.Value);
                        return 4;
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
                        var reg = m.Registers.GetRegisterById((opcode & 0x38) >> 3);
                        Instructions.Load(reg, NextInstructionByte(m));
                        return 8;
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
                        var regTarget = m.Registers.GetRegisterById((opcode & 0x38) >> 3);
                        var value = m.Memory[m.Registers.HL.Value];
                        Instructions.Load(regTarget, value);
                        return 8;
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
                        var target = new MemoryLocation(m.Memory, m.Registers.HL.Value);
                        var regSource = m.Registers.GetRegisterById(opcode & 0x07);
                        Instructions.Load(target, regSource.Value);
                        return 8;
                    }
                #endregion

                #region LD (HL), imm
                case 0x36:
                    {
                        var target = new MemoryLocation(m.Memory, m.Registers.HL.Value);
                        Instructions.Load(target, NextInstructionByte(m));
                        return 12;
                    }
                #endregion

                #region LD A, (BC)
                case 0x0A:
                    {
                        var source = new MemoryLocation(m.Memory, m.Registers.BC.Value);
                        Instructions.Load(m.Registers.A, source.Value);
                        return 8;
                    }
                #endregion

                #region LD A, (DE)
                case 0x1A:
                    {
                        var source = new MemoryLocation(m.Memory, m.Registers.DE.Value);
                        Instructions.Load(m.Registers.A, source.Value);
                        return 8;
                    }
                #endregion

                #region LD A, (C)
                case 0xF2:
                    {
                        var source = new MemoryLocation(m.Memory, 0xFF00 + m.Registers.C.Value);
                        Instructions.Load(m.Registers.A, source.Value);
                        return 8;
                    }
                #endregion

                #region LD (C), A
                case 0xE2:
                    {
                        var target = new MemoryLocation(m.Memory, 0xFF00 + m.Registers.C.Value);
                        Instructions.Load(target, m.Registers.A.Value);
                        return 8;
                    }
                #endregion

                #region LD A, (n)
                case 0xF0:
                    {
                        var source = new MemoryLocation(m.Memory, 0xFF00 + NextInstructionByte(m));
                        Instructions.Load(m.Registers.A, source.Value);
                        return 12;
                    }
                #endregion

                #region LD (n), A
                case 0xE0:
                    {
                        var target = new MemoryLocation(m.Memory, 0xFF00 + NextInstructionByte(m));
                        Instructions.Load(target, m.Registers.A.Value);
                        return 12;
                    }
                #endregion

                #region LD A, (nn)
                case 0xFA:
                    {
                        var low = NextInstructionByte(m);
                        var high = NextInstructionByte(m);
                        var source = new MemoryLocation(m.Memory, BitUtils.Combine(low, high));
                        Instructions.Load(m.Registers.A, source.Value);
                        return 16;
                    }
                #endregion

                #region LD (nn), A
                case 0xEA:
                    {
                        var low = NextInstructionByte(m);
                        var high = NextInstructionByte(m);
                        var target = new MemoryLocation(m.Memory, BitUtils.Combine(low, high));
                        Instructions.Load(target, m.Registers.A.Value);
                        return 16;
                    }
                #endregion

                #region LD A, (HL+)
                case 0x2A:
                    {
                        var source = new MemoryLocation(m.Memory, m.Registers.HL.Value);
                        Instructions.Load(m.Registers.A, source.Value);
                        m.Registers.HL.Value++;
                        return 8;
                    }
                #endregion

                #region LD A, (HL-)
                case 0x3A:
                    {
                        var source = new MemoryLocation(m.Memory, m.Registers.HL.Value);
                        Instructions.Load(m.Registers.A, source.Value);
                        m.Registers.HL.Value--;
                        return 8;
                    }
                #endregion

                #region LD (BC), A
                case 0x02:
                    {
                        var target = new MemoryLocation(m.Memory, m.Registers.BC.Value);
                        Instructions.Load(target, m.Registers.A.Value);
                        return 8;
                    }
                #endregion

                #region LD (DE), A
                case 0x12:
                    {
                        var target = new MemoryLocation(m.Memory, m.Registers.DE.Value);
                        Instructions.Load(target, m.Registers.A.Value);
                        return 8;
                    }
                #endregion

                #region LD (HL+), A
                case 0x22:
                    {
                        var target = new MemoryLocation(m.Memory, m.Registers.HL.Value);
                        Instructions.Load(target, m.Registers.A.Value);
                        m.Registers.HL.Value++;
                        return 8;
                    }
                #endregion

                #region LD (HL-), A
                case 0x32:
                    {
                        var target = new MemoryLocation(m.Memory, m.Registers.HL.Value);
                        Instructions.Load(target, m.Registers.A.Value);
                        m.Registers.HL.Value--;
                        return 8;
                    }
                #endregion

                #region LD reg16, imm16
                case 0x01:
                case 0x11:
                case 0x21:
                case 0x31:
                    {
                        var low = NextInstructionByte(m);
                        var high = NextInstructionByte(m);
                        var target = m.Registers.GetSpecialRegisterPairById((opcode & 0x30) >> 4);
                        Instructions.Load(target, BitUtils.Combine(low, high));
                        return 12;
                    }
                #endregion

                #region LD SP, HL
                case 0xF9:
                    {
                        Instructions.Load(m.Registers.SP, m.Registers.HL.Value);
                        return 8;
                    }
                #endregion

                #region PUSH qq
                case 0xF5:
                case 0xC5:
                case 0xD5:
                case 0xE5:
                    {
                        var reg = m.Registers.GetGeneralRegisterPairById((opcode & 0x30) >> 4);
                        Instructions.Push(m.Stack, reg.Value);
                        return 16;
                    }
                #endregion

                #region POP qq
                case 0xF1:
                case 0xC1:
                case 0xD1:
                case 0xE1:
                    {
                        var reg = m.Registers.GetGeneralRegisterPairById((opcode & 0x30) >> 4);
                        Instructions.Pop(reg, m.Stack);
                        return 12;
                    }
                #endregion
                    
                #region LDHL SP, e
                case 0xF8:
                    {
                        var e = (sbyte)NextInstructionByte(m);
                        Instructions.LoadWithSignedOffset(m.Registers.HL, m.Registers.SP, e, m.Registers.Flags);
                        return 12;
                    }
                #endregion

                #region LD (nn), SP
                case 0x08:
                    {
                        var low = NextInstructionByte(m);
                        var high = NextInstructionByte(m);
                        var targetLow = new MemoryLocation(m.Memory, BitUtils.Combine(low, high));
                        var targetHigh = new MemoryLocation(m.Memory, BitUtils.Combine(low, high) + 1);
                        Instructions.Load(targetLow, m.Registers.SP.Value.GetLow());
                        Instructions.Load(targetHigh, m.Registers.SP.Value.GetHigh());
                        return 20;
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
                        var regSource = m.Registers.GetRegisterById(opcode & 0x7);
                        Instructions.Add(m.Registers.A, regSource.Value, m.Registers.Flags);
                        return 4;
                    }
                #endregion

                #region ADD A, (HL)
                case 0x86:
                    {
                        var source = new MemoryLocation(m.Memory, m.Registers.HL.Value);
                        Instructions.Add(m.Registers.A, source.Value, m.Registers.Flags);
                        return 8;
                    }
                #endregion

                #region ADD A, imm
                case 0xC6:
                    {
                        var imm = NextInstructionByte(m);
                        Instructions.Add(m.Registers.A, imm, m.Registers.Flags);
                        return 8;
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
                        var regSource = m.Registers.GetRegisterById(opcode & 0x7);
                        Instructions.AddPlusCarry(m.Registers.A, regSource.Value, m.Registers.Flags);
                        return 4;
                    }
                #endregion

                #region ADC A, (HL)
                case 0x8E:
                    {
                        var source = new MemoryLocation(m.Memory, m.Registers.HL.Value);
                        Instructions.AddPlusCarry(m.Registers.A, source.Value, m.Registers.Flags);
                        return 8;
                    }
                #endregion

                #region ADC A, imm
                case 0xCE:
                    {
                        var imm = NextInstructionByte(m);
                        Instructions.AddPlusCarry(m.Registers.A, imm, m.Registers.Flags);
                        return 8;
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
                        var regSource = m.Registers.GetRegisterById(opcode & 0x7);
                        Instructions.Subtract(m.Registers.A, regSource.Value, m.Registers.Flags);
                        return 4;
                    }
                #endregion

                #region SUB A, (HL)
                case 0x96:
                    {
                        var source = new MemoryLocation(m.Memory, m.Registers.HL.Value);
                        Instructions.Subtract(m.Registers.A, source.Value, m.Registers.Flags);
                        return 8;
                    }
                #endregion

                #region SUB A, imm
                case 0xD6:
                    {
                        var imm = NextInstructionByte(m);
                        Instructions.Subtract(m.Registers.A, imm, m.Registers.Flags);
                        return 8;
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
                        var regSource = m.Registers.GetRegisterById(opcode & 0x7);
                        Instructions.SubtractMinusCarry(m.Registers.A, regSource.Value, m.Registers.Flags);
                        return 4;
                    }
                #endregion

                #region SBC A, (HL)
                case 0x9E:
                    {
                        var source = new MemoryLocation(m.Memory, m.Registers.HL.Value);
                        Instructions.SubtractMinusCarry(m.Registers.A, source.Value, m.Registers.Flags);
                        return 8;
                    }
                #endregion

                #region SBC A, imm
                case 0xDE:
                    {
                        var imm = NextInstructionByte(m);
                        Instructions.SubtractMinusCarry(m.Registers.A, imm, m.Registers.Flags);
                        return 8;
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
                        var regSource = m.Registers.GetRegisterById(opcode & 0x7);
                        Instructions.And(m.Registers.A, regSource.Value, m.Registers.Flags);
                        return 4;
                    }
                #endregion

                #region AND A, (HL)
                case 0xA6:
                    {
                        var source = new MemoryLocation(m.Memory, m.Registers.HL.Value);
                        Instructions.And(m.Registers.A, source.Value, m.Registers.Flags);
                        return 8;
                    }
                #endregion

                #region AND A, imm
                case 0xE6:
                    {
                        var imm = NextInstructionByte(m);
                        Instructions.And(m.Registers.A, imm, m.Registers.Flags);
                        return 8;
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
                        var regSource = m.Registers.GetRegisterById(opcode & 0x7);
                        Instructions.Or(m.Registers.A, regSource.Value, m.Registers.Flags);
                        return 4;
                    }
                #endregion

                #region OR A, (HL)
                case 0xB6:
                    {
                        var source = new MemoryLocation(m.Memory, m.Registers.HL.Value);
                        Instructions.Or(m.Registers.A, source.Value, m.Registers.Flags);
                        return 8;
                    }
                #endregion

                #region OR A, imm
                case 0xF6:
                    {
                        var imm = NextInstructionByte(m);
                        Instructions.Or(m.Registers.A, imm, m.Registers.Flags);
                        return 8;
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
                        var regSource = m.Registers.GetRegisterById(opcode & 0x7);
                        Instructions.Xor(m.Registers.A, regSource.Value, m.Registers.Flags);
                        return 4;
                    }
                #endregion

                #region XOR A, (HL)
                case 0xAE:
                    {
                        var source = new MemoryLocation(m.Memory, m.Registers.HL.Value);
                        Instructions.Xor(m.Registers.A, source.Value, m.Registers.Flags);
                        return 8;
                    }
                #endregion

                #region XOR A, imm
                case 0xEE:
                    {
                        var imm = NextInstructionByte(m);
                        Instructions.Xor(m.Registers.A, imm, m.Registers.Flags);
                        return 8;
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
                        var regSource = m.Registers.GetRegisterById(opcode & 0x7);
                        Instructions.Compare(m.Registers.A, regSource.Value, m.Registers.Flags);
                        return 4;
                    }
                #endregion

                #region CP A, (HL)
                case 0xBE:
                    {
                        var source = new MemoryLocation(m.Memory, m.Registers.HL.Value);
                        Instructions.Compare(m.Registers.A, source.Value, m.Registers.Flags);
                        return 8;
                    }
                #endregion

                #region CP A, imm
                case 0xFE:
                    {

                        var imm = NextInstructionByte(m);
                        Instructions.Compare(m.Registers.A, imm, m.Registers.Flags);
                        return 8;
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
                        var reg = m.Registers.GetRegisterById((opcode & 0x38) >> 3);
                        Instructions.Increment(reg, m.Registers.Flags);
                        return 4;
                    }
                #endregion

                #region INC (HL)
                case 0x34:
                    {
                        var source = new MemoryLocation(m.Memory, m.Registers.HL.Value);
                        Instructions.Increment(source, m.Registers.Flags);
                        return 8;
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
                        var reg = m.Registers.GetRegisterById((opcode & 0x38) >> 3);
                        Instructions.Decrement(reg, m.Registers.Flags);
                        return 4;
                    }
                #endregion

                #region DEC (HL)
                case 0x35:
                    {
                        var source = new MemoryLocation(m.Memory, m.Registers.HL.Value);
                        Instructions.Decrement(source, m.Registers.Flags);
                        return 8;
                    }
                #endregion

                #region ADD HL, reg16
                case 0x09:
                case 0x19:
                case 0x29:
                case 0x39:
                    {
                        var regSource = m.Registers.GetSpecialRegisterPairById((opcode & 0x30) >> 4);
                        Instructions.Add(m.Registers.HL, regSource.Value, m.Registers.Flags);
                        return 8;
                    }
                #endregion
                    
                #region ADD SP, e
                case 0xE8:
                    {
                        var reg = m.Registers.SP;
                        var e = (sbyte)NextInstructionByte(m);
                        Instructions.LoadWithSignedOffset(reg, reg, e, m.Registers.Flags);
                        return 16;
                    }
                #endregion

                #region INC reg16
                case 0x03:
                case 0x13:
                case 0x23:
                case 0x33:
                    {
                        var regSource = m.Registers.GetSpecialRegisterPairById((opcode & 0x30) >> 4);
                        Instructions.Increment(regSource);
                        return 8;
                    }
                #endregion

                #region DEC reg16
                case 0x0B:
                case 0x1B:
                case 0x2B:
                case 0x3B:
                    {
                        var regSource = m.Registers.GetSpecialRegisterPairById((opcode & 0x30) >> 4);
                        Instructions.Decrement(regSource);
                        return 8;
                    }
                #endregion

                // TODO re-check if these pairs are flipped, looks weird
                #region RLCA
                case 0x07:
                    {
                        Instructions.RotateLeftA(m.Registers.A, m.Registers.Flags);
                        return 4;
                    }
                #endregion

                #region RLA
                case 0x17:
                    {
                        Instructions.RotateLeftThroughCarryA(m.Registers.A, m.Registers.Flags);
                        return 4;
                    }
                #endregion

                #region RRCA
                case 0x0F:
                    {
                        Instructions.RotateRightA(m.Registers.A, m.Registers.Flags);
                        return 4;
                    }
                #endregion

                #region RRA
                case 0x1F:
                    {
                        Instructions.RotateRightThroughCarryA(m.Registers.A, m.Registers.Flags);
                        return 4;
                    }
                #endregion

                case 0xCB:
                    {
                        return ExecuteCB(m);
                    }

                // TODO timing correct?
                #region JP imm16
                case 0xC3:
                    {
                        var low = NextInstructionByte(m);
                        var high = NextInstructionByte(m);
                        Instructions.Load(m.Registers.PC, BitUtils.Combine(low, high));
                        return 12;
                    }
                #endregion

                // TODO: timing correct?
                #region JP cc, imm16
                case 0xC2:
                case 0xCA:
                case 0xD2:
                case 0xDA:
                    {
                        var low = NextInstructionByte(m);
                        var high = NextInstructionByte(m);
                        var condition = (JumpCondition)((opcode & 0x18) >> 3);
                        Instructions.ConditionalJump(m.Registers.PC, BitUtils.Combine(low, high),
                            condition, m.Registers.Flags);
                        return 12;
                    }
                #endregion
                    
                // TODO re-check jump offset (should be correct though)
                // TODO timing
                #region JR e
                case 0x18:
                    {
                        var offset = (sbyte)NextInstructionByte(m);
                        Instructions.JumpByOffset(m.Registers.PC, offset);    
                        return 8;
                    }
                #endregion

                // TODO: timing?
                #region JR cc, e
                case 0x20:
                case 0x28:
                case 0x30:
                case 0x38:
                    {
                        var condition = (JumpCondition)((opcode & 0x18) >> 3);
                        var offset = (sbyte)NextInstructionByte(m);
                        Instructions.ConditionalJumpByOffset(m.Registers.PC, offset,
                            condition, m.Registers.Flags);
                        return 8;
                    }
                #endregion

                #region JP (HL)
                case 0xE9:
                    {
                        // This doesn't use the memory location pointed to by HL, but the value of HL itself!
                        Instructions.Load(m.Registers.PC, m.Registers.HL.Value);
                        return 4;
                    }
                #endregion

                #region CALL imm16
                case 0xCD:
                    {
                        var low = NextInstructionByte(m);
                        var high = NextInstructionByte(m);
                        Instructions.Call(m.Registers.PC, BitUtils.Combine(low, high), m.Stack);
                        return 24;
                    }
                #endregion

                // TODO: timing!
                #region CALL cc, imm16
                case 0xC4:
                case 0xCC:
                case 0xD4:
                case 0xDC:
                    {
                        var low = NextInstructionByte(m);
                        var high = NextInstructionByte(m);
                        var condition = (JumpCondition)((opcode & 0x18) >> 3);
                        Instructions.ConditionalCall(m.Registers.PC, BitUtils.Combine(low, high), m.Stack,
                            condition, m.Registers.Flags);
                        return 24;
                    }
                #endregion
               
                // TODO timing, manual says 16 cycles?
                #region RET
                case 0xC9:
                    {
                        Instructions.Return(m.Registers.PC, m.Stack);
                        return 8;
                    }
                #endregion

                // TODO timing, manual says 16 cycles?
                #region RETI
                case 0xD9:
                    {
                        Instructions.Return(m.Registers.PC, m.Stack);
                        m.InterruptMasterEnable = true; // "returned to its pre-interrupt status"?
                        return 8;
                    }
                #endregion

                // TODO: timing
                #region RET cc
                case 0xC0:
                case 0xC8:
                case 0xD0:
                case 0xD8:
                    {
                        var condition = (JumpCondition)((opcode & 0x18) >> 3);
                        Instructions.ConditionalReturn(m.Registers.PC, m.Stack, condition, m.Registers.Flags);
                        return 8;
                    }
                #endregion

                // TODO timing, manual says 16 cycles?
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
                        var restartOffset = (byte)((opcode & 0b0011_1000) >> 3);
                        Instructions.Restart(m.Registers.PC, restartOffset, m.Stack);
                        return 16;
                    }
                #endregion

                #region DAA
                case 0x27:
                    {
                        Instructions.DecimalAdjust(m.Registers.A, m.Registers.Flags);
                        return 4;
                    }
                #endregion

                #region CPL
                case 0x2F:
                    {
                        Instructions.Complement(m.Registers.A, m.Registers.Flags);
                        return 4;
                    }
                #endregion

                #region CCF
                case 0x3F:
                    {
                        Instructions.ComplementCarryFlag(m.Registers.Flags);
                        return 4;
                    }
                #endregion

                #region SCF
                case 0x37:
                    {
                        Instructions.SetCarryFlag(m.Registers.Flags);
                        return 4;
                    }
                #endregion

                #region DI
                case 0xF3:
                    {
                        m.InterruptMasterEnable = false;
                        return 4;
                    }
                #endregion

                #region EI
                case 0xFB:
                    {
                        m.InterruptMasterEnable = true;
                        return 4;
                    }
                #endregion
                    
                #region HALT
                case 0x76:
                    {
                        m.Halted = true;
                        return 4;
                    }
                #endregion

                #region STOP
                case 0x10:
                    {
                        m.Stopped = true;
                        return 4;
                    }
                #endregion

                default:
                    throw new InvalidOperationException("Invalid opcode.");
            }

        }

        private static int ExecuteCB(IMachineState m)
        {
            var opcode = NextInstructionByte(m);

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
                        var reg = m.Registers.GetRegisterById(opcode & 0x7);
                        Instructions.RotateLeft(reg, m.Registers.Flags);
                        return 8;
                    }
                #endregion

                #region RLC (HL)
                case 0x06:
                    {
                        var mem = new MemoryLocation(m.Memory, m.Registers.HL.Value);
                        Instructions.RotateLeft(mem, m.Registers.Flags);
                        return 16;
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
                        var reg = m.Registers.GetRegisterById(opcode & 0x7);
                        Instructions.RotateLeftThroughCarry(reg, m.Registers.Flags);
                        return 8;
                    }
                #endregion

                #region RL (HL)
                case 0x16:
                    {
                        var mem = new MemoryLocation(m.Memory, m.Registers.HL.Value);
                        Instructions.RotateLeftThroughCarry(mem, m.Registers.Flags);
                        return 16;
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
                        var reg = m.Registers.GetRegisterById(opcode & 0x7);
                        Instructions.RotateRight(reg, m.Registers.Flags);
                        return 8;
                    }
                #endregion

                #region RRC (HL)
                case 0x0E:
                    {
                        var mem = new MemoryLocation(m.Memory, m.Registers.HL.Value);
                        Instructions.RotateRight(mem, m.Registers.Flags);
                        return 16;
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
                        var reg = m.Registers.GetRegisterById(opcode & 0x7);
                        Instructions.RotateRightThroughCarry(reg, m.Registers.Flags);
                        return 8;
                    }
                #endregion

                #region RR (HL)
                case 0x1E:
                    {
                        var mem = new MemoryLocation(m.Memory, m.Registers.HL.Value);
                        Instructions.RotateRightThroughCarry(mem, m.Registers.Flags);
                        return 16;
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
                        var reg = m.Registers.GetRegisterById(opcode & 0x7);
                        Instructions.ShiftLeft(reg, m.Registers.Flags);
                        return 8;
                    }
                #endregion

                #region SLA (HL)
                case 0x26:
                    {
                        var mem = new MemoryLocation(m.Memory, m.Registers.HL.Value);
                        Instructions.ShiftLeft(mem, m.Registers.Flags);
                        return 16;
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
                        var reg = m.Registers.GetRegisterById(opcode & 0x7);
                        Instructions.ShiftRightArithmetic(reg, m.Registers.Flags);
                        return 8;
                    }
                #endregion

                #region SRA (HL)
                case 0x2E:
                    {
                        var mem = new MemoryLocation(m.Memory, m.Registers.HL.Value);
                        Instructions.ShiftRightArithmetic(mem, m.Registers.Flags);
                        return 16;
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
                        var reg = m.Registers.GetRegisterById(opcode & 0x7);
                        Instructions.ShiftRightLogical(reg, m.Registers.Flags);
                        return 8;
                    }
                #endregion

                #region SRL (HL)
                case 0x3E:
                    {
                        var mem = new MemoryLocation(m.Memory, m.Registers.HL.Value);
                        Instructions.ShiftRightLogical(mem, m.Registers.Flags);
                        return 16;
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
                        var reg = m.Registers.GetRegisterById(opcode & 0x7);
                        Instructions.SwapNibbles(reg, m.Registers.Flags);
                        return 8;
                    }
                #endregion

                #region SWAP (HL)
                case 0x36:
                    {
                        var mem = new MemoryLocation(m.Memory, m.Registers.HL.Value);
                        Instructions.SwapNibbles(mem, m.Registers.Flags);
                        return 16;
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
                        var reg = m.Registers.GetRegisterById(opcode & 0x7);
                        Instructions.TestBit(reg, index, m.Registers.Flags);
                        return 8;
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
                        var mem = new MemoryLocation(m.Memory, m.Registers.HL.Value);
                        Instructions.TestBit(mem, index, m.Registers.Flags);
                        return 16;
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
                        var reg = m.Registers.GetRegisterById(opcode & 0x7);
                        Instructions.SetBit(reg, index, true);
                        return 8;
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
                        var mem = new MemoryLocation(m.Memory, m.Registers.HL.Value);
                        Instructions.SetBit(mem, index, true);
                        return 16;
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
                        var reg = m.Registers.GetRegisterById(opcode & 0x7);
                        Instructions.SetBit(reg, index, false);
                        return 8;
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
                        var mem = new MemoryLocation(m.Memory, m.Registers.HL.Value);
                        Instructions.SetBit(mem, index, false);
                        return 16;
                    }
                #endregion

                default:
                    throw new InvalidOperationException("Invalid opcode.");
            }
        }

        private static byte NextInstructionByte(IMachineState m)
        {
            return m.Memory[m.Registers.PC.Value++];
        }
    }
}
