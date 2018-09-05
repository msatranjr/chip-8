using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Chip_8.Chip_8_Emulator
{
    public class Pixel
    {
        public int x { get; set; }
        public int y { get; set; }
        public bool on { get; set; }
    }

    public class SetPixelsEventArgs : EventArgs
    {
        public List<Pixel> Pixels { get; set; }
    }

    public class CPU
    {
        public EventHandler<SetPixelsEventArgs> DrawScreen { get; set; }

        //public EventHandler ClearPixels { get; set; }

        // The Chip-8 clock speed in milliseconds.
        private static int _CLOCK_SPEED = (int)Math.Round(1000f / 540f);

        // CPU registers.
        public byte[] V = new byte[16];

        // Special flag register.
        //public bool VF;

        // Delay timer.
        bool DT_isRunning;
        byte DT;

        // Sound timer.
        bool ST_isRunning;
        byte ST;

        // Program counter.
        int PC;

        // Stack pointer.
        byte SP;

        // Instruction register.
        int IR;

        // Memory register
        int I;

        // Keyboard. Bool indicates whether the key is pressed or not.
        public ConcurrentDictionary<byte, bool> Keyboard { get; set; }

        // Stack.
        int[] stack = new int[16];

        // Physical memory.
        byte[] _mem = new byte[4096];

        // Graphics memory.
        public bool[,] _g_mem = new bool[32, 64];

        // For debugging only.
        public void printGraphicsMemory()
        {
            for (int i = 0; i < _g_mem.GetLength(0); i++)
            {
                for (int j = 0; j < _g_mem.GetLength(1); j++)
                {
                    if (_g_mem[i,j])
                    {
                        Debug.Write("X");
                    }
                    else
                    {
                        Debug.Write(" ");
                    }
                }
                Debug.WriteLine("");
            }
        }

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
                    if ((IR & 0x00FF) == 0x00E0)
                    {
                        for (int y = 0; y < _g_mem.GetLength(0); y++)
                        {
                            for (int x = 0; x < _g_mem.GetLength(1); x++)
                            {
                                _g_mem[y, x] = false;
                            }
                        }

                        DrawScreen(this, null);
                    }
                    // 0x00EE -- Return from a subroutine.
                    else if ((IR & 0x00FF) == 0x00EE)
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
                                V[0xF] = 1;
                            }
                            else
                            {
                                V[0xF] = 0;
                            }
                            break;
                            // 0x8xy5 -- Set Vx = Vx - Vy, set VF to NOT borrow.
                            case 5:
                            if (V[X] > V[Y])
                            {
                                V[0xF] = 1;
                            }
                            else
                            {
                                V[0xF] = 0;
                            }

                            V[X] -= V[Y];
                            break;
                            // 0x8xy6 -- sets Vx = Vx >> 1
                            case 6:
                            if ((V[X] & 0x01) > 0)
                            {
                                V[0xF] = 1;
                            }
                            else
                            {
                                V[0xF] = 0;
                            }

                            V[X] >>= 1;
                            break;
                            // 0x8xy7 -- SUBN Vx, Vy
                            case 7:
                            if (V[Y] > V[X])
                            {
                                V[0xF] = 1;
                            }
                            else
                            {
                                V[0xF] = 0;
                            }

                            V[X] = (byte)(V[Y] - V[X]);
                            break;
                            // 8xyE - SHL Vx {, Vy}
                            case 0xE:
                            if ((0x80 & V[X]) == 0x80)
                            {
                                V[0xF] = 1;
                            }
                            else
                            {
                                V[0xF] = 0;
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
                    V[0xF] = 0;

                    for (int i = 0; i < N; i++)
                    {
                        byte b = _mem[I+i];
                        int yPos = (V[Y] + i) % 32;

                        for (int j = 0; j < 8; j++)
                        {
                            int bMask = 0x80 >> j;
                            int xPos = (V[X] + j) % 64;

                            int b_bit = (b & bMask) << j;

                            if (_g_mem[yPos, xPos] && b_bit > 0)
                                V[0xF] = 1;

                            _g_mem[yPos, xPos] ^= (b_bit > 0 ? true : false);
                        }
                    }

                    DrawScreen?.Invoke(this, null);
                }},

                
                {

                // 0xE*** opcodes.
                0xE, () =>
                {
                    // Ex9E - SKP Vx -- Skip next instruction if key with the value of Vx is pressed.
                    if (NN == 0x9E)
                    {
                        if (Keyboard[V[X]])
                        {
                            PC += 2;
                        }
                    }
                    // ExA1 - SKNP Vx -- Skip next instruction if key with the value of Vx is not pressed.
                    else if (NN == 0xA1)
                    {
                        if (!Keyboard[V[X]])
                        {
                            PC += 2;
                        }
                    }
                }},

                // 0xF*** opcodes.
                {0xF, () =>
                {
                   int c = IR & 0x00FF;

                   switch (c)
                    {
                        // Fx07 - LD Vx, DT -- Set Vx = delay timer value.
                        case 0x07:
                            V[X] = DT;
                            break;
                        // Fx0A - LD Vx, K -- Wait for a key press, store the value of the key in Vx.
                        case 0x0A:
                            var pressedKeys = Keyboard.Where(k => k.Value);

                            while (pressedKeys.Count() <= 0)
                            {
                                // Poll until a key is pressed.
                                Task.Delay(_CLOCK_SPEED);
                            }

                            V[X] = pressedKeys.FirstOrDefault().Key;
                            break;
                        // Fx15 - LD DT, Vx -- Set delay timer = Vx.
                        case 0x15:
                            DT = V[X];
                            break;
                        // Fx18 - LD ST, Vx -- Set sound timer = Vx.
                        case 0x18:
                            ST = V[X];
                            break;
                        // Fx1E - ADD I, Vx -- Set I = I + Vx.
                        case 0x1E:
                            I = I + V[X];
                            break;
                        // Fx29 - LD F, Vx -- Set I = location of sprite for digit Vx.
                        case 0x29:
                            I = V[X] * 5;
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
                        // Fx55 - LD [I], Vx -- Store registers V0 through Vx in memory starting at location I.
                        case 0x55:
                            for (int i = 0; i <= X; i++)
                            {
                                _mem[I+i] = V[i];
                            }
                            break;
                        // Fx65 - LD Vx, [I] -- Read registers V0 through Vx from memory starting at location I.
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
            // Initialize keyboard.
            Keyboard = new ConcurrentDictionary<byte, bool>();
            for (byte i = 0x0; i < 0xF; i++)
            {
                Keyboard.GetOrAdd(i, false);
            }

            // Load sprites into memory.
            for (int i = 0; i < _sprites.Length; i++)
                _mem[i] = _sprites[i];

            // Set PC to the first instruction.
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

                // Start timers if they need to.
                if (DT > 0 && !DT_isRunning)
                {
                    DT_isRunning = true;
                    Task.Run(() =>
                    {
                        while(DT > 0)
                        {
                            DT--;
                            Task.Delay(17);
                        }
                        DT_isRunning = false;
                    });
                }

                if (ST > 0 && !ST_isRunning)
                {
                    ST_isRunning = true;
                    Task.Run(() =>
                        {
                            while (ST > 0)
                            {
                                ST--;
                                // Generate beep right here.
                                Task.Delay(17);
                            }
                            ST_isRunning = false;
                        });
                }

                System.Diagnostics.Debug.WriteLine("Executed {0:X}", IR);
                Task.Delay(_CLOCK_SPEED);
            } while (IR != 0x00);
        }

        public override string ToString()
        {
            string screen = "";

            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    screen += _g_mem[i, j] ? "*" : " ";
                }

                screen += "\n";
            }

            return screen;
        }
    }
}
