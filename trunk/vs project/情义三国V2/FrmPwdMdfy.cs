using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace register_client
{
    public partial class FrmPwdMdfy : Form
    {
        private static string serverWeb = "192.168.0.102";
        private static string serverIp = "222.187.222.166";   //哲别
        private static string StartFile = "online.dat";   //哲别
        private static int serverPort = 6633;
        private static string WebSite = "";
        private static string ReCharge = "";
        private static string GameBBS = "";
        private static string HelpCenter = "";

        private string g_versionFile = "Session.cfx";

        public FrmPwdMdfy()
        {
            InitializeComponent();
        }

        private Point mouseOffset; //记录鼠标指针的坐标
        private bool isMouseDown = false;
        private void FrmPwdMdfy_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseOffset = new Point(-e.X, -e.Y);
                isMouseDown = true;
            }
        }

        private void FrmPwdMdfy_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(mouseOffset.X, mouseOffset.Y);
                Location = mousePos;
            }
        }

        private void FrmPwdMdfy_MouseUp(object sender, MouseEventArgs e)
        {
            // 修改鼠标状态isMouseDown的值，确保只有鼠标左键按下并移动时，才移动窗体
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = false;
            }
        }

        private void lbl_Close_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void txt_userName_MouseEnter(object sender, EventArgs e)
        {
            
        }
        private void txt_userName_MouseDown(object sender, MouseEventArgs e)
        {
            txt_userName.Text = "";
        }
        private void txt_userName_MouseLeave(object sender, EventArgs e)
        {
            /*
            if (txt_userName.Text == "")
            {
                txt_userName.Text = "6-20位，请不要输入特殊字符！";
            }
             * */
        }

        private void txt_oldPwd_MouseDown(object sender, MouseEventArgs e)
        {
            txt_oldPwd.Text = "";
        }

        private void txt_newPwd_MouseDown(object sender, MouseEventArgs e)
        {
            txt_newPwd.Text = "";
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

        ValidCode validCode;
        private void FrmPwdMdfy_Load(object sender, EventArgs e)
        {
            g_versionFile = System.AppDomain.CurrentDomain.BaseDirectory + g_versionFile;
            LoadServerConf();

            validCode = new ValidCode(5, ValidCode.CodeType.Numbers);
            picValidCode.Image = Bitmap.FromStream(validCode.CreateCheckCodeImage());
        }
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

        private void btn_OK_Click(object sender, EventArgs e)
        {
            if (!this.txt_ValidCode.Text.Equals(validCode.CheckCode))
            {
                MessageBox.Show(" 请输入正确的验证码!", this.Text);
                this.txt_ValidCode.Focus();
                validCode = new ValidCode(5, ValidCode.CodeType.Numbers);
                picValidCode.Image = Bitmap.FromStream(validCode.CreateCheckCodeImage());
                return;
            }

            validCode = new ValidCode(5, ValidCode.CodeType.Numbers);
            picValidCode.Image = Bitmap.FromStream(validCode.CreateCheckCodeImage());

            string userName = txt_userName.Text;
            string oldPwd = txt_oldPwd.Text;
            string newPwd = txt_newPwd.Text;

            if (!IsNumAndEnCh(userName))
            {
                MessageBox.Show("用户名必须由字母或数字组成！");
                return;
            }
            if (!IsNumAndEnCh(newPwd))
            {
                MessageBox.Show("密码必须由字母或数字组成！");
                return;
            }

            if (userName.Length < 6 || userName.Length > 20)
            {
                MessageBox.Show("用户名错误，请核对！");
                return;
            }

            if (oldPwd.Length < 6 || oldPwd.Length > 20)
            {
                MessageBox.Show("旧密码错误，请核对！");
                return;
            }

            if (newPwd.Length < 6 || newPwd.Length > 20)
            {
                MessageBox.Show("密码长度必须为6~20位！");
                return;
            }

            if (newPwd == oldPwd)
            {
                MessageBox.Show("修改的密码与原密码一致，请确认！");
                return;
            }


            try
            {
                string keys = "";

                byte[] i_keys = getRandomNum(8, 48, 57);
                keys = System.Text.Encoding.Default.GetString(i_keys);

                Mdfy_Pwd_Info mdfyPwd_info;
                Tran_Head tran_head;

                mdfyPwd_info.name = CSecurity.EncryptDES(userName, keys);
                mdfyPwd_info.oldpasswd = CSecurity.EncryptDES(oldPwd, keys);
                mdfyPwd_info.newpasswd = CSecurity.EncryptDES(newPwd, keys);
                mdfyPwd_info.key = keys;

                string _name = CSecurity.DecryptDES(mdfyPwd_info.name, keys);
                string _oldpasswd = CSecurity.DecryptDES(mdfyPwd_info.oldpasswd, keys);
                string _newpasswd = CSecurity.DecryptDES(mdfyPwd_info.newpasswd, keys);
                if (_name == mdfyPwd_info.name || _oldpasswd == mdfyPwd_info.oldpasswd
                    || _newpasswd == mdfyPwd_info.newpasswd)
                {
                    throw (new Exception("注册失败！"));
                }



                byte[] data = CStructBytesFormat.StructToBytes(mdfyPwd_info);

                tran_head.cmd = 3;
                tran_head.length = data.Length;

                byte[] rbyte = CDataBuil.BuildBytes(tran_head, mdfyPwd_info);

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

        private void rbt_qysg_CheckedChanged(object sender, EventArgs e)
        {
            if (rbt_qysg.Checked)
            {
                //serverPort = 5633;
                serverPort = int.Parse(CIniCtrl.ReadIniData("Server", "serverPort", "", g_versionFile));
            }    
        }

        private void rbt_hcsg_CheckedChanged(object sender, EventArgs e)
        {
            if (rbt_hcsg.Checked)
            {
                //serverPort = 6633;
                serverPort = int.Parse(CIniCtrl.ReadIniData("Server", "serverPort2", "", g_versionFile));
            }
        }

    }
}
