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
    }
}
