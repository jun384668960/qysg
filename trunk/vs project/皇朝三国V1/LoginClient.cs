using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.IO;

namespace register_client
{

    public partial class LoginClient : Form
    {
        private static string   serverWeb = "192.168.0.102";
        private static string   serverIp = "222.187.222.166";   //哲别
        private static string   StartFile = "online.dat";   //哲别
        private static int      serverPort = 5633;
        private static string   WebSite = "";
        private static string   ReCharge = "";
        private static string   GameBBS = "";
        private static string   HelpCenter = "";

        Thread thClient;

        private static string   g_oldVersion;
        private static string   g_versionFile = "Session.cfx";
        public static string    g_realStartFile = "temp.tmp";
        public static string    g_strVersion;
        public static string    g_revStr;

        public byte[] dataBuffer = new byte[1024];
        Socket client;
        private static ManualResetEvent TimeoutObject = new ManualResetEvent(false);

        public LoginClient()
        {
            InitializeComponent();
            //在 进入程序的mian方法里面插入
            Application.ApplicationExit+=new EventHandler(Application_ApplicationExit);
            
        }

        //调用的事件
        static void Application_ApplicationExit(object sender, EventArgs e)
        {
            CloseAllGame();

            System.Diagnostics.Process.GetCurrentProcess().Kill();
            System.Environment.Exit(0);  
        }

