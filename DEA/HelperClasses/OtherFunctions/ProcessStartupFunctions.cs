using DEA.Next.HelperClasses.OtherFunctions;
using DEA.Next.Interfaces;
using FtpFunctions;
using GraphHelper;
using ProcessStatusMessageSetter;
using WriteLog;

namespace ProcessSartupFunctions
{
    internal class ProcessStartupFunctionsClass
    {
        private readonly IUserConfigRepository _configRepository;

        public ProcessStartupFunctionsClass(IUserConfigRepository configRepository)
        {
            _configRepository = configRepository;
        }

        public async Task StartupProcess()
        {
            foreach (var client in await _configRepository.GetAllCustomerDetails())
            {
                switch (client.FileDeliveryMethod)
                {
                    case MagicWords.ftp:
                        WriteLastStatusMessage(0, await FtpFunctionsClass.GetFtpFiles(client.Id));
                        break;
                    case MagicWords.email:
                        WriteLastStatusMessage(await GraphHelperClass.InitializeGetAttachment(_configRepository ,client.Id), 0);
                        break;
                }
            }
        }

        private static void WriteLastStatusMessage(int emailResultStatus, int ftpResultStatus)
        {
            WriteLogClass.WriteToLog(
                ProcessStatusMessageSetterClass.SetMessageTypeMain(emailResultStatus, ftpResultStatus),
                $"{ProcessStatusMessageSetterClass.SetProcessStatusMain(emailResultStatus, ftpResultStatus)}\n", 1);
        }
    }
}
