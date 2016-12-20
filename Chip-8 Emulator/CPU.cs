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

        // Instruction register.
        int IR;

        int I;

        // Stack.
        int[] stack = new int[16];

        // Physical memory.
        byte[] _mem = new byte[4096];

        // Graphics memory.
        public byte[,] _g_mem = new byte[32, 12];

        public byte[] _sprites = new byte[]
        {
            // 0 sprite
            0xF0, 0x90, 0x90, 0x90, 0xF0,
            // 1 Sprite
            0x20, 0x60, 0x20, 0x20, 0x70,
            // 2 sprite
            0xF0, 0x10, 0xF0, 0x80, 0xF0,
            // 3 sprite
            0xF0, 0x10, 0xF0, 0x10, 0xF0,
            // 4 sprite
            0x90, 0x90, 0xF0, 0x10, 0x10,
            // 5 sprite
            0xF0, 0x80, 0xF0, 0x10, 0xF0,
            // 6 sprite
            0xF0, 0x80, 0xF0, 0x90, 0xF0,
            // 7 sprite
            0xF0, 0x10, 0x20, 0x40, 0x40,
            // 8 sprite
            0xF0, 0x90, 0xF0, 0x90, 0xF0,
            // 9 sprite
            0xF0, 0x90, 0xF0, 0x10, 0xF0,
            // A sprite
            0xF0, 0x90, 0xF0, 0x90, 0x90,
            // B sprite
            0xE0, 0x90, 0xE0, 0x90, 0xE0,
            // C sprite
            0xF0, 0x80, 0x80, 0x80, 0xF0,
            // D sprite
            0xE0, 0x90, 0x90, 0x90, 0xE0,
            // E sprite
            0xF0, 0x80, 0xF0, 0x80, 0xF0,
            // F sprite
            0xF0, 0x80, 0xF0, 0x80, 0x80
        };

        Dictionary<int, Action> Decoder { get; set; }

        public int NNN
        {
            get
            {
                return (IR & 0x0FFF);
            }
        }

        public int NN
        {
            get
            {
                return (IR & 0x00FF);
            }
        }

        public int N
        {
            get
            {
                return (IR & 0x000F);
            }
        }

        public int X
        {
            get
            {
                return (IR & 0x0F00) >> 8;
            }
        }

        public int Y
        {
            get
            {
                return (IR & 0x00F0) >> 4;
            }
        }


        public CPU()
        {
            Decoder = new Dictionary<int, Action>()
            {
                // 0x0*** opcodes.
                {0x0, () =>
                {   
                    // 0x00E0 -- Clear the screen.
                    if ((IR & 0x00F0) == 0x00E0)
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
                    else if ((IR & 0x00EE) == 0x00EE)
                    {
                        PC = stack[SP];
                        SP--;
                    }
                    else
                    {
                        // No-op or halt
                    }
                }},

                 // 0x1nnn -- Jump to location nnn.
                {0x1, () =>
                {   
                    PC = NNN;
                }},

                // 0x2nnn -- Call subroutine at nnn.
                {0x2, () =>
                {
                    SP++;
                    stack[SP] = PC;
                    PC = NNN;
                }},

                // 0x3xnn -- Skip the next instruction if Vx == nn
                {0x3, () =>
                {
                    if (V[X] == NN)
                    {
                        PC += 2;
                    }
                }},

                // 0x4xnn -- Skip the next instruction if Vx != nn
                {0x4, () =>
                {
                    if (V[X] != NN)
                    {
                        PC += 2;
                    }
                }},

                // 0x5xy0 -- Skip the next instruction if Vx == Vy
                {0x5, () =>
                {
                    if (V[X] == V[Y])
                    {
                        PC += 2;
                    }
                }},

                // 0x6xkk -- Puts the value kk into register Vx
                {0x6, () =>
                {
                    V[X] = (byte)NN;
                }},

                // 0x7xkk -- Adds the value kk to the value of register Vx, then stores the result in Vx
                {0x7, () =>
                {
                    V[X] += (byte)NN;
                }},

                // 0x800* opcodes.
                {0x8, () =>
                {
                    int c = IR & 0x000F;
                    switch (c)
                    {
                            // 0x8xy0 -- Store the value of register Vy in register Vx
                            case 0:
                            V[X] = V[Y];
                            break;
                            // 0x8xy1 -- Set Vx = Vx OR Vy
                            case 1:
                            V[X] |= V[Y];
                            break;
                            // 0x8xy2 -- Set Vx = AND Vy
                            case 2:
                            V[X] &= V[Y];
                            break;
                            // 0x8xy3 -- Set Vx = Vx XOR Vy
                            case 3:
                            V[X] ^= V[Y];
                            break;
                            // 0x8xy4 -- Set Vx = Vx + Vy, set VF = carry.
                            case 4:
                            int value = V[X] + V[Y];

                            V[X] = (byte)(value & 0xFF);

                            if (value > 0xFF)
                            {
                                VF = 0x01;
                            }
                            else
                            {
                                VF = 0x00;
                            }
                            break;
                            // 0x8xy5 -- Set Vx = Vx - Vy, set VF to NOT borrow.
                            case 5:
                            if (V[X] > V[Y])
                            {
                                VF = 1;
                            }
                            else
                            {
                                VF = 0;
                            }

                            V[X] -= V[Y];
                            break;
                            // 0x8xy6 -- sets Vx = Vx >> 1
                            case 6:
                            if ((V[X] & 0x01) > 0)
                            {
                                VF = 1;
                            }
                            else
                            {
                                VF = 0;
                            }

                            V[X] >>= 1;
                            break;
                            // 0x8xy7 -- SUBN Vx, Vy
                            case 7:
                            if (V[Y] > V[X])
                            {
                                VF = 1;
                            }
                            else
                            {
                                VF = 0;
                            }

                            V[X] = (byte)(V[Y] - V[X]);
                            break;
                            // 8xyE - SHL Vx {, Vy}
                            case 0xE:
                            if ((0x80 & V[X]) == 0x80)
                            {
                                VF = 1;
                            }
                            else
                            {
                                VF = 0;
                            }
                            V[X] <<= 1;
                            break;
                    }
                }},

                // 0x9xy0 -- SNE Vx, Vy
                {0x9, () =>
                {
                    if (V[X] != V[Y])
                        PC += 2;
                }},

                // 0xAnnn -- LD I, addr
                {0xA, () =>
                {
                    I = IR & NNN;
                }},

                //Bnnn - JP V0, addr Jump to location nnn + V0.
                {0xB, () =>
                {
                    PC = NNN + V[0];
                }},

                // Cxkk - RND Vx, byte Set Vx = random byte AND kk
                {0xC, () =>
                {
                    V[X] = (byte)(((new Random(DateTime.Now.Second)).Next(0,255) & 0xFF) & NN);
                }},

                // Dxyn - DRW Vx, Vy, nibble Display n-byte sprite starting at memory location I at (Vx, Vy), set VF = collision.
                {0xD, () =>
                {   
                    for (int i = 0; i < N; i++)
                    {
                        byte b = _mem[I+i];
                        int yBitPos = (Y + i) % 32;

                        for (int j = 0; j < 8; j++)
                        {
                            int mask = 0x80 >> (7 - j);
                            int xBitPos = (X + j) % 64;

                            int g_bit = _g_mem[yBitPos, xBitPos/8] & mask;
                            int b_bit = b & mask;

                            if (g_bit == b_bit)
                                VF = 1;
                            else
                                VF = 0;

                            _g_mem[yBitPos, xBitPos/8] ^= (byte)b_bit;
                        }
                    }
                }},

                {0xE, () =>
                {
                    
                }},

                {0xF, () =>
                {
                   int c = IR & 0x00FF;

                   switch (c)
                    {
                        case 0x07:
                            break;
                        case 0x0A:
                            break;
                        case 0x15:
                            break;
                        case 0x18:
                            break;
                        case 0x1E:
                            break;
                        case 0x29:
                            break;
                        // Fx33 - LD B, Vx. Stores the BCD representation of V[X] in memory locations I, I+1, I+2
                        case 0x33:
                            int dec = V[X];
                            int hundreds = (dec / 100);
                            int tens = (dec / 10) - (hundreds * 10);
                            int ones = dec % 10;
                            _mem[I] = (byte)hundreds;
                            _mem[I+1] = (byte)tens;
                            _mem[I+2] = (byte)ones;
                            break;
                        case 0x55:
                            for (int i = 0; i <= X; i++)
                            {
                                _mem[I+i] = V[i];
                            }
                            break;
                        case 0x65:
                            for (int i = 0; i <= X; i++)
                            {
                                V[i] = _mem[I+i];
                            }
                            break;
                    }
                }},
        };
        }

        public void Load(byte[] instructions)
        {
            for (int i = 0; i < instructions.Count(); i++)
            {
                _mem[0x200 + i] = instructions[i];
            }
        }

        public void Start()
        {
            // Initialize everything.

            // Load sprites into memory.
            for (int i = 0; i < _sprites.Length; i++)
                _mem[i] = _sprites[i];

            // Load ROM into memory

            PC = 0x200;

            do
            {
                // Fetch memory from PC and store in IR
                IR &= 0;
                IR |= _mem[PC];
                IR <<= 8;
                IR |= _mem[PC + 1];

                // Increment PC for the next instruction.
                PC += 2;

                // Decode and execute.
                Decoder[(IR & 0xF000) >> 12]();

                // Delay CPU to emulate true speed.
                Task.Delay(500);
            } while (IR != 0x0);
        }

        public override string ToString()
        {
            string screen = "";

            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        int mask = 0x80 >> (7 - k);

                        screen += (_g_mem[i, j] & mask) == mask ? "1" : "0";
                    }
                }

                screen += "\n";
            }

            return screen;
        }
    }
}
