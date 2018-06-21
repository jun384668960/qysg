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
        #region //系统公共
        private void button18_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add(rTxtB_WorldWords.Text);
            UpdateWorldWordsList();
        }
        private void button21_Click(object sender, EventArgs e)
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
            if (!m_Active)
            {
                MessageBox.Show("软件尚未激活！ 请联系软件发布人给予激活！");
                return;
            }
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
    }
}
