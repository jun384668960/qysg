using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SGDataHelper;
using System.IO;
using Common;

namespace 合区工具
{
    public partial class SGDataPlus : Form
    {
        public SGDataPlus()
        {
            InitializeComponent();
        }


        private CPlayerCtrl m_MainCPlayerCtrl = new CPlayerCtrl();
        private CPlayerCtrl m_SubCPlayerCtrl = new CPlayerCtrl();
        private CUidCtrl m_MainCUidCtrl = new CUidCtrl();
        private CSGHelper m_MainSQLConn = new CSGHelper();
        private CSGHelper m_SubSQLConn = new CSGHelper();

        private CSkillsCtrl m_MainCSkillsCtrl = new CSkillsCtrl();
        private CSkillsCtrl m_SubCSkillsCtrl = new CSkillsCtrl();
        private CItemCtrl m_MainCItemCtrl = new CItemCtrl();
        private CItemCtrl m_SubCItemCtrl = new CItemCtrl();
        private CSoldierCtrl m_MainCSoldierCtrl = new CSoldierCtrl();
        private CSoldierCtrl m_SubCSoldierCtrl = new CSoldierCtrl();

        private static bool sqlConn = false;
        private static bool sqlPlus = false;
        private static bool mainDirSet = false;
        private static bool subDirSet = false;
        private static bool m_bCheckRegist = true;
        private void btn_SqlConn_Click(object sender, EventArgs e)
        {
            if (!sqlConn)
            {
                string conn_str = "";
                bool ret = false;
                //连接主数据库
                conn_str = "Data Source = " + txt_SqlAddr.Text + "," + txt_SqlPort.Text + "; Initial Catalog = " + txt_MainSqlAccount.Text + "; User Id = " + txt_SqlLoginName.Text + "; Password = " + txt_SqlLoginPwd.Text + ";";
                ret = m_MainSQLConn.SqlConn(conn_str);
                if (!ret)
                {
                    MessageBox.Show("连接主数据库失败！");
                    return;
                }
                //连接副数据库
                conn_str = "Data Source = " + txt_SqlAddr.Text + "," + txt_SqlPort.Text + "; Initial Catalog = " + txt_SubSqlAccount.Text + "; User Id = " + txt_SqlLoginName.Text + "; Password = " + txt_SqlLoginPwd.Text + ";";
                ret = m_SubSQLConn.SqlConn(conn_str);
                if (!ret)
                {
                    m_MainSQLConn.SqlClose();
                    MessageBox.Show("连接副数据库失败！");
                    return;
                }

                sqlConn = true;
                btn_SqlConn.Text = "断开连接";
                MessageBox.Show("数据库连接成功");
                
            }
            else 
            {
                m_MainSQLConn.SqlClose();
                m_SubSQLConn.SqlClose();

                sqlConn = false;
                btn_SqlConn.Text = "连接数据库";
                MessageBox.Show("数据库断开成功");
            }
            
        }

