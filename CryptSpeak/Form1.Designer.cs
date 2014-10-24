namespace CryptSpeak.UI
{
    partial class UIMessenger
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
            this.tbYourIP = new System.Windows.Forms.TextBox();
            this.tbEndIP = new System.Windows.Forms.TextBox();
            this.tbYourPort = new System.Windows.Forms.TextBox();
            this.tbEndPort = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.tbToSend = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.tbKeyFile = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lbMessages = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // tbYourIP
            // 
            this.tbYourIP.Location = new System.Drawing.Point(60, 5);
            this.tbYourIP.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbYourIP.Name = "tbYourIP";
            this.tbYourIP.Size = new System.Drawing.Size(105, 20);
            this.tbYourIP.TabIndex = 0;
            this.tbYourIP.Text = "127.0.0.1";
            // 
            // tbEndIP
            // 
            this.tbEndIP.Location = new System.Drawing.Point(60, 28);
            this.tbEndIP.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbEndIP.Name = "tbEndIP";
            this.tbEndIP.Size = new System.Drawing.Size(105, 20);
            this.tbEndIP.TabIndex = 2;
            this.tbEndIP.Text = "127.0.0.1";
            // 
            // tbYourPort
            // 
            this.tbYourPort.Location = new System.Drawing.Point(205, 5);
            this.tbYourPort.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbYourPort.Name = "tbYourPort";
            this.tbYourPort.Size = new System.Drawing.Size(46, 20);
            this.tbYourPort.TabIndex = 1;
            this.tbYourPort.Text = "80";
            // 
            // tbEndPort
            // 
            this.tbEndPort.Location = new System.Drawing.Point(205, 28);
            this.tbEndPort.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbEndPort.Name = "tbEndPort";
            this.tbEndPort.Size = new System.Drawing.Size(46, 20);
            this.tbEndPort.TabIndex = 3;
            this.tbEndPort.Text = "80";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 7);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Your IP: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 32);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "End IP: ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(169, 7);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Port: ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(169, 30);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Port: ";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(194, 56);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(56, 19);
            this.btnConnect.TabIndex = 9;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // tbToSend
            // 
            this.tbToSend.Location = new System.Drawing.Point(12, 244);
            this.tbToSend.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbToSend.Name = "tbToSend";
            this.tbToSend.Size = new System.Drawing.Size(179, 20);
            this.tbToSend.TabIndex = 10;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(194, 244);
            this.btnSend.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(56, 19);
            this.btnSend.TabIndex = 11;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(254, 28);
            this.listBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(174, 212);
            this.listBox1.TabIndex = 12;
            // 
            // tbKeyFile
            // 
            this.tbKeyFile.Location = new System.Drawing.Point(289, 246);
            this.tbKeyFile.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbKeyFile.Name = "tbKeyFile";
            this.tbKeyFile.Size = new System.Drawing.Size(139, 20);
            this.tbKeyFile.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(254, 246);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Key: ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(254, 7);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Encryptions";
            // 
            // lbMessages
            // 
            this.lbMessages.FormattingEnabled = true;
            this.lbMessages.Location = new System.Drawing.Point(12, 80);
            this.lbMessages.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.lbMessages.Name = "lbMessages";
            this.lbMessages.Size = new System.Drawing.Size(240, 160);
            this.lbMessages.TabIndex = 16;
            // 
            // UIMessenger
            // 
            this.AcceptButton = this.btnSend;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(436, 271);
            this.Controls.Add(this.lbMessages);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbKeyFile);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.tbToSend);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbEndPort);
            this.Controls.Add(this.tbYourPort);
            this.Controls.Add(this.tbEndIP);
            this.Controls.Add(this.tbYourIP);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "UIMessenger";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbYourIP;
        private System.Windows.Forms.TextBox tbEndIP;
        private System.Windows.Forms.TextBox tbYourPort;
        private System.Windows.Forms.TextBox tbEndPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox tbToSend;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TextBox tbKeyFile;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListBox lbMessages;
    }
}

