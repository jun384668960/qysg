using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using His.WebService.Utility;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using System.Net;
using Update;
using System.Diagnostics;

namespace 自动升级
{
    public partial class Form1 : Form
    {
        private static string serverIp = "183.60.106.146";   //情义
        private static int serverPort = 6633;

        //private static string HCClient = "皇朝登录器.exe";
        private static string StartFile = "情义登录器.exe";
        private static string g_oldVersion;
        private static string g_versionFile = "Session.cfx";
        private static string verUpdate = "Update.zip";

        public static bool g_bLink = false;
        public static string g_strLink;
        public static string g_strVersion;
        public static long g_lUpdateLen; 
        public static string g_revStr;
        Thread thClient;

        public byte[] dataBuffer = new byte[1024];
        Socket client;
        private static ManualResetEvent TimeoutObject = new ManualResetEvent(false);

        #region //线程下载文件，解压文件，开启登录器
        public Form1()
        {
            InitializeComponent();
        }

        public void DoDetailUpdate(string str)
        {
            g_strVersion = str.Substring(0, str.IndexOf("版本"));
            g_strLink = str.Substring(str.IndexOf("版本") + 2, str.Length - (str.IndexOf("版本") + 2));
            g_lUpdateLen = long.Parse(str.Substring(str.IndexOf("长度") + 2, str.Length - (str.IndexOf("长度") + 2)));
            //MessageBox.Show("g_lUpdateLen:" + g_lUpdateLen);
            //check version
            if (g_strVersion != g_oldVersion)
            {
                string cur_dir = System.AppDomain.CurrentDomain.BaseDirectory;
                string filePath = cur_dir + verUpdate;

                if (!DownloadFile("Update.zip", g_lUpdateLen, progressBar1, label1))
                {
                    throw new Exception("DownloadFile error");
                }

                UnZipFile("Update.zip");
                File.Delete("Update.zip");
                g_revStr = "";

                StartGameClient();
                Environment.Exit(0);
            }
            else
            {
                StartGameClient();
                Environment.Exit(0);
            }
        }

        private static void UnZipFile(string zipFilePath)
        {
            if (!File.Exists(zipFilePath))
            {
                Console.WriteLine("Cannot find file '{0}'", zipFilePath);
                return;
            }

            using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
            {

                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {

                    Console.WriteLine(theEntry.Name);

                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);

                    // create directory
                    if (directoryName.Length > 0)
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    if (fileName != String.Empty)
                    {
                        using (FileStream streamWriter = File.Create(theEntry.Name))
                        {

                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                s.Close();
            }
        }

        public bool DownloadFile(string filename, long UpdateLen, System.Windows.Forms.ProgressBar prog, System.Windows.Forms.Label label1)   
        {    
            float percent = 0;    
            try   {

                string savePath = System.AppDomain.CurrentDomain.BaseDirectory;
                //int ret = FtpHelper.DownloadFtp(savePath, "Update.zip", "183.60.106.146", "FtpUser", "Donyj_ljj@163.com");
                FtpWebResponse response = FtpHelper.GetFtpWebResponse(filename, "183.60.106.146", "FtpUser", "Donyj_ljj@163.com");
                Stream ftpStream = response.GetResponseStream();
                long totalBytes = UpdateLen;

                this.Invoke((EventHandler)delegate { this.progressBar1.Value = 0; });
                this.Invoke((EventHandler)delegate
                {
                    if (prog != null)
                    {
                        prog.Maximum = (int)UpdateLen;
                    }
                });
    
                long totalDownloadedByte = 0;
                FileStream outputStream = new FileStream(savePath  + filename, FileMode.Create);

                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];

                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    totalDownloadedByte = readCount + totalDownloadedByte;
                    this.Invoke((EventHandler)delegate
                    {
                        if (prog != null)
                        {
                            prog.Value = (int)totalDownloadedByte;
                        }
                    });
                    System.Windows.Forms.Application.DoEvents();

                    percent = (float)totalDownloadedByte / (float)totalBytes * 100;
                    this.Invoke((EventHandler)delegate
                    {
                        label1.Text = "当前补丁下载进度" + percent.ToString() + "%";
                    });


                    System.Windows.Forms.Application.DoEvents();

                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }
                outputStream.Close();
                ftpStream.Close();
                response.Close();
                return true;
            }    
            catch (Exception ex)    
            {
                LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, ex.Message, new StackTrace(new StackFrame(true)));
 
                return false;
            } 
        }
        
