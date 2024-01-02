using FtpFunctions;
using GraphHelper;
using ProcessStatusMessageSetter;
using UserConfigSetterClass;
using WriteLog;

namespace ProcessSartupFunctions
{
    internal class ProcessStartupFunctionsClass
    {
        readonly UserConfigSetterClass.UserConfigSetter.CustomerDetailsObject jsonDataObject;

        public ProcessStartupFunctionsClass()
        {
            jsonDataObject = UserConfigSetterClass.UserConfigSetter.ReadUserDotConfigAsync<UserConfigSetterClass.UserConfigSetter.CustomerDetailsObject>().Result;
        }

        public static async Task StartupProcess()
        {
            int ftpReturnCode = 0;
            int emailReturnCode = 0;

            ProcessStartupFunctionsClass jsonData = new();
            UserConfigSetterClass.UserConfigSetter.Customerdetail[] jsonCustomerData = jsonData.jsonDataObject.CustomerDetails;

            IEnumerable<UserConfigSetterClass.UserConfigSetter.Customerdetail> ftpClients = jsonCustomerData.Where(ftpc => ftpc.FileDeliveryMethod!.ToLower() == ConfigSettingStrings.ftp);
            IEnumerable<UserConfigSetterClass.UserConfigSetter.Customerdetail> emailClients = jsonCustomerData.Where(emailc => emailc.FileDeliveryMethod!.ToLower() == ConfigSettingStrings.email);

            if (ftpClients.Any())
            {
               ftpReturnCode = await StartFtpDownload(ftpClients);
            }

            if (emailClients.Any())
            {
                emailReturnCode = await StartEmailDownload(emailClients);
            }

            WriteLastStatusMessage(emailReturnCode, ftpReturnCode);
        }

        private static async Task<int> StartFtpDownload(IEnumerable<UserConfigSetterClass.UserConfigSetter.Customerdetail> ftpClients)
        {
            int ftpResult = 0;

            foreach (var ftpClient in ftpClients)
            {
                if ((ftpClient.FtpDetails!.FtpType!.ToLower() == ConfigSettingStrings.ftp || ftpClient.FtpDetails!.FtpType!.ToLower() == ConfigSettingStrings.ftps) && ftpClient.CustomerStatus == 1)
                {
                    ftpResult = await FtpFunctionsClass.GetFtpFiles(ftpClient.Id);
                }
                else
                {
                    // Awating to be implimented. Will be added when needed.
                    //SftpFunctionsClass.GetSftpFiles(ftpClient.id);
                }
            }

            return ftpResult;
        }

        private static async Task<int> StartEmailDownload(IEnumerable<UserConfigSetterClass.UserConfigSetter.Customerdetail> emailClients)
        {
            int emailResult = 0;

            foreach (var emailClient in emailClients)
            {
                if (emailClient.CustomerStatus == 1)
                {
                    emailResult = await GraphHelperClass.InitializGetAttachment(emailClient.Id);
                }
            }
            return emailResult;
        }

        private static void WriteLastStatusMessage(int emailResultStatus, int ftpResultStatus)
        {
            WriteLogClass.WriteToLog(ProcessStatusMessageSetterClass.SetMessageTypeMain(emailResultStatus, ftpResultStatus), $"{ProcessStatusMessageSetterClass.SetProcessStatusMain(emailResultStatus, ftpResultStatus)}\n", 1);
        }

        private static class ConfigSettingStrings
        {
            public const string email = "email";
            public const string ftp = "ftp";
            public const string sftp = "sftp";
            public const string ftps = "ftps";
        }
    }
}
