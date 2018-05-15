using MainServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace 注册网关
{
    public struct StageDef_Str
    {
        public string id;
        public string name;
    }
    class StageCtrl
    {
        private static List<StageDef_Str> m_StageDefList = new List<StageDef_Str>();

        public static List<StageDef_Str> GetStageDefList()
        {
            return m_StageDefList;
        }

        public static bool LoadStageDefInfo()
        {
            //文件存在
            if (!File.Exists("profile\\STAGE.H"))
            {
                return false;
            }

            //读取
            m_StageDefList.Clear();

            FileStream fs = new FileStream("profile\\STAGE.H", FileMode.Open, FileAccess.Read);
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
                    StageDef_Str stageDef;
                    string tmp = strLine.Split(new string[] { "city_" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    string name = tmp.Split(' ')[0].Split('\t')[0];
                    string id = tmp.Replace(" ", "").Replace("\\t", "").Replace(name, "");
                    id = CFormat.PureString(id);

                    //去掉前面的0
                    id = CFormat.RemoveXZeroStr(id);

                    stageDef.id = CFormat.PureString(id);
                    stageDef.name = CFormat.PureString(name);
                    m_StageDefList.Add(stageDef);
                }
                strLine = null;
                strLine = reader.ReadLine();
            }

            reader.Close();
            fs.Close();

            return true;
        }

        public static string GetNameById(string id)
        {
            string name = "";
            foreach (var item in m_StageDefList)
            {
                if (CFormat.ToSimplified(item.id) == id || CFormat.ToSimplified(item.id) == "0" + id
                    || "0" + CFormat.ToSimplified(item.id) == id)
                {
                    name = item.name;
                    break;
                }
            }
            return name;
        }

        public static string GetIdByName(string name)
        {
            string id = "";
            foreach (var item in m_StageDefList)
            {
                if (CFormat.ToSimplified(item.name) == name )
                {
                    id = item.id;
                    break;
                }
            }
            return id;
        }
    }
}
