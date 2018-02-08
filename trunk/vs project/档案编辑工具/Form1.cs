using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 档案编辑工具
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.Description = "请选择文件路径";
            string foldPath = "";
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                foldPath = folderDialog.SelectedPath;
            }

            //读取版本号到txt_svrForder
            txt_MainFolder.Text = foldPath;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Item load datas
            load_Data_All();
        }

        private void load_Data_All()
        { 
            //load_item_data
            CItemCtrl.load_Item_Data("asd");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CItemCtrl.AnalyzerItemAttr(txt_MainFolder.Text);
            MessageBox.Show("Done");
        }
        private string Build_Item_Attr()
        {
            //define
            string new_itemH = "#define " + txt_itemCode.Text+"\t";
            //get last id
            string curId = "";

            int n = int.Parse(CItemCtrl.m_lastId);
            n++;
            curId = n.ToString();
            new_itemH += curId;
            MessageBox.Show(new_itemH);

            //txt
            string newItemData = "";


            //name
            string newItemName = "item = ";
            n = CItemCtrl.Get_Item_LastNameId();
            n += 2;
            curId = n.ToString();
            newItemName += curId + ",";
            newItemName += txt_itemNameStr.Text;

            CItemCtrl.Set_Item_LastNameId(n);
                
            //name
            string newItemHelper = "";
            n = CItemCtrl.Get_Item_LastHelpId();
            n += 2;
            curId = n.ToString();
            newItemName += curId + ",";
            newItemName += txt_itemNameStr.Text;

            CItemCtrl.Set_Item_LastHelpId(n);

            return "";
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (rdb_itemMdfy.Checked)//修改
            {
                
            }
            else if (rdb_itemAdd.Checked)//添加
            {
                string new_item = Build_Item_Attr();
            }
            else if (rdb_itemDel.Checked)//删除
            {

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //load_Item_List
            string allItemDefines = CItemCtrl.load_Item_Defines("");
            var allItemDefinesArr = allItemDefines.Split(';');

            this.listView1.Items.Clear();
            foreach(var _def in allItemDefinesArr)
            {
                var _nameWithIdarr = _def.Split(',');
                if(_nameWithIdarr.Length > 1)
                {
                    string _name = _nameWithIdarr[0];
                    string _id = _nameWithIdarr[1];
                    ListViewItem lvi = new ListViewItem();

                    //lvi.ImageIndex = i;     //通过与imageList绑定，显示imageList中第i项图标 

                    lvi.Text = _id;

                    lvi.SubItems.Add(_name);

                    this.listView1.Items.Add(lvi);
                }
            }
            this.listView1.EndUpdate();  //结束数据处理，UI界面一次性绘制
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices != null && listView1.SelectedIndices.Count > 0)
            {

                string name = this.listView1.FocusedItem.SubItems[1].Text;
                
                bool done = false;
                CItemCtrl.item_attr attr = CItemCtrl.Get_Attr_ByName(name, out done);
                //MessageBox.Show(attr.code);
                Fill_Item_Attr(attr);
            }
        }
        private void Fill_Item_TypeWeapon(string types)
        {
            rdb_itemTypeBow.Checked = false;
            rdb_itemTypeTwoHand.Checked = false;
            rdb_itemTypeLance.Checked = false;
            rdb_itemTypeFalchion.Checked = false;
            rdb_itemTypeSword.Checked = false;
            rdb_itemTypeThrow.Checked = false;
            //weapon type
            var _typesArr = types.Split(',');
            foreach(var _type in _typesArr)
            {
                if (_type == "itemTypeThrow")
                {
                    rdb_itemTypeThrow.Checked = true;
                }
                else if (_type == "itemTypeSword")
                {
                    rdb_itemTypeSword.Checked = true;
                }
                else if (_type == "itemTypeFalchion")
                {
                    rdb_itemTypeFalchion.Checked = true;
                }
                else if (_type == "itemTypeLance")
                {
                    rdb_itemTypeLance.Checked = true;
                }
                else if (_type == "itemTypeTwoHand")
                {
                    rdb_itemTypeTwoHand.Checked = true;
                }
                else if (_type == "itemTypeBow")
                {
                    rdb_itemTypeBow.Checked = true;
                }
            }
        }
        private void Fill_Item_TypeWait(string wait_type)
        {
            rdb_act_STAND_SWORD.Checked = false;
            rdb_act_STAND_LANCE.Checked = false;
            rdb_act_STAND_MACHETE.Checked = false;
            rdb_act_STAND_PIKE.Checked = false;
            if(wait_type == "act_STAND_SWORD")
            {
                rdb_act_STAND_SWORD.Checked = true;
            }
            else if (wait_type == "act_STAND_LANCE")
            {
                rdb_act_STAND_LANCE.Checked = true;
            }
            else if (wait_type == "act_STAND_MACHETE")
            {
                rdb_act_STAND_MACHETE.Checked = true;
            }
            else if (wait_type == "act_STAND_PIKE")
            {
                rdb_act_STAND_PIKE.Checked = true;
            }
        }
        private void Fill_Item_TypeSuper(string super_type)
        {
            rdb_superitemTypeBow.Checked = false;
            rdb_superitemTypeTwo.Checked = false;
            rdb_superitemTypeLance.Checked = false;
            rdb_superitemTypeFalchion.Checked = false;
            rdb_superitemTypeSword.Checked = false;
            rdb_superitemTypeThrow.Checked = false;
            rdb_superitemTypeArrow.Checked = false;

            if (super_type == "itemTypeThrow")
            {
                rdb_superitemTypeThrow.Checked = true;
            }
            else if (super_type == "itemTypeSword")
            {
                rdb_superitemTypeSword.Checked = true;
            }
            else if (super_type == "itemTypeFalchion")
            {
                rdb_superitemTypeFalchion.Checked = true;
            }
            else if (super_type == "itemTypeLance")
            {
                rdb_superitemTypeLance.Checked = true;
            }
            else if (super_type == "itemTypeTwo")
            {
                rdb_superitemTypeTwo.Checked = true;
            }
            else if (super_type == "itemTypeBow")
            {
                rdb_superitemTypeBow.Checked = true;
            }
            else if (super_type == "itemTypeArrow")
            {
                rdb_superitemTypeArrow.Checked = true;
            }
        }
        private void Fill_Item_Types(string types, string wait_type,string super_type)
        {

            var _itemTypeArr = types.Split(',');
            if (_itemTypeArr.Contains("itemTypeWeapon"))
            {
                rdb_itemWeapon.Checked = true;
            }
            else
            {

                rdb_itemWeapon.Checked = false;
            }
            Fill_Item_TypeWeapon(types);
            Fill_Item_TypeWait(wait_type);
            Fill_Item_TypeSuper(super_type);

            if (_itemTypeArr.Contains("itemTypeItem"))
            {
                rdb_itemItem.Checked = true;
            }
            else
            {
                rdb_itemItem.Checked = false;
            }
            if (!_itemTypeArr.Contains("itemTypeItem") && !_itemTypeArr.Contains("itemTypeWeapon"))
            {
                rdb_itemOther.Checked = true;
            }
            else
            {
                rdb_itemOther.Checked = false;
            }
            if (_itemTypeArr.Contains("itemTypeNoUse"))
            {
                ckb_itemNoUse.Checked = true;
            }
            else
            {
                ckb_itemNoUse.Checked = false;
            }
            if (_itemTypeArr.Contains("itemTypeNoDrop"))
            {
                ckb_itemNoDrop.Checked = true;
            }
            else
            {
                ckb_itemNoDrop.Checked = false;
            }
            if (_itemTypeArr.Contains("itemTypeNoTrade"))
            {
                ckb_itemNoTrade.Checked = true;
            }
            else
            {
                ckb_itemNoTrade.Checked = false;
            }
        }
        private void Fill_Item_Types2(string types2)
        {
            ckb_itemType2MayPutStorage.Checked = false;
            ckb_itemType2NoSell.Checked = false;
            ckb_itemType2HandArmor.Checked = false;
            ckb_itemType2DoubleDagger.Checked = false;
            if (types2 == null || types2 == "")
            {
                return;
            }
            if(types2.Contains("itemType2MayPutStorage"))
            {
                ckb_itemType2MayPutStorage.Checked = true;
            }
            if (types2.Contains("itemType2NoSell"))
            {
                ckb_itemType2NoSell.Checked = true;
            }
            if (types2.Contains("itemType2HandArmor"))
            {
                ckb_itemType2HandArmor.Checked = true;
            }
            if (types2.Contains("itemType2DoubleDagger"))
            {
                ckb_itemType2DoubleDagger.Checked = true;
            }
        }
        private void Fill_Item_AtkSkillAttr(string eq_magic_attack_type)
        {
            ckb_atkskillAttr_FIRE.Checked = false;
            txt_atkskillAttr_FIRE.Text = "";
            ckb_atkskillAttr_EVIL.Checked = false;
            txt_atkskillAttr_EVIL.Text = "";
            ckb_atkskillAttr_GOD.Checked = false;
            txt_atkskillAttr_GOD.Text = "";
            ckb_atkskillAttr_WATER.Checked = false;
            txt_atkskillAttr_WATER.Text = "";
            ckb_atkskillAttr_ARROW.Checked = false;
            txt_atkskillAttr_ARROW.Text = "";
            ckb_atkskillAttr_BREAK.Checked = false;
            txt_atkskillAttr_BREAK.Text = "";
            ckb_atkskillAttr_BREAK.Checked = false;
            txt_atkskillAttr_BREAK.Text = "";
            ckb_atkskillAttr_STING.Checked = false;
            txt_atkskillAttr_STING.Text = "";
            ckb_atkskillAttr_SLASH.Checked = false;
            txt_atkskillAttr_SLASH.Text = "";

            if (eq_magic_attack_type == null || eq_magic_attack_type == "")
            {
                return;
            }
            var atkTypeArr = eq_magic_attack_type.Split(';');
            foreach(var atk_type in atkTypeArr)
            {
                var _type = atk_type.Split(',');
                if (_type[0] == "skillAttr_SLASH")
                {
                    ckb_atkskillAttr_SLASH.Checked = true;
                    txt_atkskillAttr_SLASH.Text = _type[1];
                }
                else if (_type[0] == "skillAttr_STING")
                {
                    ckb_atkskillAttr_STING.Checked = true;
                    txt_atkskillAttr_STING.Text = _type[1];
                }
                else if (_type[0] == "skillAttr_BREAK")
                {
                    ckb_atkskillAttr_BREAK.Checked = true;
                    txt_atkskillAttr_BREAK.Text = _type[1];
                }
                else if (_type[0] == "skillAttr_ARROW")
                {
                    ckb_atkskillAttr_ARROW.Checked = true;
                    txt_atkskillAttr_ARROW.Text = _type[1];
                }
                else if (_type[0] == "skillAttr_WATER")
                {
                    ckb_atkskillAttr_WATER.Checked = true;
                    txt_atkskillAttr_WATER.Text = _type[1];
                }
                else if (_type[0] == "skillAttr_GOD")
                {
                    ckb_atkskillAttr_GOD.Checked = true;
                    txt_atkskillAttr_GOD.Text = _type[1];
                }
                else if (_type[0] == "skillAttr_EVIL")
                {
                    ckb_atkskillAttr_EVIL.Checked = true;
                    txt_atkskillAttr_EVIL.Text = _type[1];
                }
                else if (_type[0] == "skillAttr_FIRE")
                {
                    ckb_atkskillAttr_FIRE.Checked = true;
                    txt_atkskillAttr_FIRE.Text = _type[1];
                }
            }
        }
        private void Fill_Item_ReSkillAttr(string eq_magic_resist)
        {
            ckb_reskillAttr_FIRE.Checked = false;
            txt_reskillAttr_FIRE.Text = "";
            ckb_reskillAttr_EVIL.Checked = false;
            txt_reskillAttr_EVIL.Text = "";
            ckb_reskillAttr_GOD.Checked = false;
            txt_reskillAttr_GOD.Text = "";
            ckb_reskillAttr_WATER.Checked = false;
            txt_reskillAttr_WATER.Text = "";
            ckb_reskillAttr_ARROW.Checked = false;
            txt_reskillAttr_ARROW.Text = "";
            ckb_reskillAttr_BREAK.Checked = false;
            txt_reskillAttr_BREAK.Text = "";
            ckb_reskillAttr_BREAK.Checked = false;
            txt_reskillAttr_BREAK.Text = "";
            ckb_reskillAttr_STING.Checked = false;
            txt_reskillAttr_STING.Text = "";
            ckb_reskillAttr_SLASH.Checked = false;
            txt_reskillAttr_SLASH.Text = "";

            if (eq_magic_resist == null || eq_magic_resist == "")
            {
                return;
            }
            var reTypeArr = eq_magic_resist.Split(';');
            foreach (var re_type in reTypeArr)
            {
                var _type = re_type.Split(',');
                if (_type[0] == "skillAttr_SLASH")
                {
                    ckb_reskillAttr_SLASH.Checked = true;
                    txt_reskillAttr_SLASH.Text = _type[1];
                }
                else if (_type[0] == "skillAttr_STING")
                {
                    ckb_reskillAttr_STING.Checked = true;
                    txt_reskillAttr_STING.Text = _type[1];
                }
                else if (_type[0] == "skillAttr_BREAK")
                {
                    ckb_reskillAttr_BREAK.Checked = true;
                    txt_reskillAttr_BREAK.Text = _type[1];
                }
                else if (_type[0] == "skillAttr_ARROW")
                {
                    ckb_reskillAttr_ARROW.Checked = true;
                    txt_reskillAttr_ARROW.Text = _type[1];
                }
                else if (_type[0] == "skillAttr_WATER")
                {
                    ckb_reskillAttr_WATER.Checked = true;
                    txt_reskillAttr_WATER.Text = _type[1];
                }
                else if (_type[0] == "skillAttr_GOD")
                {
                    ckb_reskillAttr_GOD.Checked = true;
                    txt_reskillAttr_GOD.Text = _type[1];
                }
                else if (_type[0] == "skillAttr_EVIL")
                {
                    ckb_reskillAttr_EVIL.Checked = true;
                    txt_reskillAttr_EVIL.Text = _type[1];
                }
                else if (_type[0] == "skillAttr_FIRE")
                {
                    ckb_reskillAttr_FIRE.Checked = true;
                    txt_reskillAttr_FIRE.Text = _type[1];
                }
            }
        }
        private void Fill_Item_FuncAnti(string function, string anti_status_ratio)
        {
            if (function == "" || function == null)
            {
                return;
            }
            if (function.Contains("effFun_ANTISTATUS"))
            {
                
            }
            else 
            {
                
            }
            var funcArr = function.Split(';');
            foreach(var _func in funcArr)
            {
                var _detail = _func.Split(',');
                if (_detail[0] != "effFun_ANTISTATUS") //攻
                {
                    if (_detail[0] == "effFun_POISON")
                    {
                        ckb_atkeffFun_POISON.Checked = true;
                        txt_atkeffFun_POISON1.Text = _detail[1];
                        txt_atkeffFun_POISON2.Text = _detail[2];
                        txt_atkeffFun_POISON3.Text = _detail[3];
                    }
                    else if (_detail[0] == "effFun_SLOW")
                    {
                        ckb_atkeffFun_SLOW.Checked = true;
                        txt_atkeffFun_SLOW1.Text = _detail[1];
                        txt_atkeffFun_SLOW2.Text = _detail[2];
                        txt_atkeffFun_SLOW3.Text = _detail[3];
                    }
                    else if (_detail[0] == "effFun_STUN")
                    {
                        ckb_atkeffFun_STUN.Checked = true;
                        txt_atkeffFun_STUN1.Text = _detail[1];
                        txt_atkeffFun_STUN2.Text = _detail[2];
                        txt_atkeffFun_STUN3.Text = _detail[3];
                    }
                    else if (_detail[0] == "effFun_WEAKEN")
                    {
                        ckb_atkeffFun_WEAKEN.Checked = true;
                        txt_atkeffFun_WEAKEN1.Text = _detail[1];
                        txt_atkeffFun_WEAKEN2.Text = _detail[2];
                        txt_atkeffFun_WEAKEN3.Text = _detail[3];
                    }
                    else if (_detail[0] == "effFun_NO_SPEC")
                    {
                        ckb_atkeffFun_NO_SPEC.Checked = true;
                        txt_atkeffFun_NO_SPEC1.Text = _detail[1];
                        txt_atkeffFun_NO_SPEC2.Text = _detail[2];
                        txt_atkeffFun_NO_SPEC3.Text = _detail[3];
                    }
                    else if (_detail[0] == "effFun_NO_MAGIC")
                    {
                        ckb_atkeffFun_NO_MAGIC.Checked = true;
                        txt_atkeffFun_NO_MAGIC1.Text = _detail[1];
                        txt_atkeffFun_NO_MAGIC2.Text = _detail[2];
                        txt_atkeffFun_NO_MAGIC3.Text = _detail[3];
                    }
                }
            }

            if (anti_status_ratio == "" || anti_status_ratio == null)
            {
                return;
            }
            var antiArr = anti_status_ratio.Split(';');
            foreach (var _anti in antiArr)
            {
                var _detail = _anti.Split(',');
                if (_detail[0] == "effFun_POISON")
                {
                    ckb_reeffFun_POISON.Checked = true;
                    txt_reeffFun_POISON.Text = _detail[1];
                }
                else if (_detail[0] == "effFun_SLOW")
                {
                    ckb_reeffFun_SLOW.Checked = true;
                    txt_reeffFun_SLOW.Text = _detail[1];
                }
                else if (_detail[0] == "effFun_STUN")
                {
                    ckb_reeffFun_STUN.Checked = true;
                    txt_reeffFun_STUN.Text = _detail[1];
                }
                else if (_detail[0] == "effFun_WEAKEN")
                {
                    ckb_reeffFun_WEAKEN.Checked = true;
                    txt_reeffFun_WEAKEN.Text = _detail[1];
                }
                else if (_detail[0] == "effFun_NO_SPEC")
                {
                    ckb_reeffFun_NO_SPEC.Checked = true;
                    txt_reeffFun_NO_SPEC.Text = _detail[1];
                }
                else if (_detail[0] == "effFun_NO_MAGIC")
                {
                    ckb_reeffFun_NO_MAGIC.Checked = true;
                    txt_reeffFun_NO_MAGIC.Text = _detail[1];
                }
            }
        }
        private void Fill_Item_Attr(CItemCtrl.item_attr attr)
        {
            //基本屬性
            txt_itemCode.Text = attr.code;
            txt_itemName.Text = attr.name;
            string nameStr = CItemCtrl.Get_Item_NameStr(attr.name);
            txt_itemNameStr.Text = nameStr;
            txt_itemHelp.Text = attr.help_string;
            string itemHelpStr = CItemCtrl.Get_Item_itemHelpStr(attr.help_string);
            txt_itemHelpStr.Text = itemHelpStr;
            txt_itemLight.Text = attr.light;
            txt_itemWeight.Text = attr.weight;
            txt_itemCost.Text = attr.cost;
            txt_itemSell.Text = attr.sell;
            txt_itemCost_level.Text = attr.cost_level;
            txt_itemIcon.Text = attr.icon;

            txt_itemType.Text = attr.type;
            Fill_Item_Types(attr.type, attr.wait_type,attr.super_type);

            txt_itemType2.Text = attr.type2;
            Fill_Item_Types2(attr.type2);

            //增加属性
            txt_itemAddattack_range.Text = attr.attack_range;
            txt_itemAddeffect_range.Text = attr.effect_range;
            txt_itemAddadd_attack_power.Text = attr.add_attack_power;
            txt_itemAddattack_speed.Text = attr.attack_speed;
            txt_itemAddadd_weapon_hit.Text = attr.add_weapon_hit;
            txt_itemAddadd_attr_mind.Text = attr.add_attr_mind;
            txt_itemAddadd_magic_power.Text = attr.add_magic_power;
            txt_itemAddadd_attr_con.Text = attr.add_attr_con;
            txt_itemAddadd_attackx2_ratio.Text = attr.add_attackx2_ratio;
            txt_itemAddadd_attr_int.Text = attr.add_attr_int;
            txt_itemAddadd_attr_str.Text = attr.add_attr_str;
            txt_itemAddadd_attr_dex.Text = attr.add_attr_dex;
            txt_itemAddadd_hp.Text = attr.add_hp;
            txt_itemAddadd_mp.Text = attr.add_mp;
            txt_itemAddadd_attr_leader.Text = attr.add_attr_leader;
            txt_itemAddadd_defense.Text = attr.add_defense;
            txt_itemAddadd_weapon_misshit.Text = attr.add_weapon_misshit;
            txt_itemAddmove.Text = attr.move;
            txt_itemAddadd_st.Text = attr.add_st;
            txt_itemAddadd_loyalty.Text = attr.add_loyalty;
            //属性攻
            Fill_Item_AtkSkillAttr(attr.eq_magic_attack_type);
            //属性防
            Fill_Item_ReSkillAttr(attr.eq_magic_resist);
            //function
            Fill_Item_FuncAnti(attr.function, attr.anti_status_ratio);
            
            //limit
            txt_itemlimit_level.Text = attr.limit_level;
            txt_itemlimit_str.Text = attr.limit_str;
            txt_itemlimit_mind.Text = attr.limit_mind;
            txt_itemlimit_dex.Text = attr.limit_dex;
            txt_itemlimit_int.Text = attr.limit_int;
            txt_itemlimit_leader.Text = attr.limit_leader;

            rdb_sexMALE.Checked = false;
            rdb_sexFEMALE.Checked = false;
            if (attr.limit_sex != null || attr.limit_sex != "")
            {
                if (attr.limit_sex == "sexMALE")
                {
                    rdb_sexMALE.Checked = true;
                    rdb_sexFEMALE.Checked = false;
                }
                else if (attr.limit_sex == "sexFEMALE")
                {
                    rdb_sexMALE.Checked = false;
                    rdb_sexFEMALE.Checked = true;
                }
                else
                {
                    rdb_sexMALE.Checked = false;
                    rdb_sexFEMALE.Checked = false;
                }
            }

            ckb_jobASSASSIN.Checked = false;
            ckb_jobENGINEER.Checked = false;
            ckb_jobWIZARD.Checked = false;
            ckb_jobADVISOR.Checked = false;
            ckb_jobLEADER.Checked = false;
            ckb_jobWARLORD.Checked = false;
            if (attr.limit_job != null && attr.limit_job != "")
            {
                var _limit_job = attr.limit_job.Split(',');
                foreach (var _job in _limit_job)
                {
                    if (_job == "jobWARLORD")
                    {
                        ckb_jobWARLORD.Checked = true;
                    }
                    else if (_job == "jobLEADER")
                    {
                        ckb_jobLEADER.Checked = true;
                    }
                    else if (_job == "jobADVISOR")
                    {
                        ckb_jobADVISOR.Checked = true;
                    }
                    else if (_job == "jobWIZARD")
                    {
                        ckb_jobWIZARD.Checked = true;
                    }
                    else if (_job == "jobENGINEER")
                    {
                        ckb_jobENGINEER.Checked = true;
                    }
                    else if (_job == "jobASSASSIN")
                    {
                        ckb_jobASSASSIN.Checked = true;
                    }
                }
            }
        }
        public int cur_select_index = 0;
        private void button5_Click(object sender, EventArgs e)
        {
            ListViewItem foundItem = this.listView1.FindItemWithText(this.txt_ItemSearch.Text, true, cur_select_index+1);    //参数1：要查找的文本；参数2：是否子项也要查找；参数3：开始查找位置  

            if (foundItem != null)
            {

                this.listView1.TopItem = foundItem;  //定位到该项  
                cur_select_index = foundItem.Index;
                
                //foundItem.Selected = true; ;
                string name = foundItem.SubItems[1].Text;

                bool done = false;
                CItemCtrl.item_attr attr = CItemCtrl.Get_Attr_ByName(name, out done);
                //MessageBox.Show(attr.code);
                Fill_Item_Attr(attr);
            }  
        }

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*.*)|*.*";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = fileDialog.FileName;
                pictureBox1.Image = System.Drawing.Bitmap.FromFile(file);
            }
        }
    }
}
