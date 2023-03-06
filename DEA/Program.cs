using DEA;
using WriteLog;
using FolderCleaner;
using FtpFunctions;
using UserConfigReader;
using FolderFunctions;

// DEA.Next
// ~~~~~~~~
// TODO 1: Impliment error file handler.

// Aplication title just for fun.

WriteLogClass.WriteToLog(1, "Starting download process ....", 1);
FolderFunctionsClass.CheckFolders(null!);

int ftpLoopCount = 0;
int emlLoopCount = 0;
bool result = false;

UserConfigReaderClass.CustomerDetailsObject jsonData = UserConfigReaderClass.ReadAppDotConfig<UserConfigReaderClass.CustomerDetailsObject>();
IEnumerable<UserConfigReaderClass.Customerdetail> ftpClients = jsonData.CustomerDetails!.Where(ftpc => ftpc.FileDeliveryMethod!.ToLower() == "ftp");
IEnumerable<UserConfigReaderClass.Customerdetail> emailClients = jsonData.CustomerDetails!.Where(emailc => emailc.FileDeliveryMethod!.ToLower() == "email");

if (ftpClients.Any())
{
    foreach (var ftpClient in ftpClients)
    {
        if (ftpClient.FtpDetails!.FtpType!.ToLower() == "ftp" || ftpClient.FtpDetails!.FtpType!.ToLower() == "ftps")
        {
            result = await FtpFunctionsClass.GetFtpFiles(ftpClient.id);
        }
        /*else
        {
            // Awating to be implimented. Will be added when needed.
            SftpFunctionsClass.GetSftpFiles(ftpClient.id);
        }*/
        ftpLoopCount++;
    }
}

if (emailClients.Any())
{
    foreach (var emailClient in emailClients)
    {
        await GraphHelper.InitializGetAttachment(emailClient.id);
        emlLoopCount++;
    }
}

if (ftpLoopCount == ftpClients.Count() && result)
{
    WriteLogClass.WriteToLog(1, "Process completed successfully ... ", 1);
}
else
{
    WriteLogClass.WriteToLog(1, "Process exited with errors ... ", 1);
}
