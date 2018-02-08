using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace SGDataHelper
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct SkillAttrHead
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] head;
    };

    public struct SkillAttr
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 551)]
        public byte[] use;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 551)]
        public byte[] learn;
    };
    public class CSkillsCtrl
    {
        private SkillAttrHead m_SkillAttrHead = new SkillAttrHead();
        private List<SkillAttr> m_SkillAttrList = new List<SkillAttr>();
        private string m_Forder = "";

        public bool SetForder(string forder)
        {
            m_Forder = forder;

            return true;
        }

        public List<SkillAttr> GetSkillAttrList()
        {
            return m_SkillAttrList;
        }

        public bool LoadInfos()
        {
            m_SkillAttrList.Clear();

            if (!File.Exists(m_Forder + @"\\skill.dat"))
            {
                return false;
            }


            FileStream fs = new FileStream(m_Forder + @"\\skill.dat", FileMode.Open, FileAccess.ReadWrite);

            int ret = 0;
            SkillAttrHead ts = new SkillAttrHead();
            byte[] _bytData = new byte[Marshal.SizeOf(ts)];

            ret = fs.Read(_bytData, 0, _bytData.Length);
            if (ret > 0) //记录头
            {
                //m_AccAttrHead = CStructBytesFormat.BytesToStruct<AccAttrHead>(_bytData);
                m_SkillAttrHead = CStructBytesFormat.rawDeserialize<SkillAttrHead>(_bytData);
            }
            while (ret > 0)
            {
                SkillAttr ts2 = new SkillAttr();
                byte[] bytAccAttrData = new byte[Marshal.SizeOf(ts2)];

                ret = fs.Read(bytAccAttrData, 0, bytAccAttrData.Length);
                if (ret <= 0)
                {
                    break;
                }

                ts2 = CStructBytesFormat.rawDeserialize<SkillAttr>(bytAccAttrData);

                m_SkillAttrList.Add(ts2);
            }

            fs.Close();

            return true;
        }

        public bool SaveInfos()
        {
            if (File.Exists(@"new\\skill.dat"))
            {
                File.Delete(@"new\\skill.dat");
            }

            FileStream fs = new FileStream(@"new\\skill.dat", FileMode.Create, FileAccess.Write);

            //写头
            var tmpByte = CStructBytesFormat.StructToBytes<SkillAttrHead>(m_SkillAttrHead);
            fs.Write(tmpByte, 0, tmpByte.Length);
            //写正文
            for (int i = 0; i < m_SkillAttrList.Count; i++)
            {
                var tmp = CStructBytesFormat.rawSerialize(m_SkillAttrList[i]);
                fs.Write(tmp, 0, tmp.Length);
            }

            fs.Close();

            return true;
        }

        public bool AddPlayerSkill(SkillAttr skills)
        {
            m_SkillAttrList.Add(skills);

            return true;
        }
    }
}
