/*
 * Programmer: Ken Nguyen
 * File: AsysnSocketServer.cs
 * Project: WaMP Assignment 5
 * Date: November 3, 2013
 * Description: this file contain class AsynSocketServer
 */



using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;



namespace MessengerServer
{
    /// <summary>
    /// This class create an asynchronous server socket of type TCP/IP with some basic methods
    /// It's also has a dictionary which keeps track of all client connected
    /// </summary>
    class AsynSocketServer
    {
        private const int MAX_CLIENT = 100;
        public const int DEFAULT_PORT = 11000;

        private Socket listener;
        private Dictionary<Socket, string> clients;
        private Notifier notifier;      // provide a hook for notifying to frontend

        private IPAddress address { get; set; }
        private int port { get; set; }
        


        /// <summary>
        /// default constructor
        /// </summary>
        public AsynSocketServer(Notifier notifier = null)
        {
            if (notifier == null)
            {
                this.notifier = new ConsoleNotifier();
            }
            else
            {
                this.notifier = notifier;
            }

            clients = new Dictionary<Socket, string>();
            port = DEFAULT_PORT;

            // Create listener as socket of type TCP/IP
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }



        /// <summary>
        /// Get all available IP addresses on this machine
        /// </summary>
        public IPAddress[] GetIpAddressList()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            return ipHostInfo.AddressList;
        }



        /// <summary>
        /// Bind socket to a specified IP address of address list
        /// </summary>
        /// <param name="addressIndex">index of IP Address in the list</param>
        public void Start(IPAddress address)
        {
            // Bind socket
            this.address = address;
            IPEndPoint localEndPoint = new IPEndPoint(this.address, port);
            listener.Bind(localEndPoint);

            // Start listening
            listener.Listen(MAX_CLIENT);

            notifier.Notify("Server socket is listening on " + address + ":" + port);

            // begin accepting
            listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
        }



        /// <summary>
        /// recursive callback for accepting new connection
        /// </summary>
        /// <param name="ar">asynchronous status</param>
        private void AcceptCallback(IAsyncResult ar)
        {
            Socket listener = (Socket)ar.AsyncState;

            // Asynchronously accepts an incoming connection attempt and creates a new Socket to handle remote host communication
            Socket clientSocket = listener.EndAccept(ar);

            try
            {
                // Get user name from client
                Byte[] byteData = new Byte[128];
                clientSocket.Receive(byteData, 0, byteData.Length, 0);
                string userName = Encoding.Unicode.GetString(byteData);
                userName = userName.Remove(userName.LastIndexOf(StateObject.EOF));

                // track this client
                clients.Add(clientSocket, userName);

                // display message that new client connected
                notifier.Notify(userName + " (" + clientSocket.Handle + ") connected\n");

                // Create the state object.
                StateObject stateObject = new StateObject();
                stateObject.clientSocket = clientSocket;

                // for each client connection, create new asynchronous callback to receive data from that client            
                clientSocket.BeginReceive(stateObject.buffer, 0, StateObject.BUFFER_SIZE, 0, new AsyncCallback(ReadCallback), stateObject);
            }
            catch (Exception e)
            {
                notifier.Notify("Accept exception: " + e.Message);
            }

            // recursive this callback to wait for new connections from other clients
            listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
        }



        /// <summary>
        /// recursive callback for receiving data from client
        /// </summary>
        /// <param name="ar">asynchronous status</param>
        private void ReadCallback(IAsyncResult ar)
        {
            bool disconnect = false;
            String content = String.Empty;

            // Retrieve the state object and the handler socket from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket clientSocket = state.clientSocket;

            try
            {
                // ends a pending asynchronous read and know how many bytes have been received
                int bytesRead = clientSocket.EndReceive(ar);

                // interpret byte data to string
                // There  might be more data, so store the data received so far.
                state.message.Append(Encoding.Unicode.GetString(state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read more data.
                string s = state.message.ToString();
                if (s.LastIndexOf(StateObject.EOF) == s.Length - StateObject.EOF.Length)
                {
                    string message = clients[clientSocket] + " (" + clientSocket.Handle + "): " + s;

                    // All the data has been read from the client. Display it on the console.
                    notifier.Notify(message + "\n");

                    // Deliver the data to all clients.
                    foreach (KeyValuePair<Socket, string> client in clients)
                    {
                        // Convert the string data to byte data using Unicode encoding.
                        byte[] byteData = Encoding.Unicode.GetBytes(message);
                        client.Key.Send(byteData, 0, byteData.Length, 0);
                    }

                    state.message.Clear();   // reset string builder for receiving next message
                }
            }
            catch (Exception e)
            {
                // if there is any error happen to this client, disconnect it
                notifier.Notify("Read exception (" + clientSocket.Handle + "): " + e.Message + "\n");
                disconnect = true;
            }
            finally
            {
                if (disconnect == true)
                {
                    notifier.Notify(clients[clientSocket] + " at socket " + clientSocket.Handle + " has been closed.\n");

                    // remove handler of this client in the list
                    clients.Remove(clientSocket);
                }
                else
                {
                    // recur this callback
                    clientSocket.BeginReceive(state.buffer, 0, StateObject.BUFFER_SIZE, 0, new AsyncCallback(ReadCallback), state);
                }
            }
        }
    }
}
