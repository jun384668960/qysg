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

    }
}
