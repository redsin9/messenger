using SharedCode;
using System;
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
        private volatile ILogger logger;
        private volatile bool isListening = true;



        public Client()
        {
            logger = new ConsoleNotifier();
        }

        public void Logger(ILogger logger)
        {
            if (logger != null)
            {
                this.logger = logger;
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
            while (true)
            {
                int port = NetworkProtocol.DEFAULT_PORT;
                UdpClient listener = new UdpClient(port);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
                logger.Log("Waiting for server to broadcast its address...");
                byte[] bytes = listener.Receive(ref endPoint);

                // decrypt the message
                string decryptedAddress = StringCipher.Decrypt(Encoding.ASCII.GetString(bytes, 0, bytes.Length), NetworkProtocol.PASS_PHRASE);

                // make sure this is a valid ip address
                try
                {
                    IPAddress serverAddress = IPAddress.Parse(decryptedAddress);
                }
                catch (Exception)
                {
                    logger.Log("Received unknown UDP message: " + decryptedAddress);
                    continue;
                }

                // connect to server if received a valid server address
                logger.Log("Connecting to server address " + decryptedAddress);
                handler = new TcpClient(decryptedAddress, port);
                listener.Close();
                break;
            }

            

            // listening for incoming messages
            logger.Log("Client has connected to server successfully.");
            byte[] buffer = new byte[BUFFER_SIZE];
            NetworkStream stream = handler.GetStream();
            while (isListening)
            {
                string message = NetworkProtocol.ReceiveMessage(stream);
                logger.Log(message);
            }
        }



        public void SendMessage(string message)
        {
            NetworkProtocol.SendMessage(handler.GetStream(), message);
        }
    }
}
