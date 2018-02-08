using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace SGDataHelper
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct SoldierAttrHead
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] head;
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct SoldierAttr //size 2+16+3+2+4+4+16+2+2+2+2+2+58+1+3+1+1+22 = 143
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] type;     //貌似是死的还是活的意思，只看到了1 和 0的值
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Name;     // 名称
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] A1;       // 有值但是不知道是什么
        public UInt16 Level;
        public UInt32 Hp;
        public UInt32 Exp;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] A2;  // 有值但是不知道是什么
	    public UInt16 Attr_str; //武力
	    public UInt16 Attr_int;//智力
	    public UInt16 Type;//兵种
	    public UInt16 Attr_dex;//体魄
	    public UInt16 Attr_mind; //反应
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 58)]
        public byte[] A3; // 有值但是不知道是什么
	    public byte Loyal; //忠诚
	    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] A4; // 有值但是不知道是什么
	    public byte Attack;//附属攻击力
	    public byte Defence;//附属防御力
	    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
        public byte[] A5; // 空值
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct SoldiersAttr
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public SoldierAttr[] Soldiers;
    }

    public class CSoldierCtrl
    {
        private SoldierAttrHead m_SoldierAttrHead = new SoldierAttrHead();
        private List<SoldiersAttr> m_SoldiersAttrList = new List<SoldiersAttr>();
        private string m_Forder = "";

        public bool SetForder(string forder)
        {
            m_Forder = forder;

            return true;
        }

        public List<SoldiersAttr> GetSoldiersAttrList()
        {
            return m_SoldiersAttrList;
        }

        public bool LoadInfos()
        {
            m_SoldiersAttrList.Clear();

            if (!File.Exists(m_Forder + @"\\npc.dat"))
            {
                return false;
            }


            FileStream fs = new FileStream(m_Forder + @"\\npc.dat", FileMode.Open, FileAccess.ReadWrite);

            int ret = 0;
            SoldierAttrHead ts = new SoldierAttrHead();
            byte[] _bytData = new byte[Marshal.SizeOf(ts)];

            ret = fs.Read(_bytData, 0, _bytData.Length);
            if (ret > 0) //记录头
            {
                m_SoldierAttrHead = CStructBytesFormat.rawDeserialize<SoldierAttrHead>(_bytData);
            }
            while (ret > 0)
            {
                SoldiersAttr ts2 = new SoldiersAttr();
                byte[] bytAccAttrData = new byte[Marshal.SizeOf(ts2)];

                ret = fs.Read(bytAccAttrData, 0, bytAccAttrData.Length);
                if (ret <= 0)
                {
                    break;
                }

                ts2 = CStructBytesFormat.rawDeserialize<SoldiersAttr>(bytAccAttrData);

                m_SoldiersAttrList.Add(ts2);
            }

            fs.Close();

            return true;
        }

        public bool SaveInfos()
        {
            if (File.Exists(@"new\\npc.dat"))
            {
                File.Delete(@"new\\npc.dat");
            }

            FileStream fs = new FileStream(@"new\\npc.dat", FileMode.Create, FileAccess.Write);

            //写头
            var tmpByte = CStructBytesFormat.StructToBytes<SoldierAttrHead>(m_SoldierAttrHead);
            fs.Write(tmpByte, 0, tmpByte.Length);
            //写正文
            for (int i = 0; i < m_SoldiersAttrList.Count; i++)
            {
                var tmp = CStructBytesFormat.rawSerialize(m_SoldiersAttrList[i]);
                fs.Write(tmp, 0, tmp.Length);
            }

            fs.Close();

            return true;
        }

        public bool AddPlayerSoldiers(SoldiersAttr items)
        {
            m_SoldiersAttrList.Add(items);

            return true;
        }
    }
}
