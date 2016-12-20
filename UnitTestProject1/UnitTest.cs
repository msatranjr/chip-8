using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Chip_8.Chip_8_Emulator;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void

        [TestMethod]
        public void TestInstructions()
        {
            // Test LD Vx, byte and ADD Vx, byte
            CPU cpu = new CPU();
            cpu.LDV(0, 0x0F);
            Assert.AreEqual(0x0F, cpu.V[0]);

            cpu.ADDV(0, 0x01);

            Assert.AreEqual(0x10, cpu.V[0]);
            Assert.AreEqual(0x00, cpu.VF);
        }

        [TestMethod]
        public void TestSubroutines()
        {
            CPU cpu = new CPU();

        }

        [TestMethod]
        public void TestSprites()
        {
            CPU cpu = new CPU();
            string[] expected = new string[] {

"11110000\n" +
"10010000\n" +
"10010000\n" +
"10010000\n" +
"11110000\n"
,
"00100000\n" +
"01100000\n" +
"00100000\n" +
"00100000\n" +
"01110000\n"
,
"11110000\n" +
"00010000\n" +
"11110000\n" +
"10000000\n" +
"11110000\n"
,
"11110000\n" +
"00010000\n" +
"11110000\n" +
"00010000\n" +
"11110000\n"
,
"10010000\n" +
"10010000\n" +
"11110000\n" +
"00010000\n" +
"00010000\n"
,
"11110000\n" +
"10000000\n" +
"11110000\n" +
"00010000\n" +
"11110000\n"
,
"11110000\n" +
"10000000\n" +
"11110000\n" +
"10010000\n" +
"11110000\n"
,
"11110000\n" +
"00010000\n" +
"00100000\n" +
"01000000\n" +
"01000000\n"
,
"11110000\n" +
"10010000\n" +
"11110000\n" +
"10010000\n" +
"11110000\n"
,
"11110000\n" +
"10010000\n" +
"11110000\n" +
"00010000\n" +
"11110000\n"
,
"11110000\n" +
"10010000\n" +
"11110000\n" +
"10010000\n" +
"10010000\n"
,
"11100000\n" +
"10010000\n" +
"11100000\n" +
"10010000\n" +
"11100000\n"
,
"11110000\n" +
"10000000\n" +
"10000000\n" +
"10000000\n" +
"11110000\n"
,
"11100000\n" +
"10010000\n" +
"10010000\n" +
"10010000\n" +
"11100000\n"
,
"11110000\n" +
"10000000\n" +
"11110000\n" +
"10000000\n" +
"11110000\n"
,
"11110000\n" +
"10000000\n" +
"11110000\n" +
"10000000\n" +
"10000000\n"

            };

            for (int i = 0; i < 16; i++)
            {
                string actual = "";
                for (int j = 0; j < 5; j++)
                {
                    for (int k = 0; k < 8; k++)
                        actual += (0x80 & (cpu._sprites[i,j] << k)) >> 7;
                    actual += "\n";
                }

                Assert.AreEqual(expected[i], actual);
            }

        }
    }
}
