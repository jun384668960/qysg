using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BOSS刷新工具
{
    public struct Player_Str
    {
        public string code;
        public string name;
        public string drop;
        public bool drop_enable;
        public string drop_mission;
        public List<string> more;
        public string appear_ratio;
    }
    public struct PlayerDef_Str
    {
        public string id;
        public string name;
    }

    public struct PlayerName_Str
    {
        public string id;
        public string name;
    }
    public class PlayerCtrl
    {
        private List<Player_Str> m_PlayerList = new List<Player_Str>();
        private List<PlayerDef_Str> m_PlayerDefList = new List<PlayerDef_Str>();
        private List<PlayerName_Str> m_PlayerNameList = new List<PlayerName_Str>();

        private string m_Forder = "";
        private int m_lastDefId = 0;
        private int m_lastNameId = 0;
        private int m_playerCount = 0;

        public int GetLastNameId()
        {
            return m_lastNameId;
        }
        public int GetLastDefId()
        {
            return m_lastDefId;
        }
        public bool SetForder(string forder)
        {
            m_Forder = forder;

            return true;
        }

        public List<Player_Str> GetPlayerList()
        {
            return m_PlayerList;
        }

        public List<PlayerDef_Str> GetPlayerDefList()
        {
            return m_PlayerDefList;
        }

        public bool LoadPlayerList()
        {
            //文件存在
            if (!File.Exists(m_Forder + "\\PLAYERS.TXT"))
            {
                return false;
            }

            //读取
            m_PlayerList.Clear();

            FileStream fs = new FileStream(m_Forder + "\\PLAYERS.TXT", FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(fs, Encoding.GetEncoding(950));
            reader.DiscardBufferedData();
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            reader.BaseStream.Position = 0;

            Player_Str player;
            player.code = "";
            player.name = "";
            player.appear_ratio = "";
            player.drop = "";
            player.drop_enable = true;
            player.drop_mission = "";
            player.more = new List<string>();

            bool get = false;
            string strLine = "";
            strLine = reader.ReadLine();
            while (strLine != null)
            {
                strLine = strLine.Split('/')[0];

                if (strLine.Contains("[character]") && strLine.Substring(0, CFormat.StringLength("[character]")) == "[character]")
                {
                    if (get)
                    {
                        //add army
                        m_PlayerList.Add(player);
                    }
                    get = true;
                    player.code = "";
                    player.name = "";
                    player.drop = "";
                    player.drop_enable = true;
                    player.drop_mission = "";
                    player.appear_ratio = "";
                    player.more = new List<string>();
                }
                else if (strLine.Contains("code = ") && strLine.Substring(0, CFormat.StringLength("code = ")) == "code = ")
                {
                    player.code = CFormat.PureString(strLine.Split('=')[1]);
                }
                else if (strLine.Contains("name = ") && strLine.Substring(0, CFormat.StringLength("name = ")) == "name = ")
                {
                    player.name = CFormat.PureString(strLine.Split('=')[1]);
                    m_lastNameId = int.Parse(player.name);
                }
                else if (strLine.Contains("appear_ratio = ") && strLine.Substring(0, CFormat.StringLength("appear_ratio = ")) == "appear_ratio = ")
                {
                    player.appear_ratio = CFormat.PureString(strLine.Split('=')[1]);
                }
                else if (strLine.Contains("drop = ") && strLine.Substring(0, CFormat.StringLength("drop = ")) == "drop = ")
                {
                    player.drop = CFormat.PureString(strLine.Split('=')[1]);
                }
                else if (strLine.Contains("drop_mission = ") && strLine.Substring(0, CFormat.StringLength("drop_mission = ")) == "drop_mission = ")
                {
                    player.drop_mission = CFormat.PureString(strLine.Split('=')[1]);
                }
                else if (strLine.Contains("="))
                {
                    player.more.Add(strLine);
                }

                strLine = null;
                strLine = reader.ReadLine();
            }

            //末了 写一次
            m_PlayerList.Add(player);

            reader.Close();
            fs.Close();

            return true;
        }

        public bool LoadPlayerDefList()
        {
            //文件存在
            if (!File.Exists(m_Forder + "\\PLAYERS.H"))
            {
                return false;
            }

            //读取
            m_PlayerDefList.Clear();

            FileStream fs = new FileStream(m_Forder + "\\PLAYERS.H", FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(fs, Encoding.GetEncoding(950));
            reader.DiscardBufferedData();
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            reader.BaseStream.Position = 0;

            string strLine = "";
            strLine = reader.ReadLine();
            while (strLine != null)
            {
                strLine = strLine.Split('/')[0];
                if (strLine.Contains("#define"))
                {
                    PlayerDef_Str playerDef;
                    string tmp = strLine.Split(new string[] { "role_" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    string name = tmp.Split(' ')[0].Split('\t')[0];
                    string id = tmp.Replace(" ", "").Replace("\\t", "").Replace(name, "");

                    playerDef.id = CFormat.PureString(id);
                    playerDef.name = CFormat.PureString(name);
                    m_lastDefId = int.Parse(playerDef.id);
                    m_PlayerDefList.Add(playerDef);
                }
                strLine = null;
                strLine = reader.ReadLine();
            }
            m_playerCount = m_PlayerDefList.Count;

            reader.Close();
            fs.Close();

            return true;
        }

        public bool LoadPlayerNameList()
        {
            //文件存在
            if (!File.Exists(m_Forder + "\\PLAYERS_NAME.TXT"))
            {
                return false;
            }

            //读取
            m_PlayerNameList.Clear();

            FileStream fs = new FileStream(m_Forder + "\\PLAYERS_NAME.TXT", FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(fs, Encoding.GetEncoding(950));
            reader.DiscardBufferedData();
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            reader.BaseStream.Position = 0;

            string strLine = "";
            strLine = reader.ReadLine();
            while (strLine != null)
            {
                strLine = strLine.Split('/')[0];
                if (strLine.Contains("item = ") && strLine.Substring(0, CFormat.StringLength("item = ")) == "item = ")
                {
                    PlayerName_Str player_name;
                    string tmp = strLine.Split(new string[] { "item = " }, StringSplitOptions.RemoveEmptyEntries)[0];
                    string id = tmp.Split(',')[0];
                    string name = tmp.Split(',')[1];

                    player_name.id = id;
                    player_name.name = name;
                    m_PlayerNameList.Add(player_name);
                }
                strLine = null;
                strLine = reader.ReadLine();
            }

            reader.Close();
            fs.Close();

            return true;
        }

        public bool SavePlayerInfo()
        {
            SavePlayerDef();
            SavePlayerName();
            SavePlayerTxt();

            return true;
        }

        public bool SavePlayerDef()
        {
            string file = m_Forder + "\\PLAYERS.H";
            if (!File.Exists(file))
            {
                return false;
            }


            //备份原文件
            string time = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            File.Copy(file, @"backup\\" + time + "PLAYERS.H");

            //删除原文件
            File.Delete(file);

            //写文件
            FileStream rtfs = new FileStream(file, FileMode.Create, FileAccess.Write);
            StreamWriter rt = new StreamWriter(rtfs, Encoding.GetEncoding(950));
            rt.BaseStream.Seek(0, SeekOrigin.Begin);
            rt.BaseStream.Position = 0;

            foreach (var item in m_PlayerDefList)
            {
                /*#define item_直劍				1*/
                string line = "#define role_";
                line += item.name;
                line += "\t\t\t";
                line += item.id;

                rt.WriteLine(line);
            }
            rt.Close();
            rtfs.Close();

            if (File.Exists(@"new\\PLAYERS.H"))
            {
                File.Delete(@"new\\PLAYERS.H");
            }
            File.Copy(file, @"new\\PLAYERS.H");

            return true;
        }
        public bool SavePlayerName()
        {
            string file = m_Forder + "\\PLAYERS_NAME.TXT";
            if (!File.Exists(file))
            {
                return false;
            }


            //备份原文件
            string time = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            File.Copy(file, @"backup\\" + time + "PLAYERS_NAME.TXT");

            //删除原文件
            File.Delete(file);

            //写文件
            FileStream rtfs = new FileStream(file, FileMode.Create, FileAccess.Write);
            StreamWriter rt = new StreamWriter(rtfs, Encoding.GetEncoding(950));
            rt.BaseStream.Seek(0, SeekOrigin.Begin);
            rt.BaseStream.Position = 0;

            rt.WriteLine("[name]");
            rt.WriteLine("");
            foreach (var item in m_PlayerNameList)
            {
                string line = "item = ";
                line += item.id;
                line += ",";
                line += item.name;

                rt.WriteLine(line);
            }
            rt.Close();
            rtfs.Close();

            if (File.Exists(@"new\\PLAYERS_NAME.TXT"))
            {
                File.Delete(@"new\\PLAYERS_NAME.TXT");
            }
            File.Copy(file, @"new\\PLAYERS_NAME.TXT");

            return true;
        }

        public bool SavePlayerTxt()
        {
            string file = m_Forder + "\\PLAYERS.TXT";
            if (!File.Exists(file))
            {
                return false;
            }


            //备份原文件
            string time = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            File.Copy(file, @"backup\\" + time + "PLAYERS.TXT");

            //删除原文件
            File.Delete(file);

            //写文件
            FileStream rtfs = new FileStream(file, FileMode.Create, FileAccess.Write);
            StreamWriter rt = new StreamWriter(rtfs, Encoding.GetEncoding(950));
            rt.BaseStream.Seek(0, SeekOrigin.Begin);
            rt.BaseStream.Position = 0;

            rt.WriteLine("#include TYPE.H");
            rt.WriteLine("#include GAMERESOURCE.H");
            rt.WriteLine("#include PLAYERS.H");
            rt.WriteLine("#include ITEM.H");
            rt.WriteLine("#include MAGIC.H");
            rt.WriteLine("#include MAGIC_EXP.H");
            rt.WriteLine("");

            foreach (var item in m_PlayerList)
            {
                rt.WriteLine("");
                rt.WriteLine("[character]");
                rt.WriteLine("code = " + item.code);
                rt.WriteLine("name = " + item.name);
                if (item.drop != "")
                {
                    if (item.drop_enable)
                    {
                        rt.WriteLine("drop = " + item.drop);
                    }
                    else 
                    {
                        rt.WriteLine("//drop = " + item.drop);
                    }
                }
                if (item.drop_mission != "")
                {
                    rt.WriteLine("drop_mission = " + item.drop_mission);
                }
                if (item.appear_ratio != "")
                {
                    rt.WriteLine("appear_ratio = " + item.appear_ratio);
                }
                foreach (var it in item.more)
                {
                    rt.WriteLine(it);
                }
            }
            rt.Close();
            rtfs.Close();

            if (File.Exists(@"new\\PLAYERS.TXT"))
            {
                File.Delete(@"new\\PLAYERS.TXT");
            }
            File.Copy(file, @"new\\PLAYERS.TXT");

            return true;
        }

        //role_XX ID   此处把 role_XX称作id
        public bool GetAttrById(string id, out Player_Str player)
        {
            foreach (var item in m_PlayerList)
            {
                if (CFormat.ToSimplified(item.code) == id)
                {
                    player = item;
                    return true;
                }
            }
            player = new Player_Str();
            player.more = new List<string>();
            return false;
        }

        public string NameSimpToTrad(string name)
        {
            string ret = "";
            foreach (var item in m_PlayerDefList)
            {
                if (CFormat.ToSimplified(CFormat.PureString(name)) == CFormat.ToSimplified(CFormat.PureString(item.name)))
                {
                    return item.name;
                }
            }

            return ret;
        }

        public string GetIdByCode(string code)
        {
            string id = "";
            foreach (var player in m_PlayerDefList)
            {
                if (CFormat.ToSimplified(CFormat.PureString(code)) == CFormat.ToSimplified(CFormat.PureString("role_" + player.name)))
                {
                    return player.id;
                }
            }

            return id;
        }

        public string GetNameById(string id)
        {
            string name = "";
            foreach (var item in m_PlayerDefList)
            {
                if (CFormat.ToSimplified(item.id) == id)
                {
                    name = item.name;
                    break;
                }
            }

            return name;
        }

        public string GetPlayerName(string id)
        {
            string name = "";
            foreach (var palyer in m_PlayerNameList)
            {
                if (CFormat.ToSimplified(CFormat.PureString(id)) == CFormat.ToSimplified(CFormat.PureString(palyer.id)))
                {
                    return palyer.name;
                }
            }

            return name;
        }

        public List<Player_Str> GetPlayersByDrop(string drop)
        {
            List<Player_Str> list = new List<Player_Str>();
            foreach (var item in m_PlayerList)
            {
                if (CFormat.ToSimplified(item.drop) == drop)
                {
                    list.Add(item);
                }
            }

            return list;
        }

        public bool ClearPlayerDrop(Player_Str player)
        {
            for (int i = 0; i < m_PlayerList.Count; i++ )
            {
                if (m_PlayerList[i].code == player.code)
                {
                    player.drop_enable = false;
                    m_PlayerList[i] = player;
                    return true;
                }
            }

            return false;
        }

        public bool AddPlayerAttr(Player_Str player)
        {
            m_PlayerList.Add(player);
            return true;
        }
        public bool AddPlayerName(PlayerName_Str player)
        {
            m_PlayerNameList.Add(player);
            return true;
        }
        public bool AddPlayerDef(PlayerDef_Str player)
        {
            m_PlayerDefList.Add(player);
            return true;
        }
        public string GetPalyerName(string id)
        {
            string name = "";
            foreach (var player in m_PlayerNameList)
            {
                if (CFormat.ToSimplified(CFormat.PureString(id)) == CFormat.ToSimplified(CFormat.PureString(player.id)))
                {
                    return player.name;
                }
            }

            return name;
        }
        public bool PalyerDefNameRebuild(out int maxName)
        {
            //List<PlayerDef_Str> newDefList = new List<PlayerDef_Str>();
            //List<PlayerName_Str> newNameList = new List<PlayerName_Str>();
            //int def = m_lastDefId;
            //int name = m_lastNameId;

            //for (int i = m_playerCount; i < m_PlayerList.Count; i++)
            //{
            //    def++;
            //    name++;

            //    Player_Str player = m_PlayerList[i];

            //    //define
            //    PlayerDef_Str newDef;
            //    newDef.id = def.ToString();
            //    newDef.name = player.code.Split(new string[] { "role_" }, StringSplitOptions.RemoveEmptyEntries)[0];
            //    m_PlayerDefList.Add(newDef);

            //    //获取原name string,
            //    PlayerName_Str newName;
            //    string oldName = GetPalyerName(player.name);
            //    newName.id = name.ToString();
            //    newName.name = oldName;
            //    //m_PlayerNameList.Add(newName);

            //    player.name = name.ToString();

            //    m_PlayerList[i] = player;
            //}

            //修改
            //m_PlayerDefList = newDefList;
            //m_PlayerNameList = newNameList;

            //
            SavePlayerDef();
            SavePlayerTxt();
            SavePlayerName();
            maxName = 100;

            return true;
        }
    }
}
