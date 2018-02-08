using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PidProtect
{
    public partial class Protect : Form
    {
        public static string g_ProtectFile = "";
        public static string g_StartFile = "";
        public static string g_StartFileId = "";
        public Protect(string[] args)
        {
            //MessageBox.Show(args.Length + "");
            if (args.Length < 3)
            {
                this.Dispose();
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                System.Environment.Exit(0);
            }
            g_ProtectFile = args[0];
            g_StartFile = args[1];
            g_StartFileId = args[2];

            InitializeComponent();
        }

        private void Protect_Load(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false; //不显示在系统任务栏
            this.Hide();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Process[] procList = new Process[200];
            procList = Process.GetProcesses();

            bool protect_live = false;
            foreach (var curProcess in procList)
            {
                if (curProcess.ProcessName.ToString() == g_ProtectFile && curProcess.Id.ToString() == g_StartFileId)
                {
                    protect_live = true;
                }
            }
            if (!protect_live)
            {
                //关闭所有游戏名
                var pArrayy = System.Diagnostics.Process.GetProcessesByName(g_StartFile);
                foreach (var item in pArrayy)
                {
                    item.Kill();
                }
                this.Dispose();
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                System.Environment.Exit(0);
            }
        }
    }
}
