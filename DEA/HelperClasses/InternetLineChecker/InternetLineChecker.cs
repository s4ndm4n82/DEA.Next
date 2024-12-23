using System.Net.NetworkInformation;
using AppConfigReader;
using WriteLog;

namespace DEA.Next.HelperClasses.InternetLineChecker;

internal static class InternetLineChecker
{
    /// <summary>
    ///     Checks for an active internet connection by pinging a list of public DNS servers.
    /// </summary>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a boolean indicating whether an
    ///     active internet connection was found.
    /// </returns>
    public static async Task<bool> InternetLineCheckerAsync()
    {
        // Log the start of the internet connection check.
        WriteLogClass.WriteToLog(1, "Checking for active internet connection .....", 1);

        // Read the application configuration.
        var jsonData = AppConfigReaderClass.ReadAppDotConfig();
        var publicDns = jsonData.ProgramSettings.PublicDns;

        // Get the maximum number of retry attempts from the configuration.
        var maxRetry = jsonData.ProgramSettings.RetryLine;
        var currentRetry = 0;

        // Loop until the maximum number of retry attempts is reached.
        while (currentRetry < maxRetry)
        {
            currentRetry++;

            // Check if the internet connection is active by pinging the public DNS servers.
            if (await CheckInternetPingAsync(publicDns)) return true;

            currentRetry++;
            if (currentRetry >= maxRetry) return false;

            // Wait for a period before retrying.
            await Task.Delay(2000 * currentRetry);
        }

        // Return false if no active internet connection was found after the maximum number of retry attempts.
        return false;
    }

    /// <summary>
    ///     Checks the internet connection by pinging a list of public DNS servers.
    /// </summary>
    /// <param name="publicDns">A list of public DNS server addresses to ping.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a boolean indicating whether an
    ///     active internet connection was found.
    /// </returns>
    private static async Task<bool> CheckInternetPingAsync(IEnumerable<string> publicDns)
    {
        // Iterate through each DNS server address in the list.
        foreach (var dns in publicDns)
        {
            using Ping deaPing = new();
            try
            {
                var pingHost = dns;
                var buffer = new byte[32];
                var timeout = 1000;

                // Send a ping request to the DNS server.
                var pingReply = await deaPing.SendPingAsync(pingHost, timeout, buffer);

                // If the ping is successful, return true.
                if (pingReply.Status == IPStatus.Success) return true;
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during the ping request.
                WriteLogClass.WriteToLog(0, $"CheckInternetPingAsync Error: {ex.Message}", 0);
            }
        }

        // Return false if no successful ping response was received.
        return false;
    }
}