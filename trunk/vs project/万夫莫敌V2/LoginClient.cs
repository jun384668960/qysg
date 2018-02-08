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
using System.Runtime.InteropServices;
using Splash.Drawing;

namespace register_client
{

    public partial class LoginClient : Form
    {
        private static string serverWeb = "192.168.0.102";
        private static string serverIp = "183.60.203.223";
        private static string StartFile = "online.dat";
        private static int  serverPort = 5633;
        private static string WebSite = "";
        private static string ReCharge = "";
        private static string GameBBS = "";
        private static string HelpCenter = "";

        public static Dictionary<int, Process> dic_Game_Process = new Dictionary<int, Process>();       //记录已经开启的游戏PID

        private static string g_oldVersion;
        private static string g_versionFile = "Session.cfx";    //本地配置文件
        public static string g_strVersion;
        public static string g_revStr;

        public static int g_limitNum = 6;                  //正常情况下的多开现在
        public static int g_SpcTimeLimitNum = 1;            //特殊时段多开限制
        public static bool g_bLimitBB = false;              //贝贝
        public static bool g_bLimitKeyPress = true;         //按键精灵
        public static bool g_bLimitEasyGame = true;         //简单游
        public static string SpcTimeStart = "19:57";
        public static string SpcTimeEnd = "21:03";
        //public static string SpcTimeStart = "20:50";
        //public static string SpcTimeEnd = "21:55";

        public static long StartFileSize = 55547499L;
        public static bool showStartFileSize = true;//true false
        public static string g_CmdLine = "01 02 03 4 5 6";
        public static Thread thListen;
        public static int g_ThreadListenSleep = 50;     //每隔40s执行检测
        public static bool g_thListenStop = true;       //检测线程标记
		
        public static DateTime g_NetTime;               //记录网络时间

        public byte[] dataBuffer = new byte[1024];
        Socket client;
        private static ManualResetEvent TimeoutObject = new ManualResetEvent(false);

        public static string protectfile = "Protect.exe";
        public static string protectProssesName = "Protect";

        public LoginClient()
        {
            InitializeComponent();
            //在 进入程序的mian方法里面插入
            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);

        }

        private void LoginClient_Shown(object sender, EventArgs e)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = false;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                //string Parameter = "rd /s /q data shape Script Update.cab";
                string Parameter = "rd /s /q data DATA Shape SPL Script";

                //this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                p.Start();
                p.StandardInput.WriteLine(Parameter);
                p.StandardInput.WriteLine("exit");
                p.StandardInput.WriteLine("exit");
                p.WaitForExit();

                Parameter = "del Update.cab SOLUpd3.pak SOLUpd4.pak Update.pak Update2.pak server.ini";
                p.Start();
                p.StandardInput.WriteLine(Parameter);
                p.StandardInput.WriteLine("exit");
                p.StandardInput.WriteLine("exit");
                p.WaitForExit();

                p.Close();

                //thClient = new Thread(new ThreadStart(new ThreadStart(Updating)));
                //thClient.Start();

                LoadProtectPresses();

