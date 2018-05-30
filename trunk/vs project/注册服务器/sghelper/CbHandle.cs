using MainServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace 注册网关
{
    public struct ZcTimeDate
    {
        public int date;
        public List<string> starts;
    };
    public struct ZcTime
    {
        public List<ZcTimeDate> dates;
        public int min_period;
        public int min_player;
    };
    class CbHandle
    {
        private string serverIni = System.AppDomain.CurrentDomain.BaseDirectory + "配置文件.ini";

        public delegate void WarFinishCallFunc();
        public WarFinishCallFunc m_WarFinishCallFunc = null;
        public delegate void WarStatusCallFunc(int flag);
        public WarStatusCallFunc m_WarStatusCallFunc = null;

        private ZcTime m_WarTime;
        private List<HeroScore> m_HeroScores = new List<HeroScore>();
        private List<WarRewordConf> m_WarRewordConf = new List<WarRewordConf>();

        private Thread m_Thread = null;
        private bool m_Stop = false;
        private AutoResetEvent m_ARE = new AutoResetEvent(false);
        private bool m_ResultLastChange = false;

        private string m_BaseForlder = "";
        private bool m_GoodLuck = false;
        private bool m_LessScore = false;
        private bool m_LessKill = false;
        private int m_LessScoreValue = 0;
        private int m_LessKillValue = 0;
        private bool m_AutoSend = false;
        WatcherTimer m_watcher = null;
        public bool AutoSend
        {
            get
            {
                return m_AutoSend;
            }
            set
            {
                m_AutoSend = value;
            }
        }
        public string BaseForlder
        {
            get
            {
                return m_BaseForlder;
            }
            set
            {
                m_BaseForlder = value;
            }
        }
        public bool EnableGoodLuck
        {
            get
            {
                string val = CIniCtrl.ReadIniData("CbSetting", "enableGoodLuck", "", serverIni);
                if (val != "")
                {
                    m_GoodLuck = bool.Parse(val);
                }
                return m_GoodLuck;
            }
            set
            {
                m_GoodLuck = value;
                CIniCtrl.WriteIniData("CbSetting", "enableGoodLuck", m_GoodLuck.ToString(), serverIni);
            }
        }
        public bool EnableLessScore
        {
            get
            {
                string val = CIniCtrl.ReadIniData("CbSetting", "enableLessScore", "", serverIni);
                if (val != "")
                {
                    m_LessScore = bool.Parse(val);
                }
                return m_LessScore;
            }
            set
            {
                m_LessScore = value;
                CIniCtrl.WriteIniData("CbSetting", "enableLessScore", m_LessScore.ToString(), serverIni);
            }
        }
        public bool EnableLessKill
        {
            get
            {
                string val = CIniCtrl.ReadIniData("CbSetting", "enableLessKill", "", serverIni);
                if (val != "")
                {
                    m_LessKill = bool.Parse(val);
                }
                return m_LessKill;
            }
            set
            {
                m_LessKill = value;
                CIniCtrl.WriteIniData("CbSetting", "enableLessKill", m_LessKill.ToString(), serverIni);
            }
        }
        public int LessScoreValue
        {
            get
            {
                string val = CIniCtrl.ReadIniData("CbSetting", "LessScoreValue", "", serverIni);
                if (val != "")
                {
                    m_LessScoreValue = int.Parse(val);
                }
                return m_LessScoreValue;
            }
            set
            {
                m_LessScoreValue = value;
                CIniCtrl.WriteIniData("CbSetting", "LessScoreValue", m_LessScoreValue.ToString(), serverIni);
            }
        }
        public int LessKillValue
        {
            get
            {
                string val = CIniCtrl.ReadIniData("CbSetting", "LessKillValue", "", serverIni);
                if (val != "")
                {
                    m_LessKillValue = int.Parse(val);
                }
                return m_LessKillValue;
            }
            set
            {
                m_LessKillValue = value;
                CIniCtrl.WriteIniData("CbSetting", "LessKillValue", m_LessKillValue.ToString(), serverIni);
            }
        }

        public CbHandle()
        {
            m_WarTime.dates = new List<ZcTimeDate>();
            m_watcher = new WatcherTimer(OnProcess, 2000);
        }

        public void InitWarTimeConf()
        {
            m_WarTime.dates.Clear();

            //读取 //Login/LoginHistory.ini
            string LoginHistoryFile = m_BaseForlder + "\\Login\\LoginHistory.ini";
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
                    m_WarTime.min_period = int.Parse(history_period);
                }
                if (strLine.Contains("min_team_player") && history_type == "1")
                {
                    history_min_team_player = strLine.Split('=')[1].Split(';')[0].Replace("\t", "").Replace(" ", "");
                    m_WarTime.min_player = int.Parse(history_min_team_player);

                }
                if (strLine.Contains("time") && history_session == "history" && history_type == "1")
                {
                    string history_time = strLine.Split('=')[1].Split(';')[0].Replace("\t", "").Replace(" ", "");
                    string history_weekday = history_time.Replace(" ", "").Split(',')[0];
                    if (history_weekday == "1")
                    {
                        ZcTimeDate timeDate;
                        timeDate.date = 1;
                        timeDate.starts = new List<string>();
                        timeDate.starts.Add(history_time.Substring(2).Replace(",", ":"));
                        m_WarTime.dates.Add(timeDate);
                    }
                    else if (history_weekday == "2")
                    {
                        ZcTimeDate timeDate;
                        timeDate.date = 2;
                        timeDate.starts = new List<string>();
                        //timeDate.starts.Add(history_time.Substring(2).Replace(",", ":"));
                        string tmp = history_time.Substring(2);
                        var _tmp = tmp.Split(',');
                        for (int i = 0; i < _tmp.Length - _tmp.Length % 2; i+=2 )
                        {
                            string _start = _tmp[i] + ":" + _tmp[i + 1];
                            timeDate.starts.Add(_start);
                        }
                        m_WarTime.dates.Add(timeDate);
                    }
                    else if (history_weekday == "3")
                    {
                        ZcTimeDate timeDate;
                        timeDate.date = 3;
                        timeDate.starts = new List<string>();
                        //timeDate.starts.Add(history_time.Substring(2).Replace(",", ":"));
                        string tmp = history_time.Substring(2);
                        var _tmp = tmp.Split(',');
                        for (int i = 0; i < _tmp.Length - _tmp.Length % 2; i += 2)
                        {
                            string _start = _tmp[i] + ":" + _tmp[i + 1];
                            timeDate.starts.Add(_start);
                        }
                        m_WarTime.dates.Add(timeDate);
                    }
                    else if (history_weekday == "4")
                    {
                        ZcTimeDate timeDate;
                        timeDate.date = 4;
                        timeDate.starts = new List<string>();
                        //timeDate.starts.Add(history_time.Substring(2).Replace(",", ":"));
                        string tmp = history_time.Substring(2);
                        var _tmp = tmp.Split(',');
                        for (int i = 0; i < _tmp.Length - _tmp.Length % 2; i += 2)
                        {
                            string _start = _tmp[i] + ":" + _tmp[i + 1];
                            timeDate.starts.Add(_start);
                        }
                        m_WarTime.dates.Add(timeDate);
                    }
                    else if (history_weekday == "5")
                    {
                        ZcTimeDate timeDate;
                        timeDate.date = 5;
                        timeDate.starts = new List<string>();
                        //timeDate.starts.Add(history_time.Substring(2).Replace(",", ":"));
                        string tmp = history_time.Substring(2);
                        var _tmp = tmp.Split(',');
                        for (int i = 0; i < _tmp.Length - _tmp.Length % 2; i += 2)
                        {
                            string _start = _tmp[i] + ":" + _tmp[i + 1];
                            timeDate.starts.Add(_start);
                        }
                        m_WarTime.dates.Add(timeDate);
                    }
                    else if (history_weekday == "6")
                    {
                        ZcTimeDate timeDate;
                        timeDate.date = 6;
                        timeDate.starts = new List<string>();
                        //timeDate.starts.Add(history_time.Substring(2).Replace(",", ":"));
                        string tmp = history_time.Substring(2);
                        var _tmp = tmp.Split(',');
                        for (int i = 0; i < _tmp.Length - _tmp.Length % 2; i += 2)
                        {
                            string _start = _tmp[i] + ":" + _tmp[i + 1];
                            timeDate.starts.Add(_start);
                        }
                        m_WarTime.dates.Add(timeDate);
                    }
                    else if (history_weekday == "7")
                    {
                        ZcTimeDate timeDate;
                        timeDate.date = 7;
                        timeDate.starts = new List<string>();
                        //timeDate.starts.Add(history_time.Substring(2).Replace(",", ":"));
                        string tmp = history_time.Substring(2);
                        var _tmp = tmp.Split(',');
                        for (int i = 0; i < _tmp.Length - _tmp.Length % 2; i += 2)
                        {
                            string _start = _tmp[i] + ":" + _tmp[i + 1];
                            timeDate.starts.Add(_start);
                        }
                        m_WarTime.dates.Add(timeDate);
                    }
                }
                strLine = "";
                strLine = rd.ReadLine();
            } while (strLine != null);
            rd.Close();
            rdfs.Close();
        }

        public void InitRewordConf()
        {
            m_WarRewordConf.Clear();
            FileStream fs = new FileStream(".\\Profile\\赤壁英雄奖励配置.xls", FileMode.OpenOrCreate, FileAccess.Read);
            StreamReader rd = new StreamReader(fs, Encoding.Default);

            string strLine = "";
            strLine = rd.ReadLine();
            while (strLine != null)
            {
                var details = strLine.Split('\t');
                if (details.Length >= 18)
                {
                    WarRewordConf _WarRewordConf = new WarRewordConf();
                    _WarRewordConf.flag = uint.Parse(details[0]);
                    _WarRewordConf.start = uint.Parse(details[1]);
                    _WarRewordConf.end = uint.Parse(details[2]);
                    _WarRewordConf.id1 = uint.Parse(details[3] == "" ? "0" : details[3]);
                    _WarRewordConf.name1 = details[4].ToString();
                    _WarRewordConf.count1 = uint.Parse(details[5] == "" ? "0" : details[5]);
                    _WarRewordConf.id2 = uint.Parse(details[6] == "" ? "0" : details[6]);
                    _WarRewordConf.name2 = details[7].ToString();
                    _WarRewordConf.count2 = uint.Parse(details[8] == "" ? "0" : details[8]);
                    _WarRewordConf.id3 = uint.Parse(details[9] == "" ? "0" : details[9]);
                    _WarRewordConf.name3 = details[10].ToString();
                    _WarRewordConf.count3 = uint.Parse(details[11] == "" ? "0" : details[11]);
                    _WarRewordConf.id4 = uint.Parse(details[12] == "" ? "0" : details[12]);
                    _WarRewordConf.name4 = details[13].ToString();
                    _WarRewordConf.count4 = uint.Parse(details[14] == "" ? "0" : details[14]);
                    _WarRewordConf.id5 = uint.Parse(details[15] == "" ? "0" : details[15]);
                    _WarRewordConf.name5 = details[16].ToString();
                    _WarRewordConf.count5 = uint.Parse(details[17] == "" ? "0" : details[17]);

                    m_WarRewordConf.Add(_WarRewordConf);
                }
                strLine = "";
                strLine = rd.ReadLine();
            }

            rd.Close();
            fs.Close();
        }
        private void IntiOtherConf()
        {
            string val = "";
            val = CIniCtrl.ReadIniData("CbSetting", "enableLessScore", "", serverIni);
            if (val != "")
            {
                m_LessScore = bool.Parse(val);
            }
            val = CIniCtrl.ReadIniData("CbSetting", "LessScoreValue", "", serverIni);
            if (val != "")
            {
                m_LessScoreValue = int.Parse(val);
            }
            val = CIniCtrl.ReadIniData("CbSetting", "enableLessKill", "", serverIni);
            if (val != "")
            {
                m_LessKill = bool.Parse(val);
            }
            val = CIniCtrl.ReadIniData("CbSetting", "LessKillValue", "", serverIni);
            if (val != "")
            {
                m_LessKillValue = int.Parse(val);
            }
        }

         public void Init()
        {
            //赤壁时间
            InitWarTimeConf();
            //赤壁奖励加载
            InitRewordConf();
            //其他
            IntiOtherConf();
        }

        public List<WarRewordConf> GetRewordConf()
        {
            return m_WarRewordConf;
        }

        public List<HeroScore> GetHeroScore(bool sort)
        {
            m_HeroScores.Clear();
            //读取文件重建m_HeroScores
            FileStream cwar_last_fs = new FileStream(m_BaseForlder + "\\Map\\history\\history_0351.txt", FileMode.Open, FileAccess.Read);
            StreamReader cwar_last_rd = new StreamReader(cwar_last_fs, Encoding.GetEncoding(950));

            string strLine = "";
            strLine = cwar_last_rd.ReadLine();
            int lastMode = -1;
            do
            {
                strLine = strLine.Split('/')[0];
                if (strLine.Contains("last_result"))
                {
                    lastMode = 1;
                }
                else if (strLine.Contains("register_list"))
                {
                    lastMode = 2;
                    m_HeroScores.Clear();
                }
                if (strLine.Contains("item") && lastMode == 1)
                {
                    //ToSimplified
                    HeroScore heroScore = new HeroScore();
                    strLine = CFormat.ToSimplified(strLine);
                    strLine = strLine.Split(new string[] { " = " }, StringSplitOptions.RemoveEmptyEntries)[1];
                    var heres = strLine.Split(',');
                    if (heres[0] != "")
                    {
                        heroScore.name = heres[0];
                        heroScore.level = 0;
                        heroScore.unkown1 = "";
                        heroScore.unkown2 = "";
                        heroScore.score = int.Parse(heres[3]);
                        heroScore.num = int.Parse(heres[5]);
                        heroScore.unkwon3 = "";
                        m_HeroScores.Add(heroScore);
                    }
                }
                strLine = "";
                strLine = cwar_last_rd.ReadLine();

            } while (strLine != null);

            if (m_HeroScores.Count == 0)
            {
                SGExHandle SGEx = new SGExHandle();
                SGEx.SendWorldWords("赤壁未分胜负，以讨敌数排名，自动发奖！");

                //查询数据库

            }

            if (sort)
            {
                m_HeroScores.Sort(new Comparison<HeroScore>(AnswerResultCompare));
            }


            cwar_last_rd.Close();
            cwar_last_fs.Close();

            return m_HeroScores;
        }

        public int AnswerResultCompare(HeroScore x, HeroScore y)
        {
            int value = y.score.CompareTo(x.score);

            return value;
        }

        public void SetWarTime()
        {
            InitWarTimeConf();
        }

        public bool IsUnderWar()
        {
            int date = Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"));

            foreach (var dataT in m_WarTime.dates)
            {
                if (dataT.date == date)
                {
                    foreach (var start in dataT.starts)
                    {
                        //时间比较
                        DateTime dtStart = Convert.ToDateTime(start);
                        if (m_WarTime.min_period / 60 >= 24)
                        {
                            return true;
                        }
                        DateTime dtEnd = dtStart.AddMinutes(m_WarTime.min_period);

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
        public bool IsWarFinish()
        {
            if (!m_ResultLastChange)
            {
                return false;
            }

            m_ResultLastChange = false;

            return true;
        }
        public void SendWarReward(string name, WarRewordConf conf)
        {
            uint xbCount1 = conf.count1;
            uint xbCount2 = conf.count2;
            uint xbCount3 = conf.count3;
            uint xbCount4 = conf.count4;
            uint xbCount5 = conf.count5;
            uint xbId1 = conf.id1;
            uint xbId2 = conf.id2;
            uint xbId3 = conf.id3;
            uint xbId4 = conf.id4;
            uint xbId5 = conf.id5;
            if (xbId1 == 0)
            {
                xbId1 = 1;
                xbCount1 = 0;
            }
            if (xbId2 == 0)
            {
                xbId2 = 1;
                xbCount2 = 0;
            }
            if (xbId3 == 0)
            {
                xbId3 = 1;
                xbCount3 = 0;
            }
            if (xbId4 == 0)
            {
                xbId4 = 1;
                xbCount4 = 0;
            }
            if (xbId5 == 0)
            {
                xbId5 = 1;
                xbCount5 = 0;
            }

            CSGHelper.InsertSanvtItem(CPlayerCtrl.GetAccByName(name)
                        , (uint)xbId1, (uint)xbCount1, (uint)xbId2, (uint)xbCount2, (uint)xbId3, (uint)xbCount3
                        , (uint)xbId4, (uint)xbCount4, (uint)xbId5, (uint)xbCount5);
        }
        public void SendWarReward()
        {
            //遍历排名
            for (int i = 0; i < m_HeroScores.Count; i++)
            {
                int ranking = i + 1;
                //遍历Conf
                for (int j = 0; j < m_WarRewordConf.Count; j++)
                {
                    //最小功勋
                    if (m_LessScore && m_HeroScores[i].score <= m_LessScoreValue)
                    {
                        continue;
                    }
                    //最小讨敌
                    if (m_LessKill && m_HeroScores[i].num <= m_LessKillValue)
                    {
                        continue;
                    }
                    //是否排名内
                    if (m_WarRewordConf[j].start <= ranking && m_WarRewordConf[j].end >= ranking && m_WarRewordConf[j].flag == 0)
                    {
                        SendWarReward(m_HeroScores[i].name, m_WarRewordConf[j]);
                    }
                    //是否尾数
                    if (m_GoodLuck && m_WarRewordConf[j].flag == 0 && ranking % 10 == m_WarRewordConf[j].start && m_WarRewordConf[j].end >= ranking)
                    {
                        SendWarReward(m_HeroScores[i].name, m_WarRewordConf[i]);
                    }
                }
            }
            SGExHandle SGEx = new SGExHandle();
            if (m_HeroScores.Count == 0)
            {
                SGEx.SendWorldWords("赤壁未分胜负，以讨敌数排名，自动发奖！");
            }
            else
                SGEx.SendWorldWords("赤壁个人奖励已经发放，请到大鸿儒处领取虚宝奖励！");
        }
        private void OnProcess(object source, FileSystemEventArgs e)
        {
            //if (e.ChangeType == WatcherChangeTypes.Created)
            //{
            //    m_ResultLastChange = true;
            //}
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                m_ResultLastChange = true;
            }
        }
        private void WarLastStrat(string path, string filter)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.Filter = filter;
            watcher.Changed += new FileSystemEventHandler(m_watcher.OnFileChanged);
            watcher.Created += new FileSystemEventHandler(m_watcher.OnFileChanged);
            //watcher.Deleted += new FileSystemEventHandler(OnProcess);
            //watcher.Renamed += new RenamedEventHandler(OnRenamed);
            watcher.EnableRaisingEvents = true;
            //watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess
            //                       | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;
            watcher.NotifyFilter = NotifyFilters.LastWrite;// | NotifyFilters.Size;
            //watcher.IncludeSubdirectories = true;
        }

        public bool SetWarFinishCallFunc(WarFinishCallFunc func)
        {
            m_WarFinishCallFunc = func;
            return true;
        }

        public bool SetWarStatusCallFunc(WarStatusCallFunc func)
        {
            m_WarStatusCallFunc = func;
            return true;
        }

        private void WorkThread(object obj)
        {
            //\\Map\\history\\history_0351.txt
            WarLastStrat(m_BaseForlder + "\\Map\\history\\", "history_0351.txt");

            while (true)
            {
                if (!m_Stop)
                {
                    //赤壁结束
                    if (IsWarFinish())
                    {
                        GetHeroScore(true);
                        //发送奖励
                        if (AutoSend)
                        {
                            SendWarReward();
                        }

                        //回调结束界面操作
                        if (m_WarFinishCallFunc != null)
                        {
                            m_WarFinishCallFunc();
                        }
                    }
                    if (IsUnderWar())
                    {
                        //回调状态界面操作
                        if (m_WarStatusCallFunc != null)
                        {
                            m_WarStatusCallFunc(1);
                        }
                    }
                    else
                    {
                        if (m_WarStatusCallFunc != null)
                        {
                            m_WarStatusCallFunc(0);
                        }
                    }

                    Thread.Sleep(5 * 1000);
                }
                else
                {
                    m_ARE.WaitOne();
                }
            }
        }

        public bool StartThread()
        {
            if (m_Thread == null)
            {
                m_Thread = new Thread(new ParameterizedThreadStart(WorkThread));
                m_Thread.Start(this);
                m_Stop = false;
            }
            else
            {
                m_ARE.Set();
                m_Stop = false;
            }
            return true;
        }
        public bool StopThread()
        {
            m_Stop = true;
            return true;
        }
        public bool AbortThread()
        {
            if (m_Thread != null)
            {
                m_Thread.Abort();
            }
            return true;
        }
    }
}
