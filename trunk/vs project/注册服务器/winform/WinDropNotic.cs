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
using System;
using System.Collections.Generic;

namespace MainServer
{
    public partial class WinMainServer : Form
    {
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
        private void btn_15Talk_Click(object sender, EventArgs e)
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
                if (txt_AnnItemsFile.Text != "")
                {
                    rbx_DorpTalkItems.Text = File.ReadAllText(txt_AnnItemsFile.Text, Encoding.Default);
                }
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
    }
}
