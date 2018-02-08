using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace register_server
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct AccAttrHead
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string head;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct AccAttr
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
        public string Account;  // 人物帐号
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string A1;
        public UInt32 nIndex;	// 序号
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        public string Name;		// 名称
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        public string Corps;		//军团
        public UInt32 Gold;		//金钱
        public UInt32 Exp;
        public UInt16 SkillExp;
        public UInt16 Anger;
        public UInt16 AngerNum;
        public UInt16 Level;
        public Byte job;
        public Byte sex;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
        public string A2;
        public UInt16 Attr_Num;  //属性点
        public UInt32 Honor;	//功勋值
        public UInt32 Hp;
        public UInt32 Mp;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 249)]
        public string A3;

        public UInt16 Attr_str_up;   // 属性上限
        public UInt16 Attr_int_up;
        public UInt16 Attr_con_up;
        public UInt16 Attr_dex_up;
        public UInt16 Attr_mind_up;
        public UInt16 Attr_leader_up;

        public UInt16 Attr_str; //武力
        public UInt16 Attr_int;//智力
        public UInt16 Attr_con;//精神	
        public UInt16 Attr_dex;//体魄
        public UInt16 Attr_mind; //反应	
        public UInt16 Attr_leader;//统御

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 979)]
        public string A4;

        public byte PackNum;//背包格数		
        public byte StoreNum; //仓库格数

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 169)]
        public string A5;

        public UInt16 Officer;	//官职

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 232)]
        public string A6;
    };

    public class CPlayerCtrl
    {
        private static List<AccAttr> m_playersAttrList = new List<AccAttr>();

        public static string GetAccByName(string name)
        {
            string acc = "";

            foreach (var item in m_playersAttrList)
            {
                string _name = CFormat.GameStrToSimpleCN(item.Name);
                if (_name == name)
                {
                    acc = item.Account;
                    break;
                }
            }

            return acc;
        }

        public static string GetNameByAcc(string acc)
        {
            string name = "";

            foreach (var item in m_playersAttrList)
            {
                if (acc == item.Account)
                {
                    name = CFormat.GameStrToSimpleCN(item.Name);
                    break;
                }
            }

            return name;
        }

        public static List<AccAttr> LoadPlayerInfos(string file, bool reload = false)
        {
            if (!reload && m_playersAttrList.Count != 0)
            {
                return m_playersAttrList;
            }

            List<AccAttr> AccAttrList = new List<AccAttr>();
            AccAttrList.Clear();

            string dbfile = file;
            if (!File.Exists(dbfile))
            {
                AccAttrList.Clear();
                return AccAttrList;
            }

            if (File.Exists(@".\\Temp\\players.dat"))
            {
                File.SetAttributes(@".\\Temp\\players.dat", FileAttributes.Normal);
                File.Copy(dbfile, @".\\Temp\\players.dat", true);
            }
            else
            {
                File.Copy(dbfile, @".\\Temp\\players.dat", false);
            }


            FileStream fs = new FileStream(".\\Temp\\players.dat", FileMode.Open, FileAccess.ReadWrite);
            AccAttrHead head = new AccAttrHead();

            int ret = 0;
            byte[] bytData = new byte[Marshal.SizeOf(head)];
            ret = fs.Read(bytData, 0, Marshal.SizeOf(head));
            while (ret > 0)
            {
                AccAttr aa = new AccAttr();
                byte[] bytAccAttrData = new byte[Marshal.SizeOf(aa)];
                ret = fs.Read(bytAccAttrData, 0, Marshal.SizeOf(aa));
                if (ret <= 0)
                {
                    break;
                }
                AccAttr acc_attr = CStructBytesFormat.BytesToStruct<AccAttr>(bytAccAttrData);

                AccAttrList.Add(acc_attr);
            }

            fs.Close();

            //更新本地list
            m_playersAttrList.Clear();
            m_playersAttrList = AccAttrList;

            return AccAttrList;
        }

        public static List<AccAttr> GetAttrList()
        {
            return m_playersAttrList;
        }

        public static void PlayersAttrListClear()
        {
            m_playersAttrList.Clear();
        }

        public static AccAttr GetAttrByName(string name)
        {
            Nullable<AccAttr> result = null;
            foreach (var player in m_playersAttrList)
            {
                if (CFormat.GameStrToSimpleCN(player.Name) == name)
                {
                    result = player;
                    break;
                }
            }

            return result.Value;
        }

        public static List<int> LoadAllWarloadIndex()
        {
            List<int> skills = new List<int>();

            foreach (var item in m_playersAttrList)
            {
                if (item.job == 0X01)
                {
                    int id = (int)item.nIndex;
                    skills.Add(id);
                }
            }

            return skills;
        }

        public static List<AccAttr> LoadAllWarloadIndexDesc()
        {
            List<AccAttr> skills = new List<AccAttr>();

            foreach (var item in m_playersAttrList)
            {
                if (item.job == 0X01)
                {
                    skills.Add(item);
                }
            }

            return skills;
        }

        public static List<int> LoadAllLeaderIndex()
        {
            List<int> skills = new List<int>();

            foreach (var item in m_playersAttrList)
            {
                if (item.job == 0X02)
                {
                    int id = (int)item.nIndex;
                    skills.Add(id);
                }
            }

            return skills;
        }
        public static List<AccAttr> LoadAllLeaderIndexDesc()
        {
            List<AccAttr> skills = new List<AccAttr>();

            foreach (var item in m_playersAttrList)
            {
                if (item.job == 0X02)
                {
                    skills.Add(item);
                }
            }

            return skills;
        }
        public static List<int> LoadAllAdvisorIndex()
        {
            List<int> skills = new List<int>();
            foreach (var item in m_playersAttrList)
            {
                if (item.job == 0X03)
                {
                    int id = (int)item.nIndex;
                    skills.Add(id);
                }
            }
            return skills;
        }
        public static List<AccAttr> LoadAllAdvisorIndexDesc()
        {
            List<AccAttr> skills = new List<AccAttr>();
            foreach (var item in m_playersAttrList)
            {
                if (item.job == 0X03)
                {
                    skills.Add(item);
                }
            }
            return skills;
        }
        public static List<int> LoadAllWizardIndex()
        {
            List<int> skills = new List<int>();
            foreach (var item in m_playersAttrList)
            {
                if (item.job == 0X04)
                {
                    int id = (int)item.nIndex;
                    skills.Add(id);
                }
            }
            return skills;
        }
        public static List<AccAttr> LoadAllWizardIndexDesc()
        {
            List<AccAttr> skills = new List<AccAttr>();
            foreach (var item in m_playersAttrList)
            {
                if (item.job == 0X04)
                {
                    skills.Add(item);
                }
            }
            return skills;
        }
        
    }
}
