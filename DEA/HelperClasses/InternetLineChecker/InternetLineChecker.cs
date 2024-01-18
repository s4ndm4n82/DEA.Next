using AppConfigReader;
using System.Net;
using System.Net.NetworkInformation;
using WriteLog;

namespace DEA.Next.HelperClasses.InternetLineChecker
{
    internal class InternetLineChecker
    {
        /// <summary>
        /// Try to find if the internet connection is working or not.
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> InternetLineCheckerAsync()
        {
            WriteLogClass.WriteToLog(1, "Checking for active internet connection .....", 1);

            AppConfigReaderClass.AppSettingsRoot jsonData = AppConfigReaderClass.ReadAppDotConfig();
            IEnumerable<string> publicDns = jsonData.ProgramSettings.PublicDns;
            IEnumerable<string> checkUrls = jsonData.ProgramSettings.CheckUrls;

            int maxRetry = jsonData.ProgramSettings.RetryLine;
            int currentRetry = 0;

            // Loops until the end of configured retrys.
            while (currentRetry < maxRetry)
            {
                currentRetry++;
                // Check if the interface is up or not.
                if (!await GetIsInterfaceAvailableAsync())
                {
                    WriteLogClass.WriteToLog(1, "Trying to reconnect ....", 1);
                    if (currentRetry >= maxRetry)
                    {                        
                        return false;
                    }

                    // Wait 2 second before retrying.
                    await Task.Delay(2000 * currentRetry);                    
                    continue;
                }

                // If interface is up HTTP request is used.
                if (!await CheckInternetHttpRequestAsync(checkUrls))
                {
                    // If the HTTP request fails, ping is used.
                    if (!await CheckInternetPingAsync(publicDns))
                    {
                        currentRetry++;
                        if (currentRetry >= maxRetry)
                        {
                            return false;
                        }

                        // Wait 2 second before retrying.
                        await Task.Delay(2000 * currentRetry);
                        continue;
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Use the built in .NET GetIsNetworkAvailable() function determine if the network interface is up or not.
        /// </summary>
        /// <returns>Return true if working.</returns>
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

        /// <summary>
        /// Use a HHTP web request to find if the internet connection is working or not.
        /// </summary>
        /// <param name="checkUrls">The list of urls to send the request to.</param>
        /// <returns>Return true if woring.</returns>
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

        /// <summary>
        /// Use a ping to find if the internet connection is working or not.
        /// </summary>
        /// <param name="publicDns">The list of DNS addresses to ping.</param>
        /// <returns>Return true if working</returns>
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
