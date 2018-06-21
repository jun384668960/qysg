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

namespace MainServer
{
    public partial class WinMainServer : Form
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
        private string sqlSanvtName;
        private string sqlLogName;

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
        public WinMainServer()
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
                    this.Text = this.Text + "  - 未激活,请联系管理员(QQ:384668960)";
                }
                return false;
            }
            else
            {
                this.Text = this.Text + "  - 已激活,技术支持(QQ:384668960 到期时间:" + endTime;
                return true;
            }
        }
        private void Frm_server_Load(object sender, EventArgs e)
        {
            if (!CheckActive())
            {
                m_Active = false;
                return;
            }

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
            sqlSanvtName = CIniCtrl.ReadIniData("Server", "SanvtName", "", serverIni);
            if (sqlSanvtName != "")
            {
                txt_sqlSanvtName.Text = sqlSanvtName;
            }
            sqlLogName = CIniCtrl.ReadIniData("Server", "LogName", "", serverIni);
            if (sqlLogName != "")
            {
                txt_sqlLogName.Text = sqlLogName;
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
                btn_startQues_Click(null, null);
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
                rbx_DorpTalkItems.Text = File.ReadAllText(txt_AnnItemsFile.Text, Encoding.Default);
            }

            string _15NameFilter = CIniCtrl.ReadIniData("Config", "15NameFilter", "", serverIni);
            g_15NameFilter = _15NameFilter;
            rbx_15NameFilter.Text = _15NameFilter;

            string game15TalkAutoStart = CIniCtrl.ReadIniData("Config", "15TalkAutoStart", "", serverIni);
            if (game15TalkAutoStart != "" && game15TalkAutoStart == "Enable")
            {
                cbx_AutoStart15Talk.Checked = true;
                btn_15Talk_Click(null, null);
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
            if (!m_Active)
            {
                MessageBox.Show("软件尚未激活！ 请联系软件发布人给予激活！");
                return;
            }
            CSGHelper.SetSQLNames(sqlAccountName, sqlSanvtName, sqlLogName);
            if (btn_sql.Text == "连接数据库")
            {
                //连接数据库
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
                    txt_sqlSanvtName.ReadOnly = true;
                    txt_sqlLogName.ReadOnly = true;
                }
            }
            else
            {
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
                    txt_sqlSanvtName.ReadOnly = false;
                    txt_sqlLogName.ReadOnly = false;
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

        private void txt_sqlSanvtName_TextChanged(object sender, EventArgs e)
        {
            //写入
            CIniCtrl.WriteIniData("Server", "SanvtName", txt_sqlSanvtName.Text, serverIni);
            sqlSanvtName = txt_sqlSanvtName.Text;
        }

        private void txt_sqlLogName_TextChanged(object sender, EventArgs e)
        {
            //写入
            CIniCtrl.WriteIniData("Server", "LogName", txt_sqlLogName.Text, serverIni);
            sqlLogName = txt_sqlLogName.Text;
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

    }
}
