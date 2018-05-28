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
        #region //服务管理
        private void LoadSgserverConf()
        {
            load_ServerPort_Conf();
            load_Cb_Conf();
            load_War_Conf();
            load_Soul_Conf();

            //加载服务启动
            load_Process_Conf();
        }
        private void btn_register_Click(object sender, EventArgs e)
        {
            //数据库未连接，账户管理不允许启动
            /*
            if (btn_sql.Text == "连接数据库")
            {
                MessageBox.Show("请先连接数据库！");
                return;
            }
            */
            if (btn_register.Text == "启用")
            {
                //启用监听端口
                CSocketHelper.StartListening(int.Parse(accMrgPort));

                btn_register.Text = "禁用";
                lbl_regstatus.Text = "服务已开启";
                txt_accMrgPort.ReadOnly = true;
                btn_accPortModify.Enabled = false;
            }
            else
            {
                //关闭监听
                CSocketHelper.StopListening(int.Parse(accMrgPort));
                btn_register.Text = "启用";
                lbl_regstatus.Text = "服务已关闭。";
                txt_accMrgPort.ReadOnly = false;
                btn_accPortModify.Enabled = true;
            }
        }
        private void button2_Click(object sender, EventArgs e)
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

            //读取版本号到txt_svrForder
            txt_svrForder.Text = foldPath;

            //保存次路径到工具配置文件
            CIniCtrl.WriteIniData("Config", "gameServerFolder", foldPath, serverIni);
        }

        private void btn_folderSync_Click(object sender, EventArgs e)
        {
            //Account/AccountServer.ini [system]  server_ini_dir = d:\sgserver\soldata 
            string AccountServerFile = txt_svrForder.Text + "\\Account\\AccountServer.ini";
            if (!File.Exists(AccountServerFile))
            {
                MessageBox.Show("丢失关键文件:" + AccountServerFile);
            }
            else
            {
                CIniCtrl.WriteIniData("system", "server_ini_dir", txt_svrForder.Text + "\\soldata", AccountServerFile);
            }
            //DataBase/DBServer.ini  [DBServer] server_ini_dir = d:\sgserver\soldata
            string DataBaseFile = txt_svrForder.Text + "\\DataBase\\DBServer.ini";
            if (!File.Exists(DataBaseFile))
            {
                MessageBox.Show("丢失关键文件:" + DataBaseFile);
            }
            else
            {
                CIniCtrl.WriteIniData("DBServer", "server_ini_dir", txt_svrForder.Text + "\\soldata", DataBaseFile);
            }
            //Log/LogServer.ini [system] server_ini_dir = D:\sgserver\soldata
            string LogFile = txt_svrForder.Text + "\\Log\\LogServer.ini";
            if (!File.Exists(LogFile))
            {
                MessageBox.Show("丢失关键文件:" + LogFile);
            }
            else
            {
                CIniCtrl.WriteIniData("system", "server_ini_dir", txt_svrForder.Text + "\\soldata", LogFile);
            }
            //Login/LoginServer.ini [System]  server_ini_dir = d:\sgserver\soldata 
            string LoginServerFile = txt_svrForder.Text + "\\Login\\LoginServer.ini";
            if (!File.Exists(LoginServerFile))
            {
                MessageBox.Show("丢失关键文件:" + LoginServerFile);
            }
            else
            {
                CIniCtrl.WriteIniData("System", "server_ini_dir", txt_svrForder.Text + "\\soldata", LoginServerFile);
            }
            //Map1-1/MapServer.ini [MapServer] data_dir = d:\sgserver\soldata server_ini_dir = d:\sgserver\soldata
            DirectoryInfo di = new DirectoryInfo(txt_svrForder.Text);
            DirectoryInfo[] diA = di.GetDirectories();
            foreach (var item in diA)
            {
                if (item.FullName.Contains("Map"))
                {
                    string MapServerFile = item.FullName + "\\MapServer.ini";
                    if (!File.Exists(MapServerFile))
                    {
                        MessageBox.Show("丢失关键文件:" + MapServerFile);
                    }
                    else
                    {
                        CIniCtrl.WriteIniData("MapServer", "server_ini_dir", txt_svrForder.Text + "\\soldata", MapServerFile);
                        CIniCtrl.WriteIniData("MapServer", "data_dir", txt_svrForder.Text + "\\soldata", MapServerFile);
                    }
                }
            }

            //VTServer/VTServer.ini [system] server_ini_dir = d:\sgserver\soldata
            string VTServerFile = txt_svrForder.Text + "\\VTServer\\VTServer.ini";
            if (!File.Exists(VTServerFile))
            {
                MessageBox.Show("丢失关键文件:" + VTServerFile);
            }
            else
            {
                CIniCtrl.WriteIniData("system", "server_ini_dir", txt_svrForder.Text + "\\soldata", VTServerFile);
            }
            MessageBox.Show("设置完成！");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string gServerIp = txt_GServerIP.Text;
            CIniCtrl.WriteIniData("Server", "GServerIP", gServerIp, serverIni);

            //

            //读取 //soldata/Server.ini
            string ServerFile = txt_svrForder.Text + "\\soldata\\Server.ini";
            if (!File.Exists(ServerFile))
            {
                return;
            }
            FileStream rdfs = new FileStream(ServerFile, FileMode.Open, FileAccess.Read);
            StreamReader rd = new StreamReader(rdfs, Encoding.ASCII);
            rd.BaseStream.Seek(0, SeekOrigin.Begin);

            rd.DiscardBufferedData();
            rd.BaseStream.Seek(0, SeekOrigin.Begin);
            rd.BaseStream.Position = 0;

            string allLine = "";
            string strLine = "";
            strLine = "";
            strLine = rd.ReadLine();
            do
            {
                if (strLine.Contains("ip = "))
                {
                    strLine = "ip = " + gServerIp;
                }
                else if (strLine.Contains("ip_real = "))
                {
                    strLine = "ip_real = " + gServerIp;
                }

                allLine += strLine + "\r\n";
                strLine = "";
                strLine = rd.ReadLine();
            } while (strLine != null);
            rd.Close();
            rdfs.Close();

            File.WriteAllText(ServerFile, allLine);
        }
        private void load_ServerPort_Conf()
        {
            //读取 //soldata/Server.ini
            string ServerFile = txt_svrForder.Text + "\\soldata\\Server.ini";
            if (!File.Exists(ServerFile))
            {
                return;
            }

            string set_name = CIniCtrl.ReadIniData("LoginServer", "server_set_name", "", ServerFile);
            m_SGExHandle.LoadLoginServerPtr(set_name.Split(';')[0].Split('/')[0].Replace(" ",""));

            string loginPort = CIniCtrl.ReadIniData("LoginServer", "port", "", ServerFile);
            txt_loginPort.Text = loginPort;

            string DBPort = CIniCtrl.ReadIniData("DBServer", "port", "", ServerFile);
            txt_dbPort.Text = DBPort;

            string logPort = CIniCtrl.ReadIniData("LogServer", "port", "", ServerFile);
            txt_logPort.Text = logPort;

            string accountPort = CIniCtrl.ReadIniData("AccountServer", "port", "", ServerFile);
            txt_accountPort.Text = accountPort;

            string VTPort = CIniCtrl.ReadIniData("VTServer", "port", "", ServerFile);
            txt_VTPort.Text = VTPort;

            //map
            //string mapPort = "";
            //var list = CIniCtrlComm.ReadValues(ServerFile);
            //foreach (var item in list)
            //{
            //    if (item.Section == "MapServer")
            //    {
            //        mapPort += item.GetComment("port") + ",";
            //    }
            //}
            //txt_mapPort.Text = mapPort; 

            string mapPort = "";
            FileStream rdfs = new FileStream(ServerFile, FileMode.Open, FileAccess.Read);
            StreamReader rd = new StreamReader(rdfs, Encoding.ASCII);
            rd.BaseStream.Seek(0, SeekOrigin.Begin);

            rd.DiscardBufferedData();
            rd.BaseStream.Seek(0, SeekOrigin.Begin);
            rd.BaseStream.Position = 0;

            string strLine = "";
            strLine = "";
            strLine = rd.ReadLine();
            string map_session = "";
            do
            {
                if (strLine.Contains("[MapServer]"))
                {
                    map_session = "MapServer";
                }
                if (strLine.Contains("port") && map_session == "MapServer")
                {
                    string map_port = strLine.Split('=')[1];
                    mapPort += map_port + ",";
                    map_session = "";
                }

                strLine = "";
                strLine = rd.ReadLine();
            } while (strLine != null);
            rd.Close();
            rdfs.Close();

            txt_mapPort.Text = mapPort.Replace(" ", "");
        }
        private void button6_Click(object sender, EventArgs e)
        {
            load_ServerPort_Conf();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //设置
            string ServerFile = txt_svrForder.Text + "\\soldata\\Server.ini";
            if (!File.Exists(ServerFile))
            {
                return;
            }
            string loginPort = txt_loginPort.Text;
            CIniCtrl.WriteIniData("LoginServer", "port", loginPort, ServerFile);

            string DBPort = txt_dbPort.Text;
            CIniCtrl.WriteIniData("DBServer", "port", DBPort, ServerFile);

            string logPort = txt_logPort.Text;
            CIniCtrl.WriteIniData("LogServer", "port", logPort, ServerFile);

            string accountPort = txt_accountPort.Text;
            CIniCtrl.WriteIniData("AccountServer", "port", accountPort, ServerFile);

            string VTPort = txt_VTPort.Text;
            CIniCtrl.WriteIniData("VTServer", "port", VTPort, ServerFile);

            string mapPort = txt_mapPort.Text;
            var map_list = mapPort.Split(',');

            FileStream rdfs = new FileStream(ServerFile, FileMode.Open, FileAccess.Read);
            StreamReader rd = new StreamReader(rdfs, Encoding.ASCII);
            rd.BaseStream.Seek(0, SeekOrigin.Begin);

            rd.DiscardBufferedData();
            rd.BaseStream.Seek(0, SeekOrigin.Begin);
            rd.BaseStream.Position = 0;

            string allLine = "";
            string strLine = "";
            strLine = "";
            strLine = rd.ReadLine();
            string map_session = "";
            int map_list_index = 0;
            do
            {
                if (strLine.Contains("[MapServer]"))
                {
                    map_session = "MapServer";
                }
                if (strLine.Contains("port") && map_session == "MapServer")
                {
                    map_session = "";

                    if (map_list_index < map_list.Length)
                    {
                        strLine = "port = " + map_list[map_list_index];
                        map_list_index++;
                    }
                }
                allLine += strLine + "\r\n";
                strLine = "";
                strLine = rd.ReadLine();
            } while (strLine != null);
            rd.Close();
            rdfs.Close();

            File.WriteAllText(ServerFile, allLine);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string tmp = "";
            string tmpFile = "";

            //Account AccountServer.ini [system] sql_ip = 127.0.0.1,4433 sql_inout_ip = 127.0.0.1,4433 sql_account = sa sql_password = 123456
            tmpFile = txt_svrForder.Text + "\\Account\\AccountServer.ini";
            if (!File.Exists(tmpFile))
            {
                return;
            }
            tmp = txt_sqlsvr.Text + "," + txt_sqlPort.Text;
            CIniCtrl.WriteIniData("system", "sql_ip", tmp, tmpFile);
            CIniCtrl.WriteIniData("system", "sql_inout_ip", tmp, tmpFile);

            tmp = txt_sqlAcc.Text;
            CIniCtrl.WriteIniData("system", "sql_account", tmp, tmpFile);
            tmp = txt_sqlPwd.Text;
            CIniCtrl.WriteIniData("system", "sql_password", tmp, tmpFile);
            //Log LogServer.ini [system] sql_ip = 127.0.0.1,4433 sql_account = sa sql_password = 123456
            tmpFile = txt_svrForder.Text + "\\Log\\LogServer.ini";
            if (!File.Exists(tmpFile))
            {
                return;
            }
            tmp = txt_sqlsvr.Text + "," + txt_sqlPort.Text;
            CIniCtrl.WriteIniData("system", "sql_ip", tmp, tmpFile);

            tmp = txt_sqlAcc.Text;
            CIniCtrl.WriteIniData("system", "sql_account", tmp, tmpFile);
            tmp = txt_sqlPwd.Text;
            CIniCtrl.WriteIniData("system", "sql_password", tmp, tmpFile);

            //VTServer VTServer.ini [system] sql_item_ip = 127.0.0.1,4433 sql_ip = 127.0.0.1,4433 sql_account = sa sql_password = 123456
            tmpFile = txt_svrForder.Text + "\\VTServer\\VTServer.ini";
            if (!File.Exists(tmpFile))
            {
                return;
            }
            tmp = txt_sqlsvr.Text + "," + txt_sqlPort.Text;
            CIniCtrl.WriteIniData("system", "sql_item_ip", tmp, tmpFile);
            CIniCtrl.WriteIniData("system", "sql_ip", tmp, tmpFile);

            tmp = txt_sqlAcc.Text;
            CIniCtrl.WriteIniData("system", "sql_account", tmp, tmpFile);
            tmp = txt_sqlPwd.Text;
            CIniCtrl.WriteIniData("system", "sql_password", tmp, tmpFile);
        }

        private void btn_soulset_Click(object sender, EventArgs e)
        {
            //设置
            string LoginServerFile = txt_svrForder.Text + "\\Login\\LoginServer.ini";
            if (!File.Exists(LoginServerFile))
            {
                return;
            }
            string tmp = "";
            tmp = txt_soulticketwdy.Text;
            CIniCtrl.WriteIniData("soul", "ticket_weekday", tmp, LoginServerFile);
            tmp = txt_soulsellday.Text;
            CIniCtrl.WriteIniData("soul", "ticket_sell", tmp, LoginServerFile);
            tmp = txt_soulbattlwdy.Text;
            CIniCtrl.WriteIniData("soul", "battle_weekday", tmp, LoginServerFile);
            tmp = txt_soulbattltime.Text;
            CIniCtrl.WriteIniData("soul", "battle_time", tmp, LoginServerFile);
            tmp = txt_soulbattlperiod.Text;
            CIniCtrl.WriteIniData("soul", "battle_period", tmp, LoginServerFile);

            m_SGExHandle.ReloadLoginServer();
        }

        private void btn_warset_Click(object sender, EventArgs e)
        {
            //设置
            string LoginServerFile = txt_svrForder.Text + "\\Login\\LoginServer.ini";
            if (!File.Exists(LoginServerFile))
            {
                return;
            }
            string tmp = "";
            tmp = txt_wardate.Text;
            CIniCtrl.WriteIniData("System", "country_war_date", tmp, LoginServerFile);
            tmp = txt_wartime.Text;
            CIniCtrl.WriteIniData("System", "country_war_time", tmp, LoginServerFile);
            tmp = txt_warperiod.Text;
            CIniCtrl.WriteIniData("System", "country_war_period", tmp, LoginServerFile);

            m_SGExHandle.ReloadLoginServer();
        }
        private void load_Process_Conf()
        {
            txt_ps1.Text = CIniCtrl.ReadIniData("Prosess", "ps1", "", serverIni);
            txt_ps2.Text = CIniCtrl.ReadIniData("Prosess", "ps2", "", serverIni);
            txt_ps3.Text = CIniCtrl.ReadIniData("Prosess", "ps3", "", serverIni);
            txt_ps4.Text = CIniCtrl.ReadIniData("Prosess", "ps4", "", serverIni);
            txt_ps5.Text = CIniCtrl.ReadIniData("Prosess", "ps5", "", serverIni);
            txt_ps6.Text = CIniCtrl.ReadIniData("Prosess", "ps6", "", serverIni);
            txt_ps7.Text = CIniCtrl.ReadIniData("Prosess", "ps7", "", serverIni);
            txt_ps8.Text = CIniCtrl.ReadIniData("Prosess", "ps8", "", serverIni);
            txt_ps9.Text = CIniCtrl.ReadIniData("Prosess", "ps9", "", serverIni);
            txt_ps10.Text = CIniCtrl.ReadIniData("Prosess", "ps10", "", serverIni);
            txt_ps11.Text = CIniCtrl.ReadIniData("Prosess", "ps11", "", serverIni);
            txt_ps12.Text = CIniCtrl.ReadIniData("Prosess", "ps12", "", serverIni);
            txt_ps13.Text = CIniCtrl.ReadIniData("Prosess", "ps13", "", serverIni);
            txt_ps14.Text = CIniCtrl.ReadIniData("Prosess", "ps14", "", serverIni);
            txt_ps15.Text = CIniCtrl.ReadIniData("Prosess", "ps15", "", serverIni);
        }
        private void load_Soul_Conf()
        {
            //读取武魂设置
            string LoginServerFile = txt_svrForder.Text + "\\Login\\LoginServer.ini";
            if (!File.Exists(LoginServerFile))
            {
                return;
            }
            string tmp = "";

            tmp = CIniCtrl.ReadIniData("soul", "ticket_weekday", "", LoginServerFile);
            txt_soulticketwdy.Text = tmp.Split(';')[0].Replace("\t", "").Replace(" ", "");

            tmp = CIniCtrl.ReadIniData("soul", "ticket_sell", "", LoginServerFile);
            txt_soulsellday.Text = tmp.Split(';')[0].Replace("\t", "").Replace(" ", "");

            tmp = CIniCtrl.ReadIniData("soul", "battle_weekday", "", LoginServerFile);
            txt_soulbattlwdy.Text = tmp.Split(';')[0].Replace("\t", "").Replace(" ", "");

            tmp = CIniCtrl.ReadIniData("soul", "battle_time", "", LoginServerFile);
            txt_soulbattltime.Text = tmp.Split(';')[0].Replace("\t", "").Replace(" ", "");

            tmp = CIniCtrl.ReadIniData("soul", "battle_period", "", LoginServerFile);
            txt_soulbattlperiod.Text = tmp.Split(';')[0].Replace("\t", "").Replace(" ", "");
        }

        private void load_War_Conf()
        {
            //读取国战设置
            string LoginServerFile = txt_svrForder.Text + "\\Login\\LoginServer.ini";
            if (!File.Exists(LoginServerFile))
            {
                return;
            }
            string tmp = "";

            tmp = CIniCtrl.ReadIniData("System", "country_war_date", "", LoginServerFile);
            txt_wardate.Text = tmp.Split(';')[0].Replace("\t", "").Replace(" ", "");

            tmp = CIniCtrl.ReadIniData("System", "country_war_time", "", LoginServerFile);
            txt_wartime.Text = tmp.Split(';')[0].Replace("\t", "").Replace(" ", "");

            tmp = CIniCtrl.ReadIniData("System", "country_war_period", "", LoginServerFile);
            txt_warperiod.Text = tmp.Split(';')[0].Replace("\t", "").Replace(" ", "");
        }

        private void load_Cb_Conf()
        {
            //读取 //Login/LoginHistory.ini
            string LoginHistoryFile = txt_svrForder.Text + "\\Login\\LoginHistory.ini";
            if (!File.Exists(LoginHistoryFile))
            {
                return;
            }

            FileStream rdfs = new FileStream(LoginHistoryFile, FileMode.Open, FileAccess.Read);
            StreamReader rd = new StreamReader(rdfs, Encoding.ASCII);
            rd.BaseStream.Seek(0, SeekOrigin.Begin);

            rd.DiscardBufferedData();
            rd.BaseStream.Seek(0, SeekOrigin.Begin);
            rd.BaseStream.Position = 0;

            string strLine = "";
            strLine = "";
            strLine = rd.ReadLine();
            string history_session = "";
            string history_type = "";
            string history_period = "";
            string history_min_team_player = "";
            do
            {
                if (strLine.Contains("[history]"))
                {
                    history_session = "history";
                }
                if (strLine.Contains("type"))
                {
                    history_type = strLine.Split('=')[1].Split(';')[0].Replace("\t", "").Replace(" ", "");
                }
                if (strLine.Contains("period") && history_type == "1")
                {
                    history_period = strLine.Split('=')[1].Split(';')[0].Replace("\t", "").Replace(" ", "");
                    txt_cbperiod.Text = history_period;
                }
                if (strLine.Contains("min_team_player") && history_type == "1")
                {
                    history_min_team_player = strLine.Split('=')[1].Split(';')[0].Replace("\t", "").Replace(" ", "");
                    txt_cbminnum.Text = history_min_team_player;

                }
                if (strLine.Contains("time") && history_session == "history" && history_type == "1")
                {
                    string history_time = strLine.Split('=')[1].Split(';')[0].Replace("\t", "").Replace(" ", "");
                    string history_weekday = history_time.Replace(" ", "").Split(',')[0];
                    if (history_weekday == "1")
                    {
                        txt_cbMonTime.Text = history_time.Substring(2);
                    }
                    else if (history_weekday == "2")
                    {
                        txt_cbTueTime.Text = history_time.Substring(2);
                    }
                    else if (history_weekday == "3")
                    {
                        txt_cbWedTime.Text = history_time.Substring(2);
                    }
                    else if (history_weekday == "4")
                    {
                        txt_cbThuTime.Text = history_time.Substring(2);
                    }
                    else if (history_weekday == "5")
                    {
                        txt_cbFriTime.Text = history_time.Substring(2);
                    }
                    else if (history_weekday == "6")
                    {
                        txt_cbSatTime.Text = history_time.Substring(2);
                    }
                    else if (history_weekday == "7")
                    {
                        txt_cbSunTime.Text = history_time.Substring(2);
                    }
                }
                strLine = "";
                strLine = rd.ReadLine();
            } while (strLine != null);
            rd.Close();
            rdfs.Close();
        }

        private void btn_cbset_Click(object sender, EventArgs e)
        {
            //设置赤壁
            string LoginHistoryFile = txt_svrForder.Text + "\\Login\\LoginHistory.ini";
            if (!File.Exists(LoginHistoryFile))
            {
                return;
            }
            FileStream rdfs = new FileStream(LoginHistoryFile, FileMode.Open, FileAccess.Read);
            //StreamReader rd = new StreamReader(rdfs, Encoding.ASCII);
            StreamReader rd = new StreamReader(rdfs, Encoding.Default);
            rd.BaseStream.Seek(0, SeekOrigin.Begin);

            rd.DiscardBufferedData();
            rd.BaseStream.Seek(0, SeekOrigin.Begin);
            rd.BaseStream.Position = 0;

            string allLine = "";
            string strLine = "";
            strLine = "";
            strLine = rd.ReadLine();
            string history_session = "";
            string history_type = "";
            string history_cb_time = "";

            if (txt_cbMonTime.Text != "")
            {
                history_cb_time += "time = 1," + txt_cbMonTime.Text + "\r\n";
            }
            if (txt_cbTueTime.Text != "")
            {
                history_cb_time += "time = 2," + txt_cbTueTime.Text + "\r\n";
            }
            if (txt_cbWedTime.Text != "")
            {
                history_cb_time += "time = 3," + txt_cbWedTime.Text + "\r\n";
            }
            if (txt_cbThuTime.Text != "")
            {
                history_cb_time += "time = 4," + txt_cbThuTime.Text + "\r\n";
            }
            if (txt_cbFriTime.Text != "")
            {
                history_cb_time += "time = 5," + txt_cbFriTime.Text + "\r\n";
            }
            if (txt_cbSatTime.Text != "")
            {
                history_cb_time += "time = 6," + txt_cbSatTime.Text + "\r\n";
            }
            if (txt_cbSunTime.Text != "")
            {
                history_cb_time += "time = 7," + txt_cbSunTime.Text + "\r\n";
            }

            do
            {
                if (strLine.Contains("[history]"))
                {
                    history_session = "history";
                    if (history_type == "1")
                    {
                        allLine += history_cb_time;
                        allLine += "\r\n";
                    }
                    else if (history_type != "")
                    {
                        allLine += "\r\n";
                    }
                }
                if (strLine.Contains("type"))
                {
                    history_type = strLine.Split('=')[1].Split(';')[0].Replace("\t", "").Replace(" ", "");
                }
                if (strLine.Contains("period") && history_type == "1")
                {
                    strLine = "period = " + txt_cbperiod.Text;
                }
                if (strLine.Contains("min_team_player") && history_type == "1")
                {
                    strLine = "min_team_player = " + txt_cbminnum.Text;
                }
                if (strLine.Contains("time") && history_session == "history" && history_type == "1")
                {
                    strLine = "";
                }
                if (strLine != "")
                {
                    allLine += strLine + "\r\n";
                }

                strLine = "";
                strLine = rd.ReadLine();
            } while (strLine != null);
            rd.Close();
            rdfs.Close();

            File.WriteAllText(LoginHistoryFile, allLine);

            m_SGExHandle.ReloadLoginServer();
        }

        #endregion
    }
}
