using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using System.Globalization;
using System.Collections;
using System.Windows.Forms;

namespace BOSS刷新工具
{
    public class CFormat
    {
        internal const int LOCALE_SYSTEM_DEFAULT = 0x0800;
        internal const int LCMAP_SIMPLIFIED_CHINESE = 0x02000000;
        internal const int LCMAP_TRADITIONAL_CHINESE = 0x04000000;

        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int LCMapString(int Locale, int dwMapFlags, string lpSrcStr, int cchSrc, [Out] string lpDestStr, int cchDest);
        //繁体转简体
        public static string ToSimplified(string source)
        {
            String target = new String(' ', source.Length);
            int ret = LCMapString(LOCALE_SYSTEM_DEFAULT, LCMAP_SIMPLIFIED_CHINESE, source, source.Length, target, source.Length);
            return target;
        }
        //简体转繁体
        public static string ToTraditional(string source)
        {
            String target = new String(' ', source.Length);
            int ret = LCMapString(LOCALE_SYSTEM_DEFAULT, LCMAP_TRADITIONAL_CHINESE, source, source.Length, target, source.Length);
            return target;
        }

        public static string PureString(string source)
        {
            String target = source.Split('/')[0].Replace("\t", "").Replace(" ", "");
            return target;
        }

        public static string GameStrToSimpleCN(string src)
        {
            byte[] utf8bytes = System.Text.Encoding.Default.GetBytes(src);
            string temp = System.Text.Encoding.GetEncoding(950).GetString(utf8bytes);
            temp = CFormat.ToSimplified(temp);

            return temp;
        }

        public static string RemoveXZeroStr(string src)
        {
            string des = Convert.ToInt32(src).ToString();
            return des;
        }

        public static bool IsNumber(string strNumber)
        {
            try
            {
                int var1 = Convert.ToInt32(strNumber);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static int StringLength(string src)
        {
            return src.Length;
        }
    }
    public class CIniCtrlComm
    {
        private string iniFile;
        public static System.Text.Encoding Encoding = System.Text.Encoding.UTF8;
        public CIniCtrlComm()
        {

        }
        public CIniCtrlComm(string file)
        {
            this.iniFile = file;
        }
        /// <summary>
        /// 批量读取键值对
        /// </summary>
        /// <returns>返回INI配置结构体列表,单独结构可以通过索引获取或设置</returns>
        public System.Collections.Generic.List<IniStruct> ReadValues()
        {
            return ReadValues(this.iniFile);
        }
        public string ReadValue(string key, string section)
        {
            string comments = "";
            return ReadValue(this.iniFile, key, section, ref comments);
        }
        public string ReadValue(string key, string section, ref string comments)
        {
            if (string.IsNullOrEmpty(this.iniFile)) throw new System.Exception("没有设置文件路径");
            return ReadValue(this.iniFile, key, section, ref comments);
        }
        public static string ReadValue(string file, string key, string section)
        {
            string comments = "";
            return ReadValue(file, key, section, ref comments);
        }
        private static string GetText(string file)
        {
            string content = File.ReadAllText(file);
            if (content.Contains("�"))
            {
                Encoding = System.Text.Encoding.GetEncoding("GBK");
                content = File.ReadAllText(file, System.Text.Encoding.GetEncoding("GBK"));
            }
            return content;
        }
        public static string ReadValue(string file, string key, string section, ref string comments)
        {
            string valueText = "";
            string content = GetText(file);
            if (!string.IsNullOrEmpty(section)) //首先遍历节点
            {
                MatchCollection matches = new Regex(@"\[\s*(?'section'[^\[\]\s]+)\s*\]").Matches(content);
                if (matches.Count <= 0) return "";
                Match currMatch = null;
                Match tailMatch = null;
                foreach (Match match in matches)
                {
                    string match_section = match.Groups["section"].Value;
                    if (match_section.ToLower() == section.ToLower())
                    {
                        currMatch = match;
                        continue;
                    }
                    else if (currMatch != null)
                    {
                        tailMatch = match;
                        break;
                    }

                }
                valueText = content.Substring(currMatch.Index + currMatch.Length, (tailMatch != null ? tailMatch.Index : content.Length) - currMatch.Index - currMatch.Length);//截取有效值域


            }
            else
                valueText = content;
            string[] lines = valueText.Split(new string[] { "\r\n" }, System.StringSplitOptions.None);
            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line) || line == "\r\n" || line.Contains("["))
                    continue;
                string valueLine = line;
                if (line.Contains(";"))
                {
                    string[] seqPairs = line.Split(';');
                    if (seqPairs.Length > 1)
                        comments = seqPairs[1].Trim();
                    valueLine = seqPairs[0];
                }
                string[] keyValuePairs = valueLine.Split('=');
                string line_key = keyValuePairs[0];
                string line_value = "";
                if (keyValuePairs.Length > 1)
                {
                    line_value = keyValuePairs[1];
                }
                if (key.ToLower().Trim() == line_key.ToLower().Trim())
                {
                    return line_value;
                }
            }
            return "";
        }
        public static System.Collections.Generic.List<IniStruct> ReadValues(string file)
        {
            System.Collections.Generic.List<IniStruct> iniStructList = new System.Collections.Generic.List<IniStruct>();
            string content = GetText(file);
            System.Text.RegularExpressions.MatchCollection matches = new System.Text.RegularExpressions.Regex(@"\[\s*(?'section'[^\[\]\s]+)\s*\](?'valueContent'[^\[\]]*)").Matches(content);
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                IniStruct iniStruct = new IniStruct();
                string match_section = match.Groups["section"].Value;
                string match_value = match.Groups["valueContent"].Value;
                iniStruct.Section = match_section;

                string[] lines = match_value.Split(new string[] { "\r\n" }, System.StringSplitOptions.None);
                foreach (string line in lines)
                {
                    if (string.IsNullOrEmpty(line) || line == "\r\n" || line.Contains("["))
                        continue;
                    string comments = "";//注释
                    string valueLine = line;
                    if (line.Contains(";"))
                    {
                        string[] seqPairs = line.Split(';');
                        if (seqPairs.Length > 1)
                            comments = seqPairs[1].Trim();
                        valueLine = seqPairs[0];
                    }
                    string[] keyValuePairs = valueLine.Split('=');
                    string line_key = keyValuePairs[0];
                    string line_value = "";
                    if (keyValuePairs.Length > 1)
                    {
                        line_value = keyValuePairs[1];
                    }
                    iniStruct.Add(line_key, line_value, comments);
                }
                iniStructList.Add(iniStruct);
            }

