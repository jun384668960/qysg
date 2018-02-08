using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ItemModeSinglePanel
{
    public partial class SingePanel: UserControl
    {
        public SingePanel()
        {
            InitializeComponent();
        }

        private Color colFColor;
        private Color colBColor;

        public Color PBackColor
        {
            get 
            {
                return colBColor;
            }
            set
            {
                colBColor = value;
                pnl_main.BackColor = colBColor;
                this.BackColor = colBColor;
            }
        }
        public Color PForeColor
        {
            get
            {
                return colFColor;
            }
            set
            {
                colFColor = value;
                lbl_Name.ForeColor = colFColor;
                lbl_Cost.ForeColor = colFColor;
            }
        }

        private string code = "";
        private string type = "";
        private string item_id = "";
        private string item_number = "";
        private string cost = "";
        private string item_attr = "";

        public String Code
        {
            get
            {
                return code;
            }
            set
            {
                code = value;
            }
        }
        public String Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }
        public String Item_Id
        {
            get
            {
                return item_id;
            }
            set
            {
                item_id = value;
                if (item_number != "")
                {
                    lbl_Name.Text = item_id + "(" + item_number + ")";
                }
                else 
                {
                    lbl_Name.Text = item_id;
                }
            }
        }

        public String Item_Number
        {
            get
            {
                return item_number;
            }
            set
            {
                item_number = value;
                if (item_number != "")
                {
                    lbl_Name.Text = item_id + "(" + item_number + ")";
                }
                else
                {
                    lbl_Name.Text = item_id;
                }
            }
        }

        public String Item_Attr
        {
            get
            {
                return item_attr;
            }
            set
            {
                item_attr = value;
                lbl_Attr.Text = item_attr;
            }
        }
        

        public String Cost
        {
            get
            {
                return cost;
            }
            set
            {
                cost = value;
                lbl_Cost.Text = cost;
            }
        }

        private void pnl_main_Click(object sender, EventArgs e)
        {
            OnClick(e);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            OnClick(e);
        }

        private void lbl_Attr_Click(object sender, EventArgs e)
        {
            OnClick(e);
        }

        private void lbl_Name_Click(object sender, EventArgs e)
        {
            OnClick(e);
        }

        private void lbl_Cost_Click(object sender, EventArgs e)
        {
            OnClick(e);
        }
    }
}
