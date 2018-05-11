using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Common;
using System.Threading;
using 注册网关;
using System.Data.SqlClient;

namespace register_server
{
    public partial class Frm_server : Form
    {
        #region //成员变量
        public static bool m_Active = false;
        private string serverIni;

        private static string sql_srvAddr = "192.168.0.100";
        //private static string sql_srvAddr = "120.25.239.84";
        private string sql_srvPort;
        private string sql_srvUser;
        private string sql_srvPwd;
        private string accMrgPort;
        private string sqlAccountName;

        IntPtr g_wLoginWindow = IntPtr.Zero;
        IntPtr g_bReload = IntPtr.Zero;
        IntPtr g_bWorldSend = IntPtr.Zero;
        BankHandle bankHandle = new BankHandle();
        int g_MaxQuesNum = 30;
        int g_QuesInterval = 40;//s
        string g_15NameFilter = "";
        int g_15SrchInterval = 2;//s
        string g_15MaxAnn = "15";

        SGExHandle m_SGExHandle = new SGExHandle();
        private WarHandle warHandle = new WarHandle();
        private CbHandle cbHandle = new CbHandle();
        #endregion
        public Frm_server()
        {
            InitializeComponent();
        }
        private bool CheckActive()
        {
            //激活检测
            string endTime = "";
            bool ret = RegistHelper.CheckRegist(out endTime);
            if (!ret)
            {
                if (endTime != "")
                {
                    MessageBox.Show("软件已经过期，请将目录下的ComputerInfo.key文件发给软件提供者，以获取使用权限！");
                    this.Text = this.Text + "  - 已到期 到期时间:" + endTime;
                }
                else
                {
                    MessageBox.Show("软件尚未激活，请将目录下的ComputerInfo.key文件发给软件提供者，以获取使用权限！");
                    this.Text = this.Text + "  - 未激活";
                }
                return false;
            }
            else
            {
                this.Text = this.Text + "  - 已激活 到期时间:" + endTime;
                return true;
            }
        }
        private void Frm_server_Load(object sender, EventArgs e)
        {
            //if (!CheckActive())
            //{
            //    m_Active = false;
            //    return;
            //}

            m_Active = true;
            serverIni = System.AppDomain.CurrentDomain.BaseDirectory + "配置文件.ini";
            LoadIniConf();
            LoadSgserverConf();
            LoadAutoRchConf();
            LoadSkillMrgInfos();
            //虚宝发放
            LoadXBMrgInfos();
            //国战奖励
            LoadGZMrgInfos();
            //赤壁奖励
            LoadCbMrgInfos();

            m_SGExHandle.LoadLoginServerPtr("");

            sql_srvAddr = txt_sqlsvr.Text;
            sql_srvUser = txt_sqlAcc.Text;
            sql_srvPwd = txt_sqlPwd.Text;

        }
        
        #region  //界面控件事件和配置相关
        private void txt_sqlsvr_TextChanged(object sender, EventArgs e)
        {
            sql_srvAddr = txt_sqlsvr.Text;

            CIniCtrl.WriteIniData("Server", "ServerIP", sql_srvAddr, serverIni);
        }

        private void txt_sqlAcc_TextChanged(object sender, EventArgs e)
        {
            sql_srvUser = txt_sqlAcc.Text;

            CIniCtrl.WriteIniData("Server", "SqlAccount", sql_srvUser, serverIni);
        }

        private void txt_sqlPwd_TextChanged(object sender, EventArgs e)
        {
            sql_srvPwd = txt_sqlPwd.Text;

            CIniCtrl.WriteIniData("Server", "SqlPasswd", txt_sqlPwd.Text, serverIni);
        }

        private void LoadIniConf()
        {
            #region //sgserver信息
            sql_srvAddr = CIniCtrl.ReadIniData("Server", "ServerIP", "", serverIni);
            if (sql_srvAddr != "")
            {
                txt_sqlsvr.Text = sql_srvAddr;
            }

            sql_srvPort = CIniCtrl.ReadIniData("Server", "SqlPort", "", serverIni);
            if (sql_srvPort == "")
            {
                CIniCtrl.WriteIniData("Server", "SqlPort", "1433", serverIni);
            }
            else
            {
                txt_sqlPort.Text = sql_srvPort;
            }

            sql_srvUser = CIniCtrl.ReadIniData("Server", "SqlAccount", "", serverIni);
            if (sql_srvUser != "")
            {
                txt_sqlAcc.Text = sql_srvUser;
            }

            sql_srvPwd = CIniCtrl.ReadIniData("Server", "SqlPasswd", "", serverIni);
            if (sql_srvPwd != "")
            {
                txt_sqlPwd.Text = sql_srvPwd;
            }

            accMrgPort = CIniCtrl.ReadIniData("Server", "ListenPort", "", serverIni);
            if (accMrgPort != "")
            {
                txt_accMrgPort.Text = accMrgPort;
            }

            sqlAccountName = CIniCtrl.ReadIniData("Server", "AccountName", "", serverIni);
            if (sqlAccountName != "")
            {
                txt_sqlAccountName.Text = sqlAccountName;
            }
            #endregion
            #region //版本管理
            string gameVersionFile = CIniCtrl.ReadIniData("Config", "gameVersionFile", "", serverIni);
            if (gameVersionFile != "")
            {
                if (gameVersionFile.Contains("LoginServer.ini") || gameVersionFile.Contains("loginserver.ini"))
                {
                    txt_gameVerFile.Text = gameVersionFile;

                    //读取版本号到vertxt
                    string CurVer = CIniCtrl.ReadIniData("System", "Version", "", txt_gameVerFile.Text);
                    txt_gameVer.Text = CurVer;
                    m_SGExHandle.SetConfigPath(txt_gameVerFile.Text);
                }
                else
                {
                    txt_gameVerFile.Text = "请选择版本文件！";
                }
            }

            string loginVersion = CIniCtrl.ReadIniData("Config", "loginVersion", "", serverIni);
            if (loginVersion != "")
            {
                txt_loginVer.Text = loginVersion;
            }

            string gameServerFolder = CIniCtrl.ReadIniData("Config", "gameServerFolder", "", serverIni);
            if (gameServerFolder != "")
            {
                txt_svrForder.Text = gameServerFolder;
            }
            string gServerIp = CIniCtrl.ReadIniData("Server", "GServerIP", "", serverIni);
            if (gServerIp != "")
            {
                txt_GServerIP.Text = gServerIp;
            }

            string gameFreezeFilter = CIniCtrl.ReadIniData("Config", "FreezeFilter", "", serverIni);
            if (gameFreezeFilter != "")
            {
                rtb_FreezeFilter.Text = gameFreezeFilter;
                m_FreezeFilterList = rtb_FreezeFilter.Text.Split(',');
            }
            #endregion
            #region //外挂检测
            string gameAutoFreeze = CIniCtrl.ReadIniData("Config", "AutoFreeze", "", serverIni);
            if (gameServerFolder != "" && gameAutoFreeze == "Enable")
            {
                cbx_AutoFreeze.Checked = true;
                btn_startListen_Click(null, null);
            }
            else
            {
                cbx_AutoFreeze.Checked = false;
            }
            #endregion
            #region //在线答题
            FillQAItemsView();
            rbx_QANormalDetil.Text = CIniCtrl.ReadIniData("Config", "m_NormalRecharge", "", serverIni);
            rbx_QATaskDetil.Text = CIniCtrl.ReadIniData("Config", "m_TaskRecharge1", "", serverIni);

            string sanvtName = CIniCtrl.ReadIniData("Server", "sanvtName", "", serverIni);
            txt_sanvtName.Text = sanvtName;

            string quesBankFile = CIniCtrl.ReadIniData("Config", "QuesBankFile", "", serverIni);
            txt_QusbankFile.Text = quesBankFile;


            txt_TaskDate.Text = CIniCtrl.ReadIniData("Config", "TaskDate", "", serverIni);
            if (txt_TaskDate.Text != string.Empty)
            {
                var dates = txt_TaskDate.Text.Split(',');
                m_TaskDate.Clear();
                foreach (var date in dates)
                {
                    m_TaskDate.Add(int.Parse(date));
                }
            }
            txt_TaskTime.Text = CIniCtrl.ReadIniData("Config", "TaskTime", "", serverIni);
            if (txt_TaskTime.Text != string.Empty)
            {
                var times = txt_TaskTime.Text.Split(';');
                m_TaskTime.Clear();
                foreach (var time in times)
                {
                    m_TaskTime.Add(time);
                }
            }

            string maxQuesNum = CIniCtrl.ReadIniData("Config", "MaxQuesNum", "", serverIni);
            if (maxQuesNum != string.Empty)
            {
                txt_MaxQuesNum.Text = maxQuesNum;
                g_MaxQuesNum = int.Parse(maxQuesNum);
            }

            string quesInterval = CIniCtrl.ReadIniData("Config", "QuesInterval", "", serverIni);
            if (quesInterval != string.Empty)
            {
                txt_QuesInterval.Text = quesInterval;
                g_QuesInterval = int.Parse(quesInterval);
            }

            string _AskNormalInterval = CIniCtrl.ReadIniData("Config", "m_AskNormalInterval", "", serverIni);
            if (_AskNormalInterval != string.Empty)
            {
                txt_AskNormalInterval.Text = _AskNormalInterval;
                m_AskNormalInterval = UInt32.Parse(_AskNormalInterval);
            }

            string _AnswerVtId = CIniCtrl.ReadIniData("Config", "m_AnswerVtId", "", serverIni);
            if (_AskNormalInterval != string.Empty)
            {
                txt_AnswerVtId.Text = _AnswerVtId;
                m_AnswerVtId = UInt32.Parse(_AnswerVtId);
            }

            string _AnswerVtName = CIniCtrl.ReadIniData("Config", "m_AnswerVtName", "", serverIni);
            m_AnswerVtName = _AnswerVtName;
            txt_AnswerVtName.Text = _AnswerVtName;

            string gameAutoStartQues = CIniCtrl.ReadIniData("Config", "AutoStartQues", "", serverIni);
            if (gameAutoStartQues != "" && gameAutoStartQues == "Enable")
            {
                cbx_AutoStartQues.Checked = true;
                button21_Click(null, null);
            }
            else
            {
                cbx_AutoStartQues.Checked = false;
            }
            #endregion
            #region //加持公告
            string _15SrchInterval = CIniCtrl.ReadIniData("Config", "15SrchInterval", "", serverIni);
            if (_15SrchInterval != string.Empty)
            {
                g_15SrchInterval = int.Parse(_15SrchInterval);
                txt_15srchInterval.Text = _15SrchInterval;
            }

            string _15MaxAnn = CIniCtrl.ReadIniData("Config", "15MaxAnn", "", serverIni);
            if (_15MaxAnn != string.Empty)
            {
                g_15MaxAnn = _15MaxAnn;
                txt_15MaxAnn.Text = _15MaxAnn;
            }

            string _AnnItemsFile = CIniCtrl.ReadIniData("Config", "AnnItemsFile", "", serverIni);
            if (_AnnItemsFile != string.Empty)
            {
                txt_AnnItemsFile.Text = _AnnItemsFile;
            }

            string _15NameFilter = CIniCtrl.ReadIniData("Config", "15NameFilter", "", serverIni);
            g_15NameFilter = _15NameFilter;
            rbx_15NameFilter.Text = _15NameFilter;

            string game15TalkAutoStart = CIniCtrl.ReadIniData("Config", "15TalkAutoStart", "", serverIni);
            if (game15TalkAutoStart != "" && game15TalkAutoStart == "Enable")
            {
                cbx_AutoStart15Talk.Checked = true;
                button21_Click_1(null, null);
            }
            else
            {
                cbx_AutoStart15Talk.Checked = false;
            }
            #endregion
            #region //系统公告
            string liststring = CIniCtrl.ReadIniData("Config", "WorldWordsList", "", serverIni);
            if (liststring != string.Empty)
            {
                var list = liststring.Split(';');
                listBox1.Items.Clear();
                foreach (var it in list)
                {
                    listBox1.Items.Add(it);
                }
                listBox1.Update();
            }
            #endregion
        }

        private void Frm_server_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void Frm_server_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_SGExHandle.Abort15AnnThread();
            m_SGExHandle.AbortQAThread();
            warHandle.AbortThread();
            cbHandle.AbortThread();
        }

        private void btn_sql_Click(object sender, EventArgs e)
        {
            if (btn_sql.Text == "连接数据库")
            {
                //连接数据库
                //*
                string conn_str = "Data Source = " + sql_srvAddr + "," + sql_srvPort + "; Initial Catalog = " + sqlAccountName + "; User Id = " + sql_srvUser + "; Password = " + sql_srvPwd + ";";
                if (!CSGHelper.SqlConn(conn_str))
                {
                    MessageBox.Show("数据库连接失败！");
                    return;
                }
                else
                {
                    MessageBox.Show("数据库连接成功！");

                    btn_sql.Text = "断开连接";
                    lbl_sqlStatus.Text = "数据库已连接";
                    txt_sqlsvr.ReadOnly = true;
                    txt_sqlPort.ReadOnly = true;
                    txt_sqlAcc.ReadOnly = true;
                    txt_sqlPwd.ReadOnly = true;
                    txt_sqlAccountName.ReadOnly = true;
                }
            }
            else
            {
                //*
                if (!CSGHelper.SqlClose())
                {
                    MessageBox.Show("断开数据库连接失败！");
                    return;
                }
                else
                {
                    btn_sql.Text = "连接数据库";
                    lbl_sqlStatus.Text = "数据库未连接";
                    txt_sqlsvr.ReadOnly = false;
                    txt_sqlPort.ReadOnly = false;
                    txt_sqlAcc.ReadOnly = false;
                    txt_sqlPwd.ReadOnly = false;
                    txt_sqlAccountName.ReadOnly = false;
                }
            }
        }

        private void btn_accPortModify_Click(object sender, EventArgs e)
        {
            //文本监测

            //写入
            CIniCtrl.WriteIniData("Server", "ListenPort", txt_accMrgPort.Text, serverIni);

        }

        private void txt_accMrgPort_TextChanged(object sender, EventArgs e)
        {
            //文本监测

            //写入
            CIniCtrl.WriteIniData("Server", "ListenPort", txt_accMrgPort.Text, serverIni);
            accMrgPort = txt_accMrgPort.Text;
        }

        private void txt_sqlPort_TextChanged(object sender, EventArgs e)
        {
            //文本监测

            //写入
            CIniCtrl.WriteIniData("Server", "SqlPort", txt_sqlPort.Text, serverIni);
            sql_srvPort = txt_sqlPort.Text;
        }
        private void txt_sqlAccountName_TextChanged(object sender, EventArgs e)
        {
            //写入
            CIniCtrl.WriteIniData("Server", "AccountName", txt_sqlAccountName.Text, serverIni);
            sqlAccountName = txt_sqlAccountName.Text;
        }

