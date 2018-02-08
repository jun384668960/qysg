namespace register_client
{
    partial class FrmPwdMdfy
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmPwdMdfy));
            this.lbl_Close = new System.Windows.Forms.Label();
            this.btn_OK = new System.Windows.Forms.Label();
            this.txt_userName = new System.Windows.Forms.TextBox();
            this.txt_oldPwd = new System.Windows.Forms.TextBox();
            this.txt_newPwd = new System.Windows.Forms.TextBox();
            this.txt_ValidCode = new System.Windows.Forms.TextBox();
            this.picValidCode = new System.Windows.Forms.PictureBox();
            this.rbt_qysg = new System.Windows.Forms.RadioButton();
            this.rbt_hcsg = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.picValidCode)).BeginInit();
            this.SuspendLayout();
            // 
            // lbl_Close
            // 
            this.lbl_Close.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Close.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbl_Close.Location = new System.Drawing.Point(523, 5);
            this.lbl_Close.Name = "lbl_Close";
            this.lbl_Close.Size = new System.Drawing.Size(22, 20);
            this.lbl_Close.TabIndex = 0;
            this.lbl_Close.Click += new System.EventHandler(this.lbl_Close_Click);
            // 
            // btn_OK
            // 
            this.btn_OK.BackColor = System.Drawing.Color.Transparent;
            this.btn_OK.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_OK.Location = new System.Drawing.Point(225, 220);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(133, 41);
            this.btn_OK.TabIndex = 5;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // txt_userName
            // 
            this.txt_userName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txt_userName.Location = new System.Drawing.Point(235, 50);
            this.txt_userName.Name = "txt_userName";
            this.txt_userName.Size = new System.Drawing.Size(203, 14);
            this.txt_userName.TabIndex = 1;
            this.txt_userName.Text = "6-20位，请不要输入特殊字符！";
            this.txt_userName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txt_userName_MouseDown);
            this.txt_userName.MouseEnter += new System.EventHandler(this.txt_userName_MouseEnter);
            this.txt_userName.MouseLeave += new System.EventHandler(this.txt_userName_MouseLeave);
            // 
            // txt_oldPwd
            // 
            this.txt_oldPwd.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txt_oldPwd.Location = new System.Drawing.Point(235, 89);
            this.txt_oldPwd.Name = "txt_oldPwd";
            this.txt_oldPwd.PasswordChar = '*';
            this.txt_oldPwd.Size = new System.Drawing.Size(203, 14);
            this.txt_oldPwd.TabIndex = 2;
            this.txt_oldPwd.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txt_oldPwd_MouseDown);
            // 
            // txt_newPwd
            // 
            this.txt_newPwd.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txt_newPwd.Location = new System.Drawing.Point(235, 129);
            this.txt_newPwd.Name = "txt_newPwd";
            this.txt_newPwd.PasswordChar = '*';
            this.txt_newPwd.Size = new System.Drawing.Size(203, 14);
            this.txt_newPwd.TabIndex = 3;
            this.txt_newPwd.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txt_newPwd_MouseDown);
            // 
            // txt_ValidCode
            // 
            this.txt_ValidCode.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txt_ValidCode.Location = new System.Drawing.Point(235, 168);
            this.txt_ValidCode.Name = "txt_ValidCode";
            this.txt_ValidCode.Size = new System.Drawing.Size(79, 14);
            this.txt_ValidCode.TabIndex = 4;
            // 
            // picValidCode
            // 
            this.picValidCode.BackColor = System.Drawing.Color.Transparent;
            this.picValidCode.Location = new System.Drawing.Point(341, 160);
            this.picValidCode.Name = "picValidCode";
            this.picValidCode.Size = new System.Drawing.Size(100, 30);
            this.picValidCode.TabIndex = 6;
            this.picValidCode.TabStop = false;
            // 
            // rbt_qysg
            // 
            this.rbt_qysg.AutoSize = true;
            this.rbt_qysg.BackColor = System.Drawing.Color.Transparent;
            this.rbt_qysg.Checked = true;
            this.rbt_qysg.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.rbt_qysg.Location = new System.Drawing.Point(461, 47);
            this.rbt_qysg.Name = "rbt_qysg";
            this.rbt_qysg.Size = new System.Drawing.Size(71, 16);
            this.rbt_qysg.TabIndex = 7;
            this.rbt_qysg.TabStop = true;
            this.rbt_qysg.Text = "情义无双";
            this.rbt_qysg.UseVisualStyleBackColor = false;
            this.rbt_qysg.CheckedChanged += new System.EventHandler(this.rbt_qysg_CheckedChanged);
            // 
            // rbt_hcsg
            // 
            this.rbt_hcsg.AutoSize = true;
            this.rbt_hcsg.BackColor = System.Drawing.Color.Transparent;
            this.rbt_hcsg.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.rbt_hcsg.Location = new System.Drawing.Point(461, 69);
            this.rbt_hcsg.Name = "rbt_hcsg";
            this.rbt_hcsg.Size = new System.Drawing.Size(71, 16);
            this.rbt_hcsg.TabIndex = 7;
            this.rbt_hcsg.Text = "皇朝霸业";
            this.rbt_hcsg.UseVisualStyleBackColor = false;
            this.rbt_hcsg.Visible = false;
            this.rbt_hcsg.CheckedChanged += new System.EventHandler(this.rbt_hcsg_CheckedChanged);
            // 
            // FrmPwdMdfy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Fuchsia;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(552, 292);
            this.Controls.Add(this.rbt_hcsg);
            this.Controls.Add(this.rbt_qysg);
            this.Controls.Add(this.picValidCode);
            this.Controls.Add(this.txt_ValidCode);
            this.Controls.Add(this.txt_newPwd);
            this.Controls.Add(this.txt_oldPwd);
            this.Controls.Add(this.txt_userName);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.lbl_Close);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmPwdMdfy";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "修改密码";
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            this.Load += new System.EventHandler(this.FrmPwdMdfy_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FrmPwdMdfy_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmPwdMdfy_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FrmPwdMdfy_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.picValidCode)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_Close;
        private System.Windows.Forms.Label btn_OK;
        private System.Windows.Forms.TextBox txt_userName;
        private System.Windows.Forms.TextBox txt_oldPwd;
        private System.Windows.Forms.TextBox txt_newPwd;
        private System.Windows.Forms.TextBox txt_ValidCode;
        private System.Windows.Forms.PictureBox picValidCode;
        private System.Windows.Forms.RadioButton rbt_qysg;
        private System.Windows.Forms.RadioButton rbt_hcsg;
    }
}