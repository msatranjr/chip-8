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
        byte[] V = new byte[16];

        // Special flag register.
        byte[] VF;

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
        byte[,] _g_mem = new byte[64,32];

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
        public void SE(int x, int kk)
        {
            if (V[x] == kk)
            {
                PC++;
            }
        }

        // 0x4xkk -- Skip the next instruction if Vx != kk
        public void SNE(int x, int kk)
        {
            if (V[x] != kk)
            {
                PC++;
            }
        }

        // 0x5xy0 -- Skip the next instruction if Vx == Vy
        public void SEV(int x, int y)
        {
            if (V[x] == V[y])
            {
                PC++;
            }
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

            while(true)
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
