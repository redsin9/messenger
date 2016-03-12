/*
 * Programmer: Ken Nguyen
 * File: StateObject.cs
 * Project: WaMP Assignment 5
 * Date: November 3, 2013
 * Description: This file contain class StateObject
 */



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;



namespace MessengerClient
{
    /// <summary>
    /// this class create an object to store data from receiving message
    /// </summary>
    class StateObject
    {
        // Client socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }
}
