using GameboyEmulator.Core.Memory;
using GameboyEmulator.Core.Utils;

namespace GameboyEmulator.Core.Processor
{
    public static class Instructions
    {
        public static void Load<T>(IWriteonlyRegister<T> target, T value)
        {
            target.Value = value;
        }

        public static void LoadWithSignedOffset(IWriteonlyRegister<ushort> target, IReadonlyRegister<ushort> source, sbyte offset, IFlags flags)
        {
            var old = source.Value;
            target.Value = (ushort)(source.Value + offset);
            flags.Zero = false;
            flags.Subtract = false;
            flags.Carry = (old & 0xFF) + (offset & 0xFF) >= 0x100;
            flags.HalfCarry = (old & 0xF) + (offset & 0xF) >= 0x10;
        }

        public static void Push(IStack stack, ushort value)
        {
            stack.Push(value.GetHigh());
            stack.Push(value.GetLow());
        }

        public static void Pop(IWriteonlyRegister<ushort> target, IStack stack)
        {
            var low = stack.Pop();
            var high = stack.Pop();
            target.Value = BitUtils.Combine(low, high);
        }

        public static void Add(IRegister<byte> target, byte operand, IFlags flags)
        {
            var old = target.Value;
            target.Value += operand;
            flags.Zero = target.Value == 0;
            flags.Subtract = false;
            flags.Carry = target.Value < old;
            flags.HalfCarry = (((old & 0xF) + (operand & 0xF)) & 0x10) != 0;
        }

        public static void AddPlusCarry(IRegister<byte> target, byte operand, IFlags flags)
        {
            var carry = flags.Carry ? 1 : 0; 
            var old = target.Value;
            target.Value += (byte)(operand + carry);
            flags.Zero = target.Value == 0;
            flags.Subtract = false;
            flags.Carry = ((old + operand + carry) & 0x100) != 0;
            flags.HalfCarry = (((old & 0xF) + (operand & 0xF) + carry) & 0x10) != 0;
        }

        public static void Subtract(IRegister<byte> target, byte operand, IFlags flags)
        {
            var old = target.Value;
            target.Value -= operand;
            flags.Zero = target.Value == 0;
            flags.Subtract = true;
            flags.Carry = old < operand;
            flags.HalfCarry = (old & 0xF) < (operand & 0xF);
        }

        public static void SubtractMinusCarry(IRegister<byte> target, byte operand, IFlags flags)
        {
            var carry = (byte)(flags.Carry ? 1 : 0);
            var old = target.Value;
            target.Value -= operand;
            target.Value -= carry;
            flags.Zero = target.Value == 0;
            flags.Subtract = true;
            flags.Carry = old < (operand + carry);
            flags.HalfCarry = (old & 0xF) < ((operand & 0xF) + carry);
        }

        public static void And(IRegister<byte> target, byte operand, IFlags flags)
        {
            target.Value = (byte)(target.Value & operand);
            flags.Zero = target.Value == 0;
            flags.HalfCarry = true;
            flags.Subtract = flags.Carry = false;
        }

        public static void Or(IRegister<byte> target, byte operand, IFlags flags)
        {
            target.Value = (byte)(target.Value | operand);
            flags.Zero = target.Value == 0;
            flags.Subtract = flags.HalfCarry = flags.Carry = false;
        }

        public static void Xor(IRegister<byte> target, byte operand, IFlags flags)
        {
            target.Value = (byte)(target.Value ^ operand);
            flags.Zero = target.Value == 0;
            flags.Subtract = flags.HalfCarry = flags.Carry = false;
        }

        public static void Compare(IReadonlyRegister<byte> target, byte operand, IFlags flags)
        {
            flags.Zero = target.Value == operand;
            flags.Subtract = true;
            flags.Carry = target.Value < operand;
            flags.HalfCarry = (target.Value & 0xF) < (operand & 0xF);
        }

        public static void Increment(IRegister<byte> target, IFlags flags)
        {
            var old = target.Value;
            target.Value++;
            flags.Zero = target.Value == 0;
            flags.Subtract = false;
            flags.HalfCarry = (((old & 0xF) + 1) & 0x10) != 0;
        }

        public static void Decrement(IRegister<byte> target, IFlags flags)
        {
            var old = target.Value;
            target.Value--;
            flags.Zero = target.Value == 0;
            flags.Subtract = true;
            flags.HalfCarry = (old & 0xF) < 1; // TODO check this flag
        }

