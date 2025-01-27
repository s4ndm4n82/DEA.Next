using System.Net;
using AppConfigReader;
using WriteLog;

namespace DEA.Next.HelperClasses.InternetLineChecker;

internal static class InternetLineChecker
{
    private static readonly HttpClient HttpClient = new();

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
        var publicDns = jsonData.ProgramSettings.TestUrls;

        // Get the maximum number of retry attempts from the configuration.
        var maxRetry = jsonData.ProgramSettings.RetryLine;
        var currentRetry = 0;

        // Loop until the maximum number of retry attempts is reached.
        while (currentRetry < maxRetry)
        {
            currentRetry++;

            // Check if the internet connection is active by pinging the public DNS servers.
            if (await CheckInternetHttpAsync(publicDns)) return true;

            currentRetry++;
            if (currentRetry >= maxRetry) return false;

            // Wait for a period before retrying.
            await Task.Delay(2000 * currentRetry);
        }

        // Return false if no active internet connection was found after the maximum number of retry attempts.
        return false;
    }

    /// <summary>
    ///     Checks the internet connection by using a GET request to a list of public DNS servers.
    /// </summary>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a boolean indicating whether an
    ///     active internet connection was found.
    /// </returns>
    private static async Task<bool> CheckInternetHttpAsync(IEnumerable<string> publicUrl)
    {
        // Iterate through each DNS server address in the list.
        foreach (var url in publicUrl)
            try
            {
                var response = await HttpClient.GetAsync(url);
                if (response.StatusCode != HttpStatusCode.OK) continue;
                return true;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0,
                    $"Exception at checking internet connection: {ex.Message}",
                    0);
                return false;
            }

        return false;
    }
}