using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Common;
using register_server;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace 注册网关
{
    class SGExHandle
    {
        public SGExHandle(){
            m_TaskTime.date = new List<int>();
            m_TaskTime.time = new List<string>();
        }
        #region  //封装发送系统公共
        private string m_config = "";
        IntPtr g_wLoginWindow = IntPtr.Zero;
        IntPtr g_bReload = IntPtr.Zero;
        IntPtr g_bWorldSend = IntPtr.Zero;
        const int BM_CLICK = 0xF5;

        public bool SetConfigPath(string config)
        {
            m_config = config;
            return true;
        }

        public bool LoadLoginServerPtr(string wName)
        {
            try
            {
                g_wLoginWindow = ProxyWndHandle.FindMainWindowHandle("Login Server(qyws)(ItemMall)(Apr 28 2009 15:28:02)", 100, 25);
                if (g_wLoginWindow == IntPtr.Zero)
                {
                    LogHelper.WriteLog(DateTime.Now.ToString("yyyy-MM-dd") + "答题日志.txt", "FindMainWindowHandle error!");
                    return false;
                }
                g_bReload = FindWindowEx(g_wLoginWindow, IntPtr.Zero, null, "重新读取设定");
                if (g_bReload == IntPtr.Zero)
                {
                    return false;
                }
                g_bWorldSend = FindWindowEx(g_wLoginWindow, IntPtr.Zero, null, "送出");
                if (g_bWorldSend == IntPtr.Zero)
                {
                    return false;
                }

            }
            catch (Exception)
            {
                g_wLoginWindow = IntPtr.Zero;
                return false;
            }
            return true;
        }

        public bool SendWorldWords(string words)
        {
            if (g_wLoginWindow == IntPtr.Zero)
            {
                if (!LoadLoginServerPtr(""))
                    return false;
            }
            //写Msg_Broadcast到loginServer.ini
            if (m_config == string.Empty)
            {
                return false;
               
            }
            CIniCtrl.WriteIniData("System", "Msg_Broadcast", words, m_config);

            //重读
            if (g_bReload != IntPtr.Zero)
            {
                SendMessage(g_bReload, BM_CLICK, 0, 0);
            }
            Thread.Sleep(500);
            //送出
            if (g_bReload != IntPtr.Zero)
            {
                SendMessage(g_bWorldSend, BM_CLICK, 0, 0);
            }

            return true;
        }

        public bool ReloadLoginServer()
        {
            if (g_wLoginWindow == IntPtr.Zero)
            {
                if (!LoadLoginServerPtr(""))
                    return false;
            }
            //重读
            if (g_bReload != IntPtr.Zero)
            {
                SendMessage(g_bReload, BM_CLICK, 0, 0);
            }

            return true;
        }
        #endregion

        #region //加持公共

        private Thread m_Thread15Ann = null;
        private static bool m_15AnnStop = false;
        private static AutoResetEvent m_15AnnARE = new AutoResetEvent(false);
        private static string[] m_FilterList;
        private static int m_AnnSrchInterVal = 2;

        private static string m_MaxAnn = "15";
        List<string> m_AnnItemsList = new List<string>();

        public void SetAnnItems(string fileName)
        {
            m_AnnItemsList.Clear();
            try
            {
                //读取
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader reader = new StreamReader(fs, Encoding.Default);
                reader.DiscardBufferedData();
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                reader.BaseStream.Position = 0;

                string strLine = "";
                strLine = reader.ReadLine();
                while (strLine != null)
                {
                    m_AnnItemsList.Add(strLine);

                    strLine = "";
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

        public void SetMaxAnnString(string MaxAnn)
        {
            m_MaxAnn = MaxAnn;
        }
        public void SetFilterString(string filter)
        {
            m_FilterList = filter.Split(',');
        }
        public void SetSrchInterVal(int value)
        {
            m_AnnSrchInterVal = value;
        }
        private void Game_15Talk_Thread(object obj)
        {
            string tmp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            while (true)
            {
                if (!m_15AnnStop)
                {
                    string t_start = tmp;
                    //string t_end = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    string t_end = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    tmp = t_end;
                    //查询数据库
                    List<LogItemSearch> logList = CSGHelper.SearchLogItem(m_MaxAnn, t_start, t_end, m_AnnItemsList);
                    foreach (var item in logList)
                    {
                        bool send = true;
                        if (m_FilterList != null)
                        {
                            foreach (var name in m_FilterList)
                            {
                                if (name == item.from_name)
                                {
                                    send = false;
                                    break;
                                }
                            }
                        }
                        
                        if (send)
                        {
                            string words = "";
                            if (item.type == "合成强化物品")
                            {
                                words = "贺！玩家 " + item.from_name + " 经千锤百炼，炼造出 " + item.item_name + "。";
                                SendWorldWords(words);
                                //日志
                                LogHelper.WriteLog("加持公共日志.txt", words);
                            }
                            else if (item.type == "怪物掉落")
                            {
                                words = "这是何等人品，玩家 " + item.from_name + " 在怪物掉落获得了 " + item.item_name + "，真是可喜可贺！";

                                SendWorldWords(words);
                                //日志
                                LogHelper.WriteLog("怪物掉落公共日志.txt", words);
                            }
                        }
                    }

                    Thread.Sleep(m_AnnSrchInterVal * 1000);
                }
                else
                {
                    m_15AnnARE.WaitOne();
                }
            }
        }
        public bool Start15AnnThread()
        {
            if (m_Thread15Ann == null)
            {
                m_Thread15Ann = new Thread(new ParameterizedThreadStart(Game_15Talk_Thread));
                m_Thread15Ann.Start(this);
                m_15AnnStop = false;
            }
            else
            {
                m_15AnnARE.Set();
                m_15AnnStop = false;
            }
            return true;
        }
        public bool Stop15AnnThread()
        {
            m_15AnnStop = true;
            return true;
        }
        public bool Abort15AnnThread()
        {
            if (m_Thread15Ann != null)
            {
                m_Thread15Ann.Abort();
            }
            return true;
        }
        #endregion

        #region //在线答题
        public struct TaskTime {
            public List<int> date;
            public List<string> time;
        };

        private BankHandle bankHandle = new BankHandle();
        private TaskTime m_TaskTime = new TaskTime();
        private bool m_IsTaskTime = false;
        private Thread m_ThreadQA = null;
        private static bool m_QAStop = false;
        private static AutoResetEvent m_QAARE = new AutoResetEvent(false);
        private static int m_SleepCount = 60 * 30;
        private static int m_AskNormalInterval = 60 * 30;
        private static int m_QuesInterval = 30;
        private static int m_QACount = 0;
        private static int m_MaxQuesNum = 30;
        private static int m_QuesMode = 0;
        private static int m_AnswerVtId = 1;
        private static string m_AnswerVtName = "";
        private static string m_PlayerDat = "";
        private static string m_SanVtSql = "";
        public bool LoadAQBank(string file)
        {
            return bankHandle.LoadBankItem(file);
        }
        public void SetTaskTime(List<int> date, List<string> time)
        {
            m_TaskTime.date.Clear();

            m_TaskTime.date = date;
            m_TaskTime.time = time;
        }
        public void SetNormalInterval(int value)
        {
            m_AskNormalInterval = value;
        }

        public void SetMaxQuesNum(int value)
        {
            m_MaxQuesNum = value;
        }

        public void SetQuesMode(int mode)
        {
            m_QuesMode = mode;
        }

        public void SetQAReward(int id, string name)
        {
            m_AnswerVtId = id;
            m_AnswerVtName = name;
        }

        public void SetQADatVt(string dat, string sanvt)
        {
            m_PlayerDat = dat;
            m_SanVtSql = sanvt;
        }

        public struct AnswerResult_S
        {
            public string name;
            public UInt32 score;
        };
        List<AnswerResult_S> answerResult = new List<AnswerResult_S>();

        private bool SingleAQHandle(int flag, string player_dat, string sanvt)
        {
            long tick = DateTime.Now.Ticks;
            Random ran = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
            List<BankItem> list = bankHandle.GetBankItemList();

            int n = ran.Next(0, bankHandle.GetBankItemCount());
            BankItem item;
            bool ret = bankHandle.GetItem(n, out item);

            string tmp = "";
            string nextQues = "";
            if (flag == 0)//普通
            {
                tmp = "在线问答题:";
                nextQues = " 约" + m_AskNormalInterval / 60 + "分钟后开始下一题，请关注公屏提问！";
            }
            else //系列
            {
                m_QACount++;
                tmp = "第" + m_QACount + "题:";
                nextQues = " 十秒后开始下一题，请做好准备！";
            }

            string t_start, t_end;
            SendWorldWords(tmp + item.question);
            //记录发题时间 //2017-01-02 13:01:47.000 System.DateTime.Now.ToString("f"); //2008-4-24 16:30:15
            t_start = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Thread.Sleep(m_QuesInterval * 1000);
            //记录结尾时间
            t_end = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //查询数据库
            List<LogSearch> logList = CSGHelper.SearchLogMsg(item.answer, t_start, t_end);

            string answerEx = "";
            if (logList.Count > 0)
            {
                LogSearch firtItem = logList[0];
                answerEx = "恭喜 " + firtItem.name + " 抢答成功";

                if (flag == 0)
                {
                    CPlayerCtrl.PlayersAttrListClear();
                    CPlayerCtrl.LoadPlayerInfos(player_dat, true);
                    string account = CPlayerCtrl.GetAccByName(firtItem.name);
                    //发放虚宝
                    bool vtret = CSGHelper.InsertSanvtItem(sanvt, account, (uint)m_AnswerVtId, 1, 0, 0, 0, 0, 0, 0, 0, 0);
                    if (vtret)
                    {
                        answerEx += " 答题奖励 " + m_AnswerVtName + " 已经发放，请注意查收(虚宝)！";
                    }
                    //日志
                    LogHelper.WriteLog(DateTime.Now.ToString("yyyy-MM-dd") + "答题日志.txt", answerEx);
                }
                else
                {
                    //日志
                    LogHelper.WriteLog(DateTime.Now.ToString("yyyy-MM-dd") + "答题日志.txt", answerEx);

                    //遍历参与者列表
                    AnswerResult_S newNode;
                    bool found = false;

                    for (int i = 0; i < answerResult.Count; i++)
                    {
                        if (answerResult[i].name == firtItem.name)
                        {
                            newNode = answerResult[i];
                            newNode.score++;
                            answerResult[i] = newNode;
                            found = true;
                            break;
                        }
                    }
                    if (!found)//未找到
                    {
                        //add
                        newNode.name = firtItem.name;
                        newNode.score = 1;
                        answerResult.Add(newNode);
                    }
                }

            }
            else
            {
                answerEx = "很遗憾，无人能正确答对此题";
            }

            SendWorldWords("答案是:" + item.answer + " " + answerEx + nextQues);
            Thread.Sleep(10 * 1000);

            return true;
        }
        private bool IsTaskTime()
        {
            int date = Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"));

            foreach (var dataT in m_TaskTime.date)
            {
                if (dataT == date)
                {
                    foreach (var start in m_TaskTime.time)
                    {
                        //时间比较
                        DateTime dtStart = Convert.ToDateTime(start);

                        DateTime dtEnd = dtStart.AddMinutes(3);

                        if (DateTime.Compare(DateTime.Now, dtStart) > 0
                            && DateTime.Compare(dtEnd, DateTime.Now) > 0)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        private void Game_QA_Thread(object obj)
        {
            while (true)
            {
                if (!m_QAStop)
                {
                    //check time
                    if (IsTaskTime())
                    {
                        answerResult.Clear();
                        while (m_QACount < m_MaxQuesNum)
                        {
                            if (m_QACount == 0)
                            {
                                SendWorldWords("三国知识竞答即将开始！请各位玩家做好答题准备,找个安静的地图或者关闭不需要的频道");
                                Thread.Sleep(10 * 1000);
                                SendWorldWords("最先给出正确答案者获得积分，答题需在军团或者军团互通中，发送且仅发送正确选项，否则答题将会被认为无效而过滤。");
                                Thread.Sleep(20 * 1000);
                                SendWorldWords("答题开始！");
                                Thread.Sleep(2 * 1000);
                                
                            }
                            SingleAQHandle(1, m_PlayerDat, m_SanVtSql);
                        }
                        m_IsTaskTime = false;
                        //遍历参与者列表排名
                        answerResult.Sort(new Comparison<AnswerResult_S>(AnswerResultCompare));
                        if (answerResult.Count <= 0)
                        {
                            string msg = "很遗憾，本次答题无人优胜！";
                            LogHelper.WriteLog(DateTime.Now.ToString("yyyy-MM-dd") + @"答题日志.txt", msg);
                            SendWorldWords(msg);
                        }
                        else
                        {
                            int num = 0;
                            List<AnswerResult_S> ff = new List<AnswerResult_S>();
                            AnswerResult_S tmp = new AnswerResult_S();
                            tmp.score = 0;
                            for (int i = 0; i < answerResult.Count; i++)
                            {
                                if (tmp.score == answerResult[i].score)
                                {
                                    tmp.name += "," + answerResult[i].name;
                                    ff[num - 1] = tmp;
                                }
                                else
                                {
                                    tmp = answerResult[i];
                                    ff.Add(tmp);
                                    num++;
                                }
                            }

                            int best = ff.Count > 3 ? 3 : ff.Count;
                            for (int i = 0; i < best; i++)
                            {
                                string msg = "第" + (i + 1) + "名 总得分:" + ff[i].score + " 玩家 " + ff[i].name + "。";
                                LogHelper.WriteLog(DateTime.Now.ToString("yyyy-MM-dd") + @"答题日志.txt", msg);

                                //公告
                                SendWorldWords(msg);
                            }
                            for (int i = 0; i < best; i++)
                            {
                                //发放虚宝
                                CPlayerCtrl.PlayersAttrListClear();
                                CPlayerCtrl.LoadPlayerInfos(m_PlayerDat, true);
                                var players = ff[i].name.Split(',');
                                foreach (var player in players)
                                {
                                    if (!string.IsNullOrEmpty(player))
                                    {
                                        string account = CPlayerCtrl.GetAccByName(player);
                                        //发放虚宝
                                        bool vtret = CSGHelper.InsertSanvtItem(m_SanVtSql, account, (uint)m_AnswerVtId, 1, 0, 0, 0, 0, 0, 0, 0, 0);
                                        if (vtret)
                                        {
                                            string answerEx = "角色：" + player + " 答题奖励已经发放，请注意查收(虚宝)！";
                                            //日志
                                            LogHelper.WriteLog(DateTime.Now.ToString("yyyy-MM-dd") + "答题日志.txt", answerEx);
                                            
                                        }
                                    }
                                }
                            }
                            //公告
                            SendWorldWords("排名奖励已经发放，请注意查收(虚宝)！");

                            answerResult.Clear();
                        }

                    }
                    else
                    {
                        if (m_SleepCount >= m_AskNormalInterval)//60 * 
                        {
                            m_SleepCount = 0;
                            SingleAQHandle(0, m_PlayerDat, m_SanVtSql);
                        }
                        Thread.Sleep(1000);
                        m_SleepCount++;
                    }
                }
                else
                {
                    m_QAARE.WaitOne();
                }
            }
        }
        public int AnswerResultCompare(AnswerResult_S x, AnswerResult_S y)
        {
            int value = y.score.CompareTo(x.score);

            return value;
        }
        public bool StartQAThread()
        {
            if (m_ThreadQA == null)
            {
                m_ThreadQA = new Thread(new ParameterizedThreadStart(Game_QA_Thread));
                m_ThreadQA.Start(this);
                m_QAStop = false;
            }
            else
            {
                m_QAARE.Set();
                m_QAStop = false;
            }
            return true;
        }
        public bool StopQAThread()
        {
            m_QAStop = true;
            return true;
        }
        public bool AbortQAThread()
        {
            if (m_ThreadQA != null)
            {
                m_ThreadQA.Abort();
            }
            return true;
        }
        //普通公共

        [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Auto)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "FindWindowEx", CharSet = CharSet.Auto)]
        extern static IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        #endregion

        public static void WriteXbLog(string log)
        {
            FileStream fs = new FileStream("虚宝记录.xls", FileMode.Append, FileAccess.Write);
            StreamWriter rw = new StreamWriter(fs, Encoding.Default);//建立StreamWriter为写作准备;

            rw.WriteLine(log);

            rw.Close();
            fs.Close();
        }
        public static string GetXbLogFileName()
        {
            return "虚宝记录.xls";
        }
    }
}
