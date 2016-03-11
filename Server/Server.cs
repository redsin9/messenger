using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class TcpServer
    {
        private static IPAddress ADDRESS = IPAddress.Parse("127.0.0.1");
        private static Int32 PORT = 13000;
        private static int BUFFER_SIZE = 256;



        private TcpListener listener;
        private List<TcpClient> clients;
        private Byte[] buffer;



        public TcpServer()
        {
            listener = new TcpListener(ADDRESS, PORT);
            clients = new List<TcpClient>();
            buffer = new Byte[BUFFER_SIZE];
        }



        public void Start()
        {
            try
            {
                listener.Start();
                Console.WriteLine("Server has been started.");
            }
            catch (SocketException e)
            {
                Console.WriteLine("Failed to start server. Error: {0}", e.Message);
                return;
            }

            // TODO put this loop to a separated thread
            // Enter the listening loop
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                clients.Add(client);
                String clientAddress = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                Console.WriteLine("Connected to client {0}", clientAddress);
            }
        }
    }
}
