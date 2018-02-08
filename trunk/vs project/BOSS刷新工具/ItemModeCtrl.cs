using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BOSS刷新工具
{
    public struct Item_Mode_Str
    {
        public string code;
        public string type;
        public string item_id;
        public string item_number;
        public string cost;
        public string item_attr;
    };
    class ItemModeCtrl
    {
        private static List<Item_Mode_Str> m_ItemModeList = new List<Item_Mode_Str>();

        public static List<Item_Mode_Str> GetItemModeList()
        {
            return m_ItemModeList;
        }

        public static bool LoadItemModeInfo()
        {
            //文件存在
            if (!File.Exists("data\\ITEMMODESHOP.TXT"))
            {
                return false;
            }

            //读取
            m_ItemModeList.Clear();

            FileStream fs = new FileStream("data\\ITEMMODESHOP.TXT", FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(fs, Encoding.GetEncoding(950));
            reader.DiscardBufferedData();
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            reader.BaseStream.Position = 0;

            Item_Mode_Str mode = new Item_Mode_Str();
            mode.code = "";
            mode.type = "";
            mode.item_id = "";
            mode.item_number = "";
            mode.cost = "";
            mode.item_attr = "";

            bool get = false;
            string strLine = "";
            strLine = reader.ReadLine();
            while (strLine != null)
            {
                strLine = strLine.Split('/')[0];

                if (strLine.Contains("[item_shop]") && strLine.Substring(0, CFormat.StringLength("[item_shop]")) == "[item_shop]")
                {
                    if (get)
                    {
                        //add army
                        m_ItemModeList.Add(mode);
                    }
                    get = true;
                    mode = new Item_Mode_Str();
                    mode.code = "";
                    mode.type = "";
                    mode.item_id = "";
                    mode.item_number = "";
                    mode.cost = "";
                    mode.item_attr = "";
                }
                else if (strLine.Contains("code = ") && strLine.Substring(0, CFormat.StringLength("code = ")) == "code = ")
                {
                    mode.code = CFormat.PureString(strLine.Split('=')[1]);
                }
                else if (strLine.Contains("type = ") && strLine.Substring(0, CFormat.StringLength("type = ")) == "type = ")
                {
                    mode.type = CFormat.PureString(strLine.Split('=')[1]);
                }
                else if (strLine.Contains("item_id = ") && strLine.Substring(0, CFormat.StringLength("item_id = ")) == "item_id = ")
                {
                    mode.item_id = CFormat.PureString(strLine.Split('=')[1]);
                }
                else if (strLine.Contains("item_number = ") && strLine.Substring(0, CFormat.StringLength("item_number = ")) == "item_number = ")
                {
                    mode.item_number = CFormat.PureString(strLine.Split('=')[1]);
                }
                else if (strLine.Contains("cost = ") && strLine.Substring(0, CFormat.StringLength("cost = ")) == "cost = ")
                {
                    mode.cost = CFormat.PureString(strLine.Split('=')[1]);
                }
                else if (strLine.Contains("item_attr = ") && strLine.Substring(0, CFormat.StringLength("item_attr = ")) == "item_attr = ")
                {
                    mode.item_attr = CFormat.PureString(strLine.Split('=')[1]);
                }

                strLine = null;
                strLine = reader.ReadLine();
            }

            //末了 写一次
            m_ItemModeList.Add(mode);

            ReSort();

            reader.Close();
            fs.Close();

            return true;
        }

        public static bool SaveItemModeInfo()
        {
            string file = "data\\ITEMMODESHOP.TXT";
            if (!File.Exists(file))
            {
                return false;
            }


            //备份原文件
            string time = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            File.Copy(file, @"backup\\" + time + "ITEMMODESHOP.TXT");

            //删除原文件
            File.Delete(file);

            //写文件
            FileStream rtfs = new FileStream(file, FileMode.Create, FileAccess.Write);
            StreamWriter rt = new StreamWriter(rtfs, Encoding.GetEncoding(950));
            rt.BaseStream.Seek(0, SeekOrigin.Begin);
            rt.BaseStream.Position = 0;

            rt.WriteLine("#include TYPE.H");
            rt.WriteLine("#include DEFINETXT.H");
            rt.WriteLine("#include ITEM.H");
            rt.WriteLine("");
            rt.WriteLine("");
            rt.WriteLine("[ini]");
            rt.WriteLine("shop_max = 1000");
            rt.WriteLine("");

            List<Item_Mode_Str> m_ItemModeList1 = new List<Item_Mode_Str>();
            List<Item_Mode_Str> m_ItemModeList2 = new List<Item_Mode_Str>();
            List<Item_Mode_Str> m_ItemModeList3 = new List<Item_Mode_Str>();
            List<Item_Mode_Str> m_ItemModeList4 = new List<Item_Mode_Str>();
            foreach (var mode in m_ItemModeList)
            {
                if (mode.type == "ishopType_General")
                {
                    m_ItemModeList1.Add(mode);
                }
                else if (mode.type == "ishopType_Special")
                {
                    m_ItemModeList2.Add(mode);
                }
                else if (mode.type == "ishopType_Funcion")
                {
                    m_ItemModeList3.Add(mode);
                }
                else if (mode.type == "ishopType_Cheap")
                {
                    m_ItemModeList4.Add(mode);
                }
            }
            /*
            [item_shop]
            code = 313
            type = ishopType_Funcion
            item_id = item_流星鐵
            item_number = 10
            cost = 2000
            item_attr = ITEMMALL_ATTR_SUGGEST
             */
            int code;
            code = 100;
            foreach (var mode in m_ItemModeList1)
            {
                code++;
                rt.WriteLine("");
                rt.WriteLine("[item_shop]");
                rt.WriteLine("code = " + code);
                rt.WriteLine("type = ishopType_General");
                rt.WriteLine("item_id = " + mode.item_id);
                rt.WriteLine("item_number = " + mode.item_number);
                rt.WriteLine("cost = " + mode.cost);
                rt.WriteLine("item_attr = " + mode.item_attr);
            }

            code = 200;
            foreach (var mode in m_ItemModeList2)
            {
                code++;
                rt.WriteLine("");
                rt.WriteLine("[item_shop]");
                rt.WriteLine("code = " + code);
                rt.WriteLine("type = ishopType_Special");
                rt.WriteLine("item_id = " + mode.item_id);
                rt.WriteLine("item_number = " + mode.item_number);
                rt.WriteLine("cost = " + mode.cost);
                rt.WriteLine("item_attr = " + mode.item_attr);
            }

            code = 300;
            foreach (var mode in m_ItemModeList3)
            {
                code++;
                rt.WriteLine("");
                rt.WriteLine("[item_shop]");
                rt.WriteLine("code = " + code);
                rt.WriteLine("type = ishopType_Funcion");
                rt.WriteLine("item_id = " + mode.item_id);
                rt.WriteLine("item_number = " + mode.item_number);
                rt.WriteLine("cost = " + mode.cost);
                rt.WriteLine("item_attr = " + mode.item_attr);
            }

            code = 400;
            foreach (var mode in m_ItemModeList4)
            {
                code++;
                rt.WriteLine("");
                rt.WriteLine("[item_shop]");
                rt.WriteLine("code = " + code);
                rt.WriteLine("type = ishopType_Cheap");
                rt.WriteLine("item_id = " + mode.item_id);
                rt.WriteLine("item_number = " + mode.item_number);
                rt.WriteLine("cost = " + mode.cost);
                rt.WriteLine("item_attr = " + mode.item_attr);
            }

            rt.Close();
            rtfs.Close();

            if (File.Exists(@"new\\ITEMMODESHOP.TXT"))
            {
                File.Delete(@"new\\ITEMMODESHOP.TXT");
            }
            File.Copy(file, @"new\\ITEMMODESHOP.TXT");

            return true;
        }
        private static Int32 max_code1 = 0;
        private static Int32 max_code2 = 0;
        private static Int32 max_code3 = 0;
        private static Int32 max_code4 = 0;
        private static bool ReSort()
        {
            List<Item_Mode_Str> m_ItemModeListNew = new List<Item_Mode_Str>();

            Item_Mode_Str newMode = new Item_Mode_Str();
            int code1 = 100;
            int code2 = 200;
            int code3 = 300;
            int code4 = 400;
            foreach (var mode in m_ItemModeList)
            {
                if (CFormat.PureString(mode.item_id) == "")
                {
                    continue;
                }
                newMode = mode;
                if (newMode.type == "ishopType_General")
                {
                    code1++;
                    newMode.code = code1.ToString();
                }
                else if (newMode.type == "ishopType_Special")
                {
                    code2++;
                    newMode.code = code2.ToString();
                }
                else if (newMode.type == "ishopType_Funcion")
                {
                    code3++;
                    newMode.code = code3.ToString();
                }
                else if (newMode.type == "ishopType_Cheap")
                {
                    code4++;
                    newMode.code = code4.ToString();
                }
                m_ItemModeListNew.Add(newMode);
            }
            m_ItemModeList.Clear();
            m_ItemModeList = m_ItemModeListNew;

            max_code1 = code1;
            max_code2 = code2;
            max_code3 = code3;
            max_code4 = code4;

            return true;
        }

        public static bool GetItemModeInfo(string code, out Item_Mode_Str newMode)
        {
            foreach (var mode in m_ItemModeList)
            {
                if (CFormat.ToSimplified(mode.code) == code)
                {
                    newMode = mode;
                    return true;
                }
            }
            newMode = new Item_Mode_Str();

            return false;
        }

        public static bool MdfyItemModeInfo(string code, Item_Mode_Str newMode)
        {
            for (int i = 0; i < m_ItemModeList.Count; i++ )
            {
                if (CFormat.ToSimplified(m_ItemModeList[i].code) == code)
                {
                    m_ItemModeList[i] = newMode;
                    ReSort();
                    return true;
                }
            }

            return false;
        }

        public static bool DelItemModeInfo(string code)
        {
            foreach (var mode in m_ItemModeList)
            {
                if (CFormat.PureString(mode.code) == CFormat.PureString(code))
                {
                    m_ItemModeList.Remove(mode);
                    ReSort();
                    return true;
                }
            }
            return false;
        }

        public static bool AddHeadItemModeInfo(string code, Item_Mode_Str newMode)
        {
            foreach (var mode in m_ItemModeList)
            {
                if (CFormat.PureString(mode.code) == CFormat.PureString(code))
                {
                    int index = m_ItemModeList.IndexOf(mode);
                    if(index>0)
                    {
                        m_ItemModeList.Insert(index-1, newMode);
                        ReSort();
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool AddTailItemModeInfo(string code, Item_Mode_Str newMode)
        {
            foreach (var mode in m_ItemModeList)
            {
                if (CFormat.PureString(mode.code) == CFormat.PureString(code))
                {
                    int index = m_ItemModeList.IndexOf(mode);
                    m_ItemModeList.Insert(index, newMode);
                    ReSort();
                    return true;
                }
            }
            return false;
        }

        public static bool ModeSendFwd(string code)
        {
            Int32 _code = Int32.Parse(code);
            Int32 codeType = _code / 100;
            //当前类型是否最前
            if (_code - 1 <= codeType*100)
            {
                return false;//到顶，不操作
            }

            //
            Item_Mode_Str tmp;
            for (int i = 0; i < m_ItemModeList.Count; i++)
            {
                if (CFormat.ToSimplified(m_ItemModeList[i].code) == code)
                {
                    tmp = m_ItemModeList[i - 1];
                    m_ItemModeList[i - 1] = m_ItemModeList[i];
                    m_ItemModeList[i] = tmp;
                    ReSort();
                    return true;
                }
            }

            return false;
        }

        public static bool ModeSendNext(string code)
        {
            Int32 _code = Int32.Parse(code);
            Int32 codeType = _code / 100;
            //当前类型是否最后
            int max_code = 0;
            if (codeType == 1)
            {
                max_code = max_code1;
            }
            else if (codeType == 2)
            {
                max_code = max_code2;
            }
            else if (codeType == 3)
            {
                max_code = max_code3;
            }
            else if (codeType == 4)
            {
                max_code = max_code4;
            }
            else
            {
                return false;
            }
            if (_code >= max_code)
            {
                return false;
            }

            //
            Item_Mode_Str tmp;
            for (int i = 0; i < m_ItemModeList.Count; i++)
            {
                if (CFormat.ToSimplified(m_ItemModeList[i].code) == code)
                {
                    tmp = m_ItemModeList[i + 1];
                    m_ItemModeList[i + 1] = m_ItemModeList[i];
                    m_ItemModeList[i] = tmp;
                    ReSort();
                    return true;
                }
            }

            return false;
        }

        public static int GetModeTypeNum(int type)
        {
            string _type = "";
            if (type == 1)
            {
                _type = "ishopType_General";
            }
            else if (type == 2)
            {
                _type = "ishopType_Special";
            }
            else if (type == 3)
            {
                _type = "ishopType_Funcion";
            }
            else if (type == 4)
            {
                _type = "ishopType_Cheap";
            }
            int num = 0;
            foreach (var mode in m_ItemModeList)
            {
                if (CFormat.PureString(mode.type) == CFormat.PureString(_type) 
                    && CFormat.PureString(mode.item_id) != "")
                {
                    num++;
                }
            }
            return num;
        }
    }
}
