using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Common;
using System.Collections;

namespace BOSS刷新工具
{
    public partial class 工具 : Form
    {
        public static bool m_Active = false;
        private string m_ConfIni;
        ListViewColumnSorter lvwColumnSorter = new ListViewColumnSorter();
        
        public 工具()
        {
            InitializeComponent();
            //排序设置
            lvwColumnSorter.SortColumn = 1;
            lvwColumnSorter.Order = SortOrder.Descending;
            this.lstv_DropItemList.ListViewItemSorter = lvwColumnSorter;
        }

        #region  上层控制

        public ItemCtrl m_ItemCtrl = new ItemCtrl();
        public PlayerCtrl m_PlayerCtrl = new PlayerCtrl();
        public ArmyCtrl m_ArmyCtrl = new ArmyCtrl();

        private bool CheckActive()
        {
            try
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
            catch (Exception)
            {
                MessageBox.Show("软件尚未激活，请将目录下的ComputerInfo.key文件发给软件提供者，以获取使用权限！");
                this.Text = this.Text + "  - 未激活,请联系管理员(QQ:384668960)";
            }
            return false;
        }

        private void 工具_Load(object sender, EventArgs e)
        {
            bool ret = false;
            if (!CheckActive())
            {
                m_Active = false;
                return;
            }

            m_Active = true;
            

            //生成new backup 文件夹
            if (!Directory.Exists("backup"))//如果不存在就创建文件夹
            {
                Directory.CreateDirectory("backup");
            }
            if (!Directory.Exists("new"))//如果不存在就创建文件夹
            {
                Directory.CreateDirectory("new");
            }
            if (!Directory.Exists("data"))//如果不存在就创建文件夹
            {
                Directory.CreateDirectory("data");
            }

            //生成商城的风格
            OnCreateItemModePanels();

            //load profile
            ret = OnLoadProfiles();
            if (ret)
            {
                OnFillListView();
            }

            //加载配置
            InitConf();
        }

        private bool OnLoadProfiles()
        {
            bool ret = false;
            //army
            m_ArmyCtrl.SetForder(System.Environment.CurrentDirectory + "\\data");
            ret = m_ArmyCtrl.LoadArmyInfo();
            if (!ret)
            {
                MessageBox.Show("LoadArmyInfo error!请将army相关的文件放入data目录");
                return false;
            }
            //army_name
            ret = m_ArmyCtrl.LoadArmyNameInfo();
            if (!ret)
            {
                MessageBox.Show("LoadArmyNameInfo error!请将army相关的文件放入data目录");
                return false;
            }

            //palyer
            m_PlayerCtrl.SetForder(System.Environment.CurrentDirectory + "\\data");
            ret = m_PlayerCtrl.LoadPlayerDefList();
            if (!ret)
            {
                MessageBox.Show("LoadPlayerDefList error!请将palyer相关的文件放入data目录");
                return false;
            }

            ret = m_PlayerCtrl.LoadPlayerList();
            if (!ret)
            {
                MessageBox.Show("LoadPlayerList error!请将palyer相关的文件放入data目录");
                return false;
            }

            //map
            ret = StageCtrl.LoadStageDefInfo();
            if (!ret)
            {
                MessageBox.Show("LoadStageInfo error!请将Stage相关的文件放入data目录");
                return false;
            }

            //================================================================================================
            //drop
            ret = DropCtrl.LoadDropListInfo();
            if (!ret)
            {
                MessageBox.Show("LoadDropListInfo error!请将Drop相关的文件放入data目录");
                return false;
            }
            //item
            m_ItemCtrl.SetForder(System.Environment.CurrentDirectory + "\\data");
            ret = m_ItemCtrl.LoadItemDefList();
            if (!ret)
            {
                MessageBox.Show("LoadItemDefList error!请将Item相关的文件放入data目录");
                return false;
            }

            ret = m_ItemCtrl.LoadItemList();
            if (!ret)
            {
                MessageBox.Show("LoadItemList error!请将Item相关的文件放入data目录");
                return false;
            }

            //==========================================================================
            ret = ItemModeCtrl.LoadItemModeInfo();
            if (!ret)
            {
                MessageBox.Show("LoadItemModeInfo error!请将ItemMode相关的文件放入data目录");
                return false;
            }
            OnLoadItemModePanels(1, 1);

            //==========================================================================
            ret = ShopCtrl.LoadShopDataList();
            if (!ret)
            {
                MessageBox.Show("LoadShopDataList error!请将ShopData相关的文件放入data目录");
                return false;
            }
            ret = ShopCtrl.LoadShopDefList();
            if (!ret)
            {
                MessageBox.Show("LoadShopDefList error!请将ShopData相关的文件放入data目录");
                return false;
            }
            return true;
        }

        private void OnFillListView()
        {
            //army
            FillArmyListV();
            //army_name
            FillArmyNameListV();
            //palyer
            FillPlayerListV();
            //map
            FillStageDefListV();

            RefreshListV();

            //======================================================================
            //drop
            FillDropListV();
            //item
            FillItemListV(); //包括商城、掉落、商店模块

            DropRefreshListV();


            //======================================================================
            //item

            //======================================================================
            FillShopListV();
            
        }

        #endregion

        #region  boss刷新功能
        private void FillArmyListV()
        {
            List<Army_Str> list = m_ArmyCtrl.GetArmyList();
            lstv_Army.Items.Clear();
            foreach (var item in list)
            {
                string id = item.code;
                string name = m_ArmyCtrl.GetNameById(item.name);
                name = CFormat.ToSimplified(name);

                ListViewItem lvi = new ListViewItem();
                lvi.Text = CFormat.PureString(id);
                lvi.SubItems.Add(CFormat.PureString(name));
                lstv_Army.Items.Add(lvi);
            }
            lstv_Army.EndUpdate();
        }
        private void FillArmyNameListV()
        {
            List<ArmyName_Str> list = m_ArmyCtrl.GetArmyNameList();
            lstv_ArmyName.Items.Clear();
            foreach (var item in list)
            {
                string id = CFormat.PureString(item.id);
                string name = CFormat.ToSimplified(CFormat.PureString(item.name));

                ListViewItem lvi = new ListViewItem();
                lvi.Text = id;
                lvi.SubItems.Add(name);
                lstv_ArmyName.Items.Add(lvi);
            }
            lstv_ArmyName.EndUpdate();
        }
        private void FillPlayerListV()
        {
            List<PlayerDef_Str> list = m_PlayerCtrl.GetPlayerDefList();
            lstv_Player.Items.Clear();
            foreach (var item in list)
            {
                string id = CFormat.PureString(item.id);
                string name = CFormat.ToSimplified(CFormat.PureString(item.name));

                ListViewItem lvi = new ListViewItem();
                lvi.Text = id;
                lvi.SubItems.Add(name);
                lstv_Player.Items.Add(lvi);
            }
            lstv_Player.EndUpdate();
        }
        private void FillStageDefListV()
        {
            List<StageDef_Str> list = StageCtrl.GetStageDefList();
            lstv_StageDef.Items.Clear();
            foreach (var item in list)
            {
                string id = CFormat.PureString(item.id);
                string name = CFormat.ToSimplified(CFormat.PureString(item.name));

                ListViewItem lvi = new ListViewItem();
                lvi.Text = id;
                lvi.SubItems.Add(name);
                lstv_StageDef.Items.Add(lvi);
            }
            lstv_StageDef.EndUpdate();
        }

        private void FillArmyPlayerListV(List<string> list)
        {
            lstv_ArmyList.Items.Clear();
            foreach (var item in list)
            {
                string name = CFormat.ToSimplified(CFormat.PureString(item));

                ListViewItem lvi = new ListViewItem();
                if (name.Contains("role_"))
                {
                    lvi.Text = name.Split(new string[] { "role_" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    lstv_ArmyList.Items.Add(lvi);
                }
            }
            lstv_ArmyList.EndUpdate();
        }

        private void FillArmyMapListV(List<Map_Str> list)
        {
            lstv_ArmyMap.Items.Clear();
            foreach (var item in list)
            {
                string id = CFormat.PureString(item.id);
                string x = CFormat.PureString(item.x);
                string y = CFormat.PureString(item.y);

                ListViewItem lvi = new ListViewItem();
                lvi.Text = CFormat.ToSimplified(StageCtrl.GetNameById(id));
                lvi.SubItems.Add(x);
                lvi.SubItems.Add(y);
                lstv_ArmyMap.Items.Add(lvi);
            }
            lstv_ArmyMap.EndUpdate();
        }

        private void RefreshListV()
        {
            //刷新
            lstv_ArmyMapSltIndex = 0;
            lstv_ArmyListSltIndex = 0;
            lstv_Army.TopItem = lstv_Army.Items[lstv_ArmySltIndex];
            lstv_Army.Items[lstv_ArmySltIndex].Focused = true;
            lstv_Army.Items[lstv_ArmySltIndex].Selected = true;

            lstv_Player.TopItem = lstv_Player.Items[lstv_PlayerSltIndex];
            lstv_Player.Items[lstv_PlayerSltIndex].Focused = true;
            lstv_Player.Items[lstv_PlayerSltIndex].Selected = true;

            lstv_StageDef.TopItem = lstv_StageDef.Items[lstv_StageDefSltIndex];
            lstv_StageDef.Items[lstv_StageDefSltIndex].Focused = true;
            lstv_StageDef.Items[lstv_StageDefSltIndex].Selected = true;

            lstv_ArmyName.TopItem = lstv_ArmyName.Items[lstv_ArmyNameSltIndex];
            lstv_ArmyName.Items[lstv_ArmyNameSltIndex].Focused = true;
            lstv_ArmyName.Items[lstv_ArmyNameSltIndex].Selected = true;

            //get army attr
            Army_Str army;
            bool ret = m_ArmyCtrl.GetAttrByCode(lstv_Army.Items[lstv_ArmySltIndex].Text, out army);
            if (ret)
            {
                txt_ArmyCode.Text = army.code;
                txt_ArmyName.Text = army.name;
                txt_ArmyDelay.Text = army.reborn_delay;
                txt_ArmyRange.Text = army.reborn_range;
                txt_ArmyTime.Text = army.disappear_time;

                FillArmyPlayerListV(army.list);
                FillArmyMapListV(army.map);
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            bool ret = false;
            //palyer
            ret = m_PlayerCtrl.LoadPlayerDefList();
            if (!ret)
            {
                MessageBox.Show("LoadPlayerDefList error!");
                return;
            }

            ret = m_PlayerCtrl.LoadPlayerList();
            if (!ret)
            {
                MessageBox.Show("LoadPlayerList error!");
                return;
            }

            lstv_PlayerSltIndex = 0;
            FillPlayerListV();
            RefreshListV();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            //map
            bool ret = false;
            ret = StageCtrl.LoadStageDefInfo();
            if (!ret)
            {
                MessageBox.Show("LoadStageInfo error!");
                return;
            }

            lstv_StageDefSltIndex = 0;
            FillStageDefListV();
            RefreshListV();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool ret = false;
            //army
            ret = m_ArmyCtrl.LoadArmyInfo();
            if (!ret)
            {
                MessageBox.Show("LoadArmyInfo error!");
                return;
            }
            //army_name
            ret = m_ArmyCtrl.LoadArmyNameInfo();
            if (!ret)
            {
                MessageBox.Show("LoadArmyNameInfo error!");
                return;
            }

            //army
            lstv_ArmySltIndex = 0;
            FillArmyListV();
            //army_name
            lstv_ArmyNameSltIndex = 0;
            FillArmyNameListV();

            RefreshListV();
        }

        static int lstv_ArmySltIndex = 0;
        private void lstv_Army_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstv_Army.SelectedIndices != null && lstv_Army.SelectedIndices.Count > 0)
            {
                lstv_Army.Items[lstv_ArmySltIndex].BackColor = Color.Transparent;
                lstv_ArmySltIndex = lstv_Army.SelectedItems[0].Index;
                lstv_Army.Items[lstv_ArmySltIndex].BackColor = Color.AliceBlue;

                //get army attr
                Army_Str army;
                bool ret = m_ArmyCtrl.GetAttrByCode(lstv_Army.Items[lstv_ArmySltIndex].Text, out army);
                if (ret)
                {
                    txt_ArmyCode.Text = army.code;
                    txt_ArmyName.Text = army.name;
                    txt_ArmyDelay.Text = army.reborn_delay;
                    txt_ArmyRange.Text = army.reborn_range;
                    txt_ArmyTime.Text = army.disappear_time;

                    FillArmyPlayerListV(army.list);
                    FillArmyMapListV(army.map);
                }
            }
        }
        private void btn_ArmySrch_Click(object sender, EventArgs e)
        {
            ListViewItem foundItem = null;
            lstv_Army.Items[lstv_ArmySltIndex].BackColor = Color.Transparent;
            int i = lstv_ArmySltIndex;
            do
            {
                //军团还是BOSS
                if (rdb_ArmySrch_Grp.Checked) //军团
                {
                    i++;
                    if (i == lstv_Army.Items.Count)
                    {
                        i = 0;
                    }
                    if (lstv_Army.Items[i].SubItems[1].Text.Contains(txt_ArmySrch.Text))
                    {
                        foundItem = lstv_Army.Items[i];
                        break;
                    }
                }
                else //BOSS
                {
                    i++;
                    if (i == lstv_Army.Items.Count)
                    {
                        i = 0;
                    }
                    string code = lstv_Army.Items[i].SubItems[0].Text;
                    Army_Str army;
                    bool ret = m_ArmyCtrl.GetAttrByCode(code, out army);
                    if (ret)
                    {
                        string srchRole = CFormat.ToTraditional("role_" + txt_ArmySrch.Text);
                        foreach (var role in army.list)
                        {
                            if (role == srchRole)
                            {
                                foundItem = lstv_Army.Items[i];
                                break;
                            }
                        }
                        if (foundItem != null)
                        {
                            break;
                        }
                    }
                }
            } while (i != lstv_ArmySltIndex);

            if (foundItem != null)
            {
                lstv_Player.Items[lstv_ArmySltIndex].BackColor = Color.White;

                lstv_Army.TopItem = foundItem;  //定位到该项
                lstv_Army.Items[foundItem.Index].Focused = true;
                lstv_Army.Items[foundItem.Index].Selected = true;
                lstv_Army.Items[foundItem.Index].BackColor = Color.AliceBlue;

                lstv_ArmySltIndex = foundItem.Index;

                RefreshListV();
            }
        }

        static int lstv_ArmyListSltIndex = 0;
        private void lstv_ArmyList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstv_ArmyList.SelectedIndices != null && lstv_ArmyList.SelectedIndices.Count > 0)
            {
                lstv_ArmyList.Items[lstv_ArmyListSltIndex].BackColor = Color.Transparent;
                lstv_ArmyListSltIndex = lstv_ArmyList.SelectedItems[0].Index;
                lstv_ArmyList.Items[lstv_ArmyListSltIndex].BackColor = Color.AliceBlue;

                //get army attr
                Player_Str player;
                bool ret = m_PlayerCtrl.GetAttrById("role_" + lstv_ArmyList.Items[lstv_ArmyListSltIndex].Text, out player);
                if (ret)
                {
                    txt_PlayerListRadio.Text = player.appear_ratio;
                    txt_PlayerListName.Text = CFormat.ToSimplified(player.code).Split(new string[] { "role_" }, StringSplitOptions.RemoveEmptyEntries)[0];
                }
            }
        }

