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
            CPlayerCtrl.LoadPlayerInfos(txt_svrForder.Text + "\\DataBase\\saves\\players.dat", true);
            List<Account> infos = CSGHelper.SelectAcountInfo();
            lstv_Account.Items.Clear();
            foreach(var info in infos)
            {
                //构建一个ListView的数据，存入数据库数据，以便添加到listView1的行数据中
                ListViewItem lt = new ListViewItem();
                //将数据库数据转变成ListView类型的一行数据
                lt.Text = info.account;
                lt.SubItems.Add(info.password);
                lt.SubItems.Add(CPlayerCtrl.GetNameByAcc(lt.Text));
                lt.SubItems.Add(info.enable);
                lt.SubItems.Add(info.privilege);
                lt.SubItems.Add(info.point);
                lt.SubItems.Add(info.ip);
                lt.SubItems.Add(info.LastLoginTime);
                lt.SubItems.Add(info.LastLogoutTime);
                //将lt数据添加到listView1控件中
                lstv_Account.Items.Add(lt);
            }
            lstv_Account.EndUpdate();
            lbl_AccountCount.Text = "帐号总数：" + infos.Count;
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
                log = CSGHelper.FreezeAccount(acc, 1, "手动冻结", "GM");
                LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, acc + ":" + log, new StackTrace(new StackFrame(true)));
                btn_AccountReflush_Click(null, null);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, acc + "-Exception:" + ex.Message, new StackTrace(new StackFrame(true)));
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
                log = CSGHelper.UnFreezeAccount(acc, 1, "手动解封！", "GM");
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

        private void btn_AcountAdd_Click(object sender, EventArgs e)
        {
            string account = txt_AcountName.Text;
            string pwd = txt_AcountPwd.Text;
            string result = CSGHelper.CreateAccount(account, pwd);
            MessageBox.Show(result);
        }

        private void btn_AcountMdifyPwd_Click(object sender, EventArgs e)
        {
            string account = txt_AcountName.Text;
            string pwd = txt_AcountPwd.Text;
            string pwdnew = txt_AcountNewPwd.Text;
            string result = CSGHelper.ModifyPwd(account, pwd, pwdnew);
            MessageBox.Show(result);
        }
        #endregion
    }
}
