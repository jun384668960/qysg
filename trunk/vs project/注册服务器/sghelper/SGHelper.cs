using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace MainServer
{
    public struct LogSearch
    {
        public string log_time;
        public string type;
        public string name;
        public string ip;
        public string msg;
    };
    public struct LogItemSearch
    {
        public string log_time;
        public string type;
        public string from_name;
        public string item_name;
    };
    public struct Account
    {
        public string account;
        public string password;
        public string password2;
        public string duedate;
        public string enable;
        public string lock_duedate;
        public string logout_time;
        public string ip;
        public string create_time;
        public string privilege;
        public string status;
        public string sec_pwd;
        public string first_ip;
        public string point;
        public string trade_psw;
        public string IsAdult;
        public string OnlineTime;
        public string Offline_Time;
        public string LastLoginTime;
        public string LastLogoutTime;
    };

    public class CSGHelper
    {
        public static string m_sqlLog = "";
        public static string m_sqlSanvt = "";
        public static string m_sqlAccount = "";
        public static bool m_SqlConnected = false;
        private static System.Threading.Mutex mutex = new System.Threading.Mutex(false, "MutexLog");
        private static SqlConnection conn;
        public static bool SqlConn(string cnn_str) {
            //conn = new SqlConnection("Data Source = 127.0.0.1; Initial Catalog = Account; User Id = sa; Password = 123456;");
            if (m_sqlLog == "" || m_sqlSanvt == "" || m_sqlAccount == "")
            {
                MessageBox.Show("请先设置游戏数据库名称！");
                return false;
            }
            try
            {
                mutex.WaitOne();
                conn = new SqlConnection(cnn_str); ;
                conn.Open();
                m_SqlConnected = true;
                mutex.ReleaseMutex();
                return true;
            }
            catch (Exception)
            {
                m_SqlConnected = false;
                mutex.ReleaseMutex();
                return false;
            }
        }
        public static bool SqlClose()
        {
            conn.Close();
            m_SqlConnected = false;
            return true;
        }
        public static void SetSQLNames(string account, string sanvt, string log)
        {
            if (account == string.Empty || sanvt == string.Empty || log == string.Empty)
            {
                return;
            }
            m_sqlLog = log;
            m_sqlSanvt = sanvt;
            m_sqlAccount = account;
        }
        public static string CreateAccount(string name, string passwd)
        {
            if (!m_SqlConnected)
                return "创建失败";

            mutex.WaitOne();
            string accout_name = name;
            string accout_password = passwd;
            string accout_password2 = Md5Helper.MD5ToString(passwd);


            string ErrInfo = "";
            SqlCommand sqlComm = null;
            //*
            sqlComm = new SqlCommand("AC_sp_CreateAccount", conn);
            //设置命令的类型为存储过程
            sqlComm.CommandType = CommandType.StoredProcedure;
            //设置参数
            sqlComm.Parameters.Add("@strGameAccount", SqlDbType.VarChar);
            sqlComm.Parameters.Add("@strGamePWD", SqlDbType.VarChar);
            sqlComm.Parameters.Add("@strEncryptGamePWD", SqlDbType.VarChar);
            //注意输出参数要设置大小,否则size默认为0,
            sqlComm.Parameters.Add("@strErrInfo", SqlDbType.VarChar, 512);
            //设置参数的类型为输出参数,默认情况下是输入,
            sqlComm.Parameters["@strErrInfo"].Direction = ParameterDirection.Output;
            //为参数赋值
            sqlComm.Parameters["@strGameAccount"].Value = accout_name;
            sqlComm.Parameters["@strGamePWD"].Value = accout_password;
            sqlComm.Parameters["@strEncryptGamePWD"].Value = accout_password2;
            //执行
            sqlComm.ExecuteNonQuery();
            //得到输出参数的值,把赋值给name,注意,这里得到的是object类型的,要进行相应的类型轮换
            ErrInfo = sqlComm.Parameters["@strErrInfo"].Value.ToString();
            mutex.ReleaseMutex();

            return ErrInfo;
        }

        public static string ModifyPwd(string name, string oldpasswd, string newpasswd)
        {
            if (!m_SqlConnected)
                return "修改失败";

            mutex.WaitOne();

            string accout_name = name;
            string old_EncPasswd = Md5Helper.MD5ToString(oldpasswd);
            string new_passwd = newpasswd;
            string new_EncPasswd = Md5Helper.MD5ToString(newpasswd);

            string ErrInfo = "";
            SqlCommand sqlComm = null;
            //*
            sqlComm = new SqlCommand("AC_sp_ModifyPassword", conn);
            //设置命令的类型为存储过程
            sqlComm.CommandType = CommandType.StoredProcedure;
            //设置参数
            sqlComm.Parameters.Add("@strGameAccount", SqlDbType.VarChar);
            sqlComm.Parameters.Add("@strOldEncryptGamePWD", SqlDbType.VarChar);
            sqlComm.Parameters.Add("@strNewGamePWD", SqlDbType.VarChar);
            sqlComm.Parameters.Add("@strNewEncryptGamePWD", SqlDbType.VarChar);
            //注意输出参数要设置大小,否则size默认为0,
            sqlComm.Parameters.Add("@strErrInfo", SqlDbType.VarChar, 512);
            //设置参数的类型为输出参数,默认情况下是输入,
            sqlComm.Parameters["@strErrInfo"].Direction = ParameterDirection.Output;
            //为参数赋值
            sqlComm.Parameters["@strGameAccount"].Value = accout_name;
            sqlComm.Parameters["@strOldEncryptGamePWD"].Value = old_EncPasswd;
            sqlComm.Parameters["@strNewGamePWD"].Value = new_passwd;
            sqlComm.Parameters["@strNewEncryptGamePWD"].Value = new_EncPasswd;
            //执行
            sqlComm.ExecuteNonQuery();
            //得到输出参数的值,把赋值给name,注意,这里得到的是object类型的,要进行相应的类型轮换
            ErrInfo = sqlComm.Parameters["@strErrInfo"].Value.ToString();
            mutex.ReleaseMutex();

            return ErrInfo;
        }

        

        public static string UnFreezeAccount(string accout_name, int type, string reason, string optor)
        {
            if (!m_SqlConnected)
                return "解封失败";

            mutex.WaitOne();

            string ErrInfo = "";
            SqlCommand sqlComm = null;
            //*
            sqlComm = new SqlCommand("AC_sp_UnFreezeAccount", conn);
            //设置命令的类型为存储过程
            sqlComm.CommandType = CommandType.StoredProcedure;
            //设置参数
            sqlComm.Parameters.Add("@strGameAccount", SqlDbType.VarChar, 32);
            sqlComm.Parameters.Add("@iUnFreezeType", SqlDbType.TinyInt);
            sqlComm.Parameters.Add("@FreezeReason", SqlDbType.VarChar, 50);
            sqlComm.Parameters.Add("@Operator", SqlDbType.VarChar, 20);
            //注意输出参数要设置大小,否则size默认为0,
            sqlComm.Parameters.Add("@strErrInfo", SqlDbType.VarChar, 512);
            //设置参数的类型为输出参数,默认情况下是输入,
            sqlComm.Parameters["@strErrInfo"].Direction = ParameterDirection.Output;
            //为参数赋值
            sqlComm.Parameters["@strGameAccount"].Value = accout_name;
            sqlComm.Parameters["@iUnFreezeType"].Value = type;
            sqlComm.Parameters["@FreezeReason"].Value = reason;
            sqlComm.Parameters["@Operator"].Value = optor;
            //执行
            sqlComm.ExecuteNonQuery();
            //得到输出参数的值,把赋值给name,注意,这里得到的是object类型的,要进行相应的类型轮换
            ErrInfo = sqlComm.Parameters["@strErrInfo"].Value.ToString();
            mutex.ReleaseMutex();

            return ErrInfo;
        }
        public static string FreezeAccount(string accout_name, int type, string reason, string optor)
        {
            if (!m_SqlConnected)
                return "冻结失败";

            mutex.WaitOne();

            string ErrInfo = "";
            SqlCommand sqlComm = null;
            //*
            sqlComm = new SqlCommand("AC_sp_FreezeAccount", conn);
            //设置命令的类型为存储过程
            sqlComm.CommandType = CommandType.StoredProcedure;
            //设置参数
            sqlComm.Parameters.Add("@strGameAccount", SqlDbType.VarChar, 32);
            sqlComm.Parameters.Add("@iFreezeType", SqlDbType.TinyInt);
            sqlComm.Parameters.Add("@FreezeReason", SqlDbType.VarChar, 50);
            sqlComm.Parameters.Add("@Operator", SqlDbType.VarChar, 20);
            //注意输出参数要设置大小,否则size默认为0,
            sqlComm.Parameters.Add("@strErrInfo", SqlDbType.VarChar, 512);
            //设置参数的类型为输出参数,默认情况下是输入,
            sqlComm.Parameters["@strErrInfo"].Direction = ParameterDirection.Output;
            //为参数赋值
            sqlComm.Parameters["@strGameAccount"].Value = accout_name;
            sqlComm.Parameters["@iFreezeType"].Value = type;
            sqlComm.Parameters["@FreezeReason"].Value = reason;
            sqlComm.Parameters["@Operator"].Value = optor;
            //执行
            sqlComm.ExecuteNonQuery();
            //得到输出参数的值,把赋值给name,注意,这里得到的是object类型的,要进行相应的类型轮换
            ErrInfo = sqlComm.Parameters["@strErrInfo"].Value.ToString();
            mutex.ReleaseMutex();

            return ErrInfo;
        }

        public static string SqlCommand(string sqlLine)
        {
            if (!m_SqlConnected)
                return "false";

            mutex.WaitOne();

            string ret = "";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = sqlLine;
            sqlcmd.Connection = conn;
            try
            {
                sqlcmd.ExecuteNonQuery();
                ret = "success";
            }
            catch
            {
                ret = "false";
            }
            mutex.ReleaseMutex();
            return ret;
        }

        public static int SelectAcountPoint(string account)
        {
            if (!m_SqlConnected)
                return 0;

            int point = 0;
            string sqlstr = "";

            SqlCommand sqlComm = null;
            sqlstr = "SELECT [point]"
            + "FROM " + m_sqlAccount + ".dbo.game_acc "
            + "where [account] = '" + account + "' ";
            sqlComm = new SqlCommand(sqlstr, conn);

            mutex.WaitOne();

            SqlDataReader Dr = sqlComm.ExecuteReader();
            while (Dr.Read())
            {
                //读出内容列
                string _point = Dr["point"].ToString();
                point = int.Parse(_point);
            }
            Dr.Close();
            mutex.ReleaseMutex();

            return point;
        }
        public static List<LogSearch> SearchLogMsg(string msg, string t_start, string t_end)
        {
            List<LogSearch> loglist = new List<LogSearch>();
            if (!m_SqlConnected)
                return loglist;

            string sqlstr = "";

            SqlCommand sqlComm = null;
            sqlstr = "SELECT [log_time]"
            + ",[type]"
            + ",[name]"
            + ",[ip]"
            + ",[msg] "
            + "FROM " + m_sqlLog + ".dbo.Log_Talk_01 "
            + "where (log_time > '" + t_start + "' and log_time < '" + t_end + "') and (type = '组织公会通频' or type = '组织公会') and msg = upper('" + msg + "') "
            + " order by [log_time] asc";
            sqlComm = new SqlCommand(sqlstr, conn);

            mutex.WaitOne();
            SqlDataReader Dr = sqlComm.ExecuteReader();
            
            LogSearch item;
            while (Dr.Read())
            {
                //读出内容列
                item.log_time = Dr["log_time"].ToString();
                item.type = Dr["type"].ToString();
                item.name = Dr["name"].ToString();
                item.ip = Dr["ip"].ToString();
                item.msg = Dr["msg"].ToString();

                loglist.Add(item);
            }
            Dr.Close();
            mutex.ReleaseMutex();

            return loglist;
        }

        public static List<LogItemSearch> SearchLogItem(string max, string t_start, string t_end, List<string> items)
        {
            List<LogItemSearch> logitemlist = new List<LogItemSearch>();

            if (!m_SqlConnected)
                return logitemlist;

            string sqlstr = "";

            int maxLv = int.Parse(max);

            SqlCommand sqlComm = null;
            sqlstr = "SELECT [log_time]"
                    + ",[type]"
                    + ",[from_name]"
                    + ",[item_name]"
                    + ",[item_code]"
                    + " FROM " + m_sqlLog + ".dbo.Log_Item_01"
                    + " where log_time >= '" + t_start + "' and log_time <'" + t_end + "' and "
                    + " ((type = '合成强化物品' and (item_name like '+" + maxLv + "%' or item_name like '+" + (maxLv-1) + "%'))";

            string item_str = " or (type = '怪物掉落' and (";
            if (items.Count > 0)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    item_str += " item_name = '" + items[i] +"'";
                    if (i < items.Count - 1)
                    {
                        item_str += " or ";
                    }
                }
                item_str += "))";
            }
            sqlstr += item_str;
            sqlstr += ") order by [log_time] asc";

            sqlComm = new SqlCommand(sqlstr, conn);
            mutex.WaitOne();

            SqlDataReader Dr = sqlComm.ExecuteReader();
            
            LogItemSearch item;
            while (Dr.Read())
            {
                //读出内容列
                item.log_time = Dr["log_time"].ToString();
                item.type = Dr["type"].ToString();
                item.from_name = Dr["from_name"].ToString();
                item.item_name = Dr["item_name"].ToString();

                logitemlist.Add(item);
            }
            Dr.Close();
            mutex.ReleaseMutex();

            return logitemlist;
        }

        public static bool InsertSanvtItem(string account
            , UInt32 DataID1, UInt32 Number1
            , UInt32 DataID2, UInt32 Number2
            , UInt32 DataID3, UInt32 Number3
            , UInt32 DataID4, UInt32 Number4
            , UInt32 DataID5, UInt32 Number5)
        {
            if (!m_SqlConnected)
                return false;

            if (account == string.Empty)
            {
                return false;
            }
            if (   DataID1 == 0 && Number1 == 0
                && DataID2 == 0 && Number2 == 0
                && DataID3 == 0 && Number3 == 0
                && DataID4 == 0 && Number4 == 0
                && DataID5 == 0 && Number5 == 0)
            {
                return false;
            }

            string cmd = "INSERT INTO " + m_sqlSanvt + ".dbo.vitem (Account,Disable,Card,Login_time,Get_time,SName,CharName,Type,"
            + "DataID1,Number1,DataID2,Number2,DataID3,Number3,DataID4,Number4,DataID5,Number5)"
            + "values ('" + account + "',0,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "',getdate(),getdate(),0,0,0,"
            + "" + DataID1 + "," + Number1 + "," + DataID2 + "," + Number2 + "," + DataID3 + "," + Number3 + "," + DataID4 + "," + Number4 + "," + DataID5 + "," + Number5 + ")";

            mutex.WaitOne();
            string ret = CSGHelper.SqlCommand(cmd);
            mutex.ReleaseMutex();
            if (ret != "success")
            {
                return false;
            }
            return true;
        }
        public static bool AddAcountPoint(string account, int dbCount)
        {
            if (!m_SqlConnected)
                return false;

            string cmd = "DECLARE @account varchar(21) \n"
                                + "DECLARE @point int \n"
                                + "DECLARE @old_point int \n"
                                + "DECLARE @new_point int \n"
                                + "set @account = '" + account + "' \n"
                                + "set @point = " + dbCount + " \n"
                                + "Select @old_point=point from " + m_sqlAccount + ".dbo.game_acc where account = @account \n"
                                + "SET @new_point = @point + @old_point \n"
                                + "Update " + m_sqlAccount + ".dbo.game_acc set point = @new_point where account = @account";

            mutex.WaitOne();
            string ret = CSGHelper.SqlCommand(cmd);
            mutex.ReleaseMutex();
            if (ret != "success")
            {
                return false;
            }
            return true;
        }

        public static List<Account> SelectAcountInfo()
        {
            List<Account> accountInfo = new List<Account>();
            if (!m_SqlConnected)
                return accountInfo;

            string sqlstr = "SELECT * FROM " + m_sqlAccount + ".dbo.game_acc";

            SqlCommand sqlComm = new SqlCommand(sqlstr, conn);
            mutex.WaitOne();

            SqlDataReader Dr = sqlComm.ExecuteReader();

            Account info;
            while (Dr.Read())
            {
                //读出内容列
                info.account = Dr["account"].ToString();
                info.create_time = Dr["create_time"].ToString();
                info.duedate = Dr["duedate"].ToString();
                info.enable = Dr["enable"].ToString();
                info.first_ip = Dr["first_ip"].ToString();
                info.ip = Dr["ip"].ToString();
                info.IsAdult = Dr["IsAdult"].ToString();
                info.LastLoginTime = Dr["LastLoginTime"].ToString();
                info.LastLogoutTime = Dr["LastLogoutTime"].ToString();
                info.lock_duedate = Dr["lock_duedate"].ToString();
                info.logout_time = Dr["logout_time"].ToString();
                info.Offline_Time = Dr["Offline_Time"].ToString();
                info.OnlineTime = Dr["OnlineTime"].ToString();
                info.password = Dr["password"].ToString();
                info.password2 = Dr["password2"].ToString();
                info.point = Dr["point"].ToString();
                info.privilege = Dr["privilege"].ToString();
                info.sec_pwd = Dr["sec_pwd"].ToString();
                info.status = Dr["status"].ToString();
                info.trade_psw = Dr["trade_psw"].ToString();

                accountInfo.Add(info);
            }
            Dr.Close();
            mutex.ReleaseMutex();

            return accountInfo;
        }
    }

    
}
