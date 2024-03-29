﻿using System;
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
using System.Threading;
using System.Collections;

namespace _0425
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        TcpListener Server;
        Socket Client;
        Thread Th_svr;
        Thread Th_Clt;
        Hashtable HT = new Hashtable();

        private void button1_Click(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            Th_svr = new Thread(ServerSub);//設為背景執行緒
            Th_svr.IsBackground = true;
            Th_svr.Start();
            button1.Enabled = false;
        }
        private void ServerSub()
        {
            IPEndPoint EP = new IPEndPoint(IPAddress.Parse(textBox1.Text),int.Parse(textBox2.Text));

            Server = new TcpListener(EP);
            Server.Start(100);
            while (true)
            {
                Client = Server.AcceptSocket();
                Th_Clt = new Thread(Listen);
                Th_Clt.IsBackground = true;
                Th_Clt.Start();
            }
        }

        private void Listen()
        {
            Socket Sck =Client;
            Thread TH = Th_Clt;
            while (true)
            {
                try
                {
                    byte[] B = new byte[1023];
                    int inLen = Sck.Receive(B);
                    string Msg = Encoding.Default.GetString(B, 0, inLen);

                    string Cmd = Msg.Substring(0, 1);
                    string Str = Msg.Substring(1);
                    switch (Cmd)
                    {
                        case "0":
                            HT.Add(Str, Sck);
                            listBox1.Items.Add(Str);
                            break;
                        case "9":
                            HT.Remove(Str);
                            listBox1.Items.Remove(Str);
                            TH.Abort();
                            break;
                    }
                }
                catch
                {

                }
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.ExitThread(); //關閉所有執行緒 
        }
    }
    
}
