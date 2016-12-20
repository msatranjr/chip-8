using Chip_8.Chip_8_Emulator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Chip_8
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        CPU cpu;
        public MainPage()
        {
            this.InitializeComponent();
            cpu = new CPU();
            cpu.Load(new byte[] {
            0xD0, 0x05,
            });
            cpu.Start();

            //cpu._g_mem[1, 0] = 0x1;

            var screen = cpu.ToString();
            System.Diagnostics.Debug.Write(screen);
        }
    }
}
