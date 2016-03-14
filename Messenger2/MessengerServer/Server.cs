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
        private const int BUFFER_SIZE = 1024;
        private const int DEFAULT_SLEEP = 100;  // millisecond
        private const int BOARDCAST_SPAN = 3000;

        private IPAddress address;
        private int port;
        private TcpListener listener;
        private List<TcpClient> clients = new List<TcpClient>();

        private volatile INotify notifier;
        private volatile bool isAccepting = true;
        private volatile bool isTransmitting = false;
        private volatile bool isBroadcasting = true;



        public Server(int port = NetworkProtocol.DEFAULT_PORT, INotify notifier = null)
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
            Thread acceptClientsThread = new Thread(AcceptClientsThread);
            acceptClientsThread.IsBackground = true;
            acceptClientsThread.Start();

            // create new thread broadcasting server address
            Thread broadcastAddressThread = new Thread(BroadcastAddressThread);
            acceptClientsThread.IsBackground = true;
            broadcastAddressThread.Start();
        }



        private void AcceptClientsThread()
        {
            while (isAccepting)
            {
                TcpClient client = listener.AcceptTcpClient();
                clients.Add(client);

                notifier.Notify(String.Format("New connection from {0}", client.Client.RemoteEndPoint));

                // start new thread for receiving data from the client
                Thread thread = new Thread(() => ReceiveMessageThread(client));
                thread.IsBackground = true;
                thread.Start();
            }
        }



        private void BroadcastAddressThread()
        {
            // create broadcast end point with format xxx.xxx.xxx.255
            string addressString = address.ToString();
            IPAddress submask = IPAddress.Parse(addressString.Substring(0, addressString.LastIndexOf('.') + 1) + "255");
            IPEndPoint target = new IPEndPoint(submask, port);

            // create socket for broadcasting data
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            byte[] data = Encoding.ASCII.GetBytes(address.ToString());

            notifier.Notify("Broadcasting server address to " + target.ToString());
            while (isBroadcasting)
            {
                socket.SendTo(data, target);
                Thread.Sleep(BOARDCAST_SPAN);
            }
        }



        private void ReceiveMessageThread(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[BUFFER_SIZE];
            StringBuilder sb = new StringBuilder();

            try
            {
                while (true)
                {
                    // blocking call
                    string message = NetworkProtocol.ReceiveMessage(stream);
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
                byte[] buffer = NetworkProtocol.MESSAGE_ENCODE.GetBytes(message);
                client.GetStream().Write(buffer, 0, buffer.Length);
            }
        }
    }
}
