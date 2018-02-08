using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace 地图编辑器
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct MapHead
    {
        public Int32 flag;
        public Int32 nodeCount;
        public Int32 unknow1;
        public Int32 unknow2;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct MapNode
    {
        public Int32 unknow1;
        public Int32 id;
        public Int32 pos_x;
        public Int32 pos_y;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string unknow2;
        public Int32 unknow3;
        public Int32 to_map;
        public Int32 to_map_x;
        public Int32 to_map_y;
        public Int32 to_shop;
        public Int32 unknow4;
        public Int32 unknow5;
        public Int32 unknow6;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string unknow7;
        public Int32 npc_talk;
        public Int32 res_name;
        public Int32 unknow8;
        public Int32 unknow9;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 48)]
        public string unknow10;
    }
    public class MapCtrl
    {
        //file name
        private string m_MapName = "";
        //head
        MapHead m_head = new MapHead();
        //tail
        List<byte> m_tail = new List<byte>();
        //NodeList
        List<MapNode> m_NodeList = new List<MapNode>();
        //加载信息
        public bool LoadInfo(string file)
        {
            m_NodeList.Clear();
            m_tail.Clear();

            if (!File.Exists(file))
            {
                return false;
            }
            m_MapName = file;

            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);

            int ret = 0;
            byte[] bytData = new byte[Marshal.SizeOf(m_head)];
            ret = fs.Read(bytData, 0, Marshal.SizeOf(m_head));
            m_head = CStructBytesFormat.BytesToStruct<MapHead>(bytData);
            while (ret > 0)
            {
                MapNode aa = new MapNode();
                byte[] bytMapNodeData = new byte[Marshal.SizeOf(aa)];
                ret = fs.Read(bytMapNodeData, 0, Marshal.SizeOf(aa));
                if (ret <= 0)
                {
                    break;
                }
                MapNode mapNode = CStructBytesFormat.BytesToStruct<MapNode>(bytMapNodeData);
                if (mapNode.unknow1 == 0)
                {
                    m_NodeList.Add(mapNode);
                }
                else 
                {
                    byte[] bytTailData = new byte[ret];
                    Array.Copy(bytMapNodeData, 0, bytTailData, 0, ret);
                    m_tail.AddRange(bytTailData);
                }
            }

            fs.Close();

            return true;
        }

        //Save
        public bool SaveInfo()
        {
            string newName = "new_" + m_MapName;
            if (File.Exists(newName))
            {
                File.Delete(newName);
            }

            FileStream fs = new FileStream(newName, FileMode.Create, FileAccess.Write);

            //写头
            var tmpByte = CStructBytesFormat.StructToBytes<MapHead>(m_head);
            fs.Write(tmpByte, 0, tmpByte.Length);
            //写正文
            for (int i = 0; i < m_NodeList.Count; i++)
            {
                //var tmp = CStructBytesFormat.StructToBytes<AccAttr>(m_playersAttrList[i]);
                var tmp = CStructBytesFormat.rawSerialize(m_NodeList[i]);
                fs.Write(tmp, 0, tmp.Length);
            }

            byte[] bBig = new byte[m_tail.Count];
            m_tail.CopyTo(bBig);
            fs.Write(bBig, 0, bBig.Length);

            fs.Close();

            return true;
        }
        //Get NodeList
        public List<MapNode> GetNodeList()
        {
            return m_NodeList;
        }
        //Node ADD
        public bool AddNode(MapNode node)
        {
            //
            m_head.nodeCount++;
            m_NodeList.Add(node);

            return true;
        }
        //Node Del
        public bool RemoveNode(MapNode node)
        {
            //
            bool ret = m_NodeList.Remove(node);
            if (ret)
            {
                m_head.nodeCount--;
                return true;
            }

            return false;
        }

        public void NodeClear()
        {
            m_NodeList.Clear();
            m_head.nodeCount = 0;
        }

        //Node Mdfy
        public bool MdfyNode(MapNode oldnode, MapNode newnode)
        {
            int index = FindNode(oldnode);
            if (index >= 0)
            {
                m_NodeList[index] = newnode;
            }
            return false;
        }
        //Node Find
        public int FindNode(MapNode node)
        {
            for (int index = 0; index < m_NodeList.Count; index++ )
            {
                if (m_NodeList[index].id == node.id && m_NodeList[index].pos_x == node.pos_x && m_NodeList[index].pos_y == node.pos_y)
                {
                    return index;
                }
            }
            return -1;
        }

    }
}
