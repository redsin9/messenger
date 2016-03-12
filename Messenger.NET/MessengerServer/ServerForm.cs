/*
 * Programmer: Ken Nguyen
 * File: ServerForm.cs
 * Project: WaMP Assignment 5
 * Date: November 3, 2013
 * Description: Source code for server forms
 */



using System;
using System.Net;
using System.Windows.Forms;



namespace MessengerServer
{
    public partial class ServerForm : Form, Notifier
    {
        private IPAddress[] addresses;
        private AsynSocketServer server;



        /// <summary>
        /// default constructor
        /// </summary>
        public ServerForm()
        {
            InitializeComponent();
            server = new AsynSocketServer(this);

            // get all availale IP addresses
            addresses = server.GetIpAddressList();

            // show list of ip addresses in the listbox
            ipAddressList.DataSource = addresses;
        }



        /// <summary>
        /// delegate for setting notification
        /// </summary>
        /// <param name="notification">string to be set</param>
        private delegate void DelegateSetNotification(string notification);

        

        /// <summary>
        /// write notification on the notification board of server
        /// this is used accross multiple thread in a safe manner
        /// </summary>
        /// <param name="notification">string would be written on the board</param>
        public void Notify(string notification)
        {
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true. 
            if (notificationBoard.InvokeRequired)
            {
                DelegateSetNotification d = new DelegateSetNotification(Notify);
                this.Invoke(d, new object[] { notification });
            }
            else
            {
                notificationBoard.AppendText(notification + "\n");
            }
        }



        /// <summary>
        /// Bind server socket to selected IP Address
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createSocketBtn_Click(object sender, EventArgs e)
        {
            try
            {
                server.Start(addresses[ipAddressList.SelectedIndex]);

                // if server started successful, notify user and hide button, disable listbox
                createSocketBtn.Visible = false;
                ipAddressList.Enabled = false;
            }
            catch (Exception ex)
            {
                Notify("Exception: " + ex.Message + "\n");
            }
        }
    }
}
