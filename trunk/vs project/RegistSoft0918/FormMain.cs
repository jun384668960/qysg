using System;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using Common;

namespace RegistSoft0918
{
    public partial class FormMain : Form
    {
        private string encryptComputer = string.Empty;
        private bool isRegist = false;
        private const int timeCount = 30;
        public FormMain()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }
        private void FormMain_Load(object sender, EventArgs e)
        {
            string computer = ComputerInfo.GetComputerInfo();
            encryptComputer = new EncryptionHelper().EncryptString(computer);
            if (CheckRegist() == true)
            {
                lbRegistInfo.Text = "已注册";
            }
            else
            {
                lbRegistInfo.Text = "待注册，运行十分钟后自动关闭";
                RegistFileHelper.WriteComputerInfoFile(encryptComputer);
                TryRunForm();
            }
        }
        /// <summary>
        /// 试运行窗口
        /// </summary>
        private void TryRunForm()
        {
            Thread threadClose = new Thread(CloseForm);
            threadClose.IsBackground = true;
            threadClose.Start();
        }
        private bool CheckRegist()
        {
            EncryptionHelper helper = new EncryptionHelper();
            //string md5key = helper.GetMD5String(encryptComputer);
            string md5key = helper.MD5Encrypt(encryptComputer);
            return CheckRegistData(md5key);
        }
        private bool CheckRegistData(string key)
        {
            if (RegistFileHelper.ExistRegistInfofile() == false)
            {
                isRegist = false;
                return false;
            }
            else
            {
                string info = RegistFileHelper.ReadRegistFile();
                var helper = new EncryptionHelper(EncryptionKeyEnum.KeyB);
                string registData = helper.DecryptString(info);
                string _info = helper.MD5Decrypt(registData);
                //还原info
                _info = new EncryptionHelper().DecryptString(_info);
                string _registData = _info.Split(new string[] { "0XDDFF2B" }, StringSplitOptions.RemoveEmptyEntries)[0];
                string _time = _info.Split(new string[] { "0XDDFF2B" }, StringSplitOptions.RemoveEmptyEntries)[1];
                registData = new EncryptionHelper().EncryptString(_registData);
                registData = helper.MD5Encrypt(registData);

                //时间比较
                DateTime dtEnd = Convert.ToDateTime(_time);
                if (DateTime.Compare(DateTime.Now, dtEnd) > 0)
                {
                    //过期
                    isRegist = false;
                    return false;
                }

                if (key == registData)
                {
                    isRegist = true;
                    return true;
                }
                else
                {
                    isRegist = false;
                    return false;
                }
            }
        }
        private void CloseForm()
        {
            int count = 0;
            while (count < timeCount && isRegist == false)
            {
                if (isRegist == true)
                {
                    return;
                }
                Thread.Sleep(1 * 1000);
                count++;
            }
            if (isRegist == true)
            {
                return;
            }
            else
            {
                this.Close();
            }
        }

        private void btnRegist_Click(object sender, EventArgs e)
        {
            if (lbRegistInfo.Text == "已注册")
            {
                MessageBox.Show("已经注册～");
                return;
            }
            string fileName = string.Empty;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog.FileName;
            }
            else
            {
                return;
            }
            string localFileName = string.Concat(
                Environment.CurrentDirectory,
                Path.DirectorySeparatorChar,
                RegistFileHelper.RegistInfofile);
            if (fileName != localFileName)
                File.Copy(fileName, localFileName, true);

            if (CheckRegist() == true)
            {
                lbRegistInfo.Text = "已注册";
                MessageBox.Show("注册成功～");
            }
        }
    }
}
