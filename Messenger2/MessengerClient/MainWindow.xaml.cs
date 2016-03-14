using SharedCode;
using System;
using System.Windows;

namespace MessengerClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotify
    {
        Client client;

        public MainWindow()
        {
            InitializeComponent();

            client = new Client();
            client.SetNotifier(this);
            try
            {
                client.Start();
            }
            catch (Exception e)
            {
                Notify("Failed to start client. " + e.Message);
            }
        }

        public void Notify(string notification)
        {
            Dispatcher.Invoke(delegate { notificationBoard.AppendText(notification + "\n"); });
        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            client.SendMessage(sendTextBox.Text);
            sendTextBox.Clear();
        }
    }
}
