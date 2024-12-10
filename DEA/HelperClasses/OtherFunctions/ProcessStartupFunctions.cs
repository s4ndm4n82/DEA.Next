using DEA.Next.HelperClasses.ConfigFileFunctions;
using DEA.Next.HelperClasses.OtherFunctions;
using FtpFunctions;
using GraphHelper;
using ProcessStatusMessageSetter;
using WriteLog;

namespace ProcessSartupFunctions;

internal class ProcessStartupFunctionsClass
{

    public async Task StartupProcess()
    {
        foreach (var client in await UserConfigRetriever.RetrieveAllUserConfig())
        {
            switch (client.FileDeliveryMethod)
            {
                case MagicWords.Ftp:
                    WriteLastStatusMessage(0, await FtpFunctionsClass.GetFtpFiles(client.Id));
                    break;
                case MagicWords.Email:
                    WriteLastStatusMessage(await GraphHelperClass.InitializeGetAttachment(client.Id), 0);
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