        private void btn_SqlPlus_Click(object sender, EventArgs e)
        {
            //当两个连接都成功的时候
            if(!sqlConn)
            {
                MessageBox.Show("数据库未连接成功，无法执行此操作！");
                return;
            }

            lbl_SqlPlusStatus.Text = "正在合并..";
            //执行合并的SQL语句 -- 此操作只合并game_acc game_charslot 不更新game_acc_id
            //所有，如果后面palyer.dat失败，副档的角色将出错

            bool ret = DoSQLPlus(txt_MainSqlAccount.Text, txt_SubSqlAccount.Text);
            if (ret)
            {
                MessageBox.Show("合并数据库完成，Player.dat和uid档案合并成功后，此处的主Account就是合并后的数据库");
                btn_SqlPlus.Enabled = false;
                lbl_SqlPlusStatus.Text = "合并成功";
                sqlPlus = true;
            }
            else 
            {
                MessageBox.Show("合并数据库失败，请恢复数据库后重启工具重试");
                btn_SqlPlus.Enabled = false;
                lbl_SqlPlusStatus.Text = "合并失败";
                sqlPlus = false;
            }
        }
        private bool DoSQLPlus(string mainAccount, string subAccount)
        {
            bool ret = false;
            string sqlLine = "";

            //创建game_charslot_new
            sqlLine = @"CREATE TABLE " + mainAccount + @".[dbo].[game_charslot_new](
	[account] [varchar](21) NOT NULL,
	[server_id] [int] NOT NULL,
	[char_slot] [int] NOT NULL
)";
            ret = m_MainSQLConn.SqlCommand(sqlLine);
            if (!ret)
            {
                MessageBox.Show("创建game_charslot_new 失败");
                return false;
            }
//            //创建game_charslot_bak
//            sqlLine = @"CREATE TABLE " + mainAccount + @".[dbo].[game_charslot_bak](
//	[account] [varchar](21) NOT NULL,
//	[server_id] [int] NOT NULL,
//	[char_slot] [int] NOT NULL
//)";
//            ret = m_MainSQLConn.SqlCommand(sqlLine);
//            if (!ret)
//            {
//                MessageBox.Show("创建game_charslot_bak 失败");
//                return false;
//            }
            //创建game_acc_new
            sqlLine = @"create table " + mainAccount + @".[dbo].[game_acc_new]
(
	[account] [varchar](21) NOT NULL PRIMARY KEY,
	[password] [varchar](21) NOT NULL,
	[password2] [varchar](36) NULL,
	[duedate] [datetime] NULL,
	[enable] [int] NULL,
	[lock_duedate] [datetime] NULL,
	[logout_time] [datetime] NULL,
	[ip] [varchar](21) NULL,
	[create_time] [datetime] NULL,
	[privilege] [int] NULL,
	[status] [int] NULL,
	[sec_pwd] [varchar](36) NULL,
	[first_ip] [varchar](21) NULL,
	[point] [int] NULL,
	[trade_psw] [varchar](32) NULL,
	[IsAdult] [bit] NULL,
	[OnlineTime] [int] NULL,
	[OfflineTime] [int] NULL,
	[LastLoginTime] [datetime] NULL,
	[LastLogoutTime] [datetime] NULL,
)";
            ret = m_MainSQLConn.SqlCommand(sqlLine);
            if (!ret)
            {
                MessageBox.Show("创建game_acc_new 失败");
                return false;
            }
            //创建game_acc_bak
//            sqlLine = @"create table " + mainAccount + @".[dbo].[game_acc_bak]
//(
//	[account] [varchar](21) NOT NULL PRIMARY KEY,
//	[password] [varchar](21) NOT NULL,
//	[password2] [varchar](36) NULL,
//	[duedate] [datetime] NULL,
//	[enable] [int] NULL,
//	[lock_duedate] [datetime] NULL,
//	[logout_time] [datetime] NULL,
//	[ip] [varchar](21) NULL,
//	[create_time] [datetime] NULL,
//	[privilege] [int] NULL,
//	[status] [int] NULL,
//	[sec_pwd] [varchar](36) NULL,
//	[first_ip] [varchar](21) NULL,
//	[point] [int] NULL,
//	[trade_psw] [varchar](32) NULL,
//	[IsAdult] [bit] NULL,
//	[OnlineTime] [int] NULL,
//	[OfflineTime] [int] NULL,
//	[LastLoginTime] [datetime] NULL,
//	[LastLogoutTime] [datetime] NULL,
//)";
//            ret = m_MainSQLConn.SqlCommand(sqlLine);
//            if (!ret)
//            {
//                MessageBox.Show("创建game_acc_bak 失败");
//                return false;
//            }

            //创建game_acc_id_bak
//            sqlLine = @"CREATE TABLE " + mainAccount + @".[dbo].[game_acc_id_bak](
//	[id] [int]  NOT NULL,
//	[account] [varchar](21) NOT NULL
//)";
//            ret = m_MainSQLConn.SqlCommand(sqlLine);
//            if (!ret)
//            {
//                MessageBox.Show("创建game_acc_id_bak 失败");
//                return false;
//            }

