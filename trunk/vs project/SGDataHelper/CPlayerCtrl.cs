using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace SGDataHelper
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct AccAttrHead
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] head;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct AccAttr  //1756
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] Account;
        public UInt32 nAccId;       //acc_id       -------   D4 03,表示数据库acc_id中id = 0x03d4即980的账户
        public UInt32 nIndex;	// 序号
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
        public byte[] Name;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
        public byte[] Corps;		//军团
        public UInt32 Gold;		//金钱
        public UInt32 Exp;
        public UInt16 SkillExp;
        public UInt16 Anger;
        public UInt16 AngerNum;
        public UInt16 Level;
        public Byte job;
        public Byte sex;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public byte[] A2;
        public UInt16 Attr_Num;  //属性点
        public UInt32 Honor;	//功勋值
        public UInt32 Hp;
        public UInt32 Mp;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 249)]
        public byte[] A3;

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
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 29)]
        public byte[] A4;                   //---- 401
        public SingleItemAttr UpShield;     //主盾
        public SingleItemAttr UpWeapen;     //主武器
        public SingleItemAttr Armor;        //铠甲
        public SingleItemAttr Head;         //头盔
        public SingleItemAttr Foot;         //鞋子
        public SingleItemAttr LeftOther;    //左边饰品
        public SingleItemAttr RightOther;   //右边饰品
        public SingleItemAttr Underwear;    //内衣
        public SingleItemAttr Arm;          //护腕
        public SingleItemAttr P;            //披风
        public SingleItemAttr Horse;        //马
        public SingleItemAttr DownShield;   //副盾
        public SingleItemAttr DownWeapen;   //副武器
        public UInt16 ArrowId;              //箭羽id
        public UInt16 ArrowNum;             //箭羽数量
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] A42;
        public byte PackNum;//背包格数		
        public byte StoreNum; //仓库格数
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 169)]
        public byte[] A5;

        public UInt16 Officer;	//官职
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
        public byte[] A6;
        public AccAttr_wawa_only Wa_Armor;  //装扮铠甲
        public AccAttr_wawa_only Wa_Head;   //装扮头
        public AccAttr_wawa_only Wa_Foot;   //装扮鞋
        public AccAttr_wawa_only Wa_P;      //装扮披风
        public AccAttr_wawa_only Wa_Horse;  //装扮马
        public AccAttr_wawa_only Wa_Other;  //装扮全身
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 138)]
        public byte[] A7;
    };
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct AccAttr_wawa_only
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] A7;
        public UInt16 id;
        public byte usedTimes;  //已经被使用的装备次数
        public byte inUseEx;    //00
    }

    public class CPlayerCtrl
    {
        private AccAttrHead m_AccAttrHead = new AccAttrHead();
        private List<AccAttr> m_playersAttrList = new List<AccAttr>();
        private UInt32 m_lastIndex = 0;
        private string m_Forder = "";
        public bool SetForder(string forder)
        {
            m_Forder = forder;

            return true;
        }

        public List<AccAttr> GetPlayersAttrList()
        {
            return m_playersAttrList;
        }

        //public string GameStrToSimpleCN(string src)
        public static string GameStrToSimpleCN(byte[] src)
        {
            //byte[] utf8bytes = System.Text.Encoding.Default.GetBytes(src);
            byte[] utf8bytes = src;
            string temp = System.Text.Encoding.GetEncoding(950).GetString(utf8bytes);
            temp = CFormat.ToSimplified(temp);

            return temp;
        }

        public static byte[] ReName(byte[] name)
        {
            byte[] newName = new byte[name.Length];
            newName[0] = 0X2A;
            for (int i = 0; i < name.Length - 1; i++ )
            {
                newName[i + 1] = name[i];
            }
            return newName;
        }

        public static byte[] GameStrToTradCN(string src)
        {
            //byte[] utf8bytes = System.Text.Encoding.Default.GetBytes(src);
            byte[] bytes;
            bytes = System.Text.Encoding.GetEncoding(950).GetBytes(src);

            return bytes;
        }

        public bool LoadPlayerInfos()
        {
            m_playersAttrList.Clear();

            if (!File.Exists(m_Forder + @"\\players.dat"))
            {
                return false;
            }


            FileStream fs = new FileStream(m_Forder + @"\\players.dat", FileMode.Open, FileAccess.ReadWrite);

            int ret = 0;
            AccAttrHead ts = new AccAttrHead();
            byte[] _bytData = new byte[Marshal.SizeOf(ts)];

            ret = fs.Read(_bytData, 0, _bytData.Length);
            if (ret > 0) //记录头
            {
                //m_AccAttrHead = CStructBytesFormat.BytesToStruct<AccAttrHead>(_bytData);
                m_AccAttrHead = CStructBytesFormat.rawDeserialize<AccAttrHead>(_bytData);
            }
            while (ret > 0)
            {
                AccAttr ts2 = new AccAttr();
                byte[] bytAccAttrData = new byte[Marshal.SizeOf(ts2)];

                ret = fs.Read(bytAccAttrData, 0, bytAccAttrData.Length);
                if (ret <= 0)
                {
                    break;
                }

                ts2 = CStructBytesFormat.rawDeserialize<AccAttr>(bytAccAttrData);
                m_lastIndex = (UInt32)ts2.nIndex;

                m_playersAttrList.Add(ts2);
            }

            fs.Close();

            return true;
        }

        public bool SavePlayerInfos()
        {
            if (File.Exists(@"new\\players.dat"))
            {
                File.Delete(@"new\\players.dat");
            }

            FileStream fs = new FileStream(@"new\\players.dat", FileMode.Create, FileAccess.Write);

            //写头
            var tmpByte = CStructBytesFormat.StructToBytes<AccAttrHead>(m_AccAttrHead);
            fs.Write(tmpByte, 0, tmpByte.Length);
            //写正文
            for (int i = 0; i < m_playersAttrList.Count; i++)
            {
                //var tmp = CStructBytesFormat.StructToBytes<AccAttr>(m_playersAttrList[i]);
                var tmp = CStructBytesFormat.rawSerialize(m_playersAttrList[i]);
                fs.Write(tmp, 0, tmp.Length);
            }

            fs.Close();

            return true;
        }

        public bool AddPlayer(AccAttr player)
        {
            m_playersAttrList.Add(player);

            return true;
        }

        public bool AccountExit(byte[] account, out AccAttr player)
        {
            for (int i = 0; i < m_playersAttrList.Count; i++)
            {
                if (ByteSerEquals(m_playersAttrList[i].Account, account))
                {
                    player = m_playersAttrList[i];
                    return true;
                }
            }
            player = new AccAttr();
            return false;
        }

        public bool NameExit(byte[] name)
        {
            for (int i = 0; i < m_playersAttrList.Count; i++)
            {
                if (ByteSerEquals(m_playersAttrList[i].Name, name))
                {
                    return true;
                }
            }
            return false;
        }

        public UInt32 GetLastIndex()
        {
            return m_lastIndex;
        }

        public void PlayersAttrListClear()
        {
            m_playersAttrList.Clear();
        }

        public bool GetAttrByName(string name,out AccAttr playerAttr)
        {
            for (int i=0; i<m_playersAttrList.Count; i++)
            {
                if (GameStrToSimpleCN(m_playersAttrList[i].Name) == name)
                {
                    playerAttr = m_playersAttrList[i];
                    return true;
                }
            }
            playerAttr = new AccAttr();

            return false;
        }

        private bool ByteSerEquals(byte[] b1, byte[] b2)
        {
            if (b1.Length != b2.Length) return false;
            if (b1 == null || b2 == null) return false;
            for (int i = 0; i < b1.Length; i++)
                if (b1[i] != b2[i])
                    return false;
            return true;
        }

        public bool GetAttrByName(byte[] name, out AccAttr playerAttr)
        {
            for (int i = 0; i < m_playersAttrList.Count; i++)
            {
                if (ByteSerEquals(m_playersAttrList[i].Name, name))
                {
                    playerAttr = m_playersAttrList[i];
                    return true;
                }
            }
            playerAttr = new AccAttr();

            return false;
        }

        public List<int> LoadAllWarloadIndex()
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

        public List<AccAttr> LoadAllWarloadIndexDesc()
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

        public List<int> LoadAllLeaderIndex()
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
        public List<AccAttr> LoadAllLeaderIndexDesc()
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
        public List<int> LoadAllAdvisorIndex()
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
        public List<AccAttr> LoadAllAdvisorIndexDesc()
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
        public List<int> LoadAllWizardIndex()
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
        public List<AccAttr> LoadAllWizardIndexDesc()
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

        public static AccAttr DoAddEquipIimes(AccAttr acc)
        {
            AccAttr newAcc = new AccAttr();
            newAcc = acc;

            if (acc.UpShield.usedTimes > 0)
            {
                newAcc.UpShield.usedTimes = (byte)(acc.UpShield.usedTimes - 1);
            }
            if (acc.UpWeapen.usedTimes > 0)
            {
                newAcc.UpWeapen.usedTimes = (byte)(acc.UpWeapen.usedTimes - 1);
            }
            if (acc.Armor.usedTimes > 0)
            {
                newAcc.Armor.usedTimes = (byte)(acc.Armor.usedTimes - 1);
            }
            if (acc.Head.usedTimes > 0)
            {
                newAcc.Head.usedTimes = (byte)(acc.Head.usedTimes - 1);
            }
            if (acc.Foot.usedTimes > 0)
            {
                newAcc.Foot.usedTimes = (byte)(acc.Foot.usedTimes - 1);
            }
            if (acc.LeftOther.usedTimes > 0)
            {
                newAcc.LeftOther.usedTimes = (byte)(acc.LeftOther.usedTimes - 1);
            }
            if (acc.RightOther.usedTimes > 0)
            {
                newAcc.RightOther.usedTimes = (byte)(acc.RightOther.usedTimes - 1);
            }
            if (acc.Underwear.usedTimes > 0)
            {
                newAcc.Underwear.usedTimes = (byte)(acc.Underwear.usedTimes - 1);
            }
            if (acc.Arm.usedTimes > 0)
            {
                newAcc.Arm.usedTimes = (byte)(acc.Arm.usedTimes - 1);
            }
            if (acc.P.usedTimes > 0)
            {
                newAcc.P.usedTimes = (byte)(acc.P.usedTimes - 1);
            }
            if (acc.Horse.usedTimes > 0)
            {
                newAcc.Horse.usedTimes = (byte)(acc.Horse.usedTimes - 1);
            }
            if (acc.DownShield.usedTimes > 0)
            {
                newAcc.DownShield.usedTimes = (byte)(acc.DownShield.usedTimes - 1);
            }
            if (acc.DownWeapen.usedTimes > 0)
            {
                newAcc.DownWeapen.usedTimes = (byte)(acc.DownWeapen.usedTimes - 1);
            }
            return newAcc;
        }
    }
}
