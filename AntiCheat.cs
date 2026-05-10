using System;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace desktoppusan
{
    public static class AntiCheat
    {
        private static readonly int MaxScorePerMinute = 300;
        private static DateTime lastScoreTime = DateTime.Now;
        private static int recentScore = 0;
        
        public static string GetDeviceId()
        {
            try
            {
                string mac = GetMacAddress();
                string drive = GetDriveSerial();
                string combined = $"{mac}_{drive}";
                return ComputeSha256Hash(combined);
            }
            catch { return Guid.NewGuid().ToString(); }
        }
        
        public static string GetIpHash()
        {
            string info = $"{Environment.MachineName}_{Environment.UserName}";
            return ComputeSha256Hash(info);
        }
        
        public static async Task<bool> CheckScoreValid(string username, int score)
        {
            DateTime now = DateTime.Now;
            if ((now - lastScoreTime).TotalSeconds < 60)
            {
                recentScore += score;
                if (recentScore > MaxScorePerMinute)
                    return false;
            }
            else
            {
                recentScore = score;
                lastScoreTime = now;
            }
            
            return true;
        }
        
        private static string GetMacAddress()
        {
            try
            {
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface nic in nics)
                {
                    if (nic.OperationalStatus == OperationalStatus.Up)
                    {
                        return nic.GetPhysicalAddress().ToString();
                    }
                }
            }
            catch { }
            return "unknown";
        }
        
        private static string GetDriveSerial()
        {
            try
            {
                System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
                foreach (System.IO.DriveInfo drive in drives)
                {
                    if (drive.Name == "C:\\" && drive.IsReady)
                    {
                        return drive.TotalSize.ToString();
                    }
                }
            }
            catch { }
            return "unknown";
        }
        
        private static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}