            return iniStructList;
        }
        public void Write(string section, string key, string value)
        {
            Write(section, key, value, null);
        }
        public void Write(string section, string key, string value, string comment)
        {
            Write(this.iniFile, section, key, value, comment);
        }
        public static void Write(string file, string section, string key, string value, string comment)
        {
            bool isModified = false;
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            string content = GetText(file);
            System.Text.StringBuilder newValueContent = new System.Text.StringBuilder();
            #region 写入了节点
            if (!string.IsNullOrEmpty(section))
            {
                string pattern = string.Format(@"\[\s*{0}\s*\](?'valueContent'[^\[\]]*)", section);
                MatchCollection matches = new Regex(pattern).Matches(content);
                if (matches.Count <= 0)
                {
                    stringBuilder.AppendLine(string.Format("[{0}]", section)); //检查节点是否存在
                    stringBuilder.AppendLine(string.Format("{0}={1}{2}", key, value, !string.IsNullOrEmpty(comment) ? (";" + comment) : ""));
                    stringBuilder.AppendLine(content);
                    isModified = true;
                }
                else
                {
                    Match match = matches[0];
                    string valueContent = match.Groups["valueContent"].Value;
                    string[] lines = valueContent.Split(new string[] { "\r\n" }, System.StringSplitOptions.None);

                    newValueContent.AppendLine(string.Format("[{0}]", section));
                    foreach (string line in lines)
                    {
                        if (string.IsNullOrEmpty(line) || line == "\r\n" || line.Contains("["))
                        {
                            continue;
                        }

                        string valueLine = line;
                        string comments = "";
                        if (line.Contains(";"))
                        {
                            string[] seqPairs = line.Split(';');
                            if (seqPairs.Length > 1)
                                comments = seqPairs[1].Trim();
                            valueLine = seqPairs[0];
                        }
                        string[] keyValuePairs = valueLine.Split('=');
                        string line_key = keyValuePairs[0];
                        string line_value = "";
                        if (keyValuePairs.Length > 1)
                        {
                            line_value = keyValuePairs[1];
                        }
                        if (key.ToLower().Trim() == line_key.ToLower().Trim())
                        {
                            isModified = true;
                            newValueContent.AppendLine(string.Format("{0}={1}{2}", key, value, !string.IsNullOrEmpty(comment) ? (";" + comment) : ""));
                        }
                        else
                        {
                            newValueContent.AppendLine(line);
                        }


                    }
                    if (!isModified)
                        newValueContent.AppendLine(string.Format("{0}={1}{2}", key, value, !string.IsNullOrEmpty(comment) ? (";" + comment) : ""));
                    string newVal = newValueContent.ToString();
                    content = content.Replace(match.Value, newVal);
                    stringBuilder.Append(content);

                }
            }
            #endregion
            #region 没有指明节点
            else
            {
                string valueText = "";
                //如果节点为空
                MatchCollection matches = new Regex(@"\[\s*(?'section'[^\[\]\s]+)\s*\](?'valueContent'[^\[\]]*)").Matches(content);
                if (matches.Count > 0)
                {
                    valueText = matches[0].Index > 0 ? content.Substring(0, matches[0].Index) : "";
                    string[] lines = valueText.Split(new string[] { "\r\n" }, System.StringSplitOptions.None);
                    foreach (string line in lines)
                    {
                        if (string.IsNullOrEmpty(line) || line == "\r\n" || line.Contains("["))
                        {
                            continue;
                        }

                        string valueLine = line;
                        string comments = "";
                        if (line.Contains(";"))
                        {
                            string[] seqPairs = line.Split(';');
                            if (seqPairs.Length > 1)
                                comments = seqPairs[1].Trim();
                            valueLine = seqPairs[0];
                        }
                        string[] keyValuePairs = valueLine.Split('=');
                        string line_key = keyValuePairs[0];
                        string line_value = "";
                        if (keyValuePairs.Length > 1)
                        {
                            line_value = keyValuePairs[1];
                        }
                        if (key.ToLower().Trim() == line_key.ToLower().Trim())
                        {
                            isModified = true;
                            newValueContent.AppendLine(string.Format("{0}={1}{2}", key, value, !string.IsNullOrEmpty(comment) ? (";" + comment) : ""));
                        }
                        else
                        {
                            newValueContent.AppendLine(line);
                        }


                    }
                    if (!isModified)
                        newValueContent.AppendLine(string.Format("{0}={1}{2}", key, value, !string.IsNullOrEmpty(comment) ? (";" + comment) : ""));
                    string newVal = newValueContent.ToString();
                    content = content.Replace(valueText, newVal);
                    stringBuilder.Append(content);
                }
                else
                {
                    stringBuilder.AppendLine(string.Format("{0}={1}{2}", key, value, !string.IsNullOrEmpty(comment) ? (";" + comment) : ""));
                }
            }
            #endregion
            System.IO.File.WriteAllText(file, stringBuilder.ToString(), Encoding);
        }
    }
    public class IniStruct : System.Collections.IEnumerable
    {

        private System.Collections.Generic.List<string> _commentList;
        public IniStruct()
        {
            this._keyValuePairs = new System.Collections.Generic.SortedList<string, string>();
            _commentList = new System.Collections.Generic.List<string>();
        }
        public string GetComment(string key)
        {
            if (this._keyValuePairs.ContainsKey(key))
            {
                int index = this._keyValuePairs.IndexOfKey(key);
                return this._commentList[index];
            }
            return "";
        }
        public string this[int index]
        {
            get
            {
                if (this._keyValuePairs.Count > index)
                    return this._keyValuePairs.Values[index];
                else return "";
            }
            set
            {
                if (this._keyValuePairs.Count > index)
                    this._keyValuePairs.Values[index] = value;
            }
        }
        public string this[string key]
        {
            get
            {
                if (this._keyValuePairs.ContainsKey(key))
                    return this._keyValuePairs[key];
                else return "";
            }
            set
            {
                if (this._keyValuePairs.ContainsKey(key))
                    this._keyValuePairs[key] = value;
            }
        }
        public string Section { get; set; }
        private System.Collections.Generic.SortedList<string, string> _keyValuePairs;
        public void Add(string key, string value, string commont)
        {
            this._keyValuePairs.Add(key, value);
            this._commentList.Add(commont);
        }
        public override string ToString()
        {
            return string.Format("{0}", this.Section);
        }

        public bool ContainKey(string key)
        {
            return this._keyValuePairs.ContainsKey(key);
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            return this._keyValuePairs.GetEnumerator();
        }
    }

    public class CIniCtrl
    {
        #region API函数声明

        [System.Runtime.InteropServices.DllImport("kernel32")]//返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(string section, string key,
            string val, string filePath);

        [System.Runtime.InteropServices.DllImport("kernel32")]//返回取得字符串缓冲区的长度
        private static extern long GetPrivateProfileString(string section, string key,
            string def, StringBuilder retVal, int size, string filePath);


        #endregion

        #region 读Ini文件

        public static string ReadIniData(string Section, string Key, string NoText, string iniFilePath)
        {
            if (File.Exists(iniFilePath))
            {
                StringBuilder temp = new StringBuilder(1024);
                GetPrivateProfileString(Section, Key, NoText, temp, 1024, iniFilePath);
                return temp.ToString();
            }
            else
            {
                return String.Empty;
            }
        }

        #endregion

        #region 写Ini文件

        public static bool WriteIniData(string Section, string Key, string Value, string iniFilePath)
        {
            if (!File.Exists(iniFilePath))
            {
                File.Create(iniFilePath).Close();
            }

            long OpStation = WritePrivateProfileString(Section, Key, Value, iniFilePath);
            if (OpStation == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion
    }

    public class CStructBytesFormat
    {
        //将Byte转换为结构体类型
        //*
        public static byte[] StructToBytes(object structObj, int size)
        {
            byte[] bytes = new byte[size];
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将结构体拷到分配好的内存空间
            Marshal.StructureToPtr(structObj, structPtr, false);
            //从内存空间拷贝到byte 数组
            Marshal.Copy(structPtr, bytes, 0, size);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            return bytes;

        }
        
        public static byte[] StructToBytes<T>(T obj)
        {
            int size = Marshal.SizeOf(typeof(T));
            IntPtr bufferPtr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(obj, bufferPtr, false);
                byte[] bytes = new byte[size];
                Marshal.Copy(bufferPtr, bytes, 0, size);

                return bytes;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in StructToBytes ! " + ex.Message);
            }
            finally
            {
                Marshal.FreeHGlobal(bufferPtr);
            }
        }

        //字节流转换成结构体

        public static T BytesToStruct<T>(byte[] bytes, int startIndex = 0)
        {
            if (bytes == null) return default(T);
            if (bytes.Length <= 0) return default(T);
            int objLength = Marshal.SizeOf(typeof(T));
            IntPtr bufferPtr = Marshal.AllocHGlobal(objLength);
            try//struct_bytes转换
            {
                Marshal.Copy(bytes, startIndex, bufferPtr, objLength);
                return (T)Marshal.PtrToStructure(bufferPtr, typeof(T));
            }
            catch (Exception ex)
            {
                throw new Exception("Error in BytesToStruct ! " + ex.Message);
            }
            finally
            {
                Marshal.FreeHGlobal(bufferPtr);
            }
        }
    }


    public class CSecurity
    {
        //默认密钥向量 
        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        /// <summary> 
        /// DES加密字符串 
        /// </summary> 
        /// <param name="encryptString">待加密的字符串</param> 
        /// <param name="encryptKey">加密密钥,要求为8位</param> 
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns> 
        public static string EncryptDES(string encryptString, string encryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return encryptString;
            }

        }

        /// <summary> 
        /// DES解密字符串 
        /// </summary> 
        /// <param name="decryptString">待解密的字符串</param> 
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param> 
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns> 
        public static string DecryptDES(string decryptString, string decryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return decryptString;
            }
        }
    }

    public class Md5Helper
    {
        public static string MD5ToString(String argString)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.Default.GetBytes(argString);
            byte[] result = md5.ComputeHash(data);
            String strReturn = String.Empty;
            for (int i = 0; i < result.Length; i++)
                strReturn += result[i].ToString("x").PadLeft(2, '0');
            return strReturn;
        }
    }

    public class ListViewColumnSorter : IComparer
    {
        private int ColumnToSort;// 指定按照哪个列排序      
        private System.Windows.Forms.SortOrder OrderOfSort;// 指定排序的方式               
        private CaseInsensitiveComparer ObjectCompare;// 声明CaseInsensitiveComparer类对象，
        public ListViewColumnSorter()// 构造函数
        {
            ColumnToSort = 0;// 默认按第一列排序            
            OrderOfSort = System.Windows.Forms.SortOrder.None;// 排序方式为不排序            
            ObjectCompare = new CaseInsensitiveComparer();// 初始化CaseInsensitiveComparer类对象
        }
        // 重写IComparer接口.        
        // <returns>比较的结果.如果相等返回0，如果x大于y返回1，如果x小于y返回-1</returns>
        public int Compare(object x, object y)
        {
            int compareResult;
            ListViewItem listviewX, listviewY;
            // 将比较对象转换为ListViewItem对象
            listviewX = (ListViewItem)x;
            listviewY = (ListViewItem)y;
            // 比较
            //compareResult = ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Text, listviewY.SubItems[ColumnToSort].Text);
            compareResult = ObjectCompare.Compare(int.Parse(listviewX.SubItems[ColumnToSort].Text), int.Parse(listviewY.SubItems[ColumnToSort].Text));
            // 根据上面的比较结果返回正确的比较结果
            if (OrderOfSort == System.Windows.Forms.SortOrder.Ascending)
            {   // 因为是正序排序，所以直接返回结果
                return compareResult;
            }
            else if (OrderOfSort == System.Windows.Forms.SortOrder.Descending)
            {  // 如果是反序排序，所以要取负值再返回
                return (-compareResult);
            }
            else
            {
                // 如果相等返回0
                return 0;
            }
        }

        /// 获取或设置按照哪一列排序.        
        public int SortColumn
        {
            set
            {
                ColumnToSort = value;
            }
            get
            {
                return ColumnToSort;
            }
        }
        /// 获取或设置排序方式.    
        public System.Windows.Forms.SortOrder Order
        {
            set
            {
                OrderOfSort = value;
            }
            get
            {
                return OrderOfSort;
            }
        }
    }
}
