using System;
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
        Socket sck;
        EndPoint epLocal, epRemote;
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

            listBox1.Items.Add("None");
            listBox1.Items.Add("DES");

            listBox2.Items.Add("CBC");
            listBox2.Items.Add("CTR");
            listBox2.Items.Add("ECB");
            listBox2.Items.Add("OFB");
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

        private void MessageCallBack(IAsyncResult aResult)
        {
            try
            {
                int size = sck.EndReceiveFrom(aResult, ref epRemote);
                if (size > 0)
                {
                    byte[] receivedData = new byte[charLimit];

                    receivedData = (byte[])aResult.AsyncState;

                    string temp2 = keyLoc;
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
                //MessageBox.Show(exp.ToString());
            }
        }

        void SendMessage(string strmsg)
        {
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
                //MessageBox.Show(exp.ToString());
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
                //MessageBox.Show(exp.ToString());
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
            byte TEncType = 0;
            byte TEncMeth = 0;
            switch(listBox1.SelectedIndex)
            {
                case 0:
                    TEncType = (byte)EncryptionManager.EncType.None;
                    break;
                case 1:
                    TEncType = (byte)EncryptionManager.EncType.DES;
                    break;
            }

            switch (listBox2.SelectedIndex)
            {
                case 0:
                    TEncMeth = (byte)EncryptionManager.EncMeth.CBC;
                    break;
                case 1:
                    TEncMeth = (byte)EncryptionManager.EncMeth.CTR;
                    break;
                case 2:
                    TEncMeth = (byte)EncryptionManager.EncMeth.ECB;
                    break;
                case 3:
                    TEncMeth = (byte)EncryptionManager.EncMeth.OFB;
                    break;
            }
            encMan = new EncryptionManager(keyLoc, nunceLoc, TEncType, TEncMeth);
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
