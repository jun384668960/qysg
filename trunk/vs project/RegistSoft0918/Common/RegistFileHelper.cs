using System;
using System.IO;

namespace Common
{
    public class RegistFileHelper
    {
        public static string ComputerInfofile = "ComputerInfo.key";
        public static string RegistInfofile = "RegistInfo.key";
        public static void WriteRegistFile(string info)
        {
            WriteFile(info, RegistInfofile);
        }
        public static void WriteComputerInfoFile(string info)
        {
            WriteFile(info, ComputerInfofile);
        }
        public static string ReadRegistFile()
        {
            return ReadFile(RegistInfofile);
        }
        public static string ReadComputerInfoFile()
        {
            return ReadFile(ComputerInfofile);
        }
        public static bool ExistComputerInfofile()
        {
            return File.Exists(ComputerInfofile);
        }
        public static bool ExistRegistInfofile()
        {
            return File.Exists(RegistInfofile);
        }
        private static void WriteFile(string info, string fileName)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(fileName, false))
                {
                    sw.Write(info);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
            }
        }
        private static string ReadFile(string fileName)
        {
            string info = string.Empty;
            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    info = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (Exception ex)
            {
            }
            return info;
        }
    }
}
