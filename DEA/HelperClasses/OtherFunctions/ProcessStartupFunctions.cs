using DEA.Next.Data;
using DEA.Next.Entities;
using DEA.Next.HelperClasses.OtherFunctions;
using FtpFunctions;
using GraphHelper;
using ProcessStatusMessageSetter;
using UserConfigSetterClass;
using WriteLog;

namespace ProcessSartupFunctions
{
    internal class ProcessStartupFunctionsClass
    {
        private readonly DataContext _context;

        public ProcessStartupFunctionsClass(DataContext context)
        {
            _context = context;
        }

        public async Task StartupProcess()
        {
            var ftpReturnCode = 0;
            var emailReturnCode = 0;
            
            var ftpClients = _context.CustomerDetails
                .Where(f => f.FileDeliveryMethod
                    .Equals(MagicWords.ftp, StringComparison.OrdinalIgnoreCase)).ToList();

            var emailClients = _context.CustomerDetails
                .Where(e => e.FileDeliveryMethod
                    .Equals(MagicWords.email, StringComparison.OrdinalIgnoreCase)).ToList();

            if (ftpClients.Count != 0)
            {
                ftpReturnCode = await StartFtpDownload(ftpClients);
            }
            
            if (emailClients.Count != 0)
            {
                emailReturnCode = await StartEmailDownload(emailClients);
            }

            WriteLastStatusMessage(emailReturnCode, ftpReturnCode);
        }

        private static async Task<int> StartFtpDownload(List<CustomerDetails> ftpClients)
        {
            var ftpResult = 0;

            foreach (var ftpClient in ftpClients.Where(ftpClient => ftpClient.Status))
            {
                ftpResult = await FtpFunctionsClass.GetFtpFiles(ftpClient.Id);
            }

            return ftpResult;
        }

        private static async Task<int> StartEmailDownload(List<CustomerDetails> emailClients)
        {
            var emailResult = 0;

            foreach (var emailClient in emailClients.Where(emailClient => emailClient.Status))
            {
                emailResult = await GraphHelperClass.InitializGetAttachment(emailClient.Id);
            }
            return emailResult;
        }

        private static void WriteLastStatusMessage(int emailResultStatus, int ftpResultStatus)
        {
            WriteLogClass.WriteToLog(ProcessStatusMessageSetterClass.SetMessageTypeMain(emailResultStatus, ftpResultStatus), $"{ProcessStatusMessageSetterClass.SetProcessStatusMain(emailResultStatus, ftpResultStatus)}\n", 1);
        }
    }
}
