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
        private void btn_startQues_Click(object sender, EventArgs e)
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
            //构建竞赛奖励字符串
            string normalReward = rbx_QANormalDetil.Text;
            string taskReward = "";
            for (int i = 1; i <= 10; i++)
            {
                string key = "m_TaskRecharge" + i;
                string reward = CIniCtrl.ReadIniData("Config", key, "", serverIni);
                if (reward != string.Empty && reward != "")
                {
                    taskReward += reward + "&";
                }
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
                m_SGExHandle.SetQADatVt(txt_svrForder.Text + "\\DataBase\\saves\\players.dat");
                m_SGExHandle.SetQAReward((int)m_AnswerVtId, m_AnswerVtName);
                m_SGExHandle.SetQANormalReward(normalReward);
                m_SGExHandle.SetQATaskReward(taskReward);
                m_SGExHandle.SetMaxQuesNum(g_MaxQuesNum);
                m_SGExHandle.SetNormalInterval((int)m_AskNormalInterval);
                m_SGExHandle.SetTaskTime(m_TaskDate, m_TaskTime);
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
                rbx_QATaskDetil.Text += txt_QAItemId.Text + "," + txt_QAItemName.Text + "," + txt_QAItemCount.Text + ";";
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
        #endregion
    }
}
