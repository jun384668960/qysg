using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace MainServer
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new WinMainServer());
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            string msg = string.Format("捕获到未处理异常：{0}\r\n异常信息：{1}\r\n异常堆栈：{2}\r\nCLR即将退出：{3}", ex.GetType(), ex.Message, ex.StackTrace, e.IsTerminating);
            LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, msg, new StackTrace(new StackFrame(true)));
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            string msg = string.Format("捕获到未处理异常：{0}\r\n异常信息：{1}\r\n异常堆栈：{2}", ex.GetType(), ex.Message, ex.StackTrace);
            LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, msg, new StackTrace(new StackFrame(true)));
        }
    }
}
