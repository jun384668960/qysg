using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace 物品工具
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ItemCtrl.LoadItemDefList();
            ItemCtrl.LoadItemList();
            ItemCtrl.LoadItemNameList();
            ItemCtrl.LoadItemHelpList();

            FillItemLstV();
        }

        private void FillItemLstV()
        { 
            List<Item_Str> itemList = ItemCtrl.GetItemList();
            lstv_ItemList.Items.Clear();
            foreach (var item in itemList)
            {
                string code = item.code;
                string id = ItemCtrl.GetIdByCode(code);

                ListViewItem lvi = new ListViewItem();
                lvi.Text = CFormat.PureString(id);
                lvi.SubItems.Add(CFormat.PureString(code));
                lstv_ItemList.Items.Add(lvi);
            }
            lstv_ItemList.EndUpdate();
        }

        private void btn_Rebuild_Click(object sender, EventArgs e)
        {
            ItemCtrl.ItemDefNameHelpRebuild();
            MessageBox.Show("重建成功！");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<Item_Str> ItemList = ItemCtrl.GetItemList();

            string file = textBox4.Text + "级物品.txt";

            //写文件
            FileStream rtfs = new FileStream(file, FileMode.Create, FileAccess.Write);
            //StreamWriter rt = new StreamWriter(rtfs, Encoding.GetEncoding(950));
            StreamWriter rt = new StreamWriter(rtfs, Encoding.Default);
            rt.BaseStream.Seek(0, SeekOrigin.Begin);
            rt.BaseStream.Position = 0;

            //遍历DropList
            foreach (var it in ItemList)
            {
                if (it.limit_level == textBox4.Text)
                {
                    rt.WriteLine(ItemCtrl.GetIdByCode(it.code) + "\t" + CFormat.ToSimplified(it.code));
                }
            }

            rt.Close();
            rtfs.Close();

            MessageBox.Show("输出掉宝资料【" + file + "】成功！");
        }
    }
}
