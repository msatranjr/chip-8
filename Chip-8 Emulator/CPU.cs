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

        byte[][] _g_mem;

        public byte[][] _sprites = new byte[][]
        {
            // 0 sprite
            new byte[]{ 0xF0, 0x90, 0x90, 0x90, 0xF0 },
            // 1 Sprite
            new byte[] { 0x20, 0x60, 0x20, 0x20, 0x70 },
            // 2 sprite
            new byte[]{ 0xF0, 0x10, 0xF0, 0x80, 0xF0 },
            // 3 sprite
            new byte[] { 0xF0, 0x10, 0xF0, 0x10, 0xF0 },
            // 4 sprite
            new byte[] { 0x90, 0x90, 0xF0, 0x10, 0x10 },
            // 5 sprite
            new byte[] { 0xF0, 0x80, 0xF0, 0x10, 0xF0 },
            // 6 sprite
            new byte[] { 0xF0, 0x80, 0xF0, 0x90, 0xF0},
            // 7 sprite
            new byte[] {0xF0, 0x10, 0x20, 0x40, 0x40 },
            // 8 sprite
            new byte[] { 0xF0, 0x90, 0xF0, 0x90, 0xF0},
            // 9 sprite
            new byte[] {0xF0, 0x90, 0xF0, 0x10, 0xF0},
            // A sprite
            new byte[] { 0xF0, 0x90, 0xF0, 0x90, 0x90},
            // B sprite
            new byte[] {0xE0, 0x90, 0xE0, 0x90, 0xE0},
            // C sprite
            new byte[] { 0xF0, 0x80, 0x80, 0x80, 0xF0},
            // D sprite
            new byte[] { 0xE0, 0x90, 0x90, 0x90, 0xE0},
            // E sprite
            new byte[] { 0xF0, 0x80, 0xF0, 0x80, 0xF0},
            // F sprite
            new byte[] {0xF0, 0x80, 0xF0, 0x80, 0x80}
        };

        public void Start()
        {
        }
    }
}
