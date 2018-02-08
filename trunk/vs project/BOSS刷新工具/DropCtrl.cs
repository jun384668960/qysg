using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BOSS刷新工具
{
    public struct Drop_Single_Str
    {
        public string name;
        public string num;
    };  
    public struct Drop_Str
    {
        public string id;
        public List<Drop_Single_Str> list;
    };

    class DropCtrl
    {
        private static List<Drop_Str> m_DropList = new List<Drop_Str>();

        public static List<Drop_Str> GetDropList()
        {
            return m_DropList;
        }

        public static bool LoadDropListInfo()
        {
            //文件存在
            if (!File.Exists("data\\DROPITEM.TXT"))
            {
                return false;
            }

            //读取
            m_DropList.Clear();

            FileStream fs = new FileStream("data\\DROPITEM.TXT", FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(fs, Encoding.GetEncoding(950));
            reader.DiscardBufferedData();
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            reader.BaseStream.Position = 0;

            string strLine = "";
            strLine = reader.ReadLine();
            while (strLine != null)
            {
                strLine = strLine.Split('/')[0];
                if (strLine.Contains("drop = ") && strLine.Substring(0, CFormat.StringLength("drop = ")) == "drop = ")
                {
                    Drop_Str drop;
                    drop.list = new List<Drop_Single_Str>();

                    string tmp = strLine.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    var dropList = CFormat.PureString(tmp).Split(',');
                    string id = dropList[0];
                    drop.id = id;

                    string name = "";
                    string num = "";
                    for (int i = 1; i < dropList.Length; i++ )
                    {
                        if (dropList[i] != "" && dropList[i] != null)
                        {
                            if (i % 2 == 1)
                            {
                                //name
                                name = dropList[i];
                            }
                            else 
                            {
                                //num
                                num = dropList[i];
                                Drop_Single_Str single = new Drop_Single_Str();
                                single.name = name;
                                single.num = num;

                                drop.list.Add(single);
                            }
                        }
                    }
                    m_DropList.Add(drop);
                }
                strLine = null;
                strLine = reader.ReadLine();
            }

            reader.Close();
            fs.Close();

            return true;
        }

        public static bool SaveDropInfo()
        {
            string file = "data\\DROPITEM.TXT";
            if (!File.Exists(file))
            {
                return false;
            }


            //备份原文件
            string time = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            File.Copy(file, @"backup\\" + time + "DROPITEM.TXT");

            //删除原文件
            File.Delete(file);

            //写文件
            FileStream rtfs = new FileStream(file, FileMode.Create, FileAccess.Write);
            StreamWriter rt = new StreamWriter(rtfs, Encoding.GetEncoding(950));
            rt.BaseStream.Seek(0, SeekOrigin.Begin);
            rt.BaseStream.Position = 0;

            rt.WriteLine("#include TYPE.H");
            rt.WriteLine("#include ITEM.H");
            rt.WriteLine("");
            rt.WriteLine("");
            rt.WriteLine("[ini]");
            rt.WriteLine("max = 10000");
            rt.WriteLine("");
            rt.WriteLine("[drop]");
            foreach (var drop in m_DropList)
            {
                string dropLine = "drop = ";
                dropLine += drop.id;
                if (drop.list.Count <= 0)
                {
                    dropLine += ",";
                }
                else
                {
                    foreach (var item in drop.list)
                    {
                        dropLine += "," + item.name + ",";
                        dropLine += item.num;
                    }
                }
                
                rt.WriteLine(dropLine);
            }
            rt.Close();
            rtfs.Close();

            if (File.Exists(@"new\\DROPITEM.TXT"))
            {
                File.Delete(@"new\\DROPITEM.TXT");
            }
            File.Copy(file, @"new\\DROPITEM.TXT");

            return true;
        }

        public static bool GetDropAttrById(string id, out Drop_Str drop)
        {
            foreach (var item in m_DropList)
            {
                if (CFormat.ToSimplified(item.id) == id)
                {
                    drop = item;
                    return true;
                }
            }
            drop = new Drop_Str();
            drop.list = new List<Drop_Single_Str>();
            drop.id = id;
            return false;
        }

        public static bool MdfyDropNum(string id, string name, string oldNum, string newNum)
        {
            for (int i = 0; i < m_DropList.Count; i++)
            {
                if (CFormat.ToSimplified(m_DropList[i].id) == id)
                {
                    for (int j = 0; j < m_DropList[i].list.Count; j++ )
                    {
                        if (String.Compare(m_DropList[i].list[j].name, name,true) == 0
                            && m_DropList[i].list[j].num == oldNum)
                        {
                            Drop_Single_Str single = new Drop_Single_Str();
                            single.name = name;
                            single.num = newNum;

                            m_DropList[i].list[j] = single;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static bool AddSingleDrop(string id, string name, string num)
        {
            Drop_Single_Str single = new Drop_Single_Str();
            single.name = name;
            single.num = num;
            for (int i = 0; i < m_DropList.Count; i++)
            {
                if (CFormat.ToSimplified(m_DropList[i].id) == id)
                {
                    m_DropList[i].list.Add(single);
                    return true;
                }
            }
            try
            {
                //找不到该id
                Drop_Str drop = new Drop_Str();
                drop.list = new List<Drop_Single_Str>();
                drop.id = id;
                drop.list.Add(single);
                m_DropList.Add(drop);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool DelSingleDrop(string id, string name, string num)
        {
            for (int i = 0; i < m_DropList.Count; i++)
            {
                if (CFormat.ToSimplified(m_DropList[i].id) == id)
                {
                    Drop_Single_Str single = new Drop_Single_Str();
                    single.name = name;
                    single.num = num;

                    m_DropList[i].list.Remove(single);
                    return true;
                }
            }

            return false;
        }

        public static bool ClearSingleDrops(string id)
        {
            for (int i = 0; i < m_DropList.Count; i++)
            {
                if (CFormat.ToSimplified(m_DropList[i].id) == id)
                {
                    m_DropList[i].list.Clear();
                    return true;
                }
            }

            return false;
        }
    }
}
