using FluentFTP;

namespace DEA.Next.FTP.FtpConnectionClasses
{
    internal class FtpProfilesSelector
    {
        public class FtpConfigurations
        {
            public FtpDataConnectionType DataConnectionType { get; set; }
            public FtpEncryptionMode EncryptionMode { get; set; }
            public bool ValidateCertificate { get; set; }
        }

        public class FtpProfileList
        {
            public const string ProfilePxe = "profilepxe";
            public const string ProfileEpe = "profileepe";
            public const string ProfileFsv = "profilefsv";
        }

        public static async Task<FtpConfigurations> GetFtpProfiles(string ftpProfile)
        {
            Dictionary<string, FtpConfigurations> profileConfigurations = new()
            {
                { FtpProfileList.ProfilePxe, new FtpConfigurations{DataConnectionType = FtpDataConnectionType.PASVEX, EncryptionMode = FtpEncryptionMode.None, ValidateCertificate = false} },
                { FtpProfileList.ProfileEpe, new FtpConfigurations{DataConnectionType = FtpDataConnectionType.AutoActive, EncryptionMode = FtpEncryptionMode.Auto, ValidateCertificate = false } },
                { FtpProfileList.ProfileFsv, new FtpConfigurations{DataConnectionType = FtpDataConnectionType.AutoPassive, EncryptionMode = FtpEncryptionMode.Explicit, ValidateCertificate = true} }
            };

            FtpConfigurations defaultConfig = new()
            {
                DataConnectionType = FtpDataConnectionType.AutoPassive,
                EncryptionMode = FtpEncryptionMode.Auto,
                ValidateCertificate = false
            };

            if (profileConfigurations.TryGetValue(ftpProfile, out FtpConfigurations config))
            {
                return config;
            }

            return defaultConfig;
        }
    }
}