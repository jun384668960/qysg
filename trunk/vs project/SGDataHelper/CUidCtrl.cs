using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace SGDataHelper
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct UidHead
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x3e64)]
        public byte[] head;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct UidAttr
    {
        public UInt32 nAccId;
        public UInt32 nUid;
        public UInt32 nPlayer1Index;
        public UInt32 nPlayer2Index;
        public UInt32 nPlayer3Index;
    }
    public class CUidCtrl
    {
        private UidHead m_UidHead = new UidHead();
        private List<UidAttr> m_UidAttrList = new List<UidAttr>();
        private UInt32 m_lastUid = 0;
        private UInt32 m_lastAccId = 0;
        private string m_Forder = "";

        public bool SetForder(string forder)
        {
            m_Forder = forder;

            return true;
        }

        public List<UidAttr> GetUidAttrList()
        {
            return m_UidAttrList;
        }

        public bool LoadInfo()
        {
            m_UidAttrList.Clear();

            if (!File.Exists(m_Forder + @"\\uid.dat"))
            {
                return false;
            }


            FileStream fs = new FileStream(m_Forder + @"\\uid.dat", FileMode.Open, FileAccess.ReadWrite);

            int ret = 0;
            UidHead ts = new UidHead();
            byte[] _bytData = new byte[Marshal.SizeOf(ts)];

            ret = fs.Read(_bytData, 0, _bytData.Length);
            if (ret > 0) //记录头
            {
                m_UidHead = CStructBytesFormat.rawDeserialize<UidHead>(_bytData);
            }
            while (ret > 0)
            {
                UidAttr ts2 = new UidAttr();
                byte[] bytAccAttrData = new byte[Marshal.SizeOf(ts2)];

                ret = fs.Read(bytAccAttrData, 0, bytAccAttrData.Length);
                if (ret <= 0)
                {
                    break;
                }

                ts2 = CStructBytesFormat.rawDeserialize<UidAttr>(bytAccAttrData);
                m_lastUid = (UInt32)ts2.nUid;
                m_lastAccId = (UInt32)ts2.nAccId;

                m_UidAttrList.Add(ts2);
            }

            fs.Close();

            return true;
        }

        public bool SaveInfos()
        {
            if (File.Exists(@"new\\uid.dat"))
            {
                File.Delete(@"new\\uid.dat");
            }

            FileStream fs = new FileStream(@"new\\uid.dat", FileMode.Create, FileAccess.Write);

            //写头
            var tmpByte = CStructBytesFormat.StructToBytes<UidHead>(m_UidHead);
            fs.Write(tmpByte, 0, tmpByte.Length);
            //写正文
            for (int i = 0; i < m_UidAttrList.Count; i++)
            {
                //var tmp = CStructBytesFormat.StructToBytes<AccAttr>(m_playersAttrList[i]);
                var tmp = CStructBytesFormat.rawSerialize(m_UidAttrList[i]);
                fs.Write(tmp, 0, tmp.Length);
            }

            fs.Close();

            return true;
        }

        public UInt32 GetLastUid()
        {
            return m_lastUid;
        }

        public UInt32 GetLastAccId()
        {
            return m_lastAccId;
        }

        public bool AddSubPlayer(UInt32 nAccId, UInt32 nIndex)
        { 
            for(int i=0; i<m_UidAttrList.Count; i++)
            {
                if (m_UidAttrList[i].nAccId == nAccId)
                {
                    UidAttr newAttr = new UidAttr();
                    newAttr = m_UidAttrList[i];
                    if (newAttr.nPlayer1Index == 0)
                    {
                        newAttr.nPlayer1Index = nIndex;
                    }
                    else if (newAttr.nPlayer2Index == 0)
                    {
                        newAttr.nPlayer2Index = nIndex;
                    }
                    else if (newAttr.nPlayer3Index == 0)
                    {
                        newAttr.nPlayer3Index = nIndex;
                    }
                    else 
                    {
                        return false;
                    }
                    m_UidAttrList[i] = newAttr;
                    return true;
                }
            }
            return false;
        }

        public bool AddPlayerUid(UInt32 nAccId, UInt32 uid, UInt32 playerIndex)
        {
            UidAttr newAttr = new UidAttr();
            newAttr.nAccId = nAccId;
            newAttr.nUid = uid;
            newAttr.nPlayer1Index = playerIndex;
            newAttr.nPlayer2Index = 0;
            newAttr.nPlayer3Index = 0;

            m_UidAttrList.Add(newAttr);

            return true;
        }
    }
}
