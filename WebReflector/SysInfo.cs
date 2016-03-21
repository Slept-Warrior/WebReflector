using System;
using System.IO;
using System.Management;
using System.Net;

namespace WebReflector
{
    class SysInfo
    {
        public static string GetComputerName()
        {
            string str = "";
            ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                str = mo["Caption"].ToString();
            }
            return str;
        }

        public static string GetOSname()
        {
            string str = "";
            ManagementClass mc = new ManagementClass("Win32_OperatingSystem");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                str = mo["Caption"].ToString();
            }
            return str;
        }

        public static string GetInnerIPAddress()
        {
            string str = "";
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if ((bool)mo["IPEnabled"] == true)
                {
                    //st=mo["IpAddress"].ToString(); 
                    System.Array ar;
                    ar = (System.Array)(mo.Properties["IpAddress"].Value);
                    str = ar.GetValue(0).ToString();
                    break;
                }
            }
            return str;
        }

        public static string GetHardDiskID()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");
                String strHardDiskID = null;
                foreach (ManagementObject mo in searcher.Get())
                {
                    strHardDiskID = mo["SerialNumber"].ToString().Trim();
                    break;
                }
                return strHardDiskID;
            }
            catch
            {
                return "";
            }
        }

        public static string GetOutterIPAddress()
        {
            string str = "";
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    WebRequest wr = WebRequest.Create("http://cnbrony.com/ip.php");
                    Stream s = wr.GetResponse().GetResponseStream();
                    StreamReader sr = new StreamReader(s, System.Text.Encoding.Default);
                    str = sr.ReadToEnd(); //读取网站的数据
                    /*
                    int start = all.IndexOf("您的IP地址是：[") + 9;
                    int end = all.IndexOf("]", start);
                    str = all.Substring(start, end - start);*/
                    sr.Close();
                    s.Close();
                    if ("" != str) break;
                }
                catch(Exception ex)
                {
                    ex.ToString();
                }
            }
            return str;
        }

        public static string GetPhysicalMemory()
        {
            double str = 0;
            ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                str = double.Parse(mo["TotalPhysicalMemory"].ToString());
            }
            return Math.Round(str / 1024 / 1024/1024, 1).ToString();
        }

    }
}
