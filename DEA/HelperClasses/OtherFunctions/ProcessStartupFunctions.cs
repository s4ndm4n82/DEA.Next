﻿using DEA.Next.HelperClasses.OtherFunctions;
using FtpFunctions;
using GraphHelper;
using ProcessStatusMessageSetter;
using UserConfigSetterClass;
using WriteLog;

namespace ProcessSartupFunctions
{
    internal class ProcessStartupFunctionsClass
    {
        readonly UserConfigSetter.CustomerDetailsObject jsonDataObject;

        public ProcessStartupFunctionsClass()
        {
            jsonDataObject = UserConfigSetter.ReadUserDotConfigAsync<UserConfigSetter.CustomerDetailsObject>().Result;
        }

        public static async Task StartupProcess()
        {
            int ftpReturnCode = 0;
            int emailReturnCode = 0;

            ProcessStartupFunctionsClass jsonData = new();
            UserConfigSetter.Customerdetail[] jsonCustomerData = jsonData.jsonDataObject.CustomerDetails;

            IEnumerable<UserConfigSetter.Customerdetail> ftpClients = jsonCustomerData.Where(ftpc => ftpc.FileDeliveryMethod!.ToLower() == MagicWords.ftp);
            IEnumerable<UserConfigSetter.Customerdetail> emailClients = jsonCustomerData.Where(emailc => emailc.FileDeliveryMethod!.ToLower() == MagicWords.email);

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

        private static async Task<int> StartFtpDownload(IEnumerable<UserConfigSetter.Customerdetail> ftpClients)
        {
            int ftpResult = 0;

            foreach (var ftpClient in ftpClients)
            {
                if (ftpClient.CustomerStatus == 1)
                {
                    ftpResult = await FtpFunctionsClass.GetFtpFiles(ftpClient.Id);
                }
            }

            return ftpResult;
        }

        private static async Task<int> StartEmailDownload(IEnumerable<UserConfigSetter.Customerdetail> emailClients)
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
