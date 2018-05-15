using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MainServer
{
    public class CCmdDetail
    {
        public static string CmdDetail(byte[] rbyte)
        {
            string ErrInfo = "";
            Tran_Head rcv_h;
            rcv_h = CDataBuil.GetDataHead(rbyte);
            //MessageBox.Show(rcv_h.cmd + "f");

            switch (rcv_h.cmd)
            {
                case (int)CMD_E.CMD_REGISTER: 
                    {
                        Rg_Info rcv_d;
                        rcv_d = CDataBuil.GetDataObj<Rg_Info>(rbyte, rcv_h);
                        ErrInfo = CmdRegister(rcv_d);
                        break;
                    }
                case (int)CMD_E.CMD_GET_VER:
                    {
                        ErrInfo = CmdGetVersion();
                        break;
                    }
                case (int)CMD_E.CMD_MODY_PWD:
                    {
                        Mdfy_Pwd_Info modyInfo;
                        modyInfo = CDataBuil.GetDataObj<Mdfy_Pwd_Info>(rbyte, rcv_h);
                        ErrInfo = CmdModyPwd(modyInfo);
                        break;
                    }
                default:
                    ErrInfo = "result " + System.Text.Encoding.Unicode.GetString(rbyte);
                    break;
            }

            return ErrInfo;
        }
        public static string CmdRegister(Rg_Info rg_info)
        {
            string ErrInfo = "";
            try { 
                //找出传递过来的用户名和密码
                string name = "";
                string passwd = "";
                if (rg_info.key == "110")
                {
                    name = rg_info.name;
                    passwd = rg_info.passwd;
                    ErrInfo = CSGHelper.CreateAccount(name, passwd);
                    LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, "CreateAccount name:" + name + "\tpasswd:" + passwd + "\t" + ErrInfo, new StackTrace(new StackFrame(true)));

                }
                else 
                {
                    name = CSecurity.DecryptDES(rg_info.name, rg_info.key);
                    passwd = CSecurity.DecryptDES(rg_info.passwd, rg_info.key);
                    if (name == rg_info.name || passwd == rg_info.passwd)
                    {
                        ErrInfo = "注册失败，请稍后再试！";
                    }
                    else
                    {
                        ErrInfo = CSGHelper.CreateAccount(name, passwd);
                        LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, "CreateAccount name:" + name + "\tpasswd:" + passwd, new StackTrace(new StackFrame(true)));
                    } 
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, ex.Message, new StackTrace(new StackFrame(true)));
                ErrInfo = "注册失败，请稍后再试！";
            }


            return ErrInfo;
        }

        public static string CmdModyPwd(Mdfy_Pwd_Info modyInfo)
        {
            string ErrInfo = "";
            try
            {
                //找出传递过来的用户名和密码
                string name = "";
                string oldpasswd = "";
                string newpasswd = "";
                if (modyInfo.key == "110")
                {
                    name = modyInfo.name;
                    oldpasswd = modyInfo.oldpasswd;
                    newpasswd = modyInfo.newpasswd;
      
                    ErrInfo = CSGHelper.ModifyPwd(name, oldpasswd, newpasswd);
                    LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, "ModifyPwd name:" + name + "\toldpasswd:" + oldpasswd + "\tnewpasswd:" + newpasswd, new StackTrace(new StackFrame(true)));
                }
                else 
                {
                    name = CSecurity.DecryptDES(modyInfo.name, modyInfo.key);
                    oldpasswd = CSecurity.DecryptDES(modyInfo.oldpasswd, modyInfo.key);
                    newpasswd = CSecurity.DecryptDES(modyInfo.newpasswd, modyInfo.key);
                    if (name == modyInfo.name || oldpasswd == modyInfo.oldpasswd
                        || newpasswd == modyInfo.newpasswd)
                    {
                        ErrInfo = "修改失败，请稍后再试！";
                    }
                    else
                    {
                        ErrInfo = CSGHelper.ModifyPwd(name, oldpasswd, newpasswd);
                        LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, "ModifyPwd name:" + name + "\toldpasswd:" + oldpasswd + "\tnewpasswd:" + newpasswd, new StackTrace(new StackFrame(true)));
                    }
                }
                
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, ex.Message, new StackTrace(new StackFrame(true)));
                ErrInfo = "注册失败，请稍后再试！";
            }


            return ErrInfo;
        }

        public static string CmdGetVersion()
        {
            string ErrInfo = "";

            try
            {
                string versionIni = System.AppDomain.CurrentDomain.BaseDirectory + verIni;

                //找出传递过来的用户名和密码
                string ver = "";
                string link = "111";
                ver = CIniCtrl.ReadIniData("Config", "loginVersion", "", versionIni);

                if (File.Exists("D:\\sgserver\\FtpRoot\\Update.zip"))
                {
                    System.IO.FileInfo f = new FileInfo("D:\\sgserver\\FtpRoot\\Update.zip");
                    ErrInfo = ver + "版本" + link + "长度" + f.Length.ToString();
                }
                else 
                {
                    ErrInfo = ver + "版本" + link + "长度1";
                }
                
                LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, ErrInfo, new StackTrace(new StackFrame(true)));
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, ex.Message, new StackTrace(new StackFrame(true)));
                ErrInfo = "error";
            }

            return ErrInfo;
        }
        private static string verIni = "配置文件.ini";
    }
    public class StateObject
    {
        // Client socket.     
        public Socket workSocket = null;
        // Size of receive buffer.     
        public const int BufferSize = 1024;
        // Receive buffer.     
        public byte[] buffer = new byte[BufferSize];
        // Received data string.     
        public StringBuilder sb = new StringBuilder();
    }
    public class CSocketHelper
    {
        public static bool g_bListenStop = false;
        public static Socket listener;
        private delegate void AcceptDelegete(int port);
        // Thread signal.     
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        public CSocketHelper()
        {
            //LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, "SendCallback", new StackTrace(new StackFrame(true)));
        }

        public static void StartListening(int port)
        {
            g_bListenStop = false;
            AcceptDelegete listen = AcceptConnection;
            IAsyncResult asy = listen.BeginInvoke(port, null, null);
        }
        public static void StopListening(int port)
        {
            g_bListenStop = true;
            listener.Close();
        }
        public static void AcceptConnection(int port)
        {
            // Data buffer for incoming data.     
            byte[] bytes = new Byte[1024];
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
            // Create a TCP/IP socket.     
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // Bind the socket to the local     
            //endpoint and listen for incoming connections.     
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (!g_bListenStop)
                {
                    // Set the event to nonsignaled state.     
                    allDone.Reset();
                    // Start an asynchronous socket to listen for connections.     
                    //MessageBox.Show("Waiting for a connection...");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                    // Wait until a connection is made before continuing.     
                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            //MessageBox.Show("\nPress ENTER to continue...");
        }
        public static void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                // Signal the main thread to continue.     
                allDone.Set();
                // Get the socket that handles the client request.     
                Socket listener = (Socket)ar.AsyncState;
                Socket handler = listener.EndAccept(ar);
                // Create the state object.     
                StateObject state = new StateObject();
                state.workSocket = handler;
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, "AcceptCallback" + ex.Message, new StackTrace(new StackFrame(true)));
            }
        }
        public static void ReadCallback(IAsyncResult ar)
        {
            try
            {
                String content = String.Empty;
                // Retrieve the state object and the handler socket     
                // from the asynchronous state object.     
                StateObject state = (StateObject)ar.AsyncState;
                Socket handler = state.workSocket;
                // Read data from the client socket.     
                int bytesRead = handler.EndReceive(ar);
                if (bytesRead > 0)
                {
                    //解析数据
                    content = CCmdDetail.CmdDetail(state.buffer);

                    /*
                    // There might be more data, so store the data received so far.     
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                    // Check for end-of-file tag. If it is not there, read     
                    // more data.     
                    content = state.sb.ToString();
                    
                    if (content.IndexOf("<EOF>") > -1)
                    * */
                    if (content != "" || content != null)
                    {
                        LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, content, new StackTrace(new StackFrame(true)));
                        // All the data has been read from the     
                        // client. Display it on the console.     
                        //MessageBox.Show(string.Format("Read {0} bytes from socket. \n Data : {1}", content.Length, content));
                        // Echo the data back to the client.     
                        Send(handler, content);
                    }
                    else
                    {
                        // Not all data received. Get more.     
                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                    }
                }
            }catch(Exception ex)
            {
                Socket handler = (Socket)ar.AsyncState;
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, "ReadCallback" + ex.Message, new StackTrace(new StackFrame(true)));
            }
        }
        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.     
            byte[] byteData = Encoding.Unicode.GetBytes(data);
            // Begin sending the data to the remote device.     
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
        }
        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.     
                Socket handler = (Socket)ar.AsyncState;
                // Complete sending the data to the remote device.     
                int bytesSent = handler.EndSend(ar);
                //MessageBox.Show(string.Format("Sent {0} bytes to client.", bytesSent));
                //handler.Shutdown(SocketShutdown.Both);
                //handler.Close();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(System.AppDomain.CurrentDomain.BaseDirectory, "SendCallback" + ex.Message, new StackTrace(new StackFrame(true)));
            }
        }

    }
}