        public void Updating()
        {
            try
            {
                while (true)
                {
                    if (g_revStr!= null && g_revStr != "")
                    {
                        if (g_revStr.Contains("版本"))
                        {
                            g_strVersion = g_revStr.Substring(0, g_revStr.IndexOf("版本"));
                            g_strLink = g_revStr.Substring(g_revStr.IndexOf("版本") + 2, g_revStr.Length - (g_revStr.IndexOf("版本") + 2));
                            g_lUpdateLen = long.Parse(g_revStr.Substring(g_revStr.IndexOf("长度") + 2, g_revStr.Length - (g_revStr.IndexOf("长度") + 2)));
                            //MessageBox.Show("g_lUpdateLen:" + g_lUpdateLen);
                            //check version
                            if (g_strVersion != g_oldVersion)
                            {
                                string cur_dir = System.AppDomain.CurrentDomain.BaseDirectory;
                                string filePath = cur_dir + verUpdate;

                                if (!DownloadFile("Update.zip", g_lUpdateLen, progressBar1, label1))
                                {
                                    throw new Exception("DownloadFile error");
                                }

                                UnZipFile("Update.zip");
                                File.Delete("Update.zip");
                                g_revStr = "";

                                StartGameClient();
                                Environment.Exit(0);
                            }
                            else
                            {
                                StartGameClient();
                                Environment.Exit(0);
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
            }
            catch(Exception ex)
            {
                LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, ex.Message, new StackTrace(new StackFrame(true)));

            }
        }

        private void StartGameClient()
        {
            //创建进程
            try {
                System.IO.Directory.SetCurrentDirectory(System.Environment.CurrentDirectory);
                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(StartFile);
                psi.RedirectStandardOutput = true;
                psi.UseShellExecute = false;
                Process newProcess = Process.Start(psi);
            }
            catch (Exception)
            {
                MessageBox.Show("登陆器不存在，请认真检查！");
                return;
            }
        }
        
        #endregion

        private void LoadServerConf()
        {
            serverIp = CIniCtrl.ReadIniData("Server", "ServerIP", "", g_versionFile);
            if (serverIp == "" || serverIp == null)
            {
                MessageBox.Show("丢失必须文件，请重新下载登录器！");
                Environment.Exit(0);
            }
            //获取启动游戏文件
            StartFile = CIniCtrl.ReadIniData("Version", "client", "", g_versionFile);
            if (StartFile == "" || StartFile == null)
            {
                MessageBox.Show("丢失必须文件，请重新下载登录器！");
                Environment.Exit(0);
            }

            string port = CIniCtrl.ReadIniData("Server", "serverPort", "", g_versionFile);
            if (port == "" || port == null)
            {
                MessageBox.Show("丢失必须文件，请重新下载登录器！");
                Environment.Exit(0);
            }
            else
            {
                serverPort = int.Parse(port);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //MessageBox.Show("Form1_Load");
            //全局版本文件和版本记录
            g_versionFile = System.AppDomain.CurrentDomain.BaseDirectory + g_versionFile;
            g_oldVersion = CIniCtrl.ReadIniData("Version", "ver", "", g_versionFile);

            //获取服务器IP
            LoadServerConf();

            //thClient = new Thread(new ThreadStart(new ThreadStart(Updating)));
            //thClient.Start();

            this.TopMost = true;
            this.Activate();
            this.TopMost = false;
            //Thread.Sleep(1000);
            try {

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
                else 
                {
                    label2.Text = "无法自动更新，请检查网络...";
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, ex.Message, new StackTrace(new StackFrame(true)));

                label2.Text = "无法自动更新，请检查网络...";
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            //linkLabel1.Text = "点击进行手动更新...";
            //linkLabel1.Links.Add(0, linkLabel1.Text.Length, "http://183.60.106.146");
        }

        #region //窗口移动实现
        private Point mouseOffset; //记录鼠标指针的坐标
        private bool isMouseDown = false;
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(mouseOffset.X, mouseOffset.Y);
                Location = mousePos;
            }
        }
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseOffset = new Point(-e.X, -e.Y);
                isMouseDown = true;
            }
        }
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            // 修改鼠标状态isMouseDown的值，确保只有鼠标左键按下并移动时，才移动窗体
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = false;
            }
        }
        #endregion


        private void label4_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            System.Environment.Exit(0);
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //首先把被访问超级链接的LinkVisted属性设置为true
            //通过事件参数e中的Link属性来获取被单击的超链接
            //linkLabel1.Links[linkLabel1.Links.IndexOf(e.Link)].Visited = true;
            //System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
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
                //MessageBox.Show("serverIp:" + serverIp + " serverPort:" + serverPort);
                Thread.Sleep(500);
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
                if (g_revStr.Contains("版本"))
                {
                    DoDetailUpdate(g_revStr);
                }

                string logPath = System.AppDomain.CurrentDomain.BaseDirectory + @"log\";
                LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, g_revStr, new StackTrace(new StackFrame(true)));

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
