using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BOSS刷新工具
{
    public struct Map_Str
    {
        public string id;
        public string x;
        public string y;
    }
    public struct Army_Str
    {
        public string code;
        public string name;
        public string reborn_delay;
        public string reborn_range;
        public string disappear_time;
        public List<string> list;
        public List<Map_Str> map;
    }

    public struct ArmyName_Str
    {
        public string id;
        public string name;
    }

    public class ArmyCtrl
    {
        private List<Army_Str> m_ArmyList = new List<Army_Str>();
        private List<ArmyName_Str> m_ArmyNameList = new List<ArmyName_Str>();

        private string m_Forder = "";
        public bool SetForder(string forder)
        {
            m_Forder = forder;

            return true;
        }

        public List<Army_Str> GetArmyList()
        {
            return m_ArmyList;
        }

        public List<ArmyName_Str> GetArmyNameList()
        {
            return m_ArmyNameList;
        }

        public bool LoadArmyInfo()
        {
            //文件存在
            if (!File.Exists(m_Forder + "\\ARMY.TXT"))
            {
                return false;
            }

            //读取
            m_ArmyList.Clear();

            FileStream fs = new FileStream(m_Forder + "\\ARMY.TXT", FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(fs, Encoding.GetEncoding(950));
            reader.DiscardBufferedData();
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            reader.BaseStream.Position = 0;

            Army_Str army = new Army_Str(); ;
            army.code = "";
            army.name = "";
            army.reborn_delay = "";
            army.reborn_range = "";
            army.disappear_time = "";
            army.list = new List<string>();
            army.map = new List<Map_Str>();

            bool get = false;
            string strLine = "";
            strLine = reader.ReadLine();
            while (strLine != null)
            {
                strLine = strLine.Split('/')[0];

                if (strLine.Contains("[army]") && strLine.Substring(0, CFormat.StringLength("[army]")) == "[army]")
                {
                    if (get)
                    {
                        //add army
                        m_ArmyList.Add(army);
                    }
                    get = true;
                    army = new Army_Str(); ;
                    army.code = "";
                    army.name = "";
                    army.reborn_delay = "";
                    army.reborn_range = "";
                    army.disappear_time = "";
                    army.list = new List<string>();
                    army.map = new List<Map_Str>();
                }
                else if (strLine.Contains("code = ") && strLine.Substring(0, CFormat.StringLength("code = ")) == "code = ")
                {
                    army.code = CFormat.PureString(strLine.Split('=')[1]);
                }
                else if (strLine.Contains("name = ") && strLine.Substring(0, CFormat.StringLength("name = ")) == "name = ")
                {
                    army.name = CFormat.PureString(strLine.Split('=')[1]);
                }
                else if (strLine.Contains("reborn_delay = ") && strLine.Substring(0, CFormat.StringLength("reborn_delay = ")) == "reborn_delay = ")
                {
                    army.reborn_delay = CFormat.PureString(strLine.Split('=')[1]);
                }
                else if (strLine.Contains("reborn_range = ") && strLine.Substring(0, CFormat.StringLength("reborn_range = ")) == "reborn_range = ")
                {
                    army.reborn_range = CFormat.PureString(strLine.Split('=')[1]);
                }
                else if (strLine.Contains("disappear_time = ") && strLine.Substring(0, CFormat.StringLength("disappear_time = ")) == "disappear_time = ")
                {
                    army.disappear_time = CFormat.PureString(strLine.Split('=')[1]);
                }
                else if (strLine.Contains("list = ") && strLine.Substring(0, CFormat.StringLength("list = ")) == "list = ")
                {
                    var lst = CFormat.PureString(strLine.Split('=')[1]).Split(',');
                    foreach (var item in lst)
                    {
                        army.list.Add(item);
                    }
                }
                else if (strLine.Contains("map = ") && strLine.Substring(0, CFormat.StringLength("map = ")) == "map = ")
                {
                    var map = CFormat.PureString(strLine.Split('=')[1]).Split(',');
                    Map_Str _map;
                    _map.id = map[0];
                    _map.x = map[1];
                    _map.y = map[2];
                    army.map.Add(_map);
                }

                strLine = null;
                strLine = reader.ReadLine();
            }

            //末了 写一次
            m_ArmyList.Add(army);

            reader.Close();
            fs.Close();

            return true;
        }
        public bool LoadArmyNameInfo()
        {
            //文件存在
            if (!File.Exists(m_Forder + "\\ARMY_NAME.TXT"))
            {
                return false;
            }

            //读取
            m_ArmyNameList.Clear();

            FileStream fs = new FileStream(m_Forder + "\\ARMY_NAME.TXT", FileMode.Open, FileAccess.Read);
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
                    ArmyName_Str army_name;
                    string tmp = strLine.Split(new string[] { "item = " }, StringSplitOptions.RemoveEmptyEntries)[0];
                    string id = tmp.Split(',')[0];
                    string name = tmp.Split(',')[1];

                    army_name.id = id;
                    army_name.name = name;
                    m_ArmyNameList.Add(army_name);
                }
                strLine = null;
                strLine = reader.ReadLine();
            }

            reader.Close();
            fs.Close();

            return true;
        }
        public bool SaveArmyInfo()
        {
            string file = m_Forder + "\\ARMY.TXT";
            if (!File.Exists(file))
            {
                return false;
            }


            //备份原文件
            string time = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            File.Copy(file, @"backup\\" + time + "ARMY.TXT");

            //删除原文件
            File.Delete(file);

            //写文件
            FileStream rtfs = new FileStream(file, FileMode.Create, FileAccess.Write);
            StreamWriter rt = new StreamWriter(rtfs, Encoding.GetEncoding(950));
            rt.BaseStream.Seek(0, SeekOrigin.Begin);
            rt.BaseStream.Position = 0;

            rt.WriteLine("#include PLAYERS.H");
            rt.WriteLine("");

            foreach (var army in m_ArmyList)
            {
                rt.WriteLine("");
                rt.WriteLine("[army]");
                rt.WriteLine("code = " + army.code);
                rt.WriteLine("name = " + army.name);
                rt.WriteLine("reborn_delay = " + army.reborn_delay);
                rt.WriteLine("reborn_range = " + army.reborn_range);
                rt.WriteLine("disappear_time = " + army.disappear_time);
                string list = "list = ";
                List<string> rolelist = army.list;
                for (var role = 0; role < rolelist.Count; role++)
                {
                    if (role == rolelist.Count - 1)
                    {
                        list += rolelist[role];
                    }
                    else
                    {
                        list += rolelist[role] + ",";
                    }
                }
                rt.WriteLine(list);

                List<Map_Str> maplist = army.map;
                foreach (var map in maplist)
                {
                    string _map = "map = " + map.id + "," + map.x + "," + map.y;
                    rt.WriteLine(_map);
                }
            }
            rt.Close();
            rtfs.Close();

            if (File.Exists(@"new\\ARMY.TXT"))
            {
                File.Delete(@"new\\ARMY.TXT");
            }
            File.Copy(file, @"new\\ARMY.TXT");

            return true;
        }

        public bool SaveArmyNameInfo()
        {
            string file = m_Forder + "\\ARMY_NAME.TXT";
            if (!File.Exists(file))
            {
                return false;
            }


            //备份原文件
            string time = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            File.Copy(file, @"backup\\" + time + "ARMY_NAME.TXT");

            //删除原文件
            File.Delete(file);

            //写文件
            FileStream rtfs = new FileStream(file, FileMode.Create, FileAccess.Write);
            StreamWriter rt = new StreamWriter(rtfs, Encoding.GetEncoding(950));
            rt.BaseStream.Seek(0, SeekOrigin.Begin);
            rt.BaseStream.Position = 0;

            rt.WriteLine("[name]");
            rt.WriteLine("");
            foreach (var item in m_ArmyNameList)
            {
                /*item = 100,直劍*/
                string line = "item = ";
                line += item.id;
                line += ",";
                line += item.name;

                rt.WriteLine(line);
            }
            rt.Close();
            rtfs.Close();

            if (File.Exists(@"new\\ARMY_NAME.TXT"))
            {
                File.Delete(@"new\\ARMY_NAME.TXT");
            }
            File.Copy(file, @"new\\ARMY_NAME.TXT");

            return true;
        }

        public string GetNameById(string id)
        {
            string result = "";
            foreach (var item in m_ArmyNameList)
            {
                if (item.id == id)
                {
                    result = item.name;
                    break;
                }
            }
            return result;
        }

        public string GetIdByName(string name)
        {
            string result = "";
            foreach (var item in m_ArmyNameList)
            {
                if (item.name == name)
                {
                    result = item.id;
                    break;
                }
            }
            return result;
        }

        public bool GetAttrByCode(string code, out Army_Str army)
        {
            foreach (var item in m_ArmyList)
            {
                if (CFormat.ToSimplified(item.code) == code)
                {
                    army = item;
                    return true;
                }
            }
            army = new Army_Str();
            army.list = new List<string>();
            army.map = new List<Map_Str>();
            army.code = code;
            return false;
        }

        public bool AddListPlayer(string code, string player)
        {
            foreach (var item in m_ArmyList)
            {
                if (CFormat.ToSimplified(item.code) == code)
                {
                    if (item.list.Contains(player))
                    {
                        return false;
                    }
                    item.list.Add(player);
                    return true;
                }
            }

            return false;
        }

        public bool DelListPlayer(string code, string player)
        {
            foreach (var item in m_ArmyList)
            {
                if (CFormat.ToSimplified(item.code) == code)
                {
                    if (item.list.Contains(player))
                    {
                        item.list.Remove(player);
                        return true;
                    }
                }
            }

            return false;
        }

        public bool AddListMap(string code, Map_Str map)
        {
            foreach (var item in m_ArmyList)
            {
                if (CFormat.ToSimplified(item.code) == code)
                {
                    item.map.Add(map);
                    return true;
                }
            }

            return false;
        }

        public bool DelListMap(string code, Map_Str map)
        {
            foreach (var item in m_ArmyList)
            {
                if (CFormat.ToSimplified(item.code) == code)
                {
                    if (item.map.Contains(map))
                    {
                        item.map.Remove(map);
                        return true;
                    }
                }
            }

            return false;
        }

        public bool MdfyBaseInfoByName(string name, string delay, string range, string disappear)
        {
            for (int i = 0; i < m_ArmyList.Count; i++)
            {
                if (CFormat.ToSimplified(m_ArmyList[i].name) == name)
                {
                    Army_Str newStr = new Army_Str();
                    newStr.code = m_ArmyList[i].code;
                    newStr.name = m_ArmyList[i].name;
                    newStr.reborn_delay = delay;
                    newStr.reborn_range = range;
                    newStr.disappear_time = disappear;
                    newStr.list = m_ArmyList[i].list;
                    newStr.map = m_ArmyList[i].map;
                    m_ArmyList[i] = newStr;
                }
            }

            return true;
        }

        public bool MdfyBaseInfoByCode(string code, string delay, string range, string disappear)
        {
            for (int i = 0; i < m_ArmyList.Count; i++)
            {
                if (CFormat.ToSimplified(m_ArmyList[i].code) == code)
                {
                    Army_Str newStr = new Army_Str();
                    newStr.code = m_ArmyList[i].code;
                    newStr.name = m_ArmyList[i].name;
                    newStr.reborn_delay = delay;
                    newStr.reborn_range = range;
                    newStr.disappear_time = disappear;
                    newStr.list = m_ArmyList[i].list;
                    newStr.map = m_ArmyList[i].map;
                    m_ArmyList[i] = newStr;
                    return true;
                }
            }

            return false;
        }

        public bool ArmyNameRebuild(int startId)
        {
            List<ArmyName_Str> newNameList = new List<ArmyName_Str>();
            for (int i = 0; i < m_ArmyList.Count; i++)
            {
                int name = i + startId;
                Army_Str item = m_ArmyList[i];

                //获取原name string,
                ArmyName_Str newName;
                string oldName = GetNameById(item.name);
                newName.id = name.ToString();
                newName.name = oldName;
                newNameList.Add(newName);

                item.name = name.ToString();
                m_ArmyList[i] = item;
            }

            //修改
            m_ArmyNameList = newNameList;

            SaveArmyInfo();
            SaveArmyNameInfo();

            return true;
        }
    }
}
