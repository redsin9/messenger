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
            SendMessage();
        }

        private void sendTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                SendMessage();
            }
        }

        private void SendMessage()
        {
            string message = sendTextBox.Text.Trim('\n', '\t', ' ');
            if (message != string.Empty)
            {
                client.SendMessage(sendTextBox.Text, (bool)encryptCheckBox.IsChecked);
                sendTextBox.Clear();
            }
        }
    }
}
