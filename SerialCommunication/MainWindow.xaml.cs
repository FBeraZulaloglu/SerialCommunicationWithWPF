using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SerialCommunication
{
    /// <summary>
    /// MainWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort _serialPort;
        bool _isSerialOpen;
        

        public MainWindow()
        {
            InitializeComponent();
            _serialPort = new SerialPort();
        }

        #region open Serial Port
        private void openSerial_Click(object sender, RoutedEventArgs e)
        {
            OpenSerialPort();
        }

        public void OpenSerialPort()
        {
            if(SetPortName() == null || SetPortBaudRate() == -1)
            {
                MessageBox.Show("Gerekli bilgileri seçtikten sonra tekrar deneyiniz.");
            }
            else
            {
                if (_isSerialOpen == false)
                {
                    _serialPort.PortName = SetPortName();
                    _serialPort.BaudRate = SetPortBaudRate();
                    _serialPort.Parity = Parity.None;
                    _serialPort.DataBits = 7;
                    _serialPort.StopBits = StopBits.One;
                    _serialPort.Handshake = Handshake.RequestToSendXOnXOff;
                    _serialPort.Encoding = ASCIIEncoding.ASCII;
                    _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

                    //Seri porttan okunan veya seri porta yazılan verilerin zaman aşımı sürelerini belirler
                    _serialPort.ReadTimeout = 500;
                    _serialPort.WriteTimeout = 500;
                    try
                    {
                        _serialPort.Open();
                        _isSerialOpen = true;
                        isConnected.IsChecked = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Seri Port Açılamadı!");
                        Console.WriteLine("SERİ PORT HATASI " + ex.ToString());
                    }
                }
                else
                {
                    MessageBox.Show("Seri portunuzu kapattıktan sonra tekrar deneyin.");
                }
            }
            
        }
        #endregion

        #region When new data has come from serial port
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            // Seri porta gelen verileri okuma işlemi
            SerialPort sp = (SerialPort)sender;
            String s = sp.ReadExisting();

            // Veriyi grafik arayüz threadıinde değiştir.
            Application.Current.Dispatcher.Invoke(new Action(() => {
                // Arayüzdeki elemanlara bu verileri aktar
                dataCenter.Focus(); // to make sure that richtextbox will scroll every time
                dataCenter.AppendText(s);
                dataCenter.ScrollToEnd();
            }));
        }
        #endregion

        #region Port Name
        public string SetPortName()
        {
            if(ports.SelectedItem != null)
            {
                return ports.SelectedItem.ToString();
            }
            return null;
        }

        private void ports_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // comboboxtan bir com seçimi oldu
            SetPortName();
        }

        private void PortsMouseEnter(object sender, MouseEventArgs e)
        {
            List<string> portNames = new List<string>();

            Console.WriteLine("Available Ports:");
            foreach (string s in SerialPort.GetPortNames())
            {
                portNames.Add(s);
                Console.WriteLine("   {0}", s);
            }
            ports.ItemsSource = portNames;
            // comboboxa bu elemanları ekle
        }

       
        #endregion

        #region BaudRate
        // Display BaudRate values and prompt user to enter a value.
        public int SetPortBaudRate()
        {
            string baudRate;

            if (baudRateCombo.SelectedItem != null)
            {
                ComboBoxItem cbi = (ComboBoxItem)baudRateCombo.SelectedItem;
                baudRate = cbi.Content.ToString();
                Console.WriteLine(baudRate);
                return int.Parse(baudRate);
            }
            
            return -1;
            
        }

        private void baudRate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // baud rate seçildi
            SetPortBaudRate();
        }

        #endregion

        #region close Serial Port
        private void closeSerial_Click(object sender, RoutedEventArgs e)
        {
            if (_isSerialOpen)
            {
                _serialPort.Close();
                _isSerialOpen = false;
                isConnected.IsChecked = false;
            }
        }
        #endregion

        

       
    }
}
