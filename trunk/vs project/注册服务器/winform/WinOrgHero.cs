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

    }
}
