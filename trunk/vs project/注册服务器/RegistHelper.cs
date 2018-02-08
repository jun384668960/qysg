using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class RegistHelper
    {
        static public bool CheckRegist(out string endTime)
        {
            string computer = ComputerInfo.GetComputerInfo();
            string encryptComputer = new EncryptionHelper().EncryptString(computer);
            RegistFileHelper.WriteComputerInfoFile(encryptComputer);

            return CheckRegist(encryptComputer, out endTime);
        }

        static private bool CheckRegist(string encryptComputer, out string endTime)
        {
            EncryptionHelper helper = new EncryptionHelper();
            //string md5key = helper.GetMD5String(encryptComputer);
            string md5key = helper.MD5Encrypt(encryptComputer);
            return CheckRegistData(md5key, out endTime);
        }

        static private bool CheckRegistData(string key, out string endTime)
        {
            endTime = "";
            if (RegistFileHelper.ExistRegistInfofile() == false)
            {
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
                endTime = _info.Split(new string[] { "0XDDFF2B" }, StringSplitOptions.RemoveEmptyEntries)[1];
                registData = new EncryptionHelper().EncryptString(_registData);
                registData = helper.MD5Encrypt(registData);

                //时间比较
                DateTime dtEnd = Convert.ToDateTime(endTime);
                if (DateTime.Compare(DateTime.Now, dtEnd) > 0)
                {
                    //过期
                    return false;
                }

                if (key == registData)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
