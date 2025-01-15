using DEA.Next.FTP.FtpFileRelatedFunctions;
using DEA.Next.HelperClasses.ConfigFileFunctions;
using GraphHelper;
using ProcessStatusMessageSetter;
using WriteLog;

namespace DEA.Next.HelperClasses.OtherFunctions;

internal class ProcessStartupFunctionsClass
{
    public static async Task StartupProcess()
    {
        foreach (var client in await UserConfigRetriever.RetrieveAllUserConfig())
            switch (client.FileDeliveryMethod.ToLower())
            {
                case MagicWords.Ftp:
                    if (!client.Status) continue;
                    WriteLastStatusMessage(0, await FtpFunctionsClass.GetFtpFiles(client.Id));
                    break;
                case MagicWords.Email:
                    if (!client.Status) continue;
                    WriteLastStatusMessage(await GraphHelperClass.InitializeGetAttachment(client.Id), 0);
                    break;
            }
    }

    private static void WriteLastStatusMessage(int emailResultStatus, int ftpResultStatus)
    {
        WriteLogClass.WriteToLog(
            ProcessStatusMessageSetterClass.SetMessageTypeMain(emailResultStatus, ftpResultStatus),
            $"{ProcessStatusMessageSetterClass.SetProcessStatusMain(emailResultStatus, ftpResultStatus)}\n", 1);
    }
}