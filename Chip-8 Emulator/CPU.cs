using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip_8.Chip_8_Emulator
{
    public class CPU
    {
        // CPU registers.
        public byte[] V = new byte[16];

        // Special flag register.
        public byte VF;

        // Delay timer.
        byte DT;

        // Sound timer.
        byte ST;

        // Program counter.
        int PC;

        // Stack pointer.
        byte SP;

        // Stack.
        int[] stack = new int[16];

        // Physical memory.
        byte[] _mem = new byte[4096];

        // Graphics memory.
        byte[,] _g_mem = new byte[64, 32];

        public byte[,] _sprites = new byte[,]
        {
            // 0 sprite
            { 0xF0, 0x90, 0x90, 0x90, 0xF0 },
            // 1 Sprite
            { 0x20, 0x60, 0x20, 0x20, 0x70 },
            // 2 sprite
            { 0xF0, 0x10, 0xF0, 0x80, 0xF0 },
            // 3 sprite
            { 0xF0, 0x10, 0xF0, 0x10, 0xF0 },
            // 4 sprite
            { 0x90, 0x90, 0xF0, 0x10, 0x10 },
            // 5 sprite
            { 0xF0, 0x80, 0xF0, 0x10, 0xF0 },
            // 6 sprite
            { 0xF0, 0x80, 0xF0, 0x90, 0xF0},
            // 7 sprite
            {0xF0, 0x10, 0x20, 0x40, 0x40 },
            // 8 sprite
            { 0xF0, 0x90, 0xF0, 0x90, 0xF0},
            // 9 sprite
            {0xF0, 0x90, 0xF0, 0x10, 0xF0},
            // A sprite
            { 0xF0, 0x90, 0xF0, 0x90, 0x90},
            // B sprite
            {0xE0, 0x90, 0xE0, 0x90, 0xE0},
            // C sprite
            { 0xF0, 0x80, 0x80, 0x80, 0xF0},
            // D sprite
            { 0xE0, 0x90, 0x90, 0x90, 0xE0},
            // E sprite
            { 0xF0, 0x80, 0xF0, 0x80, 0xF0},
            // F sprite
            {0xF0, 0x80, 0xF0, 0x80, 0x80}
        };

        // 0x00E0 -- Clear the screen.
        private void CLS()
        {
            for (int y = 0; y < _g_mem.GetLength(0); y++)
            {
                for (int x = 0; x < _g_mem.GetLength(1); y++)
                {
                    _g_mem[y, x] &= 0;
                }
            }
        }

        // 0x00EE -- Return from a subroutine.
        private void RET()
        {
            PC = stack[SP];
            SP--;
        }

        // 0x1nnn -- Jump to location nnn.
        private void JP(int nnn)
        {
            PC = nnn;
        }

        // 0x2nnn -- Call subroutine at nnn.
        private void CALL(int nnn)
        {
            SP++;
            stack[SP] = PC;
            PC = nnn;
        }

        // 0x3xkk -- Skip the next instruction if Vx == kk
        public void SE(byte x, byte kk)
        {
            if (V[x] == kk)
            {
                PC++;
            }
        }

        // 0x4xkk -- Skip the next instruction if Vx != kk
        public void SNE(byte x, byte kk)
        {
            if (V[x] != kk)
            {
                PC++;
            }
        }

        // 0x5xy0 -- Skip the next instruction if Vx == Vy
        public void SEV(byte x, byte y)
        {
            if (V[x] == V[y])
            {
                PC++;
            }
        }

        // 0x6xkk -- Puts the value kk into register Vx
        public void LDV(byte x, byte kk)
        {
            V[x] = kk;
        }

        // 0x7xkk -- Adds the value kk to the value of register Vx, then stores the result in Vx
        public void ADDV(byte x, byte kk)
        {
            V[x] += kk;
        }

        // 0x8xy0 -- Store the value of register Vy in register Vx
        public void LDR(byte x, byte y)
        {
            V[x] = V[y];
        }

        // 0x8xy1 -- Set Vx = Vx OR Vy
        public void OR(int x, int y)
        {
            V[x] |= V[y];
        }

        // 0x8xy2 -- Set Vx = AND Vy
        public void AND(int x, int y)
        {
            V[x] &= V[y];
        }

        // 0x8xy3 -- Set Vx = Vx XOR Vy
        public void XOR (int x, int y)
        {
            V[x] ^= V[y];
        }

        // 0x8xy4 -- Set Vx = Vx + Vy, set VF = carry.
        public void ADDRC (int x, int y)
        {
            int value = V[x] + V[y];

            V[x] = (byte)(value & 0xFF);

            if (value > 0xFF)
            {
                VF = 0x01;
            }
            else
            {
                VF = 0x00;
            }
        }

        // 0x8xy5 -- Set Vx = Vx - Vy, set VF to NOT borrow.
        public void SUBR (int x, int y)
        {
            if (V[x] > V[y])
            {
                VF = 1;
            }
            else
            {
                VF = 0;
            }

            V[x] -= V[y];
        }

        // 0x8xy6 -- sets Vx = Vx >> 1
        public void SHR (int x)
        {
            if ((V[x] & 0x01) > 0)
            {
                VF = 1;
            }
            else
            {
                VF = 0;
            }

            V[x] >>= 1;
        }

        private void Fetch()
        {
            // Fetch memory from PC and store in IR
            // Increment PC by 1.
        }

        private void Decode()
        {
            // Decode the binary of IR
        }

        private void Execute()
        {
            // Run the instruction that was decoded.
        }

        public void Start()
        {
            // Initialize everything.

            // Load ROM into memory

            // Set PC to first instruction

            while (true)
            {
                Fetch();
                Decode();
                Execute();

                // Delay CPU to emulate true speed.
                Task.Delay(500);
            }
        }
    }
}