        public static void Add(IRegister<ushort> target, ushort operand, IFlags flags)
        {
            var old = target.Value;
            target.Value += operand;
            flags.Subtract = false;
            flags.Carry = target.Value < old;
            flags.HalfCarry = (((old & 0xFFF) + (operand & 0xFFF)) & 0x1000) != 0;
        }

        public static void Add(IRegister<ushort> target, sbyte operand, IFlags flags)
        {
            var old = target.Value;
            target.Value = (ushort)(target.Value + operand);
            flags.Subtract = false;
            flags.Carry = target.Value < old;
            flags.HalfCarry = (((old & 0xFFF) + (operand & 0xFFF)) & 0x1000) != 0;
        }
        
        public static void Increment(IRegister<ushort> target)
        {
            target.Value++;
        }

        public static void Decrement(IRegister<ushort> target)
        {
            target.Value--;
        }

        public static void SwapNibbles(IRegister<byte> target, IFlags flags)
        {
            target.Value = (byte)((target.Value << 4) | (target.Value >> 4));
            flags.Zero = target.Value == 0;
            flags.Subtract = flags.Carry = flags.HalfCarry = false;
        }

        public static void DecimalAdjust(IRegister<byte> target, IFlags flags)
        {
            int reg = target.Value;
            if (flags.Subtract)
            {
                if (flags.HalfCarry)
                {
                    reg -= 0x06;
                }
                if (flags.Carry)
                {
                    reg -= 0x60;
                }
            }
            else
            {
                if (flags.HalfCarry || (reg & 0xF) > 0x9)
                {
                    reg += 0x06;
                }
                if (flags.Carry || (reg & 0xFF0) > 0x90)
                {
                    reg += 0x60;
                    flags.Carry = true;
                }
            }

            target.Value = (byte)reg;
            flags.HalfCarry = false;
            flags.Zero = target.Value == 0;
        }

        public static void Complement(IRegister<byte> target, IFlags flags)
        {
            target.Value = (byte)(~target.Value);
            flags.Subtract = flags.HalfCarry = true;
        }

        public static void ComplementCarryFlag(IFlags flags)
        {
            flags.Carry = !flags.Carry;
            flags.Subtract = flags.HalfCarry = false;
        }

        public static void SetCarryFlag(IFlags flags)
        {
            flags.Carry = true;
            flags.Subtract = flags.HalfCarry = false;
        }

        public static void RotateLeftA(IRegister<byte> target, IFlags flags)
        {
            target.Value = (byte)((target.Value << 1) | (target.Value >> 7));
            flags.Zero = false;
            flags.Subtract = flags.HalfCarry = false;
            flags.Carry = target.GetBit(0);
        }

        public static void RotateLeftThroughCarryA(IRegister<byte> target, IFlags flags)
        {
            var carry = flags.Carry ? 1 : 0;
            flags.Carry = target.GetBit(7);
            target.Value = (byte)((target.Value << 1) | carry);
            flags.Subtract = flags.HalfCarry = false;
            flags.Zero = false;
        }

        public static void RotateRightA(IRegister<byte> target, IFlags flags)
        {
            flags.Carry = target.GetBit(0);
            target.Value = (byte)(target.Value >> 1 | target.Value << 7);
            flags.Subtract = flags.HalfCarry = false;
            flags.Zero = false;
        }

        public static void RotateRightThroughCarryA(IRegister<byte> target, IFlags flags)
        {
            var carry = flags.Carry ? 1 : 0;
            flags.Carry = target.GetBit(0);
            target.Value = (byte)(carry << 7 | target.Value >> 1);
            flags.Subtract = flags.HalfCarry = false;
            flags.Zero = false;
        }

        public static void RotateLeft(IRegister<byte> target, IFlags flags)
        {
            target.Value = (byte)((target.Value << 1) | (target.Value >> 7));
            flags.Zero = target.Value == 0;
            flags.Subtract = flags.HalfCarry = false;
            flags.Carry = target.GetBit(0);
        }

        public static void RotateLeftThroughCarry(IRegister<byte> target, IFlags flags)
        {
            var carry = flags.Carry ? 1 : 0;
            flags.Carry = target.GetBit(7);
            target.Value = (byte)((target.Value << 1) | carry);
            flags.Subtract = flags.HalfCarry = false;
            flags.Zero = target.Value == 0;
        }