            //备份表
            //sqlLine = @"insert INTO " + mainAccount + @".[dbo].game_acc_id_bak SELECT * FROM " + mainAccount + @".[dbo].game_acc_id";
            //ret = m_MainSQLConn.SqlCommand(sqlLine);
            //if (!ret)
            //{
            //    MessageBox.Show("备份表 game_acc_id 到 game_acc_id_bak 失败");
            //    return false;
            //}
            //sqlLine = @"insert INTO " + mainAccount + @".[dbo].game_acc_bak SELECT * FROM " + mainAccount + @".[dbo].game_acc";
            //ret = m_MainSQLConn.SqlCommand(sqlLine);
            //if (!ret)
            //{
            //    MessageBox.Show("备份表 game_acc 到 game_acc_bak 失败");
            //    return false;
            //}
            //sqlLine = @"insert INTO " + mainAccount + @".[dbo].game_charslot_bak SELECT * FROM " + mainAccount + @".[dbo].game_charslot";
            //ret = m_MainSQLConn.SqlCommand(sqlLine);
            //if (!ret)
            //{
            //    MessageBox.Show("备份表 game_charslot 到 game_charslot_bak 失败");
            //    return false;
            //}

            //缓存表
            sqlLine = @"insert INTO " + mainAccount + @".[dbo].game_acc_new SELECT * FROM " + mainAccount + @".[dbo].game_acc";
            ret = m_MainSQLConn.SqlCommand(sqlLine);
            if (!ret)
            {
                MessageBox.Show("数据 game_acc 到 game_acc_new 缓存失败");
                return false;
            }
            sqlLine = @"insert INTO " + mainAccount + @".[dbo].game_charslot_new SELECT * FROM " + mainAccount + @".[dbo].game_charslot";
            ret = m_MainSQLConn.SqlCommand(sqlLine);
            if (!ret)
            {
                MessageBox.Show("数据 game_charslot 到 game_charslot_new 缓存失败");
                return false;
            }
            
            //融合到缓存表
            sqlLine = @"DECLARE
    @account AS [varchar](21),
    @password AS [varchar](21),
    @password2 AS [varchar](36),
    @privilege AS [int],
    @sec_pwd AS [varchar](36),
    @first_ip AS [varchar](21),
    @point AS [int];
    
DECLARE C_Employees CURSOR FAST_FORWARD FOR
    SELECT [account],[password],[password2],[privilege],[sec_pwd],[first_ip],[point] 
    FROM " + subAccount + @".dbo.game_acc
    ORDER BY [account];
    
OPEN C_Employees;

FETCH NEXT FROM C_Employees INTO @account,@password,@password2,@privilege,@sec_pwd,@first_ip,@point;

WHILE @@FETCH_STATUS=0
BEGIN
    if not exists(select 1 from " + mainAccount + @".dbo.game_acc_new where ([account] = @account))
    begin
		insert into " + mainAccount + @".dbo.game_acc_new(account,password,password2,enable,privilege,sec_pwd,first_ip,point) 
		values(@account,@password,@password2,1,1,@sec_pwd,@first_ip,@point)
    end
	else
	begin
		--update point
		DECLARE @point_new AS [int],
				@point_main AS [int],
				@point_sub AS [int];
		select @point_main = point from " + mainAccount + @".dbo.game_acc_new where ([account] = @account)
		select @point_sub = point from " + subAccount + @".dbo.game_acc where ([account] = @account)
		SET @point_new = @point_main + @point_sub
		UPDATE " + mainAccount + @".dbo.game_acc_new set point = @point_new where ([account] = @account)
		
		DECLARE @char_slot_new AS [int],
				@char_slot_main AS [int],
				@char_slot_sub AS [int];
				
