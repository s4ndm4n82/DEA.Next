using AppConfigReader;
using System.Net;
using System.Net.NetworkInformation;
using WriteLog;

namespace DEA.Next.HelperClasses.InternetLineChecker
{
    internal class InternetLineChecker
    {
        public static async Task<bool> InternetLineCheckerAsync()
        {
            WriteLogClass.WriteToLog(1, "Checking for active internet connection .....", 1);

            AppConfigReaderClass.AppSettingsRoot jsonData = AppConfigReaderClass.ReadAppDotConfig();
            IEnumerable<string> publicDns = jsonData.ProgramSettings.PublicDns;
            IEnumerable<string> checkUrls = jsonData.ProgramSettings.CheckUrls;

            if (!await GetIsInterfaceAvailableAsync())
            {
                return false;
            }

            if (!await CheckInternetHttpRequestAsync(checkUrls))
            {
                return await CheckInternetPingAsync(publicDns);
            }

            return true;
        }

        private static async Task<bool> GetIsInterfaceAvailableAsync()
        {
            try
            {
                return NetworkInterface.GetIsNetworkAvailable();
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"GetIsInterfaceAvailableAsync Error: {ex.Message}", 0);
                return false;
            }
        }

        private static async Task<bool> CheckInternetHttpRequestAsync(IEnumerable<string> checkUrls)
        {
            foreach (string url in checkUrls)
            {
                try
                {
                    HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
                    httpRequest.KeepAlive = false;
                    httpRequest.Timeout = 10000;

                    using HttpWebResponse httpResponse = (HttpWebResponse)await httpRequest.GetResponseAsync();

                    // Considered successful if the status code is in the range of 2xx
                    if ((int)httpResponse.StatusCode >= 200 && (int)httpResponse.StatusCode < 300)
                    {
                        return true;
                    }
                }
                catch (WebException ex)
                {
                    WriteLogClass.WriteToLog(0, $"CheckInternetHttpRequesAsync Error: {ex.Message}", 0);
                }
            }
            return false;
        }

        private static async Task<bool> CheckInternetPingAsync(IEnumerable<string> publicDns)
        {
            foreach (string dns in publicDns)
            {
                using Ping deaPing = new();
                try
                {
                    string pingHost = dns;
                    byte[] buffer = new byte[32];
                    int timeout = 1000;
                    PingReply pingReply = await deaPing.SendPingAsync(pingHost, timeout, buffer);

                    if (pingReply.Status == IPStatus.Success)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(0, $"CheckInternetPingAsync Error: {ex.Message}", 0);
                }
            }

            return false;
        }
    }
}