        private void Frm_server_Deactivate(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)  //判断是否最小化
            {
                this.ShowInTaskbar = false;  //不显示在系统任务栏
                notifyIcon1.Visible = true;  //托盘图标可见
            }
        }

        private void btn_gameVerMdfy_Click(object sender, EventArgs e)
        {
            if (txt_gameVerFile.Text.Contains("LoginServer.ini") || txt_gameVerFile.Text.Contains("loginserver.ini"))
            {
                if (txt_gameVer.Text != "" && txt_gameVer.Text != null)
                {
                    //写版本号到loginServer.ini
                    CIniCtrl.WriteIniData("System", "Version", txt_gameVer.Text, txt_gameVerFile.Text);
                }
            }
            else
            {
                MessageBox.Show("错误的文件，请重新选择！");
                txt_gameVerFile.Text = "请选择版本文件！";
                return;
            }
        }

        private void btn_loginVerMdfy_Click(object sender, EventArgs e)
        {
            if (txt_gameVerFile.Text != "" && txt_gameVerFile.Text != null)
            {
                //写版本号到配置文件.ini
                CIniCtrl.WriteIniData("Config", "loginVersion", txt_loginVer.Text, serverIni);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*.*)|*.*";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = fileDialog.FileName;
                if (file.Contains("LoginServer.ini") || file.Contains("loginserver.ini"))
                {
                    txt_gameVerFile.Text = file;
                }
                else
                {
                    MessageBox.Show("错误的文件，请重新选择！");
                    txt_gameVerFile.Text = "请选择版本文件！";
                    return;
                }
            }
            else
            {
                return;
            }

            //读取版本号到vertxt
            string CurVer = CIniCtrl.ReadIniData("System", "Version", "", txt_gameVerFile.Text);
            txt_gameVer.Text = CurVer;

            //保存次路径到工具配置文件
            CIniCtrl.WriteIniData("Config", "gameVersionFile", txt_gameVerFile.Text, serverIni);
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = true;  //显示在系统任务栏
                this.WindowState = FormWindowState.Normal;  //还原窗体
                notifyIcon1.Visible = false;  //托盘图标隐藏
            }
        }

        private void txt_gameVer_TextChanged(object sender, EventArgs e)
        {
            if (txt_gameVerFile.Text.Contains("LoginServer.ini") || txt_gameVerFile.Text.Contains("loginserver.ini"))
            {
                if (txt_gameVer.Text != "" && txt_gameVer.Text != null)
                {
                    //写版本号到loginServer.ini
                    CIniCtrl.WriteIniData("System", "Version", txt_gameVer.Text, txt_gameVerFile.Text);
                }
            }
            else
            {
                MessageBox.Show("错误的文件，请重新选择！");
                txt_gameVerFile.Text = "请选择版本文件！";
                return;
            }
        }

        private void txt_loginVer_TextChanged(object sender, EventArgs e)
        {
            if (txt_gameVerFile.Text != "" && txt_gameVerFile.Text != null)
            {
                //写版本号到配置文件.ini
                CIniCtrl.WriteIniData("Config", "loginVersion", txt_loginVer.Text, serverIni);
            }
        }

        #endregion

        #region //服务管理
        private void LoadSgserverConf()
        {
            load_ServerPort_Conf();
            load_Cb_Conf();
            load_War_Conf();
            load_Soul_Conf();

            //加载服务启动
            load_Process_Conf();
        }
        private void btn_register_Click(object sender, EventArgs e)
        {
            //数据库未连接，账户管理不允许启动
            /*
            if (btn_sql.Text == "连接数据库")
            {
                MessageBox.Show("请先连接数据库！");
                return;
            }
            */
            if (btn_register.Text == "启用")
            {
                //启用监听端口
                CSocketHelper.StartListening(int.Parse(accMrgPort));

                btn_register.Text = "禁用";
                lbl_regstatus.Text = "服务已开启";
                txt_accMrgPort.ReadOnly = true;
                btn_accPortModify.Enabled = false;
            }
            else
            {
                //关闭监听
                CSocketHelper.StopListening(int.Parse(accMrgPort));
                btn_register.Text = "启用";
                lbl_regstatus.Text = "服务已关闭。";
                txt_accMrgPort.ReadOnly = false;
                btn_accPortModify.Enabled = true;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.Description = "请选择文件路径";
            string foldPath = "";
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                foldPath = folderDialog.SelectedPath;
            }
            else
            {
                return;
            }

            //读取版本号到txt_svrForder
            txt_svrForder.Text = foldPath;

            //保存次路径到工具配置文件
            CIniCtrl.WriteIniData("Config", "gameServerFolder", foldPath, serverIni);
        }

        private void btn_folderSync_Click(object sender, EventArgs e)
        {
            //Account/AccountServer.ini [system]  server_ini_dir = d:\sgserver\soldata 
            string AccountServerFile = txt_svrForder.Text + "\\Account\\AccountServer.ini";
            if (!File.Exists(AccountServerFile))
            {
                MessageBox.Show("丢失关键文件:" + AccountServerFile);
            }
            else
            {
                CIniCtrl.WriteIniData("system", "server_ini_dir", txt_svrForder.Text + "\\soldata", AccountServerFile);
            }
            //DataBase/DBServer.ini  [DBServer] server_ini_dir = d:\sgserver\soldata
            string DataBaseFile = txt_svrForder.Text + "\\DataBase\\DBServer.ini";
            if (!File.Exists(DataBaseFile))
            {
                MessageBox.Show("丢失关键文件:" + DataBaseFile);
            }
            else
            {
                CIniCtrl.WriteIniData("DBServer", "server_ini_dir", txt_svrForder.Text + "\\soldata", DataBaseFile);
            }
            //Log/LogServer.ini [system] server_ini_dir = D:\sgserver\soldata
            string LogFile = txt_svrForder.Text + "\\Log\\LogServer.ini";
            if (!File.Exists(LogFile))
            {
                MessageBox.Show("丢失关键文件:" + LogFile);
            }
            else
            {
                CIniCtrl.WriteIniData("system", "server_ini_dir", txt_svrForder.Text + "\\soldata", LogFile);
            }
            //Login/LoginServer.ini [System]  server_ini_dir = d:\sgserver\soldata 
            string LoginServerFile = txt_svrForder.Text + "\\Login\\LoginServer.ini";
            if (!File.Exists(LoginServerFile))
            {
                MessageBox.Show("丢失关键文件:" + LoginServerFile);
            }
            else
            {
                CIniCtrl.WriteIniData("System", "server_ini_dir", txt_svrForder.Text + "\\soldata", LoginServerFile);
            }
            //Map1-1/MapServer.ini [MapServer] data_dir = d:\sgserver\soldata server_ini_dir = d:\sgserver\soldata
            DirectoryInfo di = new DirectoryInfo(txt_svrForder.Text);
            DirectoryInfo[] diA = di.GetDirectories();
            foreach (var item in diA)
            {
                if (item.FullName.Contains("Map"))
                {
                    string MapServerFile = item.FullName + "\\MapServer.ini";
                    if (!File.Exists(MapServerFile))
                    {
                        MessageBox.Show("丢失关键文件:" + MapServerFile);
                    }
                    else
                    {
                        CIniCtrl.WriteIniData("MapServer", "server_ini_dir", txt_svrForder.Text + "\\soldata", MapServerFile);
                        CIniCtrl.WriteIniData("MapServer", "data_dir", txt_svrForder.Text + "\\soldata", MapServerFile);
                    }
                }
            }

            //VTServer/VTServer.ini [system] server_ini_dir = d:\sgserver\soldata
            string VTServerFile = txt_svrForder.Text + "\\VTServer\\VTServer.ini";
            if (!File.Exists(VTServerFile))
            {
                MessageBox.Show("丢失关键文件:" + VTServerFile);
            }
            else
            {
                CIniCtrl.WriteIniData("system", "server_ini_dir", txt_svrForder.Text + "\\soldata", VTServerFile);
            }
            MessageBox.Show("设置完成！");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string gServerIp = txt_GServerIP.Text;
            CIniCtrl.WriteIniData("Server", "GServerIP", gServerIp, serverIni);

            //

            //读取 //soldata/Server.ini
            string ServerFile = txt_svrForder.Text + "\\soldata\\Server.ini";
            if (!File.Exists(ServerFile))
            {
                return;
            }
            FileStream rdfs = new FileStream(ServerFile, FileMode.Open, FileAccess.Read);
            StreamReader rd = new StreamReader(rdfs, Encoding.ASCII);
            rd.BaseStream.Seek(0, SeekOrigin.Begin);

            rd.DiscardBufferedData();
            rd.BaseStream.Seek(0, SeekOrigin.Begin);
            rd.BaseStream.Position = 0;

            string allLine = "";
            string strLine = "";
            strLine = "";
            strLine = rd.ReadLine();
            do
            {
                if (strLine.Contains("ip = "))
                {
                    strLine = "ip = " + gServerIp;
                }
                else if (strLine.Contains("ip_real = "))
                {
                    strLine = "ip_real = " + gServerIp;
                }

                allLine += strLine + "\r\n";
                strLine = "";
                strLine = rd.ReadLine();
            } while (strLine != null);
            rd.Close();
            rdfs.Close();

            File.WriteAllText(ServerFile, allLine);
        }
        private void load_ServerPort_Conf()
        {
            //读取 //soldata/Server.ini
            string ServerFile = txt_svrForder.Text + "\\soldata\\Server.ini";
            if (!File.Exists(ServerFile))
            {
                return;
            }
            string loginPort = CIniCtrl.ReadIniData("LoginServer", "port", "", ServerFile);
            txt_loginPort.Text = loginPort;

            string DBPort = CIniCtrl.ReadIniData("DBServer", "port", "", ServerFile);
            txt_dbPort.Text = DBPort;

            string logPort = CIniCtrl.ReadIniData("LogServer", "port", "", ServerFile);
            txt_logPort.Text = logPort;

            string accountPort = CIniCtrl.ReadIniData("AccountServer", "port", "", ServerFile);
            txt_accountPort.Text = accountPort;

            string VTPort = CIniCtrl.ReadIniData("VTServer", "port", "", ServerFile);
            txt_VTPort.Text = VTPort;

            //map
            //string mapPort = "";
            //var list = CIniCtrlComm.ReadValues(ServerFile);
            //foreach (var item in list)
            //{
            //    if (item.Section == "MapServer")
            //    {
            //        mapPort += item.GetComment("port") + ",";
            //    }
            //}
            //txt_mapPort.Text = mapPort; 

            string mapPort = "";
            FileStream rdfs = new FileStream(ServerFile, FileMode.Open, FileAccess.Read);
            StreamReader rd = new StreamReader(rdfs, Encoding.ASCII);
            rd.BaseStream.Seek(0, SeekOrigin.Begin);

            rd.DiscardBufferedData();
            rd.BaseStream.Seek(0, SeekOrigin.Begin);
            rd.BaseStream.Position = 0;

            string strLine = "";
            strLine = "";
            strLine = rd.ReadLine();
            string map_session = "";
            do
            {
                if (strLine.Contains("[MapServer]"))
                {
                    map_session = "MapServer";
                }
                if (strLine.Contains("port") && map_session == "MapServer")
                {
                    string map_port = strLine.Split('=')[1];
                    mapPort += map_port + ",";
                    map_session = "";
                }

                strLine = "";
                strLine = rd.ReadLine();
            } while (strLine != null);
            rd.Close();
            rdfs.Close();

            txt_mapPort.Text = mapPort.Replace(" ", "");
        }
        private void button6_Click(object sender, EventArgs e)
        {
            load_ServerPort_Conf();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //设置
            string ServerFile = txt_svrForder.Text + "\\soldata\\Server.ini";
            if (!File.Exists(ServerFile))
            {
                return;
            }
            string loginPort = txt_loginPort.Text;
            CIniCtrl.WriteIniData("LoginServer", "port", loginPort, ServerFile);

            string DBPort = txt_dbPort.Text;
            CIniCtrl.WriteIniData("DBServer", "port", DBPort, ServerFile);

            string logPort = txt_logPort.Text;
            CIniCtrl.WriteIniData("LogServer", "port", logPort, ServerFile);

            string accountPort = txt_accountPort.Text;
            CIniCtrl.WriteIniData("AccountServer", "port", accountPort, ServerFile);

            string VTPort = txt_VTPort.Text;
            CIniCtrl.WriteIniData("VTServer", "port", VTPort, ServerFile);

            string mapPort = txt_mapPort.Text;
            var map_list = mapPort.Split(',');

            FileStream rdfs = new FileStream(ServerFile, FileMode.Open, FileAccess.Read);
            StreamReader rd = new StreamReader(rdfs, Encoding.ASCII);
            rd.BaseStream.Seek(0, SeekOrigin.Begin);

            rd.DiscardBufferedData();
            rd.BaseStream.Seek(0, SeekOrigin.Begin);
            rd.BaseStream.Position = 0;

            string allLine = "";
            string strLine = "";
            strLine = "";
            strLine = rd.ReadLine();
            string map_session = "";
            int map_list_index = 0;
            do
            {
                if (strLine.Contains("[MapServer]"))
                {
                    map_session = "MapServer";
                }
                if (strLine.Contains("port") && map_session == "MapServer")
                {
                    map_session = "";

                    if (map_list_index < map_list.Length)
                    {
                        strLine = "port = " + map_list[map_list_index];
                        map_list_index++;
                    }
                }
                allLine += strLine + "\r\n";
                strLine = "";
                strLine = rd.ReadLine();
            } while (strLine != null);
            rd.Close();
            rdfs.Close();

            File.WriteAllText(ServerFile, allLine);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string tmp = "";
            string tmpFile = "";

            //Account AccountServer.ini [system] sql_ip = 127.0.0.1,4433 sql_inout_ip = 127.0.0.1,4433 sql_account = sa sql_password = 123456
            tmpFile = txt_svrForder.Text + "\\Account\\AccountServer.ini";
            if (!File.Exists(tmpFile))
            {
                return;
            }
            tmp = txt_sqlsvr.Text + "," + txt_sqlPort.Text;
            CIniCtrl.WriteIniData("system", "sql_ip", tmp, tmpFile);
            CIniCtrl.WriteIniData("system", "sql_inout_ip", tmp, tmpFile);

            tmp = txt_sqlAcc.Text;
            CIniCtrl.WriteIniData("system", "sql_account", tmp, tmpFile);
            tmp = txt_sqlPwd.Text;
            CIniCtrl.WriteIniData("system", "sql_password", tmp, tmpFile);
            //Log LogServer.ini [system] sql_ip = 127.0.0.1,4433 sql_account = sa sql_password = 123456
            tmpFile = txt_svrForder.Text + "\\Log\\LogServer.ini";
            if (!File.Exists(tmpFile))
            {
                return;
            }
            tmp = txt_sqlsvr.Text + "," + txt_sqlPort.Text;
            CIniCtrl.WriteIniData("system", "sql_ip", tmp, tmpFile);

            tmp = txt_sqlAcc.Text;
            CIniCtrl.WriteIniData("system", "sql_account", tmp, tmpFile);
            tmp = txt_sqlPwd.Text;
            CIniCtrl.WriteIniData("system", "sql_password", tmp, tmpFile);

            //VTServer VTServer.ini [system] sql_item_ip = 127.0.0.1,4433 sql_ip = 127.0.0.1,4433 sql_account = sa sql_password = 123456
            tmpFile = txt_svrForder.Text + "\\VTServer\\VTServer.ini";
            if (!File.Exists(tmpFile))
            {
                return;
            }
            tmp = txt_sqlsvr.Text + "," + txt_sqlPort.Text;
            CIniCtrl.WriteIniData("system", "sql_item_ip", tmp, tmpFile);
            CIniCtrl.WriteIniData("system", "sql_ip", tmp, tmpFile);

            tmp = txt_sqlAcc.Text;
            CIniCtrl.WriteIniData("system", "sql_account", tmp, tmpFile);
            tmp = txt_sqlPwd.Text;
            CIniCtrl.WriteIniData("system", "sql_password", tmp, tmpFile);
        }

        private void btn_soulset_Click(object sender, EventArgs e)
        {
            //设置
            string LoginServerFile = txt_svrForder.Text + "\\Login\\LoginServer.ini";
            if (!File.Exists(LoginServerFile))
            {
                return;
            }
            string tmp = "";
            tmp = txt_soulticketwdy.Text;
            CIniCtrl.WriteIniData("soul", "ticket_weekday", tmp, LoginServerFile);
            tmp = txt_soulsellday.Text;
            CIniCtrl.WriteIniData("soul", "ticket_sell", tmp, LoginServerFile);
            tmp = txt_soulbattlwdy.Text;
            CIniCtrl.WriteIniData("soul", "battle_weekday", tmp, LoginServerFile);
            tmp = txt_soulbattltime.Text;
            CIniCtrl.WriteIniData("soul", "battle_time", tmp, LoginServerFile);
            tmp = txt_soulbattlperiod.Text;
            CIniCtrl.WriteIniData("soul", "battle_period", tmp, LoginServerFile);

            m_SGExHandle.ReloadLoginServer();
        }

        private void btn_warset_Click(object sender, EventArgs e)
        {
            //设置
            string LoginServerFile = txt_svrForder.Text + "\\Login\\LoginServer.ini";
            if (!File.Exists(LoginServerFile))
            {
                return;
            }
            string tmp = "";
            tmp = txt_wardate.Text;
            CIniCtrl.WriteIniData("System", "country_war_date", tmp, LoginServerFile);
            tmp = txt_wartime.Text;
            CIniCtrl.WriteIniData("System", "country_war_time", tmp, LoginServerFile);
            tmp = txt_warperiod.Text;
            CIniCtrl.WriteIniData("System", "country_war_period", tmp, LoginServerFile);

            m_SGExHandle.ReloadLoginServer();
        }
        private void load_Process_Conf()
        {
            txt_ps1.Text = CIniCtrl.ReadIniData("Prosess", "ps1", "", serverIni);
            txt_ps2.Text = CIniCtrl.ReadIniData("Prosess", "ps2", "", serverIni);
            txt_ps3.Text = CIniCtrl.ReadIniData("Prosess", "ps3", "", serverIni);
            txt_ps4.Text = CIniCtrl.ReadIniData("Prosess", "ps4", "", serverIni);
            txt_ps5.Text = CIniCtrl.ReadIniData("Prosess", "ps5", "", serverIni);
            txt_ps6.Text = CIniCtrl.ReadIniData("Prosess", "ps6", "", serverIni);
            txt_ps7.Text = CIniCtrl.ReadIniData("Prosess", "ps7", "", serverIni);
            txt_ps8.Text = CIniCtrl.ReadIniData("Prosess", "ps8", "", serverIni);
            txt_ps9.Text = CIniCtrl.ReadIniData("Prosess", "ps9", "", serverIni);
            txt_ps10.Text = CIniCtrl.ReadIniData("Prosess", "ps10", "", serverIni);
            txt_ps11.Text = CIniCtrl.ReadIniData("Prosess", "ps11", "", serverIni);
            txt_ps12.Text = CIniCtrl.ReadIniData("Prosess", "ps12", "", serverIni);
            txt_ps13.Text = CIniCtrl.ReadIniData("Prosess", "ps13", "", serverIni);
            txt_ps14.Text = CIniCtrl.ReadIniData("Prosess", "ps14", "", serverIni);
            txt_ps15.Text = CIniCtrl.ReadIniData("Prosess", "ps15", "", serverIni);
        }
        private void load_Soul_Conf()
        {
            //读取武魂设置
            string LoginServerFile = txt_svrForder.Text + "\\Login\\LoginServer.ini";
            if (!File.Exists(LoginServerFile))
            {
                return;
            }
            string tmp = "";

            tmp = CIniCtrl.ReadIniData("soul", "ticket_weekday", "", LoginServerFile);
            txt_soulticketwdy.Text = tmp.Split(';')[0].Replace("\t", "").Replace(" ", "");

            tmp = CIniCtrl.ReadIniData("soul", "ticket_sell", "", LoginServerFile);
            txt_soulsellday.Text = tmp.Split(';')[0].Replace("\t", "").Replace(" ", "");

            tmp = CIniCtrl.ReadIniData("soul", "battle_weekday", "", LoginServerFile);
            txt_soulbattlwdy.Text = tmp.Split(';')[0].Replace("\t", "").Replace(" ", "");

            tmp = CIniCtrl.ReadIniData("soul", "battle_time", "", LoginServerFile);
            txt_soulbattltime.Text = tmp.Split(';')[0].Replace("\t", "").Replace(" ", "");

            tmp = CIniCtrl.ReadIniData("soul", "battle_period", "", LoginServerFile);
            txt_soulbattlperiod.Text = tmp.Split(';')[0].Replace("\t", "").Replace(" ", "");
        }

        private void load_War_Conf()
        {
            //读取国战设置
            string LoginServerFile = txt_svrForder.Text + "\\Login\\LoginServer.ini";
            if (!File.Exists(LoginServerFile))
            {
                return;
            }
            string tmp = "";

            tmp = CIniCtrl.ReadIniData("System", "country_war_date", "", LoginServerFile);
            txt_wardate.Text = tmp.Split(';')[0].Replace("\t", "").Replace(" ", "");

            tmp = CIniCtrl.ReadIniData("System", "country_war_time", "", LoginServerFile);
            txt_wartime.Text = tmp.Split(';')[0].Replace("\t", "").Replace(" ", "");

            tmp = CIniCtrl.ReadIniData("System", "country_war_period", "", LoginServerFile);
            txt_warperiod.Text = tmp.Split(';')[0].Replace("\t", "").Replace(" ", "");
        }

        private void load_Cb_Conf()
        {
            //读取 //Login/LoginHistory.ini
            string LoginHistoryFile = txt_svrForder.Text + "\\Login\\LoginHistory.ini";
            if (!File.Exists(LoginHistoryFile))
            {
                return;
            }

            FileStream rdfs = new FileStream(LoginHistoryFile, FileMode.Open, FileAccess.Read);
            StreamReader rd = new StreamReader(rdfs, Encoding.ASCII);
            rd.BaseStream.Seek(0, SeekOrigin.Begin);

            rd.DiscardBufferedData();
            rd.BaseStream.Seek(0, SeekOrigin.Begin);
            rd.BaseStream.Position = 0;

            string strLine = "";
            strLine = "";
            strLine = rd.ReadLine();
            string history_session = "";
            string history_type = "";
            string history_period = "";
            string history_min_team_player = "";
            do
            {
                if (strLine.Contains("[history]"))
                {
                    history_session = "history";
                }
                if (strLine.Contains("type"))
                {
                    history_type = strLine.Split('=')[1].Split(';')[0].Replace("\t", "").Replace(" ", "");
                }
                if (strLine.Contains("period") && history_type == "1")
                {
                    history_period = strLine.Split('=')[1].Split(';')[0].Replace("\t", "").Replace(" ", "");
                    txt_cbperiod.Text = history_period;
                }
                if (strLine.Contains("min_team_player") && history_type == "1")
                {
                    history_min_team_player = strLine.Split('=')[1].Split(';')[0].Replace("\t", "").Replace(" ", "");
                    txt_cbminnum.Text = history_min_team_player;

                }
                if (strLine.Contains("time") && history_session == "history" && history_type == "1")
                {
                    string history_time = strLine.Split('=')[1].Split(';')[0].Replace("\t", "").Replace(" ", "");
                    string history_weekday = history_time.Replace(" ", "").Split(',')[0];
                    if (history_weekday == "1")
                    {
                        txt_cbMonTime.Text = history_time.Substring(2);
                    }
                    else if (history_weekday == "2")
                    {
                        txt_cbTueTime.Text = history_time.Substring(2);
                    }
                    else if (history_weekday == "3")
                    {
                        txt_cbWedTime.Text = history_time.Substring(2);
                    }
                    else if (history_weekday == "4")
                    {
                        txt_cbThuTime.Text = history_time.Substring(2);
                    }
                    else if (history_weekday == "5")
                    {
                        txt_cbFriTime.Text = history_time.Substring(2);
                    }
                    else if (history_weekday == "6")
                    {
                        txt_cbSatTime.Text = history_time.Substring(2);
                    }
                    else if (history_weekday == "7")
                    {
                        txt_cbSunTime.Text = history_time.Substring(2);
                    }
                }
                strLine = "";
                strLine = rd.ReadLine();
            } while (strLine != null);
            rd.Close();
            rdfs.Close();
        }

        private void btn_cbset_Click(object sender, EventArgs e)
        {
            //设置赤壁
            string LoginHistoryFile = txt_svrForder.Text + "\\Login\\LoginHistory.ini";
            if (!File.Exists(LoginHistoryFile))
            {
                return;
            }
            FileStream rdfs = new FileStream(LoginHistoryFile, FileMode.Open, FileAccess.Read);
            //StreamReader rd = new StreamReader(rdfs, Encoding.ASCII);
            StreamReader rd = new StreamReader(rdfs, Encoding.Default);
            rd.BaseStream.Seek(0, SeekOrigin.Begin);

            rd.DiscardBufferedData();
            rd.BaseStream.Seek(0, SeekOrigin.Begin);
            rd.BaseStream.Position = 0;

            string allLine = "";
            string strLine = "";
            strLine = "";
            strLine = rd.ReadLine();
            string history_session = "";
            string history_type = "";
            string history_cb_time = "";

            if (txt_cbMonTime.Text != "")
            {
                history_cb_time += "time = 1," + txt_cbMonTime.Text + "\r\n";
            }
            if (txt_cbTueTime.Text != "")
            {
                history_cb_time += "time = 2," + txt_cbTueTime.Text + "\r\n";
            }
            if (txt_cbWedTime.Text != "")
            {
                history_cb_time += "time = 3," + txt_cbWedTime.Text + "\r\n";
            }
            if (txt_cbThuTime.Text != "")
            {
                history_cb_time += "time = 4," + txt_cbThuTime.Text + "\r\n";
            }
            if (txt_cbFriTime.Text != "")
            {
                history_cb_time += "time = 5," + txt_cbFriTime.Text + "\r\n";
            }
            if (txt_cbSatTime.Text != "")
            {
                history_cb_time += "time = 6," + txt_cbSatTime.Text + "\r\n";
            }
            if (txt_cbSunTime.Text != "")
            {
                history_cb_time += "time = 7," + txt_cbSunTime.Text + "\r\n";
            }

            do
            {
                if (strLine.Contains("[history]"))
                {
                    history_session = "history";
                    if (history_type == "1")
                    {
                        allLine += history_cb_time;
                        allLine += "\r\n";
                    }
                    else if (history_type != "")
                    {
                        allLine += "\r\n";
                    }
                }
                if (strLine.Contains("type"))
                {
                    history_type = strLine.Split('=')[1].Split(';')[0].Replace("\t", "").Replace(" ", "");
                }
                if (strLine.Contains("period") && history_type == "1")
                {
                    strLine = "period = " + txt_cbperiod.Text;
                }
                if (strLine.Contains("min_team_player") && history_type == "1")
                {
                    strLine = "min_team_player = " + txt_cbminnum.Text;
                }
                if (strLine.Contains("time") && history_session == "history" && history_type == "1")
                {
                    strLine = "";
                }
                if (strLine != "")
                {
                    allLine += strLine + "\r\n";
                }

                strLine = "";
                strLine = rd.ReadLine();
            } while (strLine != null);
            rd.Close();
            rdfs.Close();

            File.WriteAllText(LoginHistoryFile, allLine);

            m_SGExHandle.ReloadLoginServer();
        }

        #endregion

        #region //充值设置
        public struct Recharge_Conf
        {
            public string leftNum;
            public string rightNum;
            public string id1;
            public string name1;
            public string num1;
            public string id2;
            public string name2;
            public string num2;
            public string id3;
            public string name3;
            public string num3;
            public string id4;
            public string name4;
            public string num4;
            public string id5;
            public string name5;
            public string num5;
        };

        private void LoadAutoRchConf()
        {
            //load_Item_List
            string allItemDefines = CItemCtrl.load_Item_Defines(System.AppDomain.CurrentDomain.BaseDirectory + "profile");
            var allItemDefinesArr = allItemDefines.Replace("\t", "").Split(';');

            this.lstv_itemList.Items.Clear();
            foreach (var _def in allItemDefinesArr)
            {
                var _nameWithIdarr = _def.Split(',');
                if (_nameWithIdarr.Length > 1)
                {
                    string _name = _nameWithIdarr[0];
                    string _id = _nameWithIdarr[1];
                    ListViewItem lvi = new ListViewItem();
                    //lvi.ImageIndex = i;     //通过与imageList绑定，显示imageList中第i项图标 
                    lvi.Text = _id.Replace("\t", "");
                    //lvi.SubItems.Add(_id);
                    lvi.SubItems.Add(_name);
                    this.lstv_itemList.Items.Add(lvi);
                }
            }
            this.lstv_itemList.EndUpdate();  //结束数据处理，UI界面一次性绘制

            cbx_rcgConfNum.SelectedIndex = 0;
            string conf_num = cbx_rcgConfNum.Text;
            OnHandleRcgConfNum_SelectedIndexChanged(conf_num);

            cbx_rcgLevel.SelectedIndex = 0;
            string rcgLevel = "档位：" + cbx_rcgLevel.Text;
            DoRcgLevelSelectedIndexChanged(rcgLevel);
        }

        private void btn_rcgInit_Click(object sender, EventArgs e)
        {
            if (btn_sql.Text == "连接数据库")
            {
                MessageBox.Show("请先连接数据库！");
                return;
            }

            //加载设定的配置
            Recharge_Conf[] conf = new Recharge_Conf[7];
            for (int i = 0; i < 7; i++)
            {
                string rcgLevel = "档位：" + (i + 1);
                string rcgConf = CIniCtrl.ReadIniData(rcgLevel, "rcgConf", "", serverIni);
                string leftNum = CIniCtrl.ReadIniData(rcgLevel, "leftNum", "", serverIni);
                string rightNum = CIniCtrl.ReadIniData(rcgLevel, "rightNum", "", serverIni);

                string id = "";
                string name = "";
                string num = "";
                //读取
                id = CIniCtrl.ReadIniData("Recharge", rcgConf + "_id1", "", serverIni);
                name = CIniCtrl.ReadIniData("Recharge", rcgConf + "_name1", "", serverIni);
                num = CIniCtrl.ReadIniData("Recharge", rcgConf + "_num1", "", serverIni);
                //配置内容
                conf[i].id1 = id == "" ? "0" : id;
                conf[i].name1 = name == "" ? "空" : name;
                conf[i].num1 = num == "" ? "0" : num;

                //读取
                id = CIniCtrl.ReadIniData("Recharge", rcgConf + "_id2", "", serverIni);
                name = CIniCtrl.ReadIniData("Recharge", rcgConf + "_name2", "", serverIni);
                num = CIniCtrl.ReadIniData("Recharge", rcgConf + "_num2", "", serverIni);
                //配置内容
                conf[i].id2 = id == "" ? "0" : id;
                conf[i].name2 = name == "" ? "空" : name;
                conf[i].num2 = num == "" ? "0" : num;

                //读取
                id = CIniCtrl.ReadIniData("Recharge", rcgConf + "_id3", "", serverIni);
                name = CIniCtrl.ReadIniData("Recharge", rcgConf + "_name3", "", serverIni);
                num = CIniCtrl.ReadIniData("Recharge", rcgConf + "_num3", "", serverIni);
                //配置内容
                conf[i].id3 = id == "" ? "0" : id;
                conf[i].name3 = name == "" ? "空" : name;
                conf[i].num3 = num == "" ? "0" : num;

                //读取
                id = CIniCtrl.ReadIniData("Recharge", rcgConf + "_id4", "", serverIni);
                name = CIniCtrl.ReadIniData("Recharge", rcgConf + "_name4", "", serverIni);
                num = CIniCtrl.ReadIniData("Recharge", rcgConf + "_num4", "", serverIni);
                //配置内容
                conf[i].id4 = id == "" ? "0" : id;
                conf[i].name4 = name == "" ? "空" : name;
                conf[i].num4 = num == "" ? "0" : num;

                //读取
                id = CIniCtrl.ReadIniData("Recharge", rcgConf + "_id5", "", serverIni);
                name = CIniCtrl.ReadIniData("Recharge", rcgConf + "_name5", "", serverIni);
                num = CIniCtrl.ReadIniData("Recharge", rcgConf + "_num5", "", serverIni);
                //配置内容
                conf[i].id5 = id == "" ? "0" : id;
                conf[i].name5 = name == "" ? "空" : name;
                conf[i].num5 = num == "" ? "0" : num;

                leftNum = leftNum == "" ? "0" : leftNum;
                rightNum = rightNum == "" ? "-1" : rightNum;
                if (Int32.Parse(leftNum) >= Int32.Parse(rightNum))
                {
                    conf[i].leftNum = "0";
                    conf[i].rightNum = "-1";
                }
                else
                {
                    conf[i].leftNum = leftNum;
                    conf[i].rightNum = rightNum;
                }
            }

            string cmd = "";
            string ret = "";
            //删除[recharge_history]表
            cmd = @"DROP TABLE recharge_history";
            ret = CSGHelper.SqlCommand(cmd);
            if (ret != "success")
            {
                MessageBox.Show("删除[recharge_history]表失败！");
                //return;
            }
            //删除game_acc 的 Account_Insert触发器
            cmd = @"drop trigger Account_Insert ";
            ret = CSGHelper.SqlCommand(cmd);
            if (ret != "success")
            {
                MessageBox.Show("删除game_acc 的 Account_Insert触发器失败！");
                //return;
            }
            //创建[recharge_history]表
            cmd =
@"create table [recharge_history]
(
account varchar(21) PRIMARY KEY,
point int default 0,
time datetime
)";
            ret = CSGHelper.SqlCommand(cmd);
            if (ret != "success")
            {
                MessageBox.Show("创建[recharge_history]表失败！");
                return;
            }
            //同步账户名
            cmd =
@"insert INTO recharge_history(account,point)
SELECT account,point
FROM game_acc";
            ret = CSGHelper.SqlCommand(cmd);
            if (ret != "success")
            {
                MessageBox.Show("同步game_acc账户名和point到recharge_history失败！");
                return;
            }
            //创建game_acc 的 Account_Insert触发器，game_acc有插入时同样插入信息到recharge_history
            cmd =
@"CREATE TRIGGER Account_Insert ON dbo.game_acc 
FOR INSERT 
AS 
DECLARE @account varchar(21)
Select @account=account from inserted 
insert INTO recharge_history(account,point) values (@account,0)";
            ret = CSGHelper.SqlCommand(cmd);
            if (ret != "success")
            {
                MessageBox.Show("创建game_acc 的 Account_Insert触发器失败！");
                return;
            }
            //创建recharge_history 的 Recharge_Update触发器，充值更新point，触发更新game_acc的point
            cmd =
 @"CREATE TRIGGER Recharge_Update ON dbo.recharge_history 
FOR UPDATE 
AS 
DECLARE @account varchar(21)
DECLARE @point int
DECLARE @old_point int
DECLARE @add_point int
DECLARE @old_point_acc int
DECLARE @new_point_acc int
Select @account=account from inserted 
Select @point=point from inserted 
SELECT @old_point=point FROM DELETED
SET @add_point = @point - @old_point

Select @old_point_acc=point from dbo.game_acc where account = @account
set @new_point_acc = @old_point_acc + @add_point
Update dbo.game_acc set point = @new_point_acc where account = @account

DECLARE @cardid varchar(100)
DECLARE @dtDate datetime
set @dtDate = getdate()
SET @cardid = CONVERT(varchar(100), GETDATE(), 21)
PRINT @cardid

DECLARE @DataID1 int
DECLARE @Number1 int
DECLARE @DataID2 int
DECLARE @Number2 int
DECLARE @DataID3 int
DECLARE @Number3 int
DECLARE @DataID4 int
DECLARE @Number4 int
DECLARE @DataID5 int
DECLARE @Number5 int

set @DataID1 = 0
set @Number1 = 0
set @DataID2 = 0
set @Number2 = 0
set @DataID3 = 0
set @Number3 = 0
set @DataID4 = 0
set @Number4 = 0
set @DataID5 = 0
set @Number5 = 0

if (@add_point >= " + conf[0].leftNum + " AND @add_point < " + conf[0].rightNum + @")
	begin
	set @DataID1 = " + conf[0].id1 + @"
	set @Number1 = " + conf[0].num1 + @"
	set @DataID2 = " + conf[0].id2 + @"
	set @Number2 = " + conf[0].num2 + @"
	set @DataID3 = " + conf[0].id3 + @"
	set @Number3 = " + conf[0].num3 + @"
	set @DataID4 = " + conf[0].id4 + @"
	set @Number4 = " + conf[0].num4 + @"
	set @DataID5 = " + conf[0].id5 + @"
	set @Number5 = " + conf[0].num5 + @"
	end
else if (@add_point >= " + conf[1].leftNum + " AND @add_point < " + conf[1].rightNum + @")
	begin
	set @DataID1 = " + conf[1].id1 + @"
	set @Number1 = " + conf[1].num1 + @"
	set @DataID2 = " + conf[1].id2 + @"
	set @Number2 = " + conf[1].num2 + @"
	set @DataID3 = " + conf[1].id3 + @"
	set @Number3 = " + conf[1].num3 + @"
	set @DataID4 = " + conf[1].id4 + @"
	set @Number4 = " + conf[1].num4 + @"
	set @DataID5 = " + conf[1].id5 + @"
	set @Number5 = " + conf[1].num5 + @"
	end
else if (@add_point >= " + conf[2].leftNum + " AND @add_point < " + conf[2].rightNum + @")
	begin
	set @DataID1 = " + conf[2].id1 + @"
	set @Number1 = " + conf[2].num1 + @"
	set @DataID2 = " + conf[2].id2 + @"
	set @Number2 = " + conf[2].num2 + @"
	set @DataID3 = " + conf[2].id3 + @"
	set @Number3 = " + conf[2].num3 + @"
	set @DataID4 = " + conf[2].id4 + @"
	set @Number4 = " + conf[2].num4 + @"
	set @DataID5 = " + conf[2].id5 + @"
	set @Number5 = " + conf[2].num5 + @"
	end
else if (@add_point >= " + conf[3].leftNum + " AND @add_point < " + conf[3].rightNum + @")
	begin
	set @DataID1 = " + conf[3].id1 + @"
	set @Number1 = " + conf[3].num1 + @"
	set @DataID2 = " + conf[3].id2 + @"
	set @Number2 = " + conf[3].num2 + @"
	set @DataID3 = " + conf[3].id3 + @"
	set @Number3 = " + conf[3].num3 + @"
	set @DataID4 = " + conf[3].id4 + @"
	set @Number4 = " + conf[3].num4 + @"
	set @DataID5 = " + conf[3].id5 + @"
	set @Number5 = " + conf[3].num5 + @"
	end
else if (@add_point >= " + conf[4].leftNum + " AND @add_point < " + conf[4].rightNum + @")
	begin
	set @DataID1 = " + conf[4].id1 + @"
	set @Number1 = " + conf[4].num1 + @"
	set @DataID2 = " + conf[4].id2 + @"
	set @Number2 = " + conf[4].num2 + @"
	set @DataID3 = " + conf[4].id3 + @"
	set @Number3 = " + conf[4].num3 + @"
	set @DataID4 = " + conf[4].id4 + @"
	set @Number4 = " + conf[4].num4 + @"
	set @DataID5 = " + conf[4].id5 + @"
	set @Number5 = " + conf[4].num5 + @"
	end
else if (@add_point >= " + conf[5].leftNum + " AND @add_point < " + conf[5].rightNum + @")
	begin
	set @DataID1 = " + conf[5].id1 + @"
	set @Number1 = " + conf[5].num1 + @"
	set @DataID2 = " + conf[5].id2 + @"
	set @Number2 = " + conf[5].num2 + @"
	set @DataID3 = " + conf[5].id3 + @"
	set @Number3 = " + conf[5].num3 + @"
	set @DataID4 = " + conf[5].id4 + @"
	set @Number4 = " + conf[5].num4 + @"
	set @DataID5 = " + conf[5].id5 + @"
	set @Number5 = " + conf[5].num5 + @"
	end
else if (@add_point >= " + conf[6].leftNum + " AND @add_point < " + conf[6].rightNum + @")
	begin
	set @DataID1 = " + conf[6].id1 + @"
	set @Number1 = " + conf[6].num1 + @"
	set @DataID2 = " + conf[6].id2 + @"
	set @Number2 = " + conf[6].num2 + @"
	set @DataID3 = " + conf[6].id3 + @"
	set @Number3 = " + conf[6].num3 + @"
	set @DataID4 = " + conf[6].id4 + @"
	set @Number4 = " + conf[6].num4 + @"
	set @DataID5 = " + conf[6].id5 + @"
	set @Number5 = " + conf[6].num5 + @"
	end

INSERT INTO " + txt_sanvtName.Text + @".dbo.vitem (Account,Disable,Card,Login_time,Get_time,SName,CharName,Type,
DataID1,Number1,DataID2,Number2,DataID3,Number3,DataID4,Number4,DataID5,Number5)
values (@account,0,@cardid,@dtDate,@dtDate,0,0,0,
@DataID1,@Number1,@DataID2,@Number2,@DataID3,@Number3,@DataID4,@Number4,@DataID5,@Number5)";
            ret = CSGHelper.SqlCommand(cmd);

            if (ret == "success")
            {
                string msg = @"当前设定如下:

本服不设定非固定充值，非以下固定充值将以第三方设置比例结算。

(1)范围：" + conf[0].leftNum + "-" + conf[0].rightNum + "额外获得：" + conf[0].name1 + "*" + conf[0].num1 + ";" + conf[0].name2 + "*" + conf[0].num2 + ";" + conf[0].name3 + "*" + conf[0].num3 + ";" + conf[0].name4 + "*" + conf[0].num4 + ";" + conf[0].name5 + "*" + conf[0].num5 + @"；

(2)范围：" + conf[1].leftNum + "-" + conf[1].rightNum + "额外获得：" + conf[1].name1 + "*" + conf[1].num1 + ";" + conf[1].name2 + "*" + conf[1].num2 + ";" + conf[1].name3 + "*" + conf[1].num3 + ";" + conf[1].name4 + "*" + conf[1].num4 + ";" + conf[1].name5 + "*" + conf[1].num5 + @"；

(3)范围：" + conf[2].leftNum + "-" + conf[2].rightNum + "额外获得：" + conf[2].name1 + "*" + conf[2].num1 + ";" + conf[2].name2 + "*" + conf[2].num2 + ";" + conf[2].name3 + "*" + conf[2].num3 + ";" + conf[2].name4 + "*" + conf[2].num4 + ";" + conf[2].name5 + "*" + conf[2].num5 + @"；

(4)范围：" + conf[3].leftNum + "-" + conf[3].rightNum + "额外获得：" + conf[3].name1 + "*" + conf[3].num1 + ";" + conf[3].name2 + "*" + conf[3].num2 + ";" + conf[3].name3 + "*" + conf[3].num3 + ";" + conf[3].name4 + "*" + conf[3].num4 + ";" + conf[3].name5 + "*" + conf[3].num5 + @"；

(5)范围：" + conf[4].leftNum + "-" + conf[4].rightNum + "额外获得：" + conf[4].name1 + "*" + conf[4].num1 + ";" + conf[4].name2 + "*" + conf[4].num2 + ";" + conf[4].name3 + "*" + conf[4].num3 + ";" + conf[4].name4 + "*" + conf[4].num4 + ";" + conf[4].name5 + "*" + conf[4].num5 + @"；

(6)范围：" + conf[5].leftNum + "-" + conf[5].rightNum + "额外获得：" + conf[5].name1 + "*" + conf[5].num1 + ";" + conf[5].name2 + "*" + conf[5].num2 + ";" + conf[5].name3 + "*" + conf[5].num3 + ";" + conf[5].name4 + "*" + conf[5].num4 + ";" + conf[5].name5 + "*" + conf[5].num5 + @"；

(6)范围：" + conf[6].leftNum + "-" + conf[6].rightNum + "额外获得：" + conf[6].name1 + "*" + conf[6].num1 + ";" + conf[6].name2 + "*" + conf[6].num2 + ";" + conf[6].name3 + "*" + conf[6].num3 + ";" + conf[6].name4 + "*" + conf[6].num4 + ";" + conf[6].name5 + "*" + conf[6].num5 + @"；";
                MessageBox.Show(msg);
            }
            else
            {
                MessageBox.Show("创建recharge_history 的 Recharge_Update触发器失败！");
                return;
            }
        }

        //static string cur_slt_id = "";
        //static string cur_slt_name = "";
        static int itemDefSltIndex = 0;
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstv_itemList.SelectedIndices != null && lstv_itemList.SelectedIndices.Count > 0)
            {
                lstv_itemList.Items[itemDefSltIndex].BackColor = Color.Transparent;
                itemDefSltIndex = this.lstv_itemList.SelectedItems[0].Index;
                lstv_itemList.Items[itemDefSltIndex].BackColor = Color.Pink;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            string cur_slt_id = lstv_itemList.Items[itemDefSltIndex].SubItems[0].Text;
            string cur_slt_name = lstv_itemList.Items[itemDefSltIndex].SubItems[1].Text;

            if (cur_slt_id != "" && cur_slt_name != "")
            {
                if (cbx_confNum.SelectedIndex == 0)
                {
                    txt_confNum1.Text = "1";
                    txt_confName1.Text = cur_slt_name;
                    txt_confId1.Text = cur_slt_id;
                }
                else if (cbx_confNum.SelectedIndex == 1)
                {
                    txt_confNum2.Text = "1";
                    txt_confName2.Text = cur_slt_name;
                    txt_confId2.Text = cur_slt_id;
                }
                else if (cbx_confNum.SelectedIndex == 2)
                {
                    txt_confNum3.Text = "1";
                    txt_confName3.Text = cur_slt_name;
                    txt_confId3.Text = cur_slt_id;
                }
                else if (cbx_confNum.SelectedIndex == 3)
                {
                    txt_confNum4.Text = "1";
                    txt_confName4.Text = cur_slt_name;
                    txt_confId4.Text = cur_slt_id;
                }
                else if (cbx_confNum.SelectedIndex == 4)
                {
                    txt_confNum5.Text = "1";
                    txt_confName5.Text = cur_slt_name;
                    txt_confId5.Text = cur_slt_id;
                }
            }
        }

        private void btn_reLoadList_Click(object sender, EventArgs e)
        {
            LoadAutoRchConf();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //ListViewItem foundItem = this.listView1.FindItemWithText(this.txt_srchItemName.Text, true, cur_select_index + 1);    //参数1：要查找的文本；参数2：是否子项也要查找；参数3：开始查找位置  
            ListViewItem foundItem = null;
            lstv_itemList.Items[itemDefSltIndex].BackColor = Color.Transparent;
            int i = itemDefSltIndex + 1;
            if (i == lstv_itemList.Items.Count)
            {
                i = 0;
            }
            while (i != itemDefSltIndex)
            {
                if (lstv_itemList.Items[i].SubItems[1].Text.Contains(txt_srchItemName.Text) || lstv_itemList.Items[i].Text == txt_srchItemName.Text)
                {
                    foundItem = lstv_itemList.Items[i];
                }
                i++;
                if (i == lstv_itemList.Items.Count)
                {
                    i = 0;
                }
            }

            if (foundItem != null)
            {
                this.lstv_itemList.TopItem = foundItem;  //定位到该项
                lstv_itemList.Items[foundItem.Index].Focused = true;
                lstv_itemList.Items[foundItem.Index].Selected = true;
                foundItem.BackColor = Color.Pink;
                itemDefSltIndex = foundItem.Index;
            }
        }

        private void btn_regConfSave_Click(object sender, EventArgs e)
        {
            serverIni = System.AppDomain.CurrentDomain.BaseDirectory + "配置文件.ini";

            //配置编号
            string conf_num = cbx_rcgConfNum.Text;

            string id = "";
            string name = "";
            string num = "";
            //配置内容
            id = txt_confId1.Text;
            name = txt_confName1.Text;
            num = txt_confNum1.Text;
            //保存
            CIniCtrl.WriteIniData("Recharge", conf_num + "_id1", id, serverIni);
            CIniCtrl.WriteIniData("Recharge", conf_num + "_name1", name, serverIni);
            CIniCtrl.WriteIniData("Recharge", conf_num + "_num1", num, serverIni);

            //配置内容
            id = txt_confId2.Text;
            name = txt_confName2.Text;
            num = txt_confNum2.Text;
            //保存
            CIniCtrl.WriteIniData("Recharge", conf_num + "_id2", id, serverIni);
            CIniCtrl.WriteIniData("Recharge", conf_num + "_name2", name, serverIni);
            CIniCtrl.WriteIniData("Recharge", conf_num + "_num2", num, serverIni);

            //配置内容
            id = txt_confId3.Text;
            name = txt_confName3.Text;
            num = txt_confNum3.Text;
            //保存
            CIniCtrl.WriteIniData("Recharge", conf_num + "_id3", id, serverIni);
            CIniCtrl.WriteIniData("Recharge", conf_num + "_name3", name, serverIni);
            CIniCtrl.WriteIniData("Recharge", conf_num + "_num3", num, serverIni);

            //配置内容
            id = txt_confId4.Text;
            name = txt_confName4.Text;
            num = txt_confNum4.Text;
            //保存
            CIniCtrl.WriteIniData("Recharge", conf_num + "_id4", id, serverIni);
            CIniCtrl.WriteIniData("Recharge", conf_num + "_name4", name, serverIni);
            CIniCtrl.WriteIniData("Recharge", conf_num + "_num4", num, serverIni);

            //配置内容
            id = txt_confId5.Text;
            name = txt_confName5.Text;
            num = txt_confNum5.Text;
            //保存
            CIniCtrl.WriteIniData("Recharge", conf_num + "_id5", id, serverIni);
            CIniCtrl.WriteIniData("Recharge", conf_num + "_name5", name, serverIni);
            CIniCtrl.WriteIniData("Recharge", conf_num + "_num5", num, serverIni);
        }

        private void cbx_rcgConfNum_SelectedIndexChanged(object sender, EventArgs e)
        {
            //配置编号
            string conf_num = cbx_rcgConfNum.Text;
            OnHandleRcgConfNum_SelectedIndexChanged(conf_num);
            cbx_rcgConf.SelectedIndex = cbx_rcgConfNum.SelectedIndex;
        }

        private void OnHandleRcgConfNum_SelectedIndexChanged(string conf_num)
        {
            serverIni = System.AppDomain.CurrentDomain.BaseDirectory + "配置文件.ini";
            string id = "";
            string name = "";
            string num = "";

            //读取
            id = CIniCtrl.ReadIniData("Recharge", conf_num + "_id1", "", serverIni);
            name = CIniCtrl.ReadIniData("Recharge", conf_num + "_name1", "", serverIni);
            num = CIniCtrl.ReadIniData("Recharge", conf_num + "_num1", "", serverIni);
            //配置内容
            txt_confId1.Text = id;
            txt_confName1.Text = name;
            txt_confNum1.Text = num;

            //读取
            id = CIniCtrl.ReadIniData("Recharge", conf_num + "_id2", "", serverIni);
            name = CIniCtrl.ReadIniData("Recharge", conf_num + "_name2", "", serverIni);
            num = CIniCtrl.ReadIniData("Recharge", conf_num + "_num2", "", serverIni);
            //配置内容
            txt_confId2.Text = id;
            txt_confName2.Text = name;
            txt_confNum2.Text = num;

            //读取
            id = CIniCtrl.ReadIniData("Recharge", conf_num + "_id3", "", serverIni);
            name = CIniCtrl.ReadIniData("Recharge", conf_num + "_name3", "", serverIni);
            num = CIniCtrl.ReadIniData("Recharge", conf_num + "_num3", "", serverIni);
            //配置内容
            txt_confId3.Text = id;
            txt_confName3.Text = name;
            txt_confNum3.Text = num;

            //读取
            id = CIniCtrl.ReadIniData("Recharge", conf_num + "_id4", "", serverIni);
            name = CIniCtrl.ReadIniData("Recharge", conf_num + "_name4", "", serverIni);
            num = CIniCtrl.ReadIniData("Recharge", conf_num + "_num4", "", serverIni);
            //配置内容
            txt_confId4.Text = id;
            txt_confName4.Text = name;
            txt_confNum4.Text = num;

            //读取
            id = CIniCtrl.ReadIniData("Recharge", conf_num + "_id5", "", serverIni);
            name = CIniCtrl.ReadIniData("Recharge", conf_num + "_name5", "", serverIni);
            num = CIniCtrl.ReadIniData("Recharge", conf_num + "_num5", "", serverIni);
            //配置内容
            txt_confId5.Text = id;
            txt_confName5.Text = name;
            txt_confNum5.Text = num;
        }

        private void DoRcgLevelSelectedIndexChanged(string rcgLevel)
        {
            string rcgConf = CIniCtrl.ReadIniData(rcgLevel, "rcgConf", "", serverIni);
            string leftNum = CIniCtrl.ReadIniData(rcgLevel, "leftNum", "", serverIni);
            string rightNum = CIniCtrl.ReadIniData(rcgLevel, "rightNum", "", serverIni);

            cbx_rcgConf.Text = rcgConf;
            cbx_rcgConfNum.Text = rcgConf;
            txt_betmLeft.Text = leftNum;
            txt_betmRight.Text = rightNum;
        }
        private void cbx_rcgLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            string rcgLevel = "档位：" + cbx_rcgLevel.Text;
            DoRcgLevelSelectedIndexChanged(rcgLevel);
        }

        private void btn_rcgLevelSave_Click(object sender, EventArgs e)
        {
            //获取档位
            string rcgLevel = "档位：" + cbx_rcgLevel.Text;
            //获取配置
            string rcgConf = cbx_rcgConf.Text;
            OnHandleRcgConfNum_SelectedIndexChanged(rcgConf);

            //获取间隔
            string leftNum = txt_betmLeft.Text;
            string rightNum = txt_betmRight.Text;

            //写配置
            CIniCtrl.WriteIniData(rcgLevel, "rcgConf", rcgConf, serverIni);
            CIniCtrl.WriteIniData(rcgLevel, "leftNum", leftNum, serverIni);
            CIniCtrl.WriteIniData(rcgLevel, "rightNum", rightNum, serverIni);

            //根据当前配置更新设定

        }

        private void cbx_rcgConf_SelectedIndexChanged(object sender, EventArgs e)
        {
            string rcgConf = cbx_rcgConf.Text;
            cbx_rcgConfNum.SelectedIndex = cbx_rcgConf.SelectedIndex;
            OnHandleRcgConfNum_SelectedIndexChanged(rcgConf);
        }

        private void txt_sanvtName_TextChanged(object sender, EventArgs e)
        {
            //写入
            CIniCtrl.WriteIniData("Server", "sanvtName", txt_sanvtName.Text, serverIni);
        }

        #endregion

        #region  //技能管理


        private void button7_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("请确保游戏不处于运行状态！", "", MessageBoxButtons.OKCancel);
            if (dr != DialogResult.OK)//如果点击“确定”按钮
            {
                return;
            }

            CSkillCtrl.ClearAllSoulSkills(txt_svrForder.Text);
            CSkillCtrl.SaveSkillsData(txt_svrForder.Text + "\\DataBase\\saves\\skill.dat");

            MessageBox.Show("完成");
        }

        private void LoadSkillMrgInfos()
        {
            CPlayerCtrl.LoadPlayerInfos(txt_svrForder.Text + "\\DataBase\\saves\\players.dat");
            FillPlayerLstView();

            CSkillCtrl.LoadSkillsData(txt_svrForder.Text + "\\DataBase\\saves\\skill.dat");
            FillSkillLstView();
        }

        private void FillSkillLstView()
        {
            string allSkillDefines = CSkillCtrl.load_Skill_Defines(System.AppDomain.CurrentDomain.BaseDirectory + "profile");
            var allSkillDefinesArr = allSkillDefines.Replace("\t", "").Split(';');

            this.lsv_skillDef.Items.Clear();
            foreach (var _def in allSkillDefinesArr)
            {
                var _nameWithIdarr = _def.Split(',');
                if (_nameWithIdarr.Length > 1)
                {
                    string _name = _nameWithIdarr[0];
                    string _id = _nameWithIdarr[1];
                    ListViewItem lvi = new ListViewItem();
                    //lvi.ImageIndex = i;     //通过与imageList绑定，显示imageList中第i项图标 
                    lvi.Text = _id.Replace("\t", "");
                    //lvi.SubItems.Add(_id);
                    lvi.SubItems.Add(_name);
                    this.lsv_skillDef.Items.Add(lvi);
                }
            }
            this.lsv_skillDef.EndUpdate();  //结束数据处理，UI界面一次性绘制
        }

        private void FillPlayerLstView()
        {
            var players = CPlayerCtrl.GetAttrList();
            this.lstv_playersInfo.Items.Clear();
            foreach (var player in players)
            {
                string _acc = CFormat.GameStrToSimpleCN(player.Account);
                string _name = CFormat.GameStrToSimpleCN(player.Name);

                ListViewItem lvi = new ListViewItem();
                //lvi.ImageIndex = i;     //通过与imageList绑定，显示imageList中第i项图标 
                lvi.Text = _acc.Replace("\t", "");
                //lvi.SubItems.Add(_id);
                lvi.SubItems.Add(_name);
                this.lstv_playersInfo.Items.Add(lvi);
            }
            this.lstv_playersInfo.EndUpdate();  //结束数据处理，UI界面一次性绘制
        }



        static int playersInfoSltIndex = 0;
        private void btn_playerSrch_Click(object sender, EventArgs e)
        {
            ListViewItem foundItem = null;
            lstv_playersInfo.Items[playersInfoSltIndex].BackColor = Color.Transparent;
            int i = playersInfoSltIndex + 1;
            if (i == lstv_playersInfo.Items.Count)
            {
                i = 0;
            }
            while (i != playersInfoSltIndex)
            {
                if (lstv_playersInfo.Items[i].SubItems[1].Text.Contains(txt_srchPlayerName.Text) || lstv_playersInfo.Items[i].Text.Contains(txt_srchPlayerName.Text))
                {
                    foundItem = lstv_playersInfo.Items[i];
                    break;
                }
                i++;
                if (i == lstv_playersInfo.Items.Count)
                {
                    i = 0;
                }
            }

            if (foundItem != null)
            {
                this.lstv_playersInfo.TopItem = foundItem;  //定位到该项
                lstv_playersInfo.Items[foundItem.Index].Focused = true;
                lstv_playersInfo.Items[foundItem.Index].Selected = true;
                foundItem.BackColor = Color.Pink;
                playersInfoSltIndex = foundItem.Index;
            }
        }

        static int skillDefSltIndex = 0;
        private void btn_skillSrch_Click(object sender, EventArgs e)
        {
            ListViewItem foundItem = null;
            lsv_skillDef.Items[skillDefSltIndex].BackColor = Color.Transparent;
            int i = skillDefSltIndex + 1;
            if (i == lsv_skillDef.Items.Count)
            {
                i = 0;
            }
            while (i != skillDefSltIndex)
            {
                if (lsv_skillDef.Items[i].SubItems[1].Text.Contains(txt_srchSkillName.Text) || lsv_skillDef.Items[i].Text.Contains(txt_srchSkillName.Text))
                {
                    foundItem = lsv_skillDef.Items[i];
                    break;
                }
                i++;
                if (i == lsv_skillDef.Items.Count)
                {
                    i = 0;
                }
            }

            if (foundItem != null)
            {
                this.lsv_skillDef.TopItem = foundItem;  //定位到该项
                lsv_skillDef.Items[foundItem.Index].Focused = true;
                lsv_skillDef.Items[foundItem.Index].Selected = true;
                foundItem.BackColor = Color.Pink;
                skillDefSltIndex = foundItem.Index;
            }
        }

        private void lstv_playersInfo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstv_playersInfo.SelectedIndices != null && lstv_playersInfo.SelectedIndices.Count > 0)
            {
                lstv_playersInfo.Items[playersInfoSltIndex].BackColor = Color.Transparent;
                playersInfoSltIndex = this.lstv_playersInfo.SelectedItems[0].Index;
                lstv_playersInfo.Items[playersInfoSltIndex].BackColor = Color.Pink;
            }
        }

        private void lsv_skillDef_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lsv_skillDef.SelectedIndices != null && lsv_skillDef.SelectedIndices.Count > 0)
            {
                lsv_skillDef.Items[skillDefSltIndex].BackColor = Color.Transparent;
                skillDefSltIndex = this.lsv_skillDef.SelectedItems[0].Index;
                lsv_skillDef.Items[skillDefSltIndex].BackColor = Color.Pink;
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (txt_handlePlayers.Text.Replace(" ", "") != "")
            {
                txt_handlePlayers.Text += "\r\n";
            }
            txt_handlePlayers.Text += lstv_playersInfo.Items[playersInfoSltIndex].SubItems[0].Text
                                    + ";" + lstv_playersInfo.Items[playersInfoSltIndex].SubItems[1].Text;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (txt_handleSkills.Text.Replace(" ", "") != "")
            {
                txt_handleSkills.Text += "\r\n";
            }
            txt_handleSkills.Text += lsv_skillDef.Items[skillDefSltIndex].SubItems[0].Text
                                    + ";" + lsv_skillDef.Items[skillDefSltIndex].SubItems[1].Text;
        }

        private void button16_Click(object sender, EventArgs e)
        {
            CPlayerCtrl.PlayersAttrListClear();
            CPlayerCtrl.LoadPlayerInfos(txt_svrForder.Text + "\\DataBase\\saves\\players.dat");
            FillPlayerLstView();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            CSkillCtrl.LoadSkillsData(txt_svrForder.Text + "\\DataBase\\saves\\skill.dat");
            FillSkillLstView();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dr = MessageBox.Show("请确保游戏不处于运行状态！", "", MessageBoxButtons.OKCancel);
                if (dr != DialogResult.OK)//如果点击“确定”按钮
                {
                    return;
                }

                if (cbx_skillsAll.Checked)
                {
                    if (rdb_playersAll.Checked)
                    {
                        //增加 || 删除
                        if (rdb_skillAdd.Checked)
                        {
                            CSkillCtrl.AddAllPlayersAllSkills();
                        }
                        else if (rdb_skillDel.Checked)
                        {
                            CSkillCtrl.ClearAllPlayersAllSkills();
                        }
                    }
                    else if (rdb_playersSub.Checked)
                    {
                        //players
                        List<int> players = new List<int>();
                        if (txt_handleSkills.Text != "")
                        {
                            var tmp = txt_handlePlayers.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var item in tmp)
                            {
                                string name = item.Split(';')[1];
                                AccAttr attr = CPlayerCtrl.GetAttrByName(name);

                                players.Add((int)attr.nIndex);
                            }
                        }

                        //增加 || 删除
                        if (rdb_skillAdd.Checked)
                        {
                            CSkillCtrl.AddPlayerAllSkills(players);
                        }
                        else if (rdb_skillDel.Checked)
                        {
                            CSkillCtrl.ClearPlayerAllSkills(players);
                        }
                    }
                }
                else
                {
                    //skills
                    List<int> skills = new List<int>();
                    if (txt_handleSkills.Text != "")
                    {
                        var tmp = txt_handleSkills.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var item in tmp)
                        {
                            int skillid = int.Parse(item.Split(';')[0]);
                            skills.Add(skillid);
                        }
                    }

                    if (rdb_playersAll.Checked)
                    {
                        //增加 || 删除
                        if (rdb_skillAdd.Checked)
                        {
                            CSkillCtrl.AddAllPlayersSkills(skills);
                        }
                        else if (rdb_skillDel.Checked)
                        {
                            CSkillCtrl.ClearAllPlayersSkills(skills);
                        }
                    }
                    else if (rdb_playersSub.Checked)
                    {
                        //players
                        List<int> players = new List<int>();
                        if (txt_handleSkills.Text != "")
                        {
                            var tmp = txt_handlePlayers.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var item in tmp)
                            {
                                string name = item.Split(';')[1];
                                AccAttr attr = CPlayerCtrl.GetAttrByName(name);

                                players.Add((int)attr.nIndex);
                            }
                        }

                        //增加 || 删除
                        if (rdb_skillAdd.Checked)
                        {
                            CSkillCtrl.AddSubSkills(players, skills);
                        }
                        else if (rdb_skillDel.Checked)
                        {
                            CSkillCtrl.ClearSubSkills(players, skills);
                        }
                    }
                }
                CSkillCtrl.SaveSkillsData(txt_svrForder.Text + "\\DataBase\\saves\\skill.dat");
                MessageBox.Show("DONE!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void cbx_skillsAll_CheckedChanged(object sender, EventArgs e)
        {
            if (cbx_skillsAll.Checked)
            {
                button12.Enabled = false;
                txt_handleSkills.ReadOnly = true;
            }
            else
            {
                button12.Enabled = true;
                txt_handleSkills.ReadOnly = false;
            }
        }

        private void rdb_playersAll_CheckedChanged(object sender, EventArgs e)
        {
            if (rdb_playersAll.Checked)
            {
                button13.Enabled = false;
                txt_handlePlayers.ReadOnly = true;
            }
            else
            {
                button13.Enabled = true;
                txt_handlePlayers.ReadOnly = false;
            }
        }

        //一键按职业给予全技能
        private void button9_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("请确保游戏不处于运行状态！", "", MessageBoxButtons.OKCancel);
            if (dr != DialogResult.OK)//如果点击“确定”按钮
            {
                return;
            }
            //获取所有猛将、豪杰、军师、方士
            List<int> player_jobWarload = CPlayerCtrl.LoadAllWarloadIndex();
            List<int> player_jobLeader = CPlayerCtrl.LoadAllLeaderIndex();
            List<int> player_jobAdvisor = CPlayerCtrl.LoadAllAdvisorIndex();
            List<int> player_jobWizard = CPlayerCtrl.LoadAllWizardIndex();

            //构建按职业的List<int> skills
            List<int> skill_jobWarload = CSkillCtrl.LoadAllWarloadSkills();
            List<int> skill_jobLeader = CSkillCtrl.LoadAllLeaderSkills();
            List<int> skill_jobAdvisor = CSkillCtrl.LoadAllAdvisorSkills();
            List<int> skill_jobWizard = CSkillCtrl.LoadAllWizardSkills();

            CSkillCtrl.AddSubSkills(player_jobWarload, skill_jobWarload);
            CSkillCtrl.AddSubSkills(player_jobLeader, skill_jobLeader);
            CSkillCtrl.AddSubSkills(player_jobAdvisor, skill_jobAdvisor);
            CSkillCtrl.AddSubSkills(player_jobWizard, skill_jobWizard);

            CSkillCtrl.SaveSkillsData(txt_svrForder.Text + "\\DataBase\\saves\\skill.dat");
            MessageBox.Show("done!");
        }

        //一键按职业给予符合等级的全技能
        private void button11_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("请确保游戏不处于运行状态！", "", MessageBoxButtons.OKCancel);
            if (dr != DialogResult.OK)//如果点击“确定”按钮
            {
                return;
            }
            //获取所有猛将、豪杰、军师、方士
            List<AccAttr> player_jobWarload = CPlayerCtrl.LoadAllWarloadIndexDesc();
            List<AccAttr> player_jobLeader = CPlayerCtrl.LoadAllLeaderIndexDesc();
            List<AccAttr> player_jobAdvisor = CPlayerCtrl.LoadAllAdvisorIndexDesc();
            List<AccAttr> player_jobWizard = CPlayerCtrl.LoadAllWizardIndexDesc();

            //构建按职业的List<int> skills
            List<Magic_JobLimit_Str> skill_jobWarload = CSkillCtrl.LoadAllWarloadSkillsDesc();
            List<Magic_JobLimit_Str> skill_jobLeader = CSkillCtrl.LoadAllLeaderSkillsDesc();
            List<Magic_JobLimit_Str> skill_jobAdvisor = CSkillCtrl.LoadAllAdvisorSkillsDesc();
            List<Magic_JobLimit_Str> skill_jobWizard = CSkillCtrl.LoadAllWizardSkillsDesc();

            CSkillCtrl.AddSubSkills(player_jobWarload, skill_jobWarload, true);
            CSkillCtrl.AddSubSkills(player_jobLeader, skill_jobLeader, true);
            CSkillCtrl.AddSubSkills(player_jobAdvisor, skill_jobAdvisor, true);
            CSkillCtrl.AddSubSkills(player_jobWizard, skill_jobWizard, true);

            CSkillCtrl.SaveSkillsData(txt_svrForder.Text + "\\DataBase\\saves\\skill.dat");
            MessageBox.Show("DONE!");
        }

        #endregion

        #region//自动检测

        private void btn_startListen_Click(object sender, EventArgs e)
        {
            if (!m_Active)
            {
                MessageBox.Show("软件尚未激活！ 请联系软件发布人给予激活！");
                return;
            }
            ////封号操作需要数据库已经链接
            //if (lbl_sqlStatus.Text != "数据库已连接")
            //{
            //    MessageBox.Show("请先连接数据库！");
            //    return;
            //}

            if (btn_startListen.Text == "启动检测")
            {
                btn_startListen.Text = "停止检测";

                tm_strListen.Start();
            }
            else if (btn_startListen.Text == "停止检测")
            {
                tm_strListen.Stop();

                btn_startListen.Text = "启动检测";
            }
        }

        public struct Illegal
        {
            public string account;
            public string name;
            public string time_str;
        };

        private void FreezeCheck()
        {
            //读取loginserverlog,获取非法信息列表
            List<Illegal> nameList = GetIllegalList();

            int FreezeCount = 0;
            //更新列表
            lstv_GtList.Items.Clear();
            foreach (var name in nameList)
            {
                //过滤
                if (m_FreezeFilterList != null)
                {
                    bool filter = false;
                    String PureAcount = CFormat.PureString(name.account);
                    foreach (var item in m_FreezeFilterList)
                    {
                        if (PureAcount == item)
                        {
                            filter = true;
                            break;
                        }
                    }
                    if (filter)
                        continue;
                }

                ListViewItem lvi = new ListViewItem();

                lvi.Text = CFormat.PureString(name.account);
                //lvi.SubItems.Add(_id);
                lvi.SubItems.Add(CFormat.ToSimplified(CFormat.PureString(name.name)));
                lvi.SubItems.Add(name.time_str);

                if (cbx_AutoFreeze.Checked)
                {
                    string acc = CFormat.PureString(name.account);
                    //封号
                    try
                    {
                        string log = CSGHelper.FreezeAccount(acc, 1, "非法登录或者使用外挂！", "GM");
                        lvi.SubItems.Add(log);
                        if (log == "冻结成功")
                        {
                            FreezeCount++;
                        }
                        LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, acc + ":" + log, new StackTrace(new StackFrame(true)));
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, acc + ":" + ex.Message, new StackTrace(new StackFrame(true)));
                    }
                }
                lstv_GtList.Items.Add(lvi);
            }
            lstv_GtList.EndUpdate();

            lbl_IllCount.Text = nameList.Count.ToString();
            lbl_FreezeCount.Text = FreezeCount.ToString();
        }

        private static string[] m_FreezeFilterList;
        private void tm_strListen_Tick(object sender, EventArgs e)
        {
            FreezeCheck();
        }

        private List<Illegal> GetIllegalList()
        {
            List<Illegal> list = new List<Illegal>();

            //获取当前日期
            string time_str = DateTime.Now.ToString("yyyy-MM-dd");
            string log_name = txt_svrForder.Text +"\\Login\\log"+ "\\" + time_str + "_log_login.txt";
            if (!File.Exists(log_name))
            {
                return list;
            }
            else
            {
                try
                {
                    //读取
                    FileStream fs = new FileStream(log_name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    StreamReader reader = new StreamReader(fs, Encoding.GetEncoding(950));
                    reader.DiscardBufferedData();
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);
                    reader.BaseStream.Position = 0;

                    string strLine = "";
                    strLine = reader.ReadLine();
                    while (strLine != null)
                    {
                        if (strLine.Contains("SERIOUS: 發現外掛:"))
                        {
                            //分析出 游戏名和账户名
                            //<2016-05-13 04:31:14> SERIOUS: 發現外掛: 586, 光寒方士, 1681, finghter4(177, 2)
                            string time = strLine.Split(new string[] { "SERIOUS: 發現外掛:" }, StringSplitOptions.RemoveEmptyEntries)[0];
                            string tmp = strLine.Split(new string[] { "SERIOUS: 發現外掛:" }, StringSplitOptions.RemoveEmptyEntries)[1];
                            string name = tmp.Split(',')[1];
                            string account = tmp.Split(',')[3].Split('(')[0];

                            Illegal ill_player = new Illegal();

                            ill_player.account = account;
                            ill_player.name = name;
                            ill_player.time_str = time;

                            //判断是否已经记录此用户  -- 记录则更新其时间，没有则加入
                            bool get = false;
                            for (int i = 0; i < list.Count; i++)
                            {
                                if (list[i].account == ill_player.account)
                                {
                                    Illegal reitem = new Illegal();
                                    reitem = list[i];
                                    reitem.time_str = ill_player.time_str;
                                    list[i] = reitem;

                                    get = true;
                                    break;
                                }
                            }
                            if (!get)
                            {
                                list.Add(ill_player);
                            }
                        }
                        //<2016-05-08 00:01:36> Account info(tt8873351,online.dat,0x88671f25,2567829)
                        else if (strLine.Contains("Account info") && !strLine.Contains("qysg.dat"))
                        {
                            string time = strLine.Split(new string[] { "Account info" }, StringSplitOptions.RemoveEmptyEntries)[0];
                            string account = strLine.Split('(')[1].Split(',')[0];
                            Illegal ill_player = new Illegal();

                            ill_player.account = account;
                            ill_player.name = "非法登录程序";
                            ill_player.time_str = time;

                            //判断是否已经记录此用户  -- 记录则更新其时间，没有则加入
                            bool get = false;
                            for (int i = 0; i < list.Count; i++)
                            {
                                if (list[i].account == ill_player.account)
                                {
                                    Illegal reitem = new Illegal();
                                    reitem = list[i];
                                    reitem.time_str = ill_player.time_str;
                                    list[i] = reitem;

                                    get = true;
                                    break;
                                }
                            }
                            if (!get)
                            {
                                list.Add(ill_player);
                            }
                        }

                        strLine = null;
                        strLine = reader.ReadLine();
                    }

                    reader.Close();
                    fs.Close();
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, ex.Message, new StackTrace(new StackFrame(true)));
                }

            }
            return list;
        }

        private void btn_FreezeListAcc_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstv_GtList.Items.Count; i++)
            {
                string acc = lstv_GtList.Items[i].SubItems[0].Text;
                //封号
                try
                {
                    string log = CSGHelper.FreezeAccount(acc, 1, "非法登录或者使用外挂！", "GM");
                    LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, acc + ":" + log, new StackTrace(new StackFrame(true)));
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, acc + ":" + ex.Message, new StackTrace(new StackFrame(true)));
                }
            }
        }

        private void cbx_AutoFreeze_CheckedChanged(object sender, EventArgs e)
        {
            if (cbx_AutoFreeze.Checked)
            {
                CIniCtrl.WriteIniData("Config", "AutoFreeze", "Enable", serverIni);
            }
            else
            {
                CIniCtrl.WriteIniData("Config", "AutoFreeze", "Disable", serverIni);
            }
        }

        private void btn_FreezeFilterSet_Click(object sender, EventArgs e)
        {
            m_FreezeFilterList = rtb_FreezeFilter.Text.Split(',');
            CIniCtrl.WriteIniData("Config", "FreezeFilter", rtb_FreezeFilter.Text, serverIni);
        }

        private void IllCheckNow_Click(object sender, EventArgs e)
        {
            FreezeCheck();
        }

        #endregion
        
        #region //系统公共
        private void button18_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add(rTxtB_WorldWords.Text);
            UpdateWorldWordsList();
        }
        private void button21_Click_2(object sender, EventArgs e)
        {
            rTxtB_WorldWords.Text = "";
        }
        private void button22_Click(object sender, EventArgs e)
        {
            listBox1.Items.Remove(listBox1.SelectedItem);
            UpdateWorldWordsList();
        }
        private void UpdateWorldWordsList()
        {
            string liststring = "";
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                liststring += listBox1.Items[i].ToString();
                if (i < listBox1.Items.Count - 1)
                {
                    liststring += ";";
                }
            }

            CIniCtrl.WriteIniData("Config", "WorldWordsList", liststring, serverIni);
        }
        private void button15_Click(object sender, EventArgs e)
        {
            string words = rTxtB_WorldWords.Text;
            if (words != "" && words != string.Empty)
            {
                m_SGExHandle.SendWorldWords(words);
            }
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < listBox1.Items.Count && listBox1.SelectedIndex >= 0)
            {
                rTxtB_WorldWords.Text = listBox1.SelectedItem.ToString();
            }
        }
        #endregion

        #region //加持公共
        bool g_Stop15Talk = true;
        private void cbx_AutoStart15Talk_CheckedChanged(object sender, EventArgs e)
        {
            if (cbx_AutoStart15Talk.Checked)
            {
                CIniCtrl.WriteIniData("Config", "15TalkAutoStart", "Enable", serverIni);
            }
            else
            {
                CIniCtrl.WriteIniData("Config", "15TalkAutoStart", "Disable", serverIni);
            }
        }
        private void button21_Click_1(object sender, EventArgs e)
        {
            if (!m_Active)
            {
                MessageBox.Show("软件尚未激活！ 请联系软件发布人给予激活！");
                return;
            }
            if (g_Stop15Talk)
            {
                g_Stop15Talk = false;
                m_SGExHandle.SetConfigPath(txt_gameVerFile.Text);
                m_SGExHandle.SetFilterString(g_15NameFilter);
                m_SGExHandle.SetSrchInterVal(g_15SrchInterval);
                m_SGExHandle.SetMaxAnnString(g_15MaxAnn);
                m_SGExHandle.SetAnnItems(txt_AnnItemsFile.Text);
                m_SGExHandle.Start15AnnThread();
                btn_15Talk.Text = "停止";
            }
            else
            {
                g_Stop15Talk = true;
                m_SGExHandle.Stop15AnnThread();
                btn_15Talk.Text = "开始";
            }
        }

        private void btn_GetAnnItemsFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*.*)|*.*";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                txt_AnnItemsFile.Text = fileDialog.FileName;
            }
            else
            {
                return;
            }
        }

        private void txt_15srchInterval_TextChanged(object sender, EventArgs e)
        {
            g_15SrchInterval = int.Parse(txt_15srchInterval.Text);
            CIniCtrl.WriteIniData("Config", "15SrchInterval", txt_15srchInterval.Text, serverIni);
        }

        private void txt_AnnItemsFile_TextChanged(object sender, EventArgs e)
        {
            CIniCtrl.WriteIniData("Config", "AnnItemsFile", txt_AnnItemsFile.Text, serverIni);
        }

        private void txt_15MaxAnn_TextChanged(object sender, EventArgs e)
        {
            g_15MaxAnn = txt_15MaxAnn.Text;
            CIniCtrl.WriteIniData("Config", "15MaxAnn", txt_15MaxAnn.Text, serverIni);
        }
        private void rbx_15Talkfilter_TextChanged(object sender, EventArgs e)
        {
            g_15NameFilter = rbx_15NameFilter.Text;
            CIniCtrl.WriteIniData("Config", "15NameFilter", rbx_15NameFilter.Text, serverIni);
        }
        #endregion

        #region //在线问答

        bool g_StopQues = true;

        private List<string> m_TaskTime = new List<string>();
        private List<int> m_TaskDate = new List<int>();
        private UInt32 m_SleepCount = 60 * 30;
        private UInt32 m_AskNormalInterval = 60 * 30;
        private UInt32 m_AnswerVtId = 11849;
        private string m_AnswerVtName = "公测奖励福袋";

        private void cbx_AutoStartQues_CheckedChanged(object sender, EventArgs e)
        {
            if (cbx_AutoStartQues.Checked)
            {
                CIniCtrl.WriteIniData("Config", "AutoStartQues", "Enable", serverIni);
            }
            else
            {
                CIniCtrl.WriteIniData("Config", "AutoStartQues", "Disable", serverIni);
            }
        }
        private void button21_Click(object sender, EventArgs e)
        {
            if (!m_Active)
            {
                MessageBox.Show("软件尚未激活！ 请联系软件发布人给予激活！");
                return;
            }
            m_SGExHandle.LoadLoginServerPtr("");
            if (txt_QusbankFile.Text == string.Empty)
            {
                MessageBox.Show("请选择题库!");
                return;
            }
            if (txt_TaskTime.Text == string.Empty || txt_AskNormalInterval.Text == string.Empty
                 || txt_AnswerVtId.Text == string.Empty || txt_AnswerVtName.Text == string.Empty
                || txt_QuesInterval.Text == string.Empty || txt_MaxQuesNum.Text == string.Empty)
            {
                MessageBox.Show("请确保[开始时间][普通题间隔][奖励id][奖励名称][出题总数][问答间隔]均设置成功!");
                return;
            }
            if (g_StopQues)
            {
                g_StopQues = false;
                btn_startQues.Text = "停止";
                m_SleepCount = m_AskNormalInterval = UInt32.Parse(txt_AskNormalInterval.Text);
                m_AnswerVtId = UInt32.Parse(txt_AnswerVtId.Text);
                m_AnswerVtName = txt_AnswerVtName.Text;

                m_SGExHandle.SetConfigPath(txt_gameVerFile.Text);
                m_SGExHandle.LoadAQBank(txt_QusbankFile.Text);
                m_SGExHandle.SetQADatVt(txt_svrForder.Text + "\\DataBase\\saves\\players.dat", txt_sanvtName.Text);
                m_SGExHandle.SetQAReward((int)m_AnswerVtId, m_AnswerVtName);
                m_SGExHandle.SetMaxQuesNum(g_MaxQuesNum);
                m_SGExHandle.SetNormalInterval((int)m_AskNormalInterval);
                m_SGExHandle.SetTaskTime(m_TaskDate,m_TaskTime);
                m_SGExHandle.StartQAThread();
            }
            else
            {
                g_StopQues = true;
                btn_startQues.Text = "开始";
                m_SGExHandle.StopQAThread();
            }
        }
        private void button19_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*.*)|*.*";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = fileDialog.FileName;
                if (file.Contains("题库.txt"))
                {
                    txt_QusbankFile.Text = file;
                    CIniCtrl.WriteIniData("Config", "QuesBankFile", file, serverIni);
                }
                else
                {
                    MessageBox.Show("错误的文件，请重新选择！");
                    txt_QusbankFile.Text = "请选择版本文件！";
                    return;
                }
            }
            else
            {
                return;
            }
        }
        private void txt_TaskDate_TextChanged(object sender, EventArgs e)
        {
            CIniCtrl.WriteIniData("Config", "TaskDate", txt_TaskDate.Text, serverIni);
        }
        private void txt_TaskTime_TextChanged(object sender, EventArgs e)
        {
            CIniCtrl.WriteIniData("Config", "TaskTime", txt_TaskTime.Text, serverIni);
        }
        private void txt_MaxQuesNum_TextChanged(object sender, EventArgs e)
        {
            g_MaxQuesNum = int.Parse(txt_MaxQuesNum.Text);
            CIniCtrl.WriteIniData("Config", "MaxQuesNum", txt_MaxQuesNum.Text, serverIni);
        }
        private void txt_QuesInterval_TextChanged(object sender, EventArgs e)
        {
            g_QuesInterval = int.Parse(txt_QuesInterval.Text);
            CIniCtrl.WriteIniData("Config", "QuesInterval", txt_QuesInterval.Text, serverIni);
        }
        private void txt_AskNormalInterval_TextChanged(object sender, EventArgs e)
        {
            m_AskNormalInterval = UInt32.Parse(txt_AskNormalInterval.Text);
            CIniCtrl.WriteIniData("Config", "m_AskNormalInterval", txt_AskNormalInterval.Text, serverIni);
        }
        private void txt_AnswerVtId_TextChanged(object sender, EventArgs e)
        {
            m_AnswerVtId = UInt32.Parse(txt_AnswerVtId.Text);
            CIniCtrl.WriteIniData("Config", "m_AnswerVtId", txt_AnswerVtId.Text, serverIni);
        }

        void FillQAItemsView()
        {
            string allItemDefines = CItemCtrl.load_Item_Defines(System.AppDomain.CurrentDomain.BaseDirectory + "profile");
            var allItemDefinesArr = allItemDefines.Replace("\t", "").Split(';');

            this.lstv_QAItems.Items.Clear();
            foreach (var _def in allItemDefinesArr)
            {
                var _nameWithIdarr = _def.Split(',');
                if (_nameWithIdarr.Length > 1)
                {
                    string _name = _nameWithIdarr[0];
                    string _id = _nameWithIdarr[1];
                    ListViewItem lvi = new ListViewItem();
                    //lvi.ImageIndex = i;     //通过与imageList绑定，显示imageList中第i项图标 
                    lvi.Text = _id.Replace("\t", "");
                    //lvi.SubItems.Add(_id);
                    lvi.SubItems.Add(_name);
                    this.lstv_QAItems.Items.Add(lvi);
                }
            }
            this.lstv_QAItems.EndUpdate();  //结束数据处理，UI界面一次性绘制
        }

        private void txt_AnswerVtName_TextChanged(object sender, EventArgs e)
        {
            m_AnswerVtName = txt_AnswerVtName.Text;
            CIniCtrl.WriteIniData("Config", "m_AnswerVtName", txt_AnswerVtName.Text, serverIni);
        }
        private void button20_Click(object sender, EventArgs e)
        {
            CIniCtrl.WriteIniData("Config", "TaskDate", txt_TaskDate.Text, serverIni);
            if (txt_TaskDate.Text != string.Empty)
            {
                var dates = txt_TaskDate.Text.Split(',');
                m_TaskDate.Clear();
                foreach (var date in dates)
                {
                    m_TaskDate.Add(int.Parse(date));
                }
            }
            
            CIniCtrl.WriteIniData("Config", "TaskTime", txt_TaskTime.Text, serverIni);
            if (txt_TaskTime.Text != string.Empty)
            {
                var times = txt_TaskTime.Text.Split(';');
                m_TaskTime.Clear();
                foreach (var time in times)
                {
                    m_TaskTime.Add(time);
                }
            }
            

            g_MaxQuesNum = int.Parse(txt_MaxQuesNum.Text);
            CIniCtrl.WriteIniData("Config", "MaxQuesNum", txt_MaxQuesNum.Text, serverIni);
            g_QuesInterval = int.Parse(txt_QuesInterval.Text);
            CIniCtrl.WriteIniData("Config", "QuesInterval", txt_QuesInterval.Text, serverIni);

            m_AskNormalInterval = UInt32.Parse(txt_AskNormalInterval.Text);
            CIniCtrl.WriteIniData("Config", "m_AskNormalInterval", txt_AskNormalInterval.Text, serverIni);
            m_AnswerVtId = UInt32.Parse(txt_AnswerVtId.Text);
            CIniCtrl.WriteIniData("Config", "m_AnswerVtId", txt_AnswerVtId.Text, serverIni);
            m_AnswerVtName = txt_AnswerVtName.Text;
            CIniCtrl.WriteIniData("Config", "m_AnswerVtName", txt_AnswerVtName.Text, serverIni);
        }
        #endregion
        
        #region //进程管理
        private void btn_ps1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*.exe)|*.exe";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                txt_ps1.Text = fileDialog.FileName;
                //保存次路径到工具配置文件
                CIniCtrl.WriteIniData("Prosess", "ps1", fileDialog.FileName, serverIni);
            }
            else
            {
                txt_ps1.Text = "";
                //保存次路径到工具配置文件
                CIniCtrl.WriteIniData("Prosess", "ps1", fileDialog.FileName, serverIni);
                return;
            }
        }

        private void btn_ps2_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*.exe)|*.exe";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                txt_ps2.Text = fileDialog.FileName;
                //保存次路径到工具配置文件
                CIniCtrl.WriteIniData("Prosess", "ps2", fileDialog.FileName, serverIni);
            }
            else
            {
                txt_ps2.Text = "";
                //保存次路径到工具配置文件
                CIniCtrl.WriteIniData("Prosess", "ps1", fileDialog.FileName, serverIni);
                return;
            }
        }

        private void btn_ps3_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*.exe)|*.exe";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                txt_ps3.Text = fileDialog.FileName;
                //保存次路径到工具配置文件
                CIniCtrl.WriteIniData("Prosess", "ps3", fileDialog.FileName, serverIni);
            }
            else
            {
                txt_ps3.Text = "";
                //保存次路径到工具配置文件
                CIniCtrl.WriteIniData("Prosess", "ps1", fileDialog.FileName, serverIni);
                return;
            }
        }

        private void btn_ps4_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*.exe)|*.exe";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                txt_ps4.Text = fileDialog.FileName;
                //保存次路径到工具配置文件
                CIniCtrl.WriteIniData("Prosess", "ps4", fileDialog.FileName, serverIni);
            }
            else
            {
                txt_ps4.Text = "";
                //保存次路径到工具配置文件
                CIniCtrl.WriteIniData("Prosess", "ps1", fileDialog.FileName, serverIni);
                return;
            }
        }

        private void btn_ps5_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*.exe)|*.exe";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                txt_ps5.Text = fileDialog.FileName;
                //保存次路径到工具配置文件
                CIniCtrl.WriteIniData("Prosess", "ps5", fileDialog.FileName, serverIni);
            }
            else
            {
                txt_ps5.Text = "";
                //保存次路径到工具配置文件
                CIniCtrl.WriteIniData("Prosess", "ps1", fileDialog.FileName, serverIni);
                return;
            }
        }

        private void btn_ps6_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*.exe)|*.exe";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                txt_ps6.Text = fileDialog.FileName;
                //保存次路径到工具配置文件
                CIniCtrl.WriteIniData("Prosess", "ps6", fileDialog.FileName, serverIni);
            }
            else
            {
                txt_ps6.Text = "";
                //保存次路径到工具配置文件
                CIniCtrl.WriteIniData("Prosess", "ps1", fileDialog.FileName, serverIni);
                return;
            }
        }

        private void btn_ps7_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*.exe)|*.exe";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                txt_ps7.Text = fileDialog.FileName;
                //保存次路径到工具配置文件
                CIniCtrl.WriteIniData("Prosess", "ps7", fileDialog.FileName, serverIni);
            }
            else
            {
                txt_ps7.Text = "";
                //保存次路径到工具配置文件
                CIniCtrl.WriteIniData("Prosess", "ps1", fileDialog.FileName, serverIni);
                return;
            }
        }

        private void btn_ps8_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*.exe)|*.exe";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                txt_ps8.Text = fileDialog.FileName;
                //保存次路径到工具配置文件
                CIniCtrl.WriteIniData("Prosess", "ps8", fileDialog.FileName, serverIni);
            }
            else
            {
                txt_ps8.Text = "";
                //保存次路径到工具配置文件
                CIniCtrl.WriteIniData("Prosess", "ps1", fileDialog.FileName, serverIni);
                return;
            }
        }

        private void btn_ps9_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*.exe)|*.exe";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                txt_ps9.Text = fileDialog.FileName;
                //保存次路径到工具配置文件
                CIniCtrl.WriteIniData("Prosess", "ps9", fileDialog.FileName, serverIni);
            }
            else
            {
                txt_ps9.Text = "";
                //保存次路径到工具配置文件
                CIniCtrl.WriteIniData("Prosess", "ps1", fileDialog.FileName, serverIni);
                return;
            }
        }

        private void btn_ps10_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*.exe)|*.exe";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                txt_ps10.Text = fileDialog.FileName;
                //保存次路径到工具配置文件
                CIniCtrl.WriteIniData("Prosess", "ps10", fileDialog.FileName, serverIni);
            }
            else
            {
                txt_ps10.Text = "";
                //保存次路径到工具配置文件
                CIniCtrl.WriteIniData("Prosess", "ps1", fileDialog.FileName, serverIni);
                return;
            }
        }

        private void btn_ps11_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*.exe)|*.exe";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                txt_ps11.Text = fileDialog.FileName;
                //保存次路径到工具配置文件
                CIniCtrl.WriteIniData("Prosess", "ps11", fileDialog.FileName, serverIni);
            }
            else
            {
                txt_ps11.Text = "";
                //保存次路径到工具配置文件
                CIniCtrl.WriteIniData("Prosess", "ps1", fileDialog.FileName, serverIni);
                return;
            }
        }

        private void btn_ps12_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*.exe)|*.exe";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                txt_ps12.Text = fileDialog.FileName;
                //保存次路径到工具配置文件
                CIniCtrl.WriteIniData("Prosess", "ps12", fileDialog.FileName, serverIni);
            }
            else
            {
                txt_ps12.Text = "";
                //保存次路径到工具配置文件
                CIniCtrl.WriteIniData("Prosess", "ps1", fileDialog.FileName, serverIni);
                return;
            }
        }

        private void btn_ps13_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*.exe)|*.exe";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                txt_ps13.Text = fileDialog.FileName;
                //保存次路径到工具配置文件
                CIniCtrl.WriteIniData("Prosess", "ps13", fileDialog.FileName, serverIni);
            }
            else
            {
                txt_ps13.Text = "";
                //保存次路径到工具配置文件
                CIniCtrl.WriteIniData("Prosess", "ps1", fileDialog.FileName, serverIni);
                return;
            }
        }

        private void btn_ps14_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*.exe)|*.exe";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                txt_ps14.Text = fileDialog.FileName;
                //保存次路径到工具配置文件
                CIniCtrl.WriteIniData("Prosess", "ps14", fileDialog.FileName, serverIni);
            }
            else
            {
                txt_ps14.Text = "";
                //保存次路径到工具配置文件
                CIniCtrl.WriteIniData("Prosess", "ps1", fileDialog.FileName, serverIni);
                return;
            }
        }

        private void btn_ps15_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*.exe)|*.exe";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                txt_ps15.Text = fileDialog.FileName;
                //保存次路径到工具配置文件
                CIniCtrl.WriteIniData("Prosess", "ps15", fileDialog.FileName, serverIni);
            }
            else
            {
                txt_ps15.Text = "";
                //保存次路径到工具配置文件
                CIniCtrl.WriteIniData("Prosess", "ps1", fileDialog.FileName, serverIni);
                return;
            }
        }

        private void btn_psSave_Click(object sender, EventArgs e)
        {
            CIniCtrl.WriteIniData("Prosess", "ps1", txt_ps1.Text, serverIni);
            CIniCtrl.WriteIniData("Prosess", "ps2", txt_ps2.Text, serverIni);
            CIniCtrl.WriteIniData("Prosess", "ps3", txt_ps3.Text, serverIni);
            CIniCtrl.WriteIniData("Prosess", "ps4", txt_ps4.Text, serverIni);
            CIniCtrl.WriteIniData("Prosess", "ps5", txt_ps5.Text, serverIni);
            CIniCtrl.WriteIniData("Prosess", "ps6", txt_ps6.Text, serverIni);
            CIniCtrl.WriteIniData("Prosess", "ps7", txt_ps7.Text, serverIni);
            CIniCtrl.WriteIniData("Prosess", "ps8", txt_ps8.Text, serverIni);
            CIniCtrl.WriteIniData("Prosess", "ps9", txt_ps9.Text, serverIni);
            CIniCtrl.WriteIniData("Prosess", "ps10", txt_ps10.Text, serverIni);
            CIniCtrl.WriteIniData("Prosess", "ps11", txt_ps11.Text, serverIni);
            CIniCtrl.WriteIniData("Prosess", "ps12", txt_ps12.Text, serverIni);
            CIniCtrl.WriteIniData("Prosess", "ps13", txt_ps13.Text, serverIni);
            CIniCtrl.WriteIniData("Prosess", "ps14", txt_ps14.Text, serverIni);
            CIniCtrl.WriteIniData("Prosess", "ps15", txt_ps15.Text, serverIni);
        }

        [DllImport("kernel32.dll")]
        public static extern int WinExec(string exeName, int operType);
        private void start_process(string exe)
        {
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(exe);
            info.WorkingDirectory = Path.GetDirectoryName(exe);
            System.Diagnostics.Process.Start(info);
        }
        private void btn_psStart_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txt_ps1.Text))
                {
                    //System.Diagnostics.Process.Start(txt_ps1.Text);
                    //WinExec(@" " + txt_ps1.Text, 1);
                    //System.IO.Directory.SetCurrentDirectory(System.Environment.CurrentDirectory);
                    //System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(txt_ps1.Text, "");
                    //psi.RedirectStandardOutput = true;
                    //psi.UseShellExecute = true;
                    //Process newProcess = Process.Start(psi);
                    start_process(txt_ps1.Text);
                    Thread.Sleep(1000);
                }
                if (!string.IsNullOrEmpty(txt_ps2.Text))
                {
                    //System.Diagnostics.Process.Start(@txt_ps2.Text);
                    //WinExec(@" " + txt_ps2.Text, 1);
                    start_process(txt_ps2.Text);
                    Thread.Sleep(1000);
                }
                if (!string.IsNullOrEmpty(txt_ps3.Text))
                {
                    //System.Diagnostics.Process.Start(@txt_ps3.Text);
                    //WinExec(@" " + txt_ps3.Text, 1);
                    start_process(txt_ps3.Text);
                    Thread.Sleep(1000);
                }
                if (!string.IsNullOrEmpty(txt_ps4.Text))
                {
                    //System.Diagnostics.Process.Start(@txt_ps4.Text);
                    //WinExec(@" " + txt_ps4.Text, 1);
                    start_process(txt_ps4.Text);
                    Thread.Sleep(1000);
                }
                if (!string.IsNullOrEmpty(txt_ps5.Text))
                {
                    //System.Diagnostics.Process.Start(@txt_ps5.Text);
                    //WinExec(@" " + txt_ps5.Text, 1);
                    start_process(txt_ps5.Text);
                    Thread.Sleep(1000);
                }
                if (!string.IsNullOrEmpty(txt_ps6.Text))
                {
                    //System.Diagnostics.Process.Start(@txt_ps6.Text);
                    //WinExec(@" " + txt_ps6.Text, 1);
                    start_process(txt_ps6.Text);
                    Thread.Sleep(1000);
                }
                if (!string.IsNullOrEmpty(txt_ps7.Text))
                {
                    //System.Diagnostics.Process.Start(@txt_ps7.Text);
                    //WinExec(@" " + txt_ps7.Text, 1);
                    start_process(txt_ps7.Text);
                    Thread.Sleep(1000);
                }
                if (!string.IsNullOrEmpty(txt_ps8.Text))
                {
                    //System.Diagnostics.Process.Start(@txt_ps8.Text);
                    //WinExec(@" " + txt_ps8.Text, 1);
                    start_process(txt_ps8.Text);
                    Thread.Sleep(1000);
                }
                if (!string.IsNullOrEmpty(txt_ps9.Text))
                {
                    //System.Diagnostics.Process.Start(@txt_ps9.Text);
                    //WinExec(@" " + txt_ps9.Text, 1);
                    start_process(txt_ps9.Text);
                    Thread.Sleep(1000);
                }
                if (!string.IsNullOrEmpty(txt_ps10.Text))
                {
                    //System.Diagnostics.Process.Start(@txt_ps10.Text);
                    //WinExec(@" " + txt_ps10.Text, 1);
                    start_process(txt_ps10.Text);
                    Thread.Sleep(1000);
                }
                if (!string.IsNullOrEmpty(txt_ps11.Text))
                {
                    //System.Diagnostics.Process.Start(@txt_ps11.Text);
                    //WinExec(@" " + txt_ps11.Text, 1);
                    start_process(txt_ps11.Text);
                    Thread.Sleep(1000);
                }
                if (!string.IsNullOrEmpty(txt_ps12.Text))
                {
                    //System.Diagnostics.Process.Start(@txt_ps12.Text);
                    //WinExec(@" " + txt_ps12.Text, 1);
                    start_process(txt_ps12.Text);
                    Thread.Sleep(1000);
                }
                if (!string.IsNullOrEmpty(txt_ps13.Text))
                {
                    //System.Diagnostics.Process.Start(@txt_ps13.Text);
                    //WinExec(@" " + txt_ps13.Text, 1);
                    start_process(txt_ps13.Text);
                    Thread.Sleep(1000);
                }
                if (!string.IsNullOrEmpty(txt_ps14.Text))
                {
                    //System.Diagnostics.Process.Start(@txt_ps14.Text);
                    //WinExec(@" " + txt_ps14.Text, 1);
                    start_process(txt_ps14.Text);
                    Thread.Sleep(1000);
                }
                if (!string.IsNullOrEmpty(txt_ps15.Text))
                {
                    //System.Diagnostics.Process.Start(@txt_ps15.Text);
                    //WinExec(@" " + txt_ps15.Text, 1);
                    start_process(txt_ps15.Text);
                    Thread.Sleep(1000);
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region //发送虚宝
        private void FillXbAccLstView()
        {
            var players = CPlayerCtrl.GetAttrList();
            this.lstv_xbAcc.Items.Clear();
            foreach (var player in players)
            {
                string _acc = CFormat.GameStrToSimpleCN(player.Account);
                string _name = CFormat.GameStrToSimpleCN(player.Name);

                ListViewItem lvi = new ListViewItem();
                //lvi.ImageIndex = i;     //通过与imageList绑定，显示imageList中第i项图标 
                lvi.Text = _acc.Replace("\t", "");
                //lvi.SubItems.Add(_id);
                lvi.SubItems.Add(_name);
                this.lstv_xbAcc.Items.Add(lvi);
            }
            this.lstv_xbAcc.EndUpdate();  //结束数据处理，UI界面一次性绘制
        }
        static int xbPlayersInfoSltIndex = 0;
        void FileXbItemsView()
        {
            string allItemDefines = CItemCtrl.load_Item_Defines(System.AppDomain.CurrentDomain.BaseDirectory + "profile");
            var allItemDefinesArr = allItemDefines.Replace("\t", "").Split(';');

            this.lstv_xbItems.Items.Clear();
            foreach (var _def in allItemDefinesArr)
            {
                var _nameWithIdarr = _def.Split(',');
                if (_nameWithIdarr.Length > 1)
                {
                    string _name = _nameWithIdarr[0];
                    string _id = _nameWithIdarr[1];
                    ListViewItem lvi = new ListViewItem();
                    //lvi.ImageIndex = i;     //通过与imageList绑定，显示imageList中第i项图标 
                    lvi.Text = _id.Replace("\t", "");
                    //lvi.SubItems.Add(_id);
                    lvi.SubItems.Add(_name);
                    this.lstv_xbItems.Items.Add(lvi);
                }
            }
            this.lstv_xbItems.EndUpdate();  //结束数据处理，UI界面一次性绘制
        }

        private void LoadXbMould()
        {
            for (int i = 0; i < cbx_xbconf.Items.Count; i++)
            {
                string xbconfCap = "xbconfCap" + i;
                string xbconfDesc = "xbconfDesc" + i;
                string cap = CIniCtrl.ReadIniData("XbConf", xbconfCap, "", serverIni);
                if (!string.IsNullOrEmpty(cap) && cap != "")
                {
                    cbx_xbconf.Items[i] = cap;
                }
            }
            cbx_xbconf.SelectedIndex = 0;
            xbconf_SelectedIndex = 0;
            txt_xbconfCap.Text = CIniCtrl.ReadIniData("XbConf", "xbconfCap0", "", serverIni);
            rbx_confDesc.Text = CIniCtrl.ReadIniData("XbConf", "xbconfDesc0", "", serverIni);
        }

        private void LoadXBMrgInfos()
        {
            //加载帐号列表
            CPlayerCtrl.LoadPlayerInfos(txt_svrForder.Text + "\\DataBase\\saves\\players.dat");
            FillXbAccLstView();
            xbPlayersInfoSltIndex = 0;
            //加载物品列表
            FileXbItemsView();
            xbitemDefSltIndex = 0;
            //加载虚宝模版
            LoadXbMould();

            //加载日志
            string file = SGExHandle.GetXbLogFileName();
            FileStream fs = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Read);
            StreamReader sr = new StreamReader(fs, Encoding.Default);

            lstv_xblog.Items.Clear();
            string log = "";
            while(!string.IsNullOrEmpty(log = sr.ReadLine()) && log != "")
            {
                var items = log.Split('\t');
                if (items.Length >=16)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = items[0];
                    lvi.SubItems.Add(items[1]);
                    lvi.SubItems.Add(items[2]);
                    lvi.SubItems.Add(items[3]);
                    lvi.SubItems.Add(items[4]);
                    lvi.SubItems.Add(items[5]);
                    lvi.SubItems.Add(items[6]);
                    lvi.SubItems.Add(items[7]);
                    lvi.SubItems.Add(items[8]);
                    lvi.SubItems.Add(items[9]);
                    lvi.SubItems.Add(items[10]);
                    lvi.SubItems.Add(items[11]);
                    lvi.SubItems.Add(items[12]);
                    lvi.SubItems.Add(items[13]);
                    lvi.SubItems.Add(items[14]);
                    lvi.SubItems.Add(items[15]);
                    lvi.SubItems.Add(items[16]);
                    lstv_xblog.Items.Add(lvi);
                }
            }
            lstv_xblog.EndUpdate();  //结束数据处理，UI界面一次性绘制

            sr.Close();
            fs.Close();
        }

        private void btn_xbAccReLoad_Click(object sender, EventArgs e)
        {
            CPlayerCtrl.LoadPlayerInfos(txt_svrForder.Text + "\\DataBase\\saves\\players.dat", true);
            FillXbAccLstView();
            xbPlayersInfoSltIndex = 0;
        }

        private void lstv_xbAcc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstv_xbAcc.SelectedIndices != null && lstv_xbAcc.SelectedIndices.Count > 0)
            {
                lstv_xbAcc.Items[xbPlayersInfoSltIndex].BackColor = Color.Transparent;
                xbPlayersInfoSltIndex = this.lstv_xbAcc.SelectedItems[0].Index;
                lstv_xbAcc.Items[xbPlayersInfoSltIndex].BackColor = Color.Pink;

                txt_xbAccount.Text = lstv_xbAcc.Items[xbPlayersInfoSltIndex].SubItems[0].Text;
                txt_xbName.Text = lstv_xbAcc.Items[xbPlayersInfoSltIndex].SubItems[1].Text;

                //查询代币
                txt_dbCurr.Text = "" + CSGHelper.SelectAcountPoint(txt_xbAccount.Text);
            }
        }

        private void btn_xbAccSrch_Click(object sender, EventArgs e)
        {
            ListViewItem foundItem = null;
            lstv_xbAcc.Items[xbPlayersInfoSltIndex].BackColor = Color.Transparent;
            int i = xbPlayersInfoSltIndex + 1;
            if (i == lstv_xbAcc.Items.Count)
            {
                i = 0;
            }
            while (i != xbPlayersInfoSltIndex)
            {
                if (lstv_xbAcc.Items[i].SubItems[1].Text.Contains(txt_xbAccSrch.Text) || lstv_xbAcc.Items[i].Text.Contains(txt_xbAccSrch.Text))
                {
                    foundItem = lstv_xbAcc.Items[i];
                    break;
                }
                i++;
                if (i == lstv_xbAcc.Items.Count)
                {
                    i = 0;
                }
            }

            if (foundItem != null)
            {
                lstv_xbAcc.TopItem = foundItem;  //定位到该项
                lstv_xbAcc.Items[foundItem.Index].Focused = true;
                //lstv_xbAcc.Items[foundItem.Index].Selected = true;
                foundItem.BackColor = Color.Pink;
                xbPlayersInfoSltIndex = foundItem.Index;
            }
        }
        static int xbitemDefSltIndex = 0;
        private void btn_xbItemSrch_Click(object sender, EventArgs e)
        {
            //ListViewItem foundItem = this.listView1.FindItemWithText(this.txt_srchItemName.Text, true, cur_select_index + 1);    //参数1：要查找的文本；参数2：是否子项也要查找；参数3：开始查找位置  
            ListViewItem foundItem = null;
            lstv_xbItems.Items[xbitemDefSltIndex].BackColor = Color.Transparent;
            int i = xbitemDefSltIndex + 1;
            if (i == lstv_xbItems.Items.Count)
            {
                i = 0;
            }
            while (i != xbitemDefSltIndex)
            {
                if (lstv_xbItems.Items[i].SubItems[1].Text.Contains(txt_xbItemSrch.Text) || lstv_xbItems.Items[i].Text == txt_xbItemSrch.Text)
                {
                    foundItem = lstv_xbItems.Items[i];
                }
                i++;
                if (i == lstv_xbItems.Items.Count)
                {
                    i = 0;
                }
            }

            if (foundItem != null)
            {
                lstv_xbItems.TopItem = foundItem;  //定位到该项
                lstv_xbItems.Items[foundItem.Index].Focused = true;
                //lstv_xbItems.Items[foundItem.Index].Selected = true;
                foundItem.BackColor = Color.Pink;
                xbitemDefSltIndex = foundItem.Index;
            }
        }

        private void lstv_xbItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstv_xbItems.SelectedIndices != null && lstv_xbItems.SelectedIndices.Count > 0)
            {
                lstv_xbItems.Items[xbitemDefSltIndex].BackColor = Color.Transparent;
                xbitemDefSltIndex = this.lstv_xbItems.SelectedItems[0].Index;
                lstv_xbItems.Items[xbitemDefSltIndex].BackColor = Color.Pink;

                //if 有空位
                if (txt_xbId1.Text == string.Empty || txt_xbCount1.Text == string.Empty)
                {
                    txt_xbId1.Text = lstv_xbItems.Items[xbitemDefSltIndex].Text;
                    txt_xbName1.Text = lstv_xbItems.Items[xbitemDefSltIndex].SubItems[1].Text;
                    txt_xbCount1.Text = "1";
                }
                else if (txt_xbId2.Text == string.Empty || txt_xbCount2.Text == string.Empty)
                {
                    txt_xbId2.Text = lstv_xbItems.Items[xbitemDefSltIndex].SubItems[0].Text;
                    txt_xbName2.Text = lstv_xbItems.Items[xbitemDefSltIndex].SubItems[1].Text;
                    txt_xbCount2.Text = "1";
                }
                else if (txt_xbId3.Text == string.Empty || txt_xbCount3.Text == string.Empty)
                {
                    txt_xbId3.Text = lstv_xbItems.Items[xbitemDefSltIndex].SubItems[0].Text;
                    txt_xbName3.Text = lstv_xbItems.Items[xbitemDefSltIndex].SubItems[1].Text;
                    txt_xbCount3.Text = "1";
                }
                else if (txt_xbId4.Text == string.Empty || txt_xbCount4.Text == string.Empty)
                {
                    txt_xbId4.Text = lstv_xbItems.Items[xbitemDefSltIndex].SubItems[0].Text;
                    txt_xbName4.Text = lstv_xbItems.Items[xbitemDefSltIndex].SubItems[1].Text;
                    txt_xbCount4.Text = "1";
                }
                else if (txt_xbId5.Text == string.Empty || txt_xbCount5.Text == string.Empty)
                {
                    txt_xbId5.Text = lstv_xbItems.Items[xbitemDefSltIndex].SubItems[0].Text;
                    txt_xbName5.Text = lstv_xbItems.Items[xbitemDefSltIndex].SubItems[1].Text;
                    txt_xbCount5.Text = "1";
                }
                else if (txt_xbId6.Text == string.Empty || txt_xbCount6.Text == string.Empty)
                {
                    txt_xbId6.Text = lstv_xbItems.Items[xbitemDefSltIndex].SubItems[0].Text;
                    txt_xbName6.Text = lstv_xbItems.Items[xbitemDefSltIndex].SubItems[1].Text;
                    txt_xbCount6.Text = "1";
                }
                else if (txt_xbId7.Text == string.Empty || txt_xbCount7.Text == string.Empty)
                {
                    txt_xbId7.Text = lstv_xbItems.Items[xbitemDefSltIndex].SubItems[0].Text;
                    txt_xbName7.Text = lstv_xbItems.Items[xbitemDefSltIndex].SubItems[1].Text;
                    txt_xbCount7.Text = "1";
                }
                else if (txt_xbId8.Text == string.Empty || txt_xbCount8.Text == string.Empty)
                {
                    txt_xbId8.Text = lstv_xbItems.Items[xbitemDefSltIndex].SubItems[0].Text;
                    txt_xbName8.Text = lstv_xbItems.Items[xbitemDefSltIndex].SubItems[1].Text;
                    txt_xbCount8.Text = "1";
                }
                else if (txt_xbId9.Text == string.Empty || txt_xbCount9.Text == string.Empty)
                {
                    txt_xbId9.Text = lstv_xbItems.Items[xbitemDefSltIndex].SubItems[0].Text;
                    txt_xbName9.Text = lstv_xbItems.Items[xbitemDefSltIndex].SubItems[1].Text;
                    txt_xbCount9.Text = "1";
                }
                else if (txt_xbId10.Text == string.Empty || txt_xbCount10.Text == string.Empty)
                {
                    txt_xbId10.Text = lstv_xbItems.Items[xbitemDefSltIndex].SubItems[0].Text;
                    txt_xbName10.Text = lstv_xbItems.Items[xbitemDefSltIndex].SubItems[1].Text;
                    txt_xbCount10.Text = "1";
                }
                else
                {
                    txt_xbId1.Text = lstv_xbItems.Items[xbitemDefSltIndex].SubItems[0].Text;
                    txt_xbName1.Text = lstv_xbItems.Items[xbitemDefSltIndex].SubItems[1].Text;
                    txt_xbCount1.Text = "1";
                }
            }
        }

        private void btn_xbItemGet_Click(object sender, EventArgs e)
        {
            FileXbItemsView();
        }

        private void btn_bxSend_Click(object sender, EventArgs e)
        {
            if (!m_Active)
            {
                MessageBox.Show("软件尚未激活！ 请联系软件发布人给予激活！");
                return;
            }
            if (txt_xbAccount.Text != string.Empty)
            {
                int xbCount1 = 0;
                int xbCount2 = 0;
                int xbCount3 = 0;
                int xbCount4 = 0;
                int xbCount5 = 0;
                int xbCount6 = 0;
                int xbCount7 = 0;
                int xbCount8 = 0;
                int xbCount9 = 0;
                int xbCount10 = 0;
                int xbId1 = 1;
                int xbId2 = 1;
                int xbId3 = 1;
                int xbId4 = 1;
                int xbId5 = 1;
                int xbId6 = 1;
                int xbId7 = 1;
                int xbId8 = 1;
                int xbId9 = 1;
                int xbId10 = 1;
                if (!string.IsNullOrEmpty(txt_xbCount1.Text) || CFormat.isNumberic(txt_xbCount1.Text))
                {
                    xbCount1 = Convert.ToInt32(txt_xbCount1.Text);

                }
                if (!string.IsNullOrEmpty(txt_xbCount2.Text) || CFormat.isNumberic(txt_xbCount2.Text))
                {
                    xbCount2 = Convert.ToInt32(txt_xbCount2.Text);

                }
                if (!string.IsNullOrEmpty(txt_xbCount3.Text) || CFormat.isNumberic(txt_xbCount3.Text))
                {
                    xbCount3 = Convert.ToInt32(txt_xbCount3.Text);

                }
                if (!string.IsNullOrEmpty(txt_xbCount4.Text) || CFormat.isNumberic(txt_xbCount4.Text))
                {
                    xbCount4 = Convert.ToInt32(txt_xbCount4.Text);

                }
                if (!string.IsNullOrEmpty(txt_xbCount5.Text) || CFormat.isNumberic(txt_xbCount5.Text))
                {
                    xbCount5 = Convert.ToInt32(txt_xbCount5.Text);

                }
                if (!string.IsNullOrEmpty(txt_xbCount6.Text) || CFormat.isNumberic(txt_xbCount6.Text))
                {
                    xbCount6 = Convert.ToInt32(txt_xbCount6.Text);

                }
                if (!string.IsNullOrEmpty(txt_xbCount7.Text) || CFormat.isNumberic(txt_xbCount7.Text))
                {
                    xbCount7 = Convert.ToInt32(txt_xbCount7.Text);

                }
                if (!string.IsNullOrEmpty(txt_xbCount8.Text) || CFormat.isNumberic(txt_xbCount8.Text))
                {
                    xbCount8 = Convert.ToInt32(txt_xbCount8.Text);

                }
                if (!string.IsNullOrEmpty(txt_xbCount9.Text) || CFormat.isNumberic(txt_xbCount9.Text))
                {
                    xbCount9 = Convert.ToInt32(txt_xbCount9.Text);

                }
                if (!string.IsNullOrEmpty(txt_xbCount10.Text) || CFormat.isNumberic(txt_xbCount10.Text))
                {
                    xbCount10 = Convert.ToInt32(txt_xbCount10.Text);

                }
                if (!string.IsNullOrEmpty(txt_xbId1.Text) || CFormat.isNumberic(txt_xbId1.Text))
                {
                    xbId1 = Convert.ToInt32(txt_xbId1.Text);
                    if (xbId1 == 0)
                    {
                        xbId1 = 1;
                        xbCount1 = 0;
                    }
                }
                if (!string.IsNullOrEmpty(txt_xbId2.Text) || CFormat.isNumberic(txt_xbId2.Text))
                {
                    xbId2 = Convert.ToInt32(txt_xbId2.Text);
                    if (xbId2 == 0)
                    {
                        xbId2 = 1;
                        xbCount2 = 0;
                    }
                }
                if (!string.IsNullOrEmpty(txt_xbId3.Text) || CFormat.isNumberic(txt_xbId3.Text))
                {
                    xbId3 = Convert.ToInt32(txt_xbId3.Text);
                    if (xbId3 == 0)
                    {
                        xbId3 = 1;
                        xbCount3 = 0;
                    }
                }
                if (!string.IsNullOrEmpty(txt_xbId4.Text) || CFormat.isNumberic(txt_xbId4.Text))
                {
                    xbId4 = Convert.ToInt32(txt_xbId4.Text);
                    if (xbId4 == 0)
                    {
                        xbId4 = 1;
                        xbCount4 = 0;
                    }
                }
                if (!string.IsNullOrEmpty(txt_xbId5.Text) || CFormat.isNumberic(txt_xbId5.Text))
                {
                    xbId5 = Convert.ToInt32(txt_xbId5.Text);
                    if (xbId5 == 0)
                    {
                        xbId5 = 1;
                        xbCount5 = 0;
                    }
                }
                if (!string.IsNullOrEmpty(txt_xbId6.Text) || CFormat.isNumberic(txt_xbId6.Text))
                {
                    xbId6 = Convert.ToInt32(txt_xbId6.Text);
                    if (xbId6 == 0)
                    {
                        xbId6 = 1;
                        xbCount6 = 0;
                    }
                }
                if (!string.IsNullOrEmpty(txt_xbId7.Text) || CFormat.isNumberic(txt_xbId7.Text))
                {
                    xbId7 = Convert.ToInt32(txt_xbId7.Text);
                    if (xbId7 == 0)
                    {
                        xbId7 = 1;
                        xbCount7 = 0;
                    }
                }
                if (!string.IsNullOrEmpty(txt_xbId8.Text) || CFormat.isNumberic(txt_xbId8.Text))
                {
                    xbId8 = Convert.ToInt32(txt_xbId8.Text);
                    if (xbId8 == 0)
                    {
                        xbId8 = 1;
                        xbCount8 = 0;
                    }
                }
                if (!string.IsNullOrEmpty(txt_xbId9.Text) || CFormat.isNumberic(txt_xbId9.Text))
                {
                    xbId9 = Convert.ToInt32(txt_xbId9.Text);
                    if (xbId9 == 0)
                    {
                        xbId9 = 1;
                        xbCount9 = 0;
                    }
                }
                if (!string.IsNullOrEmpty(txt_xbId10.Text) || CFormat.isNumberic(txt_xbId10.Text))
                {
                    xbId10 = Convert.ToInt32(txt_xbId10.Text);
                    if (xbId10 == 0)
                    {
                        xbId10 = 1;
                        xbCount10 = 0;
                    }
                }
                string msg = "\n"
                    + "账户：" + txt_xbAccount.Text + "\n"
                    + "角色：" + txt_xbName.Text + "\n"
                    + "代币：" + txt_dbSend.Text + "\n";
                    if(txt_xbId1.Text != "" && xbCount1 != 0)
                    {
                        msg += "物品：" + txt_xbName1.Text + "\t数量：" + txt_xbCount1.Text + "\t\n";
                    }
                    if(txt_xbId2.Text != "" && xbCount2 != 0)
                    {
                        msg += "物品：" + txt_xbName2.Text + "\t数量：" + txt_xbCount2.Text + "\t\n";
                    }
                    if (txt_xbId3.Text != "" && xbCount3 != 0)
                    {
                        msg += "物品：" + txt_xbName3.Text + "\t数量：" + txt_xbCount3.Text + "\t\n";
                    }
                    if (txt_xbId4.Text != "" && xbCount4 != 0)
                    {
                        msg += "物品：" + txt_xbName4.Text + "\t数量：" + txt_xbCount4.Text + "\t\n";
                    }
                    if (txt_xbId5.Text != "" && xbCount5 != 0)
                    {
                        msg += "物品：" + txt_xbName5.Text + "\t数量：" + txt_xbCount5.Text + "\t\n";
                    }
                    if (txt_xbId6.Text != "" && xbCount6 != 0)
                    {
                        msg += "物品：" + txt_xbName6.Text + "\t数量：" + txt_xbCount6.Text + "\t\n";
                    }
                    if (txt_xbId7.Text != "" && xbCount7 != 0)
                    {
                        msg += "物品：" + txt_xbName7.Text + "\t数量：" + txt_xbCount7.Text + "\t\n";
                    }
                    if (txt_xbId8.Text != "" && xbCount8 != 0)
                    {
                        msg += "物品：" + txt_xbName8.Text + "\t数量：" + txt_xbCount8.Text + "\t\n";
                    }
                    if (txt_xbId9.Text != "" && xbCount9 != 0)
                    {
                        msg += "物品：" + txt_xbName9.Text + "\t数量：" + txt_xbCount9.Text + "\t\n";
                    }
                    if (txt_xbId10.Text != "" && xbCount10 != 0)
                    {
                        msg += "物品：" + txt_xbName10.Text + "\t数量：" + txt_xbCount10.Text + "\t\n";
                    }

                    msg += "\n确认信息无误？";
                DialogResult dr = MessageBox.Show(msg, "对话框标题", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr != DialogResult.OK)
                {
                    return;
                }

                //确保有效
                if ((txt_xbId1.Text == "" || xbCount1 == 0) && (txt_xbId2.Text == "" || xbCount2 == 0)
                    && (txt_xbId3.Text == "" || xbCount3 == 0) && (txt_xbId4.Text == "" || xbCount4 == 0)
                    && (txt_xbId5.Text == "" || xbCount5 == 0))
                { }
                else
                {
                    string cmd = "INSERT INTO " + txt_sanvtName.Text + @".dbo.vitem (Account,Disable,Card,Login_time,Get_time,SName,CharName,Type,"
                        + "DataID1,Number1,DataID2,Number2,DataID3,Number3,DataID4,Number4,DataID5,Number5)"
                        + "values ('" + txt_xbAccount.Text + "',0,CONVERT(varchar(100), GETDATE(), 21),getdate(),getdate(),0,0,0,"
                        + xbId1 + "," + xbCount1 + "," + xbId2 + "," + xbCount2 + "," + xbId3 + "," + xbCount3 + "," + xbId4 + "," + xbCount4 + "," + xbId5 + "," + xbCount5 + ")";
                    string ret = CSGHelper.SqlCommand(cmd);
                    if (ret == "success")
                    {

                        SGExHandle.WriteXbLog(txt_xbAccount.Text + "\t" + txt_xbName.Text + "\t"
                            + txt_xbId1.Text + "\t" + txt_xbName1.Text + "\t" + txt_xbCount1.Text + "\t"
                            + txt_xbId2.Text + "\t" + txt_xbName2.Text + "\t" + txt_xbCount2.Text + "\t"
                            + txt_xbId4.Text + "\t" + txt_xbName3.Text + "\t" + txt_xbCount3.Text + "\t"
                            + txt_xbId5.Text + "\t" + txt_xbName4.Text + "\t" + txt_xbCount4.Text + "\t"
                            + txt_xbId6.Text + "\t" + txt_xbName5.Text + "\t" + txt_xbCount5.Text + "\t");

                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = txt_xbAccount.Text;
                        lvi.SubItems.Add(txt_xbName.Text);
                        lvi.SubItems.Add(txt_xbId1.Text);
                        lvi.SubItems.Add(txt_xbName1.Text);
                        lvi.SubItems.Add(txt_xbCount1.Text);
                        lvi.SubItems.Add(txt_xbId2.Text);
                        lvi.SubItems.Add(txt_xbName2.Text);
                        lvi.SubItems.Add(txt_xbCount2.Text);
                        lvi.SubItems.Add(txt_xbId3.Text);
                        lvi.SubItems.Add(txt_xbName3.Text);
                        lvi.SubItems.Add(txt_xbCount3.Text);
                        lvi.SubItems.Add(txt_xbId4.Text);
                        lvi.SubItems.Add(txt_xbName4.Text);
                        lvi.SubItems.Add(txt_xbCount4.Text);
                        lvi.SubItems.Add(txt_xbId5.Text);
                        lvi.SubItems.Add(txt_xbName5.Text);
                        lvi.SubItems.Add(txt_xbCount5.Text);
                        lstv_xblog.Items.Add(lvi);
                    }else
                    {
                        string msg2 = "\n"
                            + "账户：" + txt_xbAccount.Text + "\n"
                            + "角色：" + txt_xbName.Text + "\n";
                        if(txt_xbId1.Text != "" && xbCount1 != 0)
                        {
                            msg += "物品：" + txt_xbName1.Text + "\t数量：" + txt_xbCount1.Text + "\t\n";
                        }
                        if(txt_xbId2.Text != "" && xbCount2 != 0)
                        {
                            msg += "物品：" + txt_xbName2.Text + "\t数量：" + txt_xbCount2.Text + "\t\n";
                        }
                        if (txt_xbId3.Text != "" && xbCount3 != 0)
                        {
                            msg += "物品：" + txt_xbName3.Text + "\t数量：" + txt_xbCount3.Text + "\t\n";
                        }
                        if (txt_xbId4.Text != "" && xbCount4 != 0)
                        {
                            msg += "物品：" + txt_xbName4.Text + "\t数量：" + txt_xbCount4.Text + "\t\n";
                        }
                        if (txt_xbId5.Text != "" && xbCount5 != 0)
                        {
                            msg += "物品：" + txt_xbName5.Text + "\t数量：" + txt_xbCount5.Text + "\t\n";
                        }
                        MessageBox.Show("发送失败，\t\n" + msg2);
                    }
                }

                Thread.Sleep(200);
                if((txt_xbId6.Text == "" || xbCount6 == 0) && (txt_xbId7.Text == "" || xbCount7 == 0) 
                    && (txt_xbId8.Text == "" || xbCount8 == 0) && (txt_xbId9.Text == "" || xbCount9 == 0) 
                    && (txt_xbId10.Text == "" || xbCount10 == 0) )
                { }
                else
                {
                    string cmd = "INSERT INTO " + txt_sanvtName.Text + @".dbo.vitem (Account,Disable,Card,Login_time,Get_time,SName,CharName,Type,"
                    + "DataID1,Number1,DataID2,Number2,DataID3,Number3,DataID4,Number4,DataID5,Number5)"
                    + "values ('" + txt_xbAccount.Text + "',0,CONVERT(varchar(100), GETDATE(), 21),getdate(),getdate(),0,0,0,"
                    + xbId6 + "," + xbCount6 + "," + xbId7 + "," + xbCount7 + "," + xbId8 + "," + xbCount8 + "," + xbId9 + "," + xbCount9 + "," + xbId10 + "," + xbCount10 + ")";
                    string ret = CSGHelper.SqlCommand(cmd);
                    if (ret == "success")
                    {
                        SGExHandle.WriteXbLog(txt_xbAccount.Text + "\t" + txt_xbName.Text + "\t"
                        + txt_xbId6.Text + "\t" + txt_xbName6.Text + "\t" + txt_xbCount6.Text + "\t"
                        + txt_xbId7.Text + "\t" + txt_xbName7.Text + "\t" + txt_xbCount7.Text + "\t"
                        + txt_xbId8.Text + "\t" + txt_xbName8.Text + "\t" + txt_xbCount8.Text + "\t"
                        + txt_xbId9.Text + "\t" + txt_xbName9.Text + "\t" + txt_xbCount9.Text + "\t"
                        + txt_xbId10.Text + "\t" + txt_xbName10.Text + "\t" + txt_xbCount10.Text + "\t");

                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = txt_xbAccount.Text;
                        lvi.SubItems.Add(txt_xbName.Text);
                        lvi.SubItems.Add(txt_xbId6.Text);
                        lvi.SubItems.Add(txt_xbName6.Text);
                        lvi.SubItems.Add(txt_xbCount6.Text);
                        lvi.SubItems.Add(txt_xbId7.Text);
                        lvi.SubItems.Add(txt_xbName7.Text);
                        lvi.SubItems.Add(txt_xbCount7.Text);
                        lvi.SubItems.Add(txt_xbId8.Text);
                        lvi.SubItems.Add(txt_xbName8.Text);
                        lvi.SubItems.Add(txt_xbCount8.Text);
                        lvi.SubItems.Add(txt_xbId9.Text);
                        lvi.SubItems.Add(txt_xbName9.Text);
                        lvi.SubItems.Add(txt_xbCount9.Text);
                        lvi.SubItems.Add(txt_xbId10.Text);
                        lvi.SubItems.Add(txt_xbName10.Text);
                        lvi.SubItems.Add(txt_xbCount10.Text);
                        lstv_xblog.Items.Add(lvi);
                    }
                    else
                    {
                        string msg2 = "\n"
                            + "账户：" + txt_xbAccount.Text + "\n"
                            + "角色：" + txt_xbName.Text + "\n";
                        if (txt_xbId6.Text != "" && xbCount6 != 0)
                        {
                            msg += "物品：" + txt_xbName6.Text + "\t数量：" + txt_xbCount6.Text + "\t\n";
                        }
                        if (txt_xbId7.Text != "" && xbCount7 != 0)
                        {
                            msg += "物品：" + txt_xbName7.Text + "\t数量：" + txt_xbCount7.Text + "\t\n";
                        }
                        if (txt_xbId8.Text != "" && xbCount8 != 0)
                        {
                            msg += "物品：" + txt_xbName8.Text + "\t数量：" + txt_xbCount8.Text + "\t\n";
                        }
                        if (txt_xbId9.Text != "" && xbCount9 != 0)
                        {
                            msg += "物品：" + txt_xbName9.Text + "\t数量：" + txt_xbCount9.Text + "\t\n";
                        }
                        if (txt_xbId10.Text != "" && xbCount10 != 0)
                        {
                            msg += "物品：" + txt_xbName10.Text + "\t数量：" + txt_xbCount10.Text + "\t\n";
                        }
                        MessageBox.Show("发送失败，\t\n" + msg2);
                    }
                }
                Thread.Sleep(200);
                //代币
                if (!string.IsNullOrEmpty(txt_dbSend.Text) && CFormat.isNumberic(txt_dbSend.Text))
                {
                    int dbCount = Convert.ToInt32(txt_dbSend.Text);
                    if (dbCount > 0)
                    {
                        //虚宝增加
                        string cmd = "DECLARE @account varchar(21) \n"
                                + "DECLARE @point int \n"
                                + "DECLARE @old_point int \n"
                                + "DECLARE @new_point int \n"
                                + "set @account = '" + txt_xbAccount.Text + "' \n"
                                + "set @point = " + dbCount + " \n"
                                + "Select @old_point=point from dbo.game_acc where account = @account \n"
                                + "SET @new_point = @point + @old_point \n"
                                + "Update dbo.game_acc set point = @new_point where account = @account";
                        string ret = CSGHelper.SqlCommand(cmd);
                        if (ret == "success")
                        {
                            txt_dbCurr.Text = "" + CSGHelper.SelectAcountPoint(txt_xbAccount.Text);
                        }
                        else
                        {
                            MessageBox.Show("发送代币失败，代币：" + dbCount);
                        }
                    }
                }

                if (cbx_xbconf.Text == "新人起步")
                {
                    m_SGExHandle.SendWorldWords("新人玩家 " + txt_xbName.Text + " 的新手起步已经发放，请到大鸿胪处领取虚宝，欢迎加入大家庭，祝游戏愉快。");
                }
            }
            else
            {
                MessageBox.Show("请核对发放帐号！");
            }
        }

        private int xbconf_SelectedIndex = 0;
        private void clear_xbConfItems()
        {
            for (int i = 1; i <= 10; i++)
            {
                string ctrlName = "txt_xbId" + i;
                Control col = this.groupBox17.Controls.Find(ctrlName, true)[0];
                TextBox txt = col as TextBox;//转为TextBox
                txt.Text = "";

                ctrlName = "txt_xbName" + i;
                col = this.groupBox17.Controls.Find(ctrlName, true)[0];
                txt = col as TextBox;//转为TextBox
                txt.Text = "";

                ctrlName = "txt_xbCount" + i;
                col = this.groupBox17.Controls.Find(ctrlName, true)[0];
                txt = col as TextBox;//转为TextBox
                txt.Text = "";
            }
        }
        private void cbx_xbconf_SelectedIndexChanged(object sender, EventArgs e)
        {
            clear_xbConfItems();
            xbconf_SelectedIndex = cbx_xbconf.SelectedIndex;
            string xbconfCap = "xbconfCap" + cbx_xbconf.SelectedIndex;
            string xbconfDesc = "xbconfDesc" + cbx_xbconf.SelectedIndex;
            txt_xbconfCap.Text = CIniCtrl.ReadIniData("XbConf", xbconfCap, "", serverIni);
            rbx_confDesc.Text = CIniCtrl.ReadIniData("XbConf", xbconfDesc, "", serverIni);

            //读取
            string tmp = "xbconfItemsId" + cbx_xbconf.SelectedIndex;
            string ret = CIniCtrl.ReadIniData("XbConf", tmp, "", serverIni);
            var Ids = ret.Split(',');
            for (int i = 0; i < Ids.Length; i++)
            {
                string ctrlName = "txt_xbId" + (i + 1);
                Control col = this.groupBox17.Controls.Find(ctrlName, true)[0];
                TextBox txt = col as TextBox;//转为TextBox
                txt.Text = Ids[i];
            }
            tmp = "xbconfItemsName" + cbx_xbconf.SelectedIndex;
            ret = CIniCtrl.ReadIniData("XbConf", tmp, "", serverIni);
            var Names = ret.Split(',');
            for (int i = 0; i < Names.Length; i++)
            {
                string ctrlName = "txt_xbName" + (i + 1);
                Control col = this.groupBox17.Controls.Find(ctrlName, true)[0];
                TextBox txt = col as TextBox;//转为TextBox
                txt.Text = Names[i];
            }

            tmp = "xbconfItemsCount" + cbx_xbconf.SelectedIndex;
            ret = CIniCtrl.ReadIniData("XbConf", tmp, "", serverIni);
            var Counts = ret.Split(',');
            for (int i = 0; i < Counts.Length; i++)
            {
                string ctrlName = "txt_xbCount" + (i + 1);
                Control col = this.groupBox17.Controls.Find(ctrlName, true)[0];
                TextBox txt = col as TextBox;//转为TextBox
                txt.Text = Counts[i];
            }

            tmp = "xbconfDb" + cbx_xbconf.SelectedIndex;
            ret = CIniCtrl.ReadIniData("XbConf", tmp, "", serverIni);
            txt_dbSend.Text = ret;
        }

        private void ben_xbconfSave_Click(object sender, EventArgs e)
        {
            if (xbconf_SelectedIndex >= 0)
            {
                if (!string.IsNullOrEmpty(txt_xbconfCap.Text))
                {
                    //保存配置
                    string xbconfCap = "xbconfCap" + xbconf_SelectedIndex;
                    string xbconfDesc = "xbconfDesc" + xbconf_SelectedIndex;
                    CIniCtrl.WriteIniData("XbConf", xbconfCap, txt_xbconfCap.Text, serverIni);
                    CIniCtrl.WriteIniData("XbConf", xbconfDesc, rbx_confDesc.Text, serverIni);

                    //物品代币
                    string tmp = "xbconfItemsId" + xbconf_SelectedIndex;
                    string xbconfItemsId = txt_xbId1.Text + "," + txt_xbId2.Text + "," + txt_xbId3.Text + "," + txt_xbId4.Text + "," + txt_xbId5.Text
                        + "," + txt_xbId6.Text + "," + txt_xbId7.Text + "," + txt_xbId8.Text + "," + txt_xbId9.Text + "," + txt_xbId10.Text;
                    CIniCtrl.WriteIniData("XbConf", tmp, xbconfItemsId, serverIni);
                    tmp = "xbconfItemsName" + xbconf_SelectedIndex;
                    string xbconfItemsName = txt_xbName1.Text + "," + txt_xbName2.Text + "," + txt_xbName3.Text + "," + txt_xbName4.Text + "," + txt_xbName5.Text
                        + "," + txt_xbName6.Text + "," + txt_xbName7.Text + "," + txt_xbName8.Text + "," + txt_xbName9.Text + "," + txt_xbName10.Text;
                    CIniCtrl.WriteIniData("XbConf", tmp, xbconfItemsName, serverIni);
                    tmp = "xbconfItemsCount" + xbconf_SelectedIndex;
                    string xbconfItemsCount = txt_xbCount1.Text + "," + txt_xbCount2.Text + "," + txt_xbCount3.Text + "," + txt_xbCount4.Text + "," + txt_xbCount5.Text
                        + "," + txt_xbCount6.Text + "," + txt_xbCount7.Text + "," + txt_xbCount8.Text + "," + txt_xbCount9.Text + "," + txt_xbCount10.Text;
                    CIniCtrl.WriteIniData("XbConf", tmp, xbconfItemsCount, serverIni);
                    tmp = "xbconfDb" + xbconf_SelectedIndex;
                    CIniCtrl.WriteIniData("XbConf", tmp, txt_dbSend.Text, serverIni);

                    cbx_xbconf.Items[xbconf_SelectedIndex] = txt_xbconfCap.Text;
                }
            }
        }

        static int xblogSltIndex = 0;
        private void lstv_xblog_SelectedIndexChanged(object sender, EventArgs e)
        {
            //
            if (lstv_xblog.SelectedIndices != null && lstv_xblog.SelectedIndices.Count > 0)
            {
                lstv_xblog.Items[xblogSltIndex].BackColor = Color.Transparent;
                xblogSltIndex = this.lstv_xblog.SelectedItems[0].Index;
                lstv_xblog.Items[xblogSltIndex].BackColor = Color.Pink;


                txt_xbAccount.Text = lstv_xblog.Items[xblogSltIndex].SubItems[0].Text;
                txt_xbName.Text = lstv_xblog.Items[xblogSltIndex].SubItems[1].Text;
                txt_xbId1.Text = lstv_xblog.Items[xblogSltIndex].SubItems[2].Text;
                txt_xbName1.Text = lstv_xblog.Items[xblogSltIndex].SubItems[3].Text;
                txt_xbCount1.Text = lstv_xblog.Items[xblogSltIndex].SubItems[4].Text;
                txt_xbId2.Text = lstv_xblog.Items[xblogSltIndex].SubItems[5].Text;
                txt_xbName2.Text = lstv_xblog.Items[xblogSltIndex].SubItems[6].Text;
                txt_xbCount2.Text = lstv_xblog.Items[xblogSltIndex].SubItems[7].Text;
                txt_xbId3.Text = lstv_xblog.Items[xblogSltIndex].SubItems[8].Text;
                txt_xbName3.Text = lstv_xblog.Items[xblogSltIndex].SubItems[9].Text;
                txt_xbCount3.Text = lstv_xblog.Items[xblogSltIndex].SubItems[10].Text;
                txt_xbId4.Text = lstv_xblog.Items[xblogSltIndex].SubItems[11].Text;
                txt_xbName4.Text = lstv_xblog.Items[xblogSltIndex].SubItems[12].Text;
                txt_xbCount4.Text = lstv_xblog.Items[xblogSltIndex].SubItems[13].Text;
                txt_xbId5.Text = lstv_xblog.Items[xblogSltIndex].SubItems[14].Text;
                txt_xbName5.Text = lstv_xblog.Items[xblogSltIndex].SubItems[15].Text;
                txt_xbCount5.Text = lstv_xblog.Items[xblogSltIndex].SubItems[16].Text;

                txt_xbId6.Text = "";
                txt_xbName6.Text = "";
                txt_xbCount6.Text = "";
                txt_xbId7.Text = "";
                txt_xbName7.Text = "";
                txt_xbCount7.Text = "";
                txt_xbId8.Text = "";
                txt_xbName8.Text = "";
                txt_xbCount8.Text = "";
                txt_xbId9.Text = "";
                txt_xbName9.Text = "";
                txt_xbCount9.Text = "";
                txt_xbId10.Text = "";
                txt_xbName10.Text = "";
                txt_xbCount10.Text = "";
                //查询代币
                txt_dbCurr.Text = "" + CSGHelper.SelectAcountPoint(txt_xbAccount.Text);
            }
        }

        private void txt_xbId1_TextChanged(object sender, EventArgs e)
        {
            txt_xbName1.Text = CItemCtrl.GetNameById(txt_xbId1.Text);
        }

        private void txt_xbId2_TextChanged(object sender, EventArgs e)
        {
            txt_xbName2.Text = CItemCtrl.GetNameById(txt_xbId2.Text);
        }

        private void txt_xbId3_TextChanged(object sender, EventArgs e)
        {
            txt_xbName3.Text = CItemCtrl.GetNameById(txt_xbId3.Text);
        }

        private void txt_xbId4_TextChanged(object sender, EventArgs e)
        {
            txt_xbName4.Text = CItemCtrl.GetNameById(txt_xbId4.Text);
        }

        private void txt_xbId5_TextChanged(object sender, EventArgs e)
        {
            txt_xbName5.Text = CItemCtrl.GetNameById(txt_xbId5.Text);
        }

        private void txt_xbId6_TextChanged(object sender, EventArgs e)
        {
            txt_xbName6.Text = CItemCtrl.GetNameById(txt_xbId6.Text);
        }

        private void txt_xbId7_TextChanged(object sender, EventArgs e)
        {
            txt_xbName7.Text = CItemCtrl.GetNameById(txt_xbId7.Text);
        }

        private void txt_xbId8_TextChanged(object sender, EventArgs e)
        {
            txt_xbName8.Text = CItemCtrl.GetNameById(txt_xbId8.Text);
        }

        private void txt_xbId9_TextChanged(object sender, EventArgs e)
        {
            txt_xbName9.Text = CItemCtrl.GetNameById(txt_xbId9.Text);
        }

        private void txt_xbId10_TextChanged(object sender, EventArgs e)
        {
            txt_xbName10.Text = CItemCtrl.GetNameById(txt_xbId10.Text);
        }

        #region -- 仅数字绑定
        private void txt_dbSend_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_xbCount1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_xbCount2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_xbCount3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_xbCount4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_xbCount5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_xbCount6_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_xbCount7_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_xbCount8_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_xbCount9_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_xbCount10_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_xbId1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_xbId2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_xbId3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_xbId4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_xbId5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_xbId6_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_xbId7_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_xbId8_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_xbId9_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_xbId10_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }
        #endregion
        #endregion

        #region//国战英雄奖励
        private void LoadGZMrgInfos()
        {
            //加载物品列表
            FileGzItemsView();
            //加载配置
            warHandle.Init();
            warHandle.BaseForlder = txt_svrForder.Text;
            warHandle.SetWarStatusCallFunc(WarStatusCallFunc);
            warHandle.SetWarFinishCallFunc(WarFinishCallFunc);

            FillGzConfView();

            //其他
            if (warHandle.EnableLessScore)
            {
                cbx_enLessScore.Checked = true;
            }
            else
            {
                cbx_enLessScore.Checked = false;
            }

            if (warHandle.EnableLessKill)
            {
                cbx_enLessKill.Checked = true;
            }
            else
            {
                cbx_enLessKill.Checked = false;
            }

            if (warHandle.EnableGoodLuck)
            {
                cbx_enGoodLuck.Checked = true;
            }
            else
            {
                cbx_enGoodLuck.Checked = false;
            }

            txt_gzLessTD.Text = warHandle.LessKillValue.ToString();
            txt_gzLessGX.Text = warHandle.LessScoreValue.ToString();

            //List<int> date = new List<int> { 2, 5 };
            //warHandle.SetWarTime(date, "12:30", 90);
        }
        static int gzitemDefSltIndex = 0;
        private void FileGzItemsView()
        {
            string allItemDefines = CItemCtrl.load_Item_Defines(System.AppDomain.CurrentDomain.BaseDirectory + "profile");
            var allItemDefinesArr = allItemDefines.Replace("\t", "").Split(';');

            this.lstv_gzItems.Items.Clear();
            foreach (var _def in allItemDefinesArr)
            {
                var _nameWithIdarr = _def.Split(',');
                if (_nameWithIdarr.Length > 1)
                {
                    string _name = _nameWithIdarr[0];
                    string _id = _nameWithIdarr[1];
                    ListViewItem lvi = new ListViewItem();
                    //lvi.ImageIndex = i;     //通过与imageList绑定，显示imageList中第i项图标 
                    lvi.Text = _id.Replace("\t", "");
                    //lvi.SubItems.Add(_id);
                    lvi.SubItems.Add(_name);
                    this.lstv_gzItems.Items.Add(lvi);
                }
            }
            this.lstv_gzItems.EndUpdate();  //结束数据处理，UI界面一次性绘制
        }
        private void FillGzConfView()
        {
            lstv_gzConf.Items.Clear();
            List<WarRewordConf> _WarRewordConf = warHandle.GetRewordConf();

            for (int j = 0; j < _WarRewordConf.Count; j++)
            {
                ListViewItem lvi = new ListViewItem();

                lvi.Text = _WarRewordConf[j].start.ToString();
                lvi.SubItems.Add(_WarRewordConf[j].end.ToString());
                lvi.SubItems.Add(_WarRewordConf[j].id1.ToString() == "0" ? "" : _WarRewordConf[j].id1.ToString());
                lvi.SubItems.Add(_WarRewordConf[j].name1.ToString());
                lvi.SubItems.Add(_WarRewordConf[j].count1.ToString() == "0" ? "" : _WarRewordConf[j].count1.ToString());
                lvi.SubItems.Add(_WarRewordConf[j].id2.ToString() == "0" ? "" : _WarRewordConf[j].id2.ToString());
                lvi.SubItems.Add(_WarRewordConf[j].name2.ToString());
                lvi.SubItems.Add(_WarRewordConf[j].count2.ToString() == "0" ? "" : _WarRewordConf[j].count2.ToString());
                lvi.SubItems.Add(_WarRewordConf[j].id3.ToString() == "0" ? "" : _WarRewordConf[j].id3.ToString());
                lvi.SubItems.Add(_WarRewordConf[j].name3.ToString());
                lvi.SubItems.Add(_WarRewordConf[j].count3.ToString() == "0" ? "" : _WarRewordConf[j].count3.ToString());
                lvi.SubItems.Add(_WarRewordConf[j].id4.ToString() == "0" ? "" : _WarRewordConf[j].id4.ToString());
                lvi.SubItems.Add(_WarRewordConf[j].name4.ToString());
                lvi.SubItems.Add(_WarRewordConf[j].count4.ToString() == "0" ? "" : _WarRewordConf[j].count4.ToString());
                lvi.SubItems.Add(_WarRewordConf[j].id5.ToString() == "0" ? "" : _WarRewordConf[j].id5.ToString());
                lvi.SubItems.Add(_WarRewordConf[j].name5.ToString());
                lvi.SubItems.Add(_WarRewordConf[j].count5.ToString() == "0" ? "" : _WarRewordConf[j].count5.ToString());
                lvi.SubItems.Add(_WarRewordConf[j].flag.ToString());

                lstv_gzConf.Items.Add(lvi);
            }

            lstv_gzConf.EndUpdate();
        }
        private void lstv_gzItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstv_gzItems.SelectedIndices != null && lstv_gzItems.SelectedIndices.Count > 0)
            {
                lstv_gzItems.Items[gzitemDefSltIndex].BackColor = Color.Transparent;
                gzitemDefSltIndex = this.lstv_gzItems.SelectedItems[0].Index;
                lstv_gzItems.Items[gzitemDefSltIndex].BackColor = Color.Pink;

                //if 有空位
                if (txt_gzId1.Text == string.Empty || txt_gzCount1.Text == string.Empty)
                {
                    txt_gzId1.Text = lstv_gzItems.Items[gzitemDefSltIndex].Text;
                    txt_gzName1.Text = lstv_gzItems.Items[gzitemDefSltIndex].SubItems[1].Text;
                    txt_gzCount1.Text = "1";
                }
                else if (txt_gzId2.Text == string.Empty || txt_gzCount2.Text == string.Empty)
                {
                    txt_gzId2.Text = lstv_gzItems.Items[gzitemDefSltIndex].Text;
                    txt_gzName2.Text = lstv_gzItems.Items[gzitemDefSltIndex].SubItems[1].Text;
                    txt_gzCount2.Text = "1";
                }
                else if (txt_gzId3.Text == string.Empty || txt_gzCount3.Text == string.Empty)
                {
                    txt_gzId3.Text = lstv_gzItems.Items[gzitemDefSltIndex].Text;
                    txt_gzName3.Text = lstv_gzItems.Items[gzitemDefSltIndex].SubItems[1].Text;
                    txt_gzCount3.Text = "1";
                }
                else if (txt_gzId4.Text == string.Empty || txt_gzCount4.Text == string.Empty)
                {
                    txt_gzId4.Text = lstv_gzItems.Items[gzitemDefSltIndex].Text;
                    txt_gzName4.Text = lstv_gzItems.Items[gzitemDefSltIndex].SubItems[1].Text;
                    txt_gzCount4.Text = "1";
                }
                else if (txt_gzId5.Text == string.Empty || txt_gzCount5.Text == string.Empty)
                {
                    txt_gzId5.Text = lstv_gzItems.Items[gzitemDefSltIndex].Text;
                    txt_gzName5.Text = lstv_gzItems.Items[gzitemDefSltIndex].SubItems[1].Text;
                    txt_gzCount5.Text = "1";
                }
            }
        }

        private void btn_gzItemsReload_Click(object sender, EventArgs e)
        {
            FileGzItemsView();
        }

        private void btn_gzItemSrch_Click(object sender, EventArgs e)
        {
            //ListViewItem foundItem = this.listView1.FindItemWithText(this.txt_srchItemName.Text, true, cur_select_index + 1);    //参数1：要查找的文本；参数2：是否子项也要查找；参数3：开始查找位置  
            ListViewItem foundItem = null;
            lstv_gzItems.Items[gzitemDefSltIndex].BackColor = Color.Transparent;
            int i = gzitemDefSltIndex + 1;
            if (i == lstv_gzItems.Items.Count)
            {
                i = 0;
            }
            while (i != gzitemDefSltIndex)
            {
                if (lstv_gzItems.Items[i].SubItems[1].Text.Contains(txt_gzItemSrch.Text) || lstv_gzItems.Items[i].Text == txt_gzItemSrch.Text)
                {
                    foundItem = lstv_gzItems.Items[i];
                }
                i++;
                if (i == lstv_gzItems.Items.Count)
                {
                    i = 0;
                }
            }

            if (foundItem != null)
            {
                lstv_gzItems.TopItem = foundItem;  //定位到该项
                lstv_gzItems.Items[foundItem.Index].Focused = true;
                //lstv_xbItems.Items[foundItem.Index].Selected = true;
                foundItem.BackColor = Color.Pink;
                gzitemDefSltIndex = foundItem.Index;
            }
        }

        void FillWarHerosLstv()
        {
            List<HeroScore> heroScore = warHandle.GetHeroScore(true);
            lstv_gzHeros.Items.Clear();
            for (int i = 0; i < heroScore.Count; i++)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = (i + 1).ToString();
                lvi.SubItems.Add(CPlayerCtrl.GetAccByName(heroScore[i].name));
                lvi.SubItems.Add(heroScore[i].name);
                lvi.SubItems.Add(heroScore[i].score.ToString());
                lvi.SubItems.Add(heroScore[i].num.ToString());

                lstv_gzHeros.Items.Add(lvi);
            }
            lstv_gzHeros.EndUpdate();
        }
        private void btn_heroRd_Click(object sender, EventArgs e)
        {
            CPlayerCtrl.LoadPlayerInfos(txt_svrForder.Text + "\\DataBase\\saves\\players.dat", true);
            warHandle.BaseForlder = txt_svrForder.Text;
            FillWarHerosLstv();
        }

        private void btn_heroItemsSend_Click(object sender, EventArgs e)
        {
            warHandle.GetHeroScore(true);
            //发送奖励
            warHandle.SendWarReward();
        }

        bool g_gzAuto = false;
        private void btn_gzAuto_Click(object sender, EventArgs e)
        {
            if (!m_Active)
            {
                MessageBox.Show("软件尚未激活！ 请联系软件发布人给予激活！");
                return;
            }
            if (!g_gzAuto)
            {
                g_gzAuto = true;
                btn_gzAuto.Text = "停止自动";
                warHandle.AutoSend = cbx_gzAutoSend.Checked;
                warHandle.Init();
                warHandle.StartThread();
            }
            else
            {
                g_gzAuto = false;
                btn_gzAuto.Text = "自动功能";
                warHandle.StopThread();
            }
        }

        private void WarFinishCallFunc()
        {
            this.Invoke((EventHandler)delegate
            {
                //更新英雄信息
                FillWarHerosLstv();

                //更新占城信息
                FillWarOrgStageLstv();

                //更新防守信息
                FillWarOrgConstStageLstv(1);
            });
        }
        private void WarStatusCallFunc(int flag)
        {
            if (flag == 1)
            {
                this.Invoke((EventHandler)delegate
                {
                    lbl_gzStatus.Text = "国战进行中...";
                    lbl_orgStatus.Text = "国战进行中...";
                    //更新占城信息
                    FillWarOrgStageLstv();

                    //更新防守信息
                    FillWarOrgConstStageLstv(0);
                });
            }
            else
            {
                this.Invoke((EventHandler)delegate
                {
                    lbl_gzStatus.Text = "国战未进行...";
                    lbl_orgStatus.Text = "国战未进行...";
                });
            }
        }

        private void txt_gzId1_TextChanged(object sender, EventArgs e)
        {
            txt_gzName1.Text = CItemCtrl.GetNameById(txt_gzId1.Text);
        }

        private void txt_gzId2_TextChanged(object sender, EventArgs e)
        {
            txt_gzName2.Text = CItemCtrl.GetNameById(txt_gzId2.Text);
        }

        private void txt_gzId3_TextChanged(object sender, EventArgs e)
        {
            txt_gzName3.Text = CItemCtrl.GetNameById(txt_gzId3.Text);
        }

        private void txt_gzId4_TextChanged(object sender, EventArgs e)
        {
            txt_gzName4.Text = CItemCtrl.GetNameById(txt_gzId4.Text);
        }

        private void txt_gzId5_TextChanged(object sender, EventArgs e)
        {
            txt_gzName5.Text = CItemCtrl.GetNameById(txt_gzId5.Text);
        }

        private void UpdateGzConfig()
        {
            //更新配置文件
            FileStream fs = new FileStream(".\\Profile\\国战英雄奖励配置.xls", FileMode.Create, FileAccess.Write);
            StreamWriter rw = new StreamWriter(fs, Encoding.Default);//建立StreamWriter为写作准备;

            foreach (ListViewItem item in lstv_gzConf.Items)
            {
                string line = item.SubItems[17].Text + "\t"
                    + item.SubItems[0].Text + "\t"
                    + item.SubItems[1].Text + "\t"
                    + item.SubItems[2].Text + "\t"
                    + item.SubItems[3].Text + "\t"
                    + item.SubItems[4].Text + "\t"
                    + item.SubItems[5].Text + "\t"
                    + item.SubItems[6].Text + "\t"
                    + item.SubItems[7].Text + "\t"
                    + item.SubItems[8].Text + "\t"
                    + item.SubItems[9].Text + "\t"
                    + item.SubItems[10].Text + "\t"
                    + item.SubItems[11].Text + "\t"
                    + item.SubItems[12].Text + "\t"
                    + item.SubItems[13].Text + "\t"
                    + item.SubItems[14].Text + "\t"
                    + item.SubItems[15].Text + "\t"
                    + item.SubItems[16].Text;

                rw.WriteLine(line);
            }

            rw.Close();
            fs.Close();
        }
        void AddGzConfig(bool flag)
        {
            //不允许start end为空, 
            if (txt_gzStartIndex.Text == "" || txt_gzEndIndex.Text == "")
            {
                MessageBox.Show("请合理输入有效排名范围！");
                return;
            }
            //不允许全部无效奖励
            if ((txt_gzId1.Text == "" || txt_gzCount1.Text == "0" || txt_gzCount1.Text == "")
                && (txt_gzId2.Text == "" || txt_gzCount2.Text == "0" || txt_gzCount2.Text == "")
                && (txt_gzId3.Text == "" || txt_gzCount3.Text == "0" || txt_gzCount3.Text == "")
                && (txt_gzId4.Text == "" || txt_gzCount4.Text == "0" || txt_gzCount4.Text == "")
                && (txt_gzId5.Text == "" || txt_gzCount5.Text == "0" || txt_gzCount5.Text == ""))
            {
                MessageBox.Show("无效的奖励设置！");
                return;
            }

            //更新lstv_gzConf
            ListViewItem lvi = new ListViewItem();
            if (!flag)
            {
                lvi.Text = txt_gzStartIndex.Text;
                lvi.SubItems.Add(txt_gzEndIndex.Text);
            }
            else
            {
                lvi.Text = txt_gzGoodLuckNumTail.Text;
                lvi.SubItems.Add(txt_gzGoodLuckNumMax.Text);
            }

            lvi.SubItems.Add(txt_gzId1.Text);
            lvi.SubItems.Add(txt_gzName1.Text);
            lvi.SubItems.Add(txt_gzCount1.Text);
            lvi.SubItems.Add(txt_gzId2.Text);
            lvi.SubItems.Add(txt_gzName2.Text);
            lvi.SubItems.Add(txt_gzCount2.Text);
            lvi.SubItems.Add(txt_gzId3.Text);
            lvi.SubItems.Add(txt_gzName3.Text);
            lvi.SubItems.Add(txt_gzCount3.Text);
            lvi.SubItems.Add(txt_gzId4.Text);
            lvi.SubItems.Add(txt_gzName4.Text);
            lvi.SubItems.Add(txt_gzCount4.Text);
            lvi.SubItems.Add(txt_gzId5.Text);
            lvi.SubItems.Add(txt_gzName5.Text);
            lvi.SubItems.Add(txt_gzCount5.Text);
            if (!flag)
            {
                lvi.SubItems.Add("0");
            }
            else
            {
                lvi.SubItems.Add("1");
            }

            lstv_gzConf.Items.Add(lvi);

            UpdateGzConfig();
        }
        private void btn_gzAddConf_Click(object sender, EventArgs e)
        {
            AddGzConfig(false);
            warHandle.InitRewordConf();
        }

        #region -- 仅数字绑定
        private void txt_gzCount1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_gzCount2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_gzCount3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_gzCount4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_gzCount5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_gzId1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_ItemId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_ItemId_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.V))
            {
                if (Clipboard.ContainsText())
                {
                    try
                    {
                        string txt = Clipboard.GetText();
                        Convert.ToInt64(txt);  //检查是否数字
                        ((TextBox)sender).Text = txt.Trim(); //Ctrl+V 粘贴  
                    }
                    catch (Exception)
                    {
                        e.Handled = true;
                    }
                }
            }
            else if (e.KeyData == (Keys.Control | Keys.C))
            {
                Clipboard.SetDataObject(((TextBox)sender).SelectedText);
                e.Handled = true;
            }
        }

        private void txt_gzId2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_gzId3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_gzId4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_gzId5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_gzStartIndex_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_gzEndIndex_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_gzGoodLuckNumTail_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_gzLessTD_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_gzLessGX_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }
        #endregion

        private void btn_SetGoodLuck_Click(object sender, EventArgs e)
        {
            if (txt_gzGoodLuckNumTail.Text == "" || txt_gzGoodLuckNumMax.Text == "")
            {
                MessageBox.Show("请输入幸运位数和最大排名！");
                return;
            }
            AddGzConfig(true);
            warHandle.InitRewordConf();
        }

        private void btn_gzDelConf_Click(object sender, EventArgs e)
        {
            if (gzConfSltIndex >= lstv_gzConf.Items.Count)
            {
                return;
            }
            lstv_gzConf.Items.Remove(lstv_gzConf.Items[gzConfSltIndex]);

            UpdateGzConfig();
            warHandle.InitRewordConf();
        }

        static int gzConfSltIndex = 0;
        private void lstv_gzConf_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstv_gzConf.SelectedIndices != null && lstv_gzConf.SelectedIndices.Count > 0)
            {
                lstv_gzConf.Items[gzConfSltIndex].BackColor = Color.Transparent;
                gzConfSltIndex = lstv_gzConf.SelectedItems[0].Index;
                lstv_gzConf.Items[gzConfSltIndex].BackColor = Color.Pink;
                if (lstv_gzConf.Items[gzConfSltIndex].SubItems[17].Text == "1")
                {
                    txt_gzGoodLuckNumTail.Text = lstv_gzConf.Items[gzConfSltIndex].SubItems[0].Text;
                    txt_gzGoodLuckNumMax.Text = lstv_gzConf.Items[gzConfSltIndex].SubItems[1].Text;
                }
                else
                {
                    txt_gzStartIndex.Text = lstv_gzConf.Items[gzConfSltIndex].SubItems[0].Text;
                    txt_gzEndIndex.Text = lstv_gzConf.Items[gzConfSltIndex].SubItems[1].Text;
                }

                txt_gzId1.Text = lstv_gzConf.Items[gzConfSltIndex].SubItems[2].Text;
                txt_gzName1.Text = lstv_gzConf.Items[gzConfSltIndex].SubItems[3].Text;
                txt_gzCount1.Text = lstv_gzConf.Items[gzConfSltIndex].SubItems[4].Text;
                txt_gzId2.Text = lstv_gzConf.Items[gzConfSltIndex].SubItems[5].Text;
                txt_gzName2.Text = lstv_gzConf.Items[gzConfSltIndex].SubItems[6].Text;
                txt_gzCount2.Text = lstv_gzConf.Items[gzConfSltIndex].SubItems[7].Text;
                txt_gzId3.Text = lstv_gzConf.Items[gzConfSltIndex].SubItems[8].Text;
                txt_gzName3.Text = lstv_gzConf.Items[gzConfSltIndex].SubItems[9].Text;
                txt_gzCount3.Text = lstv_gzConf.Items[gzConfSltIndex].SubItems[10].Text;
                txt_gzId4.Text = lstv_gzConf.Items[gzConfSltIndex].SubItems[11].Text;
                txt_gzName4.Text = lstv_gzConf.Items[gzConfSltIndex].SubItems[12].Text;
                txt_gzCount4.Text = lstv_gzConf.Items[gzConfSltIndex].SubItems[13].Text;
                txt_gzId5.Text = lstv_gzConf.Items[gzConfSltIndex].SubItems[14].Text;
                txt_gzName5.Text = lstv_gzConf.Items[gzConfSltIndex].SubItems[15].Text;
                txt_gzCount5.Text = lstv_gzConf.Items[gzConfSltIndex].SubItems[16].Text;
            }
        }

        private void cbx_enLessKill_CheckedChanged(object sender, EventArgs e)
        {
            warHandle.EnableLessKill = cbx_enLessKill.Checked;
        }

        private void cbx_enLessScore_CheckedChanged(object sender, EventArgs e)
        {
            warHandle.EnableLessScore = cbx_enLessScore.Checked;
        }

        private void cbx_enGoodLuck_CheckedChanged(object sender, EventArgs e)
        {
            warHandle.EnableGoodLuck = cbx_enGoodLuck.Checked;
        }
        private void cbx_gzAutoSend_CheckedChanged(object sender, EventArgs e)
        {
            warHandle.AutoSend = cbx_gzAutoSend.Checked;
        }
        #endregion

        #region//赤壁英雄奖励
        private void LoadCbMrgInfos()
        {
            //加载物品列表
            FileCbItemsView();
            //加载配置
            cbHandle.BaseForlder = txt_svrForder.Text;
            cbHandle.Init();
            cbHandle.SetWarStatusCallFunc(CbStatusCallFunc);
            cbHandle.SetWarFinishCallFunc(CbFinishCallFunc);

            FillCbConfView();

            //其他
            if (cbHandle.EnableLessScore)
            {
                cbx_enzcLessScore.Checked = true;
            }
            else
            {
                cbx_enzcLessScore.Checked = false;
            }

            if (cbHandle.EnableLessKill)
            {
                cbx_enzcLessKill.Checked = true;
            }
            else
            {
                cbx_enzcLessKill.Checked = false;
            }

            if (cbHandle.EnableGoodLuck)
            {
                cbx_enzcGoodLuck.Checked = true;
            }
            else
            {
                cbx_enzcGoodLuck.Checked = false;
            }

            txt_zcLessTD.Text = cbHandle.LessKillValue.ToString();
            txt_zcLessGX.Text = cbHandle.LessScoreValue.ToString();

            //List<int> date = new List<int> { 2, 5 };
            //cbHandle.SetWarTime(date, "12:30", 90);
        }
        private void FillCbConfView()
        {
            lstv_zcConf.Items.Clear();
            List<WarRewordConf> _WarRewordConf = cbHandle.GetRewordConf();

            for (int j = 0; j < _WarRewordConf.Count; j++)
            {
                ListViewItem lvi = new ListViewItem();

                lvi.Text = _WarRewordConf[j].start.ToString();
                lvi.SubItems.Add(_WarRewordConf[j].end.ToString());
                lvi.SubItems.Add(_WarRewordConf[j].id1.ToString() == "0" ? "" : _WarRewordConf[j].id1.ToString());
                lvi.SubItems.Add(_WarRewordConf[j].name1.ToString());
                lvi.SubItems.Add(_WarRewordConf[j].count1.ToString() == "0" ? "" : _WarRewordConf[j].count1.ToString());
                lvi.SubItems.Add(_WarRewordConf[j].id2.ToString() == "0" ? "" : _WarRewordConf[j].id2.ToString());
                lvi.SubItems.Add(_WarRewordConf[j].name2.ToString());
                lvi.SubItems.Add(_WarRewordConf[j].count2.ToString() == "0" ? "" : _WarRewordConf[j].count2.ToString());
                lvi.SubItems.Add(_WarRewordConf[j].id3.ToString() == "0" ? "" : _WarRewordConf[j].id3.ToString());
                lvi.SubItems.Add(_WarRewordConf[j].name3.ToString());
                lvi.SubItems.Add(_WarRewordConf[j].count3.ToString() == "0" ? "" : _WarRewordConf[j].count3.ToString());
                lvi.SubItems.Add(_WarRewordConf[j].id4.ToString() == "0" ? "" : _WarRewordConf[j].id4.ToString());
                lvi.SubItems.Add(_WarRewordConf[j].name4.ToString());
                lvi.SubItems.Add(_WarRewordConf[j].count4.ToString() == "0" ? "" : _WarRewordConf[j].count4.ToString());
                lvi.SubItems.Add(_WarRewordConf[j].id5.ToString() == "0" ? "" : _WarRewordConf[j].id5.ToString());
                lvi.SubItems.Add(_WarRewordConf[j].name5.ToString());
                lvi.SubItems.Add(_WarRewordConf[j].count5.ToString() == "0" ? "" : _WarRewordConf[j].count5.ToString());
                lvi.SubItems.Add(_WarRewordConf[j].flag.ToString());

                lstv_zcConf.Items.Add(lvi);
            }

            lstv_zcConf.EndUpdate();
        }
        private void CbFinishCallFunc()
        {
            this.Invoke((EventHandler)delegate
            {
                FillCbHerosLstv();
            });
        }
        private void CbStatusCallFunc(int flag)
        {
            if (flag == 1)
            {
                this.Invoke((EventHandler)delegate
                {
                    lbl_zcStatus.Text = "赤壁战场进行中...";
                });
            }
            else
            {
                this.Invoke((EventHandler)delegate
                {
                    lbl_zcStatus.Text = "赤壁战场未进行...";
                });
            }
        }

        private void FileCbItemsView()
        {
            string allItemDefines = CItemCtrl.load_Item_Defines(System.AppDomain.CurrentDomain.BaseDirectory + "profile");
            var allItemDefinesArr = allItemDefines.Replace("\t", "").Split(';');

            this.lstv_zcItems.Items.Clear();
            foreach (var _def in allItemDefinesArr)
            {
                var _nameWithIdarr = _def.Split(',');
                if (_nameWithIdarr.Length > 1)
                {
                    string _name = _nameWithIdarr[0];
                    string _id = _nameWithIdarr[1];
                    ListViewItem lvi = new ListViewItem();
                    //lvi.ImageIndex = i;     //通过与imageList绑定，显示imageList中第i项图标 
                    lvi.Text = _id.Replace("\t", "");
                    //lvi.SubItems.Add(_id);
                    lvi.SubItems.Add(_name);
                    this.lstv_zcItems.Items.Add(lvi);
                }
            }
            this.lstv_zcItems.EndUpdate();  //结束数据处理，UI界面一次性绘制
        }

        private void btn_zcItemsReload_Click(object sender, EventArgs e)
        {
            FileCbItemsView();
        }

        void FillCbHerosLstv()
        {
            List<HeroScore> heroScore = cbHandle.GetHeroScore(true);
            lstv_zcHeros.Items.Clear();
            for (int i = 0; i < heroScore.Count; i++)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = (i + 1).ToString();
                lvi.SubItems.Add(CPlayerCtrl.GetAccByName(heroScore[i].name));
                lvi.SubItems.Add(heroScore[i].name);
                lvi.SubItems.Add(heroScore[i].score.ToString());
                lvi.SubItems.Add(heroScore[i].num.ToString());

                lstv_zcHeros.Items.Add(lvi);
            }
            lstv_zcHeros.EndUpdate();
        }
        private void btn_zcheroRd_Click(object sender, EventArgs e)
        {
            CPlayerCtrl.LoadPlayerInfos(txt_svrForder.Text + "\\DataBase\\saves\\players.dat", true);
            cbHandle.BaseForlder = txt_svrForder.Text;
            FillCbHerosLstv();
        }
        private void btn_zcheroItemsSend_Click(object sender, EventArgs e)
        {
            cbHandle.GetHeroScore(true);
            //发送奖励
            cbHandle.SendWarReward();
        }
        static int zcitemDefSltIndex = 0;
        private void btn_zcItemSrch_Click(object sender, EventArgs e)
        {
            //ListViewItem foundItem = this.listView1.FindItemWithText(this.txt_srchItemName.Text, true, cur_select_index + 1);    //参数1：要查找的文本；参数2：是否子项也要查找；参数3：开始查找位置  
            ListViewItem foundItem = null;
            lstv_zcItems.Items[zcitemDefSltIndex].BackColor = Color.Transparent;
            int i = zcitemDefSltIndex + 1;
            if (i == lstv_zcItems.Items.Count)
            {
                i = 0;
            }
            while (i != zcitemDefSltIndex)
            {
                if (lstv_zcItems.Items[i].SubItems[1].Text.Contains(txt_zcItemSrch.Text) || lstv_zcItems.Items[i].Text == txt_zcItemSrch.Text)
                {
                    foundItem = lstv_zcItems.Items[i];
                }
                i++;
                if (i == lstv_zcItems.Items.Count)
                {
                    i = 0;
                }
            }

            if (foundItem != null)
            {
                lstv_zcItems.TopItem = foundItem;  //定位到该项
                lstv_zcItems.Items[foundItem.Index].Focused = true;
                //lstv_xbItems.Items[foundItem.Index].Selected = true;
                foundItem.BackColor = Color.Pink;
                zcitemDefSltIndex = foundItem.Index;
            }
        }

        private void lstv_zcItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstv_zcItems.SelectedIndices != null && lstv_zcItems.SelectedIndices.Count > 0)
            {
                lstv_zcItems.Items[zcitemDefSltIndex].BackColor = Color.Transparent;
                zcitemDefSltIndex = this.lstv_zcItems.SelectedItems[0].Index;
                lstv_zcItems.Items[zcitemDefSltIndex].BackColor = Color.Pink;

                //if 有空位
                if (txt_cbId1.Text == string.Empty || txt_cbCount1.Text == string.Empty)
                {
                    txt_cbId1.Text = lstv_zcItems.Items[zcitemDefSltIndex].Text;
                    txt_cbName1.Text = lstv_zcItems.Items[zcitemDefSltIndex].SubItems[1].Text;
                    txt_cbCount1.Text = "1";
                }
                else if (txt_cbId2.Text == string.Empty || txt_cbCount2.Text == string.Empty)
                {
                    txt_cbId2.Text = lstv_zcItems.Items[zcitemDefSltIndex].Text;
                    txt_cbName2.Text = lstv_zcItems.Items[zcitemDefSltIndex].SubItems[1].Text;
                    txt_cbCount2.Text = "1";
                }
                else if (txt_cbId3.Text == string.Empty || txt_cbCount3.Text == string.Empty)
                {
                    txt_cbId3.Text = lstv_zcItems.Items[zcitemDefSltIndex].Text;
                    txt_cbName3.Text = lstv_zcItems.Items[zcitemDefSltIndex].SubItems[1].Text;
                    txt_cbCount3.Text = "1";
                }
                else if (txt_cbId4.Text == string.Empty || txt_cbCount4.Text == string.Empty)
                {
                    txt_cbId4.Text = lstv_zcItems.Items[zcitemDefSltIndex].Text;
                    txt_cbName4.Text = lstv_zcItems.Items[zcitemDefSltIndex].SubItems[1].Text;
                    txt_cbCount4.Text = "1";
                }
                else if (txt_cbId5.Text == string.Empty || txt_cbCount5.Text == string.Empty)
                {
                    txt_cbId5.Text = lstv_zcItems.Items[zcitemDefSltIndex].Text;
                    txt_cbName5.Text = lstv_zcItems.Items[zcitemDefSltIndex].SubItems[1].Text;
                    txt_cbCount5.Text = "1";
                }
            }
        }

        private void cbx_enzcLessKill_CheckedChanged(object sender, EventArgs e)
        {
            cbHandle.EnableLessKill = cbx_enzcLessKill.Checked;
        }

        private void cbx_enzcLessScore_CheckedChanged(object sender, EventArgs e)
        {
            cbHandle.EnableLessScore = cbx_enzcLessScore.Checked;
        }

        private void cbx_enzcGoodLuck_CheckedChanged(object sender, EventArgs e)
        {
            cbHandle.EnableGoodLuck = cbx_enzcGoodLuck.Checked;
        }
        private void cbx_zcAutoSend_CheckedChanged(object sender, EventArgs e)
        {
            cbHandle.AutoSend = cbx_zcAutoSend.Checked;
        }

        private void txt_cbId1_TextChanged(object sender, EventArgs e)
        {
            txt_cbName1.Text = CItemCtrl.GetNameById(txt_cbId1.Text);
        }

        private void txt_cbId2_TextChanged(object sender, EventArgs e)
        {
            txt_cbName2.Text = CItemCtrl.GetNameById(txt_cbId2.Text);
        }

        private void txt_cbId3_TextChanged(object sender, EventArgs e)
        {
            txt_cbName3.Text = CItemCtrl.GetNameById(txt_cbId3.Text);
        }

        private void txt_cbId4_TextChanged(object sender, EventArgs e)
        {
            txt_cbName4.Text = CItemCtrl.GetNameById(txt_cbId4.Text);
        }

        private void txt_cbId5_TextChanged(object sender, EventArgs e)
        {
            txt_cbName5.Text = CItemCtrl.GetNameById(txt_cbId5.Text);
        }

        private void txt_cbId1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_cbId2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_cbId3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_cbId4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_cbId5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_zcStartIndex_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_zcEndIndex_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_zcGoodLuckNumTail_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_zcGoodLuckNumMax_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_zcLessTD_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txt_zcLessGX_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void UpdateCbConfig()
        {
            //更新配置文件
            FileStream fs = new FileStream(".\\Profile\\赤壁英雄奖励配置.xls", FileMode.Create, FileAccess.Write);
            StreamWriter rw = new StreamWriter(fs, Encoding.Default);//建立StreamWriter为写作准备;

            foreach (ListViewItem item in lstv_gzConf.Items)
            {
                string line = item.SubItems[17].Text + "\t"
                    + item.SubItems[0].Text + "\t"
                    + item.SubItems[1].Text + "\t"
                    + item.SubItems[2].Text + "\t"
                    + item.SubItems[3].Text + "\t"
                    + item.SubItems[4].Text + "\t"
                    + item.SubItems[5].Text + "\t"
                    + item.SubItems[6].Text + "\t"
                    + item.SubItems[7].Text + "\t"
                    + item.SubItems[8].Text + "\t"
                    + item.SubItems[9].Text + "\t"
                    + item.SubItems[10].Text + "\t"
                    + item.SubItems[11].Text + "\t"
                    + item.SubItems[12].Text + "\t"
                    + item.SubItems[13].Text + "\t"
                    + item.SubItems[14].Text + "\t"
                    + item.SubItems[15].Text + "\t"
                    + item.SubItems[16].Text;

                rw.WriteLine(line);
            }

            rw.Close();
            fs.Close();
        }
        void AddCbConfig(bool flag)
        {
            //不允许start end为空, 
            if (txt_zcStartIndex.Text == "" || txt_zcEndIndex.Text == "")
            {
                MessageBox.Show("请合理输入有效排名范围！");
                return;
            }
            //不允许全部无效奖励
            if ((txt_cbId1.Text == "" || txt_cbCount1.Text == "0" || txt_cbCount1.Text == "")
                && (txt_cbId2.Text == "" || txt_cbCount2.Text == "0" || txt_cbCount2.Text == "")
                && (txt_cbId3.Text == "" || txt_cbCount3.Text == "0" || txt_cbCount3.Text == "")
                && (txt_cbId4.Text == "" || txt_cbCount4.Text == "0" || txt_cbCount4.Text == "")
                && (txt_cbId5.Text == "" || txt_cbCount5.Text == "0" || txt_cbCount5.Text == ""))
            {
                MessageBox.Show("无效的奖励设置！");
                return;
            }

            //更新lstv_gzConf
            ListViewItem lvi = new ListViewItem();
            if (!flag)
            {
                lvi.Text = txt_zcStartIndex.Text;
                lvi.SubItems.Add(txt_zcEndIndex.Text);
            }
            else
            {
                lvi.Text = txt_zcGoodLuckNumTail.Text;
                lvi.SubItems.Add(txt_zcGoodLuckNumMax.Text);
            }

            lvi.SubItems.Add(txt_cbId1.Text);
            lvi.SubItems.Add(txt_cbName1.Text);
            lvi.SubItems.Add(txt_cbCount1.Text);
            lvi.SubItems.Add(txt_cbId2.Text);
            lvi.SubItems.Add(txt_cbName2.Text);
            lvi.SubItems.Add(txt_cbCount2.Text);
            lvi.SubItems.Add(txt_cbId3.Text);
            lvi.SubItems.Add(txt_cbName3.Text);
            lvi.SubItems.Add(txt_cbCount3.Text);
            lvi.SubItems.Add(txt_cbId4.Text);
            lvi.SubItems.Add(txt_cbName4.Text);
            lvi.SubItems.Add(txt_cbCount4.Text);
            lvi.SubItems.Add(txt_cbId5.Text);
            lvi.SubItems.Add(txt_cbName5.Text);
            lvi.SubItems.Add(txt_cbCount5.Text);
            if (!flag)
            {
                lvi.SubItems.Add("0");
            }
            else
            {
                lvi.SubItems.Add("1");
            }

            lstv_zcConf.Items.Add(lvi);

            UpdateCbConfig();
        }

        private void btn_SetCbGoodLuck_Click(object sender, EventArgs e)
        {
            if (txt_zcGoodLuckNumTail.Text == "" || txt_zcGoodLuckNumMax.Text == "")
            {
                MessageBox.Show("请输入幸运位数和最大排名！");
                return;
            }
            AddCbConfig(true);
            cbHandle.InitRewordConf();
        }

        private void btn_zcAddConf_Click(object sender, EventArgs e)
        {
            AddCbConfig(false);
            cbHandle.InitRewordConf();
        }

        private void btn_zcDelConf_Click(object sender, EventArgs e)
        {
            if (cbConfSltIndex >= lstv_zcConf.Items.Count)
            {
                return;
            }
            lstv_zcConf.Items.Remove(lstv_zcConf.Items[cbConfSltIndex]);

            UpdateCbConfig();
            cbHandle.InitRewordConf();
        }

        static int cbConfSltIndex = 0;
        private void lstv_zcConf_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstv_zcConf.SelectedIndices != null && lstv_zcConf.SelectedIndices.Count > 0)
            {
                lstv_zcConf.Items[cbConfSltIndex].BackColor = Color.Transparent;
                cbConfSltIndex = lstv_zcConf.SelectedItems[0].Index;
                lstv_zcConf.Items[cbConfSltIndex].BackColor = Color.Pink;
                if (lstv_zcConf.Items[cbConfSltIndex].SubItems[17].Text == "1")
                {
                    txt_zcGoodLuckNumTail.Text = lstv_zcConf.Items[cbConfSltIndex].SubItems[0].Text;
                    txt_zcGoodLuckNumMax.Text = lstv_zcConf.Items[cbConfSltIndex].SubItems[1].Text;
                }
                else
                {
                    txt_zcStartIndex.Text = lstv_zcConf.Items[cbConfSltIndex].SubItems[0].Text;
                    txt_zcEndIndex.Text = lstv_zcConf.Items[cbConfSltIndex].SubItems[1].Text;
                }

                txt_cbId1.Text = lstv_zcConf.Items[cbConfSltIndex].SubItems[2].Text;
                txt_cbName1.Text = lstv_zcConf.Items[cbConfSltIndex].SubItems[3].Text;
                txt_cbCount1.Text = lstv_zcConf.Items[cbConfSltIndex].SubItems[4].Text;
                txt_cbId2.Text = lstv_zcConf.Items[cbConfSltIndex].SubItems[5].Text;
                txt_cbName2.Text = lstv_zcConf.Items[cbConfSltIndex].SubItems[6].Text;
                txt_cbCount2.Text = lstv_zcConf.Items[cbConfSltIndex].SubItems[7].Text;
                txt_cbId3.Text = lstv_zcConf.Items[cbConfSltIndex].SubItems[8].Text;
                txt_cbName3.Text = lstv_zcConf.Items[cbConfSltIndex].SubItems[9].Text;
                txt_cbCount3.Text = lstv_zcConf.Items[cbConfSltIndex].SubItems[10].Text;
                txt_cbId4.Text = lstv_zcConf.Items[cbConfSltIndex].SubItems[11].Text;
                txt_cbName4.Text = lstv_zcConf.Items[cbConfSltIndex].SubItems[12].Text;
                txt_cbCount4.Text = lstv_zcConf.Items[cbConfSltIndex].SubItems[13].Text;
                txt_cbId5.Text = lstv_zcConf.Items[cbConfSltIndex].SubItems[14].Text;
                txt_cbName5.Text = lstv_zcConf.Items[cbConfSltIndex].SubItems[15].Text;
                txt_cbCount5.Text = lstv_zcConf.Items[cbConfSltIndex].SubItems[16].Text;
            }
        
        }

        bool g_cbAuto = false;
        private void btn_zcAuto_Click(object sender, EventArgs e)
        {
            if (!m_Active)
            {
                MessageBox.Show("软件尚未激活！ 请联系软件发布人给予激活！");
                return;
            }
            if (!g_cbAuto)
            {
                g_cbAuto = true;
                btn_zcAuto.Text = "停止自动";
                cbHandle.AutoSend = cbx_zcAutoSend.Checked;
                cbHandle.Init();
                cbHandle.StartThread();
            }
            else
            {
                g_cbAuto = false;
                btn_zcAuto.Text = "自动功能";
                cbHandle.StopThread();
            }
        }
        #endregion

        #region //国战攻占奖励
        void FillWarOrgStageLstv()
        {
            List<OrganizeRst> rst = warHandle.GetNoticeStage();
            lstv_OrgLast.Items.Clear();
            for (int i = 0; i < rst.Count; i++)
            {
                ListViewItem lvi = new ListViewItem();

                lvi.Text = rst[i].stage.ToString();
                lvi.SubItems.Add(rst[i].legion);
                lvi.SubItems.Add(rst[i].leader);
                lvi.SubItems.Add("");

                lstv_OrgLast.Items.Add(lvi);
            }
            lstv_OrgLast.EndUpdate();
        }
        void FillWarOrgConstStageLstv(int flag)
        {
            StageCtrl.LoadStageDefInfo();
            List<OrganizeConstRst> rst = warHandle.GetNoticeConstStage(flag);
            lstv_OrgConst.Items.Clear();
            for (int i = 0; i < rst.Count; i++)
            {
                ListViewItem lvi = new ListViewItem();

                lvi.Text = StageCtrl.GetNameById(rst[i].stage.ToString());
                lvi.SubItems.Add(rst[i].legion);
                lvi.SubItems.Add(rst[i].bTime);
                lvi.SubItems.Add(rst[i].eTime);

                lstv_OrgConst.Items.Add(lvi);
            }
            lstv_OrgConst.EndUpdate();
        }
        private void btn_OrgStageRd_Click(object sender, EventArgs e)
        {
            FillWarOrgStageLstv();
        }

        private void btn_OrgItemsSend_Click(object sender, EventArgs e)
        {

        }

        private void UpdateWarOrgConfig()
        {
            //更新配置文件
            FileStream fs = new FileStream(".\\Profile\\国战防守奖励配置.xls", FileMode.Create, FileAccess.Write);
            StreamWriter rw = new StreamWriter(fs, Encoding.Default);//建立StreamWriter为写作准备;

            foreach (ListViewItem item in lstv_orgConf.Items)
            {
                string line = item.SubItems[0].Text + "\t"
                    + item.SubItems[1].Text + "\t"
                    + item.SubItems[2].Text + "\t"
                    + item.SubItems[3].Text + "\t"
                    + item.SubItems[4].Text + "\t"
                    + item.SubItems[5].Text + "\t"
                    + item.SubItems[6].Text + "\t"
                    + item.SubItems[7].Text + "\t"
                    + item.SubItems[8].Text + "\t"
                    + item.SubItems[9].Text + "\t"
                    + item.SubItems[10].Text + "\t"
                    + item.SubItems[11].Text + "\t"
                    + item.SubItems[12].Text + "\t"
                    + item.SubItems[13].Text + "\t"
                    + item.SubItems[14].Text + "\t"
                    + item.SubItems[15].Text + "\t"
                    + item.SubItems[16].Text + "\t"
                    + item.SubItems[17].Text
                    ;

                rw.WriteLine(line);
            }

            rw.Close();
            fs.Close();
        }
        void AddWarOrgConfig(bool flag)
        {
            //不允许start end为空, 
            if (txt_gzStartIndex.Text == "" || txt_gzEndIndex.Text == "")
            {
                MessageBox.Show("请合理输入有效排名范围！");
                return;
            }
            //不允许全部无效奖励
            if ((txt_gzId1.Text == "" || txt_gzCount1.Text == "0" || txt_gzCount1.Text == "")
                && (txt_gzId2.Text == "" || txt_gzCount2.Text == "0" || txt_gzCount2.Text == "")
                && (txt_gzId3.Text == "" || txt_gzCount3.Text == "0" || txt_gzCount3.Text == "")
                && (txt_gzId4.Text == "" || txt_gzCount4.Text == "0" || txt_gzCount4.Text == "")
                && (txt_gzId5.Text == "" || txt_gzCount5.Text == "0" || txt_gzCount5.Text == ""))
            {
                MessageBox.Show("无效的奖励设置！");
                return;
            }

            //更新lstv_gzConf
            ListViewItem lvi = new ListViewItem();

            lvi.Text = cbx_orgStages.Text;
            lvi.SubItems.Add(cbx_orgType.Text);

            lvi.SubItems.Add(txt_gzId1.Text);
            lvi.SubItems.Add(txt_gzName1.Text);
            lvi.SubItems.Add(txt_gzCount1.Text);
            lvi.SubItems.Add(txt_gzId2.Text);
            lvi.SubItems.Add(txt_gzName2.Text);
            lvi.SubItems.Add(txt_gzCount2.Text);
            lvi.SubItems.Add(txt_gzId3.Text);
            lvi.SubItems.Add(txt_gzName3.Text);
            lvi.SubItems.Add(txt_gzCount3.Text);
            lvi.SubItems.Add(txt_gzId4.Text);
            lvi.SubItems.Add(txt_gzName4.Text);
            lvi.SubItems.Add(txt_gzCount4.Text);
            lvi.SubItems.Add(txt_gzId5.Text);
            lvi.SubItems.Add(txt_gzName5.Text);
            lvi.SubItems.Add(txt_gzCount5.Text);
            if (cbx_orgType.Text == "占城奖励")
            {
                lvi.SubItems.Add("0");
            }
            else
            {
                lvi.SubItems.Add(cbx_orgStillMin.Text);
            }

            lstv_orgConf.Items.Add(lvi);

            UpdateWarOrgConfig();
        }

        private void btn_orgAddConf_Click(object sender, EventArgs e)
        {
            AddWarOrgConfig(false);
            warHandle.InitRewordConf();
        }

        static int warOrgConfSltIndex = 0;
        private void btn_orgDelConf_Click(object sender, EventArgs e)
        {
            if (warOrgConfSltIndex >= lstv_orgConf.Items.Count)
            {
                return;
            }
            lstv_orgConf.Items.Remove(lstv_orgConf.Items[warOrgConfSltIndex]);

            UpdateWarOrgConfig();
            warHandle.InitRewordConf();
        }

        private void lstv_orgConf_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstv_orgConf.SelectedIndices != null && lstv_orgConf.SelectedIndices.Count > 0)
            {
                lstv_orgConf.Items[warOrgConfSltIndex].BackColor = Color.Transparent;
                warOrgConfSltIndex = lstv_orgConf.SelectedItems[0].Index;
                lstv_orgConf.Items[warOrgConfSltIndex].BackColor = Color.Pink;

                cbx_orgStages.Text = lstv_orgConf.Items[warOrgConfSltIndex].SubItems[0].Text;
                cbx_orgType.Text = lstv_orgConf.Items[warOrgConfSltIndex].SubItems[1].Text;
                txt_orgId1.Text = lstv_orgConf.Items[warOrgConfSltIndex].SubItems[2].Text;
                txt_orgName1.Text = lstv_orgConf.Items[warOrgConfSltIndex].SubItems[3].Text;
                txt_orgCount1.Text = lstv_orgConf.Items[warOrgConfSltIndex].SubItems[4].Text;
                txt_orgId2.Text = lstv_orgConf.Items[warOrgConfSltIndex].SubItems[5].Text;
                txt_orgName2.Text = lstv_orgConf.Items[warOrgConfSltIndex].SubItems[6].Text;
                txt_orgCount2.Text = lstv_orgConf.Items[warOrgConfSltIndex].SubItems[7].Text;
                txt_orgId3.Text = lstv_orgConf.Items[warOrgConfSltIndex].SubItems[8].Text;
                txt_orgName3.Text = lstv_orgConf.Items[warOrgConfSltIndex].SubItems[9].Text;
                txt_orgCount3.Text = lstv_orgConf.Items[warOrgConfSltIndex].SubItems[10].Text;
                txt_orgId4.Text = lstv_orgConf.Items[warOrgConfSltIndex].SubItems[11].Text;
                txt_orgName4.Text = lstv_orgConf.Items[warOrgConfSltIndex].SubItems[12].Text;
                txt_orgCount4.Text = lstv_orgConf.Items[warOrgConfSltIndex].SubItems[13].Text;
                txt_orgId5.Text = lstv_orgConf.Items[warOrgConfSltIndex].SubItems[14].Text;
                txt_orgName5.Text = lstv_orgConf.Items[warOrgConfSltIndex].SubItems[15].Text;
                txt_orgCount5.Text = lstv_orgConf.Items[warOrgConfSltIndex].SubItems[16].Text;
                cbx_orgStillMin.Text = lstv_orgConf.Items[warOrgConfSltIndex].SubItems[17].Text;
            }
        }

        private void FileOrgItemsView()
        { 
            string allItemDefines = CItemCtrl.load_Item_Defines(System.AppDomain.CurrentDomain.BaseDirectory + "profile");
            var allItemDefinesArr = allItemDefines.Replace("\t", "").Split(';');

            this.lstv_warOrgItems.Items.Clear();
            foreach (var _def in allItemDefinesArr)
            {
                var _nameWithIdarr = _def.Split(',');
                if (_nameWithIdarr.Length > 1)
                {
                    string _name = _nameWithIdarr[0];
                    string _id = _nameWithIdarr[1];
                    ListViewItem lvi = new ListViewItem();
                    //lvi.ImageIndex = i;     //通过与imageList绑定，显示imageList中第i项图标 
                    lvi.Text = _id.Replace("\t", "");
                    //lvi.SubItems.Add(_id);
                    lvi.SubItems.Add(_name);
                    this.lstv_warOrgItems.Items.Add(lvi);
                }
            }
            this.lstv_warOrgItems.EndUpdate();  //结束数据处理，UI界面一次性绘制
        }
        private void btn_warOrgItemLoad_Click(object sender, EventArgs e)
        {
            FileOrgItemsView();
        }

        static int warOrgitemDefSltIndex = 0;
        private void btn_warOrgItemSrch_Click(object sender, EventArgs e)
        {
            ListViewItem foundItem = null;
            lstv_warOrgItems.Items[warOrgitemDefSltIndex].BackColor = Color.Transparent;
            int i = warOrgitemDefSltIndex + 1;
            if (i == lstv_warOrgItems.Items.Count)
            {
                i = 0;
            }
            while (i != warOrgitemDefSltIndex)
            {
                if (lstv_warOrgItems.Items[i].SubItems[1].Text.Contains(txt_warOrgItemSrch.Text) || lstv_warOrgItems.Items[i].Text == txt_warOrgItemSrch.Text)
                {
                    foundItem = lstv_warOrgItems.Items[i];
                }
                i++;
                if (i == lstv_warOrgItems.Items.Count)
                {
                    i = 0;
                }
            }

            if (foundItem != null)
            {
                lstv_warOrgItems.TopItem = foundItem;  //定位到该项
                lstv_warOrgItems.Items[foundItem.Index].Focused = true;
                //lstv_xbItems.Items[foundItem.Index].Selected = true;
                foundItem.BackColor = Color.Pink;
                warOrgitemDefSltIndex = foundItem.Index;
            }
        }

        static int warOrgItemDefSltIndex = 0;
        private void lstv_warOrgItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstv_warOrgItems.SelectedIndices != null && lstv_warOrgItems.SelectedIndices.Count > 0)
            {
                lstv_warOrgItems.Items[warOrgItemDefSltIndex].BackColor = Color.Transparent;
                warOrgItemDefSltIndex = this.lstv_warOrgItems.SelectedItems[0].Index;
                lstv_warOrgItems.Items[warOrgItemDefSltIndex].BackColor = Color.Pink;

                //if 有空位
                if (txt_orgId1.Text == string.Empty || txt_orgCount1.Text == string.Empty)
                {
                    txt_orgId1.Text = lstv_warOrgItems.Items[warOrgItemDefSltIndex].Text;
                    txt_orgName1.Text = lstv_warOrgItems.Items[warOrgItemDefSltIndex].SubItems[1].Text;
                    txt_orgCount1.Text = "1";
                }
                else if (txt_orgId2.Text == string.Empty || txt_orgCount2.Text == string.Empty)
                {
                    txt_orgId2.Text = lstv_warOrgItems.Items[warOrgItemDefSltIndex].Text;
                    txt_orgName2.Text = lstv_warOrgItems.Items[warOrgItemDefSltIndex].SubItems[1].Text;
                    txt_orgCount2.Text = "1";
                }
                else if (txt_orgId3.Text == string.Empty || txt_orgCount3.Text == string.Empty)
                {
                    txt_orgId3.Text = lstv_warOrgItems.Items[warOrgItemDefSltIndex].Text;
                    txt_orgName3.Text = lstv_warOrgItems.Items[warOrgItemDefSltIndex].SubItems[1].Text;
                    txt_orgCount3.Text = "1";
                }
                else if (txt_orgId4.Text == string.Empty || txt_orgCount4.Text == string.Empty)
                {
                    txt_orgId4.Text = lstv_warOrgItems.Items[warOrgItemDefSltIndex].Text;
                    txt_orgName4.Text = lstv_warOrgItems.Items[warOrgItemDefSltIndex].SubItems[1].Text;
                    txt_orgCount4.Text = "1";
                }
                else if (txt_orgId5.Text == string.Empty || txt_orgCount5.Text == string.Empty)
                {
                    txt_orgId5.Text = lstv_warOrgItems.Items[warOrgItemDefSltIndex].Text;
                    txt_orgName5.Text = lstv_warOrgItems.Items[warOrgItemDefSltIndex].SubItems[1].Text;
                    txt_orgCount5.Text = "1";
                }
            }
        }

        private void txt_orgId1_TextChanged(object sender, EventArgs e)
        {
            txt_orgName1.Text = CItemCtrl.GetNameById(txt_orgId1.Text);
        }

        private void txt_orgId2_TextChanged(object sender, EventArgs e)
        {
            txt_orgName2.Text = CItemCtrl.GetNameById(txt_orgId2.Text);
        }

        private void txt_orgId3_TextChanged(object sender, EventArgs e)
        {
            txt_orgName3.Text = CItemCtrl.GetNameById(txt_orgId3.Text);
        }

        private void txt_orgId4_TextChanged(object sender, EventArgs e)
        {
            txt_orgName4.Text = CItemCtrl.GetNameById(txt_orgId4.Text);
        }

        private void txt_orgId5_TextChanged(object sender, EventArgs e)
        {
            txt_orgName5.Text = CItemCtrl.GetNameById(txt_orgId5.Text);
        }

        private void cbx_orgStillStageRewardAuto_CheckedChanged(object sender, EventArgs e)
        {
            warHandle.OrgConstAutoSend = cbx_orgStillStageRewardAuto.Checked;
        }

        private void cbx_orgStageRewardAuto_CheckedChanged(object sender, EventArgs e)
        {
            warHandle.OrgAutoSend = cbx_orgStageRewardAuto.Checked;
        }
        #endregion

        #region //账户管理
        private void btn_AccountReflush_Click(object sender, EventArgs e)
        {
            int count = 0;
            CPlayerCtrl.LoadPlayerInfos(txt_svrForder.Text + "\\DataBase\\saves\\players.dat", true);
            try
            {
                string conn_str = "Data Source = " + sql_srvAddr + "," + sql_srvPort + "; Initial Catalog = " + sqlAccountName + "; User Id = " + sql_srvUser + "; Password = " + sql_srvPwd + ";";
                SqlConnection con = new SqlConnection(conn_str);
                con.Open();
                //执行con对象的函数，返回一个SqlCommand类型的对象
                SqlCommand cmd = con.CreateCommand();
                //把输入的数据拼接成sql语句，并交给cmd对象
                cmd.CommandText = "SELECT * FROM [Account_hcsg].[dbo].[game_acc]";

                //用cmd的函数执行语句，返回SqlDataReader类型的结果dr,dr就是返回的结果集（也就是数据库中查询到的表数据）
                SqlDataReader dr = cmd.ExecuteReader();
                //用dr的read函数，每执行一次，返回一个包含下一行数据的集合dr
                lstv_Account.Items.Clear();
                while (dr.Read())
                {
                    //构建一个ListView的数据，存入数据库数据，以便添加到listView1的行数据中
                    ListViewItem lt = new ListViewItem();
                    //将数据库数据转变成ListView类型的一行数据
                    lt.Text = dr["account"].ToString();
                    lt.SubItems.Add(dr["password"].ToString());
                    lt.SubItems.Add(CPlayerCtrl.GetNameByAcc(lt.Text));
                    lt.SubItems.Add(dr["enable"].ToString());
                    lt.SubItems.Add(dr["privilege"].ToString());
                    lt.SubItems.Add(dr["point"].ToString());
                    lt.SubItems.Add(dr["ip"].ToString());
                    lt.SubItems.Add(dr["LastLoginTime"].ToString());
                    lt.SubItems.Add(dr["LastLogoutTime"].ToString());
                    //将lt数据添加到listView1控件中
                    lstv_Account.Items.Add(lt);
                    //账户 密码 角色 状态 权限 代币 ip 登入时间 登出时间
                    count++;
                }
                lstv_Account.EndUpdate();
                con.Close();
            }
            catch (Exception ex)
            {

            }
            lbl_AccountCount.Text = "帐号总数：" + count.ToString();
        }
        #endregion

        private void lstv_Account_SelectedIndexChanged(object sender, EventArgs e)
        {
            //给各个控件赋值
            if (lstv_Account.SelectedIndices != null && lstv_Account.SelectedIndices.Count > 0)
            {
                lstv_Account.Items[xbAccountInfoSltIndex].BackColor = Color.Transparent;
                xbAccountInfoSltIndex = this.lstv_Account.SelectedItems[0].Index;
                lstv_Account.Items[xbAccountInfoSltIndex].BackColor = Color.Pink;

                txt_AcountName.Text = lstv_Account.SelectedItems[0].SubItems[0].Text;
                txt_AcountPwd.Text = lstv_Account.SelectedItems[0].SubItems[1].Text;
                txt_AcountPoint.Text = lstv_Account.SelectedItems[0].SubItems[5].Text;
                txt_AcountLoginIp.Text = lstv_Account.SelectedItems[0].SubItems[6].Text;
            }
        }

        private void btn_AcountFreeze_Click(object sender, EventArgs e)
        {
            string acc = CFormat.PureString(txt_AcountName.Text);
            if (acc == "")
                return;
            //封号
            string log = "";
            try
            {
                log = CSGHelper.FreezeAccount(acc, 1, "手动封禁！", "GM");
                LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, acc + ":" + log, new StackTrace(new StackFrame(true)));
                btn_AccountReflush_Click(null, null);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, acc + ":" + ex.Message, new StackTrace(new StackFrame(true)));
            }
            MessageBox.Show("账户：" + acc + log);
        }

        private void btn_AcountDisFreeze_Click(object sender, EventArgs e)
        {
            string acc = CFormat.PureString(txt_AcountName.Text);
            if (acc == "")
                return;
            //封号
            string log = "";
            try
            {
                log = CSGHelper.FreezeAccount(acc, 0, "手动解封！", "GM");
                LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, acc + ":" + log, new StackTrace(new StackFrame(true)));
                btn_AccountReflush_Click(null, null);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, acc + ":" + ex.Message, new StackTrace(new StackFrame(true)));
            }
            MessageBox.Show("账户：" + acc + log);
        }

        private void btn_AcountSrch_Click(object sender, EventArgs e)
        {
            ListViewItem foundItem = null;
            lstv_Account.Items[xbAccountInfoSltIndex].BackColor = Color.Transparent;
            int i = xbAccountInfoSltIndex + 1;
            if (i == lstv_Account.Items.Count)
            {
                i = 0;
            }
            while (i != xbAccountInfoSltIndex)
            {
                if (lstv_Account.Items[i].SubItems[0].Text.Contains(txt_AcountSrch.Text)
                    || lstv_Account.Items[i].SubItems[2].Text.Contains(txt_AcountSrch.Text)
                    || lstv_Account.Items[i].SubItems[6].Text.Contains(txt_AcountSrch.Text))
                {
                    foundItem = lstv_Account.Items[i];
                    break;
                }
                i++;
                if (i == lstv_Account.Items.Count)
                {
                    i = 0;
                }
            }

            if (foundItem != null)
            {
                lstv_Account.TopItem = foundItem;  //定位到该项
                lstv_Account.Items[foundItem.Index].Focused = true;
                //lstv_Account.Items[foundItem.Index].Selected = true;
                foundItem.BackColor = Color.Pink;
                xbAccountInfoSltIndex = foundItem.Index;
            }
        }
        static int xbAccountInfoSltIndex = 0;

        private void btn_QAItemSrch_Click(object sender, EventArgs e)
        {
            ListViewItem foundItem = null;
            lstv_QAItems.Items[QAitemDefSltIndex].BackColor = Color.Transparent;
            int i = QAitemDefSltIndex + 1;
            if (i == lstv_QAItems.Items.Count)
            {
                i = 0;
            }
            while (i != QAitemDefSltIndex)
            {
                if (lstv_QAItems.Items[i].SubItems[1].Text.Contains(txt_QAItemSrch.Text) || lstv_QAItems.Items[i].Text == txt_QAItemSrch.Text)
                {
                    foundItem = lstv_QAItems.Items[i];
                }
                i++;
                if (i == lstv_QAItems.Items.Count)
                {
                    i = 0;
                }
            }

            if (foundItem != null)
            {
                lstv_QAItems.TopItem = foundItem;  //定位到该项
                lstv_QAItems.Items[foundItem.Index].Focused = true;
                //lstv_QAItems.Items[foundItem.Index].Selected = true;
                foundItem.BackColor = Color.Pink;
                QAitemDefSltIndex = foundItem.Index;
            }
        }

        private void btn_QAItemReload_Click(object sender, EventArgs e)
        {
            FillQAItemsView();
        }

        private void lstv_QAItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstv_QAItems.SelectedIndices != null && lstv_QAItems.SelectedIndices.Count > 0)
            {
                lstv_QAItems.Items[QAitemDefSltIndex].BackColor = Color.Transparent;
                QAitemDefSltIndex = this.lstv_QAItems.SelectedItems[0].Index;
                lstv_QAItems.Items[QAitemDefSltIndex].BackColor = Color.Pink;

                txt_QAItemId.Text = lstv_QAItems.Items[QAitemDefSltIndex].Text;
                txt_QAItemName.Text = lstv_QAItems.Items[QAitemDefSltIndex].SubItems[1].Text;
            }
        }
        static private int QAitemDefSltIndex = 0;

        private void btn_AQNormalAppend_Click(object sender, EventArgs e)
        {
            if (txt_QAItemId.Text != string.Empty && txt_QAItemName.Text != string.Empty)
            {
                rbx_QANormalDetil.Text += txt_QAItemId.Text + "," + txt_QAItemName.Text + ";";
            }
        }
        private void btn_QANormalSet_Click(object sender, EventArgs e)
        {
            CIniCtrl.WriteIniData("Config", "m_NormalRecharge", rbx_QANormalDetil.Text, serverIni);
        }

        private void btn_AQTaskAppend_Click(object sender, EventArgs e)
        {
            if (txt_QAItemId.Text != string.Empty && txt_QAItemName.Text != string.Empty)
            {
                rbx_QATaskDetil.Text += txt_QAItemId.Text + "," + txt_QAItemName.Text + ";";
            }
        }

        private void btn_AQTaskSet_Click(object sender, EventArgs e)
        {
            string key = "m_TaskRecharge" + cbc_QATaskIndex.Text;
            CIniCtrl.WriteIniData("Config", key, rbx_QATaskDetil.Text, serverIni);
        }

        private void cbc_QATaskIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            string key = "m_TaskRecharge" + cbc_QATaskIndex.Text;
            rbx_QATaskDetil.Text = CIniCtrl.ReadIniData("Config", key, "", serverIni);
        }
    }
}
