using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace register_client
{
    public class CSGHelper
    {
        public static bool SqlConn(string cnn_str)
        {
            //conn = new SqlConnection("Data Source = 127.0.0.1; Initial Catalog = Account; User Id = sa; Password = 123456;");
            conn = new SqlConnection(cnn_str); ;
            try
            {
                conn.Open();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool SqlClose()
        {
            conn.Close();

            return true;
        }
        public static string CreateAccount(string name, string passwd)
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

        private static SqlConnection conn;
    }
}
