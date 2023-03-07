using GraphHelper;
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
int result = 0;

UserConfigReaderClass.CustomerDetailsObject jsonData = UserConfigReaderClass.ReadAppDotConfig<UserConfigReaderClass.CustomerDetailsObject>();
IEnumerable<UserConfigReaderClass.Customerdetail> ftpClients = jsonData.CustomerDetails!.Where(ftpc => ftpc.FileDeliveryMethod!.ToLower() == "ftp");
int ftpClientCount = ftpClients.Count();
IEnumerable<UserConfigReaderClass.Customerdetail> emailClients = jsonData.CustomerDetails!.Where(emailc => emailc.FileDeliveryMethod!.ToLower() == "email");
int emailClientCount = emailClients.Count();

if (ftpClientCount > 0)
{
    foreach (var ftpClient in ftpClients)
    {
        if (ftpClient.FtpDetails!.FtpType!.ToLower() == "ftp" || ftpClient.FtpDetails!.FtpType!.ToLower() == "ftps")
        {
            await FtpFunctionsClass.GetFtpFiles(ftpClient.id);
        }
        /*else
        {
            // Awating to be implimented. Will be added when needed.
            SftpFunctionsClass.GetSftpFiles(ftpClient.id);
        }*/
        ftpLoopCount++;
    }
}

if (emailClientCount > 0)
{
    foreach (var emailClient in emailClients)
    {
        result = await GraphHelperClass.InitializGetAttachment(emailClient.id);
        emlLoopCount++;
    }
}

switch (result)
{
    case 1:
        WriteLogClass.WriteToLog(1, "Process completed successfully ....", 1);
        break;
    case 2:
        WriteLogClass.WriteToLog(1, "Process completed with issues ....", 1);
        break;
    default:
        WriteLogClass.WriteToLog(0, "Process terminated due to errors ....", 1);
        break;
}
