using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace register_client
{
    public partial class AccManager : Form
    {
        private static string serverWeb = "192.168.0.102";
        private static string serverIp = "222.187.222.166";   //哲别
        private static string StartFile = "online.dat";   //哲别
        private static int serverPort = 5633;
        private static string WebSite = "";
        private static string ReCharge = "";
        private static string GameBBS = "";
        private static string HelpCenter = "";

        private string g_versionFile = "Session.cfx";

        public AccManager()
        {
            InitializeComponent();
        }


        private void LoadServerConf()
        {
            serverIp = CIniCtrl.ReadIniData("Server", "ServerIP", "", g_versionFile);
            if (serverIp == "" || serverIp == null)
            {
                MessageBox.Show("丢失必须文件，请重新下载登录器！");
            }
            //获取启动游戏文件
            StartFile = CIniCtrl.ReadIniData("Server", "StartFile", "", g_versionFile);
            if (StartFile == "" || StartFile == null)
            {
                MessageBox.Show("丢失必须文件，请重新下载登录器！");
            }

            string port = CIniCtrl.ReadIniData("Server", "serverPort", "", g_versionFile);
            if (port == "" || port == null)
            {
                MessageBox.Show("丢失必须文件，请重新下载登录器！");
            }
            else
            {
                serverPort = int.Parse(port);
            }
        }

        private void AccManager_Load(object sender, System.EventArgs e)
        {
            g_versionFile = System.AppDomain.CurrentDomain.BaseDirectory + g_versionFile;
            LoadServerConf();

            validCode = new ValidCode(5, ValidCode.CodeType.Numbers);
            picValidCode.Image = Bitmap.FromStream(validCode.CreateCheckCodeImage());

            try {
              
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //MessageBox.Show("连接失败！");
            }
            /*
            Bitmap btm = Properties.Resources.和尚;
            GraphicsPath gp = ImageControl.CreateGraphicsPath(btm);
            this.Region = new Region(gp);
             * */

        }

        
        /// <summary>
        /// 判断输入的字符串是否只包含数字和英文字母
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNumAndEnCh(string input)
        {
            string pattern = @"^[A-Za-z0-9]+$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }
  
        public byte[] getRandomNum(int num, int minValue, int maxValue)
        {
            Random ra = new Random(unchecked((int)DateTime.Now.Ticks));
            byte[] arrNum = new byte[num];
            int tmp = 0;
            for (int i = 0; i < num; i++)
            {
                tmp = ra.Next(minValue, maxValue); //随机取数
                arrNum[i] = (byte)tmp; //取出值赋到数组中
            }

            return arrNum;
        }

        private void btn_register_Click(object sender, EventArgs e)
        {
            if (!this.txtValidCode.Text.Equals(validCode.CheckCode))
            {
                MessageBox.Show(" 请输入正确的验证码!", this.Text);
                this.txtValidCode.Focus();
                validCode = new ValidCode(5, ValidCode.CodeType.Numbers);
                picValidCode.Image = Bitmap.FromStream(validCode.CreateCheckCodeImage());
                return;
            }

            validCode = new ValidCode(5, ValidCode.CodeType.Numbers);
            picValidCode.Image = Bitmap.FromStream(validCode.CreateCheckCodeImage());

            string reg_name = txt_name.Text;
            string reg_pwd = txt_pwd.Text;
            string reg_pwd2 = txt_pwd2.Text;

            if (!IsNumAndEnCh(reg_name))
            {
                MessageBox.Show("用户名必须由字母或数字组成！");
                return;
            }
            if (!IsNumAndEnCh(reg_pwd))
            {
                MessageBox.Show("密码必须由字母或数字组成！");
                return;
            }

            if (reg_name.Length < 6 || reg_name.Length >20)
            {
                MessageBox.Show("用户名长度必须为6~20位！");
                return;
            }

            if (reg_pwd.Length < 6 || reg_pwd.Length > 20)
            {
                MessageBox.Show("密码长度必须为6~20位！");
                return;
            }

            if (reg_pwd2 != reg_pwd)
            {
                MessageBox.Show("两次输入密码不一致，请确认！");
                return;
            }


            try
            {
                string keys = "";

                byte[] i_keys = getRandomNum(8, 48, 57);
                keys = System.Text.Encoding.Default.GetString(i_keys);

                Rg_Info rg_info;
                Tran_Head tran_head;

                rg_info.name = CSecurity.EncryptDES(reg_name, keys);
                rg_info.passwd = CSecurity.EncryptDES(reg_pwd, keys);
                rg_info.key = keys;

                string _name = CSecurity.DecryptDES(rg_info.name, keys);
                string _passwd = CSecurity.DecryptDES(rg_info.passwd, keys);
                if (_name == rg_info.name || _passwd == rg_info.passwd)
                {
                    throw (new Exception("注册失败！"));
                }
                
                byte[] data = CStructBytesFormat.StructToBytes(rg_info);

                tran_head.cmd = 1;
                tran_head.length = data.Length;

                byte[] rbyte = CDataBuil.BuildBytes(tran_head, rg_info);

                if (ServerConnection())
                {
                    SendMessage(rbyte);
                }
                /*
                IPHostEntry host;
                host = Dns.GetHostEntry(serverWeb);

                foreach (IPAddress ip in host.AddressList)
                {
                    serverIp = "192.168.0.102";//ip.ToString();

                    if (ServerConnection())
                    {
                        SendMessage(rbyte);
                        break;
                    }
                }
                 * */

            }
            catch (Exception)
            {
                MessageBox.Show("注册失败！");
            }
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }


        private void AccManager_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseOffset = new Point(-e.X, -e.Y);
                isMouseDown = true;
            }
        }
        private void AccManager_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (isMouseDown)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(mouseOffset.X, mouseOffset.Y);
                Location = mousePos;
            }
        }
        private void AccManager_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // 修改鼠标状态isMouseDown的值，确保只有鼠标左键按下并移动时，才移动窗体
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = false;
            }
        }

        private Point mouseOffset; //记录鼠标指针的坐标
        private bool isMouseDown = false;


        ValidCode validCode;

        private void txt_pwd_MouseDown(object sender, MouseEventArgs e)
        {
            txt_pwd.Text = "";
        }

        private void txt_pwd2_MouseDown(object sender, MouseEventArgs e)
        {
            txt_pwd2.Text = "";
        }

        private void txt_name_MouseDown(object sender, MouseEventArgs e)
        {
            txt_name.Text = "";
        }

        private void txt_name_MouseLeave(object sender, EventArgs e)
        {
            if (txt_name.Text == "")
            {
                txt_name.Text = "6-20位，请不要输入特殊字符！";
            }
        }


        #region //网络通讯相关

        public static string g_revStr;
        public byte[] dataBuffer = new byte[1024];
        Socket client;
        private static ManualResetEvent TimeoutObject = new ManualResetEvent(false);
        public Encoding encoding = Encoding.Unicode; //解码器（可以用于汉字）
        //连接服务器
        private bool ServerConnection()
        {
            try
            {

                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                /*
                IPAddress ip = IPAddress.Parse("192.168.0.102");
                IPEndPoint iep = new IPEndPoint(ip, serverPort);
                client.BeginConnect(iep, new AsyncCallback(Connect), null);
                TimeoutObject.WaitOne(3000, false);
                */
                //TcpClient tcpclient = new TcpClient();
                //client.Connect("192.168.0.102", serverPort);
                client.Connect(serverIp, serverPort);
                TimeoutObject.WaitOne(1000, false);

                client.BeginReceive(dataBuffer, 0, dataBuffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallBack), null);

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, ex.Message, new StackTrace(new StackFrame(true)));

                return false;
            }
        }

        private void Connect(IAsyncResult asyncresult)
        {

            try
            {
                TcpClient tcpclient = asyncresult.AsyncState as TcpClient;

                if (tcpclient.Client != null)
                {
                    client.EndConnect(asyncresult);
                    client.BeginReceive(dataBuffer, 0, dataBuffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallBack), null);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, ex.Message, new StackTrace(new StackFrame(true)));
            }
            finally
            {
                TimeoutObject.Set();
            }
        }

        //接收byte字节方式
        private void RecieveCallBack(IAsyncResult iar)
        {
            try
            {
                int iRx = client.EndReceive(iar);
                if (iRx <= 0)
                {
                    return;
                }

                byte[] byte_Receive = new byte[iRx];
                for (int i = 0; i < iRx; i++)
                {
                    byte_Receive[i] = dataBuffer[i];
                    //listBox1.Items.Add(Convert.ToString(byte_Receive[i], 16));
                }
                g_revStr = encoding.GetString(byte_Receive, 0, iRx);

                MessageBox.Show(g_revStr);
                //string logPath = System.AppDomain.CurrentDomain.BaseDirectory + @"log\";
                //LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, g_revStr, new StackTrace(new StackFrame(true)));

                return;
                //继续监听下一次的数据接收
                //client.BeginReceive(dataBuffer, 0, dataBuffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallBack), null);

            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, ex.Message, new StackTrace(new StackFrame(true)));
            }
        }

        void SendMessage(byte[] byte_data)
        {
            try
            {
                byte[] buffer = byte_data;
                int snLen = 0;

                //client.BeginSend(buffer, snLen, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), client);
                client.Send(buffer, snLen, buffer.Length, SocketFlags.None);
            }
            catch (Exception ex)
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
                LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, ex.Message, new StackTrace(new StackFrame(true)));

            }
        }

        private static void SendCallBack(IAsyncResult iar)
        {
            Socket workerSocket = (Socket)iar.AsyncState;
            try
            {
                workerSocket.EndSend(iar);
            }
            catch (Exception ex)
            {
                workerSocket.Shutdown(SocketShutdown.Both);
                workerSocket.Close();
                LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, ex.Message, new StackTrace(new StackFrame(true)));

            }
        }

        #endregion
    }
}
