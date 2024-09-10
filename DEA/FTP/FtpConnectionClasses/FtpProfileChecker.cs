using static DEA.Next.FTP.FtpConnectionClasses.FtpProfilesSelector;
using WriteLog;

namespace DEA.Next.FTP.FtpConnectionClasses
{
    internal class FtpProfileChecker
    {
        /// <summary>
        /// Validate if the FTP profile exists.
        /// </summary>
        /// <param name="ftpProfile">FTP profile name.</param>
        /// <returns>Return true or false.</returns>
        public static async Task<bool> CheckProfileExistsAsync(string ftpProfile)
        {
            // List of valid FTP profiles
            HashSet<string> validProfiles = new()
            {
                FtpProfileList.ProfilePxe,
                FtpProfileList.ProfileEpe,
                FtpProfileList.ProfileFsv
            };

            // Check if the FTP profile is valid
            if (!validProfiles.Contains(ftpProfile))
            {
                WriteLogClass.WriteToLog(1, $"Invalid FTP Profile: {ftpProfile}", 3);
                return false;
            }

            return true;
        }
    }
}
