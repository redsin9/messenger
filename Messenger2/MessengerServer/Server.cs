using SharedCode;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MessengerServer
{
    class Server
    {
        public const int DEFAULT_PORT = 11000;
        private const int BUFFER_SIZE = 1024;
        private const int DEFAULT_SLEEP = 100;  // millisecond
        private const string EOF = "<EOF>";

        private IPAddress address;
        private int port;
        private TcpListener listener;
        private List<TcpClient> clients = new List<TcpClient>();

        private volatile INotify notifier;
        private volatile bool isAccepting = true;
        private volatile bool isTransmitting = false;



        public Server(int port = DEFAULT_PORT, INotify notifier = null)
        {
            // find the first IPv4 address
            foreach (IPAddress address in Dns.GetHostEntry(string.Empty).AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    this.address = address;
                    break;
                }
            }

            // stop if there is not any IPv4 address
            if (address == null)
            {
                throw new Exception("Unable to find any IPv4 address");
            }

            // set port number (default/custom)
            this.port = port;

            // setup listener
            listener = new TcpListener(address, port);

            // default notifier output to Console
            if (notifier == null)
            {
                notifier = new ConsoleNotifier();
            }
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
            // start a thread listening for new connections
            listener.Start();
            notifier.Notify(String.Format("Server has started on {0}:{1}", address, port));

            // create new thread accepting new connections
            Thread thread = new Thread(AcceptThread);
            thread.IsBackground = true;
            thread.Start();
        }



        private void AcceptThread()
        {
            while (isAccepting)
            {
                TcpClient client = listener.AcceptTcpClient();
                clients.Add(client);

                notifier.Notify(String.Format("New connection from {0}", client.Client.RemoteEndPoint));

                // start new thread for receiving data from the client
                Thread thread = new Thread(() => ReadThread(client));
                thread.IsBackground = true;
                thread.Start();
            }
        }



        private void ReadThread(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[BUFFER_SIZE];
            StringBuilder sb = new StringBuilder();

            try
            {
                while (true)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    sb.Append(Encoding.Unicode.GetString(buffer, 0, bytesRead));
                    string message = sb.ToString();

                    // if finds the EOF signal at the end of the message
                    // the full message has been received, broadcast it to all other clients
                    if (message.LastIndexOf(EOF) == message.Length - EOF.Length)
                    {
                        notifier.Notify(String.Format("{0} {1}", client.Client.RemoteEndPoint, message));

                        // TODO implement MUTEX lock later

                        // wait until other thread stop transmitting the message
                        while (isTransmitting)
                        {
                            Thread.Sleep(DEFAULT_SLEEP);
                        }

                        // lock transmit permission
                        isTransmitting = true;

                        BroadcastMessage(client, message);

                        // release transmit permission
                        isTransmitting = false;

                        // reset string builder for next message
                        sb.Clear();
                    }
                }
            }
            catch (Exception e)
            {
                notifier.Notify(String.Format("[ERROR] {0}. {1}", client.Client.RemoteEndPoint, e.Message));
            }
        }



        private void BroadcastMessage(TcpClient sender, string message)
        {
            foreach (TcpClient client in clients)
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = Encoding.Unicode.GetBytes(String.Format("{0} {1}", sender.Client.RemoteEndPoint, message));
                stream.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
