using System;
using System.Collections.Generic;
using System.Web;
using System.Net;
using System.IO;
namespace His.WebService.Utility
{
    public class FtpHelper
    {
        /// <summary>
        /// ftp方式上传 
        /// </summary>
        public static int UploadFtp(string filePath, string filename, string ftpServerIP, string ftpUserID, string ftpPassword)
        {
            FileInfo fileInf = new FileInfo(filePath + "\\" + filename);
            string uri = "ftp://" + ftpServerIP + "/" + fileInf.Name;
            FtpWebRequest reqFTP;
            // Create FtpWebRequest object from the Uri provided 
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + fileInf.Name));
            try
            {
                // Provide the WebPermission Credintials 
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);

                // By default KeepAlive is true, where the control connection is not closed 
                // after a command is executed. 
                reqFTP.KeepAlive = false;

                // Specify the command to be executed. 
                reqFTP.Method = WebRequestMethods.Ftp.UploadFile;

                // Specify the data transfer type. 
                reqFTP.UseBinary = true;

                // Notify the server about the size of the uploaded file 
                reqFTP.ContentLength = fileInf.Length;

                // The buffer size is set to 2kb 
                int buffLength = 2048;
                byte[] buff = new byte[buffLength];
                int contentLen;

                // Opens a file stream (System.IO.FileStream) to read the file to be uploaded 
                //FileStream fs = fileInf.OpenRead(); 
                FileStream fs = fileInf.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                // Stream to which the file to be upload is written 
                Stream strm = reqFTP.GetRequestStream();

                // Read from the file stream 2kb at a time 
                contentLen = fs.Read(buff, 0, buffLength);

                // Till Stream content ends 
                while (contentLen != 0)
                {
                    // Write Content from the file stream to the FTP Upload Stream 
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }

                // Close the file stream and the Request Stream 
                strm.Close();
                fs.Close();
                return 0;
            }
            catch (Exception ex)
            {
                reqFTP.Abort();
                //  Logging.WriteError(ex.Message + ex.StackTrace);
                return -2;
            }
        }

        public static FtpWebResponse GetFtpWebResponse(string fileName, string ftpServerIP, string ftpUserID, string ftpPassword)
        { 
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + fileName));
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.KeepAlive = false;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                return response;
            }
            
            catch (Exception ex)
            {
                // Logging.WriteError(ex.Message + ex.StackTrace);
                // System.Windows.Forms.MessageBox.Show(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// ftp方式下载 
        /// </summary>
        public static int DownloadFtp(string filePath, string fileName, string ftpServerIP, string ftpUserID, string ftpPassword)
        {
            FtpWebRequest reqFTP;
            try
            {
                //filePath = < <The full path where the file is to be created.>>, 
                //fileName = < <Name of the file to be created(Need not be the name of the file on FTP server).>> 
                FileStream outputStream = new FileStream(filePath + "\\" + fileName, FileMode.Create);

                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + fileName));
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.KeepAlive = false;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];

                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }

                ftpStream.Close();
                outputStream.Close();
                response.Close();
                return 0;
            }
            catch (Exception ex)
            {
                // Logging.WriteError(ex.Message + ex.StackTrace);
                // System.Windows.Forms.MessageBox.Show(ex.Message);
                return -2;
            }
        }
    }
}
