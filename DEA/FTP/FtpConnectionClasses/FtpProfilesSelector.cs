using FluentFTP;

namespace DEA.Next.FTP.FtpConnectionClasses
{
    internal class FtpProfilesSelector
    {
        /// <summary>
        /// FTP settings configuration class.
        /// </summary>
        public class FtpConfigurations
        {
            public FtpDataConnectionType DataConnectionType { get; set; }
            public FtpEncryptionMode EncryptionMode { get; set; }
            public bool ValidateCertificate { get; set; }
        }

        /// <summary>
        /// Contais valid FTP profile names list.
        /// </summary>
        public class FtpProfileList
        {
            public const string ProfilePxe = "profilepxe";
            public const string ProfileEpe = "profileepe";
            public const string ProfileFsv = "profilefsv";
        }

        /// <summary>
        /// Selectes the proper FTP profile according to the profile name.
        /// </summary>
        /// <param name="ftpProfile">FTP Profile name.</param>
        /// <returns>The ftp congiurations needed to make the connection</returns>
        public static async Task<FtpConfigurations> GetFtpProfiles(string ftpProfile)
        {
            // Dictionary of FTP profiles and their configurations
            Dictionary<string, FtpConfigurations> profileConfigurations = new()
            {
                { FtpProfileList.ProfilePxe, new FtpConfigurations{DataConnectionType = FtpDataConnectionType.PASVEX, EncryptionMode = FtpEncryptionMode.None, ValidateCertificate = false} },
                { FtpProfileList.ProfileEpe, new FtpConfigurations{DataConnectionType = FtpDataConnectionType.AutoActive, EncryptionMode = FtpEncryptionMode.Auto, ValidateCertificate = false } },
                { FtpProfileList.ProfileFsv, new FtpConfigurations{DataConnectionType = FtpDataConnectionType.AutoPassive, EncryptionMode = FtpEncryptionMode.Explicit, ValidateCertificate = true} }
            };

            // Default FTP profile
            FtpConfigurations defaultConfig = new()
            {
                DataConnectionType = FtpDataConnectionType.AutoPassive,
                EncryptionMode = FtpEncryptionMode.Auto,
                ValidateCertificate = false
            };

            // Get FTP profile
            if (profileConfigurations.TryGetValue(ftpProfile, out FtpConfigurations config))
            {
                // Return FTP profile
                return config;
            }

            // Return default FTP profile
            return defaultConfig;
        }
    }
}