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
        #region //充值设置
        public struct Recharge_Conf
        {
            public string leftNum;
            public string rightNum;
            public string id1;
            public string name1;
            public string num1;
            public string id2;
            public string name2;
            public string num2;
            public string id3;
            public string name3;
            public string num3;
            public string id4;
            public string name4;
            public string num4;
            public string id5;
            public string name5;
            public string num5;
        };

        private void LoadAutoRchConf()
        {
            //load_Item_List
            string allItemDefines = CItemCtrl.load_Item_Defines(System.AppDomain.CurrentDomain.BaseDirectory + "profile");
            var allItemDefinesArr = allItemDefines.Replace("\t", "").Split(';');

            this.lstv_itemList.Items.Clear();
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
                    this.lstv_itemList.Items.Add(lvi);
                }
            }
            this.lstv_itemList.EndUpdate();  //结束数据处理，UI界面一次性绘制

            cbx_rcgConfNum.SelectedIndex = 0;
            string conf_num = cbx_rcgConfNum.Text;
            OnHandleRcgConfNum_SelectedIndexChanged(conf_num);

            cbx_rcgLevel.SelectedIndex = 0;
            string rcgLevel = "档位：" + cbx_rcgLevel.Text;
            DoRcgLevelSelectedIndexChanged(rcgLevel);
        }

        private void btn_rcgInit_Click(object sender, EventArgs e)
        {
            if (btn_sql.Text == "连接数据库")
            {
                MessageBox.Show("请先连接数据库！");
                return;
            }

            //加载设定的配置
            Recharge_Conf[] conf = new Recharge_Conf[7];
            for (int i = 0; i < 7; i++)
            {
                string rcgLevel = "档位：" + (i + 1);
                string rcgConf = CIniCtrl.ReadIniData(rcgLevel, "rcgConf", "", serverIni);
                string leftNum = CIniCtrl.ReadIniData(rcgLevel, "leftNum", "", serverIni);
                string rightNum = CIniCtrl.ReadIniData(rcgLevel, "rightNum", "", serverIni);

                string id = "";
                string name = "";
                string num = "";
                //读取
                id = CIniCtrl.ReadIniData("Recharge", rcgConf + "_id1", "", serverIni);
                name = CIniCtrl.ReadIniData("Recharge", rcgConf + "_name1", "", serverIni);
                num = CIniCtrl.ReadIniData("Recharge", rcgConf + "_num1", "", serverIni);
                //配置内容
                conf[i].id1 = id == "" ? "0" : id;
                conf[i].name1 = name == "" ? "空" : name;
                conf[i].num1 = num == "" ? "0" : num;

                //读取
                id = CIniCtrl.ReadIniData("Recharge", rcgConf + "_id2", "", serverIni);
                name = CIniCtrl.ReadIniData("Recharge", rcgConf + "_name2", "", serverIni);
                num = CIniCtrl.ReadIniData("Recharge", rcgConf + "_num2", "", serverIni);
                //配置内容
                conf[i].id2 = id == "" ? "0" : id;
                conf[i].name2 = name == "" ? "空" : name;
                conf[i].num2 = num == "" ? "0" : num;

                //读取
                id = CIniCtrl.ReadIniData("Recharge", rcgConf + "_id3", "", serverIni);
                name = CIniCtrl.ReadIniData("Recharge", rcgConf + "_name3", "", serverIni);
                num = CIniCtrl.ReadIniData("Recharge", rcgConf + "_num3", "", serverIni);
                //配置内容
                conf[i].id3 = id == "" ? "0" : id;
                conf[i].name3 = name == "" ? "空" : name;
                conf[i].num3 = num == "" ? "0" : num;

                //读取
                id = CIniCtrl.ReadIniData("Recharge", rcgConf + "_id4", "", serverIni);
                name = CIniCtrl.ReadIniData("Recharge", rcgConf + "_name4", "", serverIni);
                num = CIniCtrl.ReadIniData("Recharge", rcgConf + "_num4", "", serverIni);
                //配置内容
                conf[i].id4 = id == "" ? "0" : id;
                conf[i].name4 = name == "" ? "空" : name;
                conf[i].num4 = num == "" ? "0" : num;

                //读取
                id = CIniCtrl.ReadIniData("Recharge", rcgConf + "_id5", "", serverIni);
                name = CIniCtrl.ReadIniData("Recharge", rcgConf + "_name5", "", serverIni);
                num = CIniCtrl.ReadIniData("Recharge", rcgConf + "_num5", "", serverIni);
                //配置内容
                conf[i].id5 = id == "" ? "0" : id;
                conf[i].name5 = name == "" ? "空" : name;
                conf[i].num5 = num == "" ? "0" : num;

                leftNum = leftNum == "" ? "0" : leftNum;
                rightNum = rightNum == "" ? "-1" : rightNum;
                if (Int32.Parse(leftNum) >= Int32.Parse(rightNum))
                {
                    conf[i].leftNum = "0";
                    conf[i].rightNum = "-1";
                }
                else
                {
                    conf[i].leftNum = leftNum;
                    conf[i].rightNum = rightNum;
                }
            }

            string cmd = "";
            string ret = "";
            //删除[recharge_history]表
            cmd = @"DROP TABLE recharge_history";
            ret = CSGHelper.SqlCommand(cmd);
            if (ret != "success")
            {
                MessageBox.Show("删除[recharge_history]表失败！");
                //return;
            }
            //删除game_acc 的 Account_Insert触发器
            cmd = @"drop trigger Account_Insert ";
            ret = CSGHelper.SqlCommand(cmd);
            if (ret != "success")
            {
                MessageBox.Show("删除game_acc 的 Account_Insert触发器失败！");
                //return;
            }
            //创建[recharge_history]表
            cmd =
@"create table [recharge_history]
(
account varchar(21) PRIMARY KEY,
point int default 0,
time datetime
)";
            ret = CSGHelper.SqlCommand(cmd);
            if (ret != "success")
            {
                MessageBox.Show("创建[recharge_history]表失败！");
                return;
            }
            //同步账户名
            cmd =
@"insert INTO recharge_history(account,point)
SELECT account,point
FROM game_acc";
            ret = CSGHelper.SqlCommand(cmd);
            if (ret != "success")
            {
                MessageBox.Show("同步game_acc账户名和point到recharge_history失败！");
                return;
            }
            //创建game_acc 的 Account_Insert触发器，game_acc有插入时同样插入信息到recharge_history
            cmd =
@"CREATE TRIGGER Account_Insert ON dbo.game_acc 
FOR INSERT 
AS 
DECLARE @account varchar(21)
Select @account=account from inserted 
insert INTO recharge_history(account,point) values (@account,0)";
            ret = CSGHelper.SqlCommand(cmd);
            if (ret != "success")
            {
                MessageBox.Show("创建game_acc 的 Account_Insert触发器失败！");
                return;
            }
            //创建recharge_history 的 Recharge_Update触发器，充值更新point，触发更新game_acc的point
            cmd =
 @"CREATE TRIGGER Recharge_Update ON dbo.recharge_history 
FOR UPDATE 
AS 
DECLARE @account varchar(21)
DECLARE @point int
DECLARE @old_point int
DECLARE @add_point int
DECLARE @old_point_acc int
DECLARE @new_point_acc int
Select @account=account from inserted 
Select @point=point from inserted 
SELECT @old_point=point FROM DELETED
SET @add_point = @point - @old_point

Select @old_point_acc=point from dbo.game_acc where account = @account
set @new_point_acc = @old_point_acc + @add_point
Update dbo.game_acc set point = @new_point_acc where account = @account

DECLARE @cardid varchar(100)
DECLARE @dtDate datetime
set @dtDate = getdate()
SET @cardid = CONVERT(varchar(100), GETDATE(), 21)
PRINT @cardid

DECLARE @DataID1 int
DECLARE @Number1 int
DECLARE @DataID2 int
DECLARE @Number2 int
DECLARE @DataID3 int
DECLARE @Number3 int
DECLARE @DataID4 int
DECLARE @Number4 int
DECLARE @DataID5 int
DECLARE @Number5 int

set @DataID1 = 0
set @Number1 = 0
set @DataID2 = 0
set @Number2 = 0
set @DataID3 = 0
set @Number3 = 0
set @DataID4 = 0
set @Number4 = 0
set @DataID5 = 0
set @Number5 = 0

if (@add_point >= " + conf[0].leftNum + " AND @add_point < " + conf[0].rightNum + @")
	begin
	set @DataID1 = " + conf[0].id1 + @"
	set @Number1 = " + conf[0].num1 + @"
	set @DataID2 = " + conf[0].id2 + @"
	set @Number2 = " + conf[0].num2 + @"
	set @DataID3 = " + conf[0].id3 + @"
	set @Number3 = " + conf[0].num3 + @"
	set @DataID4 = " + conf[0].id4 + @"
	set @Number4 = " + conf[0].num4 + @"
	set @DataID5 = " + conf[0].id5 + @"
	set @Number5 = " + conf[0].num5 + @"
	end
else if (@add_point >= " + conf[1].leftNum + " AND @add_point < " + conf[1].rightNum + @")
	begin
	set @DataID1 = " + conf[1].id1 + @"
	set @Number1 = " + conf[1].num1 + @"
	set @DataID2 = " + conf[1].id2 + @"
	set @Number2 = " + conf[1].num2 + @"
	set @DataID3 = " + conf[1].id3 + @"
	set @Number3 = " + conf[1].num3 + @"
	set @DataID4 = " + conf[1].id4 + @"
	set @Number4 = " + conf[1].num4 + @"
	set @DataID5 = " + conf[1].id5 + @"
	set @Number5 = " + conf[1].num5 + @"
	end
else if (@add_point >= " + conf[2].leftNum + " AND @add_point < " + conf[2].rightNum + @")
	begin
	set @DataID1 = " + conf[2].id1 + @"
	set @Number1 = " + conf[2].num1 + @"
	set @DataID2 = " + conf[2].id2 + @"
	set @Number2 = " + conf[2].num2 + @"
	set @DataID3 = " + conf[2].id3 + @"
	set @Number3 = " + conf[2].num3 + @"
	set @DataID4 = " + conf[2].id4 + @"
	set @Number4 = " + conf[2].num4 + @"
	set @DataID5 = " + conf[2].id5 + @"
	set @Number5 = " + conf[2].num5 + @"
	end
else if (@add_point >= " + conf[3].leftNum + " AND @add_point < " + conf[3].rightNum + @")
	begin
	set @DataID1 = " + conf[3].id1 + @"
	set @Number1 = " + conf[3].num1 + @"
	set @DataID2 = " + conf[3].id2 + @"
	set @Number2 = " + conf[3].num2 + @"
	set @DataID3 = " + conf[3].id3 + @"
	set @Number3 = " + conf[3].num3 + @"
	set @DataID4 = " + conf[3].id4 + @"
	set @Number4 = " + conf[3].num4 + @"
	set @DataID5 = " + conf[3].id5 + @"
	set @Number5 = " + conf[3].num5 + @"
	end
else if (@add_point >= " + conf[4].leftNum + " AND @add_point < " + conf[4].rightNum + @")
	begin
	set @DataID1 = " + conf[4].id1 + @"
	set @Number1 = " + conf[4].num1 + @"
	set @DataID2 = " + conf[4].id2 + @"
	set @Number2 = " + conf[4].num2 + @"
	set @DataID3 = " + conf[4].id3 + @"
	set @Number3 = " + conf[4].num3 + @"
	set @DataID4 = " + conf[4].id4 + @"
	set @Number4 = " + conf[4].num4 + @"
	set @DataID5 = " + conf[4].id5 + @"
	set @Number5 = " + conf[4].num5 + @"
	end
else if (@add_point >= " + conf[5].leftNum + " AND @add_point < " + conf[5].rightNum + @")
	begin
	set @DataID1 = " + conf[5].id1 + @"
	set @Number1 = " + conf[5].num1 + @"
	set @DataID2 = " + conf[5].id2 + @"
	set @Number2 = " + conf[5].num2 + @"
	set @DataID3 = " + conf[5].id3 + @"
	set @Number3 = " + conf[5].num3 + @"
	set @DataID4 = " + conf[5].id4 + @"
	set @Number4 = " + conf[5].num4 + @"
	set @DataID5 = " + conf[5].id5 + @"
	set @Number5 = " + conf[5].num5 + @"
	end
else if (@add_point >= " + conf[6].leftNum + " AND @add_point < " + conf[6].rightNum + @")
	begin
	set @DataID1 = " + conf[6].id1 + @"
	set @Number1 = " + conf[6].num1 + @"
	set @DataID2 = " + conf[6].id2 + @"
	set @Number2 = " + conf[6].num2 + @"
	set @DataID3 = " + conf[6].id3 + @"
	set @Number3 = " + conf[6].num3 + @"
	set @DataID4 = " + conf[6].id4 + @"
	set @Number4 = " + conf[6].num4 + @"
	set @DataID5 = " + conf[6].id5 + @"
	set @Number5 = " + conf[6].num5 + @"
	end

INSERT INTO " + txt_sanvtName.Text + @".dbo.vitem (Account,Disable,Card,Login_time,Get_time,SName,CharName,Type,
DataID1,Number1,DataID2,Number2,DataID3,Number3,DataID4,Number4,DataID5,Number5)
values (@account,0,@cardid,@dtDate,@dtDate,0,0,0,
@DataID1,@Number1,@DataID2,@Number2,@DataID3,@Number3,@DataID4,@Number4,@DataID5,@Number5)";
            ret = CSGHelper.SqlCommand(cmd);

            if (ret == "success")
            {
                string msg = @"当前设定如下:

本服不设定非固定充值，非以下固定充值将以第三方设置比例结算。

(1)范围：" + conf[0].leftNum + "-" + conf[0].rightNum + "额外获得：" + conf[0].name1 + "*" + conf[0].num1 + ";" + conf[0].name2 + "*" + conf[0].num2 + ";" + conf[0].name3 + "*" + conf[0].num3 + ";" + conf[0].name4 + "*" + conf[0].num4 + ";" + conf[0].name5 + "*" + conf[0].num5 + @"；

(2)范围：" + conf[1].leftNum + "-" + conf[1].rightNum + "额外获得：" + conf[1].name1 + "*" + conf[1].num1 + ";" + conf[1].name2 + "*" + conf[1].num2 + ";" + conf[1].name3 + "*" + conf[1].num3 + ";" + conf[1].name4 + "*" + conf[1].num4 + ";" + conf[1].name5 + "*" + conf[1].num5 + @"；

(3)范围：" + conf[2].leftNum + "-" + conf[2].rightNum + "额外获得：" + conf[2].name1 + "*" + conf[2].num1 + ";" + conf[2].name2 + "*" + conf[2].num2 + ";" + conf[2].name3 + "*" + conf[2].num3 + ";" + conf[2].name4 + "*" + conf[2].num4 + ";" + conf[2].name5 + "*" + conf[2].num5 + @"；

(4)范围：" + conf[3].leftNum + "-" + conf[3].rightNum + "额外获得：" + conf[3].name1 + "*" + conf[3].num1 + ";" + conf[3].name2 + "*" + conf[3].num2 + ";" + conf[3].name3 + "*" + conf[3].num3 + ";" + conf[3].name4 + "*" + conf[3].num4 + ";" + conf[3].name5 + "*" + conf[3].num5 + @"；

(5)范围：" + conf[4].leftNum + "-" + conf[4].rightNum + "额外获得：" + conf[4].name1 + "*" + conf[4].num1 + ";" + conf[4].name2 + "*" + conf[4].num2 + ";" + conf[4].name3 + "*" + conf[4].num3 + ";" + conf[4].name4 + "*" + conf[4].num4 + ";" + conf[4].name5 + "*" + conf[4].num5 + @"；

(6)范围：" + conf[5].leftNum + "-" + conf[5].rightNum + "额外获得：" + conf[5].name1 + "*" + conf[5].num1 + ";" + conf[5].name2 + "*" + conf[5].num2 + ";" + conf[5].name3 + "*" + conf[5].num3 + ";" + conf[5].name4 + "*" + conf[5].num4 + ";" + conf[5].name5 + "*" + conf[5].num5 + @"；

(6)范围：" + conf[6].leftNum + "-" + conf[6].rightNum + "额外获得：" + conf[6].name1 + "*" + conf[6].num1 + ";" + conf[6].name2 + "*" + conf[6].num2 + ";" + conf[6].name3 + "*" + conf[6].num3 + ";" + conf[6].name4 + "*" + conf[6].num4 + ";" + conf[6].name5 + "*" + conf[6].num5 + @"；";
                MessageBox.Show(msg);
            }
            else
            {
                MessageBox.Show("创建recharge_history 的 Recharge_Update触发器失败！");
                return;
            }
        }

        //static string cur_slt_id = "";
        //static string cur_slt_name = "";
        static int itemDefSltIndex = 0;
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstv_itemList.SelectedIndices != null && lstv_itemList.SelectedIndices.Count > 0)
            {
                lstv_itemList.Items[itemDefSltIndex].BackColor = Color.Transparent;
                itemDefSltIndex = this.lstv_itemList.SelectedItems[0].Index;
                lstv_itemList.Items[itemDefSltIndex].BackColor = Color.Pink;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            string cur_slt_id = lstv_itemList.Items[itemDefSltIndex].SubItems[0].Text;
            string cur_slt_name = lstv_itemList.Items[itemDefSltIndex].SubItems[1].Text;

            if (cur_slt_id != "" && cur_slt_name != "")
            {
                if (cbx_confNum.SelectedIndex == 0)
                {
                    txt_confNum1.Text = "1";
                    txt_confName1.Text = cur_slt_name;
                    txt_confId1.Text = cur_slt_id;
                }
                else if (cbx_confNum.SelectedIndex == 1)
                {
                    txt_confNum2.Text = "1";
                    txt_confName2.Text = cur_slt_name;
                    txt_confId2.Text = cur_slt_id;
                }
                else if (cbx_confNum.SelectedIndex == 2)
                {
                    txt_confNum3.Text = "1";
                    txt_confName3.Text = cur_slt_name;
                    txt_confId3.Text = cur_slt_id;
                }
                else if (cbx_confNum.SelectedIndex == 3)
                {
                    txt_confNum4.Text = "1";
                    txt_confName4.Text = cur_slt_name;
                    txt_confId4.Text = cur_slt_id;
                }
                else if (cbx_confNum.SelectedIndex == 4)
                {
                    txt_confNum5.Text = "1";
                    txt_confName5.Text = cur_slt_name;
                    txt_confId5.Text = cur_slt_id;
                }
            }
        }

        private void btn_reLoadList_Click(object sender, EventArgs e)
        {
            LoadAutoRchConf();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //ListViewItem foundItem = this.listView1.FindItemWithText(this.txt_srchItemName.Text, true, cur_select_index + 1);    //参数1：要查找的文本；参数2：是否子项也要查找；参数3：开始查找位置  
            ListViewItem foundItem = null;
            lstv_itemList.Items[itemDefSltIndex].BackColor = Color.Transparent;
            int i = itemDefSltIndex + 1;
            if (i == lstv_itemList.Items.Count)
            {
                i = 0;
            }
            while (i != itemDefSltIndex)
            {
                if (lstv_itemList.Items[i].SubItems[1].Text.Contains(txt_srchItemName.Text) || lstv_itemList.Items[i].Text == txt_srchItemName.Text)
                {
                    foundItem = lstv_itemList.Items[i];
                }
                i++;
                if (i == lstv_itemList.Items.Count)
                {
                    i = 0;
                }
            }

            if (foundItem != null)
            {
                this.lstv_itemList.TopItem = foundItem;  //定位到该项
                lstv_itemList.Items[foundItem.Index].Focused = true;
                lstv_itemList.Items[foundItem.Index].Selected = true;
                foundItem.BackColor = Color.Pink;
                itemDefSltIndex = foundItem.Index;
            }
        }

        private void btn_regConfSave_Click(object sender, EventArgs e)
        {
            serverIni = System.AppDomain.CurrentDomain.BaseDirectory + "配置文件.ini";

            //配置编号
            string conf_num = cbx_rcgConfNum.Text;

            string id = "";
            string name = "";
            string num = "";
            //配置内容
            id = txt_confId1.Text;
            name = txt_confName1.Text;
            num = txt_confNum1.Text;
            //保存
            CIniCtrl.WriteIniData("Recharge", conf_num + "_id1", id, serverIni);
            CIniCtrl.WriteIniData("Recharge", conf_num + "_name1", name, serverIni);
            CIniCtrl.WriteIniData("Recharge", conf_num + "_num1", num, serverIni);

            //配置内容
            id = txt_confId2.Text;
            name = txt_confName2.Text;
            num = txt_confNum2.Text;
            //保存
            CIniCtrl.WriteIniData("Recharge", conf_num + "_id2", id, serverIni);
            CIniCtrl.WriteIniData("Recharge", conf_num + "_name2", name, serverIni);
            CIniCtrl.WriteIniData("Recharge", conf_num + "_num2", num, serverIni);

            //配置内容
            id = txt_confId3.Text;
            name = txt_confName3.Text;
            num = txt_confNum3.Text;
            //保存
            CIniCtrl.WriteIniData("Recharge", conf_num + "_id3", id, serverIni);
            CIniCtrl.WriteIniData("Recharge", conf_num + "_name3", name, serverIni);
            CIniCtrl.WriteIniData("Recharge", conf_num + "_num3", num, serverIni);

            //配置内容
            id = txt_confId4.Text;
            name = txt_confName4.Text;
            num = txt_confNum4.Text;
            //保存
            CIniCtrl.WriteIniData("Recharge", conf_num + "_id4", id, serverIni);
            CIniCtrl.WriteIniData("Recharge", conf_num + "_name4", name, serverIni);
            CIniCtrl.WriteIniData("Recharge", conf_num + "_num4", num, serverIni);

            //配置内容
            id = txt_confId5.Text;
            name = txt_confName5.Text;
            num = txt_confNum5.Text;
            //保存
            CIniCtrl.WriteIniData("Recharge", conf_num + "_id5", id, serverIni);
            CIniCtrl.WriteIniData("Recharge", conf_num + "_name5", name, serverIni);
            CIniCtrl.WriteIniData("Recharge", conf_num + "_num5", num, serverIni);
        }

        private void cbx_rcgConfNum_SelectedIndexChanged(object sender, EventArgs e)
        {
            //配置编号
            string conf_num = cbx_rcgConfNum.Text;
            OnHandleRcgConfNum_SelectedIndexChanged(conf_num);
            cbx_rcgConf.SelectedIndex = cbx_rcgConfNum.SelectedIndex;
        }

        private void OnHandleRcgConfNum_SelectedIndexChanged(string conf_num)
        {
            serverIni = System.AppDomain.CurrentDomain.BaseDirectory + "配置文件.ini";
            string id = "";
            string name = "";
            string num = "";

            //读取
            id = CIniCtrl.ReadIniData("Recharge", conf_num + "_id1", "", serverIni);
            name = CIniCtrl.ReadIniData("Recharge", conf_num + "_name1", "", serverIni);
            num = CIniCtrl.ReadIniData("Recharge", conf_num + "_num1", "", serverIni);
            //配置内容
            txt_confId1.Text = id;
            txt_confName1.Text = name;
            txt_confNum1.Text = num;

            //读取
            id = CIniCtrl.ReadIniData("Recharge", conf_num + "_id2", "", serverIni);
            name = CIniCtrl.ReadIniData("Recharge", conf_num + "_name2", "", serverIni);
            num = CIniCtrl.ReadIniData("Recharge", conf_num + "_num2", "", serverIni);
            //配置内容
            txt_confId2.Text = id;
            txt_confName2.Text = name;
            txt_confNum2.Text = num;

            //读取
            id = CIniCtrl.ReadIniData("Recharge", conf_num + "_id3", "", serverIni);
            name = CIniCtrl.ReadIniData("Recharge", conf_num + "_name3", "", serverIni);
            num = CIniCtrl.ReadIniData("Recharge", conf_num + "_num3", "", serverIni);
            //配置内容
            txt_confId3.Text = id;
            txt_confName3.Text = name;
            txt_confNum3.Text = num;

            //读取
            id = CIniCtrl.ReadIniData("Recharge", conf_num + "_id4", "", serverIni);
            name = CIniCtrl.ReadIniData("Recharge", conf_num + "_name4", "", serverIni);
            num = CIniCtrl.ReadIniData("Recharge", conf_num + "_num4", "", serverIni);
            //配置内容
            txt_confId4.Text = id;
            txt_confName4.Text = name;
            txt_confNum4.Text = num;

            //读取
            id = CIniCtrl.ReadIniData("Recharge", conf_num + "_id5", "", serverIni);
            name = CIniCtrl.ReadIniData("Recharge", conf_num + "_name5", "", serverIni);
            num = CIniCtrl.ReadIniData("Recharge", conf_num + "_num5", "", serverIni);
            //配置内容
            txt_confId5.Text = id;
            txt_confName5.Text = name;
            txt_confNum5.Text = num;
        }

        private void DoRcgLevelSelectedIndexChanged(string rcgLevel)
        {
            string rcgConf = CIniCtrl.ReadIniData(rcgLevel, "rcgConf", "", serverIni);
            string leftNum = CIniCtrl.ReadIniData(rcgLevel, "leftNum", "", serverIni);
            string rightNum = CIniCtrl.ReadIniData(rcgLevel, "rightNum", "", serverIni);

            cbx_rcgConf.Text = rcgConf;
            cbx_rcgConfNum.Text = rcgConf;
            txt_betmLeft.Text = leftNum;
            txt_betmRight.Text = rightNum;
        }
        private void cbx_rcgLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            string rcgLevel = "档位：" + cbx_rcgLevel.Text;
            DoRcgLevelSelectedIndexChanged(rcgLevel);
        }

        private void btn_rcgLevelSave_Click(object sender, EventArgs e)
        {
            //获取档位
            string rcgLevel = "档位：" + cbx_rcgLevel.Text;
            //获取配置
            string rcgConf = cbx_rcgConf.Text;
            OnHandleRcgConfNum_SelectedIndexChanged(rcgConf);

            //获取间隔
            string leftNum = txt_betmLeft.Text;
            string rightNum = txt_betmRight.Text;

            //写配置
            CIniCtrl.WriteIniData(rcgLevel, "rcgConf", rcgConf, serverIni);
            CIniCtrl.WriteIniData(rcgLevel, "leftNum", leftNum, serverIni);
            CIniCtrl.WriteIniData(rcgLevel, "rightNum", rightNum, serverIni);

            //根据当前配置更新设定

        }

        private void cbx_rcgConf_SelectedIndexChanged(object sender, EventArgs e)
        {
            string rcgConf = cbx_rcgConf.Text;
            cbx_rcgConfNum.SelectedIndex = cbx_rcgConf.SelectedIndex;
            OnHandleRcgConfNum_SelectedIndexChanged(rcgConf);
        }

        private void txt_sanvtName_TextChanged(object sender, EventArgs e)
        {
            //写入
            CIniCtrl.WriteIniData("Server", "sanvtName", txt_sanvtName.Text, serverIni);
        }

        #endregion

    }
}
