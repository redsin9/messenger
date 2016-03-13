using SharedCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MessengerServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotify
    {
        Server server;

        public MainWindow()
        {
            InitializeComponent();

            // setup server
            server = new Server();
            server.SetNotifier(this);

            try
            {
                server.Start();
            }
            catch (Exception e)
            {
                Notify("Failed to start server. " + e.Message);
            }
        }

        public void Notify(string notification)
        {
            Dispatcher.Invoke(delegate
            {
                notificationBoard.AppendText(notification + "\n");
            });
        }
    }
}
