using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace SGDataHelper
{
    public class CSGHelper
    {
        public bool SqlConn(string cnn_str) {
            //conn = new SqlConnection("Data Source = 127.0.0.1; Initial Catalog = Account; User Id = sa; Password = 123456;");
           
            try
            {
                conn = new SqlConnection(cnn_str); ;
                conn.Open();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool SqlClose()
        {
            conn.Close();

            return true;
        }
        public string CreateAccount(string name, string passwd)
        {

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

            return ErrInfo;
        }

        public string ModifyPwd(string name, string oldpasswd, string newpasswd)
        {

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

            return ErrInfo;
        }

        public List<string> SearchStrCount(string str, DateTime startTime, DateTime endTime)
        {
            string sqlstr = "";

            SqlCommand sqlComm = null;
            //*
            //sqlstr = "SELECT name FROM [sanollog].[dbo].[Log_Talk_01] where name_dest = '" + str + "' and (log_time > '" + startTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and log_time < '" + endTime.ToString("yyyy-MM-dd HH:mm:ss") + "')";
            sqlstr = str + " and (log_time > '" + startTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and log_time < '" + endTime.ToString("yyyy-MM-dd HH:mm:ss") + "')";
            sqlComm = new SqlCommand(sqlstr, conn);
            //执行msg
            SqlDataReader Dr = sqlComm.ExecuteReader();
            List<string> namelist = new List<string>();
            while (Dr.Read())
            {
                //count = int.Parse(Dr["name"].ToString());
                //读出内容列
                string str1 = Dr["name"].ToString();
                if (!namelist.Contains(str1))
                {
                    namelist.Add(str1);
                }
                
            }
            Dr.Close();
            return namelist;
            /*
            foreach(var item in namelist)
            {
                sqlstr = "SELECT count(*) as number FROM [sanollog].[dbo].[Log_Talk_01] where name_dest = '" + str + "' and name='" + item + "'  and (log_time > '" + startTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and log_time < '" + endTime.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                sqlComm = new SqlCommand(sqlstr, conn);
                Dr = sqlComm.ExecuteReader();
                if (Dr.Read())
                {
                    //读出内容列
                    count = int.Parse(Dr["number"].ToString());
                }
                Dr.Close();

                if (count > 5)
                {
                    //根据游戏名获取账户名
                    SGDataCtrl.GetPlayAttr();
                    string acc = SGDataCtrl.GameStrToSimpleCN(item);
                    //冻结账户
                    FreezeAccount(acc, 1, "异常登录！或使用贝贝登录。","GM");
                    //ret = "账户：" + item + " 冻结操作："+ FreezeAccount(item, 1, "异常登录！或使用贝贝登录。", "GM") + "\r\n";
                }
            }
            namelist.Clear();

            return ret;
             * */
        }

        public bool IsIllegal(List<string> listenString)
        {
            foreach (var item in listenString)
            {
                int count = 0;// SearchStrCount(item, DateTime.Now.AddHours(-24), DateTime.Now);
                if (count>=5)
                {
                    return true;
                }
            }

            return false;
        }

        public string FreezeAccount(string accout_name, int type, string reason, string optor)
        {
            string ErrInfo = "";
            SqlCommand sqlComm = null;
            //*
            sqlComm = new SqlCommand("AC_sp_FreezeAccount", conn);
            //设置命令的类型为存储过程
            sqlComm.CommandType = CommandType.StoredProcedure;
            //设置参数
            sqlComm.Parameters.Add("@strGameAccount", SqlDbType.VarChar);
            sqlComm.Parameters.Add("@iFreezeType", SqlDbType.VarChar);
            sqlComm.Parameters.Add("@FreezeReason", SqlDbType.VarChar);
            sqlComm.Parameters.Add("@Operator", SqlDbType.VarChar);
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

            return ErrInfo;
        }

        public bool SqlCommand(string sqlLine)
        {
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = sqlLine;
            sqlcmd.Connection = conn;
            try
            {
                sqlcmd.ExecuteNonQuery();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }

        }

        public object SqlCommandSingleRet(string sqlLine)
        {
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = sqlLine;
            sqlcmd.Connection = conn;
            try
            {
                return sqlcmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        private SqlConnection conn;
    }
}
