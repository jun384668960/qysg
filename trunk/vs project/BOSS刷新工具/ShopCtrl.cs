using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BOSS刷新工具
{
    public struct ShopData_Str
    {
        public string code;
        public string ShopName;
        public List<string> more;
        public string ShopType;
        public List<string> list;
    };
    public struct ShopDef_Str
    {
        public string id;
        public string name;
    };

    public struct ShopName_Str
    {
        public string id;
        public string name;
    };

    class ShopCtrl
    {
        private static List<ShopDef_Str> m_ShopDefList = new List<ShopDef_Str>();
        private static List<ShopName_Str> m_ShopNameList = new List<ShopName_Str>();

        private static List<ShopData_Str>[] m_ShopDataList = new List<ShopData_Str>[4];

        public static List<ShopData_Str> GetShopDataList(int level)
        {
            if (level >= 0 && level < 4)
            {
                return m_ShopDataList[level];
            }
            else
            {
                return new List<ShopData_Str>();
            }
        }

        public static List<ShopDef_Str> GetShopDefList()
        {
            return m_ShopDefList;
        }

        public static bool LoadShopDataList(int level)
        {
            string file = "";
            if (level == 0)
            {
                file = "data\\SHOPDATA.TXT";
            }
            else if (level == 1)
            {
                file = "data\\SHOPDATA2.TXT";
            }
            else if (level == 2)
            {
                file = "data\\SHOPDATA3.TXT";
            }
            else if (level == 3)
            {
                file = "data\\SHOPDATA4.TXT";
            }else
            {
                return false;
            }
            //文件存在
            if (!File.Exists(file))
            {
                return false;
            }

            //读取
            m_ShopDataList[level].Clear();

            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(fs, Encoding.GetEncoding(950));
            reader.DiscardBufferedData();
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            reader.BaseStream.Position = 0;

            ShopData_Str shopData;
            shopData.code = "";
            shopData.ShopName = "";
            shopData.more = new List<string>();
            shopData.ShopType = "";
            shopData.list = new List<string>();

            bool get = false;
            string strLine = "";
            strLine = reader.ReadLine();
            while (strLine != null)
            {
                strLine = strLine.Split('/')[0];

                if (strLine.Contains("[shop]"))
                {
                    if (get)
                    {
                        //add army
                        m_ShopDataList[level].Add(shopData);
                    }
                    get = true;
                    shopData.code = "";
                    shopData.ShopName = "";
                    shopData.more = new List<string>();
                    shopData.ShopType = "";
                    shopData.list = new List<string>();
                }
                else if (strLine.Contains("code"))
                {
                    shopData.code = CFormat.PureString(strLine.Split('=')[1]);
                }
                else if (strLine.Contains("ShopName"))
                {
                    shopData.ShopName = CFormat.PureString(strLine.Split('=')[1]);
                }
                else if (strLine.Contains("ShopType"))
                {
                    shopData.ShopType = CFormat.PureString(strLine.Split('=')[1]);
                }
                else if (strLine.Contains("Item"))
                {
                    var lst = CFormat.PureString(strLine.Split('=')[1]).Split(',');
                    foreach (var item in lst)
                    {
                        if (item != "")
                        {
                            shopData.list.Add(item.ToLower());
                        }
                    }
                }
                else if (strLine.Contains("="))
                {
                    shopData.more.Add(strLine.ToLower());
                }

                strLine = null;
                strLine = reader.ReadLine();
            }

            //末了 写一次
            m_ShopDataList[level].Add(shopData);

            reader.Close();
            fs.Close();

            return true;
        }
        public static bool LoadShopDataList()
        {
            for (int i = 0; i < 4; i++ )
            {
                m_ShopDataList[i] = new List<ShopData_Str>();
                LoadShopDataList(i);
            }

            LoadShopNameList();

            return true;
        }

        public static bool ShopDataExit(string shop_name,int level)
        {
            if (level >= 0 && level<4)
            {
                foreach (var shop in m_ShopDataList[level])
                {
                    if (CFormat.PureString(shop_name) == shop.ShopName)
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }

        public static bool LoadShopDefList()
        {
            //文件存在
            if (!File.Exists("data\\SHOP.H"))
            {
                return false;
            }

            //读取
            m_ShopDefList.Clear();

            FileStream fs = new FileStream("data\\SHOP.H", FileMode.Open, FileAccess.Read);
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
                    ShopDef_Str shopDef;
                    string tmp = strLine.Split(new string[] { "shop_" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    string name = tmp.Split(' ')[0].Split('\t')[0];
                    string id = tmp.Replace(" ", "").Replace("\\t", "").Replace(name, "");

                    shopDef.id = CFormat.PureString(id).ToLower();
                    shopDef.name = CFormat.PureString(name).ToLower();
                    m_ShopDefList.Add(shopDef);
                }
                strLine = null;
                strLine = reader.ReadLine();
            }

            reader.Close();
            fs.Close();

            return true;
        }

        public static bool LoadShopNameList()
        {
            //文件存在
            if (!File.Exists("data\\SHOP_NAME.TXT"))
            {
                return false;
            }

            //读取
            m_ShopNameList.Clear();

            FileStream fs = new FileStream("data\\SHOP_NAME.TXT", FileMode.Open, FileAccess.Read);
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
                    ShopName_Str shopName;
                    string tmp = strLine.Split(new string[] { "item = " }, StringSplitOptions.RemoveEmptyEntries)[0];
                    string id = tmp.Split(',')[0];
                    string name = tmp.Split(',')[1];

                    shopName.id = CFormat.PureString(id).ToLower();
                    shopName.name = CFormat.PureString(name).ToLower();
                    m_ShopNameList.Add(shopName);
                }
                strLine = null;
                strLine = reader.ReadLine();
            }

            reader.Close();
            fs.Close();

            return true;
        }

        public static bool SaveShopDataInfo()
        {
            string file = "";

            for (int level = 0; level < 4;level++ )
            {
                if (level == 0)
                {
                    file = "SHOPDATA.TXT";
                }
                else if (level == 1)
                {
                    file = "SHOPDATA2.TXT";
                }
                else if (level == 2)
                {
                    file = "SHOPDATA3.TXT";
                }
                else if (level == 3)
                {
                    file = "SHOPDATA4.TXT";
                }
                else
                {
                    return false;
                }
                //文件存在
                if (!File.Exists("data\\" + file))
                {
                    return false;
                }

                //备份原文件
                string time = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
                File.Copy(@"data\\" + file, @"backup\\" + time + file);

                //删除原文件
                File.Delete(@"data\\" + file);

                //写文件
                FileStream rtfs = new FileStream(@"data\\" + file, FileMode.Create, FileAccess.Write);
                StreamWriter rt = new StreamWriter(rtfs, Encoding.GetEncoding(950));
                rt.BaseStream.Seek(0, SeekOrigin.Begin);
                rt.BaseStream.Position = 0;

                rt.WriteLine("#include TYPE.H");
                rt.WriteLine("#include DEFINETXT.H");
                rt.WriteLine("#include ITEM.H");
                rt.WriteLine("");
                rt.WriteLine("");
                rt.WriteLine("[ini]");
                rt.WriteLine("shop_max = 2000");
                rt.WriteLine("");

                foreach (var shop in m_ShopDataList[level])
                {
                    /*public string code;
                    public string ShopName;
                    public List<string> more;
                    public string ShopType;
                    public List<string> list;*/
                    rt.WriteLine("");
                    rt.WriteLine("[shop]");
                    rt.WriteLine("code = " + shop.code);
                    rt.WriteLine("ShopName = " + shop.ShopName);
                    foreach (var it in shop.more)
                    {
                        rt.WriteLine(it);
                    }
                    rt.WriteLine("ShopType = " + shop.ShopType);

                    string items = "Item = ";
                    for (int it = 0; it < shop.list.Count; it++)
                    {
                        items += shop.list[it];
                        if (it < shop.list.Count - 1)
                        {
                            items += ",";
                        }
                    }
                    rt.WriteLine(items);
                }
                rt.Close();
                rtfs.Close();

                if (File.Exists(@"new\\" + file))
                {
                    File.Delete(@"new\\" + file);
                }
                File.Copy(@"data\\" + file, @"new\\" + file);
            }
            
            return true;
        }

        private static string GetShopNameString(string shopName)
        {
            foreach (var item in m_ShopNameList)
            {
                if (CFormat.ToSimplified(item.id) == shopName)
                {
                    return item.name;
                }
            }
            return "";
        }

        private static string GetShopNameId(string shopName)
        {
            foreach (var item in m_ShopNameList)
            {
                if (CFormat.ToSimplified(item.name) == CFormat.ToSimplified(shopName))
                {
                    return item.id;
                }
            }
            return "";
        }

        private static string GetShopNameById(string id)
        {
            foreach (var item in m_ShopDataList[0])
            {
                if (CFormat.ToSimplified(item.code) == id)
                {
                    return item.ShopName;
                }
            }
            return "";
        }

        public static bool GetAttrByName(string shopName, int level,out ShopData_Str shopData)
        {
            List<ShopData_Str> shopDataList = new List<ShopData_Str>();
            if (level >= 0 && level< 4 )
            {
                shopDataList = m_ShopDataList[level];
            }
            else
            {
                shopData = new ShopData_Str();
                shopData.list = new List<string>();
                return false;
            }

            string shopNameId = GetShopNameId(shopName);
            if (shopNameId == "")
            {
                shopData = new ShopData_Str();
                shopData.list = new List<string>();

                return false;
            }
            
            foreach (var item in shopDataList)
            {
                if (CFormat.ToSimplified(item.ShopName) == shopNameId)
                {
                    shopData = item;
                    return true;
                }
            }

            shopData = new ShopData_Str();
            shopData.list = new List<string>();

            return false;
        }

        public static string GetNameById(string id)
        {
            string name = "";
            foreach (var item in m_ShopDefList)
            {
                if (CFormat.ToSimplified(item.id) == id)
                {
                    name = item.name;
                    break;
                }
            }

            return name;
        }

        public static bool AddShopItem(string id, int index, int level,string item)
        {
            List<ShopData_Str> shopDataList = new List<ShopData_Str>();
            if (level >= 0 && level < 4)
            {
                shopDataList = m_ShopDataList[level];
            }
            else
            {
                return false;
            }

            foreach (var it in shopDataList)
            {
                if (CFormat.ToSimplified(it.code) == id)
                {
                    //it.list.Add(item);
                    it.list.Insert(index, item);
                    return true;
                }
            }

            m_ShopDataList[level] = shopDataList;

            return false;
        }

        public static bool DelShopItem(string id, int level, string item)
        {
            List<ShopData_Str> shopDataList = new List<ShopData_Str>();
            if (level >= 0 && level < 4)
            {
                shopDataList = m_ShopDataList[level];
            }
            else
            {
                return false;
            }

            foreach (var it in shopDataList)
            {
                if (CFormat.ToSimplified(it.code).ToLower() == id)
                {
                    it.list.Remove(item);
                    m_ShopDataList[level] = shopDataList;
                    return true;
                }
            }

            return false;
        }

        public static bool ItemSendFwd(string code, int level, string item)
        {
            List<ShopData_Str> shopDataList = new List<ShopData_Str>();
            if (level >= 0 && level < 4)
            {
                shopDataList = m_ShopDataList[level];
            }
            else
            {
                return false;
            }

            foreach (var shop in shopDataList)
            {
                if (CFormat.PureString(shop.code) == CFormat.PureString(code))
                {
                    for (int i = 0; i < shop.list.Count; i++ )
                    {
                        if (CFormat.ToSimplified(shop.list[i]) == item && i > 0)
                        {
                            string tmp;
                            tmp = shop.list[i-1];
                            shop.list[i-1] = shop.list[i];
                            shop.list[i] = tmp;

                            return true;
                        }
                    }
                }
            }
            m_ShopDataList[level] = shopDataList;

            return false;
        }

        public static bool ItemSendNext(string code, int level, string item)
        {
            List<ShopData_Str> shopDataList = new List<ShopData_Str>();
            if (level >= 0 && level < 4)
            {
                shopDataList = m_ShopDataList[level];
            }
            else
            {
                return false;
            }
            foreach (var shop in shopDataList)
            {
                if (CFormat.PureString(shop.code) == CFormat.PureString(code))
                {
                    for (int i = 0; i < shop.list.Count; i++)
                    {
                        if (CFormat.ToSimplified(shop.list[i]) == item && i < shop.list.Count - 1)
                        {
                            string tmp;
                            tmp = shop.list[i + 1];
                            shop.list[i + 1] = shop.list[i];
                            shop.list[i] = tmp;

                            return true;
                        }
                    }
                }
            }
            m_ShopDataList[level] = shopDataList;

            return false;
        }
    }
}
