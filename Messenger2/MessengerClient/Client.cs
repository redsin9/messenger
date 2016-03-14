using SharedCode;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MessengerClient
{
    class Client
    {
        private const int BUFFER_SIZE = 1024;   // KB

        private TcpClient handler;
        private volatile INotify notifier;
        private volatile bool isListening = true;



        public Client()
        {
            notifier = new ConsoleNotifier();
        }

        public void SetNotifier(INotify notifier)
        {
            if (notifier != null)
            {
                this.notifier = notifier;
            }
        }



        public void Start()
        {
            Thread thread = new Thread(ConnectServerThread);
            thread.IsBackground = true;
            thread.Start();
        }



        private void ConnectServerThread()
        {
            // automatically receive server address from UDP broadcasting
            int port = NetworkProtocol.DEFAULT_PORT;
            UdpClient listener = new UdpClient(port);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
            notifier.Notify("Waiting for server to broadcast its address...");
            byte[] bytes = listener.Receive(ref endPoint);
            listener.Close();

            // connect to server
            notifier.Notify("Connecting to server...");
            handler = new TcpClient(Encoding.ASCII.GetString(bytes, 0, bytes.Length), port);

            // listening for incoming messages
            notifier.Notify("Client has connected to server successfully.");
            byte[] buffer = new byte[BUFFER_SIZE];
            NetworkStream stream = handler.GetStream();
            while (isListening)
            {
                string message = NetworkProtocol.ReceiveMessage(stream);
                notifier.Notify(message);
            }
        }



        public void SendMessage(string message)
        {
            NetworkProtocol.SendMessage(handler.GetStream(), message);
        }
    }
}