                //start thread for listen limit op
                thListen = new Thread(new ParameterizedThreadStart(ThreadListen));
                thListen.Start(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("LoginClient_Shown" + ex.Message);
            }

        }

        private void LoadProtectPresses()
        {
            if (!File.Exists(protectfile))
            {
                MessageBox.Show("丢失关键文件，请核对登录器文件！");
                this.ntyIcon.Visible = false;
                this.Dispose();
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                System.Environment.Exit(0);
            }
            FileInfo f = new FileInfo(protectfile);
            //MessageBox.Show(f.Length + "");
            if (f.Length != 8704L)
            {
                MessageBox.Show("丢失关键文件异常，请核对登录器文件！");
                this.ntyIcon.Visible = false;
                this.Dispose();
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                System.Environment.Exit(0);
            }

            string protectLine = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            //创建进程
            System.IO.Directory.SetCurrentDirectory(System.Environment.CurrentDirectory);
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(protectfile, protectLine + " " + StartFile);
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            Process newProcess = Process.Start(psi);
        }

        public static void CloseAllGame(object ob)
        {
            LoginClient clientWin = (LoginClient)ob;
            clientWin.ntyIcon.Visible = false;
            //g_thListenStop = false;

            try
            {
                FlushProcessLib();
                foreach (var item in dic_Game_Process)
                {
                    item.Value.Kill();
                }
                dic_Game_Process.Clear();

                clientWin.Dispose();

                System.Diagnostics.Process.GetCurrentProcess().Kill();
                System.Environment.Exit(0);
            }
            catch (Exception)
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

        public static bool CheckStartFile(string file)
        {
            FileInfo f = new FileInfo(file);
            if (showStartFileSize)
            {
                MessageBox.Show(f.Length + "");
            }
            if (f.Length != StartFileSize)
            {
                MessageBox.Show("检测到异常文件，请遵守游戏规则！");
                return false;
            }
            return true;
        }
        public static void StartLoginGame()
        {
            try
            {
                //检测是否启动文件是否匹配成功
                if (!CheckStartFile(StartFile))
                {
                    return;
                }

                //清楚无效的pid记录
                FlushProcessLib();

                //检测是否特殊时段禁止
                int limitNum = 0;
                //MessageBox.Show("IsSpcTime()");
                if (IsSpcTime())
                {
                    limitNum = g_SpcTimeLimitNum;
                }
                else
                {
                    limitNum = g_limitNum;
                }
                //MessageBox.Show("Count:" + dic_Game_Process.Count + "  limitNum:" + limitNum);
                if (dic_Game_Process.Count >= limitNum)
                {
                    MessageBox.Show("超过游戏开启上限！");
                    return;
                }


                //创建进程
                System.IO.Directory.SetCurrentDirectory(System.Environment.CurrentDirectory);
                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(StartFile, g_CmdLine);
                psi.RedirectStandardOutput = true;
                psi.UseShellExecute = false;
                Process newProcess = Process.Start(psi);

                //记录进程
                dic_Game_Process.Add(newProcess.Id, newProcess);
            }
            catch
            {
                MessageBox.Show("启动游戏失败，文件丢失！", "系统提示");
            }
        }

        //调用的事件
        void Application_ApplicationExit(object sender, EventArgs e)
        {
            CloseAllGame(this);

            ntyIcon.Visible = false;
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            System.Environment.Exit(0);
        }
        public static void FlushProcessLib()
        {
            var pArrayy = System.Diagnostics.Process.GetProcessesByName(StartFile);
            if (pArrayy.Length <= 0)
            {
                dic_Game_Process.Clear();
            }
            else
            {
                //foreach (var item in pArrayy)
                foreach (var item in dic_Game_Process)
                {
                    bool contain = false;
                    foreach (var _item in pArrayy)
                    {
                        if (_item.Id == item.Key)
                        {
                            contain = true;
                        }
                    }
                    if (!contain)
                    {
                        dic_Game_Process.Remove(item.Key);
                    }
                }
            }
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
                                CloseAllGame(this);
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

        #region 控件事件
        //开始游戏
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
            //this.SendToBack();
            //FrmPwdMdfy frmPwdMdfy = new FrmPwdMdfy();
            //frmPwdMdfy.Show();
            //frmPwdMdfy.BringToFront();
            CloseAllGame(this);

            ntyIcon.Visible = false;

            System.Diagnostics.Process.GetCurrentProcess().Kill();
            System.Environment.Exit(0);
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
            lbl_notic.Text = CIniCtrl.ReadIniData("Notic", "text", "", g_versionFile);
            lbl_notic.Links.Add(0, lbl_notic.Text.Length, CIniCtrl.ReadIniData("Notic", "link", "", g_versionFile));

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

            Bitmap btm = Properties.Resources.登录器界面;
            ImageControl.SetRegion(this, btm);

            this.TopMost = true;
            this.Activate();
            this.TopMost = false;
        }

        private void LoginClientClose()
        {
            CloseAllGame(this);
        }
        private void btn_close_Click(object sender, EventArgs e)
        {
            LoginClientClose();
        }
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
        #endregion

        #region 鼠标移动
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
        #endregion

        #region 公告链接
        private void lbl_notic_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Link.LinkData.ToString() == "")
            {
                return;
            }
            lbl_notic.Links[lbl_notic.Links.IndexOf(e.Link)].Visited = true;
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void lbl_notic1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //首先把被访问超级链接的LinkVisted属性设置为true
            //通过事件参数e中的Link属性来获取被单击的超链接
            if (e.Link.LinkData.ToString() == "")
            {
                return;
            }
            lbl_notic1.Links[lbl_notic1.Links.IndexOf(e.Link)].Visited = true;
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }
        private void lbl_notic2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Link.LinkData.ToString() == "")
            {
                return;
            }
            lbl_notic2.Links[lbl_notic2.Links.IndexOf(e.Link)].Visited = true;
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }
        private void lbl_notic3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Link.LinkData.ToString() == "")
            {
                return;
            }
            lbl_notic3.Links[lbl_notic3.Links.IndexOf(e.Link)].Visited = true;
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }
        private void lbl_notic4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Link.LinkData.ToString() == "")
            {
                return;
            }
            lbl_notic4.Links[lbl_notic4.Links.IndexOf(e.Link)].Visited = true;
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void lbl_notic5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Link.LinkData.ToString() == "")
            {
                return;
            }

            lbl_notic5.Links[lbl_notic5.Links.IndexOf(e.Link)].Visited = true;
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }
        #endregion

        #region 获取网络时间
        /// <summary>
        /// 发送HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="param">请求的参数</param>
        /// <returns>请求结果</returns>
        public static string request(string url, string param)
        {
            string StrDate = "";
            string strValue = "";
            try 
            {
                string strURL = url;// +'?' + param;
                System.Net.HttpWebRequest request;
                request = (System.Net.HttpWebRequest)WebRequest.Create(strURL);
                request.Method = "GET";
                // 添加header
                request.Headers.Add("apikey", "7e77148a5d402516d209e7daec087b6b");
                System.Net.HttpWebResponse response;
                response = (System.Net.HttpWebResponse)request.GetResponse();
                System.IO.Stream s;
                s = response.GetResponseStream();
                
                StreamReader Reader = new StreamReader(s, Encoding.UTF8);

                int waitcount = 0;
                while ((StrDate = Reader.ReadLine()) != null && waitcount < 10)
                {
                    Thread.Sleep(300);
                    waitcount++;
                    strValue += StrDate + "\r\n";
                }
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return strValue;
        }
        private static DateTime StampToDateTime(string timeStamp)
        {
            DateTime dateTimeStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dateTimeStart.Add(toNow);
        }

        public static bool GetAPITime(out DateTime time)
        {
            string result = request("http://apis.baidu.com/3023/time/time", "");
            //MessageBox.Show(result);
            if (result != "")
            {
                string subString = "{\"stime\":";
                string subString2 = "}\r\n";
                string r = result.Substring(result.IndexOf(subString) + subString.Length, result.Length - (subString.Length + subString2.Length));
                //MessageBox.Show(result + "\n"+ r + "length:"+r.Length);
                time = StampToDateTime(r);
                //MessageBox.Show(time.ToString());
                return true;
            }
            else
            {
                time = System.DateTime.Now;
            }
            return false;
        }
        #endregion

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

        #region 导航链接
        private void lbl_WebSite_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start(WebSite);
        }

        private void lbl_ChongZhi_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start(ReCharge);
        }

        private void lbl_bbs_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //System.Diagnostics.Process.Start(GameBBS);
        }

        private void lbl_HelpCenter_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start(HelpCenter);
        }
        #endregion

        #region 防挂检测
        [DllImport("user32.dll", EntryPoint = "FindWindowA", CharSet = CharSet.Ansi)]
        public static extern int FindWindow(string lpClassName, string lpWindowName);
        static private void ThreadListen(object ob)
        {
            Process[] procList = new Process[200];
            while (g_thListenStop)
            {
                //清楚无效的pid记录
                FlushProcessLib();

                //特定时间段检测
                bool spcTime = IsSpcTime();

                //Process curProcess;
                procList = Process.GetProcesses();
                if (spcTime && dic_Game_Process.Count > g_SpcTimeLimitNum)
                {
                    //MessageBox.Show("IsSpcTime");
                    CloseAllGame(ob);
                }
                else
                {
                    //MessageBox.Show("not IsSpcTime");
                }
                foreach (var curProcess in procList)
                {
                    try
                    {
                        if (curProcess.ProcessName.ToString().Contains("按键精灵")
                            && g_bLimitKeyPress && spcTime)
                        {
                            //MessageBox.Show("使用受禁止的辅助工具将影响游戏的正常运行，请您遵守游戏规则！");
                            //如果找到，干掉所有的temp.tmp进程兵器杀出temp.tmp文件
                            CloseAllGame(ob);
                        }

                        if (FindWindow(null, "按键精灵") != 0
                            && g_bLimitKeyPress && spcTime)
                        {
                            CloseAllGame(ob);
                        }

                        if (curProcess.ProcessName.ToString().Contains("金山游侠"))
                        {
                            //MessageBox.Show("使用受禁止的辅助工具将影响游戏的正常运行，请您遵守游戏规则！");
                            //如果找到，干掉所有的temp.tmp进程兵器杀出temp.tmp文件
                            CloseAllGame(ob);
                        }

                        if (FindWindow(null, "金山游侠") != 0)
                        {
                            CloseAllGame(ob);
                        }

                        if (curProcess.ProcessName.ToString() == "Play"
                            && g_bLimitEasyGame && spcTime)
                        {
                            //如果找到，干掉所有的temp.tmp进程兵器杀出temp.tmp文件
                            CloseAllGame(ob);
                        }


                        if (FindWindow(null, "简单游") != 0
                            && g_bLimitEasyGame && spcTime)
                        {
                            CloseAllGame(ob);
                        }

                        if (curProcess.ProcessName.ToString().Contains("main.dat")
                            && g_bLimitBB)
                        {
                            //如果找到，干掉所有的temp.tmp进程兵器杀出temp.tmp文件
                            CloseAllGame(ob);
                        }

                        if (FindWindow(null, "三国贝贝") != 0
                            && g_bLimitBB)
                        {
                            CloseAllGame(ob);
                        }

                        if (curProcess.ProcessName.ToString().Contains("LD.exe"))
                        {
                            //如果找到，干掉所有的temp.tmp进程兵器杀出temp.tmp文件
                            CloseAllGame(ob);
                        }

                        if (FindWindow(null, "Ld") != 0)
                        {
                            CloseAllGame(ob);
                        }


                        ProcessModule m = curProcess.MainModule;
                        string descStr = m.FileVersionInfo.FileDescription;
                        if (descStr.Contains("按键")
                        && descStr.Contains("精灵")
                        && g_bLimitKeyPress && spcTime)
                        {
                            //MessageBox.Show("使用受禁止的辅助工具将影响游戏的正常运行，请您遵守游戏规则！");
                            //如果找到，干掉所有的temp.tmp进程兵器杀出temp.tmp文件
                            CloseAllGame(ob);
                        }

                        if (m.FileVersionInfo.FileDescription.Contains("main.dat")
                        && g_bLimitBB && spcTime)
                        {
                            //MessageBox.Show("使用受禁止的辅助工具将影响游戏的正常运行，请您遵守游戏规则！");
                            //如果找到，干掉所有的temp.tmp进程兵器杀出temp.tmp文件
                            CloseAllGame(ob);
                        }

                        if (m.FileVersionInfo.FileDescription.Contains("Play.exe")
                        && g_bLimitEasyGame && spcTime)
                        {
                            //MessageBox.Show("使用受禁止的辅助工具将影响游戏的正常运行，请您遵守游戏规则！");
                            //如果找到，干掉所有的temp.tmp进程兵器杀出temp.tmp文件
                            CloseAllGame(ob);

                        }

                        if (m.FileVersionInfo.FileDescription.Contains("金山游侠"))
                        {
                            //MessageBox.Show("使用受禁止的辅助工具将影响游戏的正常运行，请您遵守游戏规则！");
                            //如果找到，干掉所有的temp.tmp进程兵器杀出temp.tmp文件
                            CloseAllGame(ob);
                        }
                        if (m.FileVersionInfo.FileDescription.Contains("LD.exe"))
                        {
                            //MessageBox.Show("使用受禁止的辅助工具将影响游戏的正常运行，请您遵守游戏规则！");
                            //如果找到，干掉所有的temp.tmp进程兵器杀出temp.tmp文件
                            CloseAllGame(ob);
                        }

                        if (m.FileVersionInfo.FileDescription.Contains("SoWorker"))
                        {
                            //MessageBox.Show("使用受禁止的辅助工具将影响游戏的正常运行，请您遵守游戏规则！");
                            //如果找到，干掉所有的temp.tmp进程兵器杀出temp.tmp文件
                            CloseAllGame(ob);
                        }
                    }
                    catch
                    {
                    }
                }

                Thread.Sleep(g_ThreadListenSleep * 1000);
            }
        }
        static int g_getTimeErr = -1;
        static bool IsSpcTime()
        {
            try
            {

                DateTime CurrTime;
                //获取NTP时间或者获取系统时间
                if (GetAPITime(out CurrTime)){
                    //如果前后两次事件相差太大
                    long time_diff = (CurrTime.Ticks - g_NetTime.Ticks) / 10000000;

                    if (g_getTimeErr >= 0 && time_diff > g_ThreadListenSleep * 4)
                    {
                        return true;
                    }
                    //MessageBox.Show("time_diff:" + (CurrTime.Ticks - g_NetTime.Ticks) / 10000000);
                    g_NetTime = CurrTime;
                    g_getTimeErr = 0;
                }else{ 
                    //多次获取失败则认为是错误
                    //if (++g_getTimeErr > 3) 
                    MessageBox.Show("请下载安装Microsoft .NET Framework 4.0,否则将限制部分功能！");
                    return true;
                }

                //时间比较
                DateTime dtStart = Convert.ToDateTime(SpcTimeStart);
                DateTime dtEnd = Convert.ToDateTime(SpcTimeEnd);

                if (DateTime.Compare(g_NetTime, dtStart) > 0
                    && DateTime.Compare(dtEnd, g_NetTime) > 0)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 按钮特效
        private void btn_login_MouseEnter(object sender, EventArgs e)
        {
            btn_login.BackgroundImage = Properties.Resources.开始游戏_点燃;
            btn_login.BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void btn_login_MouseLeave(object sender, EventArgs e)
        {
            btn_login.BackgroundImage = Properties.Resources.开始游戏_正常;
            btn_login.BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void btn_login_MouseDown(object sender, MouseEventArgs e)
        {
            btn_login.BackgroundImage = Properties.Resources.开始游戏_按下;
            btn_login.BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void btn_login_MouseUp(object sender, MouseEventArgs e)
        {
            btn_login.BackgroundImage = Properties.Resources.开始游戏_点燃;
            btn_login.BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void btn_accMgr_MouseDown(object sender, MouseEventArgs e)
        {
            btn_accMgr.BackgroundImage = Properties.Resources.退出游戏_按下;
            btn_accMgr.BackgroundImageLayout = ImageLayout.Stretch;

        }

        private void btn_accMgr_MouseEnter(object sender, EventArgs e)
        {
            btn_accMgr.BackgroundImage = Properties.Resources.退出游戏_点燃;
            btn_accMgr.BackgroundImageLayout = ImageLayout.Stretch;

        }

        private void btn_accMgr_MouseLeave(object sender, EventArgs e)
        {
            btn_accMgr.BackgroundImage = Properties.Resources.退出游戏_正常;
            btn_accMgr.BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void btn_accMgr_MouseUp(object sender, MouseEventArgs e)
        {
            btn_accMgr.BackgroundImage = Properties.Resources.退出游戏_点燃;
            btn_accMgr.BackgroundImageLayout = ImageLayout.Stretch;
        }
        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            Process[] procList = new Process[200];
            procList = Process.GetProcesses();

            bool protect_live = false;
            foreach (var curProcess in procList)
            {
                if (curProcess.ProcessName.ToString() == protectProssesName)
                {
                    protect_live = true;
                }
            }
            //如果protect进程被关闭了，关闭游戏
            if (!protect_live)
            {
                CloseAllGame(this);
            }
        }
    }
}
