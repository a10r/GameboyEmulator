using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using GameboyEmulator.Core.Emulation;

namespace GameboyEmulator.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var emulator = new EmulationEngine {Running = true};
            Task.Run(() => emulator.Run());

            Task.Run(async () =>
            {
                long last = emulator.ElapsedCycles;
                while (true)
                {
                    await Task.Delay(1000);
                    var elapsed = emulator.ElapsedCycles - last;
                    System.Console.WriteLine($"{(float)elapsed / 1000000} MHz");
                    last = emulator.ElapsedCycles;
                }
            });

            //var rom = MemoryBlock.LoadFromFile("C:/Users/Andreas/Dropbox/DMG_ROM.bin");

            //rom.Dump(new StreamWriter(File.Open("C:/Users/Andreas/Desktop/Bootrom.txt", FileMode.OpenOrCreate)));

            System.Console.ReadKey();
        }

        static Dictionary<int, Func<int, int>> _lut = new Dictionary<int, Func<int, int>>();

        static void LutTest()
        {
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                _lut.Add(i, b =>
                {
                    var count = 0;
                    for (int x = i; x > 0; x--)
                    {
                        count += x;
                    }
                    return count;
                });
            }

            var random = new Random();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var res = 0;
            for (int i = 0; i < 10000000; i++)
            {
                var randomIndex = random.Next(byte.MaxValue + 1);
                res = _lut[randomIndex](randomIndex);
            }

            stopwatch.Stop();
            System.Console.WriteLine(res);
            System.Console.WriteLine(stopwatch.Elapsed);
        }

        static void SwitchTest()
        {
            var random = new Random();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var res = 0;
            for (int i = 0; i < 10000000; i++)
            {
                var randomIndex = random.Next(byte.MaxValue + 1);

                var count = 0;
                switch (randomIndex)
                {
                    case 0: for (int x = 0; x > 0; x--) { count += x; } break;
                    case 1: for (int x = 1; x > 0; x--) { count += x; } break;
                    case 2: for (int x = 2; x > 0; x--) { count += x; } break;
                    case 3: for (int x = 3; x > 0; x--) { count += x; } break;
                    case 4: for (int x = 4; x > 0; x--) { count += x; } break;
                    case 5: for (int x = 5; x > 0; x--) { count += x; } break;
                    case 6: for (int x = 6; x > 0; x--) { count += x; } break;
                    case 7: for (int x = 7; x > 0; x--) { count += x; } break;
                    case 8: for (int x = 8; x > 0; x--) { count += x; } break;
                    case 9: for (int x = 9; x > 0; x--) { count += x; } break;
                    case 10: for (int x = 10; x > 0; x--) { count += x; } break;
                    case 11: for (int x = 11; x > 0; x--) { count += x; } break;
                    case 12: for (int x = 12; x > 0; x--) { count += x; } break;
                    case 13: for (int x = 13; x > 0; x--) { count += x; } break;
                    case 14: for (int x = 14; x > 0; x--) { count += x; } break;
                    case 15: for (int x = 15; x > 0; x--) { count += x; } break;
                    case 16: for (int x = 16; x > 0; x--) { count += x; } break;
                    case 17: for (int x = 17; x > 0; x--) { count += x; } break;
                    case 18: for (int x = 18; x > 0; x--) { count += x; } break;
                    case 19: for (int x = 19; x > 0; x--) { count += x; } break;
                    case 20: for (int x = 20; x > 0; x--) { count += x; } break;
                    case 21: for (int x = 21; x > 0; x--) { count += x; } break;
                    case 22: for (int x = 22; x > 0; x--) { count += x; } break;
                    case 23: for (int x = 23; x > 0; x--) { count += x; } break;
                    case 24: for (int x = 24; x > 0; x--) { count += x; } break;
                    case 25: for (int x = 25; x > 0; x--) { count += x; } break;
                    case 26: for (int x = 26; x > 0; x--) { count += x; } break;
                    case 27: for (int x = 27; x > 0; x--) { count += x; } break;
                    case 28: for (int x = 28; x > 0; x--) { count += x; } break;
                    case 29: for (int x = 29; x > 0; x--) { count += x; } break;
                    case 30: for (int x = 30; x > 0; x--) { count += x; } break;
                    case 31: for (int x = 31; x > 0; x--) { count += x; } break;
                    case 32: for (int x = 32; x > 0; x--) { count += x; } break;
                    case 33: for (int x = 33; x > 0; x--) { count += x; } break;
                    case 34: for (int x = 34; x > 0; x--) { count += x; } break;
                    case 35: for (int x = 35; x > 0; x--) { count += x; } break;
                    case 36: for (int x = 36; x > 0; x--) { count += x; } break;
                    case 37: for (int x = 37; x > 0; x--) { count += x; } break;
                    case 38: for (int x = 38; x > 0; x--) { count += x; } break;
                    case 39: for (int x = 39; x > 0; x--) { count += x; } break;
                    case 40: for (int x = 40; x > 0; x--) { count += x; } break;
                    case 41: for (int x = 41; x > 0; x--) { count += x; } break;
                    case 42: for (int x = 42; x > 0; x--) { count += x; } break;
                    case 43: for (int x = 43; x > 0; x--) { count += x; } break;
                    case 44: for (int x = 44; x > 0; x--) { count += x; } break;
                    case 45: for (int x = 45; x > 0; x--) { count += x; } break;
                    case 46: for (int x = 46; x > 0; x--) { count += x; } break;
                    case 47: for (int x = 47; x > 0; x--) { count += x; } break;
                    case 48: for (int x = 48; x > 0; x--) { count += x; } break;
                    case 49: for (int x = 49; x > 0; x--) { count += x; } break;
                    case 50: for (int x = 50; x > 0; x--) { count += x; } break;
                    case 51: for (int x = 51; x > 0; x--) { count += x; } break;
                    case 52: for (int x = 52; x > 0; x--) { count += x; } break;
                    case 53: for (int x = 53; x > 0; x--) { count += x; } break;
                    case 54: for (int x = 54; x > 0; x--) { count += x; } break;
                    case 55: for (int x = 55; x > 0; x--) { count += x; } break;
                    case 56: for (int x = 56; x > 0; x--) { count += x; } break;
                    case 57: for (int x = 57; x > 0; x--) { count += x; } break;
                    case 58: for (int x = 58; x > 0; x--) { count += x; } break;
                    case 59: for (int x = 59; x > 0; x--) { count += x; } break;
                    case 60: for (int x = 60; x > 0; x--) { count += x; } break;
                    case 61: for (int x = 61; x > 0; x--) { count += x; } break;
                    case 62: for (int x = 62; x > 0; x--) { count += x; } break;
                    case 63: for (int x = 63; x > 0; x--) { count += x; } break;
                    case 64: for (int x = 64; x > 0; x--) { count += x; } break;
                    case 65: for (int x = 65; x > 0; x--) { count += x; } break;
                    case 66: for (int x = 66; x > 0; x--) { count += x; } break;
                    case 67: for (int x = 67; x > 0; x--) { count += x; } break;
                    case 68: for (int x = 68; x > 0; x--) { count += x; } break;
                    case 69: for (int x = 69; x > 0; x--) { count += x; } break;
                    case 70: for (int x = 70; x > 0; x--) { count += x; } break;
                    case 71: for (int x = 71; x > 0; x--) { count += x; } break;
                    case 72: for (int x = 72; x > 0; x--) { count += x; } break;
                    case 73: for (int x = 73; x > 0; x--) { count += x; } break;
                    case 74: for (int x = 74; x > 0; x--) { count += x; } break;
                    case 75: for (int x = 75; x > 0; x--) { count += x; } break;
                    case 76: for (int x = 76; x > 0; x--) { count += x; } break;
                    case 77: for (int x = 77; x > 0; x--) { count += x; } break;
                    case 78: for (int x = 78; x > 0; x--) { count += x; } break;
                    case 79: for (int x = 79; x > 0; x--) { count += x; } break;
                    case 80: for (int x = 80; x > 0; x--) { count += x; } break;
                    case 81: for (int x = 81; x > 0; x--) { count += x; } break;
                    case 82: for (int x = 82; x > 0; x--) { count += x; } break;
                    case 83: for (int x = 83; x > 0; x--) { count += x; } break;
                    case 84: for (int x = 84; x > 0; x--) { count += x; } break;
                    case 85: for (int x = 85; x > 0; x--) { count += x; } break;
                    case 86: for (int x = 86; x > 0; x--) { count += x; } break;
                    case 87: for (int x = 87; x > 0; x--) { count += x; } break;
                    case 88: for (int x = 88; x > 0; x--) { count += x; } break;
                    case 89: for (int x = 89; x > 0; x--) { count += x; } break;
                    case 90: for (int x = 90; x > 0; x--) { count += x; } break;
                    case 91: for (int x = 91; x > 0; x--) { count += x; } break;
                    case 92: for (int x = 92; x > 0; x--) { count += x; } break;
                    case 93: for (int x = 93; x > 0; x--) { count += x; } break;
                    case 94: for (int x = 94; x > 0; x--) { count += x; } break;
                    case 95: for (int x = 95; x > 0; x--) { count += x; } break;
                    case 96: for (int x = 96; x > 0; x--) { count += x; } break;
                    case 97: for (int x = 97; x > 0; x--) { count += x; } break;
                    case 98: for (int x = 98; x > 0; x--) { count += x; } break;
                    case 99: for (int x = 99; x > 0; x--) { count += x; } break;
                    case 100: for (int x = 100; x > 0; x--) { count += x; } break;
                    case 101: for (int x = 101; x > 0; x--) { count += x; } break;
                    case 102: for (int x = 102; x > 0; x--) { count += x; } break;
                    case 103: for (int x = 103; x > 0; x--) { count += x; } break;
                    case 104: for (int x = 104; x > 0; x--) { count += x; } break;
                    case 105: for (int x = 105; x > 0; x--) { count += x; } break;
                    case 106: for (int x = 106; x > 0; x--) { count += x; } break;
                    case 107: for (int x = 107; x > 0; x--) { count += x; } break;
                    case 108: for (int x = 108; x > 0; x--) { count += x; } break;
                    case 109: for (int x = 109; x > 0; x--) { count += x; } break;
                    case 110: for (int x = 110; x > 0; x--) { count += x; } break;
                    case 111: for (int x = 111; x > 0; x--) { count += x; } break;
                    case 112: for (int x = 112; x > 0; x--) { count += x; } break;
                    case 113: for (int x = 113; x > 0; x--) { count += x; } break;
                    case 114: for (int x = 114; x > 0; x--) { count += x; } break;
                    case 115: for (int x = 115; x > 0; x--) { count += x; } break;
                    case 116: for (int x = 116; x > 0; x--) { count += x; } break;
                    case 117: for (int x = 117; x > 0; x--) { count += x; } break;
                    case 118: for (int x = 118; x > 0; x--) { count += x; } break;
                    case 119: for (int x = 119; x > 0; x--) { count += x; } break;
                    case 120: for (int x = 120; x > 0; x--) { count += x; } break;
                    case 121: for (int x = 121; x > 0; x--) { count += x; } break;
                    case 122: for (int x = 122; x > 0; x--) { count += x; } break;
                    case 123: for (int x = 123; x > 0; x--) { count += x; } break;
                    case 124: for (int x = 124; x > 0; x--) { count += x; } break;
                    case 125: for (int x = 125; x > 0; x--) { count += x; } break;
                    case 126: for (int x = 126; x > 0; x--) { count += x; } break;
                    case 127: for (int x = 127; x > 0; x--) { count += x; } break;
                    case 128: for (int x = 128; x > 0; x--) { count += x; } break;
                    case 129: for (int x = 129; x > 0; x--) { count += x; } break;
                    case 130: for (int x = 130; x > 0; x--) { count += x; } break;
                    case 131: for (int x = 131; x > 0; x--) { count += x; } break;
                    case 132: for (int x = 132; x > 0; x--) { count += x; } break;
                    case 133: for (int x = 133; x > 0; x--) { count += x; } break;
                    case 134: for (int x = 134; x > 0; x--) { count += x; } break;
                    case 135: for (int x = 135; x > 0; x--) { count += x; } break;
                    case 136: for (int x = 136; x > 0; x--) { count += x; } break;
                    case 137: for (int x = 137; x > 0; x--) { count += x; } break;
                    case 138: for (int x = 138; x > 0; x--) { count += x; } break;
                    case 139: for (int x = 139; x > 0; x--) { count += x; } break;
                    case 140: for (int x = 140; x > 0; x--) { count += x; } break;
                    case 141: for (int x = 141; x > 0; x--) { count += x; } break;
                    case 142: for (int x = 142; x > 0; x--) { count += x; } break;
                    case 143: for (int x = 143; x > 0; x--) { count += x; } break;
                    case 144: for (int x = 144; x > 0; x--) { count += x; } break;
                    case 145: for (int x = 145; x > 0; x--) { count += x; } break;
                    case 146: for (int x = 146; x > 0; x--) { count += x; } break;
                    case 147: for (int x = 147; x > 0; x--) { count += x; } break;
                    case 148: for (int x = 148; x > 0; x--) { count += x; } break;
                    case 149: for (int x = 149; x > 0; x--) { count += x; } break;
                    case 150: for (int x = 150; x > 0; x--) { count += x; } break;
                    case 151: for (int x = 151; x > 0; x--) { count += x; } break;
                    case 152: for (int x = 152; x > 0; x--) { count += x; } break;
                    case 153: for (int x = 153; x > 0; x--) { count += x; } break;
                    case 154: for (int x = 154; x > 0; x--) { count += x; } break;
                    case 155: for (int x = 155; x > 0; x--) { count += x; } break;
                    case 156: for (int x = 156; x > 0; x--) { count += x; } break;
                    case 157: for (int x = 157; x > 0; x--) { count += x; } break;
                    case 158: for (int x = 158; x > 0; x--) { count += x; } break;
                    case 159: for (int x = 159; x > 0; x--) { count += x; } break;
                    case 160: for (int x = 160; x > 0; x--) { count += x; } break;
                    case 161: for (int x = 161; x > 0; x--) { count += x; } break;
                    case 162: for (int x = 162; x > 0; x--) { count += x; } break;
                    case 163: for (int x = 163; x > 0; x--) { count += x; } break;
                    case 164: for (int x = 164; x > 0; x--) { count += x; } break;
                    case 165: for (int x = 165; x > 0; x--) { count += x; } break;
                    case 166: for (int x = 166; x > 0; x--) { count += x; } break;
                    case 167: for (int x = 167; x > 0; x--) { count += x; } break;
                    case 168: for (int x = 168; x > 0; x--) { count += x; } break;
                    case 169: for (int x = 169; x > 0; x--) { count += x; } break;
                    case 170: for (int x = 170; x > 0; x--) { count += x; } break;
                    case 171: for (int x = 171; x > 0; x--) { count += x; } break;
                    case 172: for (int x = 172; x > 0; x--) { count += x; } break;
                    case 173: for (int x = 173; x > 0; x--) { count += x; } break;
                    case 174: for (int x = 174; x > 0; x--) { count += x; } break;
                    case 175: for (int x = 175; x > 0; x--) { count += x; } break;
                    case 176: for (int x = 176; x > 0; x--) { count += x; } break;
                    case 177: for (int x = 177; x > 0; x--) { count += x; } break;
                    case 178: for (int x = 178; x > 0; x--) { count += x; } break;
                    case 179: for (int x = 179; x > 0; x--) { count += x; } break;
                    case 180: for (int x = 180; x > 0; x--) { count += x; } break;
                    case 181: for (int x = 181; x > 0; x--) { count += x; } break;
                    case 182: for (int x = 182; x > 0; x--) { count += x; } break;
                    case 183: for (int x = 183; x > 0; x--) { count += x; } break;
                    case 184: for (int x = 184; x > 0; x--) { count += x; } break;
                    case 185: for (int x = 185; x > 0; x--) { count += x; } break;
                    case 186: for (int x = 186; x > 0; x--) { count += x; } break;
                    case 187: for (int x = 187; x > 0; x--) { count += x; } break;
                    case 188: for (int x = 188; x > 0; x--) { count += x; } break;
                    case 189: for (int x = 189; x > 0; x--) { count += x; } break;
                    case 190: for (int x = 190; x > 0; x--) { count += x; } break;
                    case 191: for (int x = 191; x > 0; x--) { count += x; } break;
                    case 192: for (int x = 192; x > 0; x--) { count += x; } break;
                    case 193: for (int x = 193; x > 0; x--) { count += x; } break;
                    case 194: for (int x = 194; x > 0; x--) { count += x; } break;
                    case 195: for (int x = 195; x > 0; x--) { count += x; } break;
                    case 196: for (int x = 196; x > 0; x--) { count += x; } break;
                    case 197: for (int x = 197; x > 0; x--) { count += x; } break;
                    case 198: for (int x = 198; x > 0; x--) { count += x; } break;
                    case 199: for (int x = 199; x > 0; x--) { count += x; } break;
                    case 200: for (int x = 200; x > 0; x--) { count += x; } break;
                    case 201: for (int x = 201; x > 0; x--) { count += x; } break;
                    case 202: for (int x = 202; x > 0; x--) { count += x; } break;
                    case 203: for (int x = 203; x > 0; x--) { count += x; } break;
                    case 204: for (int x = 204; x > 0; x--) { count += x; } break;
                    case 205: for (int x = 205; x > 0; x--) { count += x; } break;
                    case 206: for (int x = 206; x > 0; x--) { count += x; } break;
                    case 207: for (int x = 207; x > 0; x--) { count += x; } break;
                    case 208: for (int x = 208; x > 0; x--) { count += x; } break;
                    case 209: for (int x = 209; x > 0; x--) { count += x; } break;
                    case 210: for (int x = 210; x > 0; x--) { count += x; } break;
                    case 211: for (int x = 211; x > 0; x--) { count += x; } break;
                    case 212: for (int x = 212; x > 0; x--) { count += x; } break;
                    case 213: for (int x = 213; x > 0; x--) { count += x; } break;
                    case 214: for (int x = 214; x > 0; x--) { count += x; } break;
                    case 215: for (int x = 215; x > 0; x--) { count += x; } break;
                    case 216: for (int x = 216; x > 0; x--) { count += x; } break;
                    case 217: for (int x = 217; x > 0; x--) { count += x; } break;
                    case 218: for (int x = 218; x > 0; x--) { count += x; } break;
                    case 219: for (int x = 219; x > 0; x--) { count += x; } break;
                    case 220: for (int x = 220; x > 0; x--) { count += x; } break;
                    case 221: for (int x = 221; x > 0; x--) { count += x; } break;
                    case 222: for (int x = 222; x > 0; x--) { count += x; } break;
                    case 223: for (int x = 223; x > 0; x--) { count += x; } break;
                    case 224: for (int x = 224; x > 0; x--) { count += x; } break;
                    case 225: for (int x = 225; x > 0; x--) { count += x; } break;
                    case 226: for (int x = 226; x > 0; x--) { count += x; } break;
                    case 227: for (int x = 227; x > 0; x--) { count += x; } break;
                    case 228: for (int x = 228; x > 0; x--) { count += x; } break;
                    case 229: for (int x = 229; x > 0; x--) { count += x; } break;
                    case 230: for (int x = 230; x > 0; x--) { count += x; } break;
                    case 231: for (int x = 231; x > 0; x--) { count += x; } break;
                    case 232: for (int x = 232; x > 0; x--) { count += x; } break;
                    case 233: for (int x = 233; x > 0; x--) { count += x; } break;
                    case 234: for (int x = 234; x > 0; x--) { count += x; } break;
                    case 235: for (int x = 235; x > 0; x--) { count += x; } break;
                    case 236: for (int x = 236; x > 0; x--) { count += x; } break;
                    case 237: for (int x = 237; x > 0; x--) { count += x; } break;
                    case 238: for (int x = 238; x > 0; x--) { count += x; } break;
                    case 239: for (int x = 239; x > 0; x--) { count += x; } break;
                    case 240: for (int x = 240; x > 0; x--) { count += x; } break;
                    case 241: for (int x = 241; x > 0; x--) { count += x; } break;
                    case 242: for (int x = 242; x > 0; x--) { count += x; } break;
                    case 243: for (int x = 243; x > 0; x--) { count += x; } break;
                    case 244: for (int x = 244; x > 0; x--) { count += x; } break;
                    case 245: for (int x = 245; x > 0; x--) { count += x; } break;
                    case 246: for (int x = 246; x > 0; x--) { count += x; } break;
                    case 247: for (int x = 247; x > 0; x--) { count += x; } break;
                    case 248: for (int x = 248; x > 0; x--) { count += x; } break;
                    case 249: for (int x = 249; x > 0; x--) { count += x; } break;
                    case 250: for (int x = 250; x > 0; x--) { count += x; } break;
                    case 251: for (int x = 251; x > 0; x--) { count += x; } break;
                    case 252: for (int x = 252; x > 0; x--) { count += x; } break;
                    case 253: for (int x = 253; x > 0; x--) { count += x; } break;
                    case 254: for (int x = 254; x > 0; x--) { count += x; } break;
                    case 255: for (int x = 255; x > 0; x--) { count += x; } break;    
                }
                res = count;
            }

            stopwatch.Stop();
            System.Console.WriteLine(res);
            System.Console.WriteLine(stopwatch.Elapsed);
        }
    }
}
