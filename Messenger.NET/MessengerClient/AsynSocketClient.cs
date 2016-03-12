/*
 * Programmer: Ken Nguyen
 * File: AsysnSocketClient.cs
 * Project: WaMP Assignment 5
 * Date: November 3, 2013
 * Description: this file contain class AsynSocketClient
 */



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;



namespace MessengerClient
{
    /// <summary>
    /// This class create an asynchronous client socket of type TCP/IP with some basic methods
    /// </summary>
    class AsynSocketClient
    {
        public Socket Connecter { get; set; }
        public IPAddress IpAddress { get; set; }
        public int Port { get; set; }
        


        /// <summary>
        /// default constructor
        /// </summary>
        public AsynSocketClient()
        {
            IpAddress = null;
            Port = 11000;       // default port

            // Create client socket of type TCP/IP
            Connecter = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }        



        /// <summary>
        /// Connect to a specified IP address and port
        /// </summary>
        public void Connect()
        {
            Connecter.Connect(IpAddress, Port);
        }



        /// <summary>
        /// Send message to server with "<EOF>" at the end of message
        /// </summary>
        /// <param name="message">message to be sent</param>
        public void Send(string message)
        {
            Byte[] byteData = Encoding.Unicode.GetBytes(message + "<EOF>");
            Connecter.Send(byteData, 0, byteData.Length, 0);
        }
    }
}
