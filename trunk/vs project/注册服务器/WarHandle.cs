using register_server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace 注册网关
{
    public struct HeroScore
    {
        public string name;
        public int level;
        public string unkown1;
        public string unkown2;
        public int score;
        public int num;
        public string unkwon3;
    };
    public struct WarTime
    {
        public List<int> dates;
        public string start;
        public int min_period;
    };
    public struct WarRewordConf
    {
        public uint flag; //是否是尾数奖励 若是 start:尾数 end:最大数
        public uint start;
        public uint end;
        public uint id1;
        public string name1;
        public uint count1;
        public uint id2;
        public string name2;
        public uint count2;
        public uint id3;
        public string name3;
        public uint count3;
        public uint id4;
        public string name4;
        public uint count4;
        public uint id5;
        public string name5;
        public uint count5;
    };
    public struct WarOrgRewordConf
    {
        public string stage;
        public string type;
        public uint id1;
        public string name1;
        public uint count1;
        public uint id2;
        public string name2;
        public uint count2;
        public uint id3;
        public string name3;
        public uint count3;
        public uint id4;
        public string name4;
        public uint count4;
        public uint id5;
        public string name5;
        public uint count5;
        public uint flag;
    };
    public struct OrganizeRst
    {
        public int stage;
        public string legion;
        public string leader;
    };

    public struct OrganizeConstRst
    {
        public int stage;
        public string legion;
        public string leader;
        public string bTime;
        public string eTime;
    };

    class WarHandle
    {
        private string serverIni = System.AppDomain.CurrentDomain.BaseDirectory + "配置文件.ini";
        public delegate void WarFinishCallFunc();
        public WarFinishCallFunc m_WarFinishCallFunc = null;
        public delegate void WarStatusCallFunc(int flag);
        public WarStatusCallFunc m_WarStatusCallFunc = null;

        private WarTime m_WarTime;
        private List<HeroScore> m_HeroScores = new List<HeroScore>();
        private List<WarRewordConf> m_WarRewordConf = new List<WarRewordConf>();
        private List<WarOrgRewordConf> m_WarOrgRewordConf = new List<WarOrgRewordConf>();
        private Thread m_Thread = null;
        private bool m_Stop = false;
        private AutoResetEvent m_ARE = new AutoResetEvent(false);
        private bool m_WarLastChange = false;

        private string m_BaseForlder = "";
        private bool m_GoodLuck = false;
        private bool m_LessScore = false;
        private bool m_LessKill = false;
        private int m_LessScoreValue = 0;
        private int m_LessKillValue = 0;
        private bool m_AutoSend = false;
        private bool m_OrgConstAutoSend = false;
        private bool m_OrgAutoSend = false;
        WatcherTimer m_watcher = null;
        WatcherTimer m_orgwatcher = null;

        private List<OrganizeRst> m_NoticeOrganizeRst = new List<OrganizeRst>();
        private List<OrganizeConstRst> m_NoticeConstOrganizeRst = new List<OrganizeConstRst>();
        private List<OrganizeConstRst> m_NoticeTmpConstOrganizeRst = new List<OrganizeConstRst>();
        private List<int> m_NoticeStage = new List<int>() {};
        private List<int> m_NoticeConstStage = new List<int>() {};

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

        public bool OrgConstAutoSend
        {
            get
            {
                return m_OrgConstAutoSend;
            }
            set
            {
                m_OrgConstAutoSend = value;
            }
        }

        public bool OrgAutoSend
        {
            get
            {
                return m_OrgAutoSend;
            }
            set
            {
                m_OrgAutoSend = value;
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
                string val = CIniCtrl.ReadIniData("WarSetting", "enableGoodLuck", "", serverIni);
                if (val != "")
                {
                    m_GoodLuck = bool.Parse(val);
                }
                return m_GoodLuck;
            }
            set
            {
                m_GoodLuck = value;
                CIniCtrl.WriteIniData("WarSetting", "enableGoodLuck", m_GoodLuck.ToString(), serverIni);
            }
        }
        public bool EnableLessScore
        {
            get
            {
                string val = CIniCtrl.ReadIniData("WarSetting", "enableLessScore", "", serverIni);
                if (val != "")
                {
                    m_LessScore = bool.Parse(val);
                }
                return m_LessScore;
            }
            set
            {
                m_LessScore = value;
                CIniCtrl.WriteIniData("WarSetting", "enableLessScore", m_LessScore.ToString(), serverIni);
            }
        }
        public bool EnableLessKill
        {
            get
            {
                string val = CIniCtrl.ReadIniData("WarSetting", "enableLessKill", "", serverIni);
                if (val != "")
                {
                    m_LessKill = bool.Parse(val);
                }
                return m_LessKill;
            }
            set
            {
                m_LessKill = value;
                CIniCtrl.WriteIniData("WarSetting", "enableLessKill", m_LessKill.ToString(), serverIni);
            }
        }
        public int LessScoreValue
        {
            get
            {
                string val = CIniCtrl.ReadIniData("WarSetting", "LessScoreValue", "", serverIni);
                if (val != "")
                {
                    m_LessScoreValue = int.Parse(val);
                }
                return m_LessScoreValue;
            }
            set
            {
                m_LessScoreValue = value;
                CIniCtrl.WriteIniData("WarSetting", "LessScoreValue", m_LessScoreValue.ToString(), serverIni);
            }
        }
        public int LessKillValue
        {
            get
            {
                string val = CIniCtrl.ReadIniData("WarSetting", "LessKillValue", "", serverIni);
                if (val != "")
                {
                    m_LessKillValue = int.Parse(val);
                }
                return m_LessKillValue;
            }
            set
            {
                m_LessKillValue = value;
                CIniCtrl.WriteIniData("WarSetting", "LessKillValue", m_LessKillValue.ToString(), serverIni);
            }
        }
        public WarHandle()
        {
            //Init();
            m_watcher = new WatcherTimer(OnProcess, 2000);
            m_orgwatcher = new WatcherTimer(OrgOnProcess, 1000);

        }
        public void InitWarTimeConf()
        {
            m_WarTime.dates = new List<int>();
            m_WarTime.start = "00:00";
            m_WarTime.min_period = 0;
            //读取国战设置
            string LoginServerFile = m_BaseForlder + "\\Login\\LoginServer.ini";
            if (!File.Exists(LoginServerFile))
            {
                return;
            }
            string tmp = "";

            tmp = CIniCtrl.ReadIniData("System", "country_war_date", "", LoginServerFile);
            tmp = tmp.Split(';')[0].Replace("\t", "").Replace(" ", "");
            if (tmp != "")
            {

                var dates = tmp.Split(',');
                foreach(var date in dates)
                {
                    m_WarTime.dates.Add(int.Parse(date));
                }
            }
           

            tmp = CIniCtrl.ReadIniData("System", "country_war_time", "", LoginServerFile);
            tmp = tmp.Split(';')[0].Replace("\t", "").Replace(" ", "");
            if (tmp != "")
            {
                tmp = tmp.Replace(',',':');
                m_WarTime.start = tmp;
            }

            tmp = CIniCtrl.ReadIniData("System", "country_war_period", "", LoginServerFile);
            tmp = tmp.Split(';')[0].Replace("\t", "").Replace(" ", "");
            if (tmp != "")
            {
                m_WarTime.min_period = int.Parse(tmp);
            }

            //防守奖励设置
        }

        public List<WarRewordConf> GetRewordConf()
        {
            return m_WarRewordConf;
        }
        public void InitRewordConf()
        {
            m_WarRewordConf.Clear();
            FileStream fs = new FileStream(".\\Profile\\国战英雄奖励配置.xls", FileMode.OpenOrCreate, FileAccess.Read);
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
                    _WarRewordConf.id1 = uint.Parse(details[3]==""?"0":details[3]);
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

            //////////////////////////////////////////////////////////////////////////////////
            //   防守奖励 m_WarOrgRewordConf
            StageCtrl.LoadStageDefInfo();
            m_WarOrgRewordConf.Clear();
            FileStream _fs = new FileStream(".\\Profile\\国战防守奖励配置.xls", FileMode.OpenOrCreate, FileAccess.Read);
            StreamReader _rd = new StreamReader(_fs, Encoding.Default);

            string _strLine = "";
            _strLine = _rd.ReadLine();
            while (_strLine != null)
            {
                var details = _strLine.Split('\t');
                if (details.Length >= 18)
                {
                    WarOrgRewordConf _WarOrgRewordConf = new WarOrgRewordConf();
                    _WarOrgRewordConf.stage = details[0];
                    _WarOrgRewordConf.type = details[1];
                    _WarOrgRewordConf.id1 = uint.Parse(details[2] == "" ? "0" : details[2]);
                    _WarOrgRewordConf.name1 = details[3].ToString();
                    _WarOrgRewordConf.count1 = uint.Parse(details[4] == "" ? "0" : details[4]);
                    _WarOrgRewordConf.id2 = uint.Parse(details[5] == "" ? "0" : details[5]);
                    _WarOrgRewordConf.name2 = details[6].ToString();
                    _WarOrgRewordConf.count2 = uint.Parse(details[7] == "" ? "0" : details[7]);
                    _WarOrgRewordConf.id3 = uint.Parse(details[8] == "" ? "0" : details[8]);
                    _WarOrgRewordConf.name3 = details[9].ToString();
                    _WarOrgRewordConf.count3 = uint.Parse(details[10] == "" ? "0" : details[10]);
                    _WarOrgRewordConf.id4 = uint.Parse(details[11] == "" ? "0" : details[11]);
                    _WarOrgRewordConf.name4 = details[12].ToString();
                    _WarOrgRewordConf.count4 = uint.Parse(details[13] == "" ? "0" : details[13]);
                    _WarOrgRewordConf.id5 = uint.Parse(details[14] == "" ? "0" : details[14]);
                    _WarOrgRewordConf.name5 = details[15].ToString();
                    _WarOrgRewordConf.count5 = uint.Parse(details[16] == "" ? "0" : details[16]);
                    _WarOrgRewordConf.flag = uint.Parse(details[17]);

                    m_WarOrgRewordConf.Add(_WarOrgRewordConf);

                    if (_WarOrgRewordConf.type == "防守奖励")
                    {
                        m_NoticeConstStage.Add(int.Parse(StageCtrl.GetIdByName(_WarOrgRewordConf.stage)));
                    }
                    else if (_WarOrgRewordConf.type == "占城奖励")
                    {
                        m_NoticeStage.Add(int.Parse(StageCtrl.GetIdByName(_WarOrgRewordConf.stage)));
                    }
                }
                _strLine = "";
                _strLine = _rd.ReadLine();
            }

            _rd.Close();
            _fs.Close();
        }
        private void IntiOtherConf()
        {
            string val = "";
            val = CIniCtrl.ReadIniData("WarSetting", "enableLessScore", "", serverIni);
            if (val != "")
            {
                m_LessScore = bool.Parse(val);
            }
            val = CIniCtrl.ReadIniData("WarSetting", "LessScoreValue", "", serverIni);
            if (val != "")
            {
                m_LessScoreValue = int.Parse(val);
            }
            val = CIniCtrl.ReadIniData("WarSetting", "enableLessKill", "", serverIni);
            if (val != "")
            {
                m_LessKill = bool.Parse(val);
            }
            val = CIniCtrl.ReadIniData("WarSetting", "LessKillValue", "", serverIni);
            if(val != "")
            {
                m_LessKillValue = int.Parse(val);
            }
            
        }
        public void Init()
        { 
            //国战时间
            InitWarTimeConf();
            //国战奖励加载
            InitRewordConf();
            //其他
            IntiOtherConf();
        }
        public List<HeroScore> GetHeroScore(bool sort)
        {
            m_HeroScores.Clear();
            //读取文件重建m_HeroScores
            FileStream cwar_last_fs = new FileStream(m_BaseForlder + "\\Login\\cwar\\cwar_last.txt", FileMode.Open, FileAccess.Read);
            StreamReader cwar_last_rd = new StreamReader(cwar_last_fs, Encoding.GetEncoding(950));

            string strLine = "";
            strLine = cwar_last_rd.ReadLine();

            do
            {
                strLine = strLine.Split('/')[0];
                if (strLine.Contains("item"))
                {
                    //ToSimplified
                    HeroScore heroScore = new HeroScore();
                    strLine = CFormat.ToSimplified(strLine);
                    strLine = strLine.Split(new string[] { " = " }, StringSplitOptions.RemoveEmptyEntries)[1];
                    var heres = strLine.Split(',');
                    if (heres[0] != "")
                    {
                        heroScore.name = heres[0];
                        heroScore.level = int.Parse(heres[1]);
                        heroScore.unkown1 = heres[2];
                        heroScore.unkown2 = heres[3];
                        heroScore.score = int.Parse(heres[4]);
                        heroScore.num = int.Parse(heres[5]);
                        heroScore.unkwon3 = heres[6];
                        m_HeroScores.Add(heroScore);
                    }
                }

                strLine = "";
                strLine = cwar_last_rd.ReadLine();

            } while (strLine != null);

            if(sort)
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

        public void SetWarTime(List<int> dates, string start, int min_period)
        {
            if (!start.Contains(":") && !start.Contains(","))
            {
                return;
            }
            m_WarTime.dates = dates;
            m_WarTime.start = start.Replace(',',';');
            m_WarTime.min_period = min_period;

            //写配置
            string _dates = "";
            for (int i = 0; i < m_WarTime.dates.Count; i++ )
            {
                _dates += m_WarTime.dates[i].ToString();
                if (i < m_WarTime.dates.Count - 1)
                {
                    _dates += ",";
                }
            }

            string LoginServerFile = m_BaseForlder + "\\Login\\LoginServer.ini";
            if (!File.Exists(LoginServerFile))
            {
                return;
            }
            string tmp = "";
            tmp = _dates;
            CIniCtrl.WriteIniData("System", "country_war_date", tmp, LoginServerFile);
            tmp = m_WarTime.start;
            CIniCtrl.WriteIniData("System", "country_war_time", tmp, LoginServerFile);
            tmp = m_WarTime.min_period.ToString();
            CIniCtrl.WriteIniData("System", "country_war_period", tmp, LoginServerFile);
        }
        public bool IsUnderWar()
        {
            int date = Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"));
            if (m_WarTime.dates.Contains(date))
            {
                //时间比较
                DateTime dtStart = Convert.ToDateTime(m_WarTime.start);
                if (m_WarTime.min_period /60 >= 24)
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
            return false;
        }

        public bool IsWarFinish()
        {
            if (!m_WarLastChange)
            {
                return false;
            }

            m_WarLastChange = false;

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
            string cmd = "INSERT INTO " + "sanvt_hcsg" + @".dbo.vitem (Account,Disable,Card,Login_time,Get_time,SName,CharName,Type,"
                       + "DataID1,Number1,DataID2,Number2,DataID3,Number3,DataID4,Number4,DataID5,Number5)"
                       + "values ('" + CPlayerCtrl.GetAccByName(name) + "',0,CONVERT(varchar(100), GETDATE(), 21),getdate(),getdate(),0,0,0,"
                       + xbId1 + "," + xbCount1 + "," + xbId2 + "," + xbCount2 + "," + xbId3 + "," + xbCount3 + "," + xbId4 + "," + xbCount4 + "," + xbId5 + "," + xbCount5 + ")";
            string ret = CSGHelper.SqlCommand(cmd);
        }
        public void SendWarReward()
        { 
            //遍历排名
            for (int i = 0; i < m_HeroScores.Count; i++ )
            {
                int ranking = i + 1;
                //遍历Conf
                for (int j = 0; j < m_WarRewordConf.Count; j++ )
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
                        SendWarReward(m_HeroScores[i].name,m_WarRewordConf[i]);
                    }
                }
            }
            SGExHandle SGEx = new SGExHandle();
            SGEx.SendWorldWords("国战个人奖励已经发放，请到大鸿儒处领取虚宝奖励！");
        }

        public void SendStageReward()
        {
            StageCtrl.LoadStageDefInfo();
            //m_NoticeOrganizeRst;
            foreach (var it in m_NoticeOrganizeRst)
            {
                //遍历m_WarOrgRewordConf
                foreach (var RewordConf in m_WarOrgRewordConf)
                {
                    if (RewordConf.stage == StageCtrl.GetNameById(it.stage.ToString()) && RewordConf.type == "占城奖励")
                    {
                        WarRewordConf conf = new WarRewordConf();
                        conf.id1 = RewordConf.id1;
                        conf.count1 = RewordConf.count1;
                        conf.id2 = RewordConf.id2;
                        conf.count2 = RewordConf.count2;
                        conf.id3 = RewordConf.id3;
                        conf.count3 = RewordConf.count3;
                        conf.id4 = RewordConf.id4;
                        conf.count4 = RewordConf.count4;
                        conf.id5 = RewordConf.id5;
                        conf.count5 = RewordConf.count5;

                        SendWarReward(it.leader, conf);
                    }
                }
                
            }
            SGExHandle SGEx = new SGExHandle();
            SGEx.SendWorldWords("国战占城奖励已经发放，请军团长到大鸿儒处领取虚宝奖励！");
        }
        public void SendStageConstReward()
        {
            SGExHandle SGEx = new SGExHandle();
            SGEx.SendWorldWords("国战防守奖励已经发放，请军团长到大鸿儒处领取虚宝奖励！");
        }
        private void OnProcess(object source, FileSystemEventArgs e)
        {
            //if (e.ChangeType == WatcherChangeTypes.Created)
            //{
            //    m_WarLastChange = true;
            //}
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                m_WarLastChange = true;
            }
        }
        private void WarLastStrat(string path, string filter)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.Filter = filter;
            watcher.Changed += new FileSystemEventHandler(m_watcher.OnFileChanged);
            //watcher.Changed += new FileSystemEventHandler(OnProcess);
            //watcher.Created += new FileSystemEventHandler(OnProcess);
            //watcher.Deleted += new FileSystemEventHandler(OnProcess);
            //watcher.Renamed += new RenamedEventHandler(OnRenamed);
            watcher.EnableRaisingEvents = true;
            //watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess
            //                       | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;
            watcher.NotifyFilter = NotifyFilters.LastWrite;// | NotifyFilters.Size;
            //watcher.IncludeSubdirectories = true;
        }

        private void OrgOnProcess(object source, FileSystemEventArgs e)
        { 
            //查询防守
            //m_NoticeConstOrganizeRst.Add();
        }
        private void OrganizeLastStrat(string path, string filter)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.Filter = filter;
            watcher.Changed += new FileSystemEventHandler(m_orgwatcher.OnFileChanged);
            watcher.EnableRaisingEvents = true;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
        }


        public List<OrganizeRst> GetNoticeStage()
        {
            //CPlayerCtrl.LoadPlayerInfos(txt_svrForder.Text + "\\DataBase\\saves\\players.dat", true);
            List<OrganizeAttr> organizeAttr = COrganizeCtrl.LoadOrganizeInfos(BaseForlder + "\\DataBase\\saves\\organize.dat", true);
            m_NoticeOrganizeRst.Clear();

            foreach (var it in organizeAttr)
            {
                int stageId = it.StageId;
                if (m_NoticeStage.Contains(stageId))
                {
                    OrganizeRst organizeRst = new OrganizeRst();

                    organizeRst.stage = stageId; //get stage name by stage id
                    organizeRst.legion = CFormat.GameStrToSimpleCN(it.OrganizeName);
                    organizeRst.leader = CFormat.GameStrToSimpleCN(it.OrganizeLeaderZh);

                    m_NoticeOrganizeRst.Add(organizeRst);
                }
            }

            return m_NoticeOrganizeRst;
        }

        private bool IsConstStageReward(DateTime bTime, DateTime eTime, Int32 mins)
        {
            DateTime dtEnd = bTime.AddMinutes(mins);

            if (DateTime.Compare(eTime, dtEnd) > 0)
            {
                return true;
            }
            return false;
        }

        private int IsStillConstStage(OrganizeConstRst organizeRst)
        {
            for (int i = 0; i < m_NoticeConstOrganizeRst.Count; i++ )
            {
                if (m_NoticeConstOrganizeRst[i].stage == organizeRst.stage && m_NoticeConstOrganizeRst[i].legion == organizeRst.legion)
                {
                    return i;
                }
            }

            return -1;
        }
        public List<OrganizeConstRst> GetNoticeConstStage(int flag)
        {
            if (flag == 0) //国战进行中，更新tmp,期间累计
            {
                List<OrganizeAttr> organizeAttr = COrganizeCtrl.LoadOrganizeInfos(BaseForlder + "\\DataBase\\saves\\organize.dat", true);
                foreach (var it in organizeAttr)
                {
                    int stageId = it.StageId;
                    if (m_NoticeConstStage.Contains(stageId)) // 关注的城池
                    {
                        OrganizeConstRst organizeRst = new OrganizeConstRst();

                        organizeRst.stage = stageId; //get stage name by stage id
                        organizeRst.legion = CFormat.GameStrToSimpleCN(it.OrganizeName);
                        organizeRst.leader = CFormat.GameStrToSimpleCN(it.OrganizeLeaderZh);

                        bool find = false;
                        for (int i = 0; i < m_NoticeTmpConstOrganizeRst.Count; i++)
                        {
                            if (m_NoticeTmpConstOrganizeRst[i].stage == organizeRst.stage)//如果已经找到该城池记录
                            {
                                find = true;

                                if (m_NoticeTmpConstOrganizeRst[i].legion == organizeRst.legion)//如果是同一个军团，则更新end time
                                {
                                    organizeRst = m_NoticeTmpConstOrganizeRst[i];
                                    organizeRst.eTime = DateTime.Now.ToString();
                                    m_NoticeTmpConstOrganizeRst[i] = organizeRst;

                                    //判断是否达到条件
                                    if (IsConstStageReward(Convert.ToDateTime(organizeRst.bTime), Convert.ToDateTime(organizeRst.eTime), 2)) //达到
                                    {
                                        /*
                                        int index = IsStillConstStage(organizeRst);
                                        if (index >= 0) //是否存在记录
                                        {
                                            //更新eTime
                                            OrganizeConstRst rst = m_NoticeConstOrganizeRst[index];
                                            rst.eTime = DateTime.Now.ToString();
                                            m_NoticeConstOrganizeRst[index] = rst;
                                        }
                                        else { //添加记录

                                            m_NoticeConstOrganizeRst.Add(m_NoticeTmpConstOrganizeRst[i]);
                                        }
                                        */
                                        m_NoticeConstOrganizeRst.Add(m_NoticeTmpConstOrganizeRst[i]);

                                        organizeRst.bTime = DateTime.Now.ToString();
                                        organizeRst.eTime = DateTime.Now.ToString();
                                        m_NoticeTmpConstOrganizeRst[i] = organizeRst;
                                    }
                                }
                                else //如果不是同一个军团，则先判断上一个军团是否达到奖励条件，然后替换
                                {
                                    if (IsConstStageReward(Convert.ToDateTime(m_NoticeTmpConstOrganizeRst[i].bTime), DateTime.Now, 2))//达到
                                    {
                                        OrganizeConstRst _organizeRst = new OrganizeConstRst();
                                        _organizeRst = m_NoticeTmpConstOrganizeRst[i];
                                        _organizeRst.eTime = DateTime.Now.ToString();
                                        m_NoticeConstOrganizeRst.Add(_organizeRst);

                                        organizeRst.bTime = DateTime.Now.ToString();
                                        organizeRst.eTime = DateTime.Now.ToString();
                                        m_NoticeTmpConstOrganizeRst[i] = organizeRst;
                                    }
                                    else
                                    {
                                        organizeRst.bTime = DateTime.Now.ToString();
                                        organizeRst.eTime = DateTime.Now.ToString();
                                        m_NoticeTmpConstOrganizeRst[i] = organizeRst;
                                    }
                                }

                            }
                        }

                        //否则添加记录
                        if (!find)
                        {
                            organizeRst.bTime = DateTime.Now.ToString();
                            organizeRst.eTime = DateTime.Now.ToString();
                            m_NoticeTmpConstOrganizeRst.Add(organizeRst);
                        }
                    }
                }
            }
            else if(flag == 1)//国战结束，全部计算
            {
                foreach (var it in m_NoticeTmpConstOrganizeRst)
                {
                    if (IsConstStageReward(Convert.ToDateTime(it.bTime), DateTime.Now, 2))//达到奖励条件
                    {
                        m_NoticeConstOrganizeRst.Add(it);
                    }
                }
                m_NoticeTmpConstOrganizeRst.Clear();
            }

            return m_NoticeConstOrganizeRst;
        }

        private void ClearNoticeTmpConstStage()
        {
            m_NoticeTmpConstOrganizeRst.Clear();
        }

        private void ClearNoticeConstStage()
        {
            m_NoticeConstOrganizeRst.Clear();
        }

        private void WorkThread(object obj)
        {
            WarLastStrat(m_BaseForlder + "\\Login\\cwar\\", "cwar_last.txt");
            OrganizeLastStrat(m_BaseForlder + "\\DataBase\\saves\\", "organize.dat");

            while (true)
            {
                if (!m_Stop)
                {
                    //国战结束
                    if (IsWarFinish())
                    {
                        //SGExHandle SGEx = new SGExHandle();
                        GetHeroScore(true);
                        //发送奖励
                        if (AutoSend)
                        {
                            SendWarReward();
                            //SGEx.SendWorldWords("国战个人奖励已经发放，请到大鸿儒处领取虚宝奖励！");
                        }
                        
                        //获取占城情况
                        GetNoticeStage();
                        if(false) //发送占城奖励
                        {
                            SendStageReward();
                            //SGEx.SendWorldWords("国战占城奖励已经发放，请军团长到大鸿儒处领取虚宝奖励！");
                        }

                        //获取防守情况
                        GetNoticeConstStage(1);
                        if (false) //发送防守奖励
                        {
                            SendStageConstReward();
                            //SGEx.SendWorldWords("国战防守奖励已经发放，请军团长到大鸿儒处领取虚宝奖励！");
                        } 
                        //回调结束界面操作
                        if (m_WarFinishCallFunc != null)
                        {
                            m_WarFinishCallFunc();
                        }

                        ClearNoticeTmpConstStage();
                        ClearNoticeConstStage();
                    }
                    if (IsUnderWar())
                    {
                        GetNoticeConstStage(0);
                        //回调状态界面操作
                        if (m_WarStatusCallFunc != null)
                        {
                            m_WarStatusCallFunc(1);
                        }
                    }
                    else {
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
    }
}