        public static void RotateRight(IRegister<byte> target, IFlags flags)
        {
            flags.Carry = target.GetBit(0);
            target.Value = (byte)(target.Value >> 1 | target.Value << 7);
            flags.Subtract = flags.HalfCarry = false;
            flags.Zero = target.Value == 0;
        }

        public static void RotateRightThroughCarry(IRegister<byte> target, IFlags flags)
        {
            var carry = flags.Carry ? 1 : 0;
            flags.Carry = target.GetBit(0);
            target.Value = (byte)(carry << 7 | target.Value >> 1);
            flags.Subtract = flags.HalfCarry = false;
            flags.Zero = target.Value == 0;
        }

        public static void ShiftLeft(IRegister<byte> target, IFlags flags)
        {
            flags.Carry = target.GetBit(7);
            target.Value = (byte)(target.Value << 1);
            flags.Subtract = flags.HalfCarry = false;
            flags.Zero = target.Value == 0;
        }

        public static void ShiftRightArithmetic(IRegister<byte> target, IFlags flags)
        {
            flags.Carry = target.GetBit(0);
            var msbMask = target.Value & 0x80;
            target.Value = (byte)((target.Value >> 1) | msbMask);
            flags.Subtract = flags.HalfCarry = false;
            flags.Zero = target.Value == 0;
        }

        public static void ShiftRightLogical(IRegister<byte> target, IFlags flags)
        {
            flags.Carry = target.GetBit(0);
            target.Value = (byte)(target.Value >> 1);
            flags.Subtract = flags.HalfCarry = false;
            flags.Zero = target.Value == 0;
        }

        public static void TestBit(IReadonlyRegister<byte> source, int index, IFlags flags)
        {
            flags.Zero = !source.GetBit(index);
            flags.Subtract = false;
            flags.HalfCarry = true;
        }

        public static void SetBit(IRegister<byte> target, int index, bool value)
        {
            target.SetBit(index, value);
        }

        public static void ConditionalJump(IWriteonlyRegister<ushort> target, ushort address, JumpCondition condition, IFlags flags)
        {
            if (ShouldJump(condition, flags))
            {
                Load(target, address);
            }
        }

        // TODO: maybe not necessary
        public static void JumpByOffset(IRegister<ushort> target, sbyte offset)
        {
            Load(target, (ushort)(target.Value + offset));
        }

        public static void ConditionalJumpByOffset(IRegister<ushort> target, sbyte offset, JumpCondition condition, IFlags flags)
        {
            ConditionalJump(target, (ushort)(target.Value + offset), condition, flags);
        }

        public static void Call(IRegister<ushort> pc, ushort address, IStack stack)
        {
            stack.Push(pc.Value.GetHigh());
            stack.Push(pc.Value.GetLow());
            Load(pc, address);
        }

        public static void ConditionalCall(IRegister<ushort> pc, ushort address, IStack stack, JumpCondition condition, IFlags flags)
        {
            if (ShouldJump(condition, flags))
            {
                Call(pc, address, stack);
            }
        }
        
        public static void Restart(IRegister<ushort> pc, byte restartOffset, IStack stack)
        {   
            stack.Push(pc.Value.GetHigh());
            stack.Push(pc.Value.GetLow());
            Load(pc, (ushort)(restartOffset * 0x08));
        }

        public static void Return(IWriteonlyRegister<ushort> pc, IStack stack)
        {
            var low = stack.Pop();
            var high = stack.Pop();
            Load(pc, BitUtils.Combine(low, high));
        }

        public static void ConditionalReturn(IWriteonlyRegister<ushort> pc, IStack stack, JumpCondition condition, IFlags flags)
        {
            if (ShouldJump(condition, flags))
            {
                Return(pc, stack);
            }
        }

        // TODO: RETI
        
        private static bool ShouldJump(JumpCondition condition, IFlags flags)
        {
            return (condition == JumpCondition.Carry && flags.Carry) ||
                (condition == JumpCondition.NoCarry && !flags.Carry) ||
                (condition == JumpCondition.Zero && flags.Zero) ||
                (condition == JumpCondition.NotZero && !flags.Zero);
        }
    }
}
