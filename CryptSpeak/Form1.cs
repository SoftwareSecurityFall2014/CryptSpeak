﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CryptSpeak.Encryption;

namespace CryptSpeak.UI
{
    public partial class UIMessenger : Form
    {
        //  TODO: Uncouple code

        //This has really bad coupling admittedly.
        //That will totally be fixed later or w/e
        Socket sck;
        EndPoint epLocal, epRemote;
        //This is probably a bad idea
        //Volatile is a bad idea
        //I should not do this
        //Gonna do it anyways yolo
        volatile EncryptionManager encMan;
        volatile string keyLoc;
        volatile string nunceLoc;

        int charLimit = 32;

        public UIMessenger()
        {
            InitializeComponent();
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            btnSend.Enabled = false;

            listBox1.Items.Add("DES");
            listBox1.Items.Add("AES");
            listBox1.Items.Add("Elliptic-Curve");

            listBox2.Items.Add("ECB");
            listBox2.Items.Add("CBC");
            listBox2.Items.Add("CFB");
            listBox2.Items.Add("OFB");
            listBox2.Items.Add("CTR");
            listBox2.Items.Add("This is how the encryption is");
            listBox2.Items.Add("applied");
            listBox2.Items.Add("Betta check yourself before you");
            listBox2.Items.Add("Shrek yourself; Cause eating raw");
            listBox2.Items.Add("onions is bad for your health");
        }

        //GetLocalIP
        private string GetLocalIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    lbMessages.Items.Add("ip: " + ip.ToString() + "\n");
                    return ip.ToString();
                }
            }

            return "127.0.0.1";
        }

        //MessageCallBack
        private void MessageCallBack(IAsyncResult aResult)
        {
            try
            {
                int size = sck.EndReceiveFrom(aResult, ref epRemote);
                if (size > 0)
                {
                    byte[] receivedData = new byte[charLimit];

                    receivedData = (byte[])aResult.AsyncState;

                    string temp2 = keyLoc;//"C:/Users/William/Documents/Visual Studio 2012/Projects/CryptSpeak/CryptSpeak/Key1.txt";
                    string receivedMessage = tbEndIP.Text + ":" + tbEndPort.Text + " - " + encMan.Decrypt(receivedData);
                    
                    lbMessages.Items.Add(receivedMessage);
                    lbMessages.SelectedIndex = lbMessages.Items.Count - 1;
                    lbMessages.SelectedIndex = -1;
                }

                byte[] buffer = new byte[charLimit];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }
        }

        //button 2?
        void SendMessage(string strmsg)
        {
            //THAT IS A VERY BAD IDEA WHAT WAS I THINKING
            //SO INSECURE
            //MUCH TACKY
            //(There is expected text in the code, leaving it there so we can be like, Oh shit look a mistake that we caught 10/10 oh baby a triple)
            //strmsg = tbYourIP.Text + ":" + tbYourPort.Text + " - " + strmsg;
            try
            {
                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                for (int i = strmsg.Length%charLimit; i != (charLimit); i++)
                {
                    strmsg += " ";
                }
                for (int i = 0; i < strmsg.Length / charLimit; i++)
                {
                    string toSend = strmsg.Substring(i * charLimit, charLimit);
                    byte[] msg = encMan.Encrypt(toSend);

                    sck.Send(msg);

                    lbMessages.Items.Add(tbYourIP.Text + ":" + tbYourPort.Text + " - " + toSend);
                }
                tbToSend.Clear();
            }
            catch (Exception exp)
            {
                //MessageBox.Show(exp.ToString());
            }
        }

        //Technically "button1"
        void Connect()
        {
            try
            {
                sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                epLocal = new IPEndPoint(IPAddress.Parse(tbYourIP.Text), Convert.ToInt32(tbYourPort.Text));
                sck.Bind(epLocal);

                epRemote = new IPEndPoint(IPAddress.Parse(tbEndIP.Text), Convert.ToInt32(tbEndPort.Text));
                sck.Connect(epRemote);

                byte[] buffer = new byte[charLimit];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);

                lbMessages.Items.Add("Connected to "+ tbEndIP.Text + ":" + tbEndPort.Text +"\n");
                btnSend.Enabled = true;
                tbToSend.Focus();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }
        }

        void Disconnect(object sender, EventArgs e)
        {
            try
            {
                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                string toSend = "User has disconnected";
                byte[] msg = encMan.Encrypt(toSend);

                sck.Send(msg);

                lbMessages.Items.Add(toSend);
                tbToSend.Clear();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }
            sck.Close();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            SendMessage(tbToSend.Text);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            keyLoc = tbKeyFile.Text;
            nunceLoc = tbNunceFile.Text;
            encMan = new EncryptionManager(keyLoc, nunceLoc, (byte)EncryptionManager.EncType.DES, (byte)EncryptionManager.EncMeth.ECB);
            Connect();
        }

        private void Disconnect()
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
