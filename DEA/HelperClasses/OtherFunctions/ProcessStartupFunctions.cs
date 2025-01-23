using DEA.Next.FTP.FtpFileRelatedFunctions;
using DEA.Next.Graph.GraphClientRelatedFunctions;
using DEA.Next.HelperClasses.ConfigFileFunctions;
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
                    WriteLastStatusMessage(1, await FtpFunctionsClass.GetFtpFiles(client.Id));
                    break;
                case MagicWords.Email:
                    if (!client.Status) continue;
                    WriteLastStatusMessage(await GraphHelper.InitializeGetAttachment(client.Id), 1);
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