		select @char_slot_main = char_slot from " + mainAccount + @".dbo.game_charslot_new where ([account] = @account)
		select @char_slot_sub = char_slot from " + subAccount + @".dbo.game_charslot where ([account] = @account)
		SET @char_slot_new = @char_slot_main + @char_slot_sub
        if @char_slot_new > 2
		begin
			SET @char_slot_new = 2
		end
		UPDATE " + mainAccount + @".dbo.game_charslot_new set char_slot = @char_slot_new where ([account] = @account)
		
	end

    FETCH NEXT FROM C_Employees INTO @account,@password,@password2,@privilege,@sec_pwd,@first_ip,@point;
END

CLOSE C_Employees;


DEALLOCATE C_Employees;";
            ret = m_MainSQLConn.SqlCommand(sqlLine);
            if (!ret)
            {
                MessageBox.Show("融合数据失败");
                return false;
            }

            //清空原有表，把缓存表数据填充
            sqlLine = @"TRUNCATE  TABLE  " + mainAccount + @".[dbo].game_acc";
            ret = m_MainSQLConn.SqlCommand(sqlLine);
            if (!ret)
            {
                MessageBox.Show("清空原有表game_acc失败");
                return false;
            }
            sqlLine = @"insert INTO " + mainAccount + @".[dbo].game_acc SELECT * FROM " + mainAccount + @".[dbo].game_acc_new";
            ret = m_MainSQLConn.SqlCommand(sqlLine);
            if (!ret)
            {
                MessageBox.Show("game_acc_new 回填 game_acc失败");
                return false;
            }

            sqlLine = @"TRUNCATE  TABLE  " + mainAccount + @".[dbo].game_charslot";
            ret = m_MainSQLConn.SqlCommand(sqlLine);
            if (!ret)
            {
                MessageBox.Show("清空原有表game_charslot失败");
                return false;
            }
            sqlLine = @"insert INTO " + mainAccount + @".[dbo].game_charslot SELECT * FROM " + mainAccount + @".[dbo].game_charslot_new";
            ret = m_MainSQLConn.SqlCommand(sqlLine);
            if (!ret)
            {
                MessageBox.Show("game_charslot_new 回填 game_charslot失败");
                return false;
            }

            //删除缓存表
            sqlLine = @"DROP TABLE " + mainAccount + @".[dbo].game_acc_new";
            ret = m_MainSQLConn.SqlCommand(sqlLine);
            if (!ret)
            {
                MessageBox.Show("删除缓存表game_acc_new失败");
                return false;
            }
            sqlLine = @"DROP TABLE " + mainAccount + @".[dbo].game_charslot_new";
            ret = m_MainSQLConn.SqlCommand(sqlLine);
            if (!ret)
            {
                MessageBox.Show("删除缓存表game_charslot_new失败");
                return false;
            }

            return ret;
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
            else
            {
                return;
            }

            txt_MainSaveDir.Text = foldPath;
            m_MainCPlayerCtrl.SetForder(foldPath);
            m_MainCUidCtrl.SetForder(foldPath);
            m_MainCSkillsCtrl.SetForder(foldPath);
            m_MainCItemCtrl.SetForder(foldPath);
            m_MainCSoldierCtrl.SetForder(foldPath);

            mainDirSet = true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.Description = "请选择文件路径";
            string foldPath = "";
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                foldPath = folderDialog.SelectedPath;
            }
            else
            {
                return;
            }

            txt_SubSaveDir.Text = foldPath;
            m_SubCPlayerCtrl.SetForder(foldPath);
            m_SubCSkillsCtrl.SetForder(foldPath);
            m_SubCItemCtrl.SetForder(foldPath);
            m_SubCSoldierCtrl.SetForder(foldPath);

