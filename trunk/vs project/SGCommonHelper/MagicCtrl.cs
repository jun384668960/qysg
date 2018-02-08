using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SGCommonHelper
{
    public struct MagicDef_Str
    {
        public string id;
        public string name;
    }
    public class MagicCtrl
    {
        private string m_Forder = "";

        private List<MagicDef_Str> m_MagicDef1List = new List<MagicDef_Str>();
        private List<MagicDef_Str> m_MagicDef2List = new List<MagicDef_Str>();

        public bool SetForder(string forder)
        {
            m_Forder = forder;

            return true;
        }

        public bool LoadItemDefList()
        {
            //文件存在
            if (!File.Exists(m_Forder + "\\Magic.H"))
            {
                return false;
            }

            //读取
            m_MagicDef1List.Clear();

            FileStream fs = new FileStream(m_Forder + "\\Magic.H", FileMode.Open, FileAccess.Read);
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
                    MagicDef_Str magicDef;
                    string tmp = strLine.Split(new string[] { "magic_" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    string name = tmp.Split(' ')[0].Split('\t')[0];
                    string id = tmp.Replace(" ", "").Replace("\\t", "").Replace(name, "");

                    magicDef.id = CFormat.PureString(id);
                    magicDef.name = CFormat.PureString(name);
                    m_MagicDef1List.Add(magicDef);
                }
                strLine = null;
                strLine = reader.ReadLine();
            }

            reader.Close();
            fs.Close();

            //文件存在
            if (!File.Exists(m_Forder + "\\Magic_exp.H"))
            {
                return false;
            }

            //读取
            m_MagicDef2List.Clear();

            fs = new FileStream(m_Forder + "\\Magic_exp.H", FileMode.Open, FileAccess.Read);
            reader = new StreamReader(fs, Encoding.GetEncoding(950));
            reader.DiscardBufferedData();
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            reader.BaseStream.Position = 0;

            strLine = "";
            strLine = reader.ReadLine();
            while (strLine != null)
            {
                strLine = strLine.Split('/')[0];
                if (strLine.Contains("#define"))
                {
                    MagicDef_Str magicDef;
                    string tmp = strLine.Split(new string[] { "magic_" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    string name = tmp.Split(' ')[0].Split('\t')[0];
                    string id = tmp.Replace(" ", "").Replace("\\t", "").Replace(name, "");

                    magicDef.id = CFormat.PureString(id);
                    magicDef.name = CFormat.PureString(name);
                    m_MagicDef2List.Add(magicDef);
                }
                strLine = null;
                strLine = reader.ReadLine();
            }

            reader.Close();
            fs.Close();

            return true;
        }

        public bool SaveMagicDef(int index)
        {
            string fileName = "";
            List<MagicDef_Str> m_MagicDefList = new List<MagicDef_Str>();
            if(index == 1)
            {
                fileName = "Magic.H";
                m_MagicDefList = m_MagicDef1List;
            }
            else if (index == 2)
            {
                fileName = "Magic_exp.H";
                m_MagicDefList = m_MagicDef2List;
            }
            else 
            {
                return false;
            }

            string file = m_Forder + "\\" + fileName;
            if (!File.Exists(file))
            {
                return false;
            }


            //备份原文件
            string time = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            File.Copy(file, @"backup\\" + time + fileName);

            //删除原文件
            File.Delete(file);

            //写文件
            FileStream rtfs = new FileStream(file, FileMode.Create, FileAccess.Write);
            StreamWriter rt = new StreamWriter(rtfs, Encoding.GetEncoding(950));
            rt.BaseStream.Seek(0, SeekOrigin.Begin);
            rt.BaseStream.Position = 0;

            //List<ItemDef_Str> sortedStudents = m_ItemDefList.Sort(s => s.id).ToList();
            MagicDef_Str[] SortedArray = m_MagicDefList.ToArray();
            Array.Sort(SortedArray
                , (Comparison<MagicDef_Str>)
                delegate(MagicDef_Str a, MagicDef_Str b)
                {
                    return int.Parse(a.id) > int.Parse(b.id) ? 1 : int.Parse(a.id) == int.Parse(b.id) ? 0 : -1;
                });

            m_MagicDefList = new List<MagicDef_Str>(SortedArray);
            foreach (var item in m_MagicDefList)
            {
                /*#define item_直劍				1*/
                string line = "#define magic_";
                line += item.name;
                line += "\t\t\t";
                line += item.id;

                rt.WriteLine(line);
            }
            rt.Close();
            rtfs.Close();

            if (File.Exists(@"new\\" + fileName))
            {
                File.Delete(@"new\\" + fileName);
            }
            File.Copy(file, @"new\\" + fileName);

            return true;
        }
        public bool SaveMagicDefInfo()
        {
            bool ret = SaveMagicDef(1);
            if (!ret)
            {
                return false;
            }
            ret = SaveMagicDef(2);
            if (!ret)
            {
                return false;
            }
            return true;
        }
    }
}
