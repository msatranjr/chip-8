using Chip_8.Chip_8_Emulator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Chip_8
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Rectangle[,] Pixels = new Rectangle[32, 64];

        public static async Task CallOnUiThreadAsync(DispatchedHandler handler) =>
    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
        CoreDispatcherPriority.Normal, handler);

        CPU cpu;
        public MainPage()
        {
            this.InitializeComponent();

            k0.AddHandler(PointerPressedEvent, new PointerEventHandler(OnPointerPressed), true);
            k0.AddHandler(PointerReleasedEvent, new PointerEventHandler(OnPointerReleased), true);
            k1.AddHandler(PointerPressedEvent, new PointerEventHandler(OnPointerPressed), true);
            k1.AddHandler(PointerReleasedEvent, new PointerEventHandler(OnPointerReleased), true);
            k2.AddHandler(PointerPressedEvent, new PointerEventHandler(OnPointerPressed), true);
            k2.AddHandler(PointerReleasedEvent, new PointerEventHandler(OnPointerReleased), true);
            k3.AddHandler(PointerPressedEvent, new PointerEventHandler(OnPointerPressed), true);
            k3.AddHandler(PointerReleasedEvent, new PointerEventHandler(OnPointerReleased), true);
            k4.AddHandler(PointerPressedEvent, new PointerEventHandler(OnPointerPressed), true);
            k4.AddHandler(PointerReleasedEvent, new PointerEventHandler(OnPointerReleased), true);
            k5.AddHandler(PointerPressedEvent, new PointerEventHandler(OnPointerPressed), true);
            k5.AddHandler(PointerReleasedEvent, new PointerEventHandler(OnPointerReleased), true);
            k6.AddHandler(PointerPressedEvent, new PointerEventHandler(OnPointerPressed), true);
            k6.AddHandler(PointerReleasedEvent, new PointerEventHandler(OnPointerReleased), true);
            k7.AddHandler(PointerPressedEvent, new PointerEventHandler(OnPointerPressed), true);
            k7.AddHandler(PointerReleasedEvent, new PointerEventHandler(OnPointerReleased), true);
            k8.AddHandler(PointerPressedEvent, new PointerEventHandler(OnPointerPressed), true);
            k8.AddHandler(PointerReleasedEvent, new PointerEventHandler(OnPointerReleased), true);
            k9.AddHandler(PointerPressedEvent, new PointerEventHandler(OnPointerPressed), true);
            k9.AddHandler(PointerReleasedEvent, new PointerEventHandler(OnPointerReleased), true);
            ka.AddHandler(PointerPressedEvent, new PointerEventHandler(OnPointerPressed), true);
            ka.AddHandler(PointerReleasedEvent, new PointerEventHandler(OnPointerReleased), true);
            kb.AddHandler(PointerPressedEvent, new PointerEventHandler(OnPointerPressed), true);
            kb.AddHandler(PointerReleasedEvent, new PointerEventHandler(OnPointerReleased), true);
            kc.AddHandler(PointerPressedEvent, new PointerEventHandler(OnPointerPressed), true);
            kc.AddHandler(PointerReleasedEvent, new PointerEventHandler(OnPointerReleased), true);
            kd.AddHandler(PointerPressedEvent, new PointerEventHandler(OnPointerPressed), true);
            kd.AddHandler(PointerReleasedEvent, new PointerEventHandler(OnPointerReleased), true);
            ke.AddHandler(PointerPressedEvent, new PointerEventHandler(OnPointerPressed), true);
            ke.AddHandler(PointerReleasedEvent, new PointerEventHandler(OnPointerReleased), true);
            kf.AddHandler(PointerPressedEvent, new PointerEventHandler(OnPointerPressed), true);
            kf.AddHandler(PointerReleasedEvent, new PointerEventHandler(OnPointerReleased), true);

            cpu = new CPU();

            cpu.ClearPixels += async (s, e) =>
            {
                await CallOnUiThreadAsync(() =>
                {
                    Screen.Children.Clear();
                });
            };

            cpu.SetPixels += async (s, e) =>
            {
                await CallOnUiThreadAsync(() =>
                {
                    foreach (Pixel p in e.Pixels)
                    {
                        DrawScreen(p.x, p.y, p.on);
                    }
                    
                });
            };

        }

        private async void LoadRom()
        {
            var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var file = await folder.GetFileAsync(@"Assets\PONG2");
            var properties = await file.GetBasicPropertiesAsync();
            byte[] instructions = new byte[properties.Size];

            var buffer = await Windows.Storage.FileIO.ReadBufferAsync(file);

            using (var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(buffer))
            {
                dataReader.ReadBytes(instructions);
            }

            cpu.Load(instructions);
            await Task.Run(() => cpu.Start());
        }

        private void DrawScreen(int x, int y, bool isOn)
        {
            if (isOn)
            {
                var rect = new Rectangle();
                rect.Fill = new SolidColorBrush(Windows.UI.Colors.White);
                rect.Width = 10;
                rect.Height = 10;
                Screen.Children.Add(rect);
                Canvas.SetLeft(rect, x * 10);
                Canvas.SetTop(rect, y * 10);
                Pixels[y, x] = rect;
            }
            else
            {
                Screen.Children.Remove(Pixels[y, x]);
            }

        }

        private int GetKeyValue(object sender)
        {
            int value = 0;
            char key = (sender as Button).Content.ToString().ToCharArray()[0];

            if (key <= 0x39)
                value = key - 0x30;
            else
                value = key - 0x41 + 10;

            return value;
        }

        private char GetKeyName(int value)
        {
            if (value <= 9)
            {
                return (char)(value + 0x30);
            }
            else
            {
                return (char)(value + 0x41);
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            cpu.Keyboard[(byte)GetKeyValue(sender)] = true;
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            cpu.Keyboard[(byte)GetKeyValue(sender)] = false;
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            LoadRom();
        }
    }
}
