namespace MessengerServer
{
    partial class ServerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ipAddressList = new System.Windows.Forms.ListBox();
            this.createSocketBtn = new System.Windows.Forms.Button();
            this.ipAddressListLabel = new System.Windows.Forms.Label();
            this.notificationBoard = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // ipAddressList
            // 
            this.ipAddressList.FormattingEnabled = true;
            this.ipAddressList.Location = new System.Drawing.Point(12, 51);
            this.ipAddressList.Name = "ipAddressList";
            this.ipAddressList.ScrollAlwaysVisible = true;
            this.ipAddressList.Size = new System.Drawing.Size(174, 498);
            this.ipAddressList.TabIndex = 0;
            // 
            // createSocketBtn
            // 
            this.createSocketBtn.Location = new System.Drawing.Point(96, 17);
            this.createSocketBtn.Name = "createSocketBtn";
            this.createSocketBtn.Size = new System.Drawing.Size(90, 23);
            this.createSocketBtn.TabIndex = 1;
            this.createSocketBtn.Text = "Create Socket";
            this.createSocketBtn.UseVisualStyleBackColor = true;
            this.createSocketBtn.Click += new System.EventHandler(this.createSocketBtn_Click);
            // 
            // ipAddressListLabel
            // 
            this.ipAddressListLabel.AutoSize = true;
            this.ipAddressListLabel.Location = new System.Drawing.Point(12, 22);
            this.ipAddressListLabel.Name = "ipAddressListLabel";
            this.ipAddressListLabel.Size = new System.Drawing.Size(77, 13);
            this.ipAddressListLabel.TabIndex = 2;
            this.ipAddressListLabel.Text = "IP Address List";
            // 
            // notificationBoard
            // 
            this.notificationBoard.BackColor = System.Drawing.Color.White;
            this.notificationBoard.Location = new System.Drawing.Point(192, 12);
            this.notificationBoard.Multiline = true;
            this.notificationBoard.Name = "notificationBoard";
            this.notificationBoard.ReadOnly = true;
            this.notificationBoard.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.notificationBoard.Size = new System.Drawing.Size(580, 537);
            this.notificationBoard.TabIndex = 3;
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.notificationBoard);
            this.Controls.Add(this.ipAddressListLabel);
            this.Controls.Add(this.createSocketBtn);
            this.Controls.Add(this.ipAddressList);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 600);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "ServerForm";
            this.Text = "Server";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox ipAddressList;
        private System.Windows.Forms.Button createSocketBtn;
        private System.Windows.Forms.Label ipAddressListLabel;
        private System.Windows.Forms.TextBox notificationBoard;
    }
}

