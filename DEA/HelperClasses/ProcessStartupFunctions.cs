using FtpFunctions;
using GraphHelper;
using ProcessStatusMessageSetter;
using UserConfigReader;
using WriteLog;

namespace ProcessSartupFunctions
{
    internal class ProcessStartupFunctionsClass
    {
        readonly UserConfigReaderClass.CustomerDetailsObject jsonDataObject = UserConfigReaderClass.ReadUserDotConfig<UserConfigReaderClass.CustomerDetailsObject>();

        public static async Task StartupProcess()
        {
            int ftpReturnCode = 0;
            int emailReturnCode =0;

            ProcessStartupFunctionsClass jsonData = new();
            UserConfigReaderClass.Customerdetail[] jsonCustomerData = jsonData.jsonDataObject.CustomerDetails;

            IEnumerable<UserConfigReaderClass.Customerdetail> ftpClients = jsonCustomerData.Where(ftpc => ftpc.FileDeliveryMethod!.ToLower() == "ftp");
            IEnumerable<UserConfigReaderClass.Customerdetail> emailClients = jsonCustomerData.Where(emailc => emailc.FileDeliveryMethod!.ToLower() == "email");

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

        private static async Task<int> StartFtpDownload(IEnumerable<UserConfigReaderClass.Customerdetail> ftpClients)
        {
            int ftpResult = 0;

            foreach (var ftpClient in ftpClients)
            {
                if ((ftpClient.FtpDetails!.FtpType!.ToLower() == "ftp" || ftpClient.FtpDetails!.FtpType!.ToLower() == "ftps") && ftpClient.CustomerStatus == 1)
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

        private static async Task<int> StartEmailDownload(IEnumerable<UserConfigReaderClass.Customerdetail> emailClients)
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
    }
}
