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
        #region//自动检测

        private void btn_startListen_Click(object sender, EventArgs e)
        {
            if (!m_Active)
            {
                MessageBox.Show("软件尚未激活！ 请联系软件发布人给予激活！");
                return;
            }
            ////封号操作需要数据库已经链接
            //if (lbl_sqlStatus.Text != "数据库已连接")
            //{
            //    MessageBox.Show("请先连接数据库！");
            //    return;
            //}

            if (btn_startListen.Text == "启动检测")
            {
                btn_startListen.Text = "停止检测";

                tm_strListen.Start();
            }
            else if (btn_startListen.Text == "停止检测")
            {
                tm_strListen.Stop();

                btn_startListen.Text = "启动检测";
            }
        }

        public struct Illegal
        {
            public string account;
            public string name;
            public string time_str;
        };

        private void FreezeCheck()
        {
            //读取loginserverlog,获取非法信息列表
            List<Illegal> nameList = GetIllegalList();

            int FreezeCount = 0;
            //更新列表
            lstv_GtList.Items.Clear();
            foreach (var name in nameList)
            {
                //过滤
                if (m_FreezeFilterList != null)
                {
                    bool filter = false;
                    String PureAcount = CFormat.PureString(name.account);
                    foreach (var item in m_FreezeFilterList)
                    {
                        if (PureAcount == item)
                        {
                            filter = true;
                            break;
                        }
                    }
                    if (filter)
                        continue;
                }

                ListViewItem lvi = new ListViewItem();

                lvi.Text = CFormat.PureString(name.account);
                //lvi.SubItems.Add(_id);
                lvi.SubItems.Add(CFormat.ToSimplified(CFormat.PureString(name.name)));
                lvi.SubItems.Add(name.time_str);

                if (cbx_AutoFreeze.Checked)
                {
                    string acc = CFormat.PureString(name.account);
                    //封号
                    try
                    {
                        string log = CSGHelper.FreezeAccount(acc, 1, "非法登录或者使用外挂！", "GM");
                        lvi.SubItems.Add(log);
                        if (log == "冻结成功")
                        {
                            FreezeCount++;
                        }
                        LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, acc + ":" + log, new StackTrace(new StackFrame(true)));
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, acc + ":" + ex.Message, new StackTrace(new StackFrame(true)));
                    }
                }
                lstv_GtList.Items.Add(lvi);
            }
            lstv_GtList.EndUpdate();

            lbl_IllCount.Text = nameList.Count.ToString();
            lbl_FreezeCount.Text = FreezeCount.ToString();
        }

        private static string[] m_FreezeFilterList;
        private void tm_strListen_Tick(object sender, EventArgs e)
        {
            FreezeCheck();
        }

        private List<Illegal> GetIllegalList()
        {
            List<Illegal> list = new List<Illegal>();

            //获取当前日期
            string time_str = DateTime.Now.ToString("yyyy-MM-dd");
            string log_name = txt_svrForder.Text + "\\Login\\log" + "\\" + time_str + "_log_login.txt";
            if (!File.Exists(log_name))
            {
                return list;
            }
            else
            {
                try
                {
                    //读取
                    FileStream fs = new FileStream(log_name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    StreamReader reader = new StreamReader(fs, Encoding.GetEncoding(950));
                    reader.DiscardBufferedData();
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);
                    reader.BaseStream.Position = 0;

                    string strLine = "";
                    strLine = reader.ReadLine();
                    while (strLine != null)
                    {
                        if (strLine.Contains("SERIOUS: 發現外掛:"))
                        {
                            //分析出 游戏名和账户名
                            //<2016-05-13 04:31:14> SERIOUS: 發現外掛: 586, 光寒方士, 1681, finghter4(177, 2)
                            string time = strLine.Split(new string[] { "SERIOUS: 發現外掛:" }, StringSplitOptions.RemoveEmptyEntries)[0];
                            string tmp = strLine.Split(new string[] { "SERIOUS: 發現外掛:" }, StringSplitOptions.RemoveEmptyEntries)[1];
                            string name = tmp.Split(',')[1];
                            string account = tmp.Split(',')[3].Split('(')[0];

                            Illegal ill_player = new Illegal();

                            ill_player.account = account;
                            ill_player.name = name;
                            ill_player.time_str = time;

                            //判断是否已经记录此用户  -- 记录则更新其时间，没有则加入
                            bool get = false;
                            for (int i = 0; i < list.Count; i++)
                            {
                                if (list[i].account == ill_player.account)
                                {
                                    Illegal reitem = new Illegal();
                                    reitem = list[i];
                                    reitem.time_str = ill_player.time_str;
                                    list[i] = reitem;

                                    get = true;
                                    break;
                                }
                            }
                            if (!get)
                            {
                                list.Add(ill_player);
                            }
                        }
                        //<2016-05-08 00:01:36> Account info(tt8873351,online.dat,0x88671f25,2567829)
                        else if (strLine.Contains("Account info") && !strLine.Contains("qysg.dat") && strLine.Contains("(") && strLine.Contains(")"))
                        {
                            string time = strLine.Split(new string[] { "Account info" }, StringSplitOptions.RemoveEmptyEntries)[0];
                            string account = strLine.Split('(')[1].Split(',')[0];
                            Illegal ill_player = new Illegal();

                            ill_player.account = account;
                            ill_player.name = "非法登录程序";
                            ill_player.time_str = time;

                            //判断是否已经记录此用户  -- 记录则更新其时间，没有则加入
                            bool get = false;
                            for (int i = 0; i < list.Count; i++)
                            {
                                if (list[i].account == ill_player.account)
                                {
                                    Illegal reitem = new Illegal();
                                    reitem = list[i];
                                    reitem.time_str = ill_player.time_str;
                                    list[i] = reitem;

                                    get = true;
                                    break;
                                }
                            }
                            if (!get)
                            {
                                list.Add(ill_player);
                            }
                        }

                        strLine = null;
                        strLine = reader.ReadLine();
                    }

                    reader.Close();
                    fs.Close();
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, ex.Message, new StackTrace(new StackFrame(true)));
                }

            }
            return list;
        }

        private void btn_FreezeListAcc_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstv_GtList.Items.Count; i++)
            {
                string acc = lstv_GtList.Items[i].SubItems[0].Text;
                //封号
                try
                {
                    string log = CSGHelper.FreezeAccount(acc, 1, "非法登录或者使用外挂！", "GM");
                    LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, acc + ":" + log, new StackTrace(new StackFrame(true)));
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, acc + ":" + ex.Message, new StackTrace(new StackFrame(true)));
                }
            }
        }

        private void cbx_AutoFreeze_CheckedChanged(object sender, EventArgs e)
        {
            if (cbx_AutoFreeze.Checked)
            {
                CIniCtrl.WriteIniData("Config", "AutoFreeze", "Enable", serverIni);
            }
            else
            {
                CIniCtrl.WriteIniData("Config", "AutoFreeze", "Disable", serverIni);
            }
        }

        private void btn_FreezeFilterSet_Click(object sender, EventArgs e)
        {
            m_FreezeFilterList = rtb_FreezeFilter.Text.Split(',');
            CIniCtrl.WriteIniData("Config", "FreezeFilter", rtb_FreezeFilter.Text, serverIni);
        }

        private void IllCheckNow_Click(object sender, EventArgs e)
        {
            FreezeCheck();
        }

        #endregion
    }
}
