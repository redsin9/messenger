/*
 * Programmer: Ken Nguyen
 * File: StateObject.cs
 * Project: WaMP Assignment 5
 * Date: November 3, 2013
 * Description: This file contain class StateObject
 */



using System.Text;
using System.Net.Sockets;



namespace MessengerServer
{
    /// <summary>
    /// This class creates an object to store data from received message
    /// </summary>
    class StateObject
    {
        public const int BUFFER_SIZE = 1024;
        public const string EOF = "<EOF>";

        public Socket clientSocket = null;
        public byte[] buffer = new byte[BUFFER_SIZE];
        public StringBuilder message = new StringBuilder();
    }
}
