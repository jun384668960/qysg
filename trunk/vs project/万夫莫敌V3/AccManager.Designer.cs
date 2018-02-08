namespace register_client
{
    partial class AccManager
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AccManager));
            this.txt_name = new System.Windows.Forms.TextBox();
            this.txt_pwd = new System.Windows.Forms.TextBox();
            this.txt_pwd2 = new System.Windows.Forms.TextBox();
            this.btn_register = new System.Windows.Forms.Panel();
            this.btn_close = new System.Windows.Forms.Panel();
            this.txtValidCode = new System.Windows.Forms.TextBox();
            this.picValidCode = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picValidCode)).BeginInit();
            this.SuspendLayout();
            // 
            // txt_name
            // 
            this.txt_name.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txt_name.CausesValidation = false;
            this.txt_name.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.txt_name.Location = new System.Drawing.Point(235, 51);
            this.txt_name.Name = "txt_name";
            this.txt_name.Size = new System.Drawing.Size(209, 14);
            this.txt_name.TabIndex = 1;
            this.txt_name.Text = "6-20位，请不要输入特殊字符！";
            this.txt_name.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txt_name_MouseDown);
            this.txt_name.MouseLeave += new System.EventHandler(this.txt_name_MouseLeave);
            // 
            // txt_pwd
            // 
            this.txt_pwd.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txt_pwd.CausesValidation = false;
            this.txt_pwd.Location = new System.Drawing.Point(235, 88);
            this.txt_pwd.Name = "txt_pwd";
            this.txt_pwd.PasswordChar = '*';
            this.txt_pwd.Size = new System.Drawing.Size(209, 14);
            this.txt_pwd.TabIndex = 2;
            this.txt_pwd.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txt_pwd_MouseDown);
            // 
            // txt_pwd2
            // 
            this.txt_pwd2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txt_pwd2.CausesValidation = false;
            this.txt_pwd2.Location = new System.Drawing.Point(235, 128);
            this.txt_pwd2.Name = "txt_pwd2";
            this.txt_pwd2.PasswordChar = '*';
            this.txt_pwd2.Size = new System.Drawing.Size(209, 14);
            this.txt_pwd2.TabIndex = 3;
            this.txt_pwd2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txt_pwd2_MouseDown);
            // 
            // btn_register
            // 
            this.btn_register.BackColor = System.Drawing.Color.Transparent;
            this.btn_register.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_register.Location = new System.Drawing.Point(227, 220);
            this.btn_register.Name = "btn_register";
            this.btn_register.Size = new System.Drawing.Size(131, 43);
            this.btn_register.TabIndex = 5;
            this.btn_register.Click += new System.EventHandler(this.btn_register_Click);
            // 
            // btn_close
            // 
            this.btn_close.BackColor = System.Drawing.Color.Transparent;
            this.btn_close.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_close.Location = new System.Drawing.Point(524, 4);
            this.btn_close.Name = "btn_close";
            this.btn_close.Size = new System.Drawing.Size(24, 19);
            this.btn_close.TabIndex = 3;
            this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
            // 
            // txtValidCode
            // 
            this.txtValidCode.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtValidCode.CausesValidation = false;
            this.txtValidCode.Location = new System.Drawing.Point(235, 168);
            this.txtValidCode.Name = "txtValidCode";
            this.txtValidCode.Size = new System.Drawing.Size(80, 14);
            this.txtValidCode.TabIndex = 4;
            // 
            // picValidCode
            // 
            this.picValidCode.BackColor = System.Drawing.Color.Transparent;
            this.picValidCode.Location = new System.Drawing.Point(342, 162);
            this.picValidCode.Name = "picValidCode";
            this.picValidCode.Size = new System.Drawing.Size(100, 30);
            this.picValidCode.TabIndex = 5;
            this.picValidCode.TabStop = false;
            // 
            // AccManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Magenta;
            //this.BackgroundImage = global::register_client.Properties.Resources.和尚;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(552, 292);
            this.Controls.Add(this.picValidCode);
            this.Controls.Add(this.btn_close);
            this.Controls.Add(this.btn_register);
            this.Controls.Add(this.txtValidCode);
            this.Controls.Add(this.txt_pwd2);
            this.Controls.Add(this.txt_pwd);
            this.Controls.Add(this.txt_name);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AccManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "用户注册";
            this.TransparencyKey = System.Drawing.Color.Magenta;
            this.Load += new System.EventHandler(this.AccManager_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AccManager_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.AccManager_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.AccManager_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.picValidCode)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_name;
        private System.Windows.Forms.TextBox txt_pwd;
        private System.Windows.Forms.TextBox txt_pwd2;
        private System.Windows.Forms.Panel btn_register;
        private System.Windows.Forms.Panel btn_close;
        private System.Windows.Forms.TextBox txtValidCode;
        private System.Windows.Forms.PictureBox picValidCode;

    }
}

