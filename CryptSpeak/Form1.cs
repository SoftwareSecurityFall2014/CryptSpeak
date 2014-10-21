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
        //  TODO: Uncouple code

        //This has really bad coupling admittedly.
        //That will totally be fixed later or w/e
        Socket sck;
        EndPoint epLocal, epRemote;
        DESEncryptor des;
        volatile string keyLoc;

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
                    DESEncryptor des2 = new DESEncryptor(temp2);
                    string receivedMessage = des2.decryptMessage(receivedData);
                    
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
            strmsg = tbYourIP.Text + ":" + tbYourPort.Text + " - " + strmsg;
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
                    byte[] msg = des.encryptMessage(toSend);

                    sck.Send(msg);

                    lbMessages.Items.Add(toSend);
                }
                tbToSend.Clear();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
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

        private void btnSend_Click(object sender, EventArgs e)
        {
            SendMessage(tbToSend.Text);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            keyLoc = tbKeyFile.Text;
            des = new DESEncryptor(keyLoc);
            Connect();
        }
    }
}
