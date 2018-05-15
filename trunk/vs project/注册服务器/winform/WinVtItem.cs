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
            while (!string.IsNullOrEmpty(log = sr.ReadLine()) && log != "")
            {
                var items = log.Split('\t');
                if (items.Length >= 16)
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
                if (txt_xbId1.Text != "" && xbCount1 != 0)
                {
                    msg += "物品：" + txt_xbName1.Text + "\t数量：" + txt_xbCount1.Text + "\t\n";
                }
                if (txt_xbId2.Text != "" && xbCount2 != 0)
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
                    }
                    else
                    {
                        string msg2 = "\n"
                            + "账户：" + txt_xbAccount.Text + "\n"
                            + "角色：" + txt_xbName.Text + "\n";
                        if (txt_xbId1.Text != "" && xbCount1 != 0)
                        {
                            msg += "物品：" + txt_xbName1.Text + "\t数量：" + txt_xbCount1.Text + "\t\n";
                        }
                        if (txt_xbId2.Text != "" && xbCount2 != 0)
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
                if ((txt_xbId6.Text == "" || xbCount6 == 0) && (txt_xbId7.Text == "" || xbCount7 == 0)
                    && (txt_xbId8.Text == "" || xbCount8 == 0) && (txt_xbId9.Text == "" || xbCount9 == 0)
                    && (txt_xbId10.Text == "" || xbCount10 == 0))
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
    }
}
