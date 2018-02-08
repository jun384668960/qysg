using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace register_server
{
    class LogHelper
    {
        public static void WriteLog(string path, string msg, StackTrace st)
        {
            string logPath = path + @"log\";
            if (Directory.Exists(logPath))
            {
            }
            else
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(logPath);
                directoryInfo.Create();
            }

            string fileName = System.IO.Path.GetFileName(st.GetFrame(0).GetFileName());

            StreamWriter write = new StreamWriter(logPath + DateTime.Now.ToString("yyyy-MM-dd") + ".txt",true);

            write.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss\t") + msg + "\tfile:" + fileName + "\tMethod:" + st.GetFrame(0).GetMethod() + "\tLine Number:" + st.GetFrame(0).GetFileLineNumber());
            write.Flush();
            write.Close();
        }

        public static void WriteLog(string file, string msg)
        {
            if (file == string.Empty || msg == string.Empty)
            {
                return;
            }
            string logDir = AppDomain.CurrentDomain.BaseDirectory + @"log\";
            if (!Directory.Exists(@logDir))//若文件夹不存在则新建文件夹   
            {
                Directory.CreateDirectory(@logDir); //新建文件夹   
            }

            StreamWriter write = new StreamWriter(logDir + file, true);

            write.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss\t") + msg);
            write.Flush();
            write.Close();
        }
    }
}