        static int lstv_ArmyMapSltIndex = 0;
        private void lstv_ArmyMap_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstv_ArmyMap.SelectedIndices != null && lstv_ArmyMap.SelectedIndices.Count > 0)
            {
                lstv_ArmyMap.Items[lstv_ArmyMapSltIndex].BackColor = Color.Transparent;
                lstv_ArmyMapSltIndex = lstv_ArmyMap.SelectedItems[0].Index;
                lstv_ArmyMap.Items[lstv_ArmyMapSltIndex].BackColor = Color.AliceBlue;


                //get
                txt_ArmyMapId.Text = StageCtrl.GetIdByName(lstv_ArmyMap.Items[lstv_ArmyMapSltIndex].Text);
                txt_ArmyMapName.Text = lstv_ArmyMap.Items[lstv_ArmyMapSltIndex].Text;
                txt_ArmyMapX.Text = lstv_ArmyMap.Items[lstv_ArmyMapSltIndex].SubItems[1].Text;
                txt_ArmyMapY.Text = lstv_ArmyMap.Items[lstv_ArmyMapSltIndex].SubItems[2].Text;
            }
        }

        static int lstv_PlayerSltIndex = 0;
        private void btn_PlayerSrch_Click(object sender, EventArgs e)
        {
            ListViewItem foundItem = null;
            int i = lstv_PlayerSltIndex;
            do
            {
                i++;
                if (i >= lstv_Player.Items.Count)
                {
                    i = 0;
                }
                if (lstv_Player.Items[i].SubItems[1].Text.Contains(txt_PlayerSrch.Text) && txt_PlayerSrch.Text != "")
                {
                    foundItem = lstv_Player.Items[i];
                    break;
                }
            } while (i != lstv_PlayerSltIndex);

            if (foundItem != null)
            {
                lstv_Player.Items[lstv_PlayerSltIndex].BackColor = Color.White;

                lstv_Player.TopItem = foundItem;  //定位到该项
                lstv_Player.Items[foundItem.Index].Focused = true;
                lstv_Player.Items[foundItem.Index].Selected = true;
                lstv_Player.Items[foundItem.Index].BackColor = Color.AliceBlue;
                
                lstv_PlayerSltIndex = foundItem.Index;
            }
        }

        private void lstv_Player_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstv_Player.SelectedIndices != null && lstv_Player.SelectedIndices.Count > 0)
            {
                lstv_Player.Items[lstv_PlayerSltIndex].BackColor = Color.Transparent;
                lstv_PlayerSltIndex = lstv_Player.SelectedItems[0].Index;
                lstv_Player.Items[lstv_PlayerSltIndex].BackColor = Color.AliceBlue;
                //MessageBox.Show("" + lstv_PlayerSltIndex);


                string name = lstv_Player.Items[lstv_PlayerSltIndex].SubItems[1].Text;
                txt_PlayerListName.Text = name;
                Player_Str player;
                bool ret = m_PlayerCtrl.GetAttrById("role_" + name, out player);
                if (ret)
                {
                    txt_PlayerListRadio.Text = player.appear_ratio;
                }
            }
        }

        private void btn_PlayerAdd_Click(object sender, EventArgs e)
        {
            string name = lstv_Player.Items[lstv_PlayerSltIndex].SubItems[1].Text;
            txt_PlayerListName.Text = name;
            Player_Str player;
            bool ret = m_PlayerCtrl.GetAttrById("role_" + name, out player);
            if(ret)
            {
                txt_PlayerListRadio.Text = player.appear_ratio;
            }
        }

        static int lstv_StageDefSltIndex = 0;
        private void btn_StageSrch_Click(object sender, EventArgs e)
        {
            ListViewItem foundItem = null;
            lstv_StageDef.Items[lstv_StageDefSltIndex].BackColor = Color.Transparent;
            int i = lstv_StageDefSltIndex;
            do
            {
                i++;
                if (i == lstv_StageDef.Items.Count)
                {
                    i = 0;
                }

                if (lstv_StageDef.Items[i].SubItems[1].Text.Contains(txt_StageSrch.Text))
                {
                    foundItem = lstv_StageDef.Items[i];
                    break;
                }
            } while (i != lstv_StageDefSltIndex);

            if (foundItem != null)
            {
                lstv_StageDef.Items[lstv_StageDefSltIndex].BackColor = Color.White;

                lstv_StageDef.TopItem = foundItem;  //定位到该项
                lstv_StageDef.Items[foundItem.Index].Focused = true;
                lstv_StageDef.Items[foundItem.Index].Selected = true;
                lstv_StageDef.Items[foundItem.Index].BackColor = Color.AliceBlue;

                lstv_StageDefSltIndex = foundItem.Index;
            } 
        }
        private void lstv_StageDef_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstv_StageDef.SelectedIndices != null && lstv_StageDef.SelectedIndices.Count > 0)
            {
                lstv_StageDef.Items[lstv_StageDefSltIndex].BackColor = Color.Transparent;
                lstv_StageDefSltIndex = lstv_StageDef.SelectedItems[0].Index;
                lstv_StageDef.Items[lstv_StageDefSltIndex].BackColor = Color.AliceBlue;

                string id = lstv_StageDef.Items[lstv_StageDefSltIndex].SubItems[0].Text;
                string name = lstv_StageDef.Items[lstv_StageDefSltIndex].SubItems[1].Text;

                txt_ArmyMapId.Text = id;
                txt_ArmyMapName.Text = name;
            }

        }

        private void btn_StageAdd_Click(object sender, EventArgs e)
        {
            string id = lstv_StageDef.Items[lstv_StageDefSltIndex].SubItems[0].Text;
            string name = lstv_StageDef.Items[lstv_StageDefSltIndex].SubItems[1].Text;

            txt_ArmyMapId.Text = id;
            txt_ArmyMapName.Text = name;
        }

        private void btn_ArmyListAdd_Click(object sender, EventArgs e)
        {
            //确保数据可用
            //

            string code = lstv_Army.Items[lstv_ArmySltIndex].Text;

            //修改army
            string player = "role_" + m_PlayerCtrl.NameSimpToTrad(txt_PlayerListName.Text);
            bool ret = m_ArmyCtrl.AddListPlayer(code, CFormat.ToTraditional(player));
            if (!ret)
            {
                MessageBox.Show("添加失败！（注意：不可存在相同role_xx）");
                return;
            }
            //刷新
            RefreshListV();
        }

        private void btn_ArmyListDel_Click(object sender, EventArgs e)
        {
            if (lstv_ArmyList.Items.Count <= 0)
            {
                return;
            }
            string code = lstv_Army.Items[lstv_ArmySltIndex].Text;
            //当前选择的
            string player = "role_" + lstv_ArmyList.Items[lstv_ArmyListSltIndex].SubItems[0].Text;
            //role
            bool ret = m_ArmyCtrl.DelListPlayer(code, CFormat.ToTraditional(player));
            if (!ret)
            {
                MessageBox.Show("删除失败！");
                return;
            }

            RefreshListV();
        }

        private void btn_ArmyMapAdd_Click(object sender, EventArgs e)
        {
            //合法数字
            string x = txt_ArmyMapX.Text;
            string y = txt_ArmyMapY.Text;
            if (!CFormat.IsNumber(x) || !CFormat.IsNumber(y))
            {
                MessageBox.Show("请输入正确的数字");
                return;
            }

            string code = lstv_Army.Items[lstv_ArmySltIndex].Text;
            //当前选择的
            Map_Str map;
            map.id = txt_ArmyMapId.Text;
            map.x = txt_ArmyMapX.Text;
            map.y = txt_ArmyMapY.Text;
            //role
            bool ret = m_ArmyCtrl.AddListMap(code, map);
            if (!ret)
            {
                MessageBox.Show("删除失败！");
                return;
            }

            RefreshListV();
        }

        private void btn_ArmyMapDel_Click(object sender, EventArgs e)
        {
            if (lstv_ArmyMap.Items.Count <= 0)
            {
                return;
            }
            string code = lstv_Army.Items[lstv_ArmySltIndex].Text;
            //当前选择的
            Map_Str map;
            string map_name = lstv_ArmyMap.Items[lstv_ArmyMapSltIndex].SubItems[0].Text;
            map.id = StageCtrl.GetIdByName(map_name);
            map.x = lstv_ArmyMap.Items[lstv_ArmyMapSltIndex].SubItems[1].Text;
            map.y = lstv_ArmyMap.Items[lstv_ArmyMapSltIndex].SubItems[2].Text;
            //role
            bool ret = m_ArmyCtrl.DelListMap(code, map);
            if (!ret)
            {
                MessageBox.Show("删除失败！");
                return;
            }

            RefreshListV();
        }

        static int lstv_ArmyNameSltIndex = 0;
        private void lstv_ArmyName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstv_ArmyName.SelectedIndices != null && lstv_ArmyName.SelectedIndices.Count > 0)
            {
                lstv_ArmyName.Items[lstv_ArmyNameSltIndex].BackColor = Color.Transparent;
                lstv_ArmyNameSltIndex = lstv_ArmyName.SelectedItems[0].Index;
                lstv_ArmyName.Items[lstv_ArmyNameSltIndex].BackColor = Color.AliceBlue;

                //get
                lstv_ArmyGrpName.Text = lstv_ArmyName.Items[lstv_ArmyNameSltIndex].SubItems[1].Text;
            }
        }

        private void btn_ArmyNameAllSet_Click(object sender, EventArgs e)
        {
            string name = lstv_ArmyName.Items[lstv_ArmyNameSltIndex].SubItems[0].Text;
            string delay = lstv_ArmyGrpDelay.Text;
            string range = lstv_ArmyGrpRange.Text;
            string exit = lstv_ArmyGrpExit.Text;

            if (CFormat.IsNumber(delay) && CFormat.IsNumber(range) && CFormat.IsNumber(exit))
            {
                m_ArmyCtrl.MdfyBaseInfoByName(name, delay, range, exit);

                RefreshListV();
                MessageBox.Show("修改成功！");
            }
            else 
            {
                MessageBox.Show("请输入正确的数值！");
                return;
            }
        }

        private void btn_ArmySingleMdfy_Click(object sender, EventArgs e)
        {
            string code = txt_ArmyCode.Text;
            string delay = txt_ArmyDelay.Text;
            string range = txt_ArmyRange.Text;
            string exit = txt_ArmyTime.Text;

            if (CFormat.IsNumber(delay) && CFormat.IsNumber(range) && CFormat.IsNumber(exit))
            {
                m_ArmyCtrl.MdfyBaseInfoByCode(code, delay, range, exit);

                RefreshListV();
            }
            else
            {
                MessageBox.Show("请输入正确的数值！");
                return;
            }
        }

        private void btn_SaveArmyFile_Click(object sender, EventArgs e)
        {
            bool ret = m_ArmyCtrl.SaveArmyInfo();
            if (!ret)
            {
                MessageBox.Show("保存文件失败！");
            }
            else
            {
                MessageBox.Show("保存文件成功！文件已经保存在new目录");
            }
        }
        #endregion

        #region  掉宝设置
        private void FillDropListV()
        { //lstv_DropList
            List<Player_Str> list = m_PlayerCtrl.GetPlayerList();
            List<Item_Str> listLetto = m_ItemCtrl.GetItemLettoList();
            lstv_DropList.Items.Clear();
            foreach (var item in list)
            {
                string name = CFormat.ToSimplified(CFormat.PureString(item.code));
                if (name.Contains("role_"))
                {
                    name = name.Split(new string[] { "role_" }, StringSplitOptions.RemoveEmptyEntries)[0];
                }
                string drop = CFormat.PureString(item.drop);
                string drop_mission = CFormat.PureString(item.drop_mission);

                if (drop != "" && drop != null)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = name;
                    lvi.SubItems.Add(drop);
                    lvi.SubItems.Add(drop_mission);
                    lvi.SubItems.Add("怪物");
                    lstv_DropList.Items.Add(lvi);
                }
                
            }

            foreach (var item in listLetto)
            {
                string name = CFormat.ToSimplified(CFormat.PureString(item.code));
                if (name.Contains("item_"))
                {
                    name = name.Split(new string[] { "item_" }, StringSplitOptions.RemoveEmptyEntries)[0];
                }
                string use_magic_id = CFormat.PureString(item.use_magic_id);

                if (use_magic_id != "" && use_magic_id != null)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = name;
                    lvi.SubItems.Add(use_magic_id);
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add("福袋");
                    lstv_DropList.Items.Add(lvi);
                }

            }

            lstv_DropList.EndUpdate();
        }
        private void FillItemListV()
        {
            List<ItemDef_Str> list = m_ItemCtrl.GetItemDefList();
            lstv_ItemList.Items.Clear();
            lstv_ModeItemList.Items.Clear();
            lstv_ShopItemList.Items.Clear();
            foreach (var item in list)
            {
                string id = CFormat.PureString(item.id);
                string name = CFormat.ToSimplified(CFormat.PureString(item.name));

                ListViewItem lvi = new ListViewItem();
                lvi.Text = id;
                lvi.SubItems.Add(name);
                lstv_ItemList.Items.Add(lvi);

                ListViewItem lvi2 = new ListViewItem();
                lvi2.Text = id;
                lvi2.SubItems.Add(name);
                lstv_ModeItemList.Items.Add(lvi2);

                ListViewItem lvi3 = new ListViewItem();
                lvi3.Text = id;
                lvi3.SubItems.Add(name);
                lstv_ShopItemList.Items.Add(lvi3);
            }
            lstv_ItemList.EndUpdate();
            lstv_ModeItemList.EndUpdate();
            lstv_ShopItemList.EndUpdate();
        }

        private void FillDropSingleInfo(Drop_Str dropList)
        {
            UInt32 totalNum = 0;
            UInt32 totalCount = 0;
            int topIndex = 0;
            if (lstv_DropItemList.TopItem != null)
            {
                topIndex = lstv_DropItemList.TopItem.Index;
            }
            
            lstv_DropItemList.Items.Clear();
            lstv_DropItemList.BeginUpdate();
            foreach (var item in dropList.list)
            {
                string num = CFormat.PureString(item.num);
                totalNum += UInt32.Parse(num);
                string name = CFormat.ToSimplified(CFormat.PureString(item.name));

                ListViewItem lvi = new ListViewItem();
                lvi.Text = name.Split(new string[] { "item_" }, StringSplitOptions.RemoveEmptyEntries)[0];
                lvi.SubItems.Add(num);
                lstv_DropItemList.Items.Add(lvi);
            }
            lstv_DropItemList.EndUpdate();

            totalCount = (UInt32)dropList.list.Count;

            txt_DropSingleId.Text = dropList.id;
            txt_DropSingleNum.Text = totalNum.ToString();
            txt_DropSingleItemCount.Text = totalCount.ToString();

            //lstv_DropItemList_SltIndex = 0;
            if (lstv_DropItemList.Items.Count > lstv_DropItemList_SltIndex)
            {
                lstv_DropItemList.Items[lstv_DropItemList_SltIndex].BackColor = Color.Transparent;
                lstv_DropItemList.Items[lstv_DropItemList_SltIndex].BackColor = Color.AliceBlue;
            }
            if (topIndex > 0 && topIndex < lstv_DropItemList.Items.Count)
            {
                //lstv_DropItemList.TopItem = lstv_DropList.Items[topIndex];
                lstv_DropItemList.Items[topIndex].EnsureVisible();
                lstv_DropItemList.TopItem = lstv_DropItemList.Items[topIndex];  //定位到该项
                lstv_DropItemList.Items[topIndex].Focused = true;
            }
            lvwColumnSorter.SortColumn = 1;  
            lstv_DropItemList.Sort();
        }

        private void FillDropPlayersInfo(List<Player_Str> list)
        {
            lstv_DropPlaersList.Items.Clear();
            foreach(var it in list)
            {
                string drop_id = CFormat.PureString(it.drop);
                string name = CFormat.ToSimplified(CFormat.PureString(it.code));
                if (name.Contains("role_"))
                {
                    name = name.Split(new string[] { "role_" }, StringSplitOptions.RemoveEmptyEntries)[0];
                }
                else
                {
                    name = "";
                }

                ListViewItem lvi = new ListViewItem();
                lvi.Text = drop_id;
                lvi.SubItems.Add(name);
                lstv_DropPlaersList.Items.Add(lvi);
                
            }
            lstv_DropPlaersList.EndUpdate();
        }

        private void DropRefreshListV()
        {
            ////刷新
            //lstv_DropItemList_SltIndex = 0;
            //lstv_DropItemList.TopItem = lstv_DropItemList.Items[lstv_DropItemList_SltIndex];
            //lstv_DropItemList.Items[lstv_DropItemList_SltIndex].Focused = true;
            //lstv_DropItemList.Items[lstv_DropItemList_SltIndex].Selected = true;
            ////get army attr
            //string item = lstv_DropItemList.Items[lstv_DropItemList_SltIndex].SubItems[0].Text;
            //string num = lstv_DropItemList.Items[lstv_DropItemList_SltIndex].SubItems[1].Text;
            //txt_DropSingleItemName.Text = item;
            //txt_DropSingleItemNum.Text = num;

            lstv_DropList.TopItem = lstv_DropList.Items[lstv_DropList_SltIndex]; //定位到该项
            lstv_DropList.Items[lstv_DropList_SltIndex].Focused = true;
            lstv_DropList.Items[lstv_DropList_SltIndex].Selected = true;
            lstv_DropList.Items[lstv_DropList_SltIndex].EnsureVisible(); 

            //get drop attr
            Drop_Str drop;
            string id = lstv_DropList.Items[lstv_DropList_SltIndex].SubItems[1].Text;
            bool ret = DropCtrl.GetDropAttrById(id, out drop);
            //if (ret)
            {
                FillDropSingleInfo(drop);
            }
            List<Player_Str> list = m_PlayerCtrl.GetPlayersByDrop(id);
            FillDropPlayersInfo(list);
        }

        static int lstv_DropList_SltIndex = 0;
        private void lstv_DropList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstv_DropList.SelectedIndices != null && lstv_DropList.SelectedIndices.Count > 0)
            {
                lstv_DropList.Items[lstv_DropList_SltIndex].BackColor = Color.Transparent;
                lstv_DropList_SltIndex = lstv_DropList.SelectedItems[0].Index;
                lstv_DropList.Items[lstv_DropList_SltIndex].BackColor = Color.AliceBlue;

                //get army attr
                Drop_Str drop;
                string id = lstv_DropList.Items[lstv_DropList_SltIndex].SubItems[1].Text;
                string type = lstv_DropList.Items[lstv_DropList_SltIndex].SubItems[3].Text;
                bool ret = DropCtrl.GetDropAttrById(id, out drop);
                //if (ret)
                {
                    FillDropSingleInfo(drop);
                }

                if (type == "怪物")
                {
                    List<Player_Str> list = m_PlayerCtrl.GetPlayersByDrop(id);
                    FillDropPlayersInfo(list);
                }
            }
        }

        private void btn_DropSrch_Click(object sender, EventArgs e)
        {
            try
            {
                ListViewItem foundItem = null;
                lstv_DropList.Items[lstv_DropList_SltIndex].BackColor = Color.Transparent;
                int i = lstv_DropList_SltIndex;
                do
                {
                    //name
                    if (rdb_DropSrchGrpName.Checked)
                    {
                        i++;
                        if (i == lstv_DropList.Items.Count)
                        {
                            i = 0;
                        }
                        if (lstv_DropList.Items[i].SubItems[0].Text.Contains(txt_DropSrch.Text))
                        {
                            foundItem = lstv_DropList.Items[i];
                            break;
                        }
                    }
                    else if (rdb_DropSrchGrpId.Checked)//id
                    {
                        i++;
                        if (i == lstv_DropList.Items.Count)
                        {
                            i = 0;
                        }
                        if (lstv_DropList.Items[i].SubItems[1].Text == txt_DropSrch.Text)
                        {
                            foundItem = lstv_DropList.Items[i];
                            break;
                        }
                    }
                } while (i != lstv_DropList_SltIndex);

                if (foundItem != null)
                {
                    lstv_DropList.Items[lstv_DropList_SltIndex].BackColor = Color.White;

                    lstv_DropList.TopItem = foundItem;  //定位到该项
                    lstv_DropList.Items[foundItem.Index].Focused = true;
                    lstv_DropList.Items[foundItem.Index].Selected = true;
                    lstv_DropList.Items[foundItem.Index].BackColor = Color.AliceBlue;

                    lstv_DropList_SltIndex = foundItem.Index;

                    DropRefreshListV();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static int lstv_DropItemList_SltIndex = 1;
        private void lstv_DropItemList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstv_DropItemList.SelectedIndices != null && lstv_DropItemList.SelectedIndices.Count > 0)
            {
                //lstv_DropItemList.Items[lstv_DropItemList_SltIndex].BackColor = Color.Transparent;
                if (lstv_DropItemList.Items.Count > lstv_DropItemList_SltIndex)
                {
                    lstv_DropItemList.Items[lstv_DropItemList_SltIndex].BackColor = Color.Transparent;
                }
                
                lstv_DropItemList_SltIndex = lstv_DropItemList.SelectedItems[0].Index;
                lstv_DropItemList.Items[lstv_DropItemList_SltIndex].BackColor = Color.AliceBlue;

                //get army attr
                string item = lstv_DropItemList.Items[lstv_DropItemList_SltIndex].SubItems[0].Text;
                string num = lstv_DropItemList.Items[lstv_DropItemList_SltIndex].SubItems[1].Text;

                txt_DropSingleItemName.Text = item;
                txt_DropSingleItemNum.Text = num;
            }
        }

        private void btn_DropSingleNumMdfy_Click(object sender, EventArgs e)
        {
            //信息
            string dropId = lstv_DropList.Items[lstv_DropList_SltIndex].SubItems[1].Text;
            string name = lstv_DropItemList.Items[lstv_DropItemList_SltIndex].SubItems[0].Text;
            if (name != "Money")
            {
                name = m_ItemCtrl.NameSimpToTrad(name);
            }
            
            name = "item_" + name;
            string oldNum = lstv_DropItemList.Items[lstv_DropItemList_SltIndex].SubItems[1].Text;
            string newNum = txt_DropSingleItemNum.Text;

            bool ret = DropCtrl.MdfyDropNum(dropId, name, oldNum, newNum);
            if (!ret)
            {
                MessageBox.Show("修改爆率失败！");
            }
            else //刷新
            {
                DropRefreshListV();
            }
        }

        private void btn_DropSingleItemAdd_Click(object sender, EventArgs e)
        {
            //信息
            string dropId = lstv_DropList.Items[lstv_DropList_SltIndex].SubItems[1].Text;
            string name = m_ItemCtrl.NameSimpToTrad(txt_DropSingleItemName.Text);
            string num = txt_DropSingleItemNum.Text;

            if (name == "" || !CFormat.IsNumber(num))
            {
                MessageBox.Show("错误，选择正确的物品名和输入可用数值！");
                return;
            }
            name = m_ItemCtrl.NameSimpToTrad(name);
            name = "item_" + name;

            bool ret = DropCtrl.AddSingleDrop(dropId, name, num);
            if (!ret)
            {
                MessageBox.Show("添加失败！");
            }
            else //刷新
            {
                //DropRefreshListV();
                string _num = CFormat.PureString(num);
                string _name = CFormat.ToSimplified(CFormat.PureString(txt_DropSingleItemName.Text));

                ListViewItem lvi = new ListViewItem();
                lvi.Text = _name;
                lvi.SubItems.Add(_num);
                lstv_DropItemList.Items.Add(lvi);
                txt_DropSingleNum.Text = "" + (UInt32.Parse(_num) + UInt32.Parse(txt_DropSingleNum.Text));

                txt_DropSingleItemCount.Text = lstv_DropItemList.Items.Count.ToString();

                //重排序
                lvwColumnSorter.SortColumn = 1;
                lvwColumnSorter.Order = SortOrder.Descending;
                this.lstv_DropItemList.Sort();
            }
        }

        private void btn_DropSingleItemDel_Click(object sender, EventArgs e)
        {
            if (lstv_DropItemList_SltIndex >= lstv_DropItemList.Items.Count)
            {
                return;
            }
            //信息
            string dropId = lstv_DropList.Items[lstv_DropList_SltIndex].SubItems[1].Text;
            string name = lstv_DropItemList.Items[lstv_DropItemList_SltIndex].SubItems[0].Text;
            string num = lstv_DropItemList.Items[lstv_DropItemList_SltIndex].SubItems[1].Text;

            if (name == "" || !CFormat.IsNumber(num))
            {
                MessageBox.Show("错误，选择要删除的物品！");
                return;
            }

            name = m_ItemCtrl.NameSimpToTrad(name);
            name = "item_" + name;

            bool ret = DropCtrl.DelSingleDrop(dropId, name, num);
            if (!ret)
            {
                MessageBox.Show("删除失败！");
            }
            else //刷新
            {
                //DropRefreshListV();
                lstv_DropItemList.Items.Remove(lstv_DropItemList.Items[lstv_DropItemList_SltIndex]);
                txt_DropSingleNum.Text = "" + (UInt32.Parse(txt_DropSingleNum.Text) - UInt32.Parse(num));
                txt_DropSingleItemCount.Text = lstv_DropItemList.Items.Count.ToString();
                //重排序
                lvwColumnSorter.SortColumn = 1;
                lvwColumnSorter.Order = SortOrder.Descending;
                this.lstv_DropItemList.Sort();
            }
        }

        private static int lstv_ItemList_SltIndex = 0;
        private void lstv_ItemList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstv_ItemList.SelectedIndices != null && lstv_ItemList.SelectedIndices.Count > 0)
            {
                //lstv_DropItemList.Items[lstv_DropItemList_SltIndex].BackColor = Color.Transparent;
                lstv_ItemList.Items[lstv_ItemList_SltIndex].BackColor = Color.Transparent;
                lstv_ItemList_SltIndex = lstv_ItemList.SelectedItems[0].Index;
                lstv_ItemList.Items[lstv_ItemList_SltIndex].BackColor = Color.AliceBlue;

                txt_DropSingleItemName.Text = lstv_ItemList.Items[lstv_ItemList_SltIndex].SubItems[1].Text;
            }
        }

        private void btn_ItemListSrch_Click(object sender, EventArgs e)
        {
            ListViewItem foundItem = null;
            lstv_ItemList.Items[lstv_ItemList_SltIndex].BackColor = Color.Transparent;
            int i = lstv_ItemList_SltIndex;
            do
            {
                i++;
                if (i == lstv_ItemList.Items.Count)
                {
                    i = 0;
                }

                if (lstv_ItemList.Items[i].SubItems[1].Text.Contains(txt_ItemListSrch.Text))
                {
                    foundItem = lstv_ItemList.Items[i];
                    break;
                }
            } while (i != lstv_ItemList_SltIndex);

            if (foundItem != null)
            {
                lstv_ItemList.Items[lstv_ItemList_SltIndex].BackColor = Color.White;

                lstv_ItemList.TopItem = foundItem;  //定位到该项
                lstv_ItemList.Items[foundItem.Index].Focused = true;
                lstv_ItemList.Items[foundItem.Index].Selected = true;
                lstv_ItemList.Items[foundItem.Index].BackColor = Color.AliceBlue;

                lstv_ItemList_SltIndex = foundItem.Index;
                txt_DropSingleItemName.Text = lstv_ItemList.Items[lstv_ItemList_SltIndex].SubItems[1].Text;
            } 
        }

        private void btn_ItemSendTo_Click(object sender, EventArgs e)
        {
            string name = lstv_ItemList.Items[lstv_ItemList_SltIndex].SubItems[1].Text;
            if (name != "" && name != "")
            {
                txt_DropSingleItemName.Text = name;
            }
        }

        private void btn_DropReLoad_Click(object sender, EventArgs e)
        {
            bool ret = DropCtrl.LoadDropListInfo();
            if (!ret)
            {
                MessageBox.Show("LoadDropListInfo error!");
                return;
            }
            lstv_DropList_SltIndex = 0;
            FillDropListV();

            DropRefreshListV();
        }

        private void btn_DropSaveFile_Click(object sender, EventArgs e)
        {
            bool ret = DropCtrl.SaveDropInfo();
            if (!ret)
            {
                MessageBox.Show("保存爆率文件失败！");
            }
            else
            {
                MessageBox.Show("保存爆率文件成功！文件已经保存在new目录");
            }
        }

        private void btn_DropExportFile_Click(object sender, EventArgs e)
        {
            string file = "Boss掉宝资料.txt";
            //写文件
            FileStream rtfs = new FileStream(file, FileMode.Create, FileAccess.Write);
            StreamWriter rt = new StreamWriter(rtfs, Encoding.Default);
            rt.BaseStream.Seek(0, SeekOrigin.Begin);
            rt.BaseStream.Position = 0;

            //遍历DropList
            for (int i = 0; i < lstv_DropList.Items.Count; i++) 
            {
                string name = lstv_DropList.Items[i].SubItems[0].Text;
                string id = lstv_DropList.Items[i].SubItems[1].Text;

                Drop_Str drop;
                bool ret = DropCtrl.GetDropAttrById(id, out drop);
                if (drop.list.Count > 0)
                {
                    string items = "";
                    foreach (var item in drop.list)
                    {
                        string itemName = item.name;
                        if (itemName.Contains("item_"))
                        {
                            itemName = itemName.Split(new string[] { "item_" }, StringSplitOptions.RemoveEmptyEntries)[0];
                            if (itemName == "Nothing")
                            {
                                itemName = "";
                            }
                        }
                        else 
                        {
                            itemName = "";
                        }

                        if (itemName != "")
                        {
                            items += itemName + ";";
                        }
                        
                    }
                    if (items != "")
                    {
                        name = "【" + name + "】";
                        if (cbx_DropExportSimple.Checked)
                        {
                            name = CFormat.ToSimplified(name);
                            items = CFormat.ToSimplified(items);
                        }
                        rt.WriteLine("");
                        rt.WriteLine(name);
                        rt.WriteLine(items);
                    }
                }
            }

            rt.Close();
            rtfs.Close();

            MessageBox.Show("输出掉宝资料【Boss掉宝资料.txt】成功！");
        }

        

        private void btn_DropClearLtlGw_Click(object sender, EventArgs e)
        {
            //获取所有存在掉宝的小怪
            string msg = "";
            if (rdb_ClearLtlGwDrop.Checked)
            {
                msg = "当前使用修改Drop文件方式删除，若有小怪与BOSS使用相同drop id，将造成BOSS无掉落，是否决定此方式删除？(推荐使用修改Drop)";
            }
            else 
            {
                msg = "当前使用修改Player文件方式删除，一旦删除，工具后续将无法读取到改小怪的掉宝信息，是否决定此方式删除？(推荐使用修改Drop)";
            }
            DialogResult dr = MessageBox.Show(msg, "提示", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                //用户选择确认的操作
                //MessageBox.Show("您选择的是【确认】");
            }
            else if (dr == DialogResult.Cancel)
            {
                //用户选择取消的操作
                return;
            }
            //遍历DropList
            for (int i = 0; i < lstv_DropList.Items.Count; i++)
            {
                string name = lstv_DropList.Items[i].SubItems[0].Text;
                string player_id = "role_" + name;
                string drop_id = lstv_DropList.Items[i].SubItems[1].Text;

                Player_Str player;
                bool ret = m_PlayerCtrl.GetAttrById(player_id, out player);
                if (ret && player.appear_ratio == "") //小怪是没有appear_ratio属性的
                {
                    bool del = true;
                    //条件
                    Drop_Str drop;
                    ret = DropCtrl.GetDropAttrById(drop_id, out drop);
                    if (ret)
                    {
                        foreach (var it in drop.list)
                        {
                            if (String.Compare(CFormat.ToSimplified(it.name), "item_树皮", true) == 0 && cbx_DropRsvSP.Checked)
                            { //保留树皮
                                del = false;
                                break;
                            }
                            else if (cbx_DropRsvBF.Checked)
                            {
                                if (CFormat.ToSimplified(it.name).Contains("转职令") || CFormat.ToSimplified(it.name).Contains("晋升令"))
                                {
                                    del = false;
                                    break;
                                }
                            }
                        }
                    }

                    if (del)
                    {
                        //删除这些掉宝内容 -- 会造成个别跟普通小怪一样掉宝ID的BOSS没有掉落
                        if (rdb_ClearLtlGwDrop.Checked)
                        {
                            DropCtrl.ClearSingleDrops(drop_id);
                        }
                        else
                        {
                            //应该是清除Player的drop  -- 会造成后续不好添加原有boss的掉落修改
                            m_PlayerCtrl.ClearPlayerDrop(player);
                        }
                    }
                }
            }

            MessageBox.Show("清理成功！");
            DropRefreshListV();
        }
        #endregion

        #region  商城设置
        static int lstv_ModeItemList_SltIndex = 0;
        private void lstv_ModeItemList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstv_ModeItemList.SelectedIndices != null && lstv_ModeItemList.SelectedIndices.Count > 0)
            {
                //lstv_DropItemList.Items[lstv_DropItemList_SltIndex].BackColor = Color.Transparent;
                lstv_ModeItemList.Items[lstv_ModeItemList_SltIndex].BackColor = Color.Transparent;
                lstv_ModeItemList_SltIndex = lstv_ModeItemList.SelectedItems[0].Index;
                lstv_ModeItemList.Items[lstv_ModeItemList_SltIndex].BackColor = Color.AliceBlue;

                txt_ModeItemName.Text = lstv_ModeItemList.Items[lstv_ModeItemList_SltIndex].SubItems[1].Text;
            }
        }

        private void btn_ModeItemSrch_Click(object sender, EventArgs e)
        {
            ListViewItem foundItem = null;
            lstv_ModeItemList.Items[lstv_ModeItemList_SltIndex].BackColor = Color.Transparent;
            int i = lstv_ModeItemList_SltIndex;
            do
            {
                i++;
                if (i == lstv_ModeItemList.Items.Count)
                {
                    i = 0;
                }

                if (lstv_ModeItemList.Items[i].SubItems[1].Text.Contains(txt_ModeItemSrch.Text))
                {
                    foundItem = lstv_ModeItemList.Items[i];
                    break;
                }
            } while (i != lstv_ModeItemList_SltIndex);

            if (foundItem != null)
            {
                lstv_ModeItemList.Items[lstv_ModeItemList_SltIndex].BackColor = Color.White;

                lstv_ModeItemList.TopItem = foundItem;  //定位到该项
                lstv_ModeItemList.Items[foundItem.Index].Focused = true;
                lstv_ModeItemList.Items[foundItem.Index].Selected = true;
                lstv_ModeItemList.Items[foundItem.Index].BackColor = Color.AliceBlue;

                lstv_ModeItemList_SltIndex = foundItem.Index;
                txt_ModeItemName.Text = lstv_ModeItemList.Items[lstv_ModeItemList_SltIndex].SubItems[1].Text;

            } 
        }
        private void txt_ModeItemSend_Click(object sender, EventArgs e)
        {
            //当前选定的item
            string name = lstv_ModeItemList.Items[lstv_ModeItemList_SltIndex].SubItems[1].Text;
            string id = lstv_ModeItemList.Items[lstv_ModeItemList_SltIndex].SubItems[0].Text;

            txt_ModeItemName.Text = name;
        }

        ItemModeSinglePanel.SingePanel[] singePanel1 = new ItemModeSinglePanel.SingePanel[10];
        private void OnCreateItemModePanels()
        {
            for (int i = 0; i < 10; i++)
            {
                singePanel1[i] = new ItemModeSinglePanel.SingePanel();
                singePanel1[i].Code = "";
                singePanel1[i].Cost = "";
                singePanel1[i].Item_Attr = "";
                singePanel1[i].Item_Id = "";
                singePanel1[i].Item_Number = "";
                singePanel1[i].Type = "";
                int x = i % 2 * 325 + 14;
                int y = i / 2 * 65 + 19;
                singePanel1[i].Location = new System.Drawing.Point(x, y);
                singePanel1[i].Margin = new System.Windows.Forms.Padding(0);
                singePanel1[i].Name = "singePanel1" + i;
                singePanel1[i].PBackColor = System.Drawing.Color.Empty;
                singePanel1[i].PForeColor = System.Drawing.Color.Empty;
                singePanel1[i].Size = new System.Drawing.Size(325, 65);
                singePanel1[i].TabIndex = 3;
                singePanel1[i].Click += new System.EventHandler(singePanel_Click);
            }

            //btn 事件
            btn_ModePageFwd.Click += new System.EventHandler(btn_ModePageFwd_Click);
            btn_ModePageNext.Click += new System.EventHandler(btn_ModePageNext_Click);
        }
        
        private void OnLoadItemModePanels(int tabNum,int page)
        {
            if (!m_Active) return;
            List<Item_Mode_Str> modeList = ItemModeCtrl.GetItemModeList();

            for (int num = 0; num < 10; num++ )
            {
                //先清除
                singePanel1[num].Code = "";
                singePanel1[num].Type = "";
                singePanel1[num].Item_Id = "";
                singePanel1[num].Item_Number = "";
                singePanel1[num].Cost = "";
                singePanel1[num].Item_Attr = "";
            }
            for (int i = 0, j=0, n=0; i < modeList.Count; i++)
            {
                //
                if (modeList[i].item_id != "")
                {
                    string type = "";
                    if (modeList[i].type == "ishopType_General" && tabNum == 1)
                    {
                        type = "一般";
                    }
                    else if (modeList[i].type == "ishopType_Special" && tabNum == 2)
                    {
                        type = "特殊";
                    }
                    else if (modeList[i].type == "ishopType_Funcion" && tabNum == 3)
                    {
                        type = "功能";
                    }
                    else if (modeList[i].type == "ishopType_Cheap" && tabNum == 4)
                    {
                        type = "特卖";
                    }
                    else 
                    {
                        continue;
                    }

                    if(type !="")
                    {
                        n++;
                        if (n <= (page-1) * 10)
                        {
                            continue;
                        }
                    }
                    singePanel1[j].Code = modeList[i].code;
                    singePanel1[j].Type = type;
                    string name = modeList[i].item_id.Split(new string[] { "item_" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    name = CFormat.ToSimplified(name);
                    singePanel1[j].Item_Id = name;
                    singePanel1[j].Item_Number = modeList[i].item_number;
                    singePanel1[j].Cost = modeList[i].cost;

                    string attr = "";
                    if (modeList[i].item_attr == "ITEMMALL_ATTR_NEW")
                    {
                        attr = "NEW";
                    }
                    else if (modeList[i].item_attr == "ITEMMALL_ATTR_SUGGEST")
                    {
                        attr = "HOT";
                    }
                    singePanel1[j].Item_Attr = attr;

                    //控件放在哪里
                    if (tabNum == 1)
                    {
                        this.tbpg_Mode1.Controls.Add(this.btn_ModePageNext);
                        this.tbpg_Mode1.Controls.Add(this.btn_ModePageFwd);
                        this.tbpg_Mode1.Controls.Add(this.txt_ModePage);
                        this.tbpg_Mode1.Controls.Add(singePanel1[j]);
                    }
                    else if (tabNum == 2)
                    {
                        this.tbpg_Mode2.Controls.Add(this.btn_ModePageNext);
                        this.tbpg_Mode2.Controls.Add(this.btn_ModePageFwd);
                        this.tbpg_Mode2.Controls.Add(this.txt_ModePage);
                        this.tbpg_Mode2.Controls.Add(singePanel1[j]);
                    }
                    else if (tabNum == 3)
                    {
                        this.tbpg_Mode3.Controls.Add(this.btn_ModePageNext);
                        this.tbpg_Mode3.Controls.Add(this.btn_ModePageFwd);
                        this.tbpg_Mode3.Controls.Add(this.txt_ModePage);
                        this.tbpg_Mode3.Controls.Add(singePanel1[j]);
                    }
                    else if (tabNum == 4)
                    {
                        this.tbpg_Mode4.Controls.Add(this.btn_ModePageNext);
                        this.tbpg_Mode4.Controls.Add(this.btn_ModePageFwd);
                        this.tbpg_Mode4.Controls.Add(this.txt_ModePage);
                        this.tbpg_Mode4.Controls.Add(singePanel1[j]);
                    }

                    j++;
                    if (j >= 10)
                    {
                        break;
                    }
                }
                
            }

            //选择
            singePanel_Click(singePanel1[singePanel_SltIndex],null);
        }
        private static int singePanel_SltIndex = 0;
        private void singePanel_Click(object sender, EventArgs e)
        {
            ItemModeSinglePanel.SingePanel sp = (ItemModeSinglePanel.SingePanel)sender;
            string code = sp.Code;
            string type = sp.Type;
            string item_id = sp.Item_Id;
            string item_number = sp.Item_Number;
            string cost = sp.Cost;
            string item_attr = sp.Item_Attr;

            txt_ModeItemCode.Text = code;
            txt_ModeItemName.Text = item_id;
            txt_ModeItemNum.Text = item_number;
            txt_ModeItemCost.Text = cost;
            cbx_ModeItemType.Text = type;
            cbx_ModeItemAttr.Text = item_attr == "" ? "无" : item_attr;

            for (int num = 0; num < 10; num++)
            {
                //先清除
                singePanel1[num].PBackColor = Color.Transparent;
                if (singePanel1[num].Name == sp.Name)
                {
                    singePanel_SltIndex = num;
                }
            }
            sp.PBackColor = Color.LightCoral;
        }

        private void btn_ModePageFwd_Click(object sender, EventArgs e)
        {
            Int32 page = Int32.Parse(txt_ModePage.Text);
            if (page <= 1)
            {
                return;
            }
            else 
            {
                page--;
                txt_ModePage.Text = page.ToString();
            }

            OnLoadItemModePanels(tabControl2.SelectedIndex + 1, page);
        }

        private void btn_ModePageNext_Click(object sender, EventArgs e)
        {
            //get cur tab max page
            Int32 maxNum = ItemModeCtrl.GetModeTypeNum(tabControl2.SelectedIndex + 1);
            Int32 maxPage = maxNum / 10; 
            if(maxNum % 10 > 0)
            {
                maxPage += 1;
            }
            Int32 page = Int32.Parse(txt_ModePage.Text);
            if (page >= maxPage)
            {
                return;
            }
            else
            {
                page++;
                txt_ModePage.Text = page.ToString();
            }

            OnLoadItemModePanels(tabControl2.SelectedIndex + 1, page);
        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            txt_ModePage.Text = "1";
            OnLoadItemModePanels(tabControl2.SelectedIndex + 1, 1);
        }

        private void btn_ModeSingleMdfy_Click(object sender, EventArgs e)
        {
            //code
            if (txt_ModeItemCode.Text == "" || txt_ModeItemCode.Text == null)
            {
                MessageBox.Show("请选择需要修改的物品！");
                return;
            }
            if (!CFormat.IsNumber(txt_ModeItemNum.Text) || !CFormat.IsNumber(txt_ModeItemCost.Text))
            {
                MessageBox.Show("请选输入正确的数值！");
                return;
            }

            string code = txt_ModeItemCode.Text;

            Item_Mode_Str newMode;
            newMode.code = code;
            string name = m_ItemCtrl.NameSimpToTrad(CFormat.PureString(txt_ModeItemName.Text));
            name = "item_" + name;
            newMode.item_id = name;
            newMode.item_number = CFormat.PureString(txt_ModeItemNum.Text);
            newMode.cost = CFormat.PureString(txt_ModeItemCost.Text);
            newMode.type = ModeTypeFormat(cbx_ModeItemType.SelectedIndex);
            newMode.item_attr = ModeAttrTyoeFormat(cbx_ModeItemAttr.SelectedIndex);
            bool ret = ItemModeCtrl.MdfyItemModeInfo(code, newMode);
            if (!ret)
            {
                MessageBox.Show("修改失败！");
            }
            else
            {
                Int32 page = Int32.Parse(txt_ModePage.Text);

                OnLoadItemModePanels(tabControl2.SelectedIndex + 1, page);
            }
        }
        private void btn_ModeSingleDel_Click(object sender, EventArgs e)
        {
            if (txt_ModeItemCode.Text == "" || txt_ModeItemCode.Text == null
                || txt_ModeItemName.Text == "" || txt_ModeItemName.Text == null)
            {
                MessageBox.Show("请选择需要修改的物品！");
                return;
            }
            string code = txt_ModeItemCode.Text;

            bool ret = ItemModeCtrl.DelItemModeInfo(code);
            if (!ret)
            {
                MessageBox.Show("删除失败！");
            }
            else
            {
                Int32 page = Int32.Parse(txt_ModePage.Text);

                OnLoadItemModePanels(tabControl2.SelectedIndex + 1, page);
            }
        }


        private void btn_ModeSingleAdd_Click(object sender, EventArgs e)
        {
            if (txt_ModeItemCode.Text == "" || txt_ModeItemCode.Text == null)
            {
                MessageBox.Show("请选择需要修改的物品！");
                return;
            }
            if (!CFormat.IsNumber(txt_ModeItemNum.Text) || !CFormat.IsNumber(txt_ModeItemCost.Text))
            {
                MessageBox.Show("请选输入正确的数值！");
                return;
            }

            string code = txt_ModeItemCode.Text;
            string name = m_ItemCtrl.NameSimpToTrad(txt_ModeItemName.Text);

            Item_Mode_Str newMode;
            newMode.code = code;
            newMode.item_id = CFormat.PureString("item_" + name);
            newMode.item_number = CFormat.PureString(txt_ModeItemNum.Text);
            newMode.cost = CFormat.PureString(txt_ModeItemCost.Text);
            newMode.type = ModeTypeFormat(cbx_ModeItemType.SelectedIndex);
            newMode.item_attr = ModeAttrTyoeFormat(cbx_ModeItemAttr.SelectedIndex);

            bool ret = ItemModeCtrl.AddTailItemModeInfo(code, newMode);
            if (!ret)
            {
                MessageBox.Show("添加失败！");
            }
            else
            {
                Int32 page = Int32.Parse(txt_ModePage.Text);

                OnLoadItemModePanels(tabControl2.SelectedIndex + 1, page);
            }
        }

        private string ModeTypeFormat(int index)
        {
            string type = "";

            switch (index)
            {
                case 0:
                    type = "ishopType_General";
                    break;
                case 1:
                    type = "ishopType_Special";
                    break;
                case 2:
                    type = "ishopType_Funcion";
                    break;
                case 3:
                    type = "ishopType_Cheap";
                    break;
                default:
                    break;
            }

            return type;
        }

        private int ModeTypeFormat(string type)
        {
            int index = -1;
            switch (type)
            {
                case "ishopType_General":
                    index = 0;
                    break;
                case "ishopType_Special":
                    index = 1;
                    break;
                case "ishopType_Funcion":
                    index = 2;
                    break;
                case "ishopType_Cheap":
                    index = 3;
                    break;
                default:
                    break;
            }
            return index;
        }

        private string ModeAttrTyoeFormat(int index)
        {
            string type = "";

            switch (index)
            {
                case 0:
                    type = "";
                    break;
                case 1:
                    type = "ITEMMALL_ATTR_SUGGEST";
                    break;
                case 2:
                    type = "ITEMMALL_ATTR_NEW";
                    break;
                default:
                    break;
            }

            return type;
        }

        private int ModeAttrTyoeFormat(string type)
        {
            int index = 0;
            switch (type)
            {
                case "ITEMMALL_ATTR_SUGGEST":
                    index = 1;
                    break;
                case "ITEMMALL_ATTR_NEW":
                    index = 2;
                    break;
                default:
                    index = 0;
                    break;
            }
            return index;
        }

        private void btn_ModeSingleFwd_Click(object sender, EventArgs e)
        {
            if (txt_ModeItemCode.Text == "" || txt_ModeItemCode.Text == null)
            {
                MessageBox.Show("请选择需要修改的物品！");
                return;
            }

            string code = txt_ModeItemCode.Text;
            bool ret = ItemModeCtrl.ModeSendFwd(code);
            if (ret)
            {
                Int32 page = Int32.Parse(txt_ModePage.Text);
                singePanel_SltIndex--;
                if (singePanel_SltIndex < 0)
                {
                    singePanel_SltIndex = 9;
                    page--;
                    txt_ModePage.Text = page.ToString();
                }
                OnLoadItemModePanels(tabControl2.SelectedIndex + 1, page);
            }
        }

        private void btn_ModeSingleNext_Click(object sender, EventArgs e)
        {
            if (txt_ModeItemCode.Text == "" || txt_ModeItemCode.Text == null)
            {
                MessageBox.Show("请选择需要修改的物品！");
                return;
            }

            string code = txt_ModeItemCode.Text;
            bool ret = ItemModeCtrl.ModeSendNext(code);
            if (ret)
            {
                Int32 page = Int32.Parse(txt_ModePage.Text);
                singePanel_SltIndex++;
                if (singePanel_SltIndex >= 10)
                {
                    singePanel_SltIndex = 0;
                    page++;
                    txt_ModePage.Text = page.ToString();
                }
                OnLoadItemModePanels(tabControl2.SelectedIndex + 1, page);
            }
        }

        private void btn_ModeItemSaveFile_Click(object sender, EventArgs e)
        {
            bool ret = ItemModeCtrl.SaveItemModeInfo();
            if (!ret)
            {
                MessageBox.Show("保存文件失败！");
            }
            MessageBox.Show("保存文件成功！文件已经保存在new目录");
        }

        private void btn_ReLoadModeItem_Click(object sender, EventArgs e)
        {
            bool ret = ItemModeCtrl.LoadItemModeInfo();
            if (!ret)
            {
                MessageBox.Show("LoadItemModeInfo error!");
                return;
            }
            txt_ModePage.Text = "1";
            OnLoadItemModePanels(tabControl2.SelectedIndex + 1, 1);
        }
        #endregion

        #region 配置管理
        void InitConf()
        {
            m_ConfIni = System.AppDomain.CurrentDomain.BaseDirectory + "配置文件.ini";
            //CIniCtrl.ReadIniData("Server", "ServerIP", "", serverIni);
            //CIniCtrl.WriteIniData("Config", "loginVersion", txt_loginVer.Text, serverIni);
            try
            {
                string confName = "";
                string confDetil = "";
                int index = 1;
                
                string confNameLine = "";
                string confDetilLine = "";
                lstv_DropConf.Items.Clear();
                lstv_DropConf.BeginUpdate();
                do
                {
                    confName = "ConfName" + index;
                    confDetil = "ConfDetil" + index;

                    confNameLine = CIniCtrl.ReadIniData("Configs", confName, "", m_ConfIni);
                    confDetilLine = CIniCtrl.ReadIniData("Configs", confDetil, "", m_ConfIni);
                    if (confNameLine != string.Empty && confDetilLine != string.Empty)
                    {
                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = confNameLine;
                        lvi.SubItems.Add(confDetilLine);
                        lstv_DropConf.Items.Add(lvi);
                    }
                    index++;
                //} while (confNameLine != "" && confDetilLine != "");
                } while (confNameLine != string.Empty && confDetilLine != string.Empty);
                lstv_DropConf.EndUpdate();

            }catch(Exception e)
            {
            
            }
            
        }
        #endregion

        private void FillShopListV()
        {

            List<ShopDef_Str> list = ShopCtrl.GetShopDefList();
            lstv_ShopList.Items.Clear();
            foreach (var item in list)
            {
                string id = CFormat.PureString(item.id);
                string name = CFormat.ToSimplified(CFormat.PureString(item.name));

                ListViewItem lvi = new ListViewItem();
                lvi.Text = id;
                lvi.SubItems.Add(name);
                lstv_ShopList.Items.Add(lvi);
            }
            lstv_ShopList.EndUpdate();
        }
        static int lstv_ShopItemList_SltIndex = 0;
        private void lstv_ShopItemList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstv_ShopItemList.SelectedIndices != null && lstv_ShopItemList.SelectedIndices.Count > 0)
            {
                //lstv_DropItemList.Items[lstv_DropItemList_SltIndex].BackColor = Color.Transparent;
                lstv_ShopItemList.Items[lstv_ShopItemList_SltIndex].BackColor = Color.Transparent;
                lstv_ShopItemList_SltIndex = lstv_ShopItemList.SelectedItems[0].Index;
                lstv_ShopItemList.Items[lstv_ShopItemList_SltIndex].BackColor = Color.AliceBlue;

                txt_ShopItemName.Text = lstv_ShopItemList.Items[lstv_ShopItemList_SltIndex].SubItems[1].Text;
                Item_Str item;
                if (m_ItemCtrl.GetAttrById("item_" + txt_ShopItemName.Text, out item))
                {
                    txt_ShopItemCost.Text = item.cost;
                    txt_ShopItemSell.Text = item.sell;
                }
            }
        }

        private void btn_ShopItemSrch_Click(object sender, EventArgs e)
        {
            ListViewItem foundItem = null;
            lstv_ShopItemList.Items[lstv_ShopItemList_SltIndex].BackColor = Color.Transparent;
            int i = lstv_ShopItemList_SltIndex;
            do
            {
                i++;
                if (i == lstv_ShopItemList.Items.Count)
                {
                    i = 0;
                }

                if (lstv_ShopItemList.Items[i].SubItems[1].Text.Contains(txt_ShopItemSrch.Text))
                {
                    foundItem = lstv_ShopItemList.Items[i];
                    break;
                }
            } while (i != lstv_ShopItemList_SltIndex);

            if (foundItem != null)
            {
                lstv_ShopItemList.Items[lstv_ShopItemList_SltIndex].BackColor = Color.White;

                lstv_ShopItemList.TopItem = foundItem;  //定位到该项
                lstv_ShopItemList.Items[foundItem.Index].Focused = true;
                lstv_ShopItemList.Items[foundItem.Index].Selected = true;
                lstv_ShopItemList.Items[foundItem.Index].BackColor = Color.AliceBlue;

                lstv_ShopItemList_SltIndex = foundItem.Index;
                txt_ShopItemName.Text = lstv_ShopItemList.Items[lstv_ShopItemList_SltIndex].SubItems[1].Text;
            } 
        }

        private void btn_ShopItemSend_Click(object sender, EventArgs e)
        {
            string name = lstv_ShopItemList.Items[lstv_ShopItemList_SltIndex].SubItems[1].Text;
            string itemName = "item_" + name;

            Item_Str item;
            bool ret = m_ItemCtrl.GetAttrById(itemName, out item);
            if (ret)
            {
                txt_ShopItemName.Text = name;
                txt_ShopItemCost.Text = item.cost;
                txt_ShopItemSell.Text = item.sell;
            }
        }

        private void btn_ShopItemListReLoad_Click(object sender, EventArgs e)
        {
            //item
            bool ret = m_ItemCtrl.LoadItemDefList();
            if (!ret)
            {
                MessageBox.Show("LoadItemDefList error!");
                return;
            }

            ret = m_ItemCtrl.LoadItemList();
            if (!ret)
            {
                MessageBox.Show("LoadItemList error!");
                return;
            }

            FillItemListV(); //包括商城、掉落、商店模块
        }

        private void FillShopSingleInfo(ShopData_Str data)
        {
            UInt32 totalCount = 0;
            lstv_ShopDetilList.Items.Clear();
            foreach (var item in data.list)
            {
                string name = CFormat.ToSimplified(CFormat.PureString(item)).ToLower();
                //获取物品的价格
                string cost = "";
                string sell = "";
                Item_Str item_attr;
                bool ret = m_ItemCtrl.GetAttrById(CFormat.PureString(name), out item_attr);
                if (ret)
                {
                    cost = item_attr.cost;
                    sell = item_attr.sell;
                }

                ListViewItem lvi = new ListViewItem();
                if (name.Contains("item_"))
                {
                    name = name.Split(new string[] { "item_" }, StringSplitOptions.RemoveEmptyEntries)[0];
                }
                lvi.Text = name;
                lvi.SubItems.Add(cost);
                lvi.SubItems.Add(sell);
                lstv_ShopDetilList.Items.Add(lvi);
            }
            lstv_ShopDetilList.EndUpdate();

            totalCount = (UInt32)data.list.Count;
            
            //总数
            txt_ShopItemCount.Text = totalCount.ToString();
            txt_ShopCode.Text = data.code.ToString();

            //存在的阶数
            string shop_name = data.ShopName;

            rdb_CityLevel2.Enabled = false;
            rdb_CityLevel3.Enabled = false;
            rdb_CityLevel4.Enabled = false;
            //查询第x阶是否存在
            bool exit = ShopCtrl.ShopDataExit(shop_name, 1);
            if (exit)
            {
                rdb_CityLevel2.Enabled = true;
            }
            exit = ShopCtrl.ShopDataExit(shop_name, 2);
            if (exit)
            {
                rdb_CityLevel3.Enabled = true;
            }
            exit = ShopCtrl.ShopDataExit(shop_name, 3);
            if (exit)
            {
                rdb_CityLevel4.Enabled = true;
            }
        }

        static int lstv_ShopSrchList_SltIndex = 0;
        private void lstv_ShopSrchList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstv_ShopSrchList.SelectedIndices != null && lstv_ShopSrchList.SelectedIndices.Count > 0)
            {
                if (lstv_ShopSrchList_SltIndex >= lstv_ShopSrchList.Items.Count)
                {
                    lstv_ShopSrchList_SltIndex = 0;
                }
                //lstv_DropItemList.Items[lstv_DropItemList_SltIndex].BackColor = Color.Transparent;
                lstv_ShopSrchList.Items[lstv_ShopSrchList_SltIndex].BackColor = Color.Transparent;
                lstv_ShopSrchList_SltIndex = lstv_ShopSrchList.SelectedItems[0].Index;
                lstv_ShopSrchList.Items[lstv_ShopSrchList_SltIndex].BackColor = Color.AliceBlue;

                //get Shop
                ShopData_Str data;
                string shopName = lstv_ShopSrchList.Items[lstv_ShopSrchList_SltIndex].SubItems[1].Text;
                bool ret = ShopCtrl.GetAttrByName(shopName, CityLevel, out data);
                //if (ret)
                {
                    lstv_ShopDetilList_SltIndex = 0;
                    FillShopSingleInfo(data);
                }

                //string shopName = ShopCtrl.GetNameById(data.code);
                //商店名
                txt_ShopName.Text = CFormat.ToSimplified(shopName);
            }
        }

        private void btn_ShopNameSrch_Click(object sender, EventArgs e)
        {
            lstv_ShopSrchList.Items.Clear();
            int i = 0;
            do
            {
                if (lstv_ShopList.Items[i].SubItems[1].Text.Contains(txt_ShopNameSrch.Text))
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = lstv_ShopList.Items[i].SubItems[0].Text;
                    lvi.SubItems.Add(lstv_ShopList.Items[i].SubItems[1].Text);
                    lstv_ShopSrchList.Items.Add(lvi);
                }
                
                i++;
            } while (i < lstv_ShopList.Items.Count);

            lstv_ShopSrchList.EndUpdate();
        }

        private static int lstv_ShopDetilList_SltIndex = 0;
        private void lstv_ShopDetilList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstv_ShopDetilList.SelectedIndices != null && lstv_ShopDetilList.SelectedIndices.Count > 0)
            {
                lstv_ShopDetilList.Items[lstv_ShopDetilList_SltIndex].BackColor = Color.Transparent;
                lstv_ShopDetilList_SltIndex = lstv_ShopDetilList.SelectedItems[0].Index;
                lstv_ShopDetilList.Items[lstv_ShopDetilList_SltIndex].BackColor = Color.AliceBlue;

                //get detil
                string name = lstv_ShopDetilList.Items[lstv_ShopDetilList_SltIndex].SubItems[0].Text;
                string cost = lstv_ShopDetilList.Items[lstv_ShopDetilList_SltIndex].SubItems[1].Text;
                string sell = lstv_ShopDetilList.Items[lstv_ShopDetilList_SltIndex].SubItems[2].Text;

                txt_ShopItemName.Text = name;
                txt_ShopItemCost.Text = cost;
                txt_ShopItemSell.Text = sell;
            }
        }

        private static int lstv_ShopList_SltIndex = 0;
        private void lstv_ShopList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstv_ShopList.SelectedIndices != null && lstv_ShopList.SelectedIndices.Count > 0)
            {
                //lstv_DropItemList.Items[lstv_DropItemList_SltIndex].BackColor = Color.Transparent;
                lstv_ShopList.Items[lstv_ShopList_SltIndex].BackColor = Color.Transparent;
                lstv_ShopList_SltIndex = lstv_ShopList.SelectedItems[0].Index;
                lstv_ShopList.Items[lstv_ShopList_SltIndex].BackColor = Color.AliceBlue;

                rdb_CityLevel1.Checked = true;
                //get Shop
                ShopData_Str data;
                string shopName = lstv_ShopList.Items[lstv_ShopList_SltIndex].SubItems[1].Text;
                bool ret = ShopCtrl.GetAttrByName(shopName, CityLevel, out data);
                if (ret)
                {
                    lstv_ShopDetilList_SltIndex = 0;
                    FillShopSingleInfo(data);

                    //string shopName = ShopCtrl.GetNameById(data.code);
                    //商店名
                    txt_ShopName.Text = CFormat.ToSimplified(shopName);
                }
                else
                {
                    MessageBox.Show("该商店不存在！");
                    lstv_ShopDetilList_SltIndex = 0;
                    lstv_ShopDetilList.Items.Clear();
                    lstv_ShopDetilList.EndUpdate();
                    txt_ShopName.Text = "";
                }
            }
        }

        private void btn_ShopItemAdd_Click(object sender, EventArgs e)
        {
            if (txt_ShopCode.Text == "" || txt_ShopName.Text == "")
            {
                MessageBox.Show("请选择加入的商店！");
                return;
            }
            //当前的商店
            string shopCode = txt_ShopCode.Text;
            string shopName = txt_ShopName.Text;
            string itemName = "item_" + m_ItemCtrl.NameSimpToTrad(txt_ShopItemName.Text);
            int index;
            if (lstv_ShopDetilList.SelectedIndices == null || lstv_ShopDetilList.SelectedIndices.Count <= 0)
            {
                index = 0;
            }
            else
            {
                index = lstv_ShopDetilList.SelectedItems[0].Index;
            }
            if (index >= lstv_ShopDetilList.Items.Count || index < 0)
            {
                index = 0;
            }
            //先修改价格
            string itemCost = txt_ShopItemCost.Text;
            string itemSell = txt_ShopItemSell.Text;
            if (!CFormat.IsNumber(itemCost) || !CFormat.IsNumber(itemSell))
            {
                MessageBox.Show("请输入正确的价格格式！");
                return;
            }

            Item_Str item;
            bool ret = m_ItemCtrl.GetAttrById(itemName, out item);
            if (ret)
            {
                item.cost = itemCost;
                item.sell = itemSell;
                ret = m_ItemCtrl.MdfyItemAttr(itemName, item);
                if (!ret)
                {
                    MessageBox.Show("设定物品价格失败");
                    return;
                }
            }

            ret = ShopCtrl.AddShopItem(shopCode, index, CityLevel, itemName);
            if (!ret)
            {
                MessageBox.Show("物品加入失败！");
            }
            else
            {
                ShopData_Str data;
                ret = ShopCtrl.GetAttrByName(shopName, CityLevel, out data);
                //if (ret)
                {
                    lstv_ShopDetilList_SltIndex = 0;
                    FillShopSingleInfo(data);
                }
            }
        }

        private void btn_ShopItemDel_Click(object sender, EventArgs e)
        {
            if (txt_ShopCode.Text == "" || txt_ShopName.Text == "")
            {
                MessageBox.Show("请选择删除物品的商店！");
                return;
            }

            if (lstv_ShopDetilList.Items.Count <= 0)
            {
                return;
            }

            //当前的商店
            string shopCode = txt_ShopCode.Text;
            string shopName = txt_ShopName.Text;
            string itemName = lstv_ShopDetilList.Items[lstv_ShopDetilList_SltIndex].SubItems[0].Text;
            itemName = "item_" + m_ItemCtrl.NameSimpToTrad(itemName);

            bool ret = ShopCtrl.DelShopItem(shopCode, CityLevel, itemName);
            if (!ret)
            {
                MessageBox.Show("物品删除失败！");
            }
            else
            {
                ShopData_Str data;
                ret = ShopCtrl.GetAttrByName(shopName, CityLevel, out data);
                //if (ret)
                {
                    lstv_ShopDetilList_SltIndex = 0;
                    FillShopSingleInfo(data);
                }
            }
        }

        private void btn_ShopItemMdfy_Click(object sender, EventArgs e)
        {
            if (txt_ShopCode.Text == "" || txt_ShopName.Text == "")
            {
                MessageBox.Show("请选择需要修改的物品！");
                return;
            }
            if (lstv_ShopDetilList.SelectedIndices == null || lstv_ShopDetilList.SelectedIndices.Count <= 0)
            {
                MessageBox.Show("请选择需要修改的物品！");
                return;
            }
            //当前的商店
            string shopCode = txt_ShopCode.Text;
            string shopName = txt_ShopName.Text;
            if (txt_ShopItemName.Text == "")//修改价格
            {
                MessageBox.Show("请选择需要修改的物品！");
                return;
            }

            string itemCost = txt_ShopItemCost.Text;
            string itemSell = txt_ShopItemSell.Text;
            if(!CFormat.IsNumber(itemCost) || !CFormat.IsNumber(itemSell))
            {
                MessageBox.Show("请输入正确的价格格式！");
                return;
            }

            string itemId = txt_ShopItemName.Text;
            itemId = "item_" + itemId;

            Item_Str item;
            bool ret = m_ItemCtrl.GetAttrById(itemId, out item);
            if (ret)
            {
                item.cost = itemCost;
                item.sell = itemSell;
                ret = m_ItemCtrl.MdfyItemAttr(itemId, item);
                if (!ret)
                {
                    MessageBox.Show("修改物品价格失败");
                }
                else
                {
                    ShopData_Str data;
                    ret = ShopCtrl.GetAttrByName(shopName, CityLevel, out data);
                    //if (ret)
                    {
                        lstv_ShopDetilList_SltIndex = 0;
                        FillShopSingleInfo(data);
                    }
                }
            }
            
        }

        private void btn_ShopItemFwd_Click(object sender, EventArgs e)
        {
            string sltCode = txt_ShopCode.Text;
            string shopName = txt_ShopName.Text;
            string sltName = lstv_ShopDetilList.Items[lstv_ShopDetilList_SltIndex].SubItems[0].Text;
            if (txt_ShopItemName.Text == "" || txt_ShopItemName.Text == null || txt_ShopItemName.Text != sltName)
            {
                MessageBox.Show("请选择需要修改的物品！");
                return;
            }

            string itemName = "item_" + sltName;

            bool ret = ShopCtrl.ItemSendFwd(sltCode, CityLevel, itemName);
            if (ret)
            {
                ShopData_Str data;
                ret = ShopCtrl.GetAttrByName(shopName, CityLevel, out data);
                //if (ret)
                {
                    int sltTmp = lstv_ShopDetilList_SltIndex;
                    lstv_ShopDetilList_SltIndex -= 1;
                    FillShopSingleInfo(data);

                    lstv_ShopDetilList.Items[sltTmp].BackColor = Color.Transparent;
                    lstv_ShopDetilList.Items[lstv_ShopDetilList_SltIndex].BackColor = Color.AliceBlue;
                }
            }
        }

        private void btn_ShopItemNext_Click(object sender, EventArgs e)
        {
            string sltCode = txt_ShopCode.Text;
            string shopName = txt_ShopName.Text;
            string sltName = lstv_ShopDetilList.Items[lstv_ShopDetilList_SltIndex].SubItems[0].Text;
            if (txt_ShopItemName.Text == "" || txt_ShopItemName.Text == null || txt_ShopItemName.Text != sltName)
            {
                MessageBox.Show("请选择需要修改的物品！");
                return;
            }

            string itemName = "item_" + sltName;

            bool ret = ShopCtrl.ItemSendNext(sltCode, CityLevel, itemName);
            if (ret)
            {
                ShopData_Str data;
                ret = ShopCtrl.GetAttrByName(shopName, CityLevel, out data);
                //if (ret)
                {
                    int sltTmp = lstv_ShopDetilList_SltIndex;
                    lstv_ShopDetilList_SltIndex += 1;
                    FillShopSingleInfo(data);


                    lstv_ShopDetilList.Items[sltTmp].BackColor = Color.Transparent;
                    lstv_ShopDetilList.Items[lstv_ShopDetilList_SltIndex].BackColor = Color.AliceBlue;
                }
            }
        }

        private void btn_ShopNameReLoad_Click(object sender, EventArgs e)
        {
            bool ret = ShopCtrl.LoadShopDataList();
            if (!ret)
            {
                MessageBox.Show("LoadShopDataList error!");
                return ;
            }
            ret = ShopCtrl.LoadShopDefList();
            if (!ret)
            {
                MessageBox.Show("LoadShopDefList error!");
                return ;
            }

            FillShopListV();
        }

        private void btn_ShopDataReLoad_Click(object sender, EventArgs e)
        {
            //item
            bool ret = m_ItemCtrl.LoadItemDefList();
            if (!ret)
            {
                MessageBox.Show("LoadItemDefList error!");
                return;
            }

            ret = m_ItemCtrl.LoadItemList();
            if (!ret)
            {
                MessageBox.Show("LoadItemList error!");
                return;
            }

            FillItemListV(); //包括商城、掉落、商店模块

            ret = ShopCtrl.LoadShopDataList();
            if (!ret)
            {
                MessageBox.Show("LoadShopDataList error!");
                return;
            }
            ret = ShopCtrl.LoadShopDefList();
            if (!ret)
            {
                MessageBox.Show("LoadShopDefList error!");
                return;
            }

            FillShopListV();
        }

        private void btn_ShopDataSaveFile_Click(object sender, EventArgs e)
        {
            bool ret = m_ItemCtrl.SaveItemInfo();
            if (!ret)
            {
                MessageBox.Show("保存文件失败");
                return;
            }
            ret = ShopCtrl.SaveShopDataInfo();
            if (!ret)
            {
                MessageBox.Show("保存文件失败");
                return;
            }

            MessageBox.Show("保存文件成功，文件已经保存在new目录！");
        }

        private static int CityLevel = 0;
        private void rdb_CityLevel1_CheckedChanged(object sender, EventArgs e)
        {
            if (rdb_CityLevel1.Checked)
            {
                CityLevel = 0;
                //get Shop
                ShopData_Str data;
                string shopName = txt_ShopName.Text;
                bool ret = ShopCtrl.GetAttrByName(shopName, CityLevel, out data);
                if (ret)
                {
                    lstv_ShopDetilList_SltIndex = 0;
                    FillShopSingleInfo(data);
                }
                else
                {
                    MessageBox.Show("该商店不存在！");
                    lstv_ShopDetilList_SltIndex = 0;
                    lstv_ShopDetilList.Items.Clear();
                    lstv_ShopDetilList.EndUpdate();
                }
            }
        }

        private void rdb_CityLevel2_CheckedChanged(object sender, EventArgs e)
        {
            if (rdb_CityLevel2.Checked)
            {
                CityLevel = 1;
                //get Shop
                ShopData_Str data;
                string shopName = txt_ShopName.Text;
                bool ret = ShopCtrl.GetAttrByName(shopName, CityLevel, out data);
                if (ret)
                {
                    lstv_ShopDetilList_SltIndex = 0;
                    FillShopSingleInfo(data);
                }
                else
                {
                    MessageBox.Show("该商店不存在！");
                    lstv_ShopDetilList_SltIndex = 0;
                    lstv_ShopDetilList.Items.Clear();
                    lstv_ShopDetilList.EndUpdate();
                }
            }
        }

        private void rdb_CityLevel3_CheckedChanged(object sender, EventArgs e)
        {
            if (rdb_CityLevel3.Checked)
            {
                CityLevel = 2;
                //get Shop
                ShopData_Str data;
                string shopName = txt_ShopName.Text;
                bool ret = ShopCtrl.GetAttrByName(shopName, CityLevel, out data);
                if (ret)
                {
                    lstv_ShopDetilList_SltIndex = 0;
                    FillShopSingleInfo(data);
                }
                else
                {
                    MessageBox.Show("该商店不存在！");
                    lstv_ShopDetilList_SltIndex = 0;
                    lstv_ShopDetilList.Items.Clear();
                    lstv_ShopDetilList.EndUpdate();
                }
            }
        }

        private void rdb_CityLevel4_CheckedChanged(object sender, EventArgs e)
        {
            if (rdb_CityLevel4.Checked)
            {
                CityLevel = 3;
                //get Shop
                ShopData_Str data;
                string shopName = txt_ShopName.Text;
                bool ret = ShopCtrl.GetAttrByName(shopName, CityLevel, out data);
                if (ret)
                {
                    lstv_ShopDetilList_SltIndex = 0;
                    FillShopSingleInfo(data);
                }
                else
                {
                    MessageBox.Show("该商店不存在！");
                    lstv_ShopDetilList_SltIndex = 0;
                    lstv_ShopDetilList.Items.Clear();
                    lstv_ShopDetilList.EndUpdate();
                }
            }
        }

        private void btn_DropConfAdd_Click(object sender, EventArgs e)
        {
            if(txt_DropConfName.Text == "")
            {
                MessageBox.Show("请填写当前配置备注！");
                return;
            }
            if(lstv_DropItemList.Items.Count <= 0)
            {
                MessageBox.Show("当前掉落无数据，不给予保存此配置！");
                return;
            }

            //
            string confName = txt_DropConfName.Text;
            //遍历lstv_DropItemList的items，构成字符串
            string confDetil = "";

            for(int ii=0; ii<lstv_DropItemList.Items.Count; ii++)
            {
                for (int i = 0; i < lstv_DropItemList.Items[ii].SubItems.Count; i++)
                {
                    confDetil += lstv_DropItemList.Items[ii].SubItems[i].Text + ",";
                }
            }
            //MessageBox.Show(confDetil);
            //加入配置文件
            int index = lstv_DropConf.Items.Count + 1;
            string ConfName = "ConfName" + index;
            string ConfDetil = "ConfDetil" + index;
            if (confName == "" || confName == string.Empty)
            {
                confName = "NULL";
            }
            if (confDetil == "" || confDetil == string.Empty)
            {
                confDetil = "NULL";
            }
            CIniCtrl.WriteIniData("Configs", ConfName, confName, m_ConfIni);
            CIniCtrl.WriteIniData("Configs", ConfDetil, confDetil, m_ConfIni);

            //加入列表
            ListViewItem lvi = new ListViewItem();
            lvi.Text = confName;
            lvi.SubItems.Add(confDetil);
            lstv_DropConf.Items.Add(lvi);
        }

        private static int lstv_DropConf_SltIndex = 0;
        private static int lstv_DorpConfDetil_SltIndex = 0;
        private void lstv_DropConf_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstv_DropConf.SelectedIndices != null && lstv_DropConf.SelectedIndices.Count > 0)
            {
                //lstv_DropItemList.Items[lstv_DropItemList_SltIndex].BackColor = Color.Transparent;
                lstv_DropConf.Items[lstv_DropConf_SltIndex].BackColor = Color.Transparent;
                lstv_DropConf_SltIndex = lstv_DropConf.SelectedItems[0].Index;
                lstv_DropConf.Items[lstv_DropConf_SltIndex].BackColor = Color.AliceBlue;

                //get config
                string detil = lstv_DropConf.Items[lstv_DropConf_SltIndex].SubItems[1].Text;
                if (detil != "" && detil != null && detil != "NULL")
                {
                    var detils = detil.Split(',');
                    lstv_DorpConfDetil.Items.Clear();
                    lstv_DorpConfDetil.BeginUpdate();
                    for (int i = 0; i < detils.Length; i += 2)
                    {
                        string itemName = detils[i];
                        if (itemName == "" || itemName == string.Empty)
                        {
                            break;
                        }
                        string itemDropCnt = detils[i + 1];

                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = itemName;
                        lvi.SubItems.Add(itemDropCnt);
                        lstv_DorpConfDetil.Items.Add(lvi);
                    }
                    lstv_DorpConfDetil.EndUpdate();
                }
                else
                {
                    MessageBox.Show("该配置不存在！");
                    lstv_DorpConfDetil_SltIndex = 0;
                    lstv_DorpConfDetil.Items.Clear();
                    lstv_DorpConfDetil.EndUpdate();
                }

                txt_DropConfName.Text = lstv_DropConf.Items[lstv_DropConf_SltIndex].SubItems[0].Text;
            }
        }

        private void btn_DropConfUse_Click(object sender, EventArgs e)
        {
            string dropId = txt_DropSingleId.Text;
            if (dropId != string.Empty)
            {
                lstv_DropItemList.Items.Clear();
                DropCtrl.ClearSingleDrops(txt_DropSingleId.Text);

                for(int i =0; i<lstv_DorpConfDetil.Items.Count; i++)
                {
                    string name = lstv_DorpConfDetil.Items[i].SubItems[0].Text;
                    string num = lstv_DorpConfDetil.Items[i].SubItems[1].Text;
                    if (name == "" || name == string.Empty || num == "" || num == string.Empty)
                    {
                        continue;
                    }
                    //更新lstv_DropItemList
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = name;
                    lvi.SubItems.Add(num);
                    lstv_DropItemList.Items.Add(lvi);

                    //修改对应的drop
                    string old = name;
                    if (name != "Nothing" /*&& name != "Money"*/)
                    {
                        name = m_ItemCtrl.NameSimpToTrad(name);
                    }
                    name = "item_" + name;
                    if (name == "item_")
                    {
                        old = "";
                        continue;
                    }
                    DropCtrl.AddSingleDrop(dropId, name, num);

                    DropRefreshListV();
                }
            }
        }

        private void btn_DropConfDel_Click(object sender, EventArgs e)
        {
            if (lstv_DropConf_SltIndex >= 0 && lstv_DropConf_SltIndex < lstv_DropConf.Items.Count)
            {
                int index = lstv_DropConf_SltIndex + 1;
                string ConfName = "ConfName" + index;
                string ConfDetil = "ConfDetil" + index;
                CIniCtrl.WriteIniData("Configs", ConfName, "NULL", m_ConfIni);
                CIniCtrl.WriteIniData("Configs", ConfDetil, "NULL", m_ConfIni);

                //清空
                lstv_DorpConfDetil.Items.Clear();
                lstv_DropConf.Items[lstv_DropConf_SltIndex].SubItems[1].Text = "NULL";
            }
        }

        private void btn_DropConfMdfy_Click(object sender, EventArgs e)
        {
            if (lstv_DropConf_SltIndex < 0 || lstv_DropConf_SltIndex >= lstv_DropConf.Items.Count)
            {
                MessageBox.Show("请选择需要修改的选项！");
                return;
            }
            if (txt_DropConfName.Text == "")
            {
                MessageBox.Show("请填写当前配置备注！");
                return;
            }
            if (lstv_DropItemList.Items.Count <= 0)
            {
                MessageBox.Show("当前掉落无数据，不给予保存此配置！");
                return;
            }

            //
            string confName = txt_DropConfName.Text;
            //遍历lstv_DropItemList的items，构成字符串
            string confDetil = "";

            for (int ii = 0; ii < lstv_DropItemList.Items.Count; ii++)
            {
                for (int i = 0; i < lstv_DropItemList.Items[ii].SubItems.Count; i++)
                {
                    confDetil += lstv_DropItemList.Items[ii].SubItems[i].Text + ",";
                }
            }
            //MessageBox.Show(confDetil);
            //加入配置文件
            int index = lstv_DropConf_SltIndex + 1;
            string ConfName = "ConfName" + index;
            string ConfDetil = "ConfDetil" + index;
            if (confName == "" || confName == string.Empty)
            {
                confName = "NULL";
            }
            if (confDetil == "" || confDetil == string.Empty)
            {
                confDetil = "NULL";
            }
            CIniCtrl.WriteIniData("Configs", ConfName, confName, m_ConfIni);
            CIniCtrl.WriteIniData("Configs", ConfDetil, confDetil, m_ConfIni);

            lstv_DropConf.Items[lstv_DropConf_SltIndex].SubItems[0].Text = confName;
            lstv_DropConf.Items[lstv_DropConf_SltIndex].SubItems[1].Text = confDetil;
            //更新lstv_DorpConfDetil
            lstv_DropConf_SelectedIndexChanged(sender, e);
        }

        private void btn_DropConfMdfyTag_Click(object sender, EventArgs e)
        {
            if (lstv_DropConf_SltIndex < 0 || lstv_DropConf_SltIndex >= lstv_DropConf.Items.Count)
            {
                MessageBox.Show("请选择需要修改备注的选项！");
                return;
            }
            string confName = txt_DropConfName.Text;
            int index = lstv_DropConf_SltIndex + 1;
            string ConfName = "ConfName" + index;
            string ConfDetil = "ConfDetil" + index;
            if (confName == "" || confName == string.Empty)
            {
                confName = "NULL";
            }
            CIniCtrl.WriteIniData("Configs", ConfName, confName, m_ConfIni);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string dropId = lstv_DropList.Items[lstv_DropList_SltIndex].SubItems[1].Text;
            for (int i = 0; i < lstv_DropItemList.Items.Count; i++)
            {
                string name = lstv_DropItemList.Items[i].SubItems[0].Text;
                if (name != "Money")
                {
                    name = m_ItemCtrl.NameSimpToTrad(name);
                }

                name = "item_" + name;
                string oldNum = lstv_DropItemList.Items[i].SubItems[1].Text;
                string newNum = "";
                if (i == 0)
                {
                    newNum = (100000000 / int.Parse(txt_DropSingleItemCount.Text) + 100000000 % int.Parse(txt_DropSingleItemCount.Text)).ToString();
                }
                else {
                    newNum = (100000000 / int.Parse(txt_DropSingleItemCount.Text)).ToString();
                }

                bool ret = DropCtrl.MdfyDropNum(dropId, name, oldNum, newNum);
            }
            
            DropRefreshListV();
        }
    }
}
