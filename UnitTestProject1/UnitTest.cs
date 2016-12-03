﻿using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Chip_8.Chip_8_Emulator;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
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
                foreach (byte b in cpu._sprites[i])
                {
                    for (int j = 0; j < 8; j++)
                        actual += (0x80 & (b << j)) >> 7;
                    actual += "\n";
                }

                Assert.AreEqual(expected[i], actual);
            }
            
        }
    }
}