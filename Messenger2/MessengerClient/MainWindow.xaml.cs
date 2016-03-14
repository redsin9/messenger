using SharedCode;
using System;
using System.Windows;

namespace MessengerClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ILogger
    {
        Client client;

        public MainWindow()
        {
            InitializeComponent();

            client = new Client();
            client.Logger(this);
            try
            {
                client.Start();
            }
            catch (Exception e)
            {
                Log("Failed to start client. " + e.Message);
            }
        }

        public void Log(string message)
        {
            Dispatcher.Invoke(delegate { notificationBoard.AppendText(message + "\n"); });
        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            client.SendMessage(sendTextBox.Text);
            sendTextBox.Clear();
        }
    }
}
