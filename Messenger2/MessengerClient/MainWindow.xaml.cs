using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MessengerClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TcpClient client;

        public MainWindow()
        {
            InitializeComponent();

            // connect to server
            client = new TcpClient("192.168.0.102", 11000);
            Console.WriteLine("Connected to server");

            // send something to server
            NetworkStream ns = client.GetStream();
            byte[] bytes = Encoding.Unicode.GetBytes("hello<EOF>");
            ns.Write(bytes, 0, bytes.Length);

            // send hello message
        }
    }
}