            subDirSet = true;
        }

        private void btn_SavePlus_Click(object sender, EventArgs e)
        {
            //保证数据库已经连接
            if (!sqlConn)
            {
                MessageBox.Show("请先链接数据库！");
                return;
            }

            if (!subDirSet || !mainDirSet)
            {
                MessageBox.Show("请先选择主、副档路径！");
                return;
            }

            if (!sqlPlus)
            {
                MessageBox.Show("请合并数据库！");
                return;
            }
            lbl_SavePlusStatus.Text = "正在合并..";

            bool ret = false;
            //合并player.dat
            m_MainCPlayerCtrl.LoadPlayerInfos();
            m_SubCPlayerCtrl.LoadPlayerInfos();
            m_MainCUidCtrl.LoadInfo();

            //List<AccAttr> mainList = m_MainCPlayerCtrl.GetPlayersAttrList();
            List<AccAttr> subList = m_SubCPlayerCtrl.GetPlayersAttrList();
            UInt32 startIndex = 0;
            UInt32 startUid = 0;
            UInt32 starAccId = 0;
            startIndex = m_MainCPlayerCtrl.GetLastIndex();
            startUid = m_MainCUidCtrl.GetLastUid();
            //最好从数据库获取
            starAccId = m_MainCUidCtrl.GetLastAccId();


            if (startIndex < 0 || startUid < 0 || starAccId < 0)
            {
                lbl_SavePlusStatus.Text = "正在失败..";
                MessageBox.Show("发送错误！");
                return;
            }

            try 
            {
                foreach (var acc in subList)
                {
                    AccAttr newAcc = new AccAttr();
                    newAcc = acc;

                    //重新计算nIndex
                    startIndex = startIndex + 1;
                    newAcc.nIndex = startIndex;

                    //查询当前账户是否已经存在
                    bool accountExit = false;
                    AccAttr player;
                    accountExit = m_MainCPlayerCtrl.AccountExit(newAcc.Account, out player);
                    if (accountExit)
                    {//如果当前账户已经存在，则添加uid的账户指向
                        //遍历uid列表，找到该数据，将此nIndex加入对应的角色位
                        m_MainCUidCtrl.AddSubPlayer(player.nAccId, startIndex);
                        newAcc.nAccId = player.nAccId;
                    }
                    else
                    {
                        //如果不存在 则添加uid 并且acc_id插入数据库
                        startUid = startUid + 1;
                        starAccId = starAccId + 1;
                        newAcc.nAccId = starAccId;
                        //uid列表添加, 使能第一角色位, uid index + 1， 生成的acc_id对应加入且加入数据库
                        m_MainCUidCtrl.AddPlayerUid(newAcc.nAccId, startUid, startIndex);

                        //starAccId插入数据库
                        string sqlLine = "";
                        string sqlAccount = System.Text.Encoding.ASCII.GetString(newAcc.Account);
                        sqlAccount = sqlAccount.Replace("\0", "");
                        sqlLine = @"set identity_insert " + txt_MainSqlAccount.Text + @".dbo.game_acc_id ON
  insert into " + txt_MainSqlAccount.Text + ".dbo.game_acc_id(id,account) values (" + starAccId + ",'" + sqlAccount + @"')
  set identity_insert " + txt_MainSqlAccount.Text + ".dbo.game_acc_id OFF";
                        ret = m_MainSQLConn.SqlCommand(sqlLine);
                        if (!ret)
                        {
                            btn_SavePlus.Enabled = false;
                            lbl_SavePlusStatus.Text = "正在失败..";
                            MessageBox.Show("Acc_Id插入数据库失败!，请恢复数据库和档案后重启工具重试");
                            return;
                        }
                    }

                    //判断当前角色名字是否被使用
                    ret = m_MainCPlayerCtrl.NameExit(newAcc.Name);
                    if (ret)
                    {//存在，则修改此名字
                        //这里需要保证长度问题
                        newAcc.Name = CPlayerCtrl.ReName(newAcc.Name);
                    }

                    //可装备次数 + 1
                    newAcc = CPlayerCtrl.DoAddEquipIimes(newAcc);
                   
                    //添加
                    m_MainCPlayerCtrl.AddPlayer(newAcc);
                }

                ret = m_MainCPlayerCtrl.SavePlayerInfos();
                m_MainCUidCtrl.SaveInfos();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                ret = false;
            }

            try
            { 
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                ret = false;
            }

            if (!ret)
            {
                btn_SavePlus.Enabled = false;
                lbl_SavePlusStatus.Text = "合并失败..";
                MessageBox.Show("合并Player.dat失败！请恢复数据库和档案后重启工具重试");
            }
            else 
            {
                btn_SavePlus.Enabled = false;
                lbl_SavePlusStatus.Text = "合并成功..";
                MessageBox.Show("合并Player.dat成功,生成的player.dat和uid.dat保存在new目录下！");
                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!subDirSet || !mainDirSet)
            {
                MessageBox.Show("请先选择主、副档路径！");
                return;
            }
            m_MainCSkillsCtrl.LoadInfos();
            m_SubCSkillsCtrl.LoadInfos();
            List<SkillAttr> subList = m_SubCSkillsCtrl.GetSkillAttrList();
            foreach (var skills in subList)
            {
                m_MainCSkillsCtrl.AddPlayerSkill(skills);
            }

            bool ret = m_MainCSkillsCtrl.SaveInfos();
            if (!ret)
            {
                MessageBox.Show("合并失败！");
            }
            else
            {
                MessageBox.Show("合并成功，生成的skill.dat保存在new目录下！");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!subDirSet || !mainDirSet)
            {
                MessageBox.Show("请先选择主、副档路径！");
                return;
            }
            m_MainCItemCtrl.LoadInfos();
            m_SubCItemCtrl.LoadInfos();
            List<ItemAttr> subList = m_SubCItemCtrl.GetItemAttrList();
            foreach (var items in subList)
            {
                //可装备次数+1
                ItemAttr newItems = CItemCtrl.DoAddEquipIimes(items);
                m_MainCItemCtrl.AddPlayerItems(newItems);
            }

            bool ret = m_MainCItemCtrl.SaveInfos();
            if (!ret)
            {
                MessageBox.Show("合并失败！");
            }
            else 
            {
                MessageBox.Show("合并成功，生成的item.dat保存在new目录下！");
            }
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!subDirSet || !mainDirSet)
            {
                MessageBox.Show("请先选择主、副档路径！");
                return;
            }
            m_MainCSoldierCtrl.LoadInfos();
            m_SubCSoldierCtrl.LoadInfos();
            List<SoldiersAttr> subList = m_SubCSoldierCtrl.GetSoldiersAttrList();
            foreach (var soldiers in subList)
            {
                m_MainCSoldierCtrl.AddPlayerSoldiers(soldiers);
            }

            bool ret = m_MainCSoldierCtrl.SaveInfos();
            if (!ret)
            {
                MessageBox.Show("合并失败！");
            }
            else
            {
                MessageBox.Show("合并成功，生成的npc.dat保存在new目录下！");
            }
        }

        private void SGDataPlus_Load(object sender, EventArgs e)
        {
            //激活检测
            if(m_bCheckRegist)
            {
                string endTime = "";
                bool ret = RegistHelper.CheckRegist(out endTime);
                if (!ret)
                {
                    if (endTime != "")
                    {
                        MessageBox.Show("软件已经过期，请将目录下的ComputerInfo.key文件发给软件提供者，以获取使用权限！");
                        this.Text = this.Text + "  - 已到期 到期时间:" + endTime + " (联系QQ：384668960)";
                    }
                    else
                    {
                        MessageBox.Show("软件尚未激活，请将目录下的ComputerInfo.key文件发给软件提供者，以获取使用权限！");
                        this.Text = this.Text + "  - 未激活" + " (联系QQ：384668960)";
                    }
                    groupBox1.Enabled = false;
                    groupBox2.Enabled = false;
                    return;
                }
                else
                {
                    this.Text = this.Text + "  - 已激活 到期时间:" + endTime + " (联系QQ：384668960)";
                }
            }
            

            if (!Directory.Exists("new"))//如果不存在就创建文件夹
            {
                Directory.CreateDirectory("new");
            }
        }
    }
}
