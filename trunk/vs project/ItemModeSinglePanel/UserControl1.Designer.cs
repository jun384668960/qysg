namespace ItemModeSinglePanel
{
    partial class SingePanel
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lbl_Name = new System.Windows.Forms.Label();
            this.lbl_Cost = new System.Windows.Forms.Label();
            this.pnl_main = new System.Windows.Forms.Panel();
            this.lbl_Attr = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.pnl_main.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.ErrorImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(9, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(82, 50);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // lbl_Name
            // 
            this.lbl_Name.Location = new System.Drawing.Point(145, 5);
            this.lbl_Name.Name = "lbl_Name";
            this.lbl_Name.Size = new System.Drawing.Size(170, 23);
            this.lbl_Name.TabIndex = 1;
            this.lbl_Name.Text = "三国银票";
            this.lbl_Name.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.lbl_Name.Click += new System.EventHandler(this.lbl_Name_Click);
            // 
            // lbl_Cost
            // 
            this.lbl_Cost.Location = new System.Drawing.Point(145, 28);
            this.lbl_Cost.Name = "lbl_Cost";
            this.lbl_Cost.Size = new System.Drawing.Size(170, 23);
            this.lbl_Cost.TabIndex = 1;
            this.lbl_Cost.Text = "1000";
            this.lbl_Cost.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.lbl_Cost.Click += new System.EventHandler(this.lbl_Cost_Click);
            // 
            // pnl_main
            // 
            this.pnl_main.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl_main.Controls.Add(this.lbl_Attr);
            this.pnl_main.Controls.Add(this.lbl_Name);
            this.pnl_main.Controls.Add(this.lbl_Cost);
            this.pnl_main.Controls.Add(this.pictureBox1);
            this.pnl_main.Location = new System.Drawing.Point(3, 3);
            this.pnl_main.Margin = new System.Windows.Forms.Padding(0);
            this.pnl_main.Name = "pnl_main";
            this.pnl_main.Size = new System.Drawing.Size(320, 60);
            this.pnl_main.TabIndex = 2;
            this.pnl_main.Click += new System.EventHandler(this.pnl_main_Click);
            // 
            // lbl_Attr
            // 
            this.lbl_Attr.Location = new System.Drawing.Point(98, 5);
            this.lbl_Attr.Name = "lbl_Attr";
            this.lbl_Attr.Size = new System.Drawing.Size(30, 23);
            this.lbl_Attr.TabIndex = 1;
            this.lbl_Attr.Text = "HOT";
            this.lbl_Attr.Click += new System.EventHandler(this.lbl_Attr_Click);
            // 
            // SingePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnl_main);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "SingePanel";
            this.Size = new System.Drawing.Size(325, 65);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.pnl_main.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lbl_Name;
        private System.Windows.Forms.Label lbl_Cost;
        private System.Windows.Forms.Panel pnl_main;
        private System.Windows.Forms.Label lbl_Attr;
    }
}
