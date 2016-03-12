/*
 * Programmer: Ken Nguyen
 * File: ClientForm.cs
 * Project: WaMP Assignment 5
 * Date: November 3, 2013
 * Description: Source code for client forms
 */



using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;



namespace MessengerClient
{
    public partial class ClientForm : Form
    {
        private AsynSocketClient clientSocket;



        /// <summary>
        /// default constructor
        /// </summary>
        public ClientForm()
        {
            InitializeComponent();
            clientSocket = new AsynSocketClient();

            // display default port
            portBox.Text = clientSocket.Port.ToString();
        }



        /// <summary>
        /// delegate for setting notification
        /// </summary>
        /// <param name="notification">string to be set</param>
        private delegate void DelegateSetNotification(string notification);



        /// <summary>
        /// write notification on the display box of client
        /// this is used accross multiple thread in a safe manner
        /// </summary>
        /// <param name="notification">string would be written on the board</param>
        private void SetNotification(string notification)
        {
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true. 
            if (displayBox.InvokeRequired)
            {
                DelegateSetNotification d = new DelegateSetNotification(SetNotification);
                this.Invoke(d, new object[] { notification });
            }
            else
            {
                displayBox.AppendText(notification);
            }
        }



        /// <summary>
        /// Use regex to validate user name
        /// </summary>
        /// <param name="userName">user name to be validated</param>
        /// <returns>true is name is valid and false otherwise</returns>
        private bool ValidateUserName(string userName)
        {
            bool retCode = true;
            string pattern = @"^\w( \w)*";
            Regex regex = new Regex(pattern);

            if (regex.IsMatch(userName) == false)
            {
                retCode = false;
            }

            return retCode;
        }



        /// <summary>
        /// validate IP address
        /// </summary>
        /// <param name="ipAddress">IP address to be validated</param>
        /// <returns>true if IP address is valid and false otherwise</returns>
        private bool ValidateIpAddress(string ipAddress)
        {
            bool retCode = true;

            string pattern = @"^\d{1,3}(.\d{1,3}){3}$";
            Regex regex = new Regex(pattern);

            // check the syntax
            if (regex.IsMatch(ipAddress) == false)
            {
                retCode = false;
            }
            else
            {
                string[] sections = ipAddress.Split(new char[] {'.'});

                // check the range of each section in IP address
                foreach (string section in sections)
                {
                    int convertedSection = Convert.ToInt32(section);

                    if (convertedSection < 0 || convertedSection > 255)
                    {
                        retCode = false;
                        break;
                    }
                }
            }

            return retCode;
        }



        /// <summary>
        /// validate port number
        /// </summary>
        /// <param name="port">port number would be validate</param>
        /// <param name="minRange">min range of port number</param>
        /// <param name="maxRange">max range of port number</param>
        /// <returns>true if port number if valid and false otherwise</returns>
        private bool ValidatePort(string port, int minRange = 1, int maxRange = 65535)
        {
            bool retCode = true;
            string pattern = @"^\d{1,5}$";
            Regex regex = new Regex(pattern);

            // check the syntax
            if (regex.IsMatch(port) == false)
            {
                retCode = false;
            }
            else
            {
                int convertedPort = Convert.ToInt32(port);

                // check the range
                if (convertedPort > maxRange || convertedPort < minRange)
                {
                    retCode = false;
                }
            }

            return retCode;
        }



        /// <summary>
        /// connect to specified IP address with Port
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectBtn_Click(object sender, EventArgs e)
        {
            bool validation = true;

            // validate user name
            if (ValidateUserName(nameBox.Text) == false)
            {
                validation = false;
                SetNotification("User Name is invalid.\n");
            }

            // validate IP address
            if (ValidateIpAddress(ipAddressBox.Text) == false)
            {
                validation = false;
                SetNotification("IP Address is invalid.\n");
            }

            // validate port number
            if (ValidatePort(portBox.Text) == false)
            {
                validation = false;
                SetNotification("Port number is invalid.\n");
            }

            if (validation == true)
            {
                // once IP address and port is valid, start connecting to server
                try
                {
                    clientSocket.IpAddress = IPAddress.Parse(ipAddressBox.Text);
                    clientSocket.Port = Convert.ToInt32(portBox.Text);
                    clientSocket.Connect();
                    SetNotification("Connected to IP Address " + ipAddressBox.Text + " : " + portBox.Text + "\n");

                    // disable register once connected to server
                    nameBox.Enabled = false;
                    ipAddressBox.Enabled = false;
                    portBox.Enabled = false;
                    connectBtn.Visible = false;
                    sendBox.Enabled = true;
                    sendBtn.Enabled = true;
                    this.Text += " - " + nameBox.Text;

                    // send the username to server
                    clientSocket.Send(nameBox.Text);

                    // create a recursive callback for receiving message from server
                    StateObject state = new StateObject();
                    state.workSocket = clientSocket.Connecter;
                    clientSocket.Connecter.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                }
                catch (Exception ex)
                {
                    SetNotification("Exception: " + ex.Message + "\n");
                }
            }
        }



        /// <summary>
        /// callback function for receiving message from client
        /// </summary>
        /// <param name="ar">asynchronous status</param>
        private void ReceiveCallback(IAsyncResult ar)
        {
            bool receiveSuccess = true;

            // Retrieve the state object and the client socket 
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket client = state.workSocket;

            try
            {
                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);

                // There  might be more data, so store the data received so far.
                state.sb.Append(Encoding.Unicode.GetString(state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read more data.
                int indexOfEOF = state.sb.ToString().IndexOf("<EOF>");
                if (indexOfEOF > -1)
                {
                    // All the data has been read from the client. Display it on the console.
                    state.sb.Remove(indexOfEOF, 5);     // remove <EOF> at the end of string
                    SetNotification(state.sb.ToString() + "\n");
                    state.sb.Clear();   // reset string builder for receiving next message
                }                
            }
            catch (Exception e)
            {
                SetNotification("Exception: " + e.Message + "\n");
                receiveSuccess = false;
            }
            finally
            {
                if (receiveSuccess == true)
                {
                    // recursive this callback
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // re-enable register process again
                    MessageBox.Show("Server has been disconnected.\nPress OK to exit program.");
                    Application.Exit();
                }
            }
        }



        /// <summary>
        /// send message from sendBox to server when user click sendBtn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sendBtn_Click(object sender, EventArgs e)
        {
            // check if there is '\n' at the end of string then delete it
            if (sendBox.Text.IndexOf('\n') > -1)
            {
                sendBox.Text = sendBox.Text.Remove(sendBox.Text.IndexOf('\n'));
            }

            try
            {
                clientSocket.Send(sendBox.Text);
            }
            catch (Exception ex)
            {
                SetNotification("Exception: " + ex.Message + "\n");
            }
            finally
            {
                sendBox.Text = "";      // reset send textbox
            }
        }



        /// <summary>
        /// check if a Enter key is up, send message to server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sendBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                sendBtn_Click(sender, e);
            }
        }
    }
}
