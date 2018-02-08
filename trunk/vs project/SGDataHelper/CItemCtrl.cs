using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace SGDataHelper
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct ItemAttrHead
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] head;
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct SingleItemAttr//
    {
        public UInt32 TimeStamp;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] A1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
        public byte[] PlayerName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] A2;
        public UInt16 ItemId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] A3;
        public byte usedTimes;  //如果是装备则标识此装备已经被使用的次数
        public byte ItemNum;
        public byte used;       //如果是装备则标识此装备是否已经绑定对应角色PlayerName使用 0 - 重新装备要消耗次数 1 - 绑定了，不消耗次数
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 39)]
        public byte[] A4;
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct ItemAttr
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
        public SingleItemAttr[] player1_items;
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
        //public SingleItemAttr[] player2_items;
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
        //public SingleItemAttr[] player3_items;
    };

    public class CItemCtrl
    {
        private ItemAttrHead m_ItemAttrHead = new ItemAttrHead();
        private List<ItemAttr> m_ItemAttrList = new List<ItemAttr>();
        private string m_Forder = "";

        public bool SetForder(string forder)
        {
            m_Forder = forder;

            return true;
        }

        public List<ItemAttr> GetItemAttrList()
        {
            return m_ItemAttrList;
        }

        public bool LoadInfos()
        {
            m_ItemAttrList.Clear();

            if (!File.Exists(m_Forder + @"\\item.dat"))
            {
                return false;
            }


            FileStream fs = new FileStream(m_Forder + @"\\item.dat", FileMode.Open, FileAccess.ReadWrite);

            int ret = 0;
            ItemAttrHead ts = new ItemAttrHead();
            byte[] _bytData = new byte[Marshal.SizeOf(ts)];

            ret = fs.Read(_bytData, 0, _bytData.Length);
            if (ret > 0) //记录头
            {
                m_ItemAttrHead = CStructBytesFormat.rawDeserialize<ItemAttrHead>(_bytData);
            }
            while (ret > 0)
            {
                ItemAttr ts2 = new ItemAttr();
                byte[] bytAccAttrData = new byte[Marshal.SizeOf(ts2)];

                ret = fs.Read(bytAccAttrData, 0, bytAccAttrData.Length);
                if (ret <= 0)
                {
                    break;
                }

                ts2 = CStructBytesFormat.rawDeserialize<ItemAttr>(bytAccAttrData);

                m_ItemAttrList.Add(ts2);
            }

            fs.Close();

            return true;
        }

        public bool SaveInfos()
        {
            if (File.Exists(@"new\\item.dat"))
            {
                File.Delete(@"new\\item.dat");
            }

            FileStream fs = new FileStream(@"new\\item.dat", FileMode.Create, FileAccess.Write);

            //写头
            var tmpByte = CStructBytesFormat.StructToBytes<ItemAttrHead>(m_ItemAttrHead);
            fs.Write(tmpByte, 0, tmpByte.Length);
            //写正文
            for (int i = 0; i < m_ItemAttrList.Count; i++)
            {
                var tmp = CStructBytesFormat.rawSerialize(m_ItemAttrList[i]);
                fs.Write(tmp, 0, tmp.Length);
            }

            fs.Close();

            return true;
        }

        public bool AddPlayerItems(ItemAttr items)
        {
            m_ItemAttrList.Add(items);

            return true;
        }

        public static ItemAttr DoAddEquipIimes(ItemAttr item)
        {
            ItemAttr newItem = new ItemAttr();
            newItem = item;

            for (int i = 0; i < item.player1_items.Length; i++ )
            {
                if (item.player1_items[i].usedTimes > 0)
                {
                    newItem.player1_items[i].usedTimes = (byte)(item.player1_items[i].usedTimes - 1);
                }
            }

            return newItem;
        }
    }
}
