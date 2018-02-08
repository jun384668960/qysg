using System;
using System.Collections.Generic;
using System.Text;
using FlyFramework;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace register_client
{
    class tcp_client
    {
        /// <summary> 

        /// 命令部分的长度 

        /// </summary> 

        private static readonly int CMDLEN = 50;

        /// <summary> 

        /// 命令注释部分的长度 

        /// </summary> 

        private static readonly int DESCLEN = 100;

        /// <summary> 

        /// 可变长度的长度信息部分所占的字节数 

        /// </summary> 

        private static readonly int DYNAMICLENGTHLEN = 10;

        /// <summary> 

        /// 每次处理可变信息部分的长度 

        /// </summary> 

        private static readonly int DEALLEN = 1024;

        /// <summary> 

        /// /应答的最大长度 

        /// </summary> 

        private static readonly int RESPONLEN = 20;

        /// <summary> 

        /// 用于填充命令或注释不足长度部分的字符 

        /// </summary> 

        private static readonly char FILLCHAR = '^';

        /// <summary> 

        /// 成功发送一部分数据后的回调方法(也可以认为是触发的事件，但严格来说还不是) 

        /// </summary> 

        public delegate void OnSend(int iTotal, int iSending);

        /// <summary> 

        /// 根据给定的服务器和端口号建立连接 

        /// </summary> 

        /// <param name="strHost">服务器名</param> 

        /// <param name="iPort">端口号</param> 

        /// <returns></returns> 

        public static Socket ConnectToServer(string strHost, int iPort)
        {

            try
            {

                IPAddress ipAddress = Dns.Resolve(strHost).AddressList[0];

                IPEndPoint ipPoint = new IPEndPoint(ipAddress, iPort);



                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                s.Connect(ipPoint);

                return s;

            }

            catch (Exception e)
            {

                throw (new Exception("建立到服务器的连接出错" + e.Message));

            }

        }

        /// <summary> 

        /// 将文本写到Socket中 

        /// </summary> 

        /// <param name="s">要发送信息的Socket</param> 

        /// <param name="strInfo">要发送的信息</param> 

        /// <returns>是否成功</returns> 

        public static bool WriteTextToSocket(Socket s, string strInfo)
        {

            byte[] buf = Encoding.UTF8.GetBytes(strInfo);

            try
            {

                s.Send(buf, 0, buf.Length, SocketFlags.None);

                return true;

            }

            catch (Exception err)
            {

                MessageBox.Show("发送文本失败！" + err.Message);

                return false;

            }

        }

        /// <summary> 

        /// 将命令文本写到Socket中 

        /// </summary> 

        /// <param name="s">要发送命令文本的Socket</param> 

        /// <param name="strInfo">要发送的命令文本</param> 

        /// <returns>是否成功</returns> 

        public static bool WriteCommandToSocket(Socket s, string strCmd)
        {

            if (strCmd == "")

                strCmd = "NOP";

            strCmd = strCmd.PadRight(CMDLEN, FILLCHAR);

            return WriteTextToSocket(s, strCmd);

        }

        /// <summary> 

        /// 将命令注释写到Socket中 

        /// </summary> 

        /// <param name="s">要发送命令注释的Socket</param> 

        /// <param name="strInfo">要发送的命令注释</param> 

        /// <returns>是否成功</returns> 

        public static bool WriteCommandDescToSocket(Socket s, string strDesc)
        {

            if (strDesc == "")

                strDesc = "0";

            strDesc = strDesc.PadRight(DESCLEN, FILLCHAR);

            return WriteTextToSocket(s, strDesc);

        }

        /// <summary> 

        /// 发送可变信息的字节数 

        /// </summary> 

        /// <param name="s">要发送字节数的Socket</param> 

        /// <param name="iLen">字节数</param> 

        /// <returns>是否成功</returns> 

        public static bool WriteDynamicLenToSocket(Socket s, int iLen)
        {
            string strLen = iLen.ToString().PadRight(DYNAMICLENGTHLEN, FILLCHAR);

            return WriteTextToSocket(s, strLen);

        }

        /// <summary> 

        /// 将缓存的指定部分发送到Socket 

        /// </summary> 

        /// <param name="s">要发送缓存的Socket</param> 

        /// <param name="buf">要发送的缓存</param> 

        /// <param name="iStart">要发送缓存的起始位置</param> 

        /// <param name="iCount">要发送缓存的字节数</param> 

        /// <param name="iBlock">每次发送的字节说</param> 

        /// <param name="SendSuccess">每次发送成功后的回调函数</param> 

        /// <returns>是否发送成功</returns>
        public static bool WriteBufToSocket(Socket s, byte[] buf, int iStart, int iCount, int iBlock, OnSend SendSuccess)
        {

            int iSended = 0;

            int iSending = 0;

            while (iSended < iCount)
            {

                if (iSended + iBlock <= iCount)

                    iSending = iBlock;

                else

                    iSending = iCount - iSended;

                s.Send(buf, iStart + iSended, iSending, SocketFlags.None);

                iSended += iSending;

                if (ReadResponsionFromSocket(s) == "OK")

                    if (SendSuccess != null)

                        SendSuccess(iCount, iSended);

                    else

                        return false;

            }

            return true;

        }

        /// <summary> 

        /// 将长度不固定文本发送到socket 

        /// </summary> 

        /// <param name="s">要发送文本的Socket</param> 

        /// <param name="strText">要发送的文本</param> 

        /// <param name="OnSendText">成功发送一部分文本后的回调函数</param> 

        /// <param name="settextlen">得到文本长度的回调函数</param> 

        /// <returns></returns> 

        public static bool WriteDynamicTextToSocket(Socket s, string strText, OnSend OnSendText)
        {

            byte[] buf = Encoding.UTF8.GetBytes(strText);



            int iLen = buf.Length;

            try
            {

                WriteDynamicLenToSocket(s, iLen);

                return WriteBufToSocket(s, buf, 0, iLen, DEALLEN, OnSendText);

            }

            catch (Exception err)
            {

                MessageBox.Show("发送文本失败！" + err.Message);

                return false;

            }

        }

        /// <summary> 

        /// 将文件写到Socket 

        /// </summary> 

        /// <param name="s">要发送文件的Socket</param> 

        /// <param name="strFile">要发送的文件</param> 

        /// <returns>是否成功</returns> 

        public static bool WriteFileToSocket(Socket s, string strFile, OnSend OnSendFile)
        {

            FileStream fs = new FileStream(strFile, FileMode.Open, FileAccess.Read, FileShare.Read);

            int iLen = (int)fs.Length;

            WriteDynamicLenToSocket(s, iLen);

            byte[] buf = new byte[iLen];

            try
            {

                fs.Read(buf, 0, iLen);

                return WriteBufToSocket(s, buf, 0, iLen, DEALLEN, OnSendFile);

            }

            catch (Exception err)
            {

                MessageBox.Show("发送文件失败！" + err.Message);

                return false;

            }

            finally
            {

                fs.Close();

            }

        }

        /// <summary> 

        /// 对方对自己消息的简单回应 

        /// </summary> 

        /// <param name="s"></param> 

        /// <returns></returns> 

        public static string ReadResponsionFromSocket(Socket s)
        {

            byte[] bufCmd = new byte[RESPONLEN];

            int iCount = s.Receive(bufCmd);

            string strRespon = Encoding.UTF8.GetString(bufCmd, 0, iCount);

            return strRespon;

        }

        /// <summary> 

        /// 从Socket读取命令 

        /// </summary> 

        /// <param name="s">要读取命令的Socket</param> 

        /// <returns>读取的命令</returns> 

        public static string ReadCommandFromSocket(Socket s)
        {

            byte[] bufCmd = new byte[CMDLEN];

            int iCount = s.Receive(bufCmd, 0, CMDLEN, SocketFlags.Partial);

            string strCommand = Encoding.UTF8.GetString(bufCmd, 0, CMDLEN);

            return strCommand = strCommand.TrimEnd(FILLCHAR);

        }

        /// <summary> 

        /// 读取命令注释 

        /// </summary> 

        /// <param name="s">要读取命令注释的Socket</param> 

        /// <returns>读取的命令注释</returns> 

        public static string ReadCommandDescFromSocket(Socket s)
        {

            byte[] bufCmd = new byte[DESCLEN];

            int iCount = s.Receive(bufCmd, 0, DESCLEN, SocketFlags.Partial);

            string strCommand = Encoding.UTF8.GetString(bufCmd, 0, DESCLEN);

            return strCommand = strCommand.TrimEnd(FILLCHAR);

        }

        /// <summary> 

        /// 读取可变部分的长度 

        /// </summary> 

        /// <param name="s">要读取可变部分长度的Socket</param> 

        /// <returns>读取的可变部分的长度</returns> 

        public static int ReadDynamicLenFromSocket(Socket s)
        {

            byte[] bufCmd = new byte[DYNAMICLENGTHLEN];

            int iCount = s.Receive(bufCmd, 0, DYNAMICLENGTHLEN, SocketFlags.Partial);

            string strCommand = Encoding.UTF8.GetString(bufCmd, 0, DYNAMICLENGTHLEN);

            return int.Parse(strCommand.TrimEnd(FILLCHAR));

        }

        /// <summary> 

        /// 读取文本形式的可变信息 

        /// </summary> 

        /// <param name="s">要读取可变信息的Socket</param> 

        /// <returns>读取的可变信息</returns> 

        public static string ReadDynamicTextFromSocket(Socket s)
        {

            int iLen = ReadDynamicLenFromSocket(s);



            byte[] buf = new byte[iLen];

            string strInfo = "";



            int iReceiveded = 0;

            int iReceiveing = 0;

            while (iReceiveded < iLen)
            {

                if (iReceiveded + DEALLEN <= iLen)

                    iReceiveing = DEALLEN;

                else

                    iReceiveing = iLen - iReceiveded;

                s.Receive(buf, iReceiveded, iReceiveing, SocketFlags.None);

                tcp_client.WriteTextToSocket(s, "OK");

                iReceiveded += iReceiveing;

            }



            strInfo = Encoding.UTF8.GetString(buf, 0, iLen);



            return strInfo;

        }

        /// <summary> 

        /// 读取文件形式的可变信息 

        /// </summary> 

        /// <param name="s">要读取可变信息的Socket</param> 

        /// <param name="strFile">读出后的文件保存位置</param> 

        /// <returns>是否读取成功</returns> 

        public static bool ReadDynamicFileFromSocket(Socket s, string strFile)
        {

            int iLen = ReadDynamicLenFromSocket(s);

            byte[] buf = new byte[iLen];

            FileStream fs = new FileStream(strFile, FileMode.Create, FileAccess.Write);



            try
            {

                int iReceiveded = 0;

                int iReceiveing = 0;

                while (iReceiveded < iLen)
                {

                    if (iReceiveded + DEALLEN <= iLen)

                        iReceiveing = DEALLEN;

                    else

                        iReceiveing = iLen - iReceiveded;

                    s.Receive(buf, iReceiveded, iReceiveing, SocketFlags.None);

                    tcp_client.WriteTextToSocket(s, "OK");

                    iReceiveded += iReceiveing;

                }

                fs.Write(buf, 0, iLen);

                return true;

            }

            catch (Exception err)
            {

                MessageBox.Show("接收文件失败" + err.Message);

                return false;

            }

            finally
            {

                fs.Close();

            }

        } 
    }
}
