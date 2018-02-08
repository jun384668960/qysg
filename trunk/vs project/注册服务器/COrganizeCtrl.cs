using register_server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace 注册网关
{
    public struct OrganizeAttrHead
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string head;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct OrganizeAttr
    {
	    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2312)]
        public string A1; // 空值
	    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string A2; // 有值，用途未知
	    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
        public string A3; // 空值
	    public UInt16  StageId; // 城市代码
	    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string A4; // 有值，用途未知
	    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        public string OrganizeLeader; // 军团长
	    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string A5; // 有值，用途未知
	    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string A6; // 有值，用途未知

	    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1473)]
        public string A7; // 空值
	    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
        public string A8; // 有值，用途未知
	    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 7)]
        public string A9; //空值
	    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        public string OrganizeName; //军团名
	    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        public string OrganizeLeaderZh; //团长
	    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        public string OrganizeLeaderFu; //副团
	    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
        public string A10; //猜测为团员数量
	    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string A11; // 有值，用途未知

	    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 519)]
        public string A12; // 空值
    };
    class COrganizeCtrl
    {
        private static List<OrganizeAttr> m_OrganizeAttrList = new List<OrganizeAttr>();

        public static List<OrganizeAttr> LoadOrganizeInfos(string file, bool reload = false)
        {
            if (!reload && m_OrganizeAttrList.Count != 0)
            {
                return m_OrganizeAttrList;
            }

            List<OrganizeAttr> OrganizeAttrList = new List<OrganizeAttr>();
            OrganizeAttrList.Clear();

            string dbfile = file;
            if (!File.Exists(dbfile))
            {
                OrganizeAttrList.Clear();
                return OrganizeAttrList;
            }

            if (File.Exists(@".\\Temp\\organize.dat"))
            {
                File.SetAttributes(@".\\Temp\\organize.dat", FileAttributes.Normal);
                File.Copy(dbfile, @".\\Temp\\organize.dat", true);
            }
            else
            {
                File.Copy(dbfile, @".\\Temp\\organize.dat", false);
            }


            FileStream fs = new FileStream(".\\Temp\\organize.dat", FileMode.Open, FileAccess.ReadWrite);
            OrganizeAttrHead head = new OrganizeAttrHead();

            int ret = 0;
            byte[] bytData = new byte[Marshal.SizeOf(head)];
            ret = fs.Read(bytData, 0, Marshal.SizeOf(head));
            while (ret > 0)
            {
                OrganizeAttr aa = new OrganizeAttr();
                byte[] _bytData = new byte[Marshal.SizeOf(aa)];
                ret = fs.Read(_bytData, 0, Marshal.SizeOf(aa));
                if (ret <= 0)
                {
                    break;
                }
                OrganizeAttr attr = CStructBytesFormat.BytesToStruct<OrganizeAttr>(_bytData);

                OrganizeAttrList.Add(attr);
            }

            fs.Close();

            //更新本地list
            m_OrganizeAttrList.Clear();
            m_OrganizeAttrList = OrganizeAttrList;

            return m_OrganizeAttrList;
        }

        public static List<OrganizeAttr> GetAttrList()
        {
            return m_OrganizeAttrList;
        }
    }
}
