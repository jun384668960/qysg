using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace 物品工具
{
    public struct Item_Str
    {
        public string code;
        public string name;
        public string cost;
        public string sell;
        public string help_string;
        public string limit_level;
        public List<string> more;
    }
    public struct ItemDef_Str
    {
        public string id;
        public string name;
    }

    public struct ItemName_Str
    {
        public string id;
        public string name;
    }

    public struct ItemHelp_Str
    {
        public string id;
        public string name;
    }
    class ItemCtrl
    {
        private static List<ItemDef_Str> m_ItemDefList = new List<ItemDef_Str>();
        private static List<Item_Str> m_ItemList = new List<Item_Str>();
        private static List<ItemName_Str> m_ItemNameList = new List<ItemName_Str>();
        private static List<ItemHelp_Str> m_ItemHelpList = new List<ItemHelp_Str>();

        public static List<Item_Str> GetItemList()
        {
            return m_ItemList;
        }
        public static List<ItemDef_Str> GetItemDefList()
        {
            return m_ItemDefList;
        }
        public static List<ItemName_Str> GetItemNameList()
        {
            return m_ItemNameList;
        }
        public static List<ItemHelp_Str> GetItemHelpList()
        {
            return m_ItemHelpList;
        }

        public static bool LoadItemDefList()
        {
            //文件存在
            if (!File.Exists("profile\\ITEM.H"))
            {
                return false;
            }

            //读取
            m_ItemDefList.Clear();

            FileStream fs = new FileStream("profile\\ITEM.H", FileMode.Open, FileAccess.Read);
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
                    ItemDef_Str playerDef;
                    string tmp = strLine.Split(new string[] { "item_" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    string name = tmp.Split(' ')[0].Split('\t')[0];
                    string id = tmp.Replace(" ", "").Replace("\\t", "").Replace(name, "");

                    playerDef.id = CFormat.PureString(id);
                    playerDef.name = CFormat.PureString(name);
                    m_ItemDefList.Add(playerDef);
                }
                strLine = null;
                strLine = reader.ReadLine();
            }

            reader.Close();
            fs.Close();

            return true;
        }

        public static bool LoadItemList()
        {
            //文件存在
            if (!File.Exists("profile\\ITEM.TXT"))
            {
                return false;
            }

            //读取
            m_ItemList.Clear();

            FileStream fs = new FileStream("profile\\ITEM.TXT", FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(fs, Encoding.GetEncoding(950));
            reader.DiscardBufferedData();
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            reader.BaseStream.Position = 0;

            Item_Str item;
            item.code = "";
            item.name = "";
            item.cost = "";
            item.sell = "";
            item.help_string = "";
            item.limit_level = "";
            item.more = new List<string>();

            bool get = false;
            string strLine = "";
            strLine = reader.ReadLine();
            while (strLine != null)
            {
                strLine = strLine.Split('/')[0];

                if (strLine.Contains("[item]") && strLine.Contains("[item]"))
                {
                    if (get)
                    {
                        //add army
                        m_ItemList.Add(item);
                    }
                    get = true;
                    item.code = "";
                    item.name = "";
                    item.cost = "";
                    item.sell = "";
                    item.help_string = "";
                    item.limit_level = "";
                    item.more = new List<string>();
                }
                else if (strLine.Contains("code = ") && strLine.Substring(0, CFormat.StringLength("code =")) == "code =")
                {
                    item.code = CFormat.PureString(strLine.Split('=')[1]);
                }
                else if (strLine.Contains("name = ") && strLine.Substring(0, CFormat.StringLength("name =")) == "name =")
                {
                    item.name = CFormat.PureString(strLine.Split('=')[1]);
                }
                else if (strLine.Contains("cost = ") && strLine.Substring(0, CFormat.StringLength("cost =")) == "cost =")
                {
                    item.cost = CFormat.PureString(strLine.Split('=')[1]);
                }
                else if (strLine.Contains("sell = ") && strLine.Substring(0, CFormat.StringLength("sell =")) == "sell =")
                {
                    item.sell = CFormat.PureString(strLine.Split('=')[1]);
                }
                else if (strLine.Contains("help_string = ") && strLine.Substring(0, CFormat.StringLength("help_string =")) == "help_string =")
                {
                    item.help_string = CFormat.PureString(strLine.Split('=')[1]);
                }
                else if (strLine.Contains("limit_level = ") && strLine.Substring(0, CFormat.StringLength("limit_level =")) == "limit_level =")
                {
                    item.limit_level = CFormat.PureString(strLine.Split('=')[1]);
                }
                else if (strLine.Contains("="))
                {
                    item.more.Add(strLine);
                }

                strLine = null;
                strLine = reader.ReadLine();
            }

            //末了 写一次
            m_ItemList.Add(item);

            reader.Close();
            fs.Close();

            return true;
        }

        public static bool LoadItemNameList()
        {
            //文件存在
            if (!File.Exists("profile\\ITEM_NAME.TXT"))
            {
                return false;
            }

            //读取
            m_ItemNameList.Clear();

            FileStream fs = new FileStream("profile\\ITEM_NAME.TXT", FileMode.Open, FileAccess.Read);
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
                    ItemName_Str item_name;
                    string tmp = strLine.Split(new string[] { "item = " }, StringSplitOptions.RemoveEmptyEntries)[0];
                    string id = tmp.Split(',')[0];
                    string name = tmp.Split(',')[1];

                    item_name.id = id;
                    item_name.name = name;
                    m_ItemNameList.Add(item_name);
                }
                strLine = null;
                strLine = reader.ReadLine();
            }

            reader.Close();
            fs.Close();

            return true;
        }

        public static bool LoadItemHelpList()
        {
            //文件存在
            if (!File.Exists("profile\\ITEM_HELP_STRING.TXT"))
            {
                return false;
            }

            //读取
            m_ItemHelpList.Clear();

            FileStream fs = new FileStream("profile\\ITEM_HELP_STRING.TXT", FileMode.Open, FileAccess.Read);
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
                    ItemHelp_Str item_help;
                    string tmp = strLine.Split(new string[] { "item = " }, StringSplitOptions.RemoveEmptyEntries)[0];
                    string id = tmp.Split(',')[0];
                    string name = tmp.Split(',')[1];

                    item_help.id = id;
                    item_help.name = name;
                    m_ItemHelpList.Add(item_help);
                }
                strLine = null;
                strLine = reader.ReadLine();
            }

            reader.Close();
            fs.Close();

            return true;
        }

        public static bool SaveItemDefInfo()
        { 
            string file = "profile\\ITEM.H";
            if (!File.Exists(file))
            {
                return false;
            }


            //备份原文件
            string time = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            File.Copy(file, @"back\\" + time + "ITEM.H");

            //删除原文件
            File.Delete(file);

            //写文件
            FileStream rtfs = new FileStream(file, FileMode.Create, FileAccess.Write);
            StreamWriter rt = new StreamWriter(rtfs, Encoding.GetEncoding(950));
            rt.BaseStream.Seek(0, SeekOrigin.Begin);
            rt.BaseStream.Position = 0;

            foreach (var item in m_ItemDefList)
            {
                /*#define item_直劍				1*/
                string line = "#define item_";
                line += item.name;
                line += "\t\t\t";
                line += item.id;

                rt.WriteLine(line);
            }
            rt.Close();
            rtfs.Close();

            if (File.Exists(@"new\\ITEM.H"))
            {
                File.Delete(@"new\\ITEM.H");
            }
            File.Copy(file, @"new\\ITEM.H");

            return true;
        }
        public static bool SaveItemInfo()
        {
            string file = "profile\\ITEM.TXT";
            if (!File.Exists(file))
            {
                return false;
            }


            //备份原文件
            string time = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            File.Copy(file, @"back\\" + time + "ITEM.TXT");

            //删除原文件
            File.Delete(file);

            //写文件
            FileStream rtfs = new FileStream(file, FileMode.Create, FileAccess.Write);
            StreamWriter rt = new StreamWriter(rtfs, Encoding.GetEncoding(950));
            rt.BaseStream.Seek(0, SeekOrigin.Begin);
            rt.BaseStream.Position = 0;

            rt.WriteLine("#include TYPE.H");
            rt.WriteLine("#include PROCESS.H");
            rt.WriteLine("#include DEFINETXT.H");
            rt.WriteLine("#include ITEM.H");
            rt.WriteLine("#include RANGETABLE.H");
            rt.WriteLine("#include MAGIC.H");
            rt.WriteLine("#include MAGIC_EXP.H");
            rt.WriteLine("#include PLAYERS.H");
            rt.WriteLine("#include STAGE.H");
            rt.WriteLine("#include COMPOSITE_TABLE.H");
            rt.WriteLine("");
            rt.WriteLine("");
            rt.WriteLine("[ini]");
            rt.WriteLine("item_max = 20000");
            rt.WriteLine("");

            foreach (var item in m_ItemList)
            {
                /*public string code;
                public string name;
                public string cost;
                public string sell;
                public string help_string;
                public List<string> more;*/
                rt.WriteLine("");
                rt.WriteLine("[item]");
                rt.WriteLine("code = " + item.code);
                rt.WriteLine("name = " + item.name);
                rt.WriteLine("cost = " + item.cost);
                rt.WriteLine("sell = " + item.sell);
                rt.WriteLine("help_string = " + item.help_string);
                foreach (var it in item.more)
                {
                    rt.WriteLine(it);
                }
            }
            rt.Close();
            rtfs.Close();

            if (File.Exists(@"new\\ITEM.TXT"))
            {
                File.Delete(@"new\\ITEM.TXT");
            }
            File.Copy(file, @"new\\ITEM.TXT");

            return true;
        }

        public static bool SaveItemNameInfo()
        {
            string file = "profile\\ITEM_NAME.TXT";
            if (!File.Exists(file))
            {
                return false;
            }


            //备份原文件
            string time = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            File.Copy(file, @"back\\" + time + "ITEM_NAME.TXT");

            //删除原文件
            File.Delete(file);

            //写文件
            FileStream rtfs = new FileStream(file, FileMode.Create, FileAccess.Write);
            StreamWriter rt = new StreamWriter(rtfs, Encoding.GetEncoding(950));
            rt.BaseStream.Seek(0, SeekOrigin.Begin);
            rt.BaseStream.Position = 0;

            rt.WriteLine("[name]");
            rt.WriteLine("");
            foreach (var item in m_ItemNameList)
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

            if (File.Exists(@"new\\ITEM_NAME.TXT"))
            {
                File.Delete(@"new\\ITEM_NAME.TXT");
            }
            File.Copy(file, @"new\\ITEM_NAME.TXT");

            return true;
        }

        public static bool SaveItemHelpInfo()
        {
            string file = "profile\\ITEM_HELP_STRING.TXT";
            if (!File.Exists(file))
            {
                return false;
            }

            //备份原文件
            string time = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            File.Copy(file, @"back\\" + time + "ITEM_HELP_STRING.TXT");

            //删除原文件
            File.Delete(file);

            //写文件
            FileStream rtfs = new FileStream(file, FileMode.Create, FileAccess.Write);
            StreamWriter rt = new StreamWriter(rtfs, Encoding.GetEncoding(950));
            rt.BaseStream.Seek(0, SeekOrigin.Begin);
            rt.BaseStream.Position = 0;

            rt.WriteLine("[name]");
            rt.WriteLine("");
            foreach (var item in m_ItemHelpList)
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

            if (File.Exists(@"new\\ITEM_HELP_STRING.TXT"))
            {
                File.Delete(@"new\\ITEM_HELP_STRING.TXT");
            }
            File.Copy(file, @"new\\ITEM_HELP_STRING.TXT");

            return true;
        }

        //item_XX ID   此处把 item_XX称作id
        public static bool GetAttrById(string id, out Item_Str item)
        {
            foreach (var it in m_ItemList)
            {
                if (CFormat.ToSimplified(it.code) == id)
                {
                    item = it;
                    return true;
                }
            }
            item = new Item_Str();
            item.more = new List<string>();
            return false;
        }

        public static bool MdfyItemAttr(string id, Item_Str item)
        {
            for (int i = 0; i < m_ItemList.Count; i++ )
            {
                if (CFormat.ToSimplified(m_ItemList[i].code) == id)
                {
                    m_ItemList[i] = item;
                    return true;
                }
            }

            return false;
        }

        public static string GetIdByCode(string code)
        {
            string id = "";
            foreach (var item in m_ItemDefList)
            {
                if (CFormat.ToTraditional(CFormat.PureString(code)) == CFormat.ToTraditional(CFormat.PureString("item_" + item.name)))
                {
                    return item.id;
                }
            }

            return id;
        }

        public static string GetItemName(string id)
        {
            string name = "";
            foreach(var item in m_ItemNameList)
            {
                if (CFormat.ToTraditional(CFormat.PureString(id)) == CFormat.ToTraditional(CFormat.PureString(item.id)))
                {
                    return item.name;
                }
            }

            return name;
        }
        public static string GetItemHelp(string id)
        {
            string name = "";
            foreach (var item in m_ItemHelpList)
            {
                if (CFormat.ToTraditional(CFormat.PureString(id)) == CFormat.ToTraditional(CFormat.PureString(item.id)))
                {
                    return item.name;
                }
            }

            return name;
        }
        public static bool ItemDefNameHelpRebuild()
        {
            List<ItemDef_Str> newDefList = new List<ItemDef_Str>();
            List<ItemName_Str> newNameList = new List<ItemName_Str>();
            List<ItemHelp_Str> newHelpList = new List<ItemHelp_Str>();
            for (int i = 0; i < m_ItemList.Count; i++ )
            {
                int def = i + 1;
                int name = i + 100;
                int help = i + 8100;
                Item_Str item = m_ItemList[i];

                //define
                ItemDef_Str newDef;
                newDef.id = def.ToString();
                newDef.name = item.code.Split(new string[] { "item_" }, StringSplitOptions.RemoveEmptyEntries)[0];
                newDefList.Add(newDef);

                //获取原name string,
                ItemName_Str newName;
                string oldName = GetItemName(item.name);
                newName.id = name.ToString();
                newName.name = oldName;
                newNameList.Add(newName);

                //获取原help string
                ItemHelp_Str newHelp;
                string oldHelp = GetItemHelp(item.help_string);
                newHelp.id = help.ToString();
                newHelp.name = oldHelp;
                newHelpList.Add(newHelp);

                item.name = name.ToString();
                item.help_string = help.ToString();

                m_ItemList[i] = item;
            }

            //修改
            m_ItemDefList = newDefList;
            m_ItemNameList = newNameList;
            m_ItemHelpList = newHelpList;

            //
            SaveItemDefInfo();
            SaveItemInfo();
            SaveItemNameInfo();
            SaveItemHelpInfo();

            return true;
        }
    }
}
