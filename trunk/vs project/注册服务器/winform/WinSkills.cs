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
            if (!m_Active)
            {
                MessageBox.Show("软件尚未激活！ 请联系软件发布人给予激活！");
                return;
            }
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

    }
}
