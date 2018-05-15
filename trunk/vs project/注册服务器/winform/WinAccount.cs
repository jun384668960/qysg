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
        #region //账户管理
        private void btn_AccountReflush_Click(object sender, EventArgs e)
        {
            int count = 0;
            CPlayerCtrl.LoadPlayerInfos(txt_svrForder.Text + "\\DataBase\\saves\\players.dat", true);
            try
            {
                string conn_str = "Data Source = " + sql_srvAddr + "," + sql_srvPort + "; Initial Catalog = " + sqlAccountName + "; User Id = " + sql_srvUser + "; Password = " + sql_srvPwd + ";";
                SqlConnection con = new SqlConnection(conn_str);
                con.Open();
                //执行con对象的函数，返回一个SqlCommand类型的对象
                SqlCommand cmd = con.CreateCommand();
                //把输入的数据拼接成sql语句，并交给cmd对象
                cmd.CommandText = "SELECT * FROM [Account_hcsg].[dbo].[game_acc]";

                //用cmd的函数执行语句，返回SqlDataReader类型的结果dr,dr就是返回的结果集（也就是数据库中查询到的表数据）
                SqlDataReader dr = cmd.ExecuteReader();
                //用dr的read函数，每执行一次，返回一个包含下一行数据的集合dr
                lstv_Account.Items.Clear();
                while (dr.Read())
                {
                    //构建一个ListView的数据，存入数据库数据，以便添加到listView1的行数据中
                    ListViewItem lt = new ListViewItem();
                    //将数据库数据转变成ListView类型的一行数据
                    lt.Text = dr["account"].ToString();
                    lt.SubItems.Add(dr["password"].ToString());
                    lt.SubItems.Add(CPlayerCtrl.GetNameByAcc(lt.Text));
                    lt.SubItems.Add(dr["enable"].ToString());
                    lt.SubItems.Add(dr["privilege"].ToString());
                    lt.SubItems.Add(dr["point"].ToString());
                    lt.SubItems.Add(dr["ip"].ToString());
                    lt.SubItems.Add(dr["LastLoginTime"].ToString());
                    lt.SubItems.Add(dr["LastLogoutTime"].ToString());
                    //将lt数据添加到listView1控件中
                    lstv_Account.Items.Add(lt);
                    //账户 密码 角色 状态 权限 代币 ip 登入时间 登出时间
                    count++;
                }
                lstv_Account.EndUpdate();
                con.Close();
            }
            catch (Exception ex)
            {

            }
            lbl_AccountCount.Text = "帐号总数：" + count.ToString();
        }

        private void lstv_Account_SelectedIndexChanged(object sender, EventArgs e)
        {
            //给各个控件赋值
            if (lstv_Account.SelectedIndices != null && lstv_Account.SelectedIndices.Count > 0)
            {
                lstv_Account.Items[xbAccountInfoSltIndex].BackColor = Color.Transparent;
                xbAccountInfoSltIndex = this.lstv_Account.SelectedItems[0].Index;
                lstv_Account.Items[xbAccountInfoSltIndex].BackColor = Color.Pink;

                txt_AcountName.Text = lstv_Account.SelectedItems[0].SubItems[0].Text;
                txt_AcountPwd.Text = lstv_Account.SelectedItems[0].SubItems[1].Text;
                txt_AcountPoint.Text = lstv_Account.SelectedItems[0].SubItems[5].Text;
                txt_AcountLoginIp.Text = lstv_Account.SelectedItems[0].SubItems[6].Text;
            }
        }

        private void btn_AcountFreeze_Click(object sender, EventArgs e)
        {
            string acc = CFormat.PureString(txt_AcountName.Text);
            if (acc == "")
                return;
            //封号
            string log = "";
            try
            {
                log = CSGHelper.FreezeAccount(acc, 1, "手动封禁！", "GM");
                LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, acc + ":" + log, new StackTrace(new StackFrame(true)));
                btn_AccountReflush_Click(null, null);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, acc + ":" + ex.Message, new StackTrace(new StackFrame(true)));
            }
            MessageBox.Show("账户：" + acc + log);
        }

        private void btn_AcountDisFreeze_Click(object sender, EventArgs e)
        {
            string acc = CFormat.PureString(txt_AcountName.Text);
            if (acc == "")
                return;
            //封号
            string log = "";
            try
            {
                log = CSGHelper.FreezeAccount(acc, 0, "手动解封！", "GM");
                LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, acc + ":" + log, new StackTrace(new StackFrame(true)));
                btn_AccountReflush_Click(null, null);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, acc + ":" + ex.Message, new StackTrace(new StackFrame(true)));
            }
            MessageBox.Show("账户：" + acc + log);
        }

        private void btn_AcountSrch_Click(object sender, EventArgs e)
        {
            ListViewItem foundItem = null;
            lstv_Account.Items[xbAccountInfoSltIndex].BackColor = Color.Transparent;
            int i = xbAccountInfoSltIndex + 1;
            if (i == lstv_Account.Items.Count)
            {
                i = 0;
            }
            while (i != xbAccountInfoSltIndex)
            {
                if (lstv_Account.Items[i].SubItems[0].Text.Contains(txt_AcountSrch.Text)
                    || lstv_Account.Items[i].SubItems[2].Text.Contains(txt_AcountSrch.Text)
                    || lstv_Account.Items[i].SubItems[6].Text.Contains(txt_AcountSrch.Text))
                {
                    foundItem = lstv_Account.Items[i];
                    break;
                }
                i++;
                if (i == lstv_Account.Items.Count)
                {
                    i = 0;
                }
            }

            if (foundItem != null)
            {
                lstv_Account.TopItem = foundItem;  //定位到该项
                lstv_Account.Items[foundItem.Index].Focused = true;
                //lstv_Account.Items[foundItem.Index].Selected = true;
                foundItem.BackColor = Color.Pink;
                xbAccountInfoSltIndex = foundItem.Index;
            }
        }
        static int xbAccountInfoSltIndex = 0;
        #endregion
    }
}
