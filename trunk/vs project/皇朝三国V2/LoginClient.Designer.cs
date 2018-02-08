using System.Drawing;
namespace register_client
{
    partial class LoginClient
    {

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginClient));
            this.ntyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.lbl_noticTime1 = new System.Windows.Forms.Label();
            this.lbl_noticTime2 = new System.Windows.Forms.Label();
            this.lbl_noticTime3 = new System.Windows.Forms.Label();
            this.lbl_notic1 = new System.Windows.Forms.LinkLabel();
            this.lbl_notic2 = new System.Windows.Forms.LinkLabel();
            this.lbl_notic3 = new System.Windows.Forms.LinkLabel();
            this.lbl_bbs = new System.Windows.Forms.LinkLabel();
            this.lbl_ChongZhi = new System.Windows.Forms.Label();
            this.lbl_HelpCenter = new System.Windows.Forms.Label();
            this.lbl_CurVersion = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_noticTime4 = new System.Windows.Forms.Label();
            this.lbl_notic4 = new System.Windows.Forms.LinkLabel();
            this.lbl_noticTime5 = new System.Windows.Forms.Label();
            this.lbl_notic5 = new System.Windows.Forms.LinkLabel();
            this.lbl_WebSite = new System.Windows.Forms.Label();
            this.lbl_notic = new System.Windows.Forms.LinkLabel();
            this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
            this.btn_accMgr = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_login = new System.Windows.Forms.Label();
            this.btn_accRegisger = new System.Windows.Forms.Label();
            this.btn_min = new System.Windows.Forms.Label();
            this.btn_close = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.textBox2 = new ZBobb.AlphaBlendTextBox();
            this.textBox1 = new ZBobb.AlphaBlendTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
            this.SuspendLayout();
            // 
            // ntyIcon
            // 
            this.ntyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("ntyIcon.Icon")));
            this.ntyIcon.Text = "皇朝三国";
            this.ntyIcon.Visible = true;
            this.ntyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ntyIcon_MouseDoubleClick);
            // 
            // lbl_noticTime1
            // 
            this.lbl_noticTime1.AutoSize = true;
            this.lbl_noticTime1.BackColor = System.Drawing.Color.Transparent;
            this.lbl_noticTime1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.lbl_noticTime1.Location = new System.Drawing.Point(565, 336);
            this.lbl_noticTime1.Name = "lbl_noticTime1";
            this.lbl_noticTime1.Size = new System.Drawing.Size(59, 12);
            this.lbl_noticTime1.TabIndex = 7;
            this.lbl_noticTime1.Text = "公告1时间";
            this.lbl_noticTime1.Visible = false;
            // 
            // lbl_noticTime2
            // 
            this.lbl_noticTime2.AutoSize = true;
            this.lbl_noticTime2.BackColor = System.Drawing.Color.Transparent;
            this.lbl_noticTime2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.lbl_noticTime2.Location = new System.Drawing.Point(565, 361);
            this.lbl_noticTime2.Name = "lbl_noticTime2";
            this.lbl_noticTime2.Size = new System.Drawing.Size(59, 12);
            this.lbl_noticTime2.TabIndex = 7;
            this.lbl_noticTime2.Text = "公告2时间";
            this.lbl_noticTime2.Visible = false;
            // 
            // lbl_noticTime3
            // 
            this.lbl_noticTime3.AutoSize = true;
            this.lbl_noticTime3.BackColor = System.Drawing.Color.Transparent;
            this.lbl_noticTime3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.lbl_noticTime3.Location = new System.Drawing.Point(565, 386);
            this.lbl_noticTime3.Name = "lbl_noticTime3";
            this.lbl_noticTime3.Size = new System.Drawing.Size(59, 12);
            this.lbl_noticTime3.TabIndex = 7;
            this.lbl_noticTime3.Text = "公告3时间";
            this.lbl_noticTime3.Visible = false;
            // 
            // lbl_notic1
            // 
            this.lbl_notic1.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.lbl_notic1.BackColor = System.Drawing.Color.Transparent;
            this.lbl_notic1.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lbl_notic1.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.lbl_notic1.Location = new System.Drawing.Point(388, 336);
            this.lbl_notic1.Name = "lbl_notic1";
            this.lbl_notic1.Size = new System.Drawing.Size(171, 12);
            this.lbl_notic1.TabIndex = 8;
            this.lbl_notic1.TabStop = true;
            this.lbl_notic1.Text = "公告1";
            this.lbl_notic1.Visible = false;
            this.lbl_notic1.VisitedLinkColor = System.Drawing.Color.Blue;
            this.lbl_notic1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbl_notic1_LinkClicked);
            // 
            // lbl_notic2
            // 
            this.lbl_notic2.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.lbl_notic2.BackColor = System.Drawing.Color.Transparent;
            this.lbl_notic2.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lbl_notic2.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.lbl_notic2.Location = new System.Drawing.Point(388, 361);
            this.lbl_notic2.Name = "lbl_notic2";
            this.lbl_notic2.Size = new System.Drawing.Size(171, 12);
            this.lbl_notic2.TabIndex = 8;
            this.lbl_notic2.TabStop = true;
            this.lbl_notic2.Text = "公告2";
            this.lbl_notic2.Visible = false;
            this.lbl_notic2.VisitedLinkColor = System.Drawing.Color.Blue;
            this.lbl_notic2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbl_notic2_LinkClicked);
            // 
            // lbl_notic3
            // 
            this.lbl_notic3.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.lbl_notic3.BackColor = System.Drawing.Color.Transparent;
            this.lbl_notic3.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lbl_notic3.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.lbl_notic3.Location = new System.Drawing.Point(388, 386);
            this.lbl_notic3.Name = "lbl_notic3";
            this.lbl_notic3.Size = new System.Drawing.Size(171, 12);
            this.lbl_notic3.TabIndex = 8;
            this.lbl_notic3.TabStop = true;
            this.lbl_notic3.Text = "公告3";
            this.lbl_notic3.Visible = false;
            this.lbl_notic3.VisitedLinkColor = System.Drawing.Color.Blue;
            this.lbl_notic3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbl_notic3_LinkClicked);
            // 
            // lbl_bbs
            // 
            this.lbl_bbs.BackColor = System.Drawing.Color.Transparent;
            this.lbl_bbs.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbl_bbs.Location = new System.Drawing.Point(554, 99);
            this.lbl_bbs.Name = "lbl_bbs";
            this.lbl_bbs.Size = new System.Drawing.Size(94, 30);
            this.lbl_bbs.TabIndex = 9;
            this.lbl_bbs.Visible = false;
            this.lbl_bbs.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbl_bbs_LinkClicked);
            // 
            // lbl_ChongZhi
            // 
            this.lbl_ChongZhi.BackColor = System.Drawing.Color.Transparent;
            this.lbl_ChongZhi.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbl_ChongZhi.Location = new System.Drawing.Point(230, 99);
            this.lbl_ChongZhi.Name = "lbl_ChongZhi";
            this.lbl_ChongZhi.Size = new System.Drawing.Size(97, 30);
            this.lbl_ChongZhi.TabIndex = 10;
            this.lbl_ChongZhi.Visible = false;
            this.lbl_ChongZhi.Click += new System.EventHandler(this.lbl_ChongZhi_Click);
            // 
            // lbl_HelpCenter
            // 
            this.lbl_HelpCenter.BackColor = System.Drawing.Color.Transparent;
            this.lbl_HelpCenter.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbl_HelpCenter.Location = new System.Drawing.Point(690, 99);
            this.lbl_HelpCenter.Name = "lbl_HelpCenter";
            this.lbl_HelpCenter.Size = new System.Drawing.Size(97, 30);
            this.lbl_HelpCenter.TabIndex = 10;
            this.lbl_HelpCenter.Visible = false;
            this.lbl_HelpCenter.Click += new System.EventHandler(this.lbl_HelpCenter_Click);
            // 
            // lbl_CurVersion
            // 
            this.lbl_CurVersion.BackColor = System.Drawing.Color.Transparent;
            this.lbl_CurVersion.Location = new System.Drawing.Point(748, 540);
            this.lbl_CurVersion.Name = "lbl_CurVersion";
            this.lbl_CurVersion.Size = new System.Drawing.Size(94, 16);
            this.lbl_CurVersion.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(678, 540);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 16);
            this.label1.TabIndex = 10;
            this.label1.Text = "当前版本:";
            this.label1.Visible = false;
            // 
            // lbl_noticTime4
            // 
            this.lbl_noticTime4.AutoSize = true;
            this.lbl_noticTime4.BackColor = System.Drawing.Color.Transparent;
            this.lbl_noticTime4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.lbl_noticTime4.Location = new System.Drawing.Point(565, 411);
            this.lbl_noticTime4.Name = "lbl_noticTime4";
            this.lbl_noticTime4.Size = new System.Drawing.Size(59, 12);
            this.lbl_noticTime4.TabIndex = 7;
            this.lbl_noticTime4.Text = "公告3时间";
            this.lbl_noticTime4.Visible = false;
            // 
            // lbl_notic4
            // 
            this.lbl_notic4.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.lbl_notic4.BackColor = System.Drawing.Color.Transparent;
            this.lbl_notic4.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lbl_notic4.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.lbl_notic4.Location = new System.Drawing.Point(388, 411);
            this.lbl_notic4.Name = "lbl_notic4";
            this.lbl_notic4.Size = new System.Drawing.Size(171, 12);
            this.lbl_notic4.TabIndex = 8;
            this.lbl_notic4.TabStop = true;
            this.lbl_notic4.Text = "公告4";
            this.lbl_notic4.Visible = false;
            this.lbl_notic4.VisitedLinkColor = System.Drawing.Color.Blue;
            this.lbl_notic4.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbl_notic4_LinkClicked);
            // 
            // lbl_noticTime5
            // 
            this.lbl_noticTime5.AutoSize = true;
            this.lbl_noticTime5.BackColor = System.Drawing.Color.Transparent;
            this.lbl_noticTime5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.lbl_noticTime5.Location = new System.Drawing.Point(565, 436);
            this.lbl_noticTime5.Name = "lbl_noticTime5";
            this.lbl_noticTime5.Size = new System.Drawing.Size(59, 12);
            this.lbl_noticTime5.TabIndex = 7;
            this.lbl_noticTime5.Text = "公告3时间";
            this.lbl_noticTime5.Visible = false;
            // 
            // lbl_notic5
            // 
            this.lbl_notic5.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.lbl_notic5.BackColor = System.Drawing.Color.Transparent;
            this.lbl_notic5.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lbl_notic5.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.lbl_notic5.Location = new System.Drawing.Point(388, 436);
            this.lbl_notic5.Name = "lbl_notic5";
            this.lbl_notic5.Size = new System.Drawing.Size(171, 12);
            this.lbl_notic5.TabIndex = 8;
            this.lbl_notic5.TabStop = true;
            this.lbl_notic5.Text = "公告5";
            this.lbl_notic5.Visible = false;
            this.lbl_notic5.VisitedLinkColor = System.Drawing.Color.Blue;
            this.lbl_notic5.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbl_notic5_LinkClicked);
            // 
            // lbl_WebSite
            // 
            this.lbl_WebSite.BackColor = System.Drawing.Color.Transparent;
            this.lbl_WebSite.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbl_WebSite.Location = new System.Drawing.Point(95, 103);
            this.lbl_WebSite.Name = "lbl_WebSite";
            this.lbl_WebSite.Size = new System.Drawing.Size(100, 23);
            this.lbl_WebSite.TabIndex = 11;
            this.lbl_WebSite.Visible = false;
            this.lbl_WebSite.Click += new System.EventHandler(this.lbl_WebSite_Click);
            // 
            // lbl_notic
            // 
            this.lbl_notic.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.lbl_notic.BackColor = System.Drawing.Color.Transparent;
            this.lbl_notic.Font = new System.Drawing.Font("微软雅黑", 15F);
            this.lbl_notic.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(163)))), ((int)(((byte)(90)))), ((int)(((byte)(26)))));
            this.lbl_notic.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lbl_notic.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(163)))), ((int)(((byte)(90)))), ((int)(((byte)(26)))));
            this.lbl_notic.Location = new System.Drawing.Point(392, 299);
            this.lbl_notic.Name = "lbl_notic";
            this.lbl_notic.Size = new System.Drawing.Size(246, 27);
            this.lbl_notic.TabIndex = 8;
            this.lbl_notic.TabStop = true;
            this.lbl_notic.Text = "教主三国新区";
            this.lbl_notic.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_notic.Visible = false;
            this.lbl_notic.VisitedLinkColor = System.Drawing.Color.Blue;
            this.lbl_notic.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbl_notic_LinkClicked);
            // 
            // fileSystemWatcher1
            // 
            this.fileSystemWatcher1.EnableRaisingEvents = true;
            this.fileSystemWatcher1.SynchronizingObject = this;
            // 
            // btn_accMgr
            // 
            this.btn_accMgr.BackColor = System.Drawing.Color.Transparent;
            this.btn_accMgr.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_accMgr.Font = new System.Drawing.Font("宋体", 8F);
            this.btn_accMgr.ForeColor = System.Drawing.Color.White;
            this.btn_accMgr.Location = new System.Drawing.Point(718, 449);
            this.btn_accMgr.Name = "btn_accMgr";
            this.btn_accMgr.Size = new System.Drawing.Size(49, 13);
            this.btn_accMgr.TabIndex = 12;
            this.btn_accMgr.Text = "修改密码";
            this.btn_accMgr.Click += new System.EventHandler(this.btn_accMgr_Click);
            this.btn_accMgr.MouseEnter += new System.EventHandler(this.btn_accMgr_MouseEnter);
            this.btn_accMgr.MouseLeave += new System.EventHandler(this.btn_accMgr_MouseLeave);
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label3.Image = ((System.Drawing.Image)(resources.GetObject("label3.Image")));
            this.label3.Location = new System.Drawing.Point(817, 101);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 21);
            this.label3.TabIndex = 12;
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // btn_login
            // 
            this.btn_login.BackColor = System.Drawing.Color.Transparent;
            this.btn_login.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_login.Location = new System.Drawing.Point(659, 370);
            this.btn_login.Name = "btn_login";
            this.btn_login.Size = new System.Drawing.Size(108, 31);
            this.btn_login.TabIndex = 14;
            this.btn_login.Click += new System.EventHandler(this.btn_login_Click);
            this.btn_login.MouseEnter += new System.EventHandler(this.btn_login_MouseEnter);
            this.btn_login.MouseLeave += new System.EventHandler(this.btn_login_MouseLeave);
            // 
            // btn_accRegisger
            // 
            this.btn_accRegisger.BackColor = System.Drawing.Color.Transparent;
            this.btn_accRegisger.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_accRegisger.Location = new System.Drawing.Point(659, 411);
            this.btn_accRegisger.Name = "btn_accRegisger";
            this.btn_accRegisger.Size = new System.Drawing.Size(108, 31);
            this.btn_accRegisger.TabIndex = 14;
            this.btn_accRegisger.Click += new System.EventHandler(this.btn_accRegisger_Click);
            this.btn_accRegisger.MouseEnter += new System.EventHandler(this.btn_accRegisger_MouseEnter);
            this.btn_accRegisger.MouseLeave += new System.EventHandler(this.btn_accRegisger_MouseLeave);
            // 
            // btn_min
            // 
            this.btn_min.BackColor = System.Drawing.Color.Transparent;
            this.btn_min.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_min.Font = new System.Drawing.Font("宋体", 8F);
            this.btn_min.ForeColor = System.Drawing.Color.White;
            this.btn_min.Location = new System.Drawing.Point(806, 56);
            this.btn_min.Name = "btn_min";
            this.btn_min.Size = new System.Drawing.Size(17, 17);
            this.btn_min.TabIndex = 12;
            this.btn_min.Click += new System.EventHandler(this.btn_min_Click);
            // 
            // btn_close
            // 
            this.btn_close.BackColor = System.Drawing.Color.Transparent;
            this.btn_close.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_close.Font = new System.Drawing.Font("宋体", 8F);
            this.btn_close.ForeColor = System.Drawing.Color.White;
            this.btn_close.Location = new System.Drawing.Point(833, 57);
            this.btn_close.Name = "btn_close";
            this.btn_close.Size = new System.Drawing.Size(17, 17);
            this.btn_close.TabIndex = 12;
            this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 2000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // textBox2
            // 
            this.textBox2.BackAlpha = 0;
            this.textBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(92)))), ((int)(((byte)(92)))));
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.ForeColor = System.Drawing.Color.White;
            this.textBox2.Location = new System.Drawing.Point(643, 284);
            this.textBox2.Name = "textBox2";
            this.textBox2.PasswordChar = '*';
            this.textBox2.Size = new System.Drawing.Size(143, 14);
            this.textBox2.TabIndex = 13;
            // 
            // textBox1
            // 
            this.textBox1.BackAlpha = 0;
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.ForeColor = System.Drawing.Color.White;
            this.textBox1.Location = new System.Drawing.Point(643, 242);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(143, 14);
            this.textBox1.TabIndex = 13;
            // 
            // LoginClient
            // 
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.IndianRed;
            this.BackgroundImage = global::register_client.Properties.Resources.窗体背景;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(900, 645);
            this.Controls.Add(this.btn_close);
            this.Controls.Add(this.btn_min);
            this.Controls.Add(this.btn_accMgr);
            this.Controls.Add(this.btn_accRegisger);
            this.Controls.Add(this.btn_login);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lbl_WebSite);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbl_CurVersion);
            this.Controls.Add(this.lbl_HelpCenter);
            this.Controls.Add(this.lbl_ChongZhi);
            this.Controls.Add(this.lbl_bbs);
            this.Controls.Add(this.lbl_notic5);
            this.Controls.Add(this.lbl_notic4);
            this.Controls.Add(this.lbl_notic3);
            this.Controls.Add(this.lbl_notic2);
            this.Controls.Add(this.lbl_notic);
            this.Controls.Add(this.lbl_notic1);
            this.Controls.Add(this.lbl_noticTime5);
            this.Controls.Add(this.lbl_noticTime4);
            this.Controls.Add(this.lbl_noticTime3);
            this.Controls.Add(this.lbl_noticTime2);
            this.Controls.Add(this.lbl_noticTime1);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LoginClient";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "皇朝三国";
            this.TransparencyKey = System.Drawing.Color.IndianRed;
            this.Load += new System.EventHandler(this.LoginClient_load);
            this.Shown += new System.EventHandler(this.LoginClient_Shown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LoginClient_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LoginClient_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LoginClient_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon ntyIcon;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.Label lbl_noticTime1;
        private System.Windows.Forms.Label lbl_noticTime2;
        private System.Windows.Forms.Label lbl_noticTime3;
        private System.Windows.Forms.LinkLabel lbl_notic1;
        private System.Windows.Forms.LinkLabel lbl_notic2;
        private System.Windows.Forms.LinkLabel lbl_notic3;
        private System.Windows.Forms.LinkLabel lbl_bbs;
        private System.Windows.Forms.Label lbl_ChongZhi;
        private System.Windows.Forms.Label lbl_HelpCenter;
        private System.Windows.Forms.Label lbl_CurVersion;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_noticTime4;
        private System.Windows.Forms.LinkLabel lbl_notic4;
        private System.Windows.Forms.Label lbl_noticTime5;
        private System.Windows.Forms.LinkLabel lbl_notic5;
        private System.Windows.Forms.Label lbl_WebSite;
        private System.Windows.Forms.LinkLabel lbl_notic;
        private System.IO.FileSystemWatcher fileSystemWatcher1;
        private System.Windows.Forms.Label btn_accMgr;
        private System.Windows.Forms.Label label3;
        private ZBobb.AlphaBlendTextBox textBox2;
        private ZBobb.AlphaBlendTextBox textBox1;
        private System.Windows.Forms.Label btn_login;
        private System.Windows.Forms.Label btn_accRegisger;
        private System.Windows.Forms.Label btn_min;
        private System.Windows.Forms.Label btn_close;
        private System.Windows.Forms.Timer timer1;
    }
}