using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace 地图编辑器
{
    public partial class MapEdit : Form
    {
        MapCtrl m_MapCtrl = new MapCtrl();
        public MapEdit()
        {
            InitializeComponent();
        }

        private void MapEdit_Load(object sender, EventArgs e)
        {
            //加载造型列表
            //加载Stage列表
            //加载shop列表
            //加载gameresource列表

            //读取data目录下 Levelxxx.bin文件
            if(false)//目录或者文件不存在
            {
                //修改标题
            }
            else
            {
                //遍历目录文件Levelxxx.bin文件,截取ID
                string _mapFileName = "LEVEL299.BIN";
                string _id = "299";
                string _name = "露天市场";
                if(true)//id在 stage列表中
                {
                    //lstv_MapList添加
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = _id;
                    lvi.SubItems.Add(_name);
                    lstv_MapList.Items.Add(lvi);
                }
            }
        }
        private int lstv_MapList_SelectedIndex = 0;
        private void lstv_MapList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstv_MapList.SelectedIndices != null && lstv_MapList.SelectedIndices.Count > 0)
            {
                bool ret = false;
                ret = m_MapCtrl.LoadInfo("LEVEL299.BIN");
                if (!ret)
                {
                    MessageBox.Show("文件不存在");
                    return;
                }
                //读取文件， 取得NodeList
                List<MapNode> NodeList = m_MapCtrl.GetNodeList();
                //将Node逐个添加到lstv_NodeList中
                foreach (var node in NodeList)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = node.id.ToString();
                    lvi.SubItems.Add(node.pos_x.ToString());
                    lvi.SubItems.Add(node.pos_y.ToString());
                    lvi.SubItems.Add(node.to_map.ToString());
                    lvi.SubItems.Add(node.to_map_x.ToString());
                    lvi.SubItems.Add(node.to_map_y.ToString());
                    lvi.SubItems.Add(node.npc_talk.ToString());
                    lvi.SubItems.Add(node.res_name.ToString());
                    lvi.SubItems.Add(node.to_shop.ToString());
                    lstv_NodeList.Items.Add(lvi);
                }
                //X坐标排序
            }
        }

        private void btn_MapSave_Click(object sender, EventArgs e)
        {
            m_MapCtrl.NodeClear();
            for (int i = 0; i < lstv_NodeList.Items.Count; i++ )
            {
                ListViewItem lvi = lstv_NodeList.Items[i];
                MapNode node = new MapNode();
                node.id = Int32.Parse(lvi.SubItems[0].Text);
                node.pos_x = Int32.Parse(lvi.SubItems[1].Text);
                node.pos_y = Int32.Parse(lvi.SubItems[2].Text);
                node.to_map = Int32.Parse(lvi.SubItems[3].Text);
                node.to_map_x = Int32.Parse(lvi.SubItems[4].Text);
                node.to_map_y = Int32.Parse(lvi.SubItems[5].Text);
                node.npc_talk = Int32.Parse(lvi.SubItems[6].Text);
                node.res_name = Int32.Parse(lvi.SubItems[7].Text);
                node.to_shop = Int32.Parse(lvi.SubItems[8].Text);
                m_MapCtrl.AddNode(node);
            }
            m_MapCtrl.SaveInfo();
        }

        private void btn_NodeMdfy_Click(object sender, EventArgs e)
        {
            if (lstv_NodeList_SelectedIndex >= 0 && lstv_NodeList_SelectedIndex < lstv_NodeList.Items.Count)
            {
                lstv_NodeList.Items[lstv_NodeList_SelectedIndex].SubItems[0].Text = txt_ShapeId.Text;
                lstv_NodeList.Items[lstv_NodeList_SelectedIndex].SubItems[1].Text = txt_NodeX.Text;
                lstv_NodeList.Items[lstv_NodeList_SelectedIndex].SubItems[2].Text = txt_NodeY.Text;
                lstv_NodeList.Items[lstv_NodeList_SelectedIndex].SubItems[3].Text = txt_GotoMap.Text;
                lstv_NodeList.Items[lstv_NodeList_SelectedIndex].SubItems[4].Text = txt_GotoMapX.Text;
                lstv_NodeList.Items[lstv_NodeList_SelectedIndex].SubItems[5].Text = txt_GotoMapY.Text;
                lstv_NodeList.Items[lstv_NodeList_SelectedIndex].SubItems[6].Text = txt_NodeTalkId.Text;
                lstv_NodeList.Items[lstv_NodeList_SelectedIndex].SubItems[7].Text = txt_NodeName.Text;
                lstv_NodeList.Items[lstv_NodeList_SelectedIndex].SubItems[8].Text = txt_NodeShopId.Text;
            }
        }
        private int lstv_NodeList_SelectedIndex = 0;
        private void lstv_NodeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstv_NodeList.SelectedIndices != null && lstv_NodeList.SelectedIndices.Count > 0)
            {
                lstv_NodeList.Items[lstv_NodeList_SelectedIndex].BackColor = Color.Transparent;
                lstv_NodeList_SelectedIndex = lstv_NodeList.SelectedItems[0].Index;
                lstv_NodeList.Items[lstv_NodeList_SelectedIndex].BackColor = Color.AliceBlue;

                txt_ShapeId.Text = lstv_NodeList.Items[lstv_NodeList_SelectedIndex].SubItems[0].Text;
                txt_NodeX.Text = lstv_NodeList.Items[lstv_NodeList_SelectedIndex].SubItems[1].Text;
                txt_NodeY.Text = lstv_NodeList.Items[lstv_NodeList_SelectedIndex].SubItems[2].Text;
                txt_GotoMap.Text = lstv_NodeList.Items[lstv_NodeList_SelectedIndex].SubItems[3].Text;
                txt_GotoMapX.Text = lstv_NodeList.Items[lstv_NodeList_SelectedIndex].SubItems[4].Text;
                txt_GotoMapY.Text = lstv_NodeList.Items[lstv_NodeList_SelectedIndex].SubItems[5].Text;
                txt_NodeTalkId.Text = lstv_NodeList.Items[lstv_NodeList_SelectedIndex].SubItems[6].Text;
                txt_NodeName.Text = lstv_NodeList.Items[lstv_NodeList_SelectedIndex].SubItems[7].Text;
                txt_NodeShopId.Text = lstv_NodeList.Items[lstv_NodeList_SelectedIndex].SubItems[8].Text;
            }
        }
    }
}