        public void Updating()
        {
            try
            {
                while (true)
                {
                    if (g_revStr != null && g_revStr != "")
                    {
                        if (g_revStr.Contains("版本"))
                        {
                            g_strVersion = g_revStr.Substring(0, g_revStr.IndexOf("版本"));

                            //MessageBox.Show(g_strLink);
                            //check version
                            if (g_strVersion != g_oldVersion)
                            {
                                CloseAllGame();
                                //去更新
                                StartUpdateGame();

                                Environment.Exit(0);
                            }
                            else
                            {
                                //开启游戏
                                StartLoginGame();
                            }
                            g_revStr = "";
                        }

                    }
                    else
                    {
                        Thread.Sleep(5);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, ex.Message, new StackTrace(new StackFrame(true)));

            }
        }


        //开始游戏
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        public static extern int WinExec(string exeName, int operType);
        private void btn_login_Click(object sender, EventArgs e)
        {
            StartLoginGame();

            //开始游戏前进行版本矫正
            /*
            //send check version
            Ver_Info tran_data;
            tran_data.ver = "get_version";
            Tran_Head tran_head;

            byte[] data = CStructBytesFormat.StructToBytes(tran_data);

            tran_head.cmd = 2;
            tran_head.length = data.Length;

            byte[] rbyte = CDataBuil.BuildBytes(tran_head, tran_data);

            if (ServerConnection())
            {
                SendMessage(rbyte);
            }
             * */

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
        //修改密码和删除账户
        private void btn_accMgr_Click(object sender, EventArgs e)
        {
            this.SendToBack();
            FrmPwdMdfy frmPwdMdfy = new FrmPwdMdfy();
            frmPwdMdfy.Show();
            //frmPwdMdfy.BringToFront();
        }
        //注册用户
        private void btn_accRegisger_Click(object sender, EventArgs e)
        {
            this.SendToBack();
            AccManager acc_mgr = new AccManager();
            acc_mgr.Show();
            //acc_mgr.BringToFront();
        }

        private void LoadNews()
        {
            //notice
            lbl_notic1.Text = CIniCtrl.ReadIniData("Notic1", "text", "", g_versionFile);
            lbl_notic1.Links.Add(0, lbl_notic1.Text.Length, CIniCtrl.ReadIniData("Notic1", "link", "", g_versionFile));
            lbl_noticTime1.Text = CIniCtrl.ReadIniData("Notic1", "time", "", g_versionFile);

            lbl_notic2.Text = CIniCtrl.ReadIniData("Notic2", "text", "", g_versionFile);
            lbl_notic2.Links.Add(0, lbl_notic2.Text.Length, CIniCtrl.ReadIniData("Notic2", "link", "", g_versionFile));
            lbl_noticTime2.Text = CIniCtrl.ReadIniData("Notic2", "time", "", g_versionFile);

            lbl_notic3.Text = CIniCtrl.ReadIniData("Notic3", "text", "", g_versionFile);
            lbl_notic3.Links.Add(0, lbl_notic3.Text.Length, CIniCtrl.ReadIniData("Notic3", "link", "", g_versionFile));
            lbl_noticTime3.Text = CIniCtrl.ReadIniData("Notic3", "time", "", g_versionFile);

            lbl_notic4.Text = CIniCtrl.ReadIniData("Notic4", "text", "", g_versionFile);
            lbl_notic4.Links.Add(0, lbl_notic4.Text.Length, CIniCtrl.ReadIniData("Notic4", "link", "", g_versionFile));
            lbl_noticTime4.Text = CIniCtrl.ReadIniData("Notic4", "time", "", g_versionFile);

            lbl_notic5.Text = CIniCtrl.ReadIniData("Notic5", "text", "", g_versionFile);
            lbl_notic5.Links.Add(0, lbl_notic5.Text.Length, CIniCtrl.ReadIniData("Notic5", "link", "", g_versionFile));
            lbl_noticTime5.Text = CIniCtrl.ReadIniData("Notic5", "time", "", g_versionFile);
        }

        private void LoadServerConf()
        {
            serverIp = CIniCtrl.ReadIniData("Server", "ServerIP", "", g_versionFile);
            if (serverIp == "" || serverIp == null)
            {
                MessageBox.Show("丢失必须文件，请重新下载登录器！");
                LoginClientClose();
            }
            //获取启动游戏文件
            StartFile = CIniCtrl.ReadIniData("Server", "StartFile", "", g_versionFile);
            if (StartFile == "" || StartFile == null)
            {
                MessageBox.Show("丢失必须文件，请重新下载登录器！");
                LoginClientClose();
            }
 
            string port = CIniCtrl.ReadIniData("Server", "serverPort", "", g_versionFile);
            if (port == "" || port == null)
            {
                MessageBox.Show("丢失必须文件，请重新下载登录器！");
                LoginClientClose();
            }
            else
            {
                serverPort = int.Parse(port);
            }
        }

        private void LoadWebLink()
        {
            //网站连接
            WebSite = CIniCtrl.ReadIniData("Server", "WebSite", "", g_versionFile);
            if (WebSite == "" || WebSite == null)
            {
                WebSite = "www.baidu.com";
            }

            ReCharge = CIniCtrl.ReadIniData("Server", "ReCharge", "", g_versionFile);
            if (ReCharge == "" || ReCharge == null)
            {
                ReCharge = "www.baidu.com";
            }

            GameBBS = CIniCtrl.ReadIniData("Server", "GameBBS", "", g_versionFile);
            if (GameBBS == "" || GameBBS == null)
            {
                GameBBS = "www.baidu.com";
            }

            HelpCenter = CIniCtrl.ReadIniData("Server", "HelpCenter", "", g_versionFile);
            if (HelpCenter == "" || HelpCenter == null)
            {
                HelpCenter = "www.baidu.com";
            }
        }

        private void LoginClient_load(object sender, EventArgs e)
        {
            g_versionFile = System.AppDomain.CurrentDomain.BaseDirectory + g_versionFile;
            g_oldVersion = CIniCtrl.ReadIniData("Version", "ver", "", g_versionFile);
            lbl_CurVersion.Text = g_oldVersion;

            //加载公告和新闻
            LoadNews();

            //获取服务器IP
            LoadServerConf();

            //加载网页链接
            LoadWebLink();

            thClient = new Thread(new ThreadStart(new ThreadStart(Updating)));
            thClient.Start();

            this.TopMost = true;
            this.Activate();
            this.TopMost = false;
        }
        
        private void LoginClientClose()
        {
            CloseAllGame();

            ntyIcon.Visible = false;
            this.Dispose();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            System.Environment.Exit(0);   
        }
        private void btn_close_Click(object sender, EventArgs e)
        {
            LoginClientClose();
        }

        private void LoginClient_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseOffset = new Point(-e.X, -e.Y);  
                isMouseDown = true;
            }
        }
        private void LoginClient_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (isMouseDown)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(mouseOffset.X, mouseOffset.Y);
                Location = mousePos;
            }
        }
        private void LoginClient_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // 修改鼠标状态isMouseDown的值，确保只有鼠标左键按下并移动时，才移动窗体
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = false;
            }
        }

        private Point mouseOffset; //记录鼠标指针的坐标
        private bool isMouseDown = false;

        private void ntyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.TopMost = true;
            this.Activate();
            this.TopMost = false;
        }
        private void btn_min_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
        private void lbl_notic1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //首先把被访问超级链接的LinkVisted属性设置为true
            //通过事件参数e中的Link属性来获取被单击的超链接
            lbl_notic1.Links[lbl_notic1.Links.IndexOf(e.Link)].Visited = true;
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }
        private void lbl_notic2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lbl_notic2.Links[lbl_notic2.Links.IndexOf(e.Link)].Visited = true;
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }
        private void lbl_notic3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lbl_notic3.Links[lbl_notic3.Links.IndexOf(e.Link)].Visited = true;
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }
        private void lbl_notic4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lbl_notic4.Links[lbl_notic4.Links.IndexOf(e.Link)].Visited = true;
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void lbl_notic5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lbl_notic5.Links[lbl_notic5.Links.IndexOf(e.Link)].Visited = true;
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }
        private void LoginClient_Shown(object sender, EventArgs e)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = false;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            //string Parameter = "rd /s /q data shape Script Update.cab";
            string Parameter = "rd /s /q data shape Script";

            //this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            p.Start();
            p.StandardInput.WriteLine(Parameter);
            p.StandardInput.WriteLine("exit");
            p.StandardInput.WriteLine("exit");
            p.WaitForExit();

            Parameter = "del Update.cab";
            p.Start();
            p.StandardInput.WriteLine(Parameter);
            p.StandardInput.WriteLine("exit");
            p.StandardInput.WriteLine("exit");
            p.WaitForExit();

            p.Close();

        }

        public static Dictionary<int, Process> dic_Game_Process = new Dictionary<int, Process>();

        public static void CloseAllGame()
        {
            try
            {
                if (dic_Game_Process.Count <= 0)
                {
                    return;
                }
                //需要关闭的进程
                foreach (KeyValuePair<int, Process> closeProcess in dic_Game_Process)
                {
                    //查找进程是否还存在
                    var pArrayy = System.Diagnostics.Process.GetProcessesByName(closeProcess.Value.ProcessName);
                    foreach (var item in pArrayy)
                    {
                        //如果不存在则去掉
                        if (!dic_Game_Process.ContainsKey(item.Id))
                        {
                            dic_Game_Process.Remove(item.Id);
                        }
                        else //存在则关闭
                        {
                            if (!closeProcess.Value.CloseMainWindow())
                            {
                                closeProcess.Value.Kill();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        public static void StartUpdateGame()
        {
            //创建进程
            System.IO.Directory.SetCurrentDirectory(System.Environment.CurrentDirectory);
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Update.exe");
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            Process newProcess = Process.Start(psi);
        }

        public static void StartLoginGame()
        {
            try
            {
                if (dic_Game_Process.Count >= 15)
                {
                    return;
                }

                //创建进程
                System.IO.Directory.SetCurrentDirectory(System.Environment.CurrentDirectory);
                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(StartFile, "JustForMe");
                psi.RedirectStandardOutput = true;
                psi.UseShellExecute = false;
                Process newProcess = Process.Start(psi);

                //记录进程
                dic_Game_Process.Add(newProcess.Id, newProcess);

                var pArrayy = System.Diagnostics.Process.GetProcessesByName(newProcess.ProcessName);
                foreach (var item in pArrayy)
                {
                    if (!dic_Game_Process.ContainsKey(item.Id))
                    {
                        dic_Game_Process.Remove(item.Id);
                    }
                }
            }
            catch
            {
                MessageBox.Show("启动游戏失败，-"+StartFile+"文件丢失！", "系统提示");
            }
            
        }




        #region //网络通讯相关
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

        private void lbl_WebSite_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(WebSite);
        }

        private void lbl_ChongZhi_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(ReCharge);
        }

        private void lbl_bbs_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(GameBBS);
        }

        private void lbl_HelpCenter_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(HelpCenter);
        }
    }
}
