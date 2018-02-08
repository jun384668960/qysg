namespace RegistSoft0918
{
    partial class FormMain
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
            this.btnRegist = new System.Windows.Forms.Button();
            this.lbRegistInfo = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnRegist
            // 
            this.btnRegist.Location = new System.Drawing.Point(405, 12);
            this.btnRegist.Name = "btnRegist";
            this.btnRegist.Size = new System.Drawing.Size(93, 23);
            this.btnRegist.TabIndex = 0;
            this.btnRegist.Text = "导入注册文件";
            this.btnRegist.UseVisualStyleBackColor = true;
            this.btnRegist.Click += new System.EventHandler(this.btnRegist_Click);
            // 
            // lbRegistInfo
            // 
            this.lbRegistInfo.AutoSize = true;
            this.lbRegistInfo.Location = new System.Drawing.Point(87, 12);
            this.lbRegistInfo.Name = "lbRegistInfo";
            this.lbRegistInfo.Size = new System.Drawing.Size(0, 12);
            this.lbRegistInfo.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "注册信息：";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(529, 124);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbRegistInfo);
            this.Controls.Add(this.btnRegist);
            this.Name = "FormMain";
            this.Text = "软件界面";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRegist;
        private System.Windows.Forms.Label lbRegistInfo;
        private System.Windows.Forms.Label label1;
    }